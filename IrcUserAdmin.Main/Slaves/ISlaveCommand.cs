using IrcUserAdmin.SlavePersistance;

namespace IrcUserAdmin.Slaves
{
    public interface ISlaveCommand
    {
        void Execute(ISlavePersistence slaveDao);
    }
}