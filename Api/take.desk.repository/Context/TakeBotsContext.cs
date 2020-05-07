using Microsoft.EntityFrameworkCore;
using take.desk.core.Models.Settings;

namespace take.desk.repository.Context
{
	class TakeBotsContext : DbContext
	{
		private readonly DBSettings _dBSettings;
		public TakeBotsContext(AppSettings appSettings)
		{
			_dBSettings = appSettings.DBSettings;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer(_dBSettings.ConnectionStrings.TakeBotsContext);
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}