using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Avalonia.Win.Private
{
    internal static class N
    {
        internal class F
        {
            public const int GWL_STYLE = -16;
            public const int GWL_EXSTYLE = -20;
            public const int GWL_WNDPROC = -4;
            public const int GWL_HWNDPARENT = -8;

            public const uint WS_EX_COMPOSITED = 0x02000000;
            public const uint WS_EX_CONTROLPARENT = 0x00010000;
            public const uint WS_EX_LAYERED = 0x00080000;

            public const uint WS_CLIPCHILDREN = 0x2000000;
            public const uint WS_CLIPSIBLINGS = 0x4000000;
            public const uint WS_CHILD = 0x40000000;

            public const int GCL_STYLE = -26;

            public const int LWA_COLORKEY = 0x1;

            public const uint WM_ERASEBKGND = 0x0014;
            public const uint WM_KILLFOCUS = 0x0008;
        }

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            SWP_ASYNCWINDOWPOS = 0x4000,
            SWP_DEFERERASE = 0x2000,
            SWP_DRAWFRAME = 0x0020,
            SWP_FRAMECHANGED = 0x0020,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOACTIVATE = 0x0010,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOMOVE = 0x0002,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOREDRAW = 0x0008,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_NOSIZE = 0x0001,
            SWP_NOZORDER = 0x0004,
            SWP_SHOWWINDOW = 0x0040,

            SWP_RESIZE = SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOZORDER,

            SWP_NORESIZE = SWP_NOSIZE | SWP_NOZORDER | SWP_NOMOVE | SWP_NOACTIVATE
        }

        public enum ShowWindowCommand
        {
            Hide = 0,
            Normal = 1,
            ShowMinimized = 2,
            Maximize = 3,
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        public static extern bool SetParent(IntPtr hWnd, IntPtr hWndNewParent);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong32b(IntPtr hWnd, int nIndex, uint value);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLong64b(IntPtr hWnd, int nIndex, IntPtr value);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, uint value)
        {
            if (IntPtr.Size == 4)
            {
                return new IntPtr(SetWindowLong32b(hWnd, nIndex, value));
            }
            else
            {
                return SetWindowLong64b(hWnd, nIndex, new IntPtr((uint)value));
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong32b(IntPtr hWnd, int nIndex);

        public static uint GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLong32b(hWnd, nIndex);
            }
            else
            {
                return (uint)GetWindowLongPtr(hWnd, nIndex).ToInt32();
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr handle)
        {
            if (IntPtr.Size == 4)
            {
                return new IntPtr(SetWindowLong32b(hWnd, nIndex, (uint)handle.ToInt32()));
            }
            else
            {
                return SetWindowLong64b(hWnd, nIndex, handle);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(int dwExStyle, uint lpClassName, string lpWindowName, uint dwStyle,
                                                      int x, int y, int nWidth, int nHeight,
                                                      IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);

        [DllImport("user32", ExactSpelling = true, SetLastError = true)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] ref N.RECT rect, [MarshalAs(UnmanagedType.U4)] int cPoints);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
    }
}