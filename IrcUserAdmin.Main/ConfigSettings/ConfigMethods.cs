using System;
using System.Configuration;

namespace IrcUserAdmin.ConfigSettings
{
    public static class ConfigMethods
    {
        public static string ReadConfigString(string key)
        {
            string configValue = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(configValue)) throw new ArgumentNullException(key);
            return configValue;
        }

        public static int ReadConfigInteger(string key)
        {
            string configValue = ReadConfigString(key);
            int configInt;
            if (!int.TryParse(configValue, out configInt))
            {
                throw new ArgumentNullException(key);
            }
            return configInt;
        }

        public static bool ReadConfigBoolean(string key)
        {
            string configValue = ConfigurationManager.AppSettings[key];
            bool configBool;
            if (!bool.TryParse(configValue, out configBool))
            {
                throw new ArgumentNullException(key);
            }
            return configBool;
        }
    }
}