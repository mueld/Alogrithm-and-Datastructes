using Bruderer.Core.Domain.Models.ModelComponentAggregate;

namespace Bruderer.Core.Domain.Models.ModelVariableAggregate
{
    public class ModelVariableDEO : IModelVariableDEO
    {
        #region fields

        protected IModelVariable? _VariableReference = null;

        #endregion
        #region ctor

        public ModelVariableDEO()
        { }

        public ModelVariableDEO(IModelVariable? variable, bool isReference = true)
        {
            if (variable == null)
                return;

            if (isReference)
                _VariableReference = variable;

            ModelContext = new ModelComponentContext(variable, isReference);
            RepositoryContext = new RepositoryComponentContext(variable, isReference);
            Payload = new ModelVariablePayload(variable, isReference);
        }

        #endregion
        #region props

        public IModelContext ModelContext { get; set; } = new ModelComponentContext();
        public IModelRepositoryComponent RepositoryContext { get; set; } = new RepositoryComponentContext();
        public ModelVariablePayload Payload { get; set; } = new();

        public bool HasReference
        {
            get { return _VariableReference != null; }
        }

        public virtual void SetReference(IModelVariable variable)
        {
            _VariableReference = variable;
            ModelContext = new ModelComponentContext(variable, true);
            RepositoryContext = new RepositoryComponentContext(variable, true);
            Payload.SetReference(variable);
        }

        #endregion
    }
}
