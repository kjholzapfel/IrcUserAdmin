using System.Collections.Generic;
using IrcUserAdmin.Common.NHibernate;

namespace IrcUserAdmin.Entities
{
    public class User : EntityBase<User>
    {
        public virtual string Name { get; set; }
        public virtual string Password { get; set; }
        public virtual bool IsAdmin { get; set; }
        public virtual bool IsOper { get; set; }
        public virtual bool AutoJoin { get; set; }
        public virtual string Vhost { get; set; }
        public virtual IList<Hostname> Hosts { get; set; }
        public virtual IList<Group> Groups { get; set; }
        public virtual IList<Group> GroupAdmin { get; set; }
    }
}