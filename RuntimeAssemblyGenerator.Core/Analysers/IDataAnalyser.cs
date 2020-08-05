using RuntimeAssemblyGenerator.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeAssemblyGenerator.Core.Analysers
{
    public abstract class IDataAnalyser<OriginInputModel, IntermediateModel> : IAnalyser<IntermediateModel>
    {
        public abstract Task<Dictionary<string, Dictionary<string, object>>> AnalysisData(IntermediateModel inputData, string mainModelName);

        public abstract Task<IntermediateModel> ParseDataToIntermediateModel(OriginInputModel inputData);

        public virtual async Task<Assembly> ConvertDataToAssembly(OriginInputModel rawData, string codeNamespace, string mainModelName)
        {
            var intermediateModel = await ParseDataToIntermediateModel(rawData);
            var classAndProperties = await AnalysisData(intermediateModel, mainModelName);
            return DefaultAssemblyGenerator.Instance.CreateAssembly(codeNamespace, classAndProperties);
        }

    }
}
