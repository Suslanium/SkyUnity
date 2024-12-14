using System.Runtime.CompilerServices;

namespace Core.Common
{
    public static class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFlagSet(uint flag, uint mask)
        {
            return (flag & mask) != 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFlagSet(ushort flag, ushort mask)
        {
            return (flag & mask) != 0;
        }
    }
}