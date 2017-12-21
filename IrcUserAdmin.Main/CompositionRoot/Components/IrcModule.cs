using Autofac;
using IrcUserAdmin.Commands;
using IrcUserAdmin.Commands.Common;
using IrcUserAdmin.Commands.Modules;
using IrcUserAdmin.IrcBot;

namespace IrcUserAdmin.CompositionRoot.Components
{
    public class IrcModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IrcCommands>().As<IIrcCommands>();
            builder.RegisterType<IrcEventHandler>().As<IIrcEventHandler>();
            builder.RegisterType<IrcReadWriteExchange>().As<IIrcReadWriteExchange>();
            builder.RegisterType<Help>().As<IIrcCommand>();
            builder.RegisterType<AddUser>().As<IIrcCommand>();
            builder.RegisterType<AddHost>().As<IIrcCommand>();
            builder.RegisterType<Announce>().As<IIrcCommand>();
            builder.RegisterType<AddGroup>().As<IIrcCommand>();
            builder.RegisterType<AddGroupChannel>().As<IIrcCommand>();
            builder.RegisterType<AddUserAsAdmin>().As<IIrcCommand>();
            builder.RegisterType<AddUserToGroup>().As<IIrcCommand>();
            builder.RegisterType<ChangePassword>().As<IIrcCommand>();
            builder.RegisterType<Connect>().As<IIrcCommand>();
            builder.RegisterType<DelUser>().As<IIrcCommand>();
            builder.RegisterType<DeleteKillUser>().As<IIrcCommand>();
            builder.RegisterType<DeleteGroup>().As<IIrcCommand>();
            builder.RegisterType<IrcSecuritySweep>().As<IIrcCommand>();
            builder.RegisterType<JoinChannels>().As<IIrcCommand>();
            builder.RegisterType<KillUser>().As<IIrcCommand>();
            builder.RegisterType<SaMode>().As<IIrcCommand>();
            builder.RegisterType<SetAutoJoin>().As<IIrcCommand>();
            builder.RegisterType<SetHost>().As<IIrcCommand>();
            builder.RegisterType<SetVHost>().As<IIrcCommand>();
            builder.RegisterType<ShowUsers>().As<IIrcCommand>();
            builder.RegisterType<ShowOnlineUsers>().As<IIrcCommand>();
            builder.RegisterType<ShowGroups>().As<IIrcCommand>();
            builder.RegisterType<ShowGroupDetails>().As<IIrcCommand>();
            builder.RegisterType<RemoveHost>().As<IIrcCommand>();
            builder.RegisterType<RemoveUserAsAdmin>().As<IIrcCommand>();
            builder.RegisterType<RemoveUserFromGroup>().As<IIrcCommand>();
            builder.RegisterType<UserInfo>().As<IIrcCommand>();
            builder.RegisterType<UserDisplay>();
        }
    }
}