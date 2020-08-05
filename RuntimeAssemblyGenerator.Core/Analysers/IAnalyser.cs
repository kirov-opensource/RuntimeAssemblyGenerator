using System.Collections.Generic;
using System.Threading.Tasks;

namespace RuntimeAssemblyGenerator.Core.Analysers
{
    public interface IAnalyser<InputModel>
    {
        Task<Dictionary<string, Dictionary<string, object>>> AnalysisData(InputModel inputData, string mainModelName);
    }
}
