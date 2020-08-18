using Microsoft.Extensions.DependencyInjection;
using Niverobot.Interfaces;
using Niverobot.Services;

namespace Niverobot.Telegrambot
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInternalServices(this IServiceCollection services)
        {
            services.AddSingleton<ITelegramBotService, TelegramBotService>();
            services.AddScoped<IMessageService, MessageService>();  
            services.AddScoped<IReminderService, ReminderService>();
            services.AddScoped<IGRPCService, GrpcService>();
            return services;
        }
    }
}