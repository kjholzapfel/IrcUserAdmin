using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IrcUserAdmin.ConfigSettings.ConfigClasses
{
    [Serializable]
    [XmlRoot(ElementName = "Settings", IsNullable = false)]
    public class Settings
    {
        public bool CommitUsers { get; set; }
        [XmlElement("BotSettings", typeof(BotSettings))]
        public BotSettings BotSettings { get; set; }
        [XmlElement("AddUsers", typeof(BuildInDbUsers))]
        public List<BuildInDbUsers> DbUsers { get; set; }
        [XmlElement("NHibernateSettings", typeof(NhibernateSettings))]
        public NhibernateSettings NHSettings { get; set; }
        [XmlElement("NHibernateSlaves", typeof(Slaves))]
        public Slaves NHSlaves { get; set; }
        [XmlElement("SoapSlaves", typeof(SoapSlave))]
        public List<SoapSlave> SoapSlaves { get; set; }

    }
}