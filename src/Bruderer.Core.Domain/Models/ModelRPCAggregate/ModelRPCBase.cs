using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Bruderer.Core.Domain.Models.ModelConditionAggregate;
using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Bruderer.Core.Domain.Models.ModelRPCAggregate
{
    public abstract class ModelRPCBase : ModelComponent, IModelConditionObserver
    {
        [NotMapped]
        public LocalizableValue Display { get; set; } = new();
        [NotMapped]
        public LocalizableValue Description { get; set; } = new();

        [NotMapped]
        public bool IsExecutable { get; set; } = true;

        [NotMapped]
        public ModelIdentityUserRoleEnumeration VisibilityAuthorization { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;
        [NotMapped]
        public ModelIdentityUserRoleEnumeration ManipulationAuthorization { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;
        [NotMapped]
        public List<IModelCondition> ExecutionConditions { get; set; } = new();

        public abstract Type InputArgumentType { get; }
        public abstract Type OutputArgumentType { get; }

        public virtual void ConditionChanged(IModelCondition condition)
        {
          IsExecutable = ExecutionConditions.All(condition => condition.IsFulfilled);
        }

        public void AddModelCondition(IModelCondition modelCondition)
        {
            ExecutionConditions.Add(modelCondition);
        }

        /// <summary>
        /// Invoke this method to notify registered observers. Must be invoked after successfully RPC execution.
        /// </summary>
        public void Invoked()
        {
            NotifyObserver();
        }
    }
}
