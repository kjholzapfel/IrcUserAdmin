using System.Collections.Generic;
using IrcUserAdmin.Common.NHibernate;

namespace IrcUserAdmin.Entities
{
    public class Group : EntityBase<Group>
    {
        public virtual string GroupName { get; set; }
        public virtual IList<User> Members { get; set; }
        public virtual IList<User> GroupAdmins { get; set; }
        public virtual string Channel { get; set; }
    }
}