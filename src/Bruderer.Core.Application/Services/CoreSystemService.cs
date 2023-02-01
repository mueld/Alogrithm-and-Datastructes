using Bruderer.Core.Application.Interfaces;
using Bruderer.Core.Domain.Models.ComponentAggregate;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bruderer.Core.Application.Services
{
    public class CoreSystemService : ICoreSystemService
    {
        #region fields

        protected readonly ILogger<CoreSystemService> _Logger = null;

        #endregion
        #region ctor

        public CoreSystemService(
            ILogger<CoreSystemService> logger)
        {
            _Logger = logger;

            CollectSystemComponents();
        }

        #endregion
        #region props

        public List<IComponent> Components { get; private set; } = new List<IComponent>();

        #endregion
        #region methods

        public bool AddComponent(IComponent component, bool canReplace = true)
        {
            if (Components.Any(c => c.Id == component.Id))
            {
                if (canReplace)
                {
                    var replaceIndex = Components.FindIndex(c => c.Id == component.Id);
                    if (replaceIndex < 0)
                        return false;

                    Components[replaceIndex] = component;
                    return true;
                }

                return false;
            }

            Components.Add(component);
            return true;
        }

        private void CollectSystemComponents()
        {
            AddOSComponent();
        }

        private void AddOSComponent()
        {
            var component = new Component();

            OperatingSystem os = Environment.OSVersion;
            Version version = os.Version;

            component.State = ComponentStateEnumeration.Ok;
            component.Name = os.Platform.ToString();
            component.Version = version.ToString();

            AddComponent(component);
        }

        #endregion
    }
}
