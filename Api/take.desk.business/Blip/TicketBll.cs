using Lime.Messaging.Contents;
using Lime.Protocol;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using take.desk.core.Contract.Bll.Blip;
using take.desk.core.Models;
using Take.Blip.Client;
using Takenet.Iris.Messaging.Resources;

namespace take.desk.business.Blip
{
	public class TicketBll : ITicketBll
	{
		private readonly IContainer _blipContainer;
		public TicketBll(IContainer container)
		{
			_blipContainer = container;
		}

		public async Task<BaseResponse<Dictionary<string, List<Ticket>>>> GetWaitingTicketsAsync(string blipClientId)
		{
			var response = new BaseResponse<Dictionary<string, List<Ticket>>> { Content = new Dictionary<string, List<Ticket>>() };
			try
			{
				var command = new Command
				{
					Id = Guid.NewGuid().ToString(),
					To = "postmaster@desk.msging.net",
					Method = CommandMethod.Get,
					Uri = new LimeUri($"/tickets?$filter=status%20eq%20'waiting'&$take=999999"),
				};

				var received = await _blipContainer.GetInstance<IBlipClient>(blipClientId).ProcessCommandAsync(command, CancellationToken.None);

				if (received.Status == CommandStatus.Success)
				{
					var collection = (DocumentCollection)received.Resource;

					if (collection.Items.Length > 0)
					{
						foreach (var item in collection.Items)
						{
							if (response.Content.ContainsKey(((Ticket)item).Team))
								response.Content[((Ticket)item).Team].Add((Ticket)item);
							else
								response.Content.Add(((Ticket)item).Team, new List<Ticket>() { ((Ticket)item) });
						}

						response.Content = response.Content.ToDictionary(d => d.Key, d => d.Value.OrderBy(v => v.StorageDate).ToList());
					}
				}
			}
			catch (Exception ex)
			{
				response.Message = "Error while fetching tickets with status (waiting)";
				response.Success = false;
				response.Exception = ex;
			}

			return response;
		}

		public async Task<BaseResponse<bool>> AddTagToTicketAsync(string blipClientId, string ticketId, string tag)
		{
			var response = new BaseResponse<bool>();
			var tags = new List<string>() { tag };
			try
			{
				var addTagTicket = new Command
				{
					Id = Guid.NewGuid().ToString(),
					To = "postmaster@desk.msging.net",
					Method = CommandMethod.Set,
					Uri = new LimeUri($"/tickets/{ticketId}/change-tags"),
					Resource = new Ticket()
					{
						Id = ticketId,
						Tags = tags.ToArray()
					}
				};

				await _blipContainer.GetInstance<IBlipClient>(blipClientId).ProcessCommandAsync(addTagTicket, CancellationToken.None);
			}
			catch (Exception ex)
			{
				response.Message = "Error while add tag to ticket";
				response.Success = false;
				response.Exception = ex;
			}

			return response;
		}

		public async Task<BaseResponse<bool>> CloseTicketByClientAsync(string blipClientId, string ticketId)
		{
			var response = new BaseResponse<bool>();
			try
			{
				var closeTicket = new Command
				{
					Id = Guid.NewGuid().ToString(),
					To = "postmaster@desk.msging.net",
					Method = CommandMethod.Set,
					Uri = new LimeUri($"/tickets/change-status"),
					Resource = new Ticket()
					{
						Id = ticketId,
						Status = TicketStatusEnum.ClosedClient
					}
				};

				await _blipContainer.GetInstance<IBlipClient>(blipClientId).ProcessCommandAsync(closeTicket, CancellationToken.None);
			}
			catch (Exception ex)
			{
				response.Message = "Error closing ticket";
				response.Success = false;
				response.Exception = ex;
			}

			return response;
		}

		public async Task<BaseResponse<bool>> QueuePositionAsync(string blipClientId, Ticket tkt, long index)
		{
			var response = new BaseResponse<bool>();
			try
			{
				var message = new Message
				{
					Id = Guid.NewGuid().ToString(),
					To = tkt.CustomerIdentity.ToNode(),
					Content = new PlainText { Text = $"Sua posição na fila é {index + 1}" }
				};

				await _blipContainer.GetInstance<IBlipClient>(blipClientId).SendMessageAsync(message, CancellationToken.None);
			}
			catch (Exception ex)
			{
				response.Message = "Error to send queue position message";
				response.Success = false;
				response.Exception = ex;
			}

			return response;
		}
	}
}
