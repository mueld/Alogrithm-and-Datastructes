using Bruderer.Core.Domain.Models.ModelAggregate;
using System;
using System.Text;

namespace Bruderer.Core.Domain.Exceptions
{
    [Serializable]
    public class ModelComponentScanningException : InvalidOperationException
    {
        public const string DefaultMessage = "Error scanning model components.";

        public ModelComponentScanningException(string contextName, ModelComponentScanner modelComponentScanner)
            : base(DefaultMessage)
        {
            InjectData(contextName, modelComponentScanner);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            return $"{DefaultMessage} {Data.ToString()}";
        }

        private void InjectData(string contextName, ModelComponentScanner modelComponentScanner)
        {
            Data.Add("ContextName", contextName);
            var errorCount = 0;
            var warningCount = 0;

            modelComponentScanner.ErrorMessageStack
                .ForEach(message => Data.Add($"ModelComponentScanner_ERROR_{errorCount++}", message));
            modelComponentScanner.WarningMessageStack
                .ForEach(message => Data.Add($"ModelComponentScanner_WARNING_{warningCount++}", message));
        }

        
    }
}
