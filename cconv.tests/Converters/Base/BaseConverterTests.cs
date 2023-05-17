using cconv.Converters.Base;
using cconv.Objects;

namespace cconv.tests.Converters.Base
{
    [TestClass]
    public class BaseConverterTests
    {
        [TestMethod]
        public void GetConverter_ValidLanguage_ReturnsConverter()
        {
            // Arrange
            var language = "Python";

            // Act
            var converter = BaseConverter.GetConverter(language);

            // Assert
            Assert.IsNotNull(converter);
        }

        [TestMethod]
        public void GetConverter_InvalidLanguage_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var language = "InvalidLanguage";

            // Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => BaseConverter.GetConverter(language));
        }

        [TestMethod]
        public void GetClasses_ValidFile_ReturnsDictionaryOfMethods()
        {
            // Arrange
            var fileName = Path.Combine(AppContext.BaseDirectory, "TestData", "Bibliothek.dll");

            // Act
            var converter = BaseConverter.GetConverter("Python");

            Assert.IsNotNull(converter);

            var options = new ConverterOptions(inputLibrary: fileName, outputLanguage: "Python", outputPath: AppContext.BaseDirectory);

            converter.Convert(options);
        }
    }
}