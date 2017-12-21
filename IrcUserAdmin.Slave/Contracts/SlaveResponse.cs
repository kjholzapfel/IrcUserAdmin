using System.Runtime.Serialization;

namespace IrcUserAdmin.Slave.Contracts
{
    [DataContract]
    public class SlaveResponse
    {
        [DataMember]
        public bool ResponseOk { get; set; }
        [DataMember]
        public string Exception { get; set; }
    }
}