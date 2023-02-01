using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public class ModelRepositoryInfo
    {
        #region ctor

        public ModelRepositoryInfo() { }

        public ModelRepositoryInfo(IRepositoryModelContainer sourceContainer)
        {
            RepositoryId = sourceContainer.RepositoryId;
            RepositoryTypeName = sourceContainer.RepositoryTypeName;
            RepositoryPartTypeNames.Add(sourceContainer.RepositoryPartTypeName);
            RepositoryPartTypeNames = new List<string>(RepositoryPartTypeNames.Distinct());
        }

        public ModelRepositoryInfo(ModelRepositoryData sourceData)
        {
            RepositoryId = sourceData.Context.RepositoryId;
            RepositoryTypeName = sourceData.Context.RepositoryTypeName;
            RepositoryPartTypeNames.Add(sourceData.Context.RepositoryPartTypeName);
            RepositoryPartTypeNames = new List<string>(RepositoryPartTypeNames.Distinct());
        }

        #endregion
        #region props

        public Guid RepositoryId { get; set; } = Guid.Empty;
        public string RepositoryTypeName { get; set; } = string.Empty;
        public List<string> RepositoryPartTypeNames { get; set; } = new();

        #endregion
    }
}
