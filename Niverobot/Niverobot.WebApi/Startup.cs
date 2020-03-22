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

            //services.AddDbContextPool<NiveroBotContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("SqlServer")));

            services.AddMvc().AddNewtonsoftJson();

            services.AddScoped<ITelegramUpdateService, TelegramUpdateService>();

            services.AddSingleton<ITelegramBotService, TelegramBotService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton(Configuration);

            services.AddTransient<IDadJokeService, DadJokeService>();
            services.AddTransient<IReminderService, ReminderService>();

            services.Configure<BotConfiguration>(Configuration.GetSection("BotConfiguration"));



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
