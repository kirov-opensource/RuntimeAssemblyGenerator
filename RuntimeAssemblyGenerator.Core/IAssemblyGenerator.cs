using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RuntimeAssemblyGenerator.Abstractions
{
    public interface IAssemblyGenerator
    {
        /// <summary>
        /// 生成程序集
        /// </summary>
        /// <param name="codeNamespace">程序集的Namespace</param>
        /// <param name="classAndProperties">程序集中所有的类型以及类型对应的属性</param>
        /// <returns></returns>
        Assembly CreateAssembly(string codeNamespace, IDictionary<string, Dictionary<string, object>> classAndProperties);
    }
}
