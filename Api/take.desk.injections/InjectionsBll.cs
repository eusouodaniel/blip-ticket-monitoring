using Microsoft.Extensions.DependencyInjection;
using take.desk.business;
using take.desk.business.Blip;
using take.desk.core.Contract.Bll;
using take.desk.core.Contract.Bll.Blip;
using Take.Blip.Client;

namespace take.desk.injections
{
	public static class InjectionsBll
	{
		public static IServiceCollection AddInjectionsBll(this IServiceCollection services)
		{
			services.AddSingleton<ITicketBll, TicketBll>();
			return services;
		}
	}
}
