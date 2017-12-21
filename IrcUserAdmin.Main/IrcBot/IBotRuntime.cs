using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcUserAdmin.IrcBot
{
    public interface IBotRuntime : IDisposable
    {
        void Start();
        void Stop();
        void SendMessage(string message);
        IList<WhoInfo> GetWhoInfoList();
        void WriteLine(string operText);
        void SendQueryMessage(string message, string user);
        void ShowStatus();
    }
}