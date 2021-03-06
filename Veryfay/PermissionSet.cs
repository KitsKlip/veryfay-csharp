﻿using System.Collections.Generic;
using System.Linq;

namespace Veryfay
{
    public class PermissionSet
    {
        internal List<RoleSet> RoleSets { get; set; }

        public AuthorizationEngine And { get; private set; }

        public PermissionSet(AuthorizationEngine ae)
        {
            this.RoleSets = new List<RoleSet>();
            this.And = ae;
        }

        public PermissionSet Allow<TPrincipal, TExtraInfo>(Role<TPrincipal, TExtraInfo> role, params Role<TPrincipal, TExtraInfo>[] moreRoles)
        {
            var roles = new Role<TPrincipal, TExtraInfo>[] { role }.Concat(moreRoles).ToArray();
            this.RoleSets.Add(new AllowRoleSet<TPrincipal, TExtraInfo>(roles));
            return this;
        }

        public PermissionSet AllowAny<TPrincipal, TExtraInfo>(Role<TPrincipal, TExtraInfo> role, params Role<TPrincipal, TExtraInfo>[] moreRoles)
        {
            this.Allow(role);
            
            foreach (var additionalRole in moreRoles)
                this.Allow(additionalRole);
            
            return this;
        }

        public PermissionSet Deny<TPrincipal, TExtraInfo>(Role<TPrincipal, TExtraInfo> role, params Role<TPrincipal, TExtraInfo>[] moreRoles)
        {
            var roles = new Role<TPrincipal, TExtraInfo>[] { role }.Concat(moreRoles).ToArray();
            this.RoleSets.Add(new DenyRoleSet<TPrincipal, TExtraInfo>(roles));
            return this;
        }
    }
}
