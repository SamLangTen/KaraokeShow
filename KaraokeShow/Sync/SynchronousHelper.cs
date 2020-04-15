﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Sync
{
    public class SynchronousHelper
    {
        public List<SynchronousLyricItem> SynchronousLyrics { get; set; }

        public SynchronousHelper(List<SynchronousLyricItem> lyrics)
        {
            SynchronousLyrics = lyrics;
        }

        public int GetLyricIndex(int milliseconds)
        {
            var dt = new DateTime(1, 1, 1, 0, 0, 0, 0);
            dt = dt.AddMilliseconds(milliseconds);
            var lyrics = SynchronousLyrics.FirstOrDefault(l => l.StartTime <= dt && dt <= l.EndTime);
            //If current position does not exists lyrics, select next one.
            if (lyrics == null)
                lyrics = SynchronousLyrics.FirstOrDefault(l => dt < l.StartTime);
            return SynchronousLyrics.IndexOf(lyrics);
        }
        public double GetPercentage(int milliseconds)
        {
            var dt = new DateTime(1, 1, 1, 0, 0, 0, 0);
            dt = dt.AddMilliseconds(milliseconds);
            var lyrics = SynchronousLyrics.FirstOrDefault(l => l.StartTime <= dt && dt <= l.EndTime);
            if (lyrics == null) return 0;
            return (dt - lyrics.StartTime).TotalMilliseconds / (lyrics.EndTime - lyrics.StartTime).TotalMilliseconds;
        }

    }
}
