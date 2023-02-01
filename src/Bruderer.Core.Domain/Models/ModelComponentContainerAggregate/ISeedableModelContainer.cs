namespace Bruderer.Core.Domain.Models.ModelComponentContainerAggregate
{
    public interface ISeedableModelContainer : IModelComponentContainer
    {
        void DataSeeding();
    }
}
