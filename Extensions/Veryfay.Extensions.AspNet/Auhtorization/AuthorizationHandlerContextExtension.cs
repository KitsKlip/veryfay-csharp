using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Veryfay.Extensions.AspNet.Auhtorization
{
    public static class AuthorizationHandlerContextExtension
    {
        public static Activity GetVeryfayActivity(this AuthorizationHandlerContext context)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;
            return filterContext?.GetActivity();
        }
    }
}
