using Microsoft.Extensions.DependencyInjection;
using System;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.Context;
using Take.Blip.Client.Extensions.Resource;
using StructureMap;
using Take.Blip.Client.Extensions.Contacts;
using Take.Blip.Client.Extensions.Bucket;

namespace take.desk.injections
{
	public static class InjectionsBlip
	{
		public static IServiceProvider AddBlipClientToContainer(this IServiceCollection services, Container container, IBlipClient blipClient, string botIdentifier)
		{
			
			container.Configure(config =>
			{
				config.For<IBlipClient>().Add(blipClient).Named(botIdentifier);
				config.Populate(services);
			});
			return container.GetInstance<IServiceProvider>();
		}
	}
}
