using System.Collections.Generic;
using FluentNHibernate.Mapping;
using IrcUserAdmin.Common.NHibernate;

namespace IrcUserAdmin.Entities.Slave
{
    public class SlaveUser : EntityBase<SlaveUser>
    {
        public virtual string Name { get; set; }
        public virtual string Password { get; set; }
        public virtual IList<SlaveHostname> Hosts { get; set; }
    }

    public class UsersMap : ClassMap<SlaveUser>
    {
        public UsersMap()
        {
            Table("tbl_users");
            Id(x => x.Id, "user_id")
                .GeneratedBy
                .GuidComb();
            Map(x => x.Name);
            Map(x => x.Password);
            HasMany(x => x.Hosts).Cascade.All();
        }
    }
}
