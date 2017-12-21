using System;
using System.Linq;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;
using IrcUserAdmin.Tools;

namespace IrcUserAdmin.Commands.Modules
{
    public class AddUser : AbstractIrcCommand<AdduserArguments>
    {
        private readonly IPersistance _persistance;
        private static readonly Char[] BadPassChars = { ':', '^' };

        public AddUser(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.AddUser; } }
        public override CommandType CommandType { get {return CommandType.AdminCommand;} }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            var passcheck = Mapping[AdduserArguments.Password].ToCharArray().Intersect(BadPassChars);
            if (passcheck.Any())
            {
                Messages.Add(string.Format("You cannot use the following characters for a password: {0}", string.Join(" ", BadPassChars)));
                return false;
            }
            if (_persistance.UserNameExists(Mapping[AdduserArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} already exists.", Mapping[AdduserArguments.Username]));
                return false;
            }
            if (!_persistance.GroupExists(Mapping[AdduserArguments.Groupname]))
            {
                Messages.Add(string.Format("Groupname {0} doesn't exists, please add it first.", Mapping[AdduserArguments.Groupname]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            string hashedPassword = HashFunctions.ComputeHash(Mapping[AdduserArguments.Password]);
            _persistance.AddNewUserToGroup(Mapping[AdduserArguments.Username], hashedPassword, Mapping[AdduserArguments.Groupname]);
            Messages.Add(string.Format("User {0} succesfully added to group {1}", Mapping[AdduserArguments.Username], Mapping[AdduserArguments.Groupname]));
        }
    }

    public enum AdduserArguments 
    {
        Username = 0,
        Password = 1,
        Groupname = 2,
    }
}