using Bruderer.Core.Domain.Messaging.Response;
using Bruderer.Core.Domain.Models.ComponentAggregate;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Application.DTO
{
    // OFI -> Check for auto mapping solutions as https://github.com/AutoMapper/AutoMapper
    public class ComponentDTO
    {
        #region ctor

        public ComponentDTO()
        { }

        public ComponentDTO(IComponent component)
        {
            Id = component.Id;
            Name = component.Name;
            Version = component.Version;
            State = component.State;
            MessageStack = component.MessageStack;
        }

        #endregion
        #region props

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public ComponentStateEnumeration State { get; set; }
        public List<IResponseMessage> MessageStack { get; set; }

        #endregion
    }
}
