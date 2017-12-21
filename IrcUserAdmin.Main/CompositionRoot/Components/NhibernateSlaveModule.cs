using System;
using System.Reflection;
using Autofac;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using IrcUserAdmin.Common.NHibernate;
using IrcUserAdmin.ConfigSettings;
using IrcUserAdmin.ConfigSettings.ConfigClasses;
using IrcUserAdmin.Entities.Slave;
using IrcUserAdmin.SlavePersistance;
using IrcUserAdmin.Slaves;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Module = Autofac.Module;

namespace IrcUserAdmin.CompositionRoot.Components
{
    public class NhibernateSlaveModule : Module
    {
        private readonly IBotConfig _config;

        public NhibernateSlaveModule(IBotConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");
            _config = config;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblyEntities = typeof(SlaveUser).Assembly;

            for (int index = 0; index < _config.Settings.NHSlaves.NHSlave.Count; index++)
            {
                var slave = _config.Settings.NHSlaves.NHSlave[index];
                var cfg = BuildConfiguration(slave, assemblyEntities);

                var persistenceModel = BuildPersistenceModel();
                persistenceModel.Configure(cfg);

                var sessionFactory = BuildSessionFactory(cfg);

                RegisterComponents(builder, cfg, sessionFactory, index);
            }
        }

        private Configuration BuildConfiguration(NhibernateSettings settings, Assembly assemblyEntities)
        {
            var config =
                Fluently.Configure()
                    .Database(GetPersistenceConfigurer(settings))
                    .Mappings(map => map.FluentMappings.AddFromAssembly(assemblyEntities));
            if (settings.ReCreateDatabase)
            {
                config.ExposeConfiguration(GetCreateSchema);
            }
            else
            {
                config.ExposeConfiguration(GetSchema);
            }
            Configuration buildConfig = config.BuildConfiguration();
            if (buildConfig == null)
                throw new Exception("Cannot build NHibernate configuration");

            return buildConfig;
        }
        private static IPersistenceConfigurer GetPersistenceConfigurer(NhibernateSettings nhibernateSettings)
        {
            IPersistenceConfigurer config;
            switch (nhibernateSettings.DbType)
            {
                case DbType.Postgres:
                    config = GetPostgresConfiguration(nhibernateSettings);
                    break;
                case DbType.SqlLite:
                    config = GetSqlLiteConfiguration(nhibernateSettings);
                    break;
                case DbType.Mysql:
                    config = GetMysqlConfiguration(nhibernateSettings);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return config;
        }

        private static PostgreSQLConfiguration GetPostgresConfiguration(NhibernateSettings nhibernateSettings)
        {
            PostgreSQLConfiguration config =
                            PostgreSQLConfiguration.PostgreSQL82
                            .ConnectionString(nhibernateSettings.ConnectionString)
                            .DefaultSchema(nhibernateSettings.DBSchema);
            if (nhibernateSettings.ShowSQL) config.ShowSql();
            return config;
        }

        private static SQLiteConfiguration GetSqlLiteConfiguration(NhibernateSettings nhibernateSettings)
        {
            SQLiteConfiguration config =
                           SQLiteConfiguration.Standard
                           .ConnectionString(nhibernateSettings.ConnectionString)
                           .DefaultSchema(nhibernateSettings.DBSchema);
            if (nhibernateSettings.ShowSQL) config.ShowSql();
            return config;
        }

        private static MySQLConfiguration GetMysqlConfiguration(NhibernateSettings nhibernateSettings)
        {
            MySQLConfiguration config =
               MySQLConfiguration.Standard
                               .ConnectionString(nhibernateSettings.ConnectionString)
                               .DefaultSchema(nhibernateSettings.DBSchema);
            if (nhibernateSettings.ShowSQL) config.ShowSql();
            return config;
        }

        private static AutoPersistenceModel BuildPersistenceModel()
        {
            var persistenceModel = new AutoPersistenceModel();
            return persistenceModel;
        }

        private static ISessionFactory BuildSessionFactory(Configuration config)
        {
            var sessionFactory = config.BuildSessionFactory();

            if (sessionFactory == null)
                throw new Exception("Cannot build NHibernate Session Factory");

            return sessionFactory;
        }

        private static void RegisterComponents(ContainerBuilder builder, Configuration config, ISessionFactory sessionFactory, int key)
        {
            key++;
            builder.RegisterInstance(config).Keyed<Configuration>(key).SingleInstance();
            builder.RegisterType<ExecutionContextFactory>().SingleInstance();
            builder.RegisterInstance(sessionFactory).Keyed<ISessionFactory>(key).SingleInstance();
            builder.RegisterType<UnitOfWork>().Keyed<IUnitOfWork>(key).InstancePerLifetimeScope()
                .WithParameter((pi, c) => pi.ParameterType == typeof(ISessionFactory),
                               (pi, c) => c.ResolveKeyed<ISessionFactory>(key));
            builder.RegisterType<SlavePersistence>().Keyed<ISlavePersistence>(key).InstancePerLifetimeScope()
                .WithParameter((pi, c) => pi.ParameterType == typeof(IUnitOfWork),
                               (pi, c) => c.ResolveKeyed<IUnitOfWork>(key)); 
        }

        private static void GetSchema(Configuration config)
        {
            new SchemaUpdate(config).Execute(false, true);
        }

        private static void GetCreateSchema(Configuration config)
        {
            new SchemaExport(config).Drop(true, true);
            new SchemaExport(config).Create(true, true);
        }
    }
}