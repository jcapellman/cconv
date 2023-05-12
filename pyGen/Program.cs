using System.Reflection;
using System.Runtime.InteropServices;

namespace pyGen
{
    internal class Program
    {
        private static Dictionary<string, List<MethodInfo>> GetClasses(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            var asm = Assembly.LoadFrom(fileName);

            var types = asm.GetTypes();

            var result = new Dictionary<string, List<MethodInfo>>();

            foreach (var type in types)
            {
                if (!type.IsPublic)
                {
                    continue;
                }

                var methodInfos = type.GetMethods().Where(a => a.Attributes.HasFlag(MethodAttributes.Public) && 
                    a.Attributes.HasFlag(MethodAttributes.Static)).ToList();

                var functions = new List<MethodInfo>();

                foreach (MethodInfo method in methodInfos)
                {
                    if (method.GetCustomAttribute<UnmanagedCallersOnlyAttribute>() is null)
                    {
                        continue;
                    }

                    functions.Add(method);
                }

                if (functions.Any())
                {
                    result.Add(type.Name, functions);
                }
            }

            return result;
        }

        static void Main(string[] args)
        {
            var libName = args[0];

            var classes = GetClasses(libName);

            var baseTemplate = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "template.py"));

            foreach (var className in classes.Keys)
            {
                var classTemplate = baseTemplate;

                classTemplate = classTemplate.Replace("CLASS_NAME", className);

                classTemplate = classTemplate.Replace("LIB_NAME", libName);

                var functionBlock = string.Empty;

                foreach (var function in classes[className])
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

                classTemplate = classTemplate.Replace("FUNCTION_BLOCK", functionBlock);

                File.WriteAllText(Path.Combine(AppContext.BaseDirectory, $"{className}.py"), classTemplate);
            }
        }
    }
}