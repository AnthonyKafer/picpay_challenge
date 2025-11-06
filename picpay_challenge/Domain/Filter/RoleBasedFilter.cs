using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using picpay_challenge.Domain.Models.User;
using System.Net;
using System.Security.Claims;
using System.Text;
using static picpay_challenge.Domain.Models.User.BaseUser;

namespace picpay_challenge.Domain.Filter
{
    public class RoleBasedFilter : AuthorizeAttribute, IAuthorizationFilter 
    {
        private readonly IEnumerable<BaseUser.Roles>? _allow = [];
        public RoleBasedFilter(BaseUser.Roles[]? allow)
        {
            _allow = allow;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {

            var currentRole = Enum.Parse<BaseUser.Roles>(filterContext.HttpContext.User.FindFirst(ClaimTypes.Role)!.Value);
            if (!_allow!.Contains(currentRole))
            {
                filterContext.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                filterContext.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("You cannot access this resource"));
            }


        }

    }
}
