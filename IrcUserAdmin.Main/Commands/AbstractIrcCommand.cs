using System;
using System.Collections.Generic;
using System.Linq;
using IrcUserAdmin.Commands.OperCommands;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands
{
    public abstract class AbstractIrcCommand<TArguments> : IIrcCommand where TArguments : IComparable, IConvertible, IFormattable
    {
        private readonly IPersistance _persistance;
        private readonly IIrcReadWriteExchange _ircReadWriteExchange;
        public abstract CommandName Name { get; }
        public abstract CommandType CommandType { get; }
        protected bool IsOper { get; private set; }
        protected List<string> Messages {  get; private set; }
        protected List<IOperCommand> OperCommands { get; private set; }
        protected IrcCommandContext Context { get; private set; }
        protected Dictionary<TArguments, string> Mapping { get; private set; }
        public virtual bool NeedsOperPermission { get { return true; } }
        protected AbstractIrcCommand(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
            _ircReadWriteExchange = ircReadWriteExchange;
            Messages = new List<string>();
            Mapping = new Dictionary<TArguments, string>();
            OperCommands = new List<IOperCommand>();
        }

        public void SetContext(IrcCommandContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            Context = context;
        }
        public bool CheckPermissions()
        {
            var user = _persistance.GetUserFromIdent(Context.Ident);
            if (user != null)
            {
                IsOper = user.IsOper;
            }
            return IsOper;
        }

        public virtual bool CheckPreconditions()
        {
            if (typeof(TArguments) == typeof (EmptyEnum)) return true;
            var enumInts = Enum.GetValues(typeof(TArguments)).Cast<int>().ToList();
            if (Context.MsgArray.Count < enumInts.Max())
            {
                Messages.Add("Insufficient parameters");
                Help();
                return false;
            }
            return true;
        }

        public abstract void Execute();
     
        public virtual void Help()
        {
            var enums = Enum.GetValues(typeof(TArguments)).Cast<TArguments>().ToList();
            var helpBuilder = new List<string> {"Usage", Name.ToString()};
            foreach (var enumName in enums)
            {
                string formatted = string.Concat("<", enumName, ">");
                helpBuilder.Add(formatted);
            }
            string helpString = string.Join(" ", helpBuilder);
            Messages.Add(helpString.ToLowerInvariant());
        }

        public virtual void MapInput()
        {
            var enumInts = Enum.GetValues(typeof(TArguments)).Cast<int>().ToList();
            if (!enumInts.Any()) return;
            if (Context.MsgArray.Count >= enumInts.Max())
            {
                var enums = Enum.GetValues(typeof(TArguments)).Cast<TArguments>().ToList();
                for (int i = 0; i <= enumInts.Max(); i++)
                {
                    var enumtype = enums[i];
                    Mapping.Add(enumtype, Context.MsgArray[i]);
                }
            }
        }

        public void Write()
        {
            _ircReadWriteExchange.Write(GetWriteContext());
            _ircReadWriteExchange.ExecuteOperCommands(OperCommands);
        }

        private IEnumerable<IrcWriteContext> GetWriteContext()
        {
            foreach (var message in Messages)
            {
               var context = new IrcWriteContext
               {
                   Message = message,
                   MessageOrigin = Context.MessageOrigin,
                   User = Context.User
               };
                yield return context;
            }
        }
    }
}
