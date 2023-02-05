using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
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
        public bool TraverseContainerModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            if(elementProperty.Name.Equals("ParentModelContainer"))
            {
                return false;
            }
            return true;
        }

        public bool TraverseModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable)
        {
            throw new NotImplementedException();
        }

        public bool TraverseServiceContainer(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
