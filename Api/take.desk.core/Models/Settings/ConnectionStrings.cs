using Newtonsoft.Json;

namespace take.desk.core.Models.Settings
{
	public class ConnectionStrings
	{
		[JsonProperty("TakeBotsContext")]
		public string TakeBotsContext { get; set; }
	}
}
