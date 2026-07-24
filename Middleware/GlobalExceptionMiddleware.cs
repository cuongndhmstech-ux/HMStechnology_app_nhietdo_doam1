using HMS_Temp_Humdity_ApiManager.BaseException;
using HMS_Temp_Humdity_ApiManager.DTO;

namespace HMS_Temp_Humdity_ApiManager.Middleware
{
	public class GlobalExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger;

		public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				//_logger.LogError(ex, "Đã xảy ra lỗi. TraceId={TraceId}, Path={Path}",
				//	context.TraceIdentifier, context.Request.Path);

				await HandleExceptionAsync(context, ex);
			}
		}

		private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";

			var (statusCode, message, errorData) = MapException(exception, context.TraceIdentifier);

			context.Response.StatusCode = statusCode;

			await context.Response.WriteAsJsonAsync(new ApiResponse<object>
			{
				Success = false,
				Message = message,
				Data = errorData
			});
		}

		private static (int StatusCode, string Message, object? Data) MapException(Exception exception, string traceId)
		{
			return exception switch
			{
				DuplicateResourceException dupEx =>
					(StatusCodes.Status409Conflict,
					 dupEx.Message,
					 dupEx.Errors),

				ResourceNotFoundException notFoundEx =>
					(StatusCodes.Status404NotFound,
					 notFoundEx.Message,
					 null),

				BadRequestException badReqEx =>
					(StatusCodes.Status400BadRequest,
					 badReqEx.Message,
					 null),

				UnauthorizedAccessException unAuEx =>
					(StatusCodes.Status401Unauthorized,
					unAuEx.Message,
					 null),

				ArgumentNullException argNullEx =>
					(StatusCodes.Status400BadRequest,
					 $"Thiếu tham số bắt buộc: {argNullEx.ParamName}",
					 null),

				ArgumentException argEx =>
					(StatusCodes.Status400BadRequest,
					 argEx.Message,
					 null),

				TimeoutException =>
					(StatusCodes.Status504GatewayTimeout,
					 "Yêu cầu xử lý quá thời gian cho phép",
					 null),

				_ =>
					(StatusCodes.Status500InternalServerError,
					 "Đã xảy ra lỗi hệ thống, vui lòng thử lại sau",
					 new { traceId }) // để user report kèm traceId khi gặp lỗi 500
			};
		}
	}
}