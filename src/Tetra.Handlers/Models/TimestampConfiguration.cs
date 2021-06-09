using System.Text.RegularExpressions;

namespace Tetra.Handlers.Models
{
    public record TimestampConfiguration
    {
        public string DateTimeFormat { get; set; } = string.Empty;
        public Regex RegexPattern { get; set; } = null!;
    }
}
