using GlobalSuite.Web.Filters;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using GlobalSuite.Web.Providers;
using Ninject.Web.WebApi;

namespace GlobalSuite.Web
{

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config )
        {
            config.SetCorsPolicyProviderFactory(new CorsPolicyFactory());
            config.EnableCors();
            // Web API configuration and services
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Filters.Add(new SessionFilter());
            config.Filters.Add(new ApiExceptionFilter());
            config.MessageHandlers.Add(new WrappingHandler());
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
        }
    }
}
