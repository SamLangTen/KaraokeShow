using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MusicBeePlugin.Window.Helper
{

    public static class NativeMethods
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public Int32 x;
            public Int32 y;
            public Point(Int32 x, Int32 y)
            {
                this.x = x;
                this.y = y;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public Int32 cx;
            public Int32 cy;
            public Size(Int32 cx, Int32 cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ARGB
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        public const Int32 ULW_COLORKEY = 0x1;
        public const Int32 ULW_ALPHA = 0x2;
        public const Int32 ULW_OPAQUE = 0x4;
        public const byte AC_SRC_OVER = 0x0;
        public const byte AC_SRC_ALPHA = 0x1;
        public const Int32 GWL_EXSTYLE = -20;
        public const Int32 WS_EX_LAYERED = 0x80000;

        [DllImport("user32.dll", EntryPoint = "SetWindowLongA")]
        internal static extern Int32 SetWindowLong(IntPtr hWnd, Int32 nIndex, Int32 dwNewLong);

        [DllImport("user32.dll")]
        internal static extern Int32 GetWindowLong(IntPtr hWnd, Int32 nIndex);


        [DllImport("user32.dll")]
        internal static extern bool UpdateLayeredWindow(IntPtr hWnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);


        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hWnd);


        [DllImport("user32.dll")]
        internal static extern Int32 ReleaseDC(IntPtr hWnd, IntPtr hDC);


        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);


        [DllImport("gdi32.dll")]
        internal static extern bool DeleteDC(IntPtr hDC);



        [DllImport("gdi32.dll")]
        internal static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);



        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);


    }
}
