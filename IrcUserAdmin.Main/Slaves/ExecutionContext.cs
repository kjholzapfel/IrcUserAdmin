using System;
using System.Threading.Tasks;
using Autofac;

namespace IrcUserAdmin.Slaves
{
    public abstract class ExecutionContext<T, TKey> where T : class
    {
        private readonly ILifetimeScope _scope;
        protected T Resolved;
        protected TKey Key;
        protected ExecutionContext(ILifetimeScope scope)
        {
            if (scope == null) throw new ArgumentNullException("scope");
            _scope = scope;
        }

        protected void StartThread() 
        {
            Task.Factory.StartNew(ResolveAndExecute);
        }

        private void ResolveAndExecute()
        {
            using (var scope = _scope.BeginLifetimeScope())
            {
                Resolved = scope.ResolveKeyed<T>(Key);
                InnerExecute();
            }
        }

        protected abstract void InnerExecute();
    }
}