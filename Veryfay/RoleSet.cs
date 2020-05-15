using System;
using System.Linq;

namespace Veryfay
{
    internal abstract class RoleSet
    {
        internal abstract Type PrincipalType { get; }
        internal abstract Type ExtraInfoType { get; }

        internal abstract bool Check<TPrincipal, TExtraInfo>(TPrincipal principal, TExtraInfo extraInfo = default(TExtraInfo));
    }

    internal abstract class RoleSet<TPrincipal, TExtraInfo> : RoleSet
    {
        internal abstract Role<TPrincipal, TExtraInfo>[] Roles { get; }

        internal override Type PrincipalType { get { return typeof(TPrincipal); } }
        internal override Type ExtraInfoType { get { return typeof(TExtraInfo); } }

        internal override bool Check<TP, TE>(TP principal, TE extraInfo = default(TE))
        {
            foreach (var role in this.Roles)
            {
                var res = role.GetType().IsAssignableFrom(typeof(Role<TP, TE>));
                if (PrincipalType.IsAssignableFrom(typeof(TP)) && ExtraInfoType.IsAssignableFrom(typeof(TE)))
                {
                    var principalValue = (TPrincipal)((dynamic)principal);
                    var extraInfoValue = default(TExtraInfo);
                    
                    if (extraInfo != null)//TODO:Fix != default
                        extraInfoValue = (TExtraInfo)((dynamic)extraInfo);

                    if (role.Contains(principalValue, extraInfoValue))
                        return true;
                }
            }

            return false;
        }
    }

    internal sealed class AllowRoleSet<TPrincipal, TExtraInfo> : RoleSet<TPrincipal, TExtraInfo>
    {
        private Role<TPrincipal, TExtraInfo>[] roles;
        internal override Role<TPrincipal, TExtraInfo>[] Roles { get { return this.roles; } }

        public AllowRoleSet(Role<TPrincipal, TExtraInfo>[] roles)
        {
            this.roles = roles;
        }
    }

    internal sealed class DenyRoleSet<TPrincipal, TExtraInfo> : RoleSet<TPrincipal, TExtraInfo>
    {
        private Role<TPrincipal, TExtraInfo>[] roles;
        internal override Role<TPrincipal, TExtraInfo>[] Roles { get { return this.roles; } }

        public DenyRoleSet(Role<TPrincipal, TExtraInfo>[] roles)
        {
            this.roles = roles;
        }
    }
}
