using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Tetra.Application.Console.Bootstrap;
using Tetra.Application.Console.Settings;
using Tetra.Domain.Commands;
using Tetra.Domain.ErrorLogs;
using Tetra.Interfaces.ErrorLogging;

namespace Tetra.Application.Console
{
    public static class Program
    {
        private static ArrangeFilesCommand GetCommandFromLaunchSettings(IComponentContext scope)
        {
            var launchSettings = scope.Resolve<ArrangeSettings>();

            return new ArrangeFilesCommand
            {
                MaximumThreadSize = launchSettings.MaximumThreadSize,
                DestinationPattern = launchSettings.DestinationPattern,
                DestinationRoot = launchSettings.Destination,
                FilterSourceFileExtensions = launchSettings.FilterSourceFileExtensions,
                SourceRoots = launchSettings.Sources,
                IncludeSourceSubFolders = launchSettings.IncludeSubFolders,
                MoveFiles = launchSettings.MoveFiles,
                MinimumAllowedDateTaken = launchSettings.MinimumAllowedDateTaken,
                MaximumAllowedDateTaken = launchSettings.MaximumAllowedDateTaken
            };
        }

        private static async Task WriteErrors(IComponentContext scope, IEnumerable<ErrorLog> errorLogs)
        {
            var enumeratedErrorLogs = errorLogs.ToList();

            if (!enumeratedErrorLogs.Any())
            {
                return;
            }

            var launchSettings = scope.Resolve<ArrangeSettings>();
            var errorWriter = scope.Resolve<IErrorWriter>();
            var output = await errorWriter.Write(launchSettings.ErrorLogPath, enumeratedErrorLogs);

            System.Console.WriteLine(
                "There were one or more errors during the processing of the files. " + 
                $"Please view the error log at `{output}`");
        }

        private static async Task Main()
        {
            await using var scope = FileArrangerBootstrapper.Bootstrap();
            var mediator = scope.Resolve<IMediator>();
            var command = GetCommandFromLaunchSettings(scope);
            var errorLogs = await mediator.Send(command);

            await WriteErrors(scope, errorLogs);

            System.Console.WriteLine("Done");
            System.Console.Read();
        }
    }
}
