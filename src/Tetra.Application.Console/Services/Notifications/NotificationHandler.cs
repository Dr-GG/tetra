using System;
using System.Threading.Tasks;
using Tetra.Application.Console.Settings;
using Tetra.Domain.Enums;
using Tetra.Domain.Notifications;
using Tetra.Interfaces.Notifications;

namespace Tetra.Application.Console.Services.Notifications
{
    public class NotificationHandler : INotificationHandler
    {
        private const string PercentageFormat = "D2";

        private readonly object _lock = new();
        private readonly NotificationSettings _settings;

        private string _padPattern = string.Empty;
        private long _numberOfFiles;
        private long _fileCounter;

        public NotificationHandler(NotificationSettings settings)
        {
            _settings = settings;
        }

        public Task Notify(NotificationEvent notification)
        {
            switch (notification.Type)
            {
                case NotificationEventType.GatheringFiles:
                    GatheringFiles();
                    break;

                case NotificationEventType.StartingProcessing:
                    StartingProcessing(notification);
                    break;

                case NotificationEventType.ProcessedFile:
                    FileProcessed();
                    break;

                case NotificationEventType.FinishedProcessing:
                    Done();
                    break;

                default:
                    throw new NotSupportedException($"Could not support notification event type {notification.Type}");
            }

            return Task.CompletedTask;
        }

        private void GatheringFiles()
        {
            Write("Gathering files...");
        }

        private void StartingProcessing(NotificationEvent notificationEvent)
        {
            _numberOfFiles = notificationEvent.Value;

            WriteLine("DONE!");
            WriteLine("");

            var log = Math.Log10(_numberOfFiles);
            var numberOfDigits = (int) (Math.Floor(log) + 1);

            _padPattern = $"D{numberOfDigits}";
        }

        private void FileProcessed()
        {
            bool writeToConsole;
            long fileCounter;

            lock (_lock)
            {
                fileCounter = ++_fileCounter;
                writeToConsole = _fileCounter % _settings.ProcessRefreshInterval == 0;
            }

            if (!writeToConsole)
            {
                return;
            }

            var percentage = _numberOfFiles == 0
                ? 0
                : (int)(fileCounter / (float) _numberOfFiles * 100.0f);
            var text = $"Processed {fileCounter.ToString(_padPattern)} of {_numberOfFiles} [{percentage.ToString(PercentageFormat)}%]";

            WriteLine(text);
        }

        private void Done()
        {
            WriteLine("");
            WriteLine($"Successfully processed {_numberOfFiles} files");
        }

        private void Write(string format, ConsoleColor color = ConsoleColor.White)
        {
            lock (_lock)
            {
                System.Console.Write(format, color);
            }
        }

        private void WriteLine(string format, ConsoleColor color = ConsoleColor.White)
        {
            lock (_lock)
            {
                System.Console.ForegroundColor = color;
                System.Console.WriteLine(format);
            }
        }
    }
}
