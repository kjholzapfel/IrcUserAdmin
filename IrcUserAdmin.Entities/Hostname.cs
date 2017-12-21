using IrcUserAdmin.Common.NHibernate;

namespace IrcUserAdmin.Entities
{
    public class Hostname : EntityBase<Hostname>
    {
        public virtual string Host { get; set; }
        public virtual User User { get; set; }
    }
}