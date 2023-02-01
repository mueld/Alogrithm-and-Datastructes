using System;

namespace Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments
{
    public abstract class ModelRPCArgument
    {
        abstract public bool IsArray { get; }
        abstract public Type ArgumentType { get; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        abstract public void SetArgument(object argument);
        abstract public object GetArgument();
    }
}
