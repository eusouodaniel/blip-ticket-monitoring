using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Net;

namespace take.desk.core.Models
{
    public class BaseResponse<T>
    {
        public BaseResponse()
        {
            Message = string.Empty;
            Success = true;
            ClientHttpStatus = default;
        }

        public HttpStatusCode ClientHttpStatus { get; set; }
        public T Content { get; set; }
        public string Message { get; set; }
        public string Where { get; set; }
        public Exception Exception { get; set; }
        public bool Success { get; set; }

        public IActionResult CheckAndReturn(ILogger logger)
        {
            if (Success)
            {
                return new ObjectResult(this)
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            else
            {
                logger.Error(Exception, Message);

                return new ObjectResult(default)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}

