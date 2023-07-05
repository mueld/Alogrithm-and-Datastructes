using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2
{
    public interface IModelScannerFactory
    {
        IModelScanner BuildScanner();
        IModelScanner BuildScanner(ModelScanningProps scanningProps);
        IModelScanner BuildScanner(ModelScanningProps scanningProps, ITraversalCondition traversalCondition);
        IModelScanner BuildScanner(Handler.IModelScannerHandler chainOfResponsibilities);
        IModelScanner BuildScanner(Handler.IRecursiveModelScannerHandler chainOfResponsibilities);
        IModelScanner BuildScanner(Handler.IModelScannerHandler chainOfResponsibilities, ITraversalCondition traversalCondition);
        IModelScanner BuildScanner(Handler.IRecursiveModelScannerHandler chainOfResponsibilities, ITraversalCondition traversalCondition);
    }
}
