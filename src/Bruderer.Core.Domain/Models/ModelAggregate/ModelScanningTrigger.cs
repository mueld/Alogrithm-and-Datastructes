using Bruderer.Core.Domain.Constants;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public class ModelScanningTrigger
    {
        public bool HasTriggers
        {
            get
            {
                return HasModelKeyTriggers | HasRepositoryNameTriggers | HasServiceNameTriggers;
            }
        }

        public bool HasModelKeyTriggers
        {
            get
            {
                return ModelKeys.Count > 0;
            }
        }
        public bool HasRepositoryNameTriggers
        {
            get
            {
                return RepositoryNames.Count > 0;
            }
        }
        public bool HasServiceNameTriggers
        {
            get
            {
                return ServiceNames.Count > 0;
            }
        }

        public List<string> ModelKeys { get; set; } = new();
        public List<string> RepositoryNames { get; set; } = new();
        public List<string> ServiceNames { get; set; } = new();

        public ModelTriggerContext TriggerContext { get; set; } = new();

        public List<List<string>> GetSplitedModelKeys()
        {
            return GetSplittedtKey(ModelKeys);
        }
        public List<List<string>> GetSplitedRepositoryNames()
        {
            return GetSplittedtKey(RepositoryNames);
        }

        private static List<List<string>> GetSplittedtKey(List<string> keys)
        {
            var result = new List<List<string>>();
            keys.ForEach(key =>
            {
                var splittedKeys = new List<string>();
                var splitedKey = key.Split(StringConstants.Separator);
                for (var index = 0; index < splitedKey.Length; index++)
                {
                    if (!string.IsNullOrEmpty(splitedKey[index]))
                    {
                        splittedKeys.Add(splitedKey[index]);
                    }
                }
                result.Add(splittedKeys);
            });
            return result;
        }
    }

    public class ModelTriggerContext
    {
        public Guid RepositoryIdContext { get; set; } = Guid.Empty;
        public string RepositoryPartNameContext { get; set; } = string.Empty;
        public string RepositoryNameContext { get; set; } = string.Empty;
        public string ServiceNameContext { get; set; } = string.Empty;
    }
}
