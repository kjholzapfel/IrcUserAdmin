using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IrcUserAdmin.Slave.Contracts
{
    [DataContract(Name = "User")]
    public class WcfUser
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public IList<WcfHost> Hosts { get; set; }
    }
}