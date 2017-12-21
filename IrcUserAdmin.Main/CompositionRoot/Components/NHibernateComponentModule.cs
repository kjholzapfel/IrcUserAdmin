using System;
using System.Reflection;
using Autofac;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using IrcUserAdmin.Common.NHibernate;
using IrcUserAdmin.ConfigSettings;
using IrcUserAdmin.ConfigSettings.ConfigClasses;
using IrcUserAdmin.NHibernate;
using IrcUserAdmin.Slaves;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Module = Autofac.Module;

namespace IrcUserAdmin.CompositionRoot.Components
{
    public class NHibernateComponentModule : Module
    {
        private readonly IBotConfig _botconfig;

        public NHibernateComponentModule(IBotConfig botconfig)
        {
            if (botconfig == null) throw new ArgumentNullException("botconfig");
            _botconfig = botconfig;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblyEntities = typeof (Entities.User).Assembly;

            var cfg = BuildConfiguration(_botconfig, assemblyEntities);

            var persistenceModel = BuildPersistenceModel();
            persistenceModel.Configure(cfg);

            var sessionFactory = BuildSessionFactory(cfg);

            RegisterComponents(builder, cfg, sessionFactory);
        }

        private Configuration BuildConfiguration(IBotConfig botconfig, Assembly assemblyEntities)
        {
            var config =
                Fluently.Configure()
                    .Database(GetPersistenceConfigurer(botconfig.Settings.NHSettings))
                    .Mappings(map => map.FluentMappings.AddFromAssembly(assemblyEntities));
            if (botconfig.Settings.NHSettings.ReCreateDatabase)
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

        private IPersistenceConfigurer GetPersistenceConfigurer(NhibernateSettings nhibernateSettings)
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

        private PostgreSQLConfiguration GetPostgresConfiguration(NhibernateSettings nhibernateSettings)
        {
            PostgreSQLConfiguration config = 
                            PostgreSQLConfiguration.PostgreSQL82
                            .ConnectionString(nhibernateSettings.ConnectionString)
                            .DefaultSchema(nhibernateSettings.DBSchema);
            if (nhibernateSettings.ShowSQL) config.ShowSql();
            return config;
        }

        private SQLiteConfiguration GetSqlLiteConfiguration(NhibernateSettings nhibernateSettings)
        {
            SQLiteConfiguration config =
                           SQLiteConfiguration.Standard
                           .ConnectionString(nhibernateSettings.ConnectionString)
                           .DefaultSchema(nhibernateSettings.DBSchema);
            if (nhibernateSettings.ShowSQL) config.ShowSql();
            return config;
        }

        private MySQLConfiguration GetMysqlConfiguration(NhibernateSettings nhibernateSettings)
        {
            MySQLConfiguration config =
               MySQLConfiguration.Standard
                               .ConnectionString(nhibernateSettings.ConnectionString)
                               .DefaultSchema(nhibernateSettings.DBSchema);
            if (nhibernateSettings.ShowSQL) config.ShowSql();
            return config;
        }

        private AutoPersistenceModel BuildPersistenceModel()
        {
            var persistenceModel = new AutoPersistenceModel();
            return persistenceModel;
        }

        private ISessionFactory BuildSessionFactory(Configuration config)
        {
            var sessionFactory = config.BuildSessionFactory();

            if (sessionFactory == null)
                throw new Exception("Cannot build NHibernate Session Factory");

            return sessionFactory;
        }

        private void RegisterComponents(ContainerBuilder builder, Configuration config, ISessionFactory sessionFactory)
        {
            builder.RegisterInstance(config).As<Configuration>().SingleInstance();
            builder.RegisterInstance(sessionFactory).As<ISessionFactory>().SingleInstance();
            builder.Register(x => new UnitOfWork(x.Resolve<ISessionFactory>())).As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<UserDao>().As<IUserDao>().InstancePerLifetimeScope();
            builder.RegisterType<Persistence>().As<IPersistance>().InstancePerLifetimeScope();
            builder.RegisterType<SlaveExecutor>().As<ISlaveExecutor>().InstancePerLifetimeScope();
        }

        private void GetSchema(Configuration config)
        {
            new SchemaUpdate(config).Execute(false, true);
        }

        private void GetCreateSchema(Configuration config)
        {
            new SchemaExport(config).Drop(true, true);
            new SchemaExport(config).Create(true, true);
        }
    }
}