using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RuntimeAssemblyGenerator.Core.Analysers
{
    public class JsonDataAnalyser : IDataAnalyser<string, ExpandoObject>
    {
        public override async Task<Dictionary<string, Dictionary<string, object>>> AnalysisData(ExpandoObject inputData, string mainModelName)
        {
            Dictionary<string, Dictionary<string, object>> classAndProps = new Dictionary<string, Dictionary<string, object>>();
            var mainModel = new KeyValuePair<string, Dictionary<string, object>>(mainModelName, new Dictionary<string, object>());
            AnalysisTypeAndFillToDictionary(inputData, classAndProps, mainModel);
            classAndProps.Add(mainModel.Key, mainModel.Value);
            return classAndProps;
        }
        private static void AnalysisTypeAndFillToDictionary(ExpandoObject item, Dictionary<string, Dictionary<string, object>> typePropsDictionary, KeyValuePair<string, Dictionary<string, object>> currentItem)
        {
            foreach (PropertyDescriptor propertyInfo in TypeDescriptor.GetProperties(item))
            {
                if (propertyInfo.PropertyType == typeof(List<object>))
                {
                    //新对象
                    var propertyValue = propertyInfo.GetValue(item) as List<object>;
                    if (propertyValue[0] is ExpandoObject subItem)
                    {
                        var newTypeEntity = new KeyValuePair<string, Dictionary<string, object>>(propertyInfo.Name, new Dictionary<string, object>());
                        AnalysisTypeAndFillToDictionary(subItem, typePropsDictionary, newTypeEntity);
                        typePropsDictionary.Add(newTypeEntity.Key, newTypeEntity.Value);
                        currentItem.Value.Add(newTypeEntity.Key, new System.CodeDom.CodeTypeReference($"List<{propertyInfo.Name}>"));
                    }
                    else
                    {
                        currentItem.Value.Add(propertyInfo.Name, typeof(List<>).MakeGenericType(propertyValue[0].GetType()));
                    }
                }
                else
                {
                    currentItem.Value.Add(propertyInfo.Name, propertyInfo.PropertyType);
                }
            }
        }

        public override async Task<ExpandoObject> ParseDataToIntermediateModel(string inputData)
        {
            var intermediateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(inputData);
            return intermediateModel;
        }
    }
}
