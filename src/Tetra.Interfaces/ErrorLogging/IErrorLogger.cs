using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tetra.Domain.ErrorLogs;

namespace Tetra.Interfaces.ErrorLogging
{
    public interface IErrorLogger
    {
        Task LogError(string filename, Exception error);
        Task<IEnumerable<ErrorLog>> Export();
    }
}
