using System;
using System.Reflection;
using Autofac;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using IrcUserAdmin.Common.NHibernate;
using IrcUserAdmin.Entities.Slave;
using IrcUserAdmin.Slave.ConfigurationUtils;
using IrcUserAdmin.SlavePersistance;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Configuration = NHibernate.Cfg.Configuration;
using Module = Autofac.Module;

namespace IrcUserAdmin.Slave.Autofac
{
    public class NHibernateComponentModule : Module
    {
        private readonly bool _recreateDb;
        private readonly string _dbType;
        private readonly bool _showSql;
        private readonly string _connectionString;
        private readonly string _nhibernateSchema;

        public NHibernateComponentModule()
        {
            _recreateDb = ConfigMethods.ReadConfigBoolean("NhibernateRecreateDb");
            _dbType = ConfigMethods.ReadConfigString("NhibernateDatabaseType");
            _showSql = ConfigMethods.ReadConfigBoolean("NhibernateShowSql");
            _connectionString = ConfigMethods.GetConnectionString();
            _nhibernateSchema = ConfigMethods.ReadConfigString("NhibernateSchema");
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblyEntities = typeof (SlaveUser).Assembly;

            var cfg = BuildConfiguration(assemblyEntities);

            var persistenceModel = BuildPersistenceModel();
            persistenceModel.Configure(cfg);

            var sessionFactory = BuildSessionFactory(cfg);

            RegisterComponents(builder, cfg, sessionFactory);
        }

        private Configuration BuildConfiguration(Assembly assemblyEntities)
        {
            var config =
                Fluently.Configure()
                    .Database(GetPersistenceConfigurer())
                    .Mappings(map => map.FluentMappings.AddFromAssembly(assemblyEntities));
            if (_recreateDb)
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

        private IPersistenceConfigurer GetPersistenceConfigurer()
        {
            IPersistenceConfigurer config;

            switch (_dbType.ToLowerInvariant())
            {
                case "postgres":
                    config = GetPostgresConfiguration();
                    break;
                case "sqllite":
                    config = GetSqlLiteConfiguration();
                    break;
                case "mysql":
                    config = GetMysqlConfiguration();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return config;
        }

        private PostgreSQLConfiguration GetPostgresConfiguration()
        {
            PostgreSQLConfiguration config = 
                            PostgreSQLConfiguration.PostgreSQL82
                            .ConnectionString(_connectionString)
                            .DefaultSchema(_nhibernateSchema);
            if (_showSql) config.ShowSql();
            return config;
        }

        private SQLiteConfiguration GetSqlLiteConfiguration()
        {
            SQLiteConfiguration config =
                           SQLiteConfiguration.Standard
                           .ConnectionString(_connectionString)
                           .DefaultSchema(_nhibernateSchema);
            if (_showSql) config.ShowSql();
            return config;
        }

        private MySQLConfiguration GetMysqlConfiguration()
        {
            MySQLConfiguration config =
               MySQLConfiguration.Standard
                               .ConnectionString(_connectionString)
                               .DefaultSchema(_nhibernateSchema);
            if (_showSql) config.ShowSql();
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
            builder.RegisterType<SlavePersistence>().As<ISlavePersistence>().InstancePerLifetimeScope();
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