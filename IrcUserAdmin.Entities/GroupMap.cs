using FluentNHibernate.Mapping;

namespace IrcUserAdmin.Entities
{
    public class GroupMap : ClassMap<Group>
    {
        public GroupMap()
        {
            Table("tbl_groups");
            Id(x => x.Id, "group_id").GeneratedBy.GuidComb();
            Map(x => x.GroupName).Unique();
            Map(x => x.Channel);
            HasManyToMany(x => x.Members)
                .Table("tbl_groupusers")
                .ParentKeyColumn("group_id")
                .ChildKeyColumn("user_id");
            HasManyToMany(x => x.GroupAdmins)
                .Table("tbl_groupadmins")
                .ParentKeyColumn("group_id")
                .ChildKeyColumn("user_id");
        }
    }
}