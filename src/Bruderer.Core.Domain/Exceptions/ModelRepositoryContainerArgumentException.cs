using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Exceptions
{
    [Serializable]
    public class ModelRepositoryContainerArgumentException : ArgumentException
    {
        public const string DefaultMessage = "Repository container [{0}] in model [{1}] could not be found for the following arguments: {2}";

        public ModelRepositoryContainerArgumentException(string modelName, string repositoryTypeName, List<string> arguments)
            : base(string.Format(DefaultMessage, repositoryTypeName, modelName, string.Join(",", arguments)))
        {
            InjectData(modelName, repositoryTypeName, arguments);
        }

        private void InjectData(string modelName, string repositoryTypeName, List<string> arguments)
        {
            Data.Add("ModelName", modelName);
            Data.Add("RepositoryTypeName", repositoryTypeName);

            var argumentCount = 0;
            arguments
                .ForEach(argument => Data.Add($"Argument_{argumentCount++}", argument));
        }
    }
}
