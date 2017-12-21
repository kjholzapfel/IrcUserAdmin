using System;

namespace IrcUserAdmin.ConfigSettings.ConfigClasses
{
    [Serializable]
    public class NhibernateSettings
    {
        public string ConnectionString { get; set; }
        public string DBSchema { get; set; }
        public bool ShowSQL { get; set; }
        public bool ReCreateDatabase { get; set; }
        public string Salt { get; set; }
        public string ServerHost { get; set; }
        public int ServerPort { get; set; }
        public DbType DbType { get; set; }
    }
}
