using DA.DataAccesses;
using DA.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using SSOAuthServer.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace SSOAuthServerHelpers
{

    public class AppConfig
    {
        private static IServiceProvider _services;
        private const string UnexpectedServiceResponse = "Unexpected service response";
        private static readonly string _configFile = "appsettings.json";
        private static IConfigurationRoot _config;
        private static IDistributedCache _cache;

        public static IServiceProvider Service
        {
            get { return _services; }
            set
            {
                if (_services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }

                _services = value;

                _cache = Service.GetService(typeof(IDistributedCache)) as IDistributedCache;
            }

        }

        private static IConfigurationRoot Config
        {
            get
            {
                if (_config == null)
                {
                    var builder = new ConfigurationBuilder();
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile(_configFile);

                    _config = builder.Build();
                }

                return _config;
            }
        }

        #region config from file

        public static string AppName
        {
            get
            {
                return Config.Get<string>("App:Name");
            }
        }

        public static string AppDbConn
        {
            get
            {
                return Config.Get<string>("DB:App:ConnStr");
            }
        }
        #endregion

        #region config from db to cache
        private static Config _preMemoryConfig { get; set; }
        private static List<SSOClient> _preMemorySSOClients { get; set; }

        public static void PreSetConfigInMemory(Config config, List<SSOClient> ssoClients)
        {
            _preMemoryConfig = config;
            _preMemorySSOClients = ssoClients;
        }

        private class Key
        {
            public const string DBConfig = "DBConfig";
            public const string DBSSOClients = "DBSSOClients";
        }

        public static Config DBConfig
        {
            get
            {
                if (_preMemoryConfig != null)
                {
                    _cache.Set(Key.DBConfig, _preMemoryConfig);

                    _preMemoryConfig = null;
                }

                Config config = _cache.Get<Config>(Key.DBConfig);

                return config;
            }
        }

        public static List<SSOClient> DBSSOClients
        {
            get
            {
                if (_preMemorySSOClients != null)
                {
                    _cache.Set(Key.DBSSOClients, _preMemorySSOClients);

                    _preMemorySSOClients = null;
                }

                List<SSOClient> ssoClients = _cache.Get<List<SSOClient>>(Key.DBSSOClients);

                return ssoClients;
            }
        }

        public static void RefreshConfig()
        {
            ConfigDA da = new ConfigDA(AppConfig.AppDbConn);
            Config config = da.GetConfig<Config>(AppName);

            SSOClientDA ssoClientDA = new SSOClientDA(AppConfig.AppDbConn);
            var ssoClients = ssoClientDA.GetSSOClients();

            PreSetConfigInMemory(config, ssoClients);
        }

        #endregion

        #region info from memory

        public static Exception LastException { get; set; }

        #endregion
    }
}
