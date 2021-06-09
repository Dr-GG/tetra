using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tetra.Domain.Commands;
using Tetra.Domain.Drives;
using Tetra.Domain.Enums;
using Tetra.Domain.ErrorLogs;
using Tetra.Domain.Notifications;
using Tetra.Handlers.Utilities;
using Tetra.Interfaces.Drives;
using Tetra.Interfaces.ErrorLogging;
using Tetra.Interfaces.Notifications;

namespace Tetra.Handlers.CommandHandlers
{
    public class ArrangeFilesCommandHandler : 
        IRequestHandler<ArrangeFilesCommand, IEnumerable<ErrorLog>>
    {
        private readonly IDriveService _driveService;
        private readonly INotificationHandler _notificationHandler;
        private readonly IErrorLogger _errorLogger;

        private static readonly IEnumerable<string> ExifFileExtensions = new[]
        {
            ".jpg",
            ".jpeg"
        };

        public ArrangeFilesCommandHandler(
            IDriveService driveService, 
            INotificationHandler notificationHandler, 
            IErrorLogger errorLogger)
        {
            _driveService = driveService;
            _notificationHandler = notificationHandler;
            _errorLogger = errorLogger;
        }

        public async Task<IEnumerable<ErrorLog>> Handle(
            ArrangeFilesCommand request, 
            CancellationToken cancellationToken)
        {
            var filenames = await GetFilenames(request);
            var notification = new NotificationEvent(NotificationEventType.FinishedProcessing);
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = request.MaximumThreadSize
            };

            Parallel.ForEach(filenames, options, async filename => await ProcessFile(request, filename));

            await _notificationHandler.Notify(notification);

            return await _errorLogger.Export();
        }

        private async Task ProcessFile(ArrangeFilesCommand request, string filename)
        {
            try
            {
                var timestamp = await GetDateTime(request, filename);
                var destinationFolder = timestamp.ToString(request.DestinationPattern);
                var destination = $"{request.DestinationRoot}{destinationFolder}\\";

                await _driveService.CreateFolder(destination);

                if (request.MoveFiles)
                {
                    await _driveService.Move(filename, destination);
                }
                else
                {
                    await _driveService.Copy(filename, destination);
                }
            }
            catch (Exception error)
            {
                await _errorLogger.LogError(filename, error);
            }

            var notification = new NotificationEvent(NotificationEventType.ProcessedFile);

            await _notificationHandler.Notify(notification);
        }

        private async Task<DateTime?> GetExifDateTimeTaken(string filename)
        {
            var extension = Path.GetExtension(filename).ToLower();

            if (!ExifFileExtensions.Contains(extension))
            {
                return null;
            }

            try
            {
                return await _driveService.GetExifDateTaken(filename);
            }
            catch
            {
                return null;
            }
        }

        private async Task<DateTime> GetDateTime(ArrangeFilesCommand request, string filename)
        {
            var timestamp = DateTimeUtilities.ExtractDateTimeFromFilename(filename);

            if (timestamp != null && IsDateTakenWithinRange(request, timestamp.Value))
            {
                return timestamp.Value;
            }

            timestamp = await GetExifDateTimeTaken(filename);

            if (timestamp == null)
            {
                return await _driveService.GetFileEditedTimestamp(filename);
            }

            return timestamp.Value;
        }

        private static bool IsDateTakenWithinRange(ArrangeFilesCommand request, DateTime timestamp)
        {
            var maximumDate = request.MaximumAllowedDateTaken ?? GetEndOfToday();
            var minimumDate = request.MinimumAllowedDateTaken;

            return timestamp >= minimumDate && timestamp <= maximumDate;
        }

        private static DateTime GetEndOfToday()
        {
            return DateTime.Today.Add(TimeSpan.FromDays(1));
        }

        private async Task<IEnumerable<string>> GetFilenames(ArrangeFilesCommand request)
        {
            var notification = new NotificationEvent(NotificationEventType.GatheringFiles);
            var parameters = GetFileFilterParameters(request);
            var result = new List<string>();

            await _notificationHandler.Notify(notification);

            foreach (var parameter in parameters)
            {
                result.AddRange(
                    await _driveService.GetFilenames(parameter));
            }

            notification = new NotificationEvent(
                NotificationEventType.StartingProcessing, result.Count);

            await _notificationHandler.Notify(notification);

            return result;
        }

        private static IEnumerable<FileFilterParameters> GetFileFilterParameters(
            ArrangeFilesCommand request)
        {
            return request.SourceRoots
                .Select(r => new FileFilterParameters
                {
                    Root = r,
                    IncludeFileExtensions = request.FilterSourceFileExtensions,
                    IncludeSubFolders = request.IncludeSourceSubFolders
                });
        }
    }
}
