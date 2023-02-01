using Bruderer.Core.Domain.Models.ModelAggregate;
using System;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RepositoryDataAttribute : Attribute
    {
        #region ctor

        public RepositoryDataAttribute(ModelRepositoryDataFlags repositoryDataFlags)
        {
            RepositoryDataFlags = repositoryDataFlags;
        }

        #endregion
        #region props

        public ModelRepositoryDataFlags RepositoryDataFlags { get; } = ModelRepositoryDataFlags.None;

        #endregion
    }
}
