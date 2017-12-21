using IrcUserAdmin.Slave.Autofac;

namespace IrcUserAdmin.Slave
{
    class Program
    {
        static void Main(string[] args)
        {
            AutofacCompositionRoot.InitAutofac();
        }
    }
}
