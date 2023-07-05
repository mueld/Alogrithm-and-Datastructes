using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Models.ModelConditionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2
{
    public interface IModelScannerFactory
    {
        IModelScanner BuildScanner();
        IModelScanner BuildScanner(ModelScanningProps scanningProps);
        IModelScanner BuildScanner(ModelScanningProps scanningProps, ITraversalCondition traversalCondition);
        IModelScanner BuildScanner(IModelScannerHandler chainOfResponsibilities);
        IModelScanner BuildScanner(IRecursiveModelScannerHandler chainOfResponsibilities);
        IModelScanner BuildScanner(IModelScannerHandler chainOfResponsibilities, ITraversalCondition traversalCondition);
        IModelScanner BuildScanner(IRecursiveModelScannerHandler chainOfResponsibilities, ITraversalCondition traversalCondition);
    }
}
