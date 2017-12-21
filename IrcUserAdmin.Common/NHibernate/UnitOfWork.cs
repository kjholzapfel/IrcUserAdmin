using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate;

namespace IrcUserAdmin.Common.NHibernate
{
    public class UnitOfWork : IUnitOfWork 
    {
        private readonly ISession _session;
        private readonly IList<IEntity> _saveEntities;
        private readonly IList<IEntity> _deleteEntities;

        private static readonly object Lock = new object();

        public UnitOfWork(ISessionFactory sessionFactory)
        {
            _saveEntities = new List<IEntity>();
            _deleteEntities = new List<IEntity>();
            _session = sessionFactory.OpenSession();
        }

        public IEnumerable<IEntity> SavedEntities { get { return _saveEntities; } }

        public void AddToSave(IEntity entity)
        {
            if (!_saveEntities.Contains(entity))
            {
                _saveEntities.Add(entity);
            }
        }

        public void AddToDelete(IEntity entity)
        {
            if (!_deleteEntities.Contains(entity))
            {
                _deleteEntities.Add(entity);
            }
        }

        public void PersistEntities()
        {
            if (!_deleteEntities.Any() && !_saveEntities.Any()) return;
            lock (Lock)
            {
                foreach (var saveEntity in _saveEntities)
                {
                    _session.SaveOrUpdate(saveEntity);
                }
                foreach (var deleteEntity in _deleteEntities)
                {
                    _session.Delete(deleteEntity);
                }
                using (var transaction = _session.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_session != null)
            {
                if (_session.IsConnected) _session.Close();
                _session.Dispose();
            }
        }

        public ISession Session
        {
            get { return _session; }
        }
    }
}