using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using MusicBeePlugin.Sync;
using MusicBeePlugin.Config;
using MusicBeePlugin.Window.Helper;
using System.Diagnostics;
using System.Windows.Forms;

namespace MusicBeePlugin.Window
{
    public class DestopLyrics
    {

        private SynchronousHelper SyncHelper { get; set; }
        private DynamicLyricsGenerator LyricsGen { get; set; }
        private Dictionary<int, SynchronousLyricItem> LineInfo { get; set; } = new Dictionary<int, SynchronousLyricItem>();
        private int LastIndex { get; set; } = -1;
        private Form MusicBeeForm { get; set; }
        private FormLyrics _FormLyrics;
        private FormLyrics FormLyrics
        {
            get
            {
                if (_FormLyrics == null || _FormLyrics.IsDisposed)
                    _FormLyrics = new FormLyrics();
                return _FormLyrics;
            }
        }

        public DestopLyrics(List<SynchronousLyricItem> lyrics, Form MusicBeeForm)
        {
            SyncHelper = new SynchronousHelper(lyrics);
            LyricsGen = new DynamicLyricsGenerator(Configuration.TextFont, Configuration.Width, Configuration.Line);
            LyricsGen.Brush1 = new LinearGradientBrush(new Point(0, 0), new Point(0, Configuration.TextFont.Height * Configuration.Line), Configuration.BackColor1, Configuration.BackColor2);
            LyricsGen.Brush2 = new LinearGradientBrush(new Point(0, 0), new Point(0, Configuration.TextFont.Height * Configuration.Line), Configuration.ForeColor2, Configuration.ForeColor2);
            LyricsGen.OutlineBrush1 = new SolidBrush(Configuration.OutlineBackColor);
            LyricsGen.OutlineBrush2 = new SolidBrush(Configuration.OutlineForeColor);
            this.MusicBeeForm = MusicBeeForm;
        }

        public void ShowWindow()
        {
            MusicBeeForm.Invoke(new Action(() =>
            {
                FormLyrics.Show();
            }));
        }

        public void HideWindow()
        {
            MusicBeeForm.Invoke(new Action(() =>
            {
                FormLyrics.Hide();
            }));
        }

        public void Update(int milliseconds)
        {
            //Debug.WriteLine($"lastIndex {lastIndex}");
            if (LastIndex == -1)
            {
                for (int i = 1; i <= Configuration.Line && i <= SyncHelper.SynchronousLyrics.Count; i++)
                {
                    LineInfo[i] = SyncHelper.SynchronousLyrics[i - 1];
                    var updatedBmp = LyricsGen.GetUpdatedLyricsImage(LineInfo[i].Content, i, 0);
                    RefreshWindow(updatedBmp);
                    Debug.WriteLine($"Updated Line {i}: {LineInfo[i].Content}");
                }
            }

            var newIndex = SyncHelper.GetLyricIndex(milliseconds);
            if (newIndex == -1) return;
            if (newIndex > LastIndex && LastIndex != -1)
            {
                //Should update last outdated lyrics line
                int outdatedLine = (LastIndex % Configuration.Line) + 1;
                int outdatedIndex = SyncHelper.SynchronousLyrics.IndexOf(LineInfo[outdatedLine]);
                if (outdatedIndex < newIndex)
                {
                    //Set outdated line to next new line
                    if (newIndex + (Configuration.Line - 1) < SyncHelper.SynchronousLyrics.Count)
                    {
                        LineInfo[outdatedLine] = SyncHelper.SynchronousLyrics[newIndex + (Configuration.Line - 1)];
                        var updatedBmp = LyricsGen.GetUpdatedLyricsImage(LineInfo[outdatedLine].Content, outdatedLine, 0);
                        RefreshWindow(updatedBmp);
                        Debug.WriteLine($"Updated Line {outdatedLine}: {LineInfo[outdatedLine].Content}");
                    }
                }
            }

            //Draw this line percentage
            int thisLine = (newIndex % Configuration.Line) + 1;
            double percentage = SyncHelper.GetPercentage(milliseconds);
            var bmp = LyricsGen.GetUpdatedLyricsImage(LineInfo[thisLine].Content, thisLine, percentage);
            RefreshWindow(bmp);
            //Debug.WriteLine($"Updated Percentage {thisLine}: {lineInfo[thisLine].Content}, {percentage}");
            LastIndex = newIndex;
        }

        private void RefreshWindow(Bitmap bmp)
        {
            MusicBeeForm.Invoke(new Action(() =>
            {
                if (bmp != null)
                {
                    FormLyrics.BackgroundImage = bmp;
                    FormLyrics.Size = bmp.Size;
                }
            
            }));
           
        }

    }
}
