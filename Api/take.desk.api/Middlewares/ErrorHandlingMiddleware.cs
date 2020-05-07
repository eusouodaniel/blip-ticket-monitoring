using System;
using System.Net;
using System.Threading.Tasks;
using Lime.Protocol;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace take.desk.api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private const string BLIP_BOT_HEADER = "X-Blip-Bot";

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invoke Method, to validate requisition errors
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var botId = "Bot Id";
            if (context.Request.Headers.ContainsKey(BLIP_BOT_HEADER)) {
                botId = context.Request.Headers[BLIP_BOT_HEADER];
            }

            _logger.Error(exception, "[{@bot}] Error: {@exception}", botId, exception.Message);

            // Thrown whenever a RestEase call returns with a non-success HttpStatusCode
            if (exception is RestEase.ApiException apiException)
            {
                context.Response.StatusCode = (int)apiException.StatusCode;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            context.Response.ContentType = MediaType.ApplicationJson;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(exception.Message));
        }
    }
}
