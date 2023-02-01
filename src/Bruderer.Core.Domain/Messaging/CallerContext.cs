using System;

namespace Bruderer.Core.Domain.Messaging
{
    public class CallerContext
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int LineNumber { get; set; } = -1;
    }
}
