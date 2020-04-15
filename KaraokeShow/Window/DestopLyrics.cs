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
            LyricsGen = new DynamicLyricsGenerator();
            LyricsGen.Brush1 = new LinearGradientBrush(new Point(0, 0), new Point(0, 100), Configuration.BackColor1, Configuration.BackColor2);
            LyricsGen.Brush2 = new LinearGradientBrush(new Point(0, 0), new Point(0, 100), Configuration.ForeColor1, Configuration.ForeColor2);
            LyricsGen.OutlineBrush1 = new SolidBrush(Configuration.OutlineBackColor);
            LyricsGen.OutlineBrush2 = new SolidBrush(Configuration.OutlineForeColor);
            LyricsGen.OutlinePen1 = new Pen(LyricsGen.OutlineBrush1, Configuration.OutlineWidth);
            LyricsGen.OutlinePen2 = new Pen(LyricsGen.OutlineBrush2, Configuration.OutlineWidth);
            this.MusicBeeForm = MusicBeeForm;
        }

        public void ShowWindow()
        {
            MusicBeeForm.Invoke(new Action(() =>
            {
                FormLyrics.Show();
                FormLyrics.Location = new Point(Configuration.X, Configuration.Y);
            }));
        }

        public void HideWindow()
        {
            MusicBeeForm.Invoke(new Action(() =>
            {
                FormLyrics.Hide();
            }));
        }

        public void CloseWindow()
        {
            MusicBeeForm.Invoke(new Action(() =>
            {
                FormLyrics.Close();
            }));
        }

        public Point WindowLocation { get => FormLyrics.Location; }


        public void Update(int milliseconds)
        {
            if (Configuration.EnabledSliding)
                UpdateDynamic(milliseconds);
            else
                UpdateStatic(milliseconds);
        }

        public void UpdateStatic(int milliseconds)
        {
            if (LastIndex == -1)
            {
                for (int i = 1; i <= Configuration.Line && i <= SyncHelper.SynchronousLyrics.Count; i++)
                {
                    if (!LineInfo.ContainsKey(i) || LineInfo[i] != SyncHelper.SynchronousLyrics[i - 1])
                    {
                        LineInfo[i] = SyncHelper.SynchronousLyrics[i - 1];
                        var updatedBmp = LyricsGen.GetUpdatedStaticLyricsImage(LineInfo[i].Content, i);
                        RefreshWindow(updatedBmp);
                    }
                }
            }

            var newIndex = SyncHelper.GetLyricIndex(milliseconds);
            if (newIndex == -1) return;
            if (newIndex != LastIndex)
            {
                for (int i = 0; i < Configuration.Line; i++)
                {
                    int updatingIndex = newIndex + i - (newIndex % Configuration.Line);
                    if (updatingIndex < newIndex) updatingIndex += Configuration.Line;
                    if (updatingIndex < SyncHelper.SynchronousLyrics.Count)
                    {
                        if (LineInfo[i + 1] != SyncHelper.SynchronousLyrics[updatingIndex])
                        {
                            LineInfo[i + 1] = SyncHelper.SynchronousLyrics[updatingIndex];
                            var updatedBmp = LyricsGen.GetUpdatedStaticLyricsImage(LineInfo[i + 1].Content, i + 1);
                            RefreshWindow(updatedBmp);
                        }
                    }
                }
            }
            //Draw this line percentage
            int thisLine = (newIndex % Configuration.Line) + 1;
            double percentage = SyncHelper.GetPercentage(milliseconds);
            var bmp = LyricsGen.GetUpdatedStaticLyricsImage(LineInfo[thisLine].Content, thisLine);
            RefreshWindow(bmp);
            LastIndex = newIndex;
        }

        public void UpdateDynamic(int milliseconds)
        {
            if (LastIndex == -1)
            {
                for (int i = 1; i <= Configuration.Line && i <= SyncHelper.SynchronousLyrics.Count; i++)
                {
                    if (!LineInfo.ContainsKey(i) || LineInfo[i] != SyncHelper.SynchronousLyrics[i - 1])
                    {
                        LineInfo[i] = SyncHelper.SynchronousLyrics[i - 1];
                        var updatedBmp = LyricsGen.GetUpdatedDynamicLyricsImage(LineInfo[i].Content, i, 0);
                        RefreshWindow(updatedBmp);
                    }
                }
            }

            var newIndex = SyncHelper.GetLyricIndex(milliseconds);
            if (newIndex == -1) return;
            if (newIndex != LastIndex)
            {
                for (int i = 0; i < Configuration.Line; i++)
                {
                    int updatingIndex = newIndex + i - (newIndex % Configuration.Line);
                    if (updatingIndex < newIndex) updatingIndex += Configuration.Line;
                    if (updatingIndex < SyncHelper.SynchronousLyrics.Count)
                    {
                        if (LineInfo[i + 1] != SyncHelper.SynchronousLyrics[updatingIndex])
                        {
                            LineInfo[i + 1] = SyncHelper.SynchronousLyrics[updatingIndex];
                            var updatedBmp = LyricsGen.GetUpdatedDynamicLyricsImage(LineInfo[i + 1].Content, i + 1, 0);
                            RefreshWindow(updatedBmp);
                        }
                    }
                }
            }

            //Draw this line percentage
            int thisLine = (newIndex % Configuration.Line) + 1;
            double percentage = SyncHelper.GetPercentage(milliseconds);
            var bmp = LyricsGen.GetUpdatedDynamicLyricsImage(LineInfo[thisLine].Content, thisLine, percentage);
            RefreshWindow(bmp);
            LastIndex = newIndex;
        }

        private void RefreshWindow(Bitmap bmp)
        {
            MusicBeeForm.Invoke(new Action(() =>
            {
                if (bmp != null)
                {
                    //FormLyrics.Size = bmp.Size;
                    FormLyrics.UpdateLayeredWindow(bmp);

                }

            }));

        }

    }
}
