using cconv.Objects;

namespace cconv
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var options = new ConverterOptions(args);

            var converter = new Converter(options);

            converter.Convert();
        }
    }
}