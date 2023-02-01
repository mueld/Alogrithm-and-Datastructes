using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Infrastructure.Services.BackgroundServices
{
    public abstract class BackgoundServiceHandler
    {
        public BackgoundServiceHandler(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; private set; }
    }
}
