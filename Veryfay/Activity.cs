using System;

namespace Veryfay
{
    public interface Activity
    {
        string Target { get; }
    }

    public interface Activity<TTarget> : Activity { }
    public interface Container
    {
        Activity[] Activities { get; }
    }

    public abstract class ActivityOnTargetBase : Activity
    {
        public string Target { get; }

        public ActivityOnTargetBase(string target)
            => this.Target = target;

        public ActivityOnTargetBase(Type target)
            : this(target.FullName) { }
    }

    public class Create : ActivityOnTargetBase
    {
        public Create(string target) : base(target) { }

        public Create(Type target)
            : base(target) { }
    }
    public class Read : ActivityOnTargetBase
    {
        public Read(string target) : base(target) { }

        public Read(Type target)
            : base(target) { }
    }
    public class Update : ActivityOnTargetBase
    {
        public Update(string target) : base(target) { }

        public Update(Type target)
            : base(target) { }
    }
    public class Patch : ActivityOnTargetBase
    {
        public Patch(string target) : base(target) { }

        public Patch(Type target)
            : base(target) { }
    }
    public class Delete : ActivityOnTargetBase
    {
        public Delete(string target) : base(target) { }

        public Delete(Type target)
            : base(target) { }
    }

    public class CRUD : ActivityOnTargetBase, Container
    {
        public Activity[] Activities { get; private set; }

        public CRUD(string target) : base(target)
            => this.InitializeActivities();

        public CRUD(Type target) : base(target)
            => this.InitializeActivities();

        private void InitializeActivities()
            => this.Activities = new Activity[] { new Create(this.Target), new Read(this.Target), new Update(this.Target), new Delete(this.Target) };

    }
    public class CRUDP : ActivityOnTargetBase, Container
    {
        public Activity[] Activities { get; private set; }

        public CRUDP(string target) : base(target)
            => this.InitializeActivities();

        public CRUDP(Type target) : base(target)
            => this.InitializeActivities();

        private void InitializeActivities()
            => this.Activities = new Activity[] { new CRUD(this.Target), new Patch(this.Target) };
    }

    #region [Type T]    
    public sealed class Create<TTarget> : Create { public Create() : base(typeof(TTarget)) { } }
    public sealed class Read<TTarget> : Create { public Read() : base(typeof(TTarget)) { } }
    public sealed class Update<TTarget> : Update { public Update() : base(typeof(TTarget)) { } }
    public sealed class Patch<TTarget> : Patch { public Patch() : base(typeof(TTarget)) { } }
    public sealed class Delete<TTarget> : Delete { public Delete() : base(typeof(TTarget)) { } }

    public sealed class CRUD<TTarget> : CRUD
    {
        public CRUD() : base(typeof(TTarget)) { }
    }
    public sealed class CRUDP<TTarget> : CRUDP
    {
        public CRUDP() : base(typeof(TTarget)) { }
    }
    #endregion
}