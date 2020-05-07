using System;
using take.desk.core.Models;

namespace take.desk.core.Extensions
{
	public static class ExceptionExtensions
	{
		public static BaseResponse<T> PopulateResponseObject<T>(this Exception ex, BaseResponse<T> response, string kind, string method)
		{
			response.Success = false;
			response.Message = ex.Message;
			response.Where = $"Failed on method {method} in class {kind}";
			return response;
		}
	}
}
