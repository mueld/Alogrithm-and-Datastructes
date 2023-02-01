using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Bruderer.Core.Infrastructure.ModelContext
{
    public class ModelUserContext
    {
        #region fields

        protected readonly ILogger<ModelUserContext> _Logger;

        #endregion
        #region ctor

        public ModelUserContext(
            IModelUser modelUser,
            ILoggerFactory loggerFactory)
        {
            ModelUser = modelUser;
            _Logger = loggerFactory.CreateLogger<ModelUserContext>();
        }

        #endregion
        #region props

        public IModelUser ModelUser { get; private set; } = default;
        public List<IModel> ProvidedModels { get; private set; } = new List<IModel>();
        public List<IModel> ConsumedModels { get; private set; } = new List<IModel>();

        #endregion
        #region methods

        public bool AddProvidedModel(IModel model, bool canReplace = false)
        {
            if (ProvidedModels.Any(m => m.Id == model.Id))
            {
                if (canReplace)
                {
                    var index = ProvidedModels.IndexOf(model);
                    if (index < 0)
                    {
                        _Logger.LogError($"Model [{model.Name}] can not be replaced as provided element for model user [{ModelUser.Name}].");
                        return false;
                    }

                    _Logger.LogWarning($"Model [{model.Name}] is replaced as provided element for model user [{ModelUser.Name}].");
                    return true;
                }

                _Logger.LogError($"Model [{model.Name}] is already present as provided element for model user [{ModelUser.Name}].");
                return false;
            }

            ProvidedModels.Add(model);
            _Logger.LogInformation($"Model [{model.Name}] is set as provided element for model user [{ModelUser.Name}].");
            return true;
        }

        public bool AddConsumedModel(IModel model, bool canReplace = false)
        {
            if (ConsumedModels.Any(m => m.Id == model.Id))
            {
                if (canReplace)
                {
                    var index = ConsumedModels.IndexOf(model);
                    if (index < 0)
                    {
                        _Logger.LogError($"Model [{model.Name}] can not be replaced as consumed element for model user [{ModelUser.Name}].");
                        return false;
                    }

                    _Logger.LogWarning($"Model [{model.Name}] is replaced as consumed element for model user [{ModelUser.Name}].");
                    return true;
                }

                _Logger.LogError($"Model [{model.Name}] is already present as consumed element for model user [{ModelUser.Name}].");
                return false;
            }

            ConsumedModels.Add(model);
            _Logger.LogInformation($"Model [{model.Name}] is set as consumed element for model user [{ModelUser.Name}].");
            return true;
        }

        #endregion
    }
}
