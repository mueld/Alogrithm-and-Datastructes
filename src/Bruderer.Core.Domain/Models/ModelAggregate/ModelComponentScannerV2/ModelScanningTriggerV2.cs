using Bruderer.Core.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2
{
    public class ModelScanningTriggerV2
    {
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
}
