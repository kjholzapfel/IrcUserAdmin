using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IrcUserAdmin.ConfigSettings;
using IrcUserAdmin.Tools;
using Meebey.SmartIrc4net;

namespace IrcUserAdmin.IrcBot
{
    public class BotRuntime : IBotRuntime
    {
        private readonly IIrcEventHandler _ircEventHandler;
        private readonly IBotConfig _botConfig;
        private IrcClient _irc;
        private Thread _thread;
        private volatile bool _isStopping;
        private volatile bool _isConnecting;

        public BotRuntime(IIrcEventHandler ircEventHandler, IBotConfig botConfig)
        {
            _ircEventHandler = ircEventHandler ?? throw new ArgumentNullException(nameof(ircEventHandler));
            _botConfig = botConfig ?? throw new ArgumentNullException(nameof(botConfig));
        }

        public void Start()
        {
            _thread = new Thread(StartThread) {Name = "Irc bot thread."};
            _thread.Start();
        }

        private void StartThread()
        {
            _irc = new IrcClient
            {
                UseSsl = _botConfig.BotSettings.SSL,
                ValidateServerCertificate = false,
                AutoJoinOnInvite = false,
                AutoReconnect = true,
                AutoRelogin = true,
                AutoRetryLimit = 3,
                AutoRetryDelay = 10,
                AutoRetry = true
            };
            RegisterEventHandlers();
            Connect();
        }

        public void Connect()
        {
            try
            {
                _isConnecting = true;
                _irc.Connect(_botConfig.BotSettings.Server, _botConfig.BotSettings.Port);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Irc connection exception: {0}.", exp.Message);
                if (!_isStopping)
                {
                    Connect();
                }
            }
            finally
            {
                _isConnecting = false;
            }
        }
        
        public void Stop()
        {
            //stop comes in from a different thread 
            _isStopping = true;
            while (_isConnecting)
            {
                // IRC client has no way of aborting connecting, we must wait for it to finish before it can be closed gracefully.
            }

            UnRegisterEventHandlers();
            if (_irc.IsConnected)
            {
                _irc.Listen(false);
                _irc.Disconnect();
            }
            while (_irc.IsConnected)
            {
                // block the thread to allow irc to be gracefully disconnected
            }
            while(_thread.IsAlive)
            {
                //wait for the thread to finish
            }
        }

        public void SendMessage(string message)
        {
            _irc.SendMessage(SendType.Message, _botConfig.BotSettings.OperChannel, message);
        }

        public void SendQueryMessage(string message, string user)
        {
            _irc.SendMessage(SendType.Message, user, message);
        }

        public void ShowStatus()
        {
            bool isConnect = _irc.IsConnected;
            Console.WriteLine("Is connected: {0}.", isConnect);
        }

        public IList<WhoInfo> GetWhoInfoList()
        {
            return _irc.GetWhoList("*");
        }

        public void WriteLine(string operText)
        {
            _irc.WriteLine(operText);
        }

        private void OnQueryNotice(object sender, IrcEventArgs e)
        {
            string message = e.Data.Message;
            string[] msgparts = message.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (msgparts.Length > 7)
            {
                string connectpart = msgparts[1];
                var withOutSpecialCharacters = new string(connectpart.Where(char.IsLetterOrDigit).ToArray());
                var noticeType = withOutSpecialCharacters.ParseEnum<NoticeTypes>();
                if (noticeType != null)
                {
                    Task.Factory.StartNew(() => _ircEventHandler.HandleQueryNotice(e));
                }
            }
        }

        private void OnQueryMessage(object sender, IrcEventArgs e)
        {
            Task.Factory.StartNew(() => _ircEventHandler.HandleQueryMessage(e));
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected.");
            if (!_isStopping)
            {
                Connect();
            }
        }

        private void OnChannelMessage(object sender, IrcEventArgs e)
        {
            Console.WriteLine(e.Data.Type + ":");
            Console.WriteLine("(" + e.Data.Channel + ") <" +
            e.Data.Nick + "> " + e.Data.Message);
            string commandPreFix = e.Data.MessageArray[0];
            string[] msgarray = e.Data.Message.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (!string.IsNullOrEmpty(commandPreFix))
            {
                if (commandPreFix.ToLowerInvariant() == _botConfig.BotSettings.IrcBotName.ToLowerInvariant())
                {
                    if (msgarray.Length < 2)
                    {
                        _irc.SendMessage(SendType.Message, _botConfig.BotSettings.OperChannel, "No command given, use help for more info.");
                    }
                    else
                    {
                        Task.Factory.StartNew(() => _ircEventHandler.HandleChannelMessage(e)); 
                    }
                }
            }
        }

        private void OnConnecting(object sender, EventArgs e)
        {
            Console.WriteLine("Connecting to {0}.", _botConfig.BotSettings.Server);
        }

        private void OnConnected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected.");
            _irc.Login(_botConfig.BotSettings.IrcBotName, _botConfig.BotSettings.IrcBotName, 0, _botConfig.BotSettings.IrcBotName, _botConfig.BotSettings.Password);
            _irc.RfcOper(_botConfig.BotSettings.OperUser, _botConfig.BotSettings.OperPassword, Priority.High);
            _irc.WriteLine(Rfc2812.Join(_botConfig.BotSettings.OperChannel));
            Task.Factory.StartNew(DelayedCommands);
            _irc.Listen(true);
        }

        //slight delay in the oper/net lines command after connect is necessary with inspircd.
        private void DelayedCommands()
        {
            Thread.Sleep(3000);
            if (_irc.IsConnected)
            {
                _irc.WriteLine($"MODE {_irc.Nickname} +s +CcAa", Priority.Low);
            }
        }

        private void RegisterEventHandlers()
        {
            _irc.OnConnected += OnConnected;
            _irc.OnConnecting += OnConnecting;
            _irc.OnChannelMessage += OnChannelMessage;
            _irc.OnDisconnected += OnDisconnected;
            _irc.OnQueryMessage += OnQueryMessage;
            _irc.OnQueryNotice += OnQueryNotice;
        }

        private void UnRegisterEventHandlers()
        {
            _irc.OnConnected -= OnConnected;
            _irc.OnConnecting -= OnConnecting;
            _irc.OnChannelMessage -= OnChannelMessage;
            _irc.OnDisconnected -= OnDisconnected;
            _irc.OnQueryMessage -= OnQueryMessage;
            _irc.OnQueryNotice -= OnQueryNotice;
        }

        public void Dispose()
        {
            if (_irc != null)
            {
                UnRegisterEventHandlers();
            }
        }
    }
}