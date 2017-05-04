namespace DependencyInjectionExample
{
    using System;

    using Dapper;

    using DependencyInjectionExample.Infrastructure.Data;
    using DependencyInjectionExample.Infrastructure.Resolver;
    using DependencyInjectionExample.Services;
    using DependencyInjectionExample.Settings;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.ViewComponents;
    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Smart.Resolver;

    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("assistance", new Info { Title = "Example API", Version = "v1" });
                options.DescribeAllEnumsAsStrings();    // Enum
            });

            // Replace activator.
            services.AddSingleton<IControllerActivator, SmartResolverControllerActivator>();
            services.AddSingleton<IViewComponentActivator, SmartResolverViewComponentActivator>();

            // Settings
            ConfigureSettings(services);

            // Add application services.
            var config = new ResolverConfig();
            SetupComponents(config);

            // Use custom service provider.
            return SmartResolverHelper.BuildServiceProvider(config, services);
        }

        private void ConfigureSettings(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<ProfileSettings>(Configuration.GetSection("ProfileSettings"));
        }

        private void SetupComponents(ResolverConfig config)
        {
            var connectionStringMaster = Configuration.GetConnectionString("Master");
            config
                .Bind<IConnectionFactory>()
                .ToConstant(new CallbackConnectionFactory(() => new SqliteConnection(connectionStringMaster)))
                .Named("Master");
            var connectionStringCharacter = Configuration.GetConnectionString("Character");
            config
                .Bind<IConnectionFactory>()
                .ToConstant(new CallbackConnectionFactory(() => new SqliteConnection(connectionStringCharacter)))
                .Named("Character");

            config
                .Bind<MasterService>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument("connectionFactory", kernel => kernel.Get<IConnectionFactory>("Master"));
            config
                .Bind<CharacterService>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument("connectionFactory", kernel => kernel.Get<IConnectionFactory>("Character"));

            config
                .Bind<MetricsManager>()
                .ToSelf()
                .InSingletonScope();

            config
                .Bind<ScopedObject>()
                .ToSelf()
                .InRequestScope();

            // Prepare daabase
            SetupMasterDatabase(connectionStringMaster);
            SetupCharacterDatabase(connectionStringCharacter);
        }

        private static void SetupMasterDatabase(string connectionString)
        {
            using (var con = new SqliteConnection(connectionString))
            {
                con.Execute("CREATE TABLE IF NOT EXISTS Item (Id int PRIMARY KEY, Name text, Price int)");
                con.Execute("DELETE FROM Item");
                con.Execute("INSERT INTO Item (Id, Name, Price) VALUES (1, 'Item-1', 100)");
                con.Execute("INSERT INTO Item (Id, Name, Price) VALUES (2, 'Item-2', 200)");
                con.Execute("INSERT INTO Item (Id, Name, Price) VALUES (3, 'Item-3', 300)");
            }
        }

        private static void SetupCharacterDatabase(string connectionString)
        {
            using (var con = new SqliteConnection(connectionString))
            {
                con.Execute("CREATE TABLE IF NOT EXISTS Character (Id int PRIMARY KEY, Name text, Level int)");
                con.Execute("DELETE FROM Character");
                con.Execute("INSERT INTO Character (Id, Name, Level) VALUES (1, 'Character-1', 43)");
                con.Execute("INSERT INTO Character (Id, Name, Level) VALUES (2, 'Character-2', 65)");
                con.Execute("INSERT INTO Character (Id, Name, Level) VALUES (3, 'Character-3', 27)");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/assistance/swagger.json", "Example API");
            });
        }
    }
}
