using System;

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate
{
    public class RepositoryComponentContext : IModelRepositoryComponent
    {
        #region fields

        protected IModelRepositoryComponent? _ComponentReference = null;

        private bool _IsRepositoryClientLocal = false;
        private string _RepositoryTypeNameLocal = string.Empty;
        private string _RepositoryPartTypeNameLocal = string.Empty;
        private Guid _RepositoryIdLocal = Guid.Empty;
        private string _RepositoryLink = string.Empty;

        #endregion
        #region ctor

        public RepositoryComponentContext()
        { }

        public RepositoryComponentContext(IModelRepositoryComponent? component, bool isReference = true)
        {
            if (component == null)
                return;

            if (isReference)
            {
                _ComponentReference = component;
                return;
            }

            _IsRepositoryClientLocal = component.IsRepositoryClient;
            _RepositoryTypeNameLocal = component.RepositoryTypeName;
            _RepositoryPartTypeNameLocal = component.RepositoryPartTypeName;
            _RepositoryIdLocal = component.RepositoryId;
            _RepositoryLink = component.RepositoryLink;
        }

        #endregion
        #region props

        public bool IsRepositoryClient
        {
            get
            {
                if (_ComponentReference != null)
                    return _ComponentReference.IsRepositoryClient;

                return _IsRepositoryClientLocal;
            }
            set
            {
                if (_ComponentReference != null)
                    _ComponentReference.IsRepositoryClient = value;

                _IsRepositoryClientLocal = value;
            }
        }

        public string RepositoryTypeName
        {
            get
            {
                if (_ComponentReference != null)
                    return _ComponentReference.RepositoryTypeName;


                return _RepositoryTypeNameLocal;
            }
            set
            {
                if (_ComponentReference != null)
                    _ComponentReference.RepositoryTypeName = value;

                _RepositoryTypeNameLocal = value;
            }
        }

        public string RepositoryPartTypeName
        {
            get
            {
                if (_ComponentReference != null)
                    return _ComponentReference.RepositoryPartTypeName;


                return _RepositoryPartTypeNameLocal;
            }
            set
            {
                if (_ComponentReference != null)
                    _ComponentReference.RepositoryPartTypeName = value;

                _RepositoryPartTypeNameLocal = value;
            }
        }

        public Guid RepositoryId
        {
            get
            {
                if (_ComponentReference != null)
                    return _ComponentReference.RepositoryId;


                return _RepositoryIdLocal;
            }
            set
            {
                if (_ComponentReference != null)
                    _ComponentReference.RepositoryId = value;

                _RepositoryIdLocal = value;
            }
        }

        public string RepositoryLink
        {
            get
            {
                if (_ComponentReference != null)
                    return _ComponentReference.RepositoryLink;


                return _RepositoryLink;
            }
            set
            {
                if (_ComponentReference != null)
                    _ComponentReference.RepositoryLink = value;

                _RepositoryLink = value;
            }
        }

        #endregion
        #region methods

        public void SetReference(IModelRepositoryComponent component)
        {
            _ComponentReference = component;
        }

        #endregion
    }
}
