namespace Veryfay
{
    public class AuthorizationEngine
    {
        private ActivityRegistry activityRegistry = new ActivityRegistry();

        public PermissionSet Register(Activity activity, params Activity[] moreActivities)
        {
            var ps = new PermissionSet(this);
            this.activityRegistry.Add(activity, ps);
            foreach (var a in moreActivities)
                this.activityRegistry.Add(a, ps);

            return ps;
        }

        public ActivityAuthorization this[Activity activity]
            => new ActivityAuthorization(activity, activityRegistry);

        public IsAllowingResult IsAllowing<TPrincipal, TExtraInfo>(Activity activity, TPrincipal principal)
            => this[activity].IsAllowing(principal);

        public IsAllowingResult IsAllowing<TPrincipal, TExtraInfo>(Activity activity, TPrincipal principal, TExtraInfo extraInfo)
            => this[activity].IsAllowing(principal, extraInfo);

        public string Verify<TPrincipal>(Activity activity, TPrincipal principal)
           => this.Verify(activity, principal, Nothing.AtAll);

        public string Verify<TPrincipal, TExtraInfo>(Activity activity, TPrincipal principal, TExtraInfo extraInfo)
        => this[activity].Verify(principal, extraInfo);

    }
}
