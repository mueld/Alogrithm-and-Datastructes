using System;

namespace Bruderer.Core.Domain.Exceptions
{
    [Serializable]
    public class CountMismatchException : ArgumentException
    {
        public const string DefaultMessage = "The number of elements from two passed lists does not match.";

        public CountMismatchException(string sourceContext, int sourceCount, string targetContext, int targetCount)
            : base(DefaultMessage)
        {
            InjectData(sourceContext, sourceCount, targetContext, targetCount);
        }

        public CountMismatchException(string sourceContext, int sourceCount, string targetContext, int targetCount, Exception inner)
            : base(DefaultMessage, inner)
        {
            InjectData(sourceContext, sourceCount, targetContext, targetCount);
        }

        private void InjectData(string sourceContext, int sourceCount, string targetContext, int targetCount)
        {
            Data.Add("SourceContext", sourceContext);
            Data.Add("SourceCount", sourceCount);
            Data.Add("TargetContext", targetContext);
            Data.Add("TargetCount", targetCount);
        }
    }
}
