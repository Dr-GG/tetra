using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tetra.Domain.ErrorLogs;
using Tetra.Interfaces.ErrorLogging;

namespace Tetra.Handlers.Services.ErrorLogging
{
    public class ErrorLogger : IErrorLogger
    {
        private readonly ICollection<ErrorLog> _errorLogs = new List<ErrorLog>();

        public Task LogError(string filename, Exception error)
        {
            var errorLog = new ErrorLog
            {
                Filename = filename,
                ErrorMessage = error.ToString()
            };

            _errorLogs.Add(errorLog);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<ErrorLog>> Export()
        {
            return Task.FromResult<IEnumerable<ErrorLog>>(
                _errorLogs.ToList());
        }
    }
}
