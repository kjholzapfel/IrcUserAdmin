using FluentNHibernate.Mapping;

namespace IrcUserAdmin.Entities
{
    public class UsersMap : ClassMap<User>
    {
        public UsersMap()
        {
            Table("tbl_users");
            Id(x => x.Id, "user_id")
                .GeneratedBy
                .GuidComb();
            Map(x => x.Name);
            Map(x => x.Password);
            Map(x => x.AutoJoin);
            Map(x => x.IsAdmin);
            Map(x => x.Vhost);
            Map(x => x.IsOper);
            HasMany(x => x.Hosts).Cascade.All();
            HasManyToMany(x => x.Groups)
                .Table("tbl_groupusers")
                .ParentKeyColumn("user_id")
                .ChildKeyColumn("group_id");
            HasManyToMany(x => x.GroupAdmin)
                .Table("tbl_groupadmins")
                .ParentKeyColumn("user_id")
                .ChildKeyColumn("group_id");
        }
    }
}