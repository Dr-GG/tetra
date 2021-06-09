using Autofac;
using Microsoft.Extensions.Configuration;
using Tetra.Application.Console.Extensions;
using Tetra.Application.Console.Settings;
using Tetra.Drive;
using Tetra.Handlers;

namespace Tetra.Application.Console.Bootstrap
{
    public static class FileArrangerBootstrapper
    {
        public static IContainer Bootstrap()
        {
            var configuration = GetConfiguration();
            var builder = new ContainerBuilder();

            AddModules(configuration, builder);
            AddLaunchSettings(configuration, builder);

            return builder.Build();
        }

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private static void AddLaunchSettings(
            IConfiguration configuration,
            ContainerBuilder container)
        {
            var arrangeSettings = configuration.BindSettings<ArrangeSettings>("Launch");

            container
                .RegisterInstance(arrangeSettings)
                .AsSelf()
                .SingleInstance();
        }

        private static void AddModules(
            IConfiguration configuration, 
            ContainerBuilder container)
        {
            var notificationSettings = configuration.BindSettings<NotificationSettings>("Notification");
            var consoleModule = new ConsoleModule(notificationSettings);
            var driveModule = new DriveModule();
            var handlersModule = new HandlersModule();

            container.RegisterModule(consoleModule);
            container.RegisterModule(driveModule);
            container.RegisterModule(handlersModule);
        }
    }
}
