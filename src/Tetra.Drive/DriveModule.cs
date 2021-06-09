using Autofac;
using Tetra.Drive.Services.Drives;
using Tetra.Interfaces.Drives;

namespace Tetra.Drive
{
    public class DriveModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<DriverService>()
                .As<IDriveService>()
                .InstancePerLifetimeScope();
        }
    }
}
