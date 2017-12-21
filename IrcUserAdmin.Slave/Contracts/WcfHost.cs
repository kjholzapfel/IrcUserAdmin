using System.Collections;
using System.Runtime.Serialization;

namespace IrcUserAdmin.Slave.Contracts
{
    [DataContract(Name = "Host")]
    public class WcfHost
    {
        [DataMember]
        public string HostAdress { get; set; }
    }
}