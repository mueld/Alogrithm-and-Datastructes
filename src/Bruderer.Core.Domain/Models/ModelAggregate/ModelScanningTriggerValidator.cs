using Bruderer.Core.Domain.Constants;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public class ModelScanningTriggerValidator
    {
        private readonly List<string> _currentSplittedPath = new();
        private string _currentServiceName = "";
        private readonly List<string> _currentSplittedRepositoryName = new();

        public bool CheckNestedContainer { get; set; }
        public bool ResetContainer { get; set; }
        public bool Breakpoint { get; set; }

        public static bool CheckMachineNameSpace(List<List<string>> keys, List<string> currentNameSpace)
        {
            var result = false;
            keys.ForEach(key =>
            {
                if (key.Contains(currentNameSpace[0]))
                {
                    result = true;
                }
            });

            return result;
        }

        public void CheckTiggers(ModelScanningTrigger triggers,
            string currentPath,
            string currentRepositoryName,
            string currentServiceName
            )
        {

            CheckNestedContainer = false;
            ResetContainer = false;
            Breakpoint = false;

            if (triggers.HasTriggers)
            {
                SetData(currentPath, currentRepositoryName, currentServiceName);
                CheckModelKeyTriggers(triggers);
                CheckRepositorTriggers(triggers);
                CheckServiceNames(triggers);
            }
        }

        private void SetData(string currentPath, string currentRepositoryName, string currentServiceName)
        {
            _currentSplittedPath.Clear();
            _currentSplittedRepositoryName.Clear();

            if (currentPath == null)
            {
                return;
            }

            foreach (var s in currentPath.Split(StringConstants.Separator))
            {
                if (!string.IsNullOrEmpty(s))
                    _currentSplittedPath.Add(s);
            }

            foreach (var s in currentRepositoryName.Split(StringConstants.Separator))
            {
                if (!string.IsNullOrEmpty(s))
                    _currentSplittedRepositoryName.Add(s);
            }

            this._currentServiceName = currentServiceName;
        }

        private void CheckServiceNames(ModelScanningTrigger triggers)
        {
            if (triggers.HasServiceNameTriggers)
            {
                ResetContainer = false;
                CheckNestedContainer = true;
                foreach (var key in triggers.ServiceNames)
                    if (_currentServiceName.Equals(key))
                    {
                        ResetContainer = true;
                        CheckNestedContainer = false;
                    }
            }
        }

        private void CheckRepositorTriggers(ModelScanningTrigger triggers)
        {
            if (triggers.HasRepositoryNameTriggers)
            {
                var splittedModelKeyList = triggers.GetSplitedRepositoryNames();

                if (_currentSplittedRepositoryName.Count == 1)
                {
                    if (CheckMachineNameSpace(splittedModelKeyList, _currentSplittedRepositoryName))
                    {
                        CheckNestedContainer = true;
                        return;
                    }
                    else
                    {
                        Breakpoint = true;
                        return;
                    }
                }

                CheckPrefix(splittedModelKeyList, _currentSplittedRepositoryName);
                CheckSuffix(splittedModelKeyList, _currentSplittedRepositoryName);
            }
        }

        private void CheckModelKeyTriggers(ModelScanningTrigger triggers)
        {
            if (triggers.HasModelKeyTriggers)
            {
                var splittedModelKeyList = triggers.GetSplitedModelKeys();

                if (_currentSplittedPath.Count == 1)
                {
                    if (CheckMachineNameSpace(splittedModelKeyList, _currentSplittedPath))
                    {
                        CheckNestedContainer = true;
                        return;
                    }
                    else
                    {
                        Breakpoint = true;
                        return;
                    }
                }
                CheckPrefix(splittedModelKeyList, _currentSplittedPath);
                CheckSuffix(splittedModelKeyList, _currentSplittedPath);
            }
        }

        /// <summary>
        /// Checks whether the current namespace occurs completely in the passed keys as long as the passed namespace is smaller then the keys.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private void CheckPrefix(List<List<string>> keys, List<string> currentNamespace)
        {
            keys.ForEach(key =>
            {
                if (currentNamespace.Count < key.Count)
                {
                    if (currentNamespace.TrueForAll(path => key.Contains(path)))
                    {
                        CheckNestedContainer = true;
                        Breakpoint = false;
                    }
                    else
                    {
                        if (!CheckNestedContainer)
                            Breakpoint = true;
                    }
                }
            });
        }

        private void CheckSuffix(List<List<string>> keys, List<string> currentNamespace)
        {
            foreach (var key in keys)
            {
                if (currentNamespace.Count == key.Count)
                {
                    if (key.TrueForAll(k => currentNamespace.Contains(k)))
                    {
                        ResetContainer = true;
                        Breakpoint = false;
                    }
                    else
                    {
                        if (!ResetContainer && !CheckNestedContainer)
                            Breakpoint = true;
                    }
                }
                else if (currentNamespace.Count > key.Count)
                {
                    if (key.TrueForAll(k => currentNamespace.Contains(k)))
                    {
                        ResetContainer = true;
                        Breakpoint = false;
                    }
                    else
                    {
                        if (!ResetContainer && !CheckNestedContainer)
                            Breakpoint = true;
                    }
                }
            }
        }
    }
}
