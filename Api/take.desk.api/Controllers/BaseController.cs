using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace take.desk.api.Controllers
{
	public class BaseController : Controller
	{
		public string BotKey;
		public string BotUser;

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var req = context.HttpContext.Request;
			if (req.Headers.ContainsKey("X-Blip-Bot"))
			{
				BotKey = req.Headers["X-Blip-Bot"].ToString().Split("@")[0];
				if (req.Headers.ContainsKey("X-Blip-User"))
					BotUser = req.Headers["X-Blip-User"];
			}
			else			
				context.Result = Unauthorized();
			
			base.OnActionExecuting(context);
		}
	}
}
