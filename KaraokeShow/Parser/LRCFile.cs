using MusicBeePlugin.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MusicBeePlugin.Parser
{
    public class LRCFile
    {
        public string Album { get; set; }
        public string Artist { get; set; }
        public string Author { get; set; }
        public string LRCMaker { get; set; }
        public int Offset { get; set; }
        public string Editor { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public List<LRCItem> Lyrics { get; set; }
        private bool IgnoreOffset { get; set; }

        public LRCFile(string lrcText, bool ignoreOffset = true)
        {
            try
            {
                Parse(lrcText, ignoreOffset);
                Lyrics.Sort((a, b) => a.Time.CompareTo(b.Time));
            }
            catch (Exception)
            {
            }
        }

        private void Parse(string lrcText, bool ignoreOffset = true)
        {
            var timestampRegex = new Regex("^\\[\\d(\\d)?:\\d\\d(\\.\\d\\d(\\d)?)?\\]");
            Func<string, DateTime> timestampToDateTime = t =>
             {
                 var split1 = t.Replace("[", "").Replace("]", "").Split(':');
                 var minute = int.Parse(split1[0]);
                 var split2 = split1[1].Split('.');
                 var second = int.Parse(split2[0]);
                 var millsec = 0;
                 if (split2.Length == 2)
                     millsec = int.Parse(split2[1]) * (int)Math.Pow(10, 3 - split2[1].Length);
                 var dt = new DateTime(1, 1, 1, 0, minute, second, millsec);
                 return dt;
             };
            //Parse IDTag
            Func<string, string, string> dynamicTagToTag = (tag, input) =>
             {
                 var regex = new Regex($"(?<=^\\[{tag}\\:)[^\\]]*(?=\\])", RegexOptions.Multiline);
                 return regex.Match(input).Value;
             };
            Artist = dynamicTagToTag("ar", lrcText);
            Album = dynamicTagToTag("al", lrcText);
            Author = dynamicTagToTag("au", lrcText);
            LRCMaker = dynamicTagToTag("by", lrcText);
            var offsetText = dynamicTagToTag("offset", lrcText);
            Offset = offsetText == "" ? 0 : int.Parse(offsetText);
            Editor = dynamicTagToTag("re", lrcText);
            Version = dynamicTagToTag("ve", lrcText);
            Title = dynamicTagToTag("ti", lrcText);
            //Parse Lyrics
            Lyrics = (from line in Regex.Split(lrcText, "\\r\\n|\\r|\\n")
                      where timestampRegex.Match(line).Success
                      select new LRCItem()
                      {
                          Time = timestampToDateTime(timestampRegex.Match(line).Value),
                          Lyric = timestampRegex.Replace(line, "")
                      }).ToList();
            if (!IgnoreOffset)
            {
                Lyrics = (from i in Lyrics
                          select new LRCItem()
                          {
                              Time = i.Time.AddMilliseconds((int)Offset),
                              Lyric = i.Lyric
                          }).ToList();
            }
        }

        public List<SynchronousLyricItem> ToSynchronousLyrics(int musicLengthMillisecond)
        {
            return Lyrics.Select(l =>
            {
                var sli = new SynchronousLyricItem()
                {
                    StartTime = l.Time,
                    Content = l.Lyric
                };
                if (Lyrics.LastOrDefault() == l)
                {
                    var beginDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0);
                    beginDateTime = beginDateTime.AddMilliseconds(musicLengthMillisecond);
                    if (beginDateTime > l.Time)
                        sli.EndTime = beginDateTime;
                    else
                        sli.EndTime = sli.StartTime + new TimeSpan(0, 0, 10);
                }
                else
                {
                    sli.EndTime = Lyrics.Find(n => n.Time > l.Time).Time;
                }
                return sli;
            }).Where(l => l.Content.Trim() != "").ToList();
        }
    }

    public class LRCItem
    {
        public DateTime Time { get; set; }
        public String Lyric { get; set; }
    }
}
