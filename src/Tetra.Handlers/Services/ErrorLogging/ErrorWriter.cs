using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Tetra.Domain.ErrorLogs;
using Tetra.Interfaces.Drives;
using Tetra.Interfaces.ErrorLogging;

namespace Tetra.Handlers.Services.ErrorLogging
{
    public class ErrorWriter : IErrorWriter
    {
        private readonly IDriveService _driveService;

        public ErrorWriter(IDriveService driveService)
        {
            _driveService = driveService;
        }

        public async Task<string> Write(string outputPath, IEnumerable<ErrorLog> errorLog)
        {
            var json = JsonSerializer.Serialize(errorLog);
            var timestamp = DateTime.Now.ToString("yyyymmdd-hhmmss");
            var filename = $"error-log-{timestamp}.json";
            var path = $"{outputPath}{filename}";

            await _driveService.Write(path, json);

            return path;
        }
    }
}
