using System.Collections.Generic;
using System.Threading.Tasks;
using take.desk.core.Models;
using Takenet.Iris.Messaging.Resources;

namespace take.desk.core.Contract.Bll.Blip
{
	public interface ITicketBll
	{
		Task<BaseResponse<Dictionary<string, List<Ticket>>>> GetWaitingTicketsAsync(string blipClientId);
		Task<BaseResponse<bool>> AddTagToTicketAsync(string blipClientId, string ticketId, string tag);
		Task<BaseResponse<bool>> CloseTicketByClientAsync(string blipClientId, string ticketId);
		Task<BaseResponse<bool>> QueuePositionAsync(string blipClientId, Ticket tkt, long index);
	}
}
