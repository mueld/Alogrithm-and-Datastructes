using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2
{
    public interface IModelScanner
    {
        void Scan(object modelComponent);
        public List<IServiceModelContainer> ServiceModelContainers { get;  }
        public List<IRepositoryModelContainer> RepositoryModelContainers { get; } 
        public List<IModelComponentContainerCollection> EnumerableModelContainers { get;} 

        public ModelComponentContainer ModelComponentTree { get;  }
        public List<IModelVariable> Variables { get; } 
        public List<IModelRPC> RPCs { get; }
    }
}
