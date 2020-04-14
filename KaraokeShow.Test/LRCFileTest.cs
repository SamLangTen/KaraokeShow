using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicBeePlugin.Parser;

namespace KaraokeShow.Test
{
    [TestClass]
    public class LRCFileTest
    {
        [TestMethod]
        public void TestLRCParse()
        {
            var lrc = new LRCFile(Properties.Resources.LRCFileText);
            Assert.IsTrue(lrc.Album == "Overfly");
            Assert.IsTrue(lrc.Artist == "春奈るな");
            Assert.IsTrue(lrc.Author == "Saku");
            Assert.IsTrue(lrc.LRCMaker == "Samersions");
            Assert.IsTrue(lrc.Editor == "ASS2LRC");
            Assert.IsTrue(lrc.Title == "Overfly （TV size ver.）");
            Assert.IsTrue(lrc.Lyrics.Count == 20);
        }

        [TestMethod]
        public void TestSynchronousLyrics()
        {
            var lrc = new LRCFile(Properties.Resources.LRCFileText);
            var lyrics = lrc.ToSynchronousLyrics(96000);
            var lyricsText = from l in lyrics select $"[{l.StartTime.ToString("mm:ss.ff")}=>{l.EndTime.ToString("mm:ss.ff")}]{l.Content}";
            Console.WriteLine(string.Join("\n", lyricsText));
        }
    }
}
