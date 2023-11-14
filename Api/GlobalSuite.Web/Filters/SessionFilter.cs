using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using BaseUtility.Business;
using GlobalSuite.Core;

namespace GlobalSuite.Web.Filters
{
    public class SessionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
             if (actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                var identity = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
                var userName = identity.Claims.First(x => x.Type == JwtRegisteredClaimNames.NameId)?.Value;
                var companyNumber = identity.Claims.First(x => x.Type == Constants.GClaimTypes.CompanyName)?.Value;
                var branchId = identity.Claims.First(x => x.Type == Constants.GClaimTypes.BranchNumber)?.Value;
              GeneralFunc.UserName = userName;
              if(!string.IsNullOrWhiteSpace(companyNumber))
                  GeneralFunc.CompanyNumber = Convert.ToInt16(companyNumber);
              if(!string.IsNullOrWhiteSpace(branchId))
                  GeneralFunc.UserBranchNumber = branchId;
            }
        }
    }
}
