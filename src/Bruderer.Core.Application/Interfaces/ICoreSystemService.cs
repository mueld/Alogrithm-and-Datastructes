using Bruderer.Core.Domain.Models.ComponentAggregate;
using System.Collections.Generic;

namespace Bruderer.Core.Application.Interfaces
{
    public interface ICoreSystemService
    {
        List<IComponent> Components { get; }

        bool AddComponent(IComponent component, bool canReplace = true);
    }
}
