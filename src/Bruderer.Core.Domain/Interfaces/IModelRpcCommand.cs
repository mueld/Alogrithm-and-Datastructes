using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Interfaces
{
    public interface IModelRpcCommand : IModelCommand
    {
        Guid RpcId { get; }
        IList<object> InputArguments { get; }
    }
}
