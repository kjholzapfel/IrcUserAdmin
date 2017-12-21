using System.Collections.Generic;
using System.ServiceModel;
using IrcUserAdmin.Slave.Contracts;

namespace IrcUserAdmin.Slave.Service
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = "http://ircuseradmin.net ")]
    public interface IUserAdmin
    {
        [OperationContract]
        SlaveResponse AddUser(WcfUser wcfUser);
        [OperationContract]
        SlaveResponse ChangePassword(WcfUser wcfUser);
        [OperationContract]
        SlaveResponse DeleteUser(string name);
        [OperationContract]
        SlaveResponse ClearDatabase();
        [OperationContract]
        WcfUserList GetUsers();
        [OperationContract]
        SlaveResponse AddHosts(string username, IList<WcfHost> wcfHosts);
        [OperationContract]
        SlaveResponse RemoveHosts(string username, IList<WcfHost> wcfHosts);
    }
}