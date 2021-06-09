using Autofac;
using MediatR.Extensions.Autofac.DependencyInjection;
using Tetra.Handlers.Services.ErrorLogging;
using Tetra.Interfaces.ErrorLogging;

namespace Tetra.Handlers
{
    public class HandlersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMediatR(typeof(HandlersModule).Assembly);

            builder
                .RegisterType<ErrorLogger>()
                .As<IErrorLogger>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ErrorWriter>()
                .As<IErrorWriter>()
                .InstancePerLifetimeScope();
        }
    }
}
