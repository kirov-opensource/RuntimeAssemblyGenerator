using RuntimeAssemblyGenerator.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace RuntimeAssemblyGenerator.Core
{
    public class DefaultAssemblyGenerator : IAssemblyGenerator
    {
        private static object _lockObject = new object();
        private static DefaultAssemblyGenerator _instance;
        public static DefaultAssemblyGenerator Instance
        {
            get
            {

                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new DefaultAssemblyGenerator();
                        }
                    }
                }
                return _instance;
            }
        }

        public Assembly CreateAssembly(string codeNamespace, IDictionary<string, Dictionary<string, object>> typePropsDictionary)
        {
            var csc = new Microsoft.CSharp.CSharpCodeProvider();
            var parameters = new CompilerParameters(new[] { "System.dll", "System.Core.dll" });
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;

            var compileUnit = new System.CodeDom.CodeCompileUnit();

            var ns = new CodeNamespace(codeNamespace);

            compileUnit.Namespaces.Add(ns);

            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));


            foreach (var typeProps in typePropsDictionary)
            {
                var classType = new CodeTypeDeclaration(typeProps.Key);
                classType.Attributes = MemberAttributes.Public;
                ns.Types.Add(classType);

                foreach (var prop in typeProps.Value)
                {
                    if (prop.Value is Type type)
                    {
                        var fieldName = "_" + prop.Key;
                        var field = new CodeMemberField(type, fieldName);
                        classType.Members.Add(field);

                        var property = new CodeMemberProperty();
                        property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                        property.Type = new CodeTypeReference(type);
                        property.Name = prop.Key;
                        property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
                        property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName), new CodePropertySetValueReferenceExpression()));
                        classType.Members.Add(property);
                    }
                    else if (prop.Value is CodeTypeReference codeType)
                    {
                        var fieldName = "_" + prop.Key;
                        var field = new CodeMemberField(codeType, fieldName);
                        classType.Members.Add(field);

                        var property = new CodeMemberProperty();
                        property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                        property.Type = codeType;
                        property.Name = prop.Key;
                        property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
                        property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName), new CodePropertySetValueReferenceExpression()));
                        classType.Members.Add(property);
                    }
                }
            }
            var results = csc.CompileAssemblyFromDom(parameters, compileUnit);
            return results.CompiledAssembly;
        }
    }
}
