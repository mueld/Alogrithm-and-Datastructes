using System;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    [Flags] // Define enumeration constants in powers of two
    public enum ModelRepositoryDataFlags
    {
        None = 1,
        VariableData =2
    }
}
