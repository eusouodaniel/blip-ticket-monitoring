using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using take.desk.core.Contract.Bll.Blip;
using take.desk.core.Models;
using take.desk.core.Models.Settings;
using Takenet.Iris.Messaging.Resources;

namespace take.desk.business.HostedService
{
	public class CheckQueueStatusBll : BackgroundService
	{
		private readonly ITicketBll _ticketBll;
		private readonly List<string> _botsWithQueueCheck;
		private readonly ILogger _logger;
		private readonly int _minutesHostedService = 2;
		private readonly int _minutesToCloseTicket = 30;
		private readonly string _ticketTag = "Fila Cheia";
		private readonly int UTC_BRASILIA_TIME = -3;

		public CheckQueueStatusBll(ITicketBll ticketBll, ILogger logger, AppSettings appSettings)
		{
			_ticketBll = ticketBll;
			_botsWithQueueCheck = appSettings.BotConfigurations.BotsWithQueueCheck;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				Parallel.ForEach(_botsWithQueueCheck, async bot =>
				{
					var tickets = await _ticketBll.GetWaitingTicketsAsync(bot);
					if (tickets.Success)
						Check(tickets.Content, bot);
				});

				await Delay();
			}
		}

		private void Check(Dictionary<string, List<Ticket>> dictionayTickets, string botIdentity)
		{
			var response = new BaseResponse<bool>();
			try
			{
				Parallel.ForEach(dictionayTickets, (dctkt, dctState, dctIndex) =>
				{
					Parallel.ForEach(dctkt.Value, (tkt, state, index) =>
					{
						var interval = DateTime.UtcNow.AddHours(UTC_BRASILIA_TIME) - tkt.StorageDate.AddHours(UTC_BRASILIA_TIME);
						if (interval.TotalMinutes >= _minutesToCloseTicket)
						{
							_ticketBll.AddTagToTicketAsync(botIdentity, tkt.Id, _ticketTag);
							_ticketBll.CloseTicketByClientAsync(botIdentity, tkt.Id);
						}
					});					

				});
			}
			catch (Exception ex)
			{
				if (response.Exception != null)
					_logger.Error(response.Exception, response.Message);
				else
					_logger.Error(ex, response.Message);
			}
		}

		private async Task Delay()
		{
			await Task.Delay(TimeSpan.FromMinutes(_minutesHostedService));
		}


	}

}
