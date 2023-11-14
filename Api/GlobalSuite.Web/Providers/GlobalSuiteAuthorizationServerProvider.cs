using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;

namespace GlobalSuite.Web.Providers
{
    public class GlobalSuiteAuthorizationServerProvider : OAuthBearerAuthenticationProvider
    {

//        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
//        {
//            context.Validated();
//        }

//        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
//        {

//            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

//;              var tokenHeader = context.OwinContext.Request.Headers["Authorization"];
//            if(tokenHeader != null)
//            {
//                var token = tokenHeader.Split(' ')[1];
//                if(!string.IsNullOrEmpty(token) ) {
//            context.Validated();
//                               }
//            }
            

//        }

        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            return base.RequestToken(context);
        }

        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            return base.ValidateIdentity(context);
        }
    }
}