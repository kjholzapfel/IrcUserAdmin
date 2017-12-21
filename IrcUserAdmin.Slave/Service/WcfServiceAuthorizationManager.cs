using System.ServiceModel;
using System.ServiceModel.Channels;

namespace IrcUserAdmin.Slave.Service
{
    public class WcfServiceAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            try
            {
                var remoteEndpoint = operationContext.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (remoteEndpoint != null && remoteEndpoint.Address == "127.0.0.1")
                {
                    return true;
                }
                return false;

            }
            catch 
            {
                return false;
            }
        }
    }
}