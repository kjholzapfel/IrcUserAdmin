using FluentNHibernate.Mapping;
using IrcUserAdmin.Common.NHibernate;

namespace IrcUserAdmin.Entities.Slave
{
    public class SlaveHostname : EntityBase<SlaveHostname>
    {
        public virtual string Host { get; set; }
        public virtual SlaveUser User { get; set; }
    }

    public class HostnameMap : ClassMap<SlaveHostname>
    {
        public HostnameMap()
        {
            Table("tbl_hostnames");
            Id(x => x.Id, "hostname_id")
                .GeneratedBy
                .GuidComb();
            Map(x => x.Host);
            References(x => x.User);
        }
    }
}