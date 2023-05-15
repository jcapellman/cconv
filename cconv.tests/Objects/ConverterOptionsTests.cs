using cconv.Objects;

namespace cconv.tests.Objects
{
    [TestClass]
    public class ConverterOptionsTests
    {
        [TestMethod]
        public void Constructor_ValidArguments_PropertiesShouldBeSet()
        {
            // Arrange
            var inputLibrary = "InputLibrary.dll";
            var outputLanguage = "Python";
            var outputPath = "C:\\Output";
            var commandLineArguments = new string[]
            {
                "inputlibrary", inputLibrary,
                "outputlanguage", outputLanguage,
                "outputpath", outputPath
            };

            // Act
            var options = new ConverterOptions(commandLineArguments);

            // Assert
            Assert.AreEqual(inputLibrary, options.InputLibrary);
            Assert.AreEqual(outputLanguage, options.OutputLanguage);
            Assert.AreEqual(outputPath, options.OutputPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NoArguments_ThrowsArgumentException()
        {
            // Arrange
            var commandLineArguments = Array.Empty<string>();

            // Act and Assert
            var _ = new ConverterOptions(commandLineArguments);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_InvalidArguments_ThrowsArgumentException()
        {
            // Arrange
            var commandLineArguments = new string[]
            {
                "inputlibrary", "InputLibrary.dll",
                "outputlanguage" // Missing argument value
            };

            // Act and Assert
            var _ = new ConverterOptions(commandLineArguments);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_UnknownArgument_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var commandLineArguments = new string[]
            {
                "inputlibrary", "InputLibrary.dll",
                "outputformat", "Python" // Invalid argument
            };

            // Act and Assert
            var _ = new ConverterOptions(commandLineArguments);
        }
    }
}
