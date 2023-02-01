using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public interface IModelRepositoryDataContext
    {
        Guid RepositoryId { get; set; }
        string RepositoryTypeName { get; set; }
        string RepositoryPartTypeName { get; set; }
        List<string> RepositoryRelatedModelLinks { get; set; }
    }
}
