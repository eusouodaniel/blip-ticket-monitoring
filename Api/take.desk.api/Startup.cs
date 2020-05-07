using Lime.Messaging.Resources;
using Lime.Protocol.Serialization;
using Lime.Protocol.Serialization.Newtonsoft;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using Serilog.Exceptions;
using StructureMap;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using take.desk.api.Middlewares;
using take.desk.business.HostedService;
using take.desk.core.Models.Settings;
using take.desk.injections;
using Take.Blip.Client;
using Take.Blip.Client.Extensions;


namespace take.desk.api
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		private readonly IHostingEnvironment _hostingEnvironment;

		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			Configuration = configuration;
			_hostingEnvironment = env;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// Insert AppSettings
			var appSettings = Configuration.Get<AppSettings>();
			services.AddSingleton(appSettings);

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			services.AddCors();

            var documentResolver = new DocumentTypeResolver().WithBlipDocuments();
            services.AddSingleton<IEnvelopeSerializer>(new EnvelopeSerializer(documentResolver));

			#region Dependency injection

			// Serilog
			var loggerConfiguration = new LoggerConfiguration()
					 .ReadFrom.Configuration(Configuration)
					 .Enrich.WithMachineName()
					 .Enrich.WithProperty("Application", "Take.Api.Desk")
					 .Enrich.WithExceptionDetails()
					 .CreateLogger();
		
			services.AddSingleton<ILogger>(loggerConfiguration);

			services.AddHostedService<CheckQueueStatusBll>();

			// Initialize Blip CLients
			var container = new Container();
			foreach (var cfg in appSettings.BotConfigurations.Bots)
			{
				var blipClient = new BlipClientBuilder()
						   .UsingHostName("msging.net")
						   .UsingPort(55321)
						   .UsingAccessKey(cfg.BotId, cfg.BotAccessKey)
						   .UsingRoutingRule(RoutingRule.Instance)
						   .UsingInstance(cfg.BotId)
						   .WithChannelCount(1)
						   .Build();
				services.AddBlipClientToContainer(container, blipClient, cfg.BotId);
			}
			services.AddSingleton<IContainer>(container);

			services.AddMvc().AddJsonOptions(options =>
			{
				foreach (var settingsConverter in JsonNetSerializer.Settings.Converters)
				{
					options.SerializerSettings.Converters.Add(settingsConverter);
				}
			});

			// Injections Layer
			services.AddInjectionsBll();

			#endregion

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info
				{
					Title = "Take desk",
					Version = "v1",
					Description = "Api"
				});
			});
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseMiddleware<ErrorHandlingMiddleware>();

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Take desk v1");
				c.DocumentTitle = "Take desk Documentation";
				c.DocExpansion(DocExpansion.None);
				c.RoutePrefix = string.Empty;
			});

			app.UseCors(c =>
			{
				c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
			});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseAuthentication();
			app.UseSwagger();
			app.UseHttpsRedirection();
		}
	}
}
