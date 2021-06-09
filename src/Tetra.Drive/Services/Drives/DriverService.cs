using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExifLib;
using Tetra.Domain.Drives;
using Tetra.Interfaces.Drives;

namespace Tetra.Drive.Services.Drives
{
    public class DriverService : IDriveService
    {
        private const string CopyPrefix = " - Copy ";

        public Task Write(string path, string text)
        {
            File.WriteAllText(path, text);

            return Task.CompletedTask;
        }

        public Task CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return Task.CompletedTask;
        }

        public Task Copy(string source, string destination)
        {
            var suggestedDestination = GetSuggestedPath(source, destination);

            File.Copy(source, suggestedDestination, false);

            return Task.CompletedTask;
        }

        public Task Move(string source, string destination)
        {
            var suggestedDestination = GetSuggestedPath(source, destination);

            File.Move(source, suggestedDestination, false);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> GetFilenames(FileFilterParameters parameters)
        {
            var result = new List<string>();

            GetFilenames(parameters, parameters.Root, result);

            return Task.FromResult<IEnumerable<string>>(result);
        }

        public Task<DateTime?> GetExifDateTaken(string filename)
        {
            using var reader = new ExifReader(filename);

            return reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out var result) 
                ? Task.FromResult<DateTime?>(result) 
                : Task.FromResult<DateTime?>(null);
        }

        public Task<DateTime> GetFileEditedTimestamp(string filename)
        {
            return Task.FromResult(File.GetLastWriteTime(filename));
        }

        private static void GetFilenames(
            FileFilterParameters parameters, 
            string root,
            List<string> output)
        {
            var filenames = GetFilenames(root);
            var filteredFilenames = Filter(parameters, filenames);

            output.AddRange(filteredFilenames);

            if (!parameters.IncludeSubFolders)
            {
                return;
            }

            var folders = GetDirectories(root);

            foreach (var folder in folders)
            {
                GetFilenames(parameters, folder, output);
            }
        }

        private static string GetSuggestedPath(string source, string destination, int invokeCount = 1)
        {
            var filename = Path.GetFileName(source);
            var files = Directory.GetFiles(destination, filename);

            if (!files.Any())
            {
                return $"{destination}{filename}";
            }

            var newFilename = GetCopyFilename(filename, invokeCount + 1);
            var directory = Path.GetDirectoryName(source);
            var newSource = $"{directory}\\{newFilename}";

            return GetSuggestedPath(newSource, destination, invokeCount + 1);
        }

        private static string GetCopyFilename(string filename, int existingCopies)
        {
            var strippedFilename = StripCopyPrefix(filename);
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(strippedFilename);
            var extension = Path.GetExtension(strippedFilename);

            return $"{filenameWithoutExtension}{CopyPrefix}{existingCopies}{extension}";
        }

        private static string StripCopyPrefix(string filename)
        {
            var index = filename.IndexOf(CopyPrefix, StringComparison.Ordinal);

            if (index == -1)
            {
                return filename;
            }

            var preCopyPrefix = filename.Substring(0, index);
            var postCopyPrefix = filename[(index + CopyPrefix.Length)..];

            return $"{preCopyPrefix}{postCopyPrefix}";
        }

        private static IEnumerable<string> GetFilenames(string root)
        {
            return Directory.GetFiles(root, "*", SearchOption.TopDirectoryOnly);
        }

        private static IEnumerable<string> GetDirectories(string root)
        {
            return Directory.GetDirectories(root, "*", SearchOption.TopDirectoryOnly);
        }

        private static IEnumerable<string> Filter(
            FileFilterParameters parameters,
            IEnumerable<string> files)
        {
            if (parameters.IncludeFileExtensions == null)
            {
                return files;
            }

            return files
                .Where(f => parameters.IncludeFileExtensions
                    .Any(e => e
                        .Equals(Path.GetExtension(f), StringComparison.OrdinalIgnoreCase)));
        }
    }
}
