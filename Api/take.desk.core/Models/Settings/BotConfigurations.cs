using System.Collections.Generic;

namespace take.desk.core.Models.Settings
{
	public class BotConfigurations
	{
		public List<Bots> Bots { get; set; }	
		public List<string> BotsWithQueueCheck { get; set; }

		public BotConfigurations()
		{
			Bots = new List<Bots>();
			BotsWithQueueCheck = new List<string>();
		}
	}
}
