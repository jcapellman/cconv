using cconv.Converters;

namespace cconv.tests.Converters
{
    [TestClass]
    public class PythonConverterTests
    {
        [TestMethod]
        public void TemplateName_ShouldReturnBasePythonClass()
        {
            // Arrange
            var converter = new PythonConverter();

            // Act
            var templateName = converter.TemplateName;

            // Assert
            Assert.AreEqual("BasePythonClass.py", templateName);
        }

        [TestMethod]
        public void LanguageName_ShouldReturnPython()
        {
            // Arrange
            var converter = new PythonConverter();

            // Act
            var languageName = converter.LanguageName;

            // Assert
            Assert.AreEqual("Python", languageName);
        }

        [TestMethod]
        public void FileExtension_ShouldReturnPy()
        {
            // Arrange
            var converter = new PythonConverter();

            // Act
            var fileExtension = converter.FileExtension;

            // Assert
            Assert.AreEqual("py", fileExtension);
        }
    }
}