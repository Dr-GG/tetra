using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tetra.Domain.Drives;

namespace Tetra.Interfaces.Drives
{
    public interface IDriveService
    {
        Task Write(string path, string text);
        Task CreateFolder(string path);
        Task Copy(string source, string destination);
        Task Move(string source, string destination);
        Task<IEnumerable<string>> GetFilenames(FileFilterParameters parameters);
        Task<DateTime?> GetExifDateTaken(string filename);
        Task<DateTime> GetFileEditedTimestamp(string filename);
    }
}
