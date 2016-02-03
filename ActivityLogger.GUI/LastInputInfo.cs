using System;
using System.Runtime.InteropServices;

namespace ActivityLogger.GUI
{
    // See: http://pinvoke.net/default.aspx/Structures/LASTINPUTINFO.html
    [StructLayout(LayoutKind.Sequential)]
    struct LASTINPUTINFO
    {
        public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 cbSize;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dwTime;
    }
}
