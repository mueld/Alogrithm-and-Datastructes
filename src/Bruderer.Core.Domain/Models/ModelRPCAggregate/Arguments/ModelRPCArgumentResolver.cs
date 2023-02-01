using Bruderer.Core.Domain.Attributes.Resolver;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments
{
    public static class ModelRPCArgumentResolver
    {
        public static IList<ModelRPCArgument> GetArguments(object argumentCollection)
        {
            List<ModelRPCArgument> rpcArgumentList = new List<ModelRPCArgument>();
            var argumentCollectionType = argumentCollection.GetType();

            if (argumentCollection is ModelRPCInputArgumentContainer)
            {

                var propertyInfos = argumentCollectionType.GetProperties();
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    var propertyInfo = propertyInfos[i];

                    if (!ContainsBaseAssembly(propertyInfo.PropertyType.Assembly))
                        continue;

                    if (!propertyInfo.PropertyType.IsSubclassOf(typeof(ModelRPCArgument)))
                        continue;

                    var rpcArgument = propertyInfo.GetValue(argumentCollection, null) as ModelRPCArgument;
                    if (rpcArgument == null)
                    {
                        throw new NullReferenceException($"Input argument object can not be resolved to type [{typeof(ModelRPCArgument)}].");
                    }

                    string descriptionValue = AttributeResolver.GetDescriptionAttribute(propertyInfo);
                    string displayValue = AttributeResolver.GetDisplayNameAttribute(propertyInfo);

                    rpcArgument.Name = displayValue;
                    rpcArgument.Description = descriptionValue;        
                    rpcArgumentList.Add(rpcArgument);
                }
            }

            if (argumentCollection is ModelRPCOutputArgumentContainer)
            {

                var propertyInfos = argumentCollectionType.GetProperties();
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    var propertyInfo = propertyInfos[i];

                    if (!ContainsBaseAssembly(propertyInfo.PropertyType.Assembly))
                        continue;

                    if (!propertyInfo.PropertyType.IsSubclassOf(typeof(ModelRPCArgument)))
                        continue;

                    var rpcArgument = propertyInfo.GetValue(argumentCollection, null) as ModelRPCArgument;
                    if (rpcArgument == null)
                    {
                        throw new NullReferenceException($"Output argument object can not be resolved to type [{typeof(ModelRPCArgument)}].");
                    }

                    string descriptionValue = AttributeResolver.GetDescriptionAttribute(propertyInfo);
                    string displayValue = AttributeResolver.GetDisplayNameAttribute(propertyInfo);

                    rpcArgument.Name = displayValue;
                    rpcArgument.Description = descriptionValue;
                    rpcArgumentList.Add(rpcArgument);
                }
            }

            return rpcArgumentList;
        }

        public static T GetInputArgumentContainer<T>(IList<object> inputArguments)
            where T : ModelRPCInputArgumentContainer
        {
            T rpcInputArgumentContainer = Activator.CreateInstance(typeof(T)) as T;

            int ii = 0;

            var rpcInputArgumentContainerType = rpcInputArgumentContainer.GetType();
            if (!(rpcInputArgumentContainer is ModelRPCInputArgumentContainer))
                return rpcInputArgumentContainer;

            var propertyInfos = rpcInputArgumentContainerType.GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var propertyInfo = propertyInfos[i];

                if (!ContainsBaseAssembly(propertyInfo.PropertyType.Assembly))
                    continue;

                if (!propertyInfo.PropertyType.IsSubclassOf(typeof(ModelRPCArgument)))
                    continue;

                var rpcArgument = propertyInfo.GetValue(rpcInputArgumentContainer, null) as ModelRPCArgument;
                if (rpcArgument == null)
                {
                    throw new NullReferenceException("Input argument object is null.");
                }

                if (ii >= inputArguments.Count)
                {
                    throw new IndexOutOfRangeException("Input argument raw array does not match the size of the typed argument list.");

                }

                rpcArgument.SetArgument(inputArguments[ii]);
                ii++;
            }

            return rpcInputArgumentContainer;
        }

        public static T GetOutputArgumentContainer<T>(IList<object> outputArguments)
            where T : ModelRPCOutputArgumentContainer
        {
            T rpcOutputArgumentContainer = Activator.CreateInstance(typeof(T)) as T;

            int ii = 0;

            var rpcOutputArgumentContainerType = rpcOutputArgumentContainer.GetType();
            if (!(rpcOutputArgumentContainer is ModelRPCOutputArgumentContainer))
                return rpcOutputArgumentContainer;

            var propertyInfos = rpcOutputArgumentContainerType.GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var propertyInfo = propertyInfos[i];

                if (!ContainsBaseAssembly(propertyInfo.PropertyType.Assembly))
                    continue;

                if (!propertyInfo.PropertyType.IsSubclassOf(typeof(ModelRPCArgument)))
                    continue;

                var rpcArgument = propertyInfo.GetValue(rpcOutputArgumentContainer, null) as ModelRPCArgument;
                if (rpcArgument == null)
                {
                    throw new NullReferenceException("Output argument object is null.");
                }

                if (ii >= outputArguments.Count)
                {
                    throw new IndexOutOfRangeException("Output argument raw array does not match the size of the typed argument list.");

                }

                rpcArgument.SetArgument(outputArguments[ii]);
                ii++;
            }

            return rpcOutputArgumentContainer;
        }

        public static IList<object> GetInputArguments(ModelRPCInputArgumentContainer argumentcollection)
        {
            IList<object> inputArgumentObjects = new List<object>();
            var argumentType = argumentcollection.GetType();

            if (argumentcollection is ModelRPCInputArgumentContainer)
            {

                var propertyInfos = argumentType.GetProperties();
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    var propertyInfo = propertyInfos[i];

                    if (!ContainsBaseAssembly(propertyInfo.PropertyType.Assembly))
                        continue;

                    if (!propertyInfo.PropertyType.IsSubclassOf(typeof(ModelRPCArgument)))
                        continue;

                    var rpcArgument = propertyInfo.GetValue(argumentcollection, null) as ModelRPCArgument;
                    if (rpcArgument == null)
                    {
                        throw new NullReferenceException("Input argument object is null.");
                    }

                    var argument = rpcArgument.GetArgument();
                    if (argument == null)
                    {
                        throw new NullReferenceException("Input argument is null.");
                    }

                    inputArgumentObjects.Add(argument);
                }
            }

            return inputArgumentObjects;
        }

        public static IList<object> GetOutputArguments(ModelRPCOutputArgumentContainer argumentcollection)
        {
            IList<object> outputArgumentObjects = new List<object>();
            var argumentType = argumentcollection.GetType();

            if (argumentcollection is ModelRPCOutputArgumentContainer)
            {

                var propertyInfos = argumentType.GetProperties();
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    var propertyInfo = propertyInfos[i];

                    if (!ContainsBaseAssembly(propertyInfo.PropertyType.Assembly))
                        continue;

                    if (!propertyInfo.PropertyType.IsSubclassOf(typeof(ModelRPCArgument)))
                        continue;

                    var rpcArgument = propertyInfo.GetValue(argumentcollection, null) as ModelRPCArgument;
                    if (rpcArgument == null)
                    {
                        throw new NullReferenceException("Output argument object is null.");
                    }

                    var argument = rpcArgument.GetArgument();
                    if (argument == null)
                    {
                        throw new NullReferenceException("Output argument is null.");
                    }

                    outputArgumentObjects.Add(argument);
                }
            }

            return outputArgumentObjects;
        }

        public static bool ContainsBaseAssembly(Assembly assembly)
        {
            if (assembly == typeof(ModelComponentContainer).Assembly)
                return true;

            return false;
        }
    }
}
