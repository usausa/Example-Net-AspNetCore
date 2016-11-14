namespace DependencyInjectionExample
{
    using Dapper;

    using DependencyInjectionExample.Infrastructure.Data;
    using DependencyInjectionExample.Infrastructure.Resolver;
    using DependencyInjectionExample.Services;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.ViewComponents;
    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Smart.Resolver;

    public class Startup
    {
        private readonly StandardResolver resolver = new StandardResolver();

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddSingleton<IControllerActivator>(new SmartResolverControllerActivator(resolver));
            services.AddSingleton<IViewComponentActivator>(new SmartResolverViewComponentActivator(resolver));

            // Add application services.
            var connectionStringMaster = Configuration.GetValue<string>("ConnectionString:Master");
            resolver
                .Bind<IConnectionFactory>()
                .ToConstant(new CallbackConnectionFactory(() => new SqliteConnection(connectionStringMaster)))
                .Named("Master");
            var connectionStringCharacter = Configuration.GetValue<string>("ConnectionString:Character");
            resolver
                .Bind<IConnectionFactory>()
                .ToConstant(new CallbackConnectionFactory(() => new SqliteConnection(connectionStringCharacter)))
                .Named("Character");

            resolver
                .Bind<MasterService>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument("connectionFactory", kernel => kernel.Get<IConnectionFactory>("Master"));
            resolver
                .Bind<CharacterService>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument("connectionFactory", kernel => kernel.Get<IConnectionFactory>("Character"));

            PrepareMasterDatabase(connectionStringMaster);
            PrepareCharacterDatabase(connectionStringCharacter);
        }

        private static void PrepareMasterDatabase(string connectionString)
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

        private static void PrepareCharacterDatabase(string connectionString)
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
        }
    }
}
