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
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer")));
            
            services.AddInternalServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
