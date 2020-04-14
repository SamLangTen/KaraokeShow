using MusicBeePlugin.Window.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MusicBeePlugin.Window
{
    public partial class FormLyrics : Form
    {
        public FormLyrics()
        {
            InitializeComponent();
        }

        public void UpdateLayeredWindow(Bitmap bmp)
        {
            var screenDC = LayeredWindowWin32.GetDC(IntPtr.Zero);
            var hBitmap = IntPtr.Zero;
            var memDC = LayeredWindowWin32.CreateCompatibleDC(screenDC);
            var oldBits = IntPtr.Zero;
            try
            {
                var topLoc = new LayeredWindowWin32.Point(Left, Top);
                var bitmapSize = new LayeredWindowWin32.Size(bmp.Width, bmp.Height);
                var srcLoc = new LayeredWindowWin32.Point(0, 0);
                hBitmap = bmp.GetHbitmap(Color.FromArgb(0));
                oldBits = LayeredWindowWin32.SelectObject(memDC, hBitmap);
                var bfunc = new LayeredWindowWin32.BLENDFUNCTION()
                {
                    BlendOp = LayeredWindowWin32.AC_SRC_OVER,
                    AlphaFormat = LayeredWindowWin32.AC_SRC_ALPHA,
                    SourceConstantAlpha = 255,
                    BlendFlags = 0
                };
                LayeredWindowWin32.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitmapSize, memDC, ref srcLoc, Color.FromArgb(255, 255, 255).ToArgb(), ref bfunc, LayeredWindowWin32.ULW_ALPHA);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (hBitmap != IntPtr.Zero)
                {
                    LayeredWindowWin32.SelectObject(memDC, oldBits);
                    LayeredWindowWin32.DeleteObject(hBitmap);
                }
                LayeredWindowWin32.ReleaseDC(IntPtr.Zero, screenDC);
                LayeredWindowWin32.DeleteDC(memDC);
            }
            bmp.Dispose();
        }

        private void FormLyrics_Load(object sender, EventArgs e)
        {
            LayeredWindowWin32.SetWindowLong(Handle, LayeredWindowWin32.GWL_EXSTYLE, LayeredWindowWin32.GetWindowLong(Handle, LayeredWindowWin32.GWL_EXSTYLE) | LayeredWindowWin32.WS_EX_LAYERED);
        }
    }
}
