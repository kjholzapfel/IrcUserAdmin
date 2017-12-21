using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IrcUserAdmin.Slave.Contracts
{
    [DataContract(Name = "UserList")]
    public class WcfUserList
    {
        public IList<WcfUser> Users { get; set; }
        public bool ResponseOk { get; set; }
        public string Exception { get; set; }
    }
}