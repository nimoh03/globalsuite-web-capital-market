using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http.Cors;
 

namespace GlobalSuite.Web.Providers
{
    public class GCorsProvider:ICorsPolicyProvider
    {
        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var  policy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true,
                SupportsCredentials = true
                
            };
            var allowedOrigins = ConfigurationManager.AppSettings.Get("AllowedOrigins");
            if (string.IsNullOrEmpty(allowedOrigins)) return Task.FromResult(policy);
            
            var origins = allowedOrigins.Split(',');
            
            foreach (var origin in origins)
            {
                policy.Origins.Add(origin);
            }
            return Task.FromResult(policy);
        }
    }
    
    public class CorsPolicyFactory : ICorsPolicyProviderFactory
    {
        private readonly ICorsPolicyProvider _provider = new GCorsProvider();


        public ICorsPolicyProvider GetCorsPolicyProvider(HttpRequestMessage request)
        {
            return _provider;
        }
    }
}