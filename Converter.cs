using csharp2py.Converters.Base;
using csharp2py.Objects;

using System.Reflection;
using System.Runtime.InteropServices;

namespace csharp2py
{
    internal class Converter
    {
        private readonly ConverterOptions _options;

        public Converter(ConverterOptions options)
        {
            _options = options;
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

        private static BaseConverter? GetConverter(string language)
        {
            var converters = Assembly.GetExecutingAssembly().GetTypes().Where(a => a.BaseType == typeof(BaseConverter)).Select(b => (BaseConverter?)Activator.CreateInstance(b)).ToList();

            return converters.FirstOrDefault(a => a != null && string.Equals(a.LanguageName, language, StringComparison.OrdinalIgnoreCase));
        }

        public void Convert()
        {
            var converter = GetConverter(_options.OutputLanguage)
                ?? throw new ArgumentOutOfRangeException($"{_options.OutputLanguage} was not found in the assembly");

            var classes = GetClasses(_options.InputLibrary);

            var csharpLibFileInfo = new FileInfo(_options.InputLibrary);

            converter.Convert(classes, csharpLibFileInfo.Name, _options);
        }
    }
}