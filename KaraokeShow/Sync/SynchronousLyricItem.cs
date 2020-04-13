using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Sync
{
    public class SynchronousLyricItem
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Content { get; set; }
    }
}
