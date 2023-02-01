using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelConditionAggregate
{
    public interface IModelConditionObserver
    {
        Guid Id { get; }
        void ConditionChanged(IModelCondition condition);
    }
}
