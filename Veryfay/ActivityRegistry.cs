using System;
using System.Collections.Generic;
using System.Linq;

namespace Veryfay
{
    internal class ActivityRegistry
    {
        private Dictionary<string, PermissionSet[]> registeredPermissions = new Dictionary<string, PermissionSet[]>();

        internal void Add(Activity activity, PermissionSet ps)
        {
            if (activity is Container)
                foreach (var a in (activity as Container).Activities)
                    this.Add(a, ps);
            else
                this.AddActivityPermissions(activity, ps);
        }

        private void AddActivityPermissions(Activity activity, PermissionSet ps)
        {
            PermissionSet[] activityPermissionList = new PermissionSet[0];
            var activityKey = this.GetActivityKey(activity);
            if (this.registeredPermissions.ContainsKey(activityKey))
                activityPermissionList = this.registeredPermissions[activityKey];
            this.registeredPermissions[activityKey] = activityPermissionList.Concat(new PermissionSet[] { ps }).ToArray();
        }

        internal PermissionSet[] Get(Activity activity)
        {
            PermissionSet[] permissionSets;
            var activityKey = this.GetActivityKey(activity);
            if (this.registeredPermissions.TryGetValue(activityKey, out permissionSets))
                return permissionSets;
            else
            {
                string msg = string.Format("no registered activity of type {0}", activity);
                throw new KeyNotFoundException(msg);
            }
        }

        internal string GetActivityKey(Activity activity)
            => $"{activity.GetType().GetNameWithoutGenericArity()}<{activity.Target}>";
    }

    internal static class TypeExtensions
    {
        internal static string GetNameWithoutGenericArity(this Type type)
        {
            if (type.IsGenericType)
                return type.Name.Substring(0, type.Name.IndexOf('`'));
            else
                return type.Name;
        }
    }
}
