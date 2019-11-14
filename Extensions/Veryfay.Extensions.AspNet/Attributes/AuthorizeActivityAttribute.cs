using System;
using Microsoft.AspNetCore.Authorization;

namespace Veryfay.Extensions.AspNet.Attributes
{
    public class AuthorizeActivityAttribute : AuthorizeAttribute
    {
        public Type EntityType { get; }

        public AuthorizeActivityAttribute(Type entityType)
            => this.EntityType = entityType;
    }
}
