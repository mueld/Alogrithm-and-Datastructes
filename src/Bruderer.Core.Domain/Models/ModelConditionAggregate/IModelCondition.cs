using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelConditionAggregate
{
    public interface IModelCondition
    {
        string ConditionName{ get; }
        bool IsFulfilled { get; }
    }
}
