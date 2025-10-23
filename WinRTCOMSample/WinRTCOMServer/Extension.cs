using System;

namespace WinRTCOMServer
{
    internal static class Extension
    {
        public static bool IsValid(this IntPtr Ptr)
        {
            return Ptr != IntPtr.Zero && Ptr.ToInt64() != -1;
        }
    }
}
