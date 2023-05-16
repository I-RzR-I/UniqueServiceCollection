using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;
using TestServiceCollection.Abstractions;
using TestServiceCollection.Services;
using UniqueServiceCollection;

namespace TestServiceCollection
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create service collection
            var serviceCollection = new ServiceCollection();
            
            serviceCollection.AddUnique<IServiceOne, ServiceOne>();
            serviceCollection.AddUnique<IServiceOne, ServiceOne>(ServiceLifetime.Scoped);

            ConfigureServices(serviceCollection);

            // Create service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = serviceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger<Program>();


            logger.LogDebug("Application started");


        }

        static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add logging
            serviceCollection.AddSingleton<ILoggerFactory, LoggerFactory>();
            serviceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            serviceCollection.AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .AddDebug()
                .SetMinimumLevel(LogLevel.Debug));

            serviceCollection.AddScoped<IServiceOne, ServiceOne>();
            serviceCollection.AddScoped<IServiceTwo, ServiceTwo>();
            serviceCollection.AddScoped<IServiceOne, ServiceOne>();
            serviceCollection.AddUnique<IServiceOne, ServiceOne>();
            serviceCollection.AddUnique<IServiceOne, ServiceOne>(ServiceLifetime.Scoped);

            serviceCollection.CheckAndCleanUpDuplicateService<IServiceOne>();
        }
    }
}
