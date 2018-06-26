using RabbitMQ.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Bus.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册RabbitMQBus
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString">RabbitMQ连接字符串（例：amqp://guest:guest@172.0.0.1:5672/）</param>
        /// <param name="actionSetup"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMQBus(this IServiceCollection services, string connectionString, Action<RabbitMQConfig> actionSetup = null)
        {
            if (connectionString.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(connectionString));

            services.TryAddSingleton(sp =>
            {
                var config = new RabbitMQConfig(connectionString);
                actionSetup?.Invoke(config);
                return config;
            });

            services.TryAddSingleton<RabbitMQBusService>();

            services.TryAddSingleton<IRabbitMQBusService>(sp => sp.GetService<RabbitMQBusService>());

            var allhandles = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
                a.GetTypes()
                .Where(t =>
                (!t.IsAbstract)
                && (!t.IsInterface)
                && t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRabbitMQBusHandler<>)))).ToArray();

            var nonGenericHandlerIfaceType = typeof(IRabbitMQBusHandler);
             
            foreach (var handleType in allhandles)
            {

                var genericTypes = handleType.GetInterfaces()
                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRabbitMQBusHandler<>));
                if (!services.Any(x => x.ServiceType == nonGenericHandlerIfaceType && x.ImplementationType == handleType))
                {
                    services.AddScoped(nonGenericHandlerIfaceType, handleType);
                }
                foreach (var genericType in genericTypes)
                {
                    if (services.Any(x => x.ServiceType == genericType && x.ImplementationType == handleType))
                    {
                        continue;
                    }
                    services.AddScoped(genericType, handleType);
                }

            }
            return services;
        }
    }
}
