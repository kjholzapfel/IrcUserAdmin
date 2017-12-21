using System;
using System.IO;
using IrcUserAdmin.ConfigSettings.ConfigClasses;
using IrcUserAdmin.Tools;

namespace IrcUserAdmin.ConfigSettings
{
    public class BotConfig : IBotConfig
    {
        private const string XmlFileName = "config.xml";

        public BotConfig()
        {
            ReadConfig();
        }

        public BotSettings BotSettings => Settings.BotSettings;

        public Settings Settings { get; private set; }

        private void ReadConfig()
        {
            string configPath = AppConfigSettings.GetInstance.ConfigFileLocation;
            if (string.IsNullOrWhiteSpace(configPath))
            {
                throw new ApplicationException(
                    "ConfigFileLocation in app.config not found, please edit it and restar the program.");
            }
            var xmlPath = Path.Combine(configPath, XmlFileName);
            var fi = new FileInfo(xmlPath);
            var xmlhelper = new XmlSerializerHelper<Settings>();
            if (fi.Exists)
            {
                Settings = xmlhelper.Read(xmlPath);
            }
            else
            {
                throw new ApplicationException($"Configuration file not found at location {fi.FullName}, please edit it and restart the program.");
            }
        } 
    }
}