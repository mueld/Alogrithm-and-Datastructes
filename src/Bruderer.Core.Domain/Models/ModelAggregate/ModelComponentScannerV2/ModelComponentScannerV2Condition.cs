using Bruderer.Core.Domain.Constants;
using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2
{
    public class ModelComponentScannerV2Condition : ITraversalCondition
    {
        private ModelScanningTriggerV2 _Trigger;

        public ModelComponentScannerV2Condition(ModelScanningTriggerV2 trigger)
        {
            _Trigger = trigger;
            

        }

       
        public bool TraverseModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            if (_Trigger.HasModelKeyTriggers)
            {
                return CheckModelLinks(container.ModelLink.Key, _Trigger.ModelKeys);
            }
            else if(_Trigger.HasRepositoryNameTriggers)
            {
                return CheckModelLinks(container.RepositoryLink, _Trigger.RepositoryNames);
            }
            return true;
        }

        public bool TraverseModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection collection)
        {
            if (_Trigger.HasModelKeyTriggers)
            {
                return CheckModelLinks(collection.ModelLink.Key, _Trigger.ModelKeys);
            }
            else if (_Trigger.HasRepositoryNameTriggers)
            {
                return CheckModelLinks(collection.RepositoryLink, _Trigger.RepositoryNames);
            }
            return true;
        }

        public bool TraverseModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer item, int index)
        {
            if (_Trigger.HasModelKeyTriggers)
            {
                return CheckModelLinks(item.ModelLink.Key, _Trigger.ModelKeys);
            }
            else if (_Trigger.HasRepositoryNameTriggers)
            {
                return CheckModelLinks(item.RepositoryLink, _Trigger.RepositoryNames);
            }
            return true;
        }

        public bool TraverseServiceContainer(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            if (_Trigger.HasModelKeyTriggers)
            {
                return CheckModelLinks(item.ModelLink.Key, _Trigger.ModelKeys);
            }
            else if (_Trigger.HasRepositoryNameTriggers)
            {
                return CheckModelLinks(item.RepositoryLink, _Trigger.RepositoryNames);
            }
            return true;
        }

        public bool VisitModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            return CheckToVisitContainer(container.ModelLink.Key, container.RepositoryLink);
        }

        public bool VisitModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection collection)
        {
            return CheckToVisitContainer(collection.ModelLink.Key, collection.RepositoryLink);
        }

        public bool VisitModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer item, int index)
        {
            return CheckToVisitContainer(item.ModelLink.Key, item.RepositoryLink);
        }

        public bool VisitModelRPC(PropertyInfo elementProperty, ModelRPCBase rpc)
        {
            throw new NotImplementedException();
        }

        public bool VisitModelVariable(PropertyInfo elementProperty, ModelVariableBase variable)
        {
            if (_Trigger.HasModelKeyTriggers)
            {
                foreach (var link in _Trigger.ModelKeys)
                {
                    if (variable.Equals(link))
                    {
                        return true;
                    }
                }
            }
            else if (_Trigger.HasRepositoryNameTriggers)
            {
                foreach (var link in _Trigger.RepositoryNames)
                {
                    if (variable.Equals(link))
                    {
                        return true;
                    }
                }
            }
        }

        public bool VisitServiceContainer(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            throw new NotImplementedException();
        }


        private bool CheckToVisitContainer(string modelKey, string repositoryLink)
        {
            if (_Trigger.HasModelKeyTriggers)
            {
                foreach (var link in _Trigger.ModelKeys)
                {
                    if (modelKey.Equals(link))
                    {
                        return true;
                    }
                }
            }
            else if (_Trigger.HasRepositoryNameTriggers)
            {
                foreach (var link in _Trigger.RepositoryNames)
                {
                    if (repositoryLink.Equals(link))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckModelLinks(string modelContainerModelLink, List<string> triggers)
        {
            foreach(var link in triggers)
            {
                if (modelContainerModelLink.Length < link.Length)
                {
                    var substring = link.Substring(0, modelContainerModelLink.Length);
                    if (substring.Equals(modelContainerModelLink))
                    {
                        return true;
                    }
                }
                else if(modelContainerModelLink.Length == link.Length)
                {
                    if (modelContainerModelLink.Equals(link))
                    {
                        return true;
                    }
                }
                else
                {
                    var substring = modelContainerModelLink.Substring(0, link.Length);
                    if (substring.Equals(link))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void LeaveModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            throw new NotImplementedException();
        }

        public void LeaveModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection collection)
        {
            throw new NotImplementedException();
        }

        public void LeaveModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer item, int index)
        {
            throw new NotImplementedException();
        }

        public void LeaveServiceContainer(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
