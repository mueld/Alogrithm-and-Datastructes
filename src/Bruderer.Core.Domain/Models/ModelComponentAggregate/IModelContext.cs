using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;

#nullable enable

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate
{
    public interface IModelContext
    {
        Guid Id { get; set; }
        KeyValue ModelLink { get; set; }
        string ServiceName { get; set; }
        List<string> Filters { get; set; }
    }
}
