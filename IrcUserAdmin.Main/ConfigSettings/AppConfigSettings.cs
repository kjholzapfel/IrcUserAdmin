using System;
using System.Configuration;

namespace IrcUserAdmin.ConfigSettings
{
    public class AppConfigSettings
    {
        private static readonly Lazy<AppConfigSettings> Instance =
            new Lazy<AppConfigSettings>(() => new AppConfigSettings());

        private AppConfigSettings()
        {
            ReadConfig();
        }

        public static AppConfigSettings GetInstance => Instance.Value;

        private void ReadConfig()
        {
            ConfigFileLocation = ConfigMethods.ReadConfigString("ConfigFileLocation");
        }

        public string ConfigFileLocation { get; private set; }
    }
}