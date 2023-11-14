using GlobalSuite.Core.Exceptions;
using GlobalSuite.Web.Responses;
using System;
using System.Net.Http;
using System.Web.Http.Filters;
using GlobalSuite.Core.Helpers;
using System.Net;
using Serilog;


namespace GlobalSuite.Web.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger=Log.ForContext<ApiExceptionFilter>();

        public ApiExceptionFilter()
        {
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            _logger.Error(context.Exception.Message, context.Exception);

            ApiError apiError = null;
            ApiResponse apiResponse = null;
            var code = 0;

            switch (context.Exception)
            {
                case ApiException exception:
                {
                    apiError = new ApiError(exception.Message)
                    {
                        ValidationErrors = exception.Errors,
                        ReferenceErrorCode = exception.ReferenceErrorCode,
                        ReferenceDocumentLink = exception.ReferenceDocumentLink
                    };
                    code = exception.StatusCode;
                    break;
                }
                case UnauthorizedAccessException _:
                    apiError = new ApiError("Unauthorized Access");
                    code = (int)HttpStatusCode.Unauthorized;
                    break;
                default:
                {
                     var msg = "An unhandled error occurred.";
                     apiError = new ApiError(msg);
#if !DEBUG
 msg += " It has been logged."+
                   " Contact the web administrator. ErrorId: ${apiError.CorrelationId}";
            string stack = null;
#else
                     msg = context.Exception.GetBaseException().Message;
                    var stack = context.Exception.StackTrace;
#endif
                    apiError = new ApiError(msg)
                    {
                        Details = stack
                    };
                    
                    code = (int)HttpStatusCode.InternalServerError;
                    break;
                }
            }

            apiResponse = new ApiResponse
                          (code, ResponseMessageEnum.Exception.GetDescription(), null, apiError);

            var c = (HttpStatusCode)code;

            context.Response = context.Request.CreateResponse(c, apiResponse);
        }
    }
}
