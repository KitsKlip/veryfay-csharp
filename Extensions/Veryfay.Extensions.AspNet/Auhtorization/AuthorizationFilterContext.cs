using System;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Veryfay.Extensions.AspNet.Attributes;

namespace Veryfay.Extensions.AspNet.Auhtorization
{
    public static class AuthorizationFilterContextExtension
    {
        internal static Activity GetActivity(this AuthorizationFilterContext context)
            => context?.GetActivityEntityType().GetActivity(context.GetHttpMethod());

        internal static Activity GetActivity(this ControllerBase controller)
           => controller?.GetActivityEntityType().GetActivity(controller.GetHttpMethod());

        internal static Activity GetActivity(this Type entityType, string httpMethod)
        {
            if (entityType == null) return default;

            switch (httpMethod)
            {
                case "POST":
                    return new Create(entityType);
                case "PUT":
                    return new Update(entityType);
                case "GET":
                    return new Read(entityType);
                case "DELETE":
                    return new Delete(entityType);
                case "PATCH":
                    return new Patch(entityType);
                default:
                    return default;
            }
        }

        internal static string GetHttpMethod(this AuthorizationFilterContext context)
            => context?.HttpContext?.GetHttpMethod();

        internal static string GetHttpMethod(this ControllerBase controller)
            => controller?.HttpContext.GetHttpMethod();

        internal static string GetHttpMethod(this HttpContext httpContext)
            => httpContext?.Request.Method.ToUpperInvariant();

        internal static AuthorizeActivityAttribute GetAuthorizeActivityAttribute(this AuthorizationFilterContext context)
        {
            if (context == null) return null;

            var descriptor = context?.ActionDescriptor as ControllerActionDescriptor;

            if (descriptor == null) return null;

            var activityAttribute = descriptor.MethodInfo.GetCustomAttribute(typeof(AuthorizeActivityAttribute), true);

            if (activityAttribute is null)
                activityAttribute = descriptor.ControllerTypeInfo.GetCustomAttribute(typeof(AuthorizeActivityAttribute), true);

            return (AuthorizeActivityAttribute)activityAttribute;
        }

        internal static AuthorizeActivityAttribute GetAuthorizeActivityAttribute(this ControllerActionDescriptor descriptor)
        {
            if (descriptor == null) return null;
            var activityAttribute = (AuthorizeActivityAttribute)descriptor.MethodInfo.GetCustomAttribute(typeof(AuthorizeActivityAttribute), true);

            if (activityAttribute is null)
                activityAttribute = (AuthorizeActivityAttribute)descriptor.ControllerTypeInfo.GetCustomAttribute(typeof(AuthorizeActivityAttribute), true);

            return activityAttribute;
        }

        internal static Type GetActivityEntityType(this AuthorizationFilterContext context)
            => context?.GetAuthorizeActivityAttribute()?.EntityType;

        internal static Type GetActivityEntityType(this ControllerBase controller)
            => controller?.ControllerContext?.ActionDescriptor?.GetAuthorizeActivityAttribute()?.EntityType;

    }
}