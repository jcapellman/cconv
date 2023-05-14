using cconv.Objects;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace cconv.Converters.Base
{
    internal abstract class BaseConverter
    {
        public abstract string LanguageName { get;}

        public abstract string TemplateName { get; }

        public abstract string FileExtension { get; }

        protected abstract string LangConvert(string template, string className, List<MethodInfo> functions, string cSharpFileName);

        /// <summary>
        /// Retrieves the Converter based on the language argument
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static BaseConverter GetConverter(string language)
        {
            var converters = Assembly.GetExecutingAssembly().GetTypes().Where(a => a.BaseType == typeof(BaseConverter)).Select(b => (BaseConverter?)Activator.CreateInstance(b)).ToList();

            var converter = converters.FirstOrDefault(a => a != null && string.Equals(a.LanguageName, language, StringComparison.OrdinalIgnoreCase));

            return converter is null ? throw new ArgumentOutOfRangeException($"Language ({language}) is not supported.") : converter;
        }

        /// <summary>
        /// Retrieves all of the classes and methods to convert
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">If the C# Assembly is not found</exception>
        /// <exception cref="ArgumentException">Invalid C# Assembly provided</exception>
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
            }
            catch (BadImageFormatException)
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

        /// <summary>
        /// Retrieves the Language Template to populate
        /// </summary>
        /// <param name="templateName">Name of the template to use</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">Invalid assembly</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid template name</exception>
        private static string GetTemplate(string templateName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly()
                ?? throw new ApplicationException("Could not obtain the assembly");

            var resourceNames = assembly.GetManifestResourceNames();

            var resourceName = resourceNames.FirstOrDefault(a => a.Contains(templateName, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArgumentOutOfRangeException($"Invalid template name {templateName}");

            var resourceStream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new ArgumentOutOfRangeException($"Could not retrieve the {resourceName}");

            byte[] resourceBytes = new byte[resourceStream.Length];
            resourceStream.Read(resourceBytes, 0, (int)resourceStream.Length);

            return Encoding.UTF8.GetString(resourceBytes);
        }

        public void Convert(ConverterOptions options)
        {
            var csharpClasses = GetClasses(options.InputLibrary);

            if (!csharpClasses.Keys.Any())
            {
                Console.WriteLine("No classes found with functions decorated with UnmanagedCallersOnlyAttribute in the provided assembly");

                return;
            }

            var csharpLibFileInfo = new FileInfo(options.InputLibrary);

            var baseTemplate = GetTemplate(TemplateName);

            foreach (var className in csharpClasses.Keys)
            {
                var classTemplate = baseTemplate;

                classTemplate = LangConvert(classTemplate, className, csharpClasses[className], csharpLibFileInfo.Name);

                if (!Path.Exists(options.OutputPath))
                {
                    Directory.CreateDirectory(options.OutputPath);
                }

                var outputPath = Path.Combine(options.OutputPath, $"{className}.{FileExtension}");

                File.WriteAllText(outputPath, classTemplate);

                Console.WriteLine($"Successfully wrote out {outputPath}");
            }
        }
    }
}