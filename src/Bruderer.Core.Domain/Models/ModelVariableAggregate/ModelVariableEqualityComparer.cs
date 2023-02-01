using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelVariableAggregate
{

    class ModelVariableEqualityComparer : IEqualityComparer<IModelVariable>
    {
        public bool Equals(IModelVariable? x, IModelVariable? y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode([DisallowNull] IModelVariable obj)
        {
            return obj.Id.GetHashCode();
        }
    }

}
