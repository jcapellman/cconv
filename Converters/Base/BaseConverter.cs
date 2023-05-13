using cconv.Objects;

using System.Reflection;
using System.Text;

namespace cconv.Converters.Base
{
    internal abstract class BaseConverter
    {
        public abstract string LanguageName { get;}

        public abstract string TemplateName { get; }

        public abstract string FileExtension { get; }

        protected abstract string LangConvert(string template, string className, List<MethodInfo> functions, string cSharpFileName);

        protected static string GetTemplate(string templateName)
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

        public void Convert(Dictionary<string, List<MethodInfo>> csharpClasses, string cSharpFileName, ConverterOptions options)
        {
            var baseTemplate = GetTemplate(TemplateName);

            foreach (var className in csharpClasses.Keys)
            {
                var classTemplate = baseTemplate;

                classTemplate = LangConvert(classTemplate, className, csharpClasses[className], cSharpFileName);

                if (!Path.Exists(options.OutputPath))
                {
                    Directory.CreateDirectory(options.OutputPath);
                }

                File.WriteAllText(Path.Combine(options.OutputPath, $"{className}.{FileExtension}"), classTemplate);
            }
        }
    }
}