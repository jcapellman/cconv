using System.Runtime.InteropServices;

namespace Bibliothek
{
    public class DasGut
    {
        [UnmanagedCallersOnly(EntryPoint = "SieSquare")]
        public static int SieSquare(int x)
        {
            return x * x;
        }
    }
}