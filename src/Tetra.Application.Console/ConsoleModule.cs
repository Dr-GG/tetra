using Autofac;
using Tetra.Application.Console.Services.Notifications;
using Tetra.Application.Console.Settings;
using Tetra.Interfaces.Notifications;

namespace Tetra.Application.Console
{
    public class ConsoleModule : Module
    {
        private readonly NotificationSettings _settings;

        public ConsoleModule(NotificationSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterInstance(_settings)
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterType<NotificationHandler>()
                .As<INotificationHandler>()
                .SingleInstance();
        }
    }
}
