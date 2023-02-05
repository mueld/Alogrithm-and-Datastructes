using Bruderer.Core.Domain.Attributes.Resolver;
using Bruderer.Core.Domain.Constants;
using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Helpers
{
    public class Twincat3MetaDataBuilder : RecursiveVistor
    {
        private TC3MetaData _CurrentMetaData;
        private Stack<TC3MetaData> _MetaDataStack;
        private string collectionName = string.Empty;
        public Twincat3MetaDataBuilder()
        {
            _CurrentMetaData = new TC3MetaData();
            _MetaDataStack = new Stack<TC3MetaData>();
            _MetaDataStack.Push(_CurrentMetaData);
        }
        public Twincat3MetaDataBuilder(ModelComponentContainer rootNode)
        {
            _CurrentMetaData = new TC3MetaData();
            _MetaDataStack = new Stack<TC3MetaData>();
            if (rootNode.TwinCAT3Links.Count != 1)
            {
                throw new ArgumentException("Root node has less or more than 1 TwinCAT3Link");
            }

            _CurrentMetaData.SamplingRate = rootNode.TwinCAT3Links[0].SamplingRate;
            _CurrentMetaData.SymbolPath = rootNode.TwinCAT3Links[0].SymbolKey.Key;
            _CurrentMetaData.ModelUserName = rootNode.TwinCAT3Links[0].Name;
            _CurrentMetaData.IsIgnored = rootNode.TwinCAT3Links[0].Ignore;
            _MetaDataStack.Push(_CurrentMetaData);
        }

        public override void LeaveModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer)
        {
            _MetaDataStack.Pop();
            _CurrentMetaData = _MetaDataStack.Peek();
        }

        public override void LeaveModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable)
        {
            _MetaDataStack.Pop();
            _CurrentMetaData = _MetaDataStack.Peek();
        }

        public override void LeaveModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer modelContainer, int index)
        {
            _MetaDataStack.Pop();
            _CurrentMetaData = _MetaDataStack.Peek();
        }

        public override void LeaveServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer)
        {
            _MetaDataStack.Pop();
            _CurrentMetaData = _MetaDataStack.Peek();
        }

        public override void VisitModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer)
        {
            var metadata = UpdateMetaData(elementProperty);

            var twincat3Link = GetModelTwinCAT3LinkFromMetaData(metadata);
            modelComponentContainer.TwinCAT3Links.Add(twincat3Link);
            _CurrentMetaData = metadata;
            _CurrentMetaData.SymbolPath = BuildModelPath(metadata.SymbolPath, metadata.ElementName);
            _MetaDataStack.Push(_CurrentMetaData);
        }

        public override void VisitModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable)
        {
            var metadata = UpdateMetaData(elementProperty);
            collectionName = metadata.ElementName;
            var twincat3Link = GetModelTwinCAT3LinkFromMetaData(metadata);
            variable.TwinCAT3Links.Add(twincat3Link);
            _CurrentMetaData = metadata;
            _CurrentMetaData.SymbolPath = BuildModelPath(metadata.SymbolPath, metadata.ElementName);
            _MetaDataStack.Push(_CurrentMetaData);
        }

        public override void VisitModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer serviceContainer, int index)
        {
            var metadata = UpdateMetaData(elementProperty);

            metadata.ElementName = collectionName + GetEnumerableString(index + 1);

            var twincat3Link = GetModelTwinCAT3LinkFromMetaData(metadata);
            serviceContainer.TwinCAT3Links.Add(twincat3Link);
            _CurrentMetaData = metadata;
            _CurrentMetaData.SymbolPath = BuildModelPath(metadata.SymbolPath, metadata.ElementName, string.Empty);
            _MetaDataStack.Push(_CurrentMetaData);
        }

        public override void VisitModelRPC(PropertyInfo elementProperty, ModelRPCBase rpc)
        {
            var metadata = UpdateMetaData(elementProperty);

            var twincat3Link = GetModelTwinCAT3LinkFromMetaData(metadata);
            twincat3Link.SamplingRate = ModelVariableSamplingRateEnumeration.None;
            rpc.TwinCAT3Links.Add(twincat3Link);
        }

        public override void VisitModelVariable(PropertyInfo elementProperty, ModelVariableBase variable)
        {
            var metadata = UpdateMetaData(elementProperty);

            var twincat3Link = GetModelTwinCAT3LinkFromMetaData(metadata);
            variable.TwinCAT3Links.Add(twincat3Link);
        }

        public override void VisitServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer)
        {
            var metadata = UpdateMetaData(elementProperty);

            var twincat3Link = GetModelTwinCAT3LinkFromMetaData(metadata);
            serviceContainer.TwinCAT3Links.Add(twincat3Link);
            _CurrentMetaData = metadata;
            _CurrentMetaData.SymbolPath = BuildModelPath(metadata.SymbolPath, metadata.ElementName);
            _MetaDataStack.Push(_CurrentMetaData);
        }


        private TC3MetaData UpdateMetaData(PropertyInfo elementProperty)
        {
            var metaData = new TC3MetaData(_CurrentMetaData);
            metaData.SamplingRate = GetTC3ModeSamplingRateFromAttribute(elementProperty);
            metaData.SymbolPath = GetTC3ModelPathFromAttribute(elementProperty);
            metaData.ModelUserName = GetTC3ModelUserName(elementProperty);
            metaData.IsIgnored = GetTC3IsIgnoredFromAttribute(elementProperty);
            metaData.ElementName = GetTC3ModelNameFromAttribute(elementProperty);
            return metaData;
        }

        private string GetTC3ModelUserName(PropertyInfo elementProperty)
        {
            var twinCAT3EntryPointAttribute = AttributeResolver.GetTwinCAT3EntryPointAttribute(elementProperty);
            if (twinCAT3EntryPointAttribute != null)
            {
                return twinCAT3EntryPointAttribute.ModelUserName;
            }
            else
            {
                return _CurrentMetaData.ModelUserName;
            }

        }

        private string GetTC3ModelPathFromAttribute(PropertyInfo elementProperty)
        {
            var symbolPathAttribute = AttributeResolver.GetTwinCAT3SymbolLinkAttribute(elementProperty);
            if (symbolPathAttribute != null)
            {
                if (!string.IsNullOrEmpty(symbolPathAttribute.SymbolLink))
                {
                    return symbolPathAttribute.SymbolLink.ToUpper(); 
                }
                else
                {
                    return _CurrentMetaData.SymbolPath;
                }
            }
            else
            {
                return _CurrentMetaData.SymbolPath;
            }

        }

        private ModelVariableSamplingRateEnumeration GetTC3ModeSamplingRateFromAttribute(PropertyInfo elementProperty)
        {
            var twinCAT3SamplingRateAttribute = AttributeResolver.GetTwinCAT3SamplingRateAttribute(elementProperty);
            if (twinCAT3SamplingRateAttribute != null)
            {
                return twinCAT3SamplingRateAttribute.SamplingRate;
            }
            else
            {
                return _CurrentMetaData.SamplingRate;
            }
        }

        private string GetTC3ModelNameFromAttribute(PropertyInfo elementProperty)
        {
            var twinCAT3SymbolNameAttribute = AttributeResolver.GetTwinCAT3SymbolNameAttribute(elementProperty);
            if (twinCAT3SymbolNameAttribute != null)
            {
                if (!string.IsNullOrEmpty(twinCAT3SymbolNameAttribute.SymbolName))
                {
                    return twinCAT3SymbolNameAttribute.SymbolName.ToUpper();
                }
                else
                {
                    return elementProperty.Name.ToUpper();
                }
            }
            else
            {
                return elementProperty.Name.ToUpper();
            }
                
        }

        private bool GetTC3IsIgnoredFromAttribute(PropertyInfo elementProperty)
        {
            var twinCAT3IgnoreAttribute = AttributeResolver.GetTwinCAT3IgnoreAttribute(elementProperty);
            if (twinCAT3IgnoreAttribute != null)
            {
                return twinCAT3IgnoreAttribute.IsEnabled;
            }
            else
            {
               return _CurrentMetaData.IsIgnored;
            }

        }

        private ModelTwinCAT3Link GetModelTwinCAT3LinkFromMetaData(TC3MetaData metaData)
        {
            var twincat3Link = new ModelTwinCAT3Link()
            {
                Name = metaData.ModelUserName,
                Platform = ModelUserPlatformEnumeration.TwinCAT3,
                Ignore = metaData.IsIgnored,
                SamplingRate = metaData.SamplingRate
            };
            twincat3Link.SymbolKey.Path = metaData.SymbolPath;
            twincat3Link.SymbolKey.Name = metaData.ElementName;
            return twincat3Link;
        }

        private static string BuildModelPath(string currentPath, string elementName, string seperator = StringConstants.Separator)
        {
            if (string.IsNullOrEmpty(currentPath))
                return elementName;
            if (string.IsNullOrEmpty(elementName))
                return currentPath;

            return currentPath + seperator + elementName;
        }

        private static string GetEnumerableString(int index, string startArray = StringConstants.StartArray, string endArray = StringConstants.EndArray)
        {
            return startArray + index.ToString() + endArray;
        }
    }

    internal class TC3MetaData
    {
        public TC3MetaData() {
            ModelUserName = string.Empty;
            ElementName = string.Empty;
            SymbolPath = string.Empty;
            SamplingRate = ModelVariableSamplingRateEnumeration.Undefined;
            IsIgnored = false;
        }
        public TC3MetaData(TC3MetaData metaData)
        {
            ModelUserName = metaData.ModelUserName;
            ElementName = string.Empty;
            SymbolPath = metaData.SymbolPath;
            SamplingRate = metaData.SamplingRate;
            IsIgnored = metaData.IsIgnored;
        }

        public string ModelUserName;
        public string SymbolPath;
        public string ElementName;
        public ModelVariableSamplingRateEnumeration SamplingRate;
        public bool IsIgnored;
    }
}
