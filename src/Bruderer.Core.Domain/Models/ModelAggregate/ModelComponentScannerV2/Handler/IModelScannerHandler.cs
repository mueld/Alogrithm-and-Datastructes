using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Handler
{
    public interface IModelScannerHandler : IVisitor
    {
        void setNextHandler(IModelScannerHandler nextHandler);
    }
}
