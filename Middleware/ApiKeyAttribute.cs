using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HMS_Temp_Humdity_ApiManager.Middleware
{
	public class ApiKeyAttribute : Attribute, IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(
			ActionExecutingContext context,
			ActionExecutionDelegate next)
		{
			var config = context.HttpContext.RequestServices
				.GetRequiredService<IConfiguration>();

			var apiKey = config["InternalApi:ApiKey"];

			if (!context.HttpContext.Request.Headers.TryGetValue(
					"X-Api-Key",
					out var requestKey))
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			if (requestKey != apiKey)
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			await next();
		}
	}
}
