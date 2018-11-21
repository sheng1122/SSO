using DA.DataAccesses;
using DA.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using SSOClient.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace SSOClient.Helpers
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
        private static List<SSOProvider> _preMemorySSOProviders { get; set; }

        public static void PreSetConfigInMemory(Config config, List<SSOProvider> ssoProviders)
        {
            _preMemoryConfig = config;
            _preMemorySSOProviders = ssoProviders;
        }

        private class Key
        {
            public const string DBConfig = "DBConfig";
            public const string DBSSOProviders = "DBSSOProviders";
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

        public static List<SSOProvider> DBSSOProviders
        {
            get
            {
                if (_preMemorySSOProviders != null)
                {
                    _cache.Set(Key.DBSSOProviders, _preMemorySSOProviders);

                    _preMemorySSOProviders = null;
                }

                List<SSOProvider> ssoProviders = _cache.Get<List<SSOProvider>>(Key.DBSSOProviders);

                return ssoProviders;
            }
        }

        public static void RefreshConfig()
        {
            ConfigDA da = new ConfigDA(AppConfig.AppDbConn);
            Config config = da.GetConfig<Config>(AppName);

            SSOProviderDA ssoProviderDA = new SSOProviderDA(AppConfig.AppDbConn);
            var ssoProviders = ssoProviderDA.GetSSOProviders();

            PreSetConfigInMemory(config, ssoProviders);
        }

        #endregion

        #region info from memory

        public static Exception LastException { get; set; }

        #endregion
    }
}
