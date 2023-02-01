using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public class ModelRepositoryDataContext : IModelRepositoryDataContext
    {
        #region ctor

        public ModelRepositoryDataContext() { }

        public ModelRepositoryDataContext(IRepositoryModelContainer sourceContainer)
        {
            RepositoryId = sourceContainer.RepositoryId;
            RepositoryTypeName = sourceContainer.RepositoryTypeName;
            RepositoryPartTypeName = sourceContainer.RepositoryPartTypeName;
            RepositoryRelatedModelLinks.AddRange(sourceContainer.RepositoryRelatedModelLinks);
            RepositoryRelatedModelLinks = new List<string>(RepositoryRelatedModelLinks.Distinct());
        }

        #endregion
        #region props

        public Guid RepositoryId { get; set; } = Guid.Empty;
        public string RepositoryTypeName { get; set; } = string.Empty;
        public string RepositoryPartTypeName { get; set; } = string.Empty;
        public List<string> RepositoryRelatedModelLinks { get; set; } = new();

        #endregion
    }
}
