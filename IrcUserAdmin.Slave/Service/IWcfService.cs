using System;

namespace IrcUserAdmin.Slave.Service
{
    public interface IWcfService : IDisposable
    {
        void Run();
        void Stop();
    }
}