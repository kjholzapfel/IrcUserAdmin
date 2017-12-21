using System;
using System.Collections.Generic;
using NHibernate;

namespace IrcUserAdmin.Common.NHibernate
{
    public interface IUnitOfWork : IDisposable 
    {
        IEnumerable<IEntity> SavedEntities { get; }
        void AddToSave(IEntity entity);
        void AddToDelete(IEntity entity);

        void PersistEntities();

        ISession Session { get; }
    }
}