using cconv.Converters.Base;

using System.Reflection;

namespace cconv.Converters
{
    internal class PythonConverter : BaseConverter
    {
        public override string TemplateName => "BasePythonClass.py";

        public override string LanguageName => "Python";

        public override string FileExtension => "py";

        protected override string LangConvert(string template, string className, List<MethodInfo> functions, string cSharpFileName)
        {
            template = template.Replace("CLASS_NAME", className);

            template = template.Replace("LIB_NAME", cSharpFileName);

            var functionBlock = string.Empty;

            foreach (var function in functions)
            {
                var line = $"\tdef {function.Name}";

                var parameters = string.Empty;

                if (function.GetParameters().Any())
                {
                    parameters = string.Join(',', function.GetParameters().Select(a => a.Name));
                }

                line += $"({parameters}):{System.Environment.NewLine}\t\t";

                line += $"return __library.{function.Name}({parameters})";

                line += System.Environment.NewLine;

                line += System.Environment.NewLine;

                functionBlock += line;
            }

            return template.Replace("FUNCTION_BLOCK", functionBlock);
        }
    }
}