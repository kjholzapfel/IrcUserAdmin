using System;

namespace IrcUserAdmin.Common.NHibernate
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}