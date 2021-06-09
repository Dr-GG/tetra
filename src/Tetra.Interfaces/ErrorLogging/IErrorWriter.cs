using System.Collections.Generic;
using System.Threading.Tasks;
using Tetra.Domain.ErrorLogs;

namespace Tetra.Interfaces.ErrorLogging
{
    public interface IErrorWriter
    {
        Task<string> Write(string outputPath, IEnumerable<ErrorLog> errorLog);
    }
}
