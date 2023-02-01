using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Bruderer.Core.Infrastructure.ModelContext
{
    public class ModelContext
    {
        #region fields

        protected readonly ILogger<ModelContext> _Logger;

        #endregion
        #region ctor

        public ModelContext(
            IModel model,
            ILoggerFactory loggerFactory)
        {
            Model = model;
            _Logger = loggerFactory.CreateLogger<ModelContext>();
        }

        #endregion
        #region props

        public IModel Model { get; private set; } = default;
        public IModelUser Provider { get; private set; } = default;
        public List<IModelUser> Consumers { get; private set; } = new List<IModelUser>();

        #endregion
        #region methods

        public bool AddProvider(IModelUser modelUser, bool canReplace = false)
        {
            if (Provider == null)
            {
                Provider = modelUser;
                _Logger.LogInformation($"Model user [{modelUser.Name}] is set as provider for model [{Model.Name}].");
                return true;
            }

            if (canReplace)
            {
                _Logger.LogWarning($"Model user [{modelUser.Name}] replaces [{Provider.Name}] as provider for model [{Model.Name}]. Only one provider per model is possible.");
                Provider = modelUser;
                return true;
            }

            _Logger.LogError($"Model user [{modelUser.Name}] can not be set as provider for model [{Model.Name}]. Only one provider per model is possible.");
            return false;
        }

        public bool AddConsumer(IModelUser modelUser, bool canReplace = false)
        {
            if (Consumers.Any(mu => mu.Id == modelUser.Id))
            {
                if (canReplace)
                {
                    var index = Consumers.IndexOf(modelUser);
                    if (index < 0)
                    {
                        _Logger.LogError($"Model user [{modelUser.Name}] can not be replaced as consumer for model [{Model.Name}].");
                        return false;
                    }

                    _Logger.LogWarning($"Model user [{modelUser.Name}] is replaced as consumer for model [{Model.Name}].");
                    return true;
                }

                _Logger.LogError($"Model user [{modelUser.Name}] is already present as consumer for model [{Model.Name}].");
                return false;
            }

            Consumers.Add(modelUser);
            _Logger.LogInformation($"Model user [{modelUser.Name}] is set as consumer for model [{Model.Name}].");
            return true;
        }

        #endregion
    }
}
