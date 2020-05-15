using System.Collections.Generic;

namespace Veryfay
{
    public class ActivityAuthorization
    {
        private Activity activity;
        private ActivityRegistry activityRegistry;

        internal ActivityAuthorization(Activity activity, ActivityRegistry activityRegistry)
        {
            this.activity = activity;
            this.activityRegistry = activityRegistry;
        }

        public IsAllowingResult IsAllowing<TPrincipal>(TPrincipal principal)
            => this.IsAllowing(principal, Nothing.AtAll);

        public IsAllowingResult IsAllowing<TPrincipal, TExtraInfo>(TPrincipal principal, TExtraInfo extraInfo)
            => this.Authorize(principal, extraInfo);

        public string Verify<TPrincipal>(TPrincipal principal)
            => this.Verify(principal, Nothing.AtAll);

        public string Verify<TPrincipal, TExtraInfo>(TPrincipal principal, TExtraInfo extraInfo)
        {
            var result = this.Authorize(principal, extraInfo);
            if (result.IsSuccess)
                return result.Details;

            throw new AuthorizationException(result.Details);
        }

        private IsAllowingResult Authorize<TPrincipal, TExtraInfo>(TPrincipal principal, TExtraInfo extraInfo)
        {
            PermissionSet[] activityPermissions;
            try
            {
                activityPermissions = this.activityRegistry.Get(this.activity);
            }
            catch (KeyNotFoundException e)
            {
                return new IsAllowingResult(false, e.Message);
            }

            return activityPermissions.Verify(principal, extraInfo);
        }
    }

    public struct IsAllowingResult
    {
        public IsAllowingResult(bool isAllowing, string details)
        {
            this.IsSuccess = isAllowing;
            this.IsFailure = !isAllowing;
            this.Details = details;
        }

        public IsAllowingResult(bool isAllowing)
            : this(isAllowing, string.Empty)
        { }

        public bool IsFailure { get; }
        public bool IsSuccess { get; }
        public string Details { get; }
    }
}