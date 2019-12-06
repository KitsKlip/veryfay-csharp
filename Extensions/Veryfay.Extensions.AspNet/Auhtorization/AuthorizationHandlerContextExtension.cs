using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Veryfay.Extensions.AspNet.Auhtorization
{
    public static class AuthorizationHandlerContextExtension
    {
        public static Activity GetVeryfayActivity(this AuthorizationHandlerContext context)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;
            if (filterContext != null)
                return filterContext?.GetActivity();

            var controller = context.Resource as ControllerBase;
            if (controller != null)
                return controller?.GetActivity();

            return null;
        }
    }
}