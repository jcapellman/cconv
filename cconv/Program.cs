using cconv.Converters.Base;
using cconv.Objects;

namespace cconv
{
    public static class Program
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

            BaseConverter converter;

            try
            {
                converter = BaseConverter.GetConverter(options.OutputLanguage);
            } catch (ArgumentOutOfRangeException are)
            {
                Console.WriteLine(are.Message);

                return;
            }

            try
            {
                converter.Convert(options);
            } catch (FileNotFoundException fnfe)
            {
                Console.WriteLine(fnfe.Message);
            } catch (ArgumentOutOfRangeException are)
            {
                Console.WriteLine(are.Message);
            } catch (ApplicationException ae)
            {
                Console.WriteLine(ae.Message);
            }
        }
    }
}