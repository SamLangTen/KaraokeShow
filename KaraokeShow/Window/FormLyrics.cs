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
        private Bitmap lastSentBitmap { get; set; }

        public FormLyrics()
        {
            InitializeComponent();
        }


        public void UpdateLayeredWindow(Bitmap bmp)
        {

            Bitmap layerBmp;
            if (!IsResizing)
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
            }

            g.Dispose();
            UpdateLayeredWindowLow(layerBmp);
            layerBmp.Dispose();
        }

        public void UpdateLayeredWindowLow(Bitmap bmp)
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
            LayeredWindowWin32.SetWindowLong(Handle, LayeredWindowWin32.GWL_EXSTYLE, LayeredWindowWin32.GetWindowLong(Handle, LayeredWindowWin32.GWL_EXSTYLE) | LayeredWindowWin32.WS_EX_LAYERED);
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
                    UpdateLayeredWindow(null);
                    Width = e.X;
                }
                else if (IsMoving)
                {
                    //Moving
                    Location = new Point(Location.X - mouseX + e.X, Location.Y - mouseY + e.Y);
                }

            }
        }


        #endregion

        private void FormLyrics_MouseHover(object sender, EventArgs e)
        {
            IsMouseIn = true;
        }

        private void FormLyrics_MouseLeave(object sender, EventArgs e)
        {
            IsMouseIn = false;
        }
    }
}
