using System;
using System.Collections.Generic;
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


    }
}
