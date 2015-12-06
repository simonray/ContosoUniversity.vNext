using ContosoUniversity.DAL;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ContosoUniversity.Website
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC services to the services container.
            services.AddMvc();

            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();

            var configuration = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(configuration);

            var sqlConnectionString = configuration["Data:DefaultConnection:ConnectionString"];
            var useInMemoryDatabase = string.IsNullOrWhiteSpace(sqlConnectionString);

            if (useInMemoryDatabase)
            {
                services.AddEntityFramework()

                    .AddInMemoryDatabase()
                    .AddDbContext<SchoolContext>(options =>
                    {
                        options.UseInMemoryDatabase();
                    });
            }
            else
            {
                services.AddEntityFramework()
                    .AddSqlServer()
                    .AddDbContext<SchoolContext>(options =>
                    {
                        options.UseSqlServer(sqlConnectionString);
                    });
            }

            services.AddTransient<ISchoolContext>(s => s.GetService<SchoolContext>());
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            ActivatorUtilities
                .CreateInstance<SchoolInitializer>(app.ApplicationServices, app.ApplicationServices.GetService<SchoolContext>())
                .Seed();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
