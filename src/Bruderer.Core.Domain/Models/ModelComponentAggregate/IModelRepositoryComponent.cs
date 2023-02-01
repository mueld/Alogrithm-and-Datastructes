using System;

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate
{
    public interface IModelRepositoryComponent
    {
        bool IsRepositoryClient { get; set; }
        string RepositoryTypeName { get; set; }
        string RepositoryPartTypeName { get; set; }
        Guid RepositoryId { get; set; }
        string RepositoryLink { get; set; }
    }
}
