using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Niverobot.Domain;
using Niverobot.Domain.EfModels;
using Niverobot.Interfaces;
using Niverobot.Services;

namespace Niverobot.WebApi
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInternalServices(this IServiceCollection services)
        {
            services.AddSingleton<ITelegramBotService, TelegramBotService>();
            services.AddScoped<IMessageService, MessageService>();  
            services.AddScoped<IReminderService, ReminderService>();
            services.AddTransient<ITelegramUpdateService, TelegramUpdateService>();
            services.AddScoped<IGRPCService, GrpcService>();
            return services;
        }
    }
}