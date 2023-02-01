using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;

#nullable enable

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate
{
    public class ModelComponentContext : IModelContext
    {
        #region fields

        protected IModelComponent? _ComponentReference = null;

        private Guid _IdLocal = Guid.Empty;
        private KeyValue _ModelLinkLocal = new();
        private string _ServiceNameLocal = string.Empty;
        private List<string> _FiltersLocal = new();

        #endregion
        #region ctor

        public ModelComponentContext()
        { }

        public ModelComponentContext(IModelComponent? component, bool isReference = true)
        {
            if (component == null)
                return;

            if (isReference)
            {
                _ComponentReference = component;
                return;
            }

            _IdLocal = component.Id;
            _ModelLinkLocal.Path = component.ModelLink.Path;
            _ModelLinkLocal.Name = component.ModelLink.Name;
            _ServiceNameLocal = component.ServiceName;
            _FiltersLocal = new List<string>(component.Filters);
        }

        #endregion
        #region props

        public Guid Id
        {
            get
            {
                if (_ComponentReference != null)
                    return _ComponentReference.Id;

                return _IdLocal;
            }
            set
            {
                if (_ComponentReference != null)
                    _ComponentReference.Id = value;

                _IdLocal = value;
            }
        }

        public KeyValue ModelLink
        {
            get
            {
                if (_ComponentReference != null)
                    return _ComponentReference.ModelLink;


                return _ModelLinkLocal;
            }
            set
            {
                if (_ComponentReference != null)
                    _ComponentReference.ModelLink = value;

                _ModelLinkLocal = value;
            }
        }

        public string ServiceName
        {
            get
            {
                if (_ComponentReference != null)
                    return _ComponentReference.ServiceName;


                return _ServiceNameLocal;
            }
            set
            {
                if (_ComponentReference != null)
                    _ComponentReference.ServiceName = value;

                _ServiceNameLocal = value;
            }
        }

        public List<string> Filters
        {
            get
            {
                if (_ComponentReference != null)
                    return _ComponentReference.Filters;


                return _FiltersLocal;
            }
            set
            {
                if (_ComponentReference != null)
                    _ComponentReference.Filters = value;

                _FiltersLocal = value;
            }
        }

    #endregion
    #region methods

    public void SetReference(IModelComponent component)
        {
            _ComponentReference = component;
        }

        #endregion
    }
}
