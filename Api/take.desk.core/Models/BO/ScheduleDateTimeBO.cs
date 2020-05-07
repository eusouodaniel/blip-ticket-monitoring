using System;

namespace take.desk.core.Models.BO
{
	public class ScheduleDateTimeBO
	{
		public DateTime? StartDateTime { get; set; }
		public DateTime? StopDateTime { get; set; }
		public bool NoAttendence { get; set; }
	}
}
