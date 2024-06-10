namespace SopraProjekt
{
    internal static class Globals
    {
        internal const int Zero = 0;
        internal const int One = 1;
        internal const int Two = 2;
        internal const int Three = 3;
        internal const int Four = 4;
        internal const int Five = 5;
        internal const int Six = 6;
        internal const int Seven = 7;
        internal const int Eight = 8;
        // internal const int Nine = 9;
        internal const int Ten = 10;
        // internal const int Eleven = 11;
        internal const int Twelve = 12;

        internal const int Fifteen = 15;
        internal const int Twenty = 20;

        internal static void Swap<T>(ref T lhs, ref T rhs)
        {
            var temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
