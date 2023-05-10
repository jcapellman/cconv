using System.Runtime.InteropServices;

namespace Bibliothek
{
    public class DasGut
    {
        [UnmanagedCallersOnly(EntryPoint = "siesquare")]
        public static int SieSquare(int x)
        {
            return x * x;
        }
    }
}