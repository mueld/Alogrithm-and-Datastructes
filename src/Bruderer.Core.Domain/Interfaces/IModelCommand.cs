using System;

namespace Bruderer.Core.Domain.Interfaces
{
    public interface IModelCommand : ICommand
    {
        Guid ModelId { get; }
        string ModelName { get; set; }
    }
}
