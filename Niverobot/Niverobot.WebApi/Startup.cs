using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Niverobot.Domain.EfModels;
using Niverobot.WebApi.Interfaces;
using Niverobot.WebApi.Services;

namespace Niverobot.WebApi
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
            services.AddControllers();

            services.AddMvc().AddNewtonsoftJson();


            services.AddSingleton<ITelegramBotService, TelegramBotService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton(Configuration);

            services.AddTransient<IDadJokeService, DadJokeService>();
            services.AddTransient<IReminderService, ReminderService>();
            services.AddTransient<ITelegramUpdateService, TelegramUpdateService>();

            services.Configure<BotConfiguration>(Configuration.GetSection("BotConfiguration"));

            services.AddDbContext<NiveroBotContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer")));
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
