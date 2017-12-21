using System;
using System.Linq;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;
using IrcUserAdmin.Tools;

namespace IrcUserAdmin.Commands.Modules
{
    public class ChangePassword : AbstractIrcCommand<ChangePasswordArguments>
    {
        private readonly IPersistance _persistance;
        private static readonly Char[] BadPassChars = { ':', '^' };
        public ChangePassword(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.ChangePassword; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            var passcheck = Mapping[ChangePasswordArguments.Password].ToCharArray().Intersect(BadPassChars);
            if (passcheck.Any())
            {
                Messages.Add(string.Format("You cannot use the following characters for a password: {0}", string.Join(" ", BadPassChars)));
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[ChangePasswordArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[ChangePasswordArguments.Username]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            string hashedPassword = HashFunctions.ComputeHash(Mapping[ChangePasswordArguments.Password]);
            _persistance.ChangePassword(Mapping[ChangePasswordArguments.Username], hashedPassword);
            Messages.Add(string.Format("Password for user {0} succesfully changed.", Mapping[ChangePasswordArguments.Username]));
        }
    }

    public enum ChangePasswordArguments
    {
        Username,
        Password
    }
}