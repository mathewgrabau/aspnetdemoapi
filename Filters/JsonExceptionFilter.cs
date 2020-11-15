using DemoApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;

namespace DemoApi.Filters
{
	public class JsonExceptionFilter : IExceptionFilter
	{
		private readonly IWebHostEnvironment _environment;

		public JsonExceptionFilter(IWebHostEnvironment environment)
		{
			_environment = environment;
		}

		public void OnException(ExceptionContext context)
		{
			var error = new ApiError();
			if (_environment.IsDevelopment())
			{
				error.Message = context.Exception.Message;
				error.Detail = context.Exception.StackTrace;
			}
			else
			{
				error.Message = "A server error occurred";
				error.Detail = context.Exception.Message;
			}


			context.Result = new ObjectResult(error)
			{
				StatusCode = 500
			};
		}
	}
}