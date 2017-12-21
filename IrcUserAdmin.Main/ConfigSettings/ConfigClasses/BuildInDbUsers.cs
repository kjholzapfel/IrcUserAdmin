using System;

namespace IrcUserAdmin.ConfigSettings.ConfigClasses
{
    [Serializable]
    public class BuildInDbUsers
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Group { get; set; }
        public bool IsOper { get; set; }
        public bool IsAdmin { get; set; }
    }
}