using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using IrcUserAdmin.Commands.OperCommands;
using Meebey.SmartIrc4net;

namespace IrcUserAdmin.IrcBot
{
    public class IrcReadWriteExchange : IIrcReadWriteExchange
    {
        private readonly IBotRuntime _botRuntime;
        private readonly ConcurrentQueue<string> _queue;
        private const int QueueDepth = 3;
        public IrcReadWriteExchange(IBotRuntime botRuntime)
        {
            if (botRuntime == null) throw new ArgumentNullException("botRuntime");
            _queue = new ConcurrentQueue<string>();
            _botRuntime = botRuntime;
        }

        public void Write(IEnumerable<IrcWriteContext> context)
        {
            foreach (var ctx in context)
            {
                switch (ctx.MessageOrigin)
                {
                    case MessageOrigin.Channel:
                        _botRuntime.SendMessage(ctx.Message);
                        break;
                    case MessageOrigin.Query:
                        _botRuntime.SendQueryMessage(ctx.Message, ctx.User);
                        break;
                    case MessageOrigin.Notice:
                        if (!_queue.Contains(ctx.Message))
                        {
                            _botRuntime.SendMessage(ctx.Message);
                        }
                        _queue.Enqueue(ctx.Message);
                        while (_queue.Count > QueueDepth)
                        {
                            string temp;
                            _queue.TryDequeue(out temp);
                        }
                        break;
                    case MessageOrigin.Quartz:
                        _botRuntime.SendMessage(ctx.Message);
                        break;
                }
            }
        }

        public void ExecuteOperCommands(IEnumerable<IOperCommand> operCommands)
        {
            foreach (var operCommand in operCommands)
            {
                var operText = operCommand.GetOperText();
                _botRuntime.WriteLine(operText);
            }
        }

        public IList<WhoInfo> GetWhoList()
        {
            return _botRuntime.GetWhoInfoList();
        }
    }
}