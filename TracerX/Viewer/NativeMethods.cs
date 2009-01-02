using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TracerX.Viewer {
    internal static class NativeMethods {
        public delegate bool EnumWinCallBack(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.Dll")]
        public static extern int EnumChildWindows(IntPtr hWndParent, EnumWinCallBack callBackFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

    }
}
