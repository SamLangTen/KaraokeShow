﻿using System;
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
    public class DestopLyrics : IDisposable
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
            var newIndex = SyncHelper.GetLyricIndex(milliseconds);
            int refreshingIndex;
            if (newIndex == -1)
                refreshingIndex = SyncHelper.GetNearNextIndex(milliseconds);
            else
                refreshingIndex = newIndex;

            if (refreshingIndex != LastIndex)
            {
                for (int i = 0; i < Configuration.Line; i++)
                {

                    int updatingIndex = refreshingIndex + i - (refreshingIndex % Configuration.Line);

                    if (updatingIndex < refreshingIndex) updatingIndex += Configuration.Line;
                    if (updatingIndex >= SyncHelper.SynchronousLyrics.Count)
                    {
                        updatingIndex -= Configuration.Line;
                    }
                    if (!LineInfo.ContainsKey(i + 1) || LineInfo[i + 1] != SyncHelper.SynchronousLyrics[updatingIndex])
                    {
                        LineInfo[i + 1] = SyncHelper.SynchronousLyrics[updatingIndex];
                    }

                    if (updatingIndex == refreshingIndex) continue;
                    var updatedBmp = LyricsGen.GetUpdatedStaticLyricsImage(LineInfo[i + 1].Content, i + 1, false);
                    RefreshWindow(updatedBmp);
                    updatedBmp?.Dispose();

                }
            }

            if (newIndex != -1)
            {
                //Draw this line percentage
                int thisLine = (newIndex % Configuration.Line) + 1;
                double percentage = SyncHelper.GetPercentage(milliseconds);
                var bmp = LyricsGen.GetUpdatedStaticLyricsImage(LineInfo[thisLine].Content, thisLine, true);
                RefreshWindow(bmp);
                bmp?.Dispose();
                LastIndex = newIndex;
            }
        }

        public void UpdateDynamic(int milliseconds)
        {
            var newIndex = SyncHelper.GetLyricIndex(milliseconds);
            int refreshingIndex;
            if (newIndex == -1)
                refreshingIndex = SyncHelper.GetNearNextIndex(milliseconds);
            else
                refreshingIndex = newIndex;

            if (refreshingIndex != LastIndex)
            {
                for (int i = 0; i < Configuration.Line; i++)
                {
                    int updatingIndex = refreshingIndex + i - (refreshingIndex % Configuration.Line);
                    if (updatingIndex < refreshingIndex) updatingIndex += Configuration.Line;
                    if (updatingIndex < SyncHelper.SynchronousLyrics.Count)
                    {
                        if (!LineInfo.ContainsKey(i + 1) || LineInfo[i + 1] != SyncHelper.SynchronousLyrics[updatingIndex])
                        {
                            LineInfo[i + 1] = SyncHelper.SynchronousLyrics[updatingIndex];
                            var updatedBmp = LyricsGen.GetUpdatedDynamicLyricsImage(LineInfo[i + 1].Content, i + 1, 0);
                            RefreshWindow(updatedBmp);
                            updatedBmp?.Dispose();
                        }
                    }
                }
            }

            if (newIndex != -1)
            {
                //Draw this line percentage
                int thisLine = (newIndex % Configuration.Line) + 1;
                double percentage = SyncHelper.GetPercentage(milliseconds);
                var bmp = LyricsGen.GetUpdatedDynamicLyricsImage(LineInfo[thisLine].Content, thisLine, percentage);
                RefreshWindow(bmp);
                bmp?.Dispose();
                LastIndex = newIndex;
            }

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


        public bool IsDisposed { get; private set; } = false;

        public void Dispose()
        {
            IsDisposed = true;
            LyricsGen.Dispose();
            MusicBeeForm.Invoke(new Action(() =>
            {
                FormLyrics.Dispose();
            }));

        }
    }
}
