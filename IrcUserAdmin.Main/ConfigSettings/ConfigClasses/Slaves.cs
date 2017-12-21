using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IrcUserAdmin.ConfigSettings.ConfigClasses
{
    [Serializable]
    public class Slaves
    {
        [XmlElement("NHibernateSlave", typeof(NhibernateSettings))]
        public List<NhibernateSettings> NHSlave { get; set; }
    }
}