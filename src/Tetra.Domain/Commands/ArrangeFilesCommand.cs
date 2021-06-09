using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Tetra.Domain.ErrorLogs;

namespace Tetra.Domain.Commands
{
    public class ArrangeFilesCommand : IRequest<IEnumerable<ErrorLog>>
    {
        public bool MoveFiles { get; set; }
        public int MaximumThreadSize { get; set; }
        public string DestinationRoot { get; set; } = string.Empty;
        public string DestinationPattern { get; set; } = string.Empty;
        public bool IncludeSourceSubFolders { get; set; } = true;
        public IEnumerable<string> SourceRoots { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string>? FilterSourceFileExtensions { get; set; }
        public DateTime MinimumAllowedDateTaken { get; set; }
        public DateTime? MaximumAllowedDateTaken { get; set; }
    }
}
