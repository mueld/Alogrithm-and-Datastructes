using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal
{
    public abstract class VisitorSimple
    {
        public abstract void VisitModelComponent(Type elementType, ModelComponent component);
    }
}
