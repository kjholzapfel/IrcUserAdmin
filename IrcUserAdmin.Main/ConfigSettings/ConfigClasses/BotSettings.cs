using System;
using System.Xml.Serialization;

namespace IrcUserAdmin.ConfigSettings.ConfigClasses
{
    [Serializable]
    public class BotSettings
    {
        public string IrcBotName { get; set; }
        public string OperChannel { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string IrcUserName { get; set; }
        public string IrcPassword { get; set; }
        public string OperUser { get; set; }
        public string OperPassword { get; set; }
        public bool SSL { get; set; }

        [XmlIgnore]
        public string Password { get { return string.Format("{0}:{1}", IrcUserName, IrcPassword); } }
    }
}