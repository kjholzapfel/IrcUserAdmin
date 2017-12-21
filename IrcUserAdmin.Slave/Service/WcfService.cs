using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using Autofac;
using Autofac.Integration.Wcf;

namespace IrcUserAdmin.Slave.Service
{
    public class WcfService : IWcfService
    {
        private readonly ILifetimeScope _scope;
        private ServiceHost _host;

        public WcfService(ILifetimeScope scope)
        {
            if (scope == null) throw new ArgumentNullException("scope");
            _scope = scope;
        }

        public void Run()
        {
            var baseAddress = new Uri("https://localhost:8080/hello");

            _host = new ServiceHost(typeof (UserAdmin), baseAddress);

            var smb = new ServiceMetadataBehavior
            {
                HttpsGetEnabled = true,
                MetadataExporter = {PolicyVersion = PolicyVersion.Policy15}
            };
            _host.Description.Behaviors.Add(smb);
            var mexBinding = MetadataExchangeBindings.CreateMexHttpsBinding();
            _host.AddServiceEndpoint(
                ServiceMetadataBehavior.MexContractName,
                mexBinding,
                "mex"
                );
            _host.Credentials.ServiceCertificate.Certificate = new X509Certificate2("c:\\temp\\TempCA.pfx");
            var binding = new BasicHttpBinding
            {
                Security =
                {
                    Mode = BasicHttpSecurityMode.Transport,
                }
            };

            _host.AddServiceEndpoint(typeof (IUserAdmin), binding, "");
            _host.Authorization.ServiceAuthorizationManager = new WcfServiceAuthorizationManager();
            _host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
            _host.AddDependencyInjectionBehavior<IUserAdmin>(_scope);
            _host.Open();
        }

        public void Stop()
        {
            _host.Close();
        }

        public void Dispose()
        {
            if (_host != null)
            {
                var disposableObj = _host as IDisposable;
                disposableObj.Dispose(); 
            }
        }
    }
}