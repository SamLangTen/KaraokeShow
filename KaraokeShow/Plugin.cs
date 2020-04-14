using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using MusicBeePlugin.Config;
using MusicBeePlugin.Window;
using MusicBeePlugin.Parser;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        private MusicBeeApiInterface mbApiInterface;
        private PluginInfo about = new PluginInfo();
        private ConfigWindow config;
        private DestopLyrics destopLyrics;
        private System.Timers.Timer timer;

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            mbApiInterface = new MusicBeeApiInterface();
            mbApiInterface.Initialise(apiInterfacePtr);
            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = "KaraokeShow";
            about.Description = "A plugin to display synchronised lyrics on desktop";
            about.Author = "Samersions";
            about.TargetApplication = "";   //  the name of a Plugin Storage device or panel header for a dockable panel
            about.Type = PluginType.General;
            about.VersionMajor = 1;  // your plugin version
            about.VersionMinor = 0;
            about.Revision = 1;
            about.MinInterfaceVersion = MinInterfaceVersion;
            about.MinApiRevision = 23;
            about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
            about.ConfigurationPanelHeight = 0;   // height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function
            return about;
        }

        public bool Configure(IntPtr panelHandle)
        {
            // save any persistent settings in a sub-folder of this path
            string dataPath = mbApiInterface.Setting_GetPersistentStoragePath();
            // panelHandle will only be set if you set about.ConfigurationPanelHeight to a non-zero value
            // keep in mind the panel width is scaled according to the font the user has selected
            // if about.ConfigurationPanelHeight is set to 0, you can display your own popup window

            //if (panelHandle == IntPtr.Zero) return true;
            config = new ConfigWindow();
            config.Font = mbApiInterface.Setting_GetDefaultFont();
            config.TextFont = Configuration.TextFont;
            config.Line = Configuration.Line;
            config.BlurRadial = Configuration.BlurRadial;
            config.BackColor1 = Configuration.BackColor1;
            config.BackColor2 = Configuration.BackColor2;
            config.OutlineBackColor = Configuration.OutlineBackColor;
            config.ForeColor1 = Configuration.ForeColor1;
            config.ForeColor2 = Configuration.ForeColor2;
            config.OutlineForeColor = Configuration.OutlineForeColor;
            config.EnabledSliding = Configuration.EnabledSliding;
            if (config.ShowDialog() == DialogResult.Cancel)
            {
                config.TextFont = Configuration.TextFont;
                config.Line = Configuration.Line;
                config.BlurRadial = Configuration.BlurRadial;
                config.BackColor1 = Configuration.BackColor1;
                config.BackColor2 = Configuration.BackColor2;
                config.OutlineBackColor = Configuration.OutlineBackColor;
                config.ForeColor1 = Configuration.ForeColor1;
                config.ForeColor2 = Configuration.ForeColor2;
                config.OutlineForeColor = Configuration.OutlineForeColor;
                config.EnabledSliding = Configuration.EnabledSliding;
            }
            return true;
        }

        // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        // its up to you to figure out whether anything has changed and needs updating
        public void SaveSettings()
        {
            string dataPath = Path.Combine(mbApiInterface.Setting_GetPersistentStoragePath(), "KaraokeShow2.xml");
            if (config != null)
            {
                Configuration.TextFont = config.TextFont;
                Configuration.Line = config.Line;
                Configuration.BlurRadial = config.BlurRadial;
                Configuration.BackColor1 = config.BackColor1;
                Configuration.BackColor2 = config.BackColor2;
                Configuration.OutlineBackColor = config.OutlineBackColor;
                Configuration.ForeColor1 = config.ForeColor1;
                Configuration.ForeColor2 = config.ForeColor2;
                Configuration.OutlineForeColor = config.OutlineForeColor;
                Configuration.EnabledSliding = config.EnabledSliding;
            }
            Configuration.SaveConfig(dataPath);
        }

        // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
        public void Close(PluginCloseReason reason)
        {
        }

        // uninstall this plugin - clean up any persisted files
        public void Uninstall()
        {
            string dataPath = Path.Combine(mbApiInterface.Setting_GetPersistentStoragePath(), "KaraokeShow2.xml");
            if (File.Exists(dataPath))
                File.Delete(dataPath);
        }

        // receive event notifications from MusicBee
        // you need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, and not just the startup event
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            // perform some action depending on the notification type
            switch (type)
            {
                case NotificationType.PluginStartup:
                    string dataPath = Path.Combine(mbApiInterface.Setting_GetPersistentStoragePath(), "KaraokeShow2.xml");
                    Configuration.LoadConfig(dataPath);
                    break;
                case NotificationType.NowPlayingLyricsReady:

                    break;
                case NotificationType.TrackChanged:
                    var lrc = new LRCFile(mbApiInterface.NowPlaying_GetLyrics(), true);
                    destopLyrics = new DestopLyrics(lrc.ToSynchronousLyrics(mbApiInterface.NowPlaying_GetDuration()), (Form)Control.FromHandle(mbApiInterface.MB_GetWindowHandle()));
                    timer = new System.Timers.Timer()
                    {
                        Interval = 50,
                        Enabled = true
                    };
                    timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_tick);
                    timer.Start();
                    destopLyrics.ShowWindow();
                    break;
                default:
                    break;
            }
        }

        private void timer_tick(object sender, EventArgs e)
        {
            destopLyrics?.Update(mbApiInterface.Player_GetPosition());
        }

        // return an array of lyric or artwork provider names this plugin supports
        // the providers will be iterated through one by one and passed to the RetrieveLyrics/ RetrieveArtwork function in order set by the user in the MusicBee Tags(2) preferences screen until a match is found
        //public string[] GetProviders()
        //{
        //}

        // return lyrics for the requested artist/title from the requested provider
        // only required if PluginType = LyricsRetrieval
        // return null if no lyrics are found
        //public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album, bool synchronisedPreferred, string provider)
        //{
        //}

        // return Base64 string representation of the artwork binary data from the requested provider
        // only required if PluginType = ArtworkRetrieval
        // return null if no artwork is found
        //public string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album, string provider)
        //{
        //    //Return Convert.ToBase64String(artworkBinaryData)
        //    return null;
        //}

        //  presence of this function indicates to MusicBee that this plugin has a dockable panel. MusicBee will create the control and pass it as the panel parameter
        //  you can add your own controls to the panel if needed
        //  you can control the scrollable area of the panel using the mbApiInterface.MB_SetPanelScrollableArea function
        //  to set a MusicBee header for the panel, set about.TargetApplication in the Initialise function above to the panel header text
        //public int OnDockablePanelCreated(Control panel)
        //{
        //  //    return the height of the panel and perform any initialisation here
        //  //    MusicBee will call panel.Dispose() when the user removes this panel from the layout configuration
        //  //    < 0 indicates to MusicBee this control is resizable and should be sized to fill the panel it is docked to in MusicBee
        //  //    = 0 indicates to MusicBee this control resizeable
        //  //    > 0 indicates to MusicBee the fixed height for the control.Note it is recommended you scale the height for high DPI screens(create a graphics object and get the DpiY value)
        //    float dpiScaling = 0;
        //    using (Graphics g = panel.CreateGraphics())
        //    {
        //        dpiScaling = g.DpiY / 96f;
        //    }
        //    panel.Paint += panel_Paint;
        //    return Convert.ToInt32(100 * dpiScaling);
        //}

        // presence of this function indicates to MusicBee that the dockable panel created above will show menu items when the panel header is clicked
        // return the list of ToolStripMenuItems that will be displayed
        //public List<ToolStripItem> GetHeaderMenuItems()
        //{
        //    List<ToolStripItem> list = new List<ToolStripItem>();
        //    list.Add(new ToolStripMenuItem("A menu item"));
        //    return list;
        //}

        //private void panel_Paint(object sender, PaintEventArgs e)
        //{
        //    e.Graphics.Clear(Color.Red);
        //    TextRenderer.DrawText(e.Graphics, "hello", SystemFonts.CaptionFont, new Point(10, 10), Color.Blue);
        //}

    }
}