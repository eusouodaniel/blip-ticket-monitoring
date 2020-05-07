using Microsoft.AspNetCore.Mvc;

namespace take.desk.api.Controllers
{
	[Route("[controller]")]
	public class HealthController : Controller
	{
		[HttpGet("Ping")]
		public IActionResult Ping()
		{
			return Json("Pong");
		}
		
	}
}
