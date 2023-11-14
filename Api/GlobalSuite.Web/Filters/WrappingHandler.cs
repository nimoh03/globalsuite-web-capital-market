using GlobalSuite.Web.Responses;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using GlobalSuite.Core.Helpers;
using Serilog;
using Serilog.Core;


namespace GlobalSuite.Web.Filters
{
    public class WrappingHandler : DelegatingHandler
    {
        private ILogger _logger=Log.ForContext<WrappingHandler>();

        public WrappingHandler()
        {
        }

        protected override async Task<HttpResponseMessage>
             SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (IsSwagger(request))
                return await base.SendAsync(request, cancellationToken);

            var response = await base.SendAsync(request, cancellationToken);
            return BuildApiResponse(request, response);
        }

        private  HttpResponseMessage
               BuildApiResponse(HttpRequestMessage request, HttpResponseMessage response)
        {

            dynamic content = null;
            object data = null;
            string errorMessage = null;
            ApiError apiError = null;

            var code = (int)response.StatusCode;
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                apiError = new ApiError(response.ReasonPhrase);
                data = new ApiResponse(code, ResponseMessageEnum.Conflict.GetDescription(),
                    null, apiError);
            }
            if (response.TryGetContentValue(out content) && !response.IsSuccessStatusCode)
            {
                //handle exception
                if (content is HttpError error)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        apiError = new ApiError("The specified URI does not exist.Please verify and try again.");
                    else if (response.StatusCode == HttpStatusCode.NoContent)
                        apiError = new ApiError("The specified URI does not contain any content.");
                    else
                    {
                        errorMessage = error.Message;
                        

#if DEBUG
                        errorMessage = string.Concat
                            (errorMessage, error.ExceptionMessage, error.StackTrace);
#endif

                        _logger.Error(errorMessage);
                        apiError = new ApiError(errorMessage);
                    }

                    data = new ApiResponse((int)code, ResponseMessageEnum.Failure.GetDescription(),
                        null, apiError);

                }
                else
                    data = content;
            }
            else
            {
                if (response.TryGetContentValue(out content))
                {
                    Type type = content?.GetType();

                    if (type.Name.Equals("ApiResponse"))
                    {
                        response.StatusCode = Enum.Parse(typeof(HttpStatusCode),
                            content.StatusCode.ToString());
                        data = content;
                    }
                    else if (type.Name.Equals("SwaggerDocument"))
                        data = content;
                    else
                        data = new ApiResponse(code, ResponseMessageEnum.Success.GetDescription(), content);
                }
                else
                {
                    if (response.IsSuccessStatusCode)
                        data = new ApiResponse((int)response.StatusCode,
                            ResponseMessageEnum.Success.GetDescription());
                }
            }

            var newResponse = request.CreateResponse(response.StatusCode, data);

            foreach (var header in response.Headers)
            {
                newResponse.Headers.Add(header.Key, header.Value);
            }

            return newResponse;
            }

            private bool IsSwagger(HttpRequestMessage request)
            {
                return request.RequestUri.PathAndQuery.StartsWith("/swagger") ||
                       request.RequestUri.PathAndQuery.Contains("/reports");
            }
        }
    }