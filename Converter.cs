using csharp2py.Common;
using csharp2py.Objects;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace csharp2py
{
    internal class Converter
    {
        private readonly ConverterOptions _options;

        public Converter(ConverterOptions options)
        {
            _options = options;
        }

        private static string GetTemplate(string templateName = AppConstants.DEFAULT_TEMPLATE_NAME)
        {
            Assembly assembly = Assembly.GetExecutingAssembly()
                ?? throw new ApplicationException("Could not obtain the assembly");

            var resourceNames = assembly.GetManifestResourceNames();

            var resourceName = resourceNames.FirstOrDefault(a => a.Contains(templateName, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArgumentOutOfRangeException($"Invalid template name {templateName}");

            var resourceStream = assembly.GetManifestResourceStream(resourceName) 
                ?? throw new ApplicationException($"Could not retrieve the {resourceName}");

            byte[] resourceBytes = new byte[resourceStream.Length];
            resourceStream.Read(resourceBytes, 0, (int)resourceStream.Length);

            return Encoding.UTF8.GetString(resourceBytes);
        }

        private static Dictionary<string, List<MethodInfo>> GetClasses(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            Assembly asm;

            try
            {
                asm = Assembly.LoadFrom(fileName);
            } catch (BadImageFormatException)
            {
                throw new ArgumentException($"{fileName} was not a valid C# library");
            }

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

        public void Convert()
        {
            var classes = GetClasses(_options.InputLibrary);

            var baseTemplate = GetTemplate();

            var csharpLibFileInfo = new FileInfo(_options.InputLibrary);

            foreach (var className in classes.Keys)
            {
                var classTemplate = baseTemplate;

                classTemplate = classTemplate.Replace("CLASS_NAME", className);

                classTemplate = classTemplate.Replace("LIB_NAME", csharpLibFileInfo.Name);

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

                if (!Path.Exists(_options.OutputPath))
                {
                    Directory.CreateDirectory(_options.OutputPath);
                }

                File.WriteAllText(Path.Combine(_options.OutputPath, $"{className}.py"), classTemplate);
            }
        }
    }
}