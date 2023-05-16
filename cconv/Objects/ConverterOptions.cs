namespace cconv.Objects
{
    internal class ConverterOptions
    {
        public string InputLibrary { get; private set; } = string.Empty;

        public string OutputLanguage { get; private set; } = string.Empty;

        public string OutputPath { get; private set; } = AppContext.BaseDirectory;

        /// <summary>
        /// Used for Unit Tests Only
        /// </summary>
        public ConverterOptions(string inputLibrary, string outputLanguage, string outputPath)
        {
            InputLibrary = inputLibrary;
            OutputLanguage = outputLanguage;
            OutputPath = outputPath;
        }

        /// <summary>
        /// Accepts the string[] from the command line and parses them
        /// </summary>
        /// <param name="commandLineArguments"></param>
        /// <exception cref="ArgumentException">Throws if the parameters were not in pairs or if no arguments were passed in</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the parameter passed in does not match an expected argument</exception>
        public ConverterOptions(string[] commandLineArguments)
        {
            if (commandLineArguments.Length == 0)
            {
                throw new ArgumentException("Cannot run without arguments");
            }

            if (commandLineArguments.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid arguments - they must come in pairs");
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