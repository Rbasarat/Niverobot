using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Niverobot.Domain;
using Niverobot.Domain.EfModels;
using Niverobot.Interfaces;
using Niverobot.Services;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Serilog;

namespace Niverobot.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMvc().AddNewtonsoftJson();

            services.AddScoped<IDadJokeService, DadJokeService>();
            services.AddSingleton(Configuration);
            
            services.Configure<BotConfiguration>(Configuration.GetSection("BotConfiguration"));

            services.AddDbContext<NiveroBotContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("SqlServer"), mySqlOptions => mySqlOptions
                    // replace with your Server Version and Type
                    .ServerVersion(new Version(8, 0, 18), ServerType.MySql)
                ));
            
            // // The following line enables Application Insights telemetry collection.
            services.AddApplicationInsightsTelemetry();
            
            services.AddInternalServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, NiveroBotContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseSerilogRequestLogging();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            // Apply migrations on startup.
            context.Database.Migrate();
        }
    }
}
