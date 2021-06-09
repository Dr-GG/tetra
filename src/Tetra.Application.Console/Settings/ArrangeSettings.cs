using System;
using System.Collections.Generic;

namespace Tetra.Application.Console.Settings
{
    public class ArrangeSettings
    {
        public int MaximumThreadSize { get; set; }
        public bool MoveFiles { get; set; }
        public string ErrorLogPath { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string DestinationPattern { get; set; } = string.Empty;
        public bool IncludeSubFolders { get; set; } = true;
        public IEnumerable<string> Sources { get; set; } = null!;
        public IEnumerable<string>? FilterSourceFileExtensions { get; set; }
        public DateTime MinimumAllowedDateTaken { get; set; }
        public DateTime? MaximumAllowedDateTaken { get; set; }
    }
}
