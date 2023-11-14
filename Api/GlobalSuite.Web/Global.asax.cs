using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Serilog;


namespace GlobalSuite.Web
{
    public class WebApiApplication : HttpApplication
    {
        private ILogger _logger = Log.ForContext<WebApiApplication>();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(SwaggerConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            LoggerConfig.Configure();
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }
        protected void Application_BeginRequest()
        {
// #if !DEBUG
//             // SECURE: Ensure any request is returned over SSL in production
//             if (!Request.IsLocal && !Context.Request.IsSecureConnection) {
//                 var redirect = Context.Request.Url.ToString().ToLower().Replace("http:", "https:");
//                 Response.Redirect(redirect);
//             }
// #endif
        }
        
        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            _logger.Error(ex.Message, ex);
        } 
    }
}
