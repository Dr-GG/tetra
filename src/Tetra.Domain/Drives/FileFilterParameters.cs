using System.Collections.Generic;

namespace Tetra.Domain.Drives
{
    public record FileFilterParameters
    {
        public bool IncludeSubFolders { get; set; } = true;
        public string Root { get; set; } = string.Empty;
        public IEnumerable<string>? IncludeFileExtensions { get; set; } = null;
    }
}
