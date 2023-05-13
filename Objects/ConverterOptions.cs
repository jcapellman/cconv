namespace cconv.Objects
{
    internal class ConverterOptions
    {
        public string InputLibrary { get; set; } = string.Empty;

        public string OutputLanguage { get; set; } = string.Empty;

        public string OutputPath { get; set; } = AppContext.BaseDirectory;

        public ConverterOptions(string[] commandLineArguments)
        {
            if (commandLineArguments.Length == 0 || commandLineArguments.Length % 2 != 0)
            {
                throw new ArgumentException($"Invalid parameter ({nameof(commandLineArguments)}) - must come in pairs");
            }

            var properties = GetType().GetProperties();

            for (var x = 0; x < commandLineArguments.Length; x += 2)
            {
                var argumentName = commandLineArguments[x].Trim();
                var argumentValue = commandLineArguments[x + 1].Trim();

                var property = properties.FirstOrDefault(a => string.Equals(argumentName, a.Name, StringComparison.OrdinalIgnoreCase))
                    ?? throw new ArgumentOutOfRangeException($"{argumentName} is an invalid command line parameter");

                property.SetValue(this, argumentValue, null);
            }
        }
    }
}