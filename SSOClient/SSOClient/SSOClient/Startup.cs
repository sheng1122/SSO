using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;
using SSOClient.Helpers;
using DA.DataAccesses;
using DA.Models;

namespace SSOClient
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
            //set password validation
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
            });

            //get app config from database
            ConfigDA da = new ConfigDA(AppConfig.AppDbConn);

            var config = da.GetConfig<Config>(AppConfig.AppName);

            AppConfig.PreSetConfigInMemory(config);
            
            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
            
            services.AddSession(options =>
            {
                options.Cookie.Name = config.CookieName;
                options.IdleTimeout = TimeSpan.FromSeconds(config.WebSessionIdleTimeout);
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSession();
            if (env.IsDevelopment() && false)
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            UserSession.Service = app.ApplicationServices;
            AppConfig.Service = app.ApplicationServices;

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "register",
                   template: "register",
                   defaults: new { controller = "Account", action = "Register" }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
