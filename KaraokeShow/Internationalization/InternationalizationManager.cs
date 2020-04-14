using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;

namespace MusicBeePlugin.Internationalization
{
    class InternationalizationManager
    {
        public static string GetCurrentMusicBeeLanguage(string mbMainField173Text)
        {
            //Initialize langDict that convert MusicBee setting to .Net Culture Text
            var langDict = new Dictionary<string, string>();
            langDict["Language"] = "en";
            langDict["语言"] = "zh-Hans";
            var cultureText = langDict.ContainsKey(mbMainField173Text) ? langDict[mbMainField173Text] : "en";
            return cultureText;
        }

        public static void EnableLanguage(string mbMainField173Text)
        {
            var cultureText = GetCurrentMusicBeeLanguage(mbMainField173Text);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureText);
        }

        public static void AppyResourceToWinForm(Control c)
        {
            var res = new EmbedResourceManager(c.GetType());
            foreach (Control item in c.Controls)
            {
                res.ApplyResources(item, item.Name);
            }
            c.ResumeLayout(false);
            c.PerformLayout();
            res.ApplyResources(c, c.Name);
        }
    }
}
