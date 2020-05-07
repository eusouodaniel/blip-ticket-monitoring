using Newtonsoft.Json;

namespace take.desk.core.Models.Settings
{
	public class DBSettings
	{
		[JsonProperty("ConnectionStrings")]
		public ConnectionStrings ConnectionStrings { get; set; }
	}
}
