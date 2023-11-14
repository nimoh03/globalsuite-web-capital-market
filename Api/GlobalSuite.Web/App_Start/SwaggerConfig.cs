using System;
using System.Configuration;
using System.IO;
using System.Web.Http;
using Swashbuckle.Application;

//[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]/

namespace GlobalSuite.Web
{
    public class SwaggerConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            var isSwagger = ConfigurationManager.AppSettings["IsSwaggerEnabled"];
            var isSwaggerEnabled =isSwagger == null || bool.Parse(isSwagger);
            if(!isSwaggerEnabled) return;
           configuration
                .EnableSwagger(c =>
                    {
                        // By default, the service root url is inferred from the request used to access the docs.
                        // However, there may be situations (e.g. proxy and load-balanced environments) where this does not
                        // resolve correctly. You can workaround this by providing your own code to determine the root URL.
                        //
                        //c.RootUrl(req => GetRootUrlFromAppConfig());

                        // If schemes are not explicitly provided in a Swagger 2.0 document, then the scheme used to access
                        // the docs is taken as the default. If your API supports multiple schemes and you want to be explicit
                        // about them, you can use the "Schemes" option as shown below.
                        //
                        //c.Schemes(new[] { "http", "https" });

                        // Use "SingleApiVersion" to describe a single version API. Swagger 2.0 includes an "Info" object to
                        // hold additional metadata for an API. Version and title are required but you can also provide
                        // additional fields by chaining methods off SingleApiVersion.
                        //
                        c.SingleApiVersion("v1", "Global Suite");
                        // Token 
                        c.ApiKey("Token")
                            .Description("Bearer token")
                            .Name("Authorization")
                            .In("header");
                        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,"bin","GlobalSuite.Web.xml"));
                         


                    })
                .EnableSwaggerUi(c =>
                    {
                         c.DocumentTitle("Global Suite API");
                        c.EnableApiKeySupport("apiKey", "header");
                    });
        }
    }
}
