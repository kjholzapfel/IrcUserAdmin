using FluentNHibernate.Mapping;

namespace IrcUserAdmin.Entities
{
    public class HostnameMap : ClassMap<Hostname>
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