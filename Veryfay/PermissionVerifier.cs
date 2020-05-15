using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veryfay
{
    internal static class PermissionVerifier
    {
        //internal static IsAllowingResult Verify<TPrincipal, TExtraInfo>(PermissionSet[] activityPermissions, TPrincipal principal, TExtraInfo extraInfo)
        //{
        //    return Verify(activityPermissions, principal, extraInfo);
        //    /*StringBuilder resMsg = new StringBuilder();
        //    var activityPermissionsCuratedRoleSets = CurateRoleSets(activityPermissions, principal, extraInfo);

        //    foreach (var ap in activityPermissionsCuratedRoleSets)
        //    {
        //        var denyRoleSets = ap.RoleSets.Where(x =>
        //                        x.GetType().GetGenericTypeDefinition() == typeof(DenyRoleSet<TPrincipal, TExtraInfo>).GetGenericTypeDefinition());

        //        foreach (dynamic denyRoleSet in denyRoleSets)
        //        {
        //            var msg = typeof(TExtraInfo) == typeof(Nothing) ? denyRoleSet.GetMsg(principal) : denyRoleSet.GetMsg(principal, extraInfo);
        //            if (typeof(TExtraInfo) == typeof(Nothing) ? denyRoleSet.Check(principal) : denyRoleSet.Check(principal, extraInfo))
        //            {
        //                resMsg.AppendLine(string.Format("###---- DENY SET => TRUE:\n {0}", msg));
        //                throw new AuthorizationException(resMsg.ToString());
        //            }
        //            else
        //            {
        //                resMsg.AppendLine(string.Format("---#### DENY SET => FALSE:\n {0}", msg));
        //            }
        //        }
        //    }

        //    foreach (var ap in activityPermissionsCuratedRoleSets)
        //    {
        //        var allowRoleSets = ap.RoleSets.Where(x =>
        //            x.GetType().GetGenericTypeDefinition() == typeof(AllowRoleSet<TPrincipal, TExtraInfo>).GetGenericTypeDefinition());

        //        foreach (dynamic allowRoleSet in allowRoleSets)
        //        {
        //            var msg = typeof(TExtraInfo) == typeof(Nothing) ? allowRoleSet.GetMsg(principal) : allowRoleSet.GetMsg(principal, extraInfo);
        //            if (typeof(TExtraInfo) == typeof(Nothing) ? allowRoleSet.Check(principal) : allowRoleSet.Check(principal, extraInfo))
        //            {
        //                resMsg.AppendLine(string.Format("###---------- ALLOW SET => TRUE:\n {0}", msg));
        //                return resMsg.ToString();
        //            }
        //            else
        //            {
        //                resMsg.AppendLine(string.Format("---#### ALLOW SET => FALSE:\n {0}", msg));
        //            }
        //        }
        //    }

        //    throw new AuthorizationException(string.IsNullOrEmpty(resMsg.ToString()) ? "NO MATCHING ROLE SET FOUND" : resMsg.ToString());*/
        //}

        internal static IsAllowingResult Verify<TPrincipal, TExtraInfo>(
        this PermissionSet[] activityPermissions,
        TPrincipal principal,
        TExtraInfo extraInfo
        )
        {
            var curatedPermissionSets = activityPermissions.CurateRoleSets<TPrincipal, TExtraInfo>();

            foreach (var permissionSet in curatedPermissionSets)
            {
                var denyRoleSets = permissionSet.RoleSets.GetAllDenyRuleSets<TPrincipal, TExtraInfo>();

                if (denyRoleSets.Matches(principal, extraInfo))
                    return new IsAllowingResult(false, "### DENY SET => TRUE");

                var allowRoleSets = permissionSet.RoleSets.GetAllAllowRoleSets<TPrincipal, TExtraInfo>();
                if (allowRoleSets.Matches(principal, extraInfo))
                    return new IsAllowingResult(true);
            }

            return new IsAllowingResult(false, "NO MATCHING ROLE SET FOUND");
        }

        private static PermissionSet[] CurateRoleSets<TPrincipal, TExtraInfo>(this PermissionSet[] activityPermissions)
        {
            var result = from ap in activityPermissions
                         select new PermissionSet(ap.And)
                         {
                             RoleSets = (from rs in ap.RoleSets
                                         where 
                                             (
                                                rs.PrincipalType == typeof(TPrincipal) 
                                                ||
                                                rs.PrincipalType.IsAssignableFrom(typeof(TPrincipal))
                                             )
                                             &&
                                         (typeof(TExtraInfo) == typeof(Nothing) || rs.ExtraInfoType == typeof(TExtraInfo))
                                         select rs).ToList()
                         };
            return result.ToArray();
        }        

        static bool Matches<TPrincipal, TExtraInfo>(this IEnumerable<RoleSet> roleSets, TPrincipal principal, TExtraInfo extraInfo)
        {
            if (typeof(TExtraInfo) == typeof(Nothing))
                return roleSets.Any(roleSet => ((dynamic)roleSet).Check(principal));

            return roleSets.Any(roleSet => roleSet.Check(principal, extraInfo));
            //return roleSets.Any(roleSet => ((dynamic)roleSet).Check(principal, extraInfo));
        }

        static IEnumerable<RoleSet> GetAllDenyRuleSets<TPrincipal, TExtraInfo>(this IEnumerable<RoleSet> roleSets)
            => roleSets.Where(x => x.GetType().GetGenericTypeDefinition() == typeof(DenyRoleSet<TPrincipal, TExtraInfo>).GetGenericTypeDefinition());

        static IEnumerable<RoleSet> GetAllAllowRoleSets<TPrincipal, TExtraInfo>(this IEnumerable<RoleSet> roleSets)
            => roleSets.Where(x => x.GetType().GetGenericTypeDefinition() == typeof(AllowRoleSet<TPrincipal, TExtraInfo>).GetGenericTypeDefinition());
    }
}