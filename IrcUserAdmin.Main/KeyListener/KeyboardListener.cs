using System;
using System.Collections.Concurrent;
using System.Threading;
using IrcUserAdmin.ConfigSettings;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.Quartz;

namespace IrcUserAdmin.KeyListener
{
    public class KeyboardListener : IKeyboardListener
    {
        private readonly IBotRuntime _runtime;
        private readonly IDbBootstrap _bootstrap;
        private readonly IQuartzScheduler _quartzScheduler;
        private readonly object _lockObj;
        private readonly BlockingCollection<ConsoleKeyInfo> _consoleKeyInfos;
        private Thread _thread;
        private bool _running;

        public KeyboardListener(IBotRuntime runtime, IDbBootstrap bootstrap, IQuartzScheduler quartzScheduler)
        {
            if (runtime == null) throw new ArgumentNullException("runtime");
            if (bootstrap == null) throw new ArgumentNullException("bootstrap");
            if (quartzScheduler == null) throw new ArgumentNullException("quartzScheduler");
            _lockObj = new object();
            _consoleKeyInfos = new BlockingCollection<ConsoleKeyInfo>();
            _runtime = runtime;
            _bootstrap = bootstrap;
            _quartzScheduler = quartzScheduler;
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
                //stop the scheduler and bot runtime:
                try
                {
                    _runtime.Stop();
                    _quartzScheduler.StopScheduler();
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
                case ConsoleKey.I:
                    InsertDbUsers();
                    break;
                case ConsoleKey.S:
                    _runtime.ShowStatus();
                    break;
                case ConsoleKey.H:
                    WriteHelpToConsoleOutput();
                    break;
                case ConsoleKey.T:
                    TriggerDailyJob();
                    break;
                default:
                    Console.WriteLine("Press <H> to display help");
                    break;
            }
        }

        private void TriggerDailyJob()
        {
            _quartzScheduler.FireTrigger(QuartzJobs.DailyJob);
        }

        private void InsertDbUsers()
        {
            _bootstrap.BootStrapInitialUsers();
        }

        private static void WriteHelpToConsoleOutput()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("<I> Insert standard user(s) from XML configuration");
            Console.WriteLine("<T> Trigger Daily cron job.");
            Console.WriteLine("<ESC> Exit application");
        }
    }
}
