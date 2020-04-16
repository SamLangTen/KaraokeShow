using MusicBeePlugin.Window.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MusicBeePlugin.Window
{
    public partial class FormLyrics : Form
    {
        private Dictionary<Size, Bitmap> BackgroundBitmapCache { get; set; } = new Dictionary<Size, Bitmap>();
        private Bitmap lastBitmapCache { get; set; } = null;

        public FormLyrics()
        {
            InitializeComponent();
        }


        public void UpdateLayeredWindow(Bitmap bmp)
        {

            Bitmap layerBmp;
            if (bmp != null)
            {
                Size = bmp.Size;
                layerBmp = new Bitmap(bmp.Width, bmp.Height);
            }
            else
            {
                layerBmp = new Bitmap(Size.Width, Size.Height);
            }

            var g = Graphics.FromImage(layerBmp);

            if (IsMouseIn)
            {
                g.DrawImage(GetBackgroundImage(), new Point(0, 0));
            }

            if (bmp != null && !IsResizing)
            {
                g.DrawImage(bmp, new Point(0, 0));
                lastBitmapCache?.Dispose();
                lastBitmapCache = (Bitmap)bmp.Clone();
            }
            else if (lastBitmapCache != null)
            {
                g.DrawImage(lastBitmapCache, new Point(0, 0));
            }

            g.Dispose();
            UpdateLayeredWindowLow(layerBmp);
            layerBmp.Dispose();
        }

        public void UpdateLayeredWindowLow(Bitmap bmp)
        {
            var screenDC = NativeMethods.GetDC(IntPtr.Zero);
            var hBitmap = IntPtr.Zero;
            var memDC = NativeMethods.CreateCompatibleDC(screenDC);
            var oldBits = IntPtr.Zero;
            try
            {
                var topLoc = new NativeMethods.Point(Left, Top);
                var bitmapSize = new NativeMethods.Size(bmp.Width, bmp.Height);
                var srcLoc = new NativeMethods.Point(0, 0);
                hBitmap = bmp.GetHbitmap(Color.FromArgb(0));
                oldBits = NativeMethods.SelectObject(memDC, hBitmap);
                var bfunc = new NativeMethods.BLENDFUNCTION()
                {
                    BlendOp = NativeMethods.AC_SRC_OVER,
                    AlphaFormat = NativeMethods.AC_SRC_ALPHA,
                    SourceConstantAlpha = 255,
                    BlendFlags = 0
                };
                NativeMethods.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitmapSize, memDC, ref srcLoc, Color.FromArgb(255, 255, 255).ToArgb(), ref bfunc, NativeMethods.ULW_ALPHA);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (hBitmap != IntPtr.Zero)
                {
                    NativeMethods.SelectObject(memDC, oldBits);
                    NativeMethods.DeleteObject(hBitmap);
                }
                NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);
                NativeMethods.DeleteDC(memDC);
            }
            bmp.Dispose();
        }

        private Bitmap GetBackgroundImage()
        {
            if (BackgroundBitmapCache.ContainsKey(Size))
                return BackgroundBitmapCache[Size];

            BackgroundBitmapCache.Clear();
            var backBmp = new Bitmap(Width, Height);
            var g = Graphics.FromImage(backBmp);
            var backColor = Color.FromArgb(100, Color.Gray);
            g.Clear(backColor);
            g.Dispose();
            var blur = new GaussianBlur(backBmp);
            backBmp = blur.Process(3);
            BackgroundBitmapCache[Size] = backBmp;
            return backBmp;
        }

        private void FormLyrics_Load(object sender, EventArgs e)
        {
            NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE) | NativeMethods.WS_EX_LAYERED);
        }

        #region "For Move & Resize"
        private int mouseX;
        private int mouseY;
        private bool IsMoving { get; set; } = false;
        private bool IsResizing { get; set; } = false;
        private bool IsMouseIn { get; set; } = false;
        private void FormLyrics_MouseDown(object sender, MouseEventArgs e)
        {
            if (Width - e.X <= 20)
            {
                IsResizing = true;
            }
            else
            {
                IsMoving = true;
            }
            mouseX = e.X;
            mouseY = e.Y;
        }

        private void FormLyrics_MouseUp(object sender, MouseEventArgs e)
        {
            IsResizing = false;
            IsMoving = false;
        }

        private void FormLyrics_MouseMove(object sender, MouseEventArgs e)
        {
            if (Width - e.X <= 20)
            {
                Cursor = Cursors.SizeWE;
            }
            else
            {
                Cursor = Cursors.SizeAll;
            }


            if (e.Button == MouseButtons.Left)
            {
                if (IsResizing) //Right area is resize strip
                {
                    //Resize
                    Configuration.Width = e.X;
                    Width = e.X;
                    UpdateLayeredWindow(null);
                }
                else if (IsMoving)
                {
                    //Moving
                    Location = new Point(Location.X - mouseX + e.X, Location.Y - mouseY + e.Y);
                    Configuration.X = Location.X;
                    Configuration.Y = Location.Y;
                }

            }
        }


        #endregion

        private void FormLyrics_MouseHover(object sender, EventArgs e)
        {
            IsMouseIn = true;
            UpdateLayeredWindow(null);
        }

        private void FormLyrics_MouseLeave(object sender, EventArgs e)
        {
            IsMouseIn = false;
            UpdateLayeredWindow(null);
        }
    }
}
