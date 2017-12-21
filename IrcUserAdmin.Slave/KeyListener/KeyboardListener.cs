using System;
using System.Collections.Concurrent;
using System.Threading;
using IrcUserAdmin.Slave.Service;

namespace IrcUserAdmin.Slave.KeyListener
{
    public class KeyboardListener : IKeyboardListener
    {
        private readonly IWcfService _wcfService;
        private readonly object _lockObj;
        private readonly BlockingCollection<ConsoleKeyInfo> _consoleKeyInfos;
        private Thread _thread;
        private bool _running;

        public KeyboardListener(IWcfService wcfService)
        {
            if (wcfService == null) throw new ArgumentNullException("wcfService");
            _wcfService = wcfService;
            _lockObj = new object();
            _consoleKeyInfos = new BlockingCollection<ConsoleKeyInfo>();
        }

        public void Start()
        {
            lock (_lockObj)
            {
                WriteHelpToConsoleOutput();
                _running = true;
                _thread = new Thread(() =>
                {
                    while (_running)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (!_consoleKeyInfos.IsCompleted)
                        {
                            _consoleKeyInfos.Add(key);
                        }
                    }
                }) { IsBackground = true };

                _thread.Start();
                //block the main thread
                BlockWaitForKey();
            }
        }

        public void Stop()
        {
            lock (_lockObj)
            {
                //unblock the blocking collection, allowing the main thread to terminate 
                _consoleKeyInfos.CompleteAdding();
                //stop the background task:
                _running = false;
                //stop the wcf service
                try
                {
                    _wcfService.Stop();
                }
                catch (Exception exp)
                {
                    //write exception to console
                    Console.Write(exp);
                    Console.ReadLine();
                }
                // the main thread should end here
            }
        }

        private void BlockWaitForKey()
        {
            foreach (ConsoleKeyInfo value in _consoleKeyInfos.GetConsumingEnumerable())
            {
                HandleKey(value);
            }
        }

        private void HandleKey(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    Stop();
                    break;
                case ConsoleKey.H:
                    WriteHelpToConsoleOutput();
                    break;
                default:
                    Console.WriteLine("Press <H> to display help");
                    break;
            }
        }


        private static void WriteHelpToConsoleOutput()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("<ESC> Exit application");
        }
    }
}
