using System;
using AutoMapper;
using GlobalSuite.Core;
//using GlobalSuite.Web.Providers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Ninject;
using Owin;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using BaseUtility.Business;
using Ninject.Web.Common;
using Ninject.Web.WebApi;

[assembly: OwinStartup(typeof(GlobalSuite.Web.Startup))]

namespace GlobalSuite.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            ConfigureSecurityHeaders(app);
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            var key = ConfigurationManager.AppSettings["JwtKey"];
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
            app.UseJwtBearerAuthentication(
               new JwtBearerAuthenticationOptions
               {
                   AuthenticationMode = AuthenticationMode.Active,
                   TokenValidationParameters = new TokenValidationParameters()
                   {
                      
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = issuer,
                       ValidAudience = issuer,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                       
                       
                   },
                   TokenHandler=new JwtSecurityTokenHandler(),
                   Provider=new OAuthBearerAuthenticationProvider()
               });

            
            app.UseNinject(CreateKernel);
            app.UseWebApi(GlobalConfiguration.Configuration);
            //OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            //{
            //    AllowInsecureHttp = true,
            //    TokenEndpointPath = new PathString("/token"),
            //    AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
            //    Provider = new GlobalSuiteAuthorizationServerProvider()
            //};

            //// Token Generation
            //app.UseOAuthAuthorizationServer(OAuthServerOptions);
            //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }

         

        private void ConfigureSecurityHeaders(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
#if DEBUG
#else

                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-Powered-By");
                // Set X-Content-Type-Options header to prevent content type sniffing
                context.Response.Headers.Add("X-Content-Type-Options", new []{"nosniff"} );

                // Set X-Frame-Options header to prevent Clickjacking attacks
                context.Response.Headers.Add("X-Frame-Options", new []{"SAMEORIGIN"} );

                // Set X-XSS-Protection header to enable browser's XSS filter
                context.Response.Headers.Add("X-XSS-Protection", new []{ "1; mode=block"});

                // Set Content-Security-Policy header to prevent various types of attacks
               // context.Response.Headers.Add("Content-Security-Policy", new []{ "default-src 'self' script-src 'unsafe-hashes' 'sha256-{HASHED_EVENT_HANDLER}'"});

                // Set Strict-Transport-Security header to enforce HTTPS (optional)
                context.Response.Headers.Add("Strict-Transport-Security",new []{"max-age=31536000"} );
#endif
                await next.Invoke();
            });
        }

        private IKernel CreateKernel()
        {
            var kernel= new StandardKernel();
            // GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
            kernel.Load(Assembly.GetExecutingAssembly());
            kernel.Load<CoreModule>();
            kernel. Bind<GeneralFunc>().ToSelf();
            var mapperConfiguration = MapperConfig.InitializeAutomapper();
            kernel.Bind<MapperConfiguration>().ToConstant(mapperConfiguration).InSingletonScope();
            kernel.Bind<IMapper>().ToMethod(ctx =>
            new Mapper(mapperConfiguration, type => ctx.Kernel.Get(type)));
            // kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);  
            // kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            return kernel;
        }
    }
}
