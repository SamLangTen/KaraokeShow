using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;
using System.Resources;

namespace MusicBeePlugin.Internationalization
{
    class InternationalizationManager
    {
        private static ResourceManager resMan = new EmbedResourceManager(typeof(Properties.Resources));
        public static string CultureText { get; set; } = "en";
        public static void SetCurrentLanguage(string mbMainField173Text)
        {
            //Initialize langDict that convert MusicBee setting to .Net Culture Text
            var langDict = new Dictionary<string, string>();
            langDict["Language"] = "en";
            langDict["语言"] = "zh-Hans";
            var cultureText = langDict.ContainsKey(mbMainField173Text) ? langDict[mbMainField173Text] : "en";
            CultureText = cultureText;
        }

        public static void EnableLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(CultureText);
        }

        public static string GetResourceString(string stringName)
        {
            return resMan.GetString(stringName, new CultureInfo(CultureText));
        }

        public static void ApplyResourceToWinForm(Control c)
        {
            var res = new EmbedResourceManager(c.GetType());
            ApplyResourceToControl(c, res);
        }

        private static void ApplyResourceToControl(Control c, EmbedResourceManager res)
        {
            foreach (Control item in c.Controls)
            {
                ApplyResourceToControl(item, res);
            }
            res.ApplyResources(c, c.Name);
            c.ResumeLayout(false);
            c.PerformLayout();
        }

    }
}
