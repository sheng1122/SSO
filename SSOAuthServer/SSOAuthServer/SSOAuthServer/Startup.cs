using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DA.DataAccesses;
using DA.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SSOAuthServer.Helpers;
using SSOAuthServerHelpers;

namespace SSOAuthServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //get app config from database
            ConfigDA configDA = new ConfigDA(AppConfig.AppDbConn);
            var config = configDA.GetConfig<Config>(AppConfig.AppName);

            SSOClientDA ssoClientDA = new SSOClientDA(AppConfig.AppDbConn);
            var ssoClients = ssoClientDA.GetSSOClients();

            AppConfig.PreSetConfigInMemory(config, ssoClients);

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.Configure<Models.PagingOptions>(Configuration.GetSection("DefaultPagingOptions"));

            services.AddSession(options =>
            {
                options.Cookie.Name = config.CookieName;
                options.IdleTimeout = TimeSpan.FromSeconds(config.WebSessionIdleTimeout);
            });

            services.AddMvc()
            .AddJsonOptions(opt =>
            {
                // These should be the defaults, but we can be explicit:
                opt.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                opt.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                opt.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSession();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
            }

            UserSession.Service = app.ApplicationServices;
            AppConfig.Service = app.ApplicationServices;

            app.UseStaticFiles();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}");
            });
        }
    }
}
