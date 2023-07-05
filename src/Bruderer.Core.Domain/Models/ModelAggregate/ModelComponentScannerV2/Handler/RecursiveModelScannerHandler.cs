using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Handler
{
    public abstract class RecursiveModelScannerHandler : RecursiveVistor, IRecursiveModelScannerHandler
    {
        private IRecursiveModelScannerHandler handler;
        public void SetNextHandler(IRecursiveModelScannerHandler handler)
        {
            this.handler = handler;
        }
    }
}
