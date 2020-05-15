using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Veryfay.Extensions.AspNet.Attributes;

namespace Veryfay.Extensions.AspNet.Auhtorization
{
    public static class AuthorizationHandlerContextExtension
    {
        public static Activity GetVeryfayActivity(this AuthorizationHandlerContext context)
        {
            var routeEndpoint = ((Microsoft.AspNetCore.Routing.RouteEndpoint)context.Resource);
            if (routeEndpoint != null)
            {
                var authActivity = routeEndpoint.Metadata.GetMetadata<AuthorizeActivityAttribute>();
                if (authActivity != null)
                {
                    var httpMethodMetaData = routeEndpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Routing.HttpMethodMetadata>();
                    return authActivity.EntityType.GetActivity(httpMethodMetaData.HttpMethods.First());
                }
            }

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