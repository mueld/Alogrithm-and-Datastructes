using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Infrastructure.PLC.Connection;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Infrastructure.PLC
{
    public interface IPLCModelUser : IModelUser
    {
        PLCConnectionStateEnumeration ConnectionState { get; }
        PLCRuntimeStateEnumeration RuntimeState { get; }
        TimeSpan ProcessingPeriod { get; set; }
        List<IModelVariable> UnlinkedVariables { get; }
        int ConnectionCounter { get; }
    }
}
