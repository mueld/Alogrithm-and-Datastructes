using System.Collections.Generic;
using TwinCAT.TypeSystem;

namespace Bruderer.Core.Infrastructure.PLC.TwinCAT3.Connection
{
    public class TC3VariableBlock
    {
        public int Count { get; set; } = 0;
        public List<TC3Variable> Variables { get; } = new List<TC3Variable>();
        public List<object> VariableValues { get; } = new List<object>();
        public SymbolCollection SymbolCollection { get; } = new SymbolCollection();
    }
}
