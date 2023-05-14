using cconv.Objects;

namespace cconv
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConverterOptions options;

            try
            {
                options = new ConverterOptions(args);
            } catch (ArgumentOutOfRangeException are)
            {
                Console.WriteLine(are.Message);

                return;
            } catch (ArgumentException ae)
            {
                Console.WriteLine(ae.Message);

                return;
            }

            var converter = new Converter(options);

            converter.Convert();
        }
    }
}