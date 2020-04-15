using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Window.Helper
{
    public class DynamicLyricsGenerator
    {
        private Font TextFont { get => Configuration.TextFont; }
        private int WindowWidth { get => Configuration.Width; }
        private int LineHeight { get => (int)GetCorrectFontSize("Test", TextFont).Height; }
        private int Line { get => Configuration.Line; }
        private int OutLineWidth { get => Configuration.OutlineWidth; }
        private int BlurRadial { get => Configuration.BlurRadial; }
        public Brush Brush1 { get; set; }
        public Brush Brush2 { get; set; }
        public Brush OutlineBrush1 { get; set; }
        public Brush OutlineBrush2 { get; set; }
        public Pen OutlinePen1 { get; set; }
        public Pen OutlinePen2 { get; set; }

        private Dictionary<string, Bitmap> ForeBitmapCache { get; set; } = new Dictionary<string, Bitmap>();
        private Dictionary<string, Bitmap> ForeBlurBitmapCache { get; set; } = new Dictionary<string, Bitmap>();
        private Dictionary<string, Bitmap> BackBitmapCache { get; set; } = new Dictionary<string, Bitmap>();
        private Dictionary<string, Bitmap> BackBlurBitmapCache { get; set; } = new Dictionary<string, Bitmap>();
        private Dictionary<int, Bitmap> LineBitmapCache { get; set; } = new Dictionary<int, Bitmap>();
        private Font LastFont { get; set; }


        public Bitmap GetUpdatedStaticLyricsImage(string text, int line)
        {
            try
            {
                //Draw This Line
                var fz = GetCorrectFontSize(text, TextFont);
                var updatingBitmap = new Bitmap((int)fz.Width, LineHeight * Line);
                var g = Graphics.FromImage(updatingBitmap);
                DrawDynamicLyricBeforeRoll(g, text, line, 0);
                g.Dispose();
                LineBitmapCache[line] = updatingBitmap;

                //Check max width
                var maxWidth = 0;
                for (int i = 1; i <= Line; i++)
                {
                    if (!LineBitmapCache.ContainsKey(i)) continue;
                    if (LineBitmapCache[i].Width > maxWidth) maxWidth = LineBitmapCache[i].Width;
                }
                //Paint
                var bmp = new Bitmap(maxWidth, LineHeight * Line);
                var graphics = Graphics.FromImage(bmp);
                for (int i = 1; i <= Line; i++)
                {
                    if (!LineBitmapCache.ContainsKey(i)) continue;
                    graphics.DrawImage(LineBitmapCache[i], new Point(0, 0));
                }
                graphics.Dispose();
                return bmp;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public Bitmap GetUpdatedDynamicLyricsImage(string text, int line, double percentage)
        {
            try
            {
                var updatingBitmap = DrawDynamicLyric(text, line, percentage);
                LineBitmapCache[line] = updatingBitmap;

                var bmp = new Bitmap(WindowWidth, LineHeight * Line);
                var graphics = Graphics.FromImage(bmp);
                for (int i = 1; i <= Line; i++)
                {
                    if (!LineBitmapCache.ContainsKey(i)) continue;
                    graphics.DrawImage(LineBitmapCache[i], new Point(0, 0));
                }
                graphics.Dispose();
                return bmp;
            }
            catch (Exception)
            {
            }

            return null;
        }


        private SizeF GetCorrectFontSize(string text, Font font)
        {
            var preGraphics = Graphics.FromImage(new Bitmap(10, 10));
            var fontSize = preGraphics.MeasureString(text, font);
            //var scalePercentage = (preGraphics.DpiX / 0.96) / 100;
            //var y = scalePercentage / 3 * 4;
            //fontSize = new SizeF((float)(fontSize.Width / y), (float)(fontSize.Height / y));
            //preGraphics.Dispose();
            return fontSize;
        }

        private Bitmap DrawDynamicLyric(string text, int line, double percentage)
        {
            //Clear
            var rtnBmp = new Bitmap(WindowWidth, LineHeight * Line);
            var graphics = Graphics.FromImage(rtnBmp);
            var textSize = GetCorrectFontSize(text, TextFont);
            var beforeRollPercentage = WindowWidth / 2 / textSize.Width;
            var afterRollPercentage = 1 - beforeRollPercentage;
            if (percentage < beforeRollPercentage)
                DrawDynamicLyricBeforeRoll(graphics, text, line, percentage);
            else if (beforeRollPercentage <= percentage && percentage <= afterRollPercentage)
                DrawDynamicLyricWhileRoll(graphics, text, line, percentage);
            else if (afterRollPercentage < percentage)
                DrawDynamicLyricAfterRoll(graphics, text, line, percentage);
            graphics.Dispose();
            return rtnBmp;
        }


        private Bitmap DrawForeground(string text, Size fontSize, double percentage)
        {
            Bitmap fullBmp = null;
            if (!ForeBitmapCache.ContainsKey(text))
            {
                fullBmp = new Bitmap(fontSize.Width, fontSize.Height);
                var fg = Graphics.FromImage(fullBmp);
                fg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                fg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                fg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                var path = new GraphicsPath();
                path.AddString(text, TextFont.FontFamily, (int)TextFont.Style, fg.DpiY * TextFont.Size / 72, new Point(0, 0), new StringFormat());
                fg.FillPath(Brush2, path);
                fg.DrawPath(OutlinePen2, path);

                fg.Dispose();
                ForeBitmapCache[text] = fullBmp;
            }
            else
            {
                fullBmp = ForeBitmapCache[text];
            }

            var bmp = new Bitmap((int)(fontSize.Width * percentage), fontSize.Height);
            var g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.DrawImage(fullBmp, new PointF(0, 0));
            g.Dispose();
            return bmp;
        }

        private Bitmap DrawBackground(string text, Size fontSize)
        {
            Bitmap fullBmp = null;
            if (!BackBitmapCache.ContainsKey(text))
            {
                fullBmp = new Bitmap(fontSize.Width, fontSize.Height);
                var fg = Graphics.FromImage(fullBmp);
                fg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                fg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                fg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                var path = new GraphicsPath();
                path.AddString(text, TextFont.FontFamily, (int)TextFont.Style, fg.DpiY * TextFont.Size / 72, new Point(0, 0), new StringFormat());
                fg.FillPath(Brush1, path);
                fg.DrawPath(OutlinePen1, path);

                fg.Dispose();
                BackBitmapCache[text] = fullBmp;
            }
            else
            {
                fullBmp = BackBitmapCache[text];
            }
            return fullBmp;
        }

        private Bitmap DrawOutlineBackground(string text, Size fontSize)
        {
            if (BackBlurBitmapCache.ContainsKey(text))
                return BackBlurBitmapCache[text];

            var bmp = new Bitmap(fontSize.Width, fontSize.Height);
            var g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.DrawString(text, TextFont, OutlineBrush1, new PointF(0, 0));
            g.Dispose();

            if (BlurRadial > 0)
            {
                var blur = new GaussianBlur(bmp);
                bmp = blur.Process(BlurRadial);
            }
            BackBlurBitmapCache[text] = bmp;
            return BackBlurBitmapCache[text];
        }

        private Bitmap DrawBlurForeground(string text, Size fontSize, double percentage)
        {
            Bitmap fullBmp = null;

            if (!ForeBlurBitmapCache.ContainsKey(text))
            {
                fullBmp = new Bitmap(fontSize.Width, fontSize.Height);
                var fg = Graphics.FromImage(fullBmp);
                fg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                fg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                fg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                fg.DrawString(text, TextFont, OutlineBrush2, new PointF(0, 0));
                fg.Dispose();

                if (BlurRadial > 0)
                {
                    var blur = new GaussianBlur(fullBmp);
                    fullBmp = blur.Process(BlurRadial);
                }
                ForeBlurBitmapCache[text] = fullBmp;
                fullBmp = ForeBlurBitmapCache[text];
            }
            else
            {
                fullBmp = ForeBlurBitmapCache[text];
            }
            var bmp = new Bitmap((int)(fontSize.Width * percentage), fontSize.Height);
            var g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.DrawImage(fullBmp, new PointF(0, 0));
            g.Dispose();
            return bmp;
        }

        private void DrawDynamicLyricBeforeRoll(Graphics graphics, string text, int line, double percentage)
        {
            var textSize = GetCorrectFontSize(text, TextFont);
            var y = textSize.Height * (line - 1);

            //Draw background
            var blurBmp = DrawOutlineBackground(text, textSize.ToSize());
            graphics.DrawImage(blurBmp, new Point(0, (int)y));
            var backBmp = DrawBackground(text, textSize.ToSize());
            graphics.DrawImage(backBmp, new Point(0, (int)y));

            //Draw foreground
            if (percentage != 0)
            {
                var fBlurBmp = DrawBlurForeground(text, textSize.ToSize(), percentage);
                var bmp = DrawForeground(text, textSize.ToSize(), percentage);
                graphics.DrawImage(fBlurBmp, new Point(0, (int)y));
                graphics.DrawImage(bmp, new Point(0, (int)y));
                bmp.Dispose();
                fBlurBmp.Dispose();
            }


        }
        private void DrawDynamicLyricWhileRoll(Graphics graphics, string text, int line, double percentage)
        {
            var textSize = GetCorrectFontSize(text, TextFont);
            var y = textSize.Height * (line - 1);
            var x = (float)(WindowWidth / 2 - textSize.Width * percentage);

            //Draw background
            var blurBmp = DrawOutlineBackground(text, textSize.ToSize());
            graphics.DrawImage(blurBmp, new Point((int)x, (int)y));
            var backBmp = DrawBackground(text, textSize.ToSize());
            graphics.DrawImage(backBmp, new Point((int)x, (int)y));

            //Draw foreground
            var bmp = DrawForeground(text, textSize.ToSize(), percentage);
            var fBlurBmp = DrawBlurForeground(text, textSize.ToSize(), percentage);
            graphics.DrawImage(fBlurBmp, new Point((int)x, (int)y));
            graphics.DrawImage(bmp, new Point((int)x, (int)y));
            bmp.Dispose();
            fBlurBmp.Dispose();
        }
        private void DrawDynamicLyricAfterRoll(Graphics graphics, string text, int line, double percentage)
        {
            var textSize = GetCorrectFontSize(text, TextFont);
            var y = textSize.Height * (line - 1);
            var x = (WindowWidth - textSize.Width);
            if (x > 0) x = 0;

            //Draw background
            var blurBmp = DrawOutlineBackground(text, textSize.ToSize());
            graphics.DrawImage(blurBmp, new Point((int)x, (int)y));
            var backBmp = DrawBackground(text, textSize.ToSize());
            graphics.DrawImage(backBmp, new Point((int)x, (int)y));

            //Draw foreground
            var bmp = DrawForeground(text, textSize.ToSize(), percentage);
            var fBlurBmp = DrawBlurForeground(text, textSize.ToSize(), percentage);
            graphics.DrawImage(fBlurBmp, new Point((int)x, (int)y));
            graphics.DrawImage(bmp, new Point((int)x, (int)y));
            fBlurBmp.Dispose();
            bmp.Dispose();
        }


    }
}
