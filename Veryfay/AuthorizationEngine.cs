namespace Veryfay
{
    public class AuthorizationEngine
    {
        private ActivityRegistry activityRegistry = new ActivityRegistry();

        public PermissionSet Register(Activity activity, params Activity[] moreActivities)
        {
            var ps = new PermissionSet(this);
            activityRegistry.Add(activity, ps);
            foreach (var a in moreActivities)
                activityRegistry.Add(a, ps);

            return ps;
        }

        public ActivityAuthorization this[Activity activity]
            => new ActivityAuthorization(activity, activityRegistry);
    }
}
