using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LyricsBar
{
    enum RollingState
    {
        Before,
        Rolling,
        After
    }
    class DynamicLyricsPainter
    {
        public Graphics Graphics { get; private set; }
        public Brush Brush1 { get; set; } = new SolidBrush(Color.Green);
        public Brush Brush2 { get; set; } = new SolidBrush(Color.Red);
        public Font Font { get; set; } = new Font("微软雅黑", 40);

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

        public DynamicLyricsPainter(Graphics graphics)
        {
            Graphics = graphics;
            Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

        }

        public DynamicLyricsPainter(Font font, int windowLength, int line)
        {
            var size = GetCorrectFontSize("Test", font);
            var height = size.Height * line;
            Graphics = Graphics.FromImage(new Bitmap(windowLength, (int)height));
            Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        }

        public void DrawLyric(string text, int line, double percentage)
        {
            Graphics.Clear(Color.White);
            var textSize = GetCorrectFontSize(text, Font);
            var beforeRollPercentage = Graphics.VisibleClipBounds.Width / 2 / textSize.Width;
            var afterRollPercentage = 1 - beforeRollPercentage;
            if (percentage < beforeRollPercentage)
                DrawLyricBeforeRoll(text, line, percentage);
            else if (beforeRollPercentage <= percentage && percentage <= afterRollPercentage)
                DrawLyricWhileRoll(text, line, percentage);
            else if (afterRollPercentage < percentage)
                DrawLyricAfterRoll(text, line, percentage);
        }

        public RollingState GetRollingState(string text, int line, double percentage)
        {
            //Graphics.Clear(Color.White);
            var textSize = GetCorrectFontSize(text, Font);
            var beforeRollPercentage = Graphics.VisibleClipBounds.Width / 2 / textSize.Width;
            var afterRollPercentage = 1 - beforeRollPercentage;
            if (percentage < beforeRollPercentage)
                return RollingState.Before;
            else if (beforeRollPercentage <= percentage && percentage <= afterRollPercentage)
                return RollingState.Rolling;
            else if (afterRollPercentage < percentage)
                return RollingState.After;
            return RollingState.Rolling;
        }

        private Bitmap DrawForeground(string text, Size fontSize, double percentage)
        {
            var bmp = new Bitmap((int)(fontSize.Width * percentage), fontSize.Height);
            var g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.DrawString(text, Font, Brush2, new PointF(0, 0));
            g.Dispose();
            return bmp;
        }

        private void DrawLyricBeforeRoll(string text, int line, double percentage)
        {
            var textSize = GetCorrectFontSize(text, Font);
            var y = textSize.Height * (line - 1);

            //Draw background
            Graphics.DrawRectangle(new Pen(Brush1), new Rectangle(0, (int)y, (int)textSize.Width, (int)textSize.Height));
            Graphics.DrawString(text, Font, Brush1, new PointF(0, y));

            //Draw foreground
            if (percentage > 0)
            {
                Graphics.DrawRectangle(new Pen(Brush2), new Rectangle(0, (int)y, (int)(textSize.Width * percentage), (int)textSize.Height));
                //Graphics.DrawString(text, Font, Brush2, new RectangleF(new PointF(0, y), new SizeF((float)(textSize.Width * percentage), textSize.Height)));
                var bmp = DrawForeground(text, textSize.ToSize(), percentage);
                Graphics.DrawImage(bmp, new Point(0, (int)y));
                bmp.Dispose();
            }

        }
        private void DrawLyricWhileRoll(string text, int line, double percentage)
        {
            var textSize = GetCorrectFontSize(text, Font);
            var y = textSize.Height * (line - 1);
            var x = (float)(Graphics.VisibleClipBounds.Width / 2 - textSize.Width * percentage);

            //Draw background
            Graphics.DrawRectangle(new Pen(Brush1), new Rectangle((int)x, (int)y, (int)textSize.Width, (int)textSize.Height));
            Graphics.DrawString(text, Font, Brush1, new PointF(x, y));
            //Draw foreground
            Graphics.DrawRectangle(new Pen(Brush2), new Rectangle((int)x, (int)y, (int)(textSize.Width * percentage), (int)textSize.Height));
            //Graphics.DrawString(text, Font, Brush2, new RectangleF(new PointF(x, y), new SizeF((float)(textSize.Width * percentage), textSize.Height)));
            var bmp = DrawForeground(text, textSize.ToSize(), percentage);
            Graphics.DrawImage(bmp, new Point((int)x, (int)y));
            bmp.Dispose();
        }
        private void DrawLyricAfterRoll(string text, int line, double percentage)
        {
            var textSize = GetCorrectFontSize(text, Font);
            var y = textSize.Height * (line - 1);
            var x = (Graphics.VisibleClipBounds.Width - textSize.Width);
            if (x > 0) x = 0;

            //Draw background
            Graphics.DrawRectangle(new Pen(Brush1), new Rectangle((int)x, (int)y, (int)textSize.Width, (int)textSize.Height));
            Graphics.DrawString(text, Font, Brush1, new PointF(x, y));
            //Draw foreground
            Graphics.DrawRectangle(new Pen(Brush2), new Rectangle((int)x, (int)y, (int)(textSize.Width * percentage), (int)textSize.Height));
            //Graphics.DrawString(text, Font, Brush2, new RectangleF(new PointF(x, y), new SizeF((float)(textSize.Width * percentage), textSize.Height)));
            var bmp = DrawForeground(text, textSize.ToSize(), percentage);
            Graphics.DrawImage(bmp, new Point((int)x, (int)y));
            bmp.Dispose();
        }

    }
}
