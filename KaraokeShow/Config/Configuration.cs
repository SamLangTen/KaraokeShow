using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MusicBeePlugin
{
    public class Configuration
    {

        public class SerializedConfig
        {
            public string FontName { get; set; }
            public float FontSize { get; set; }
            public string FontStyle { get; set; }
            public int OutlineBackColor { get; set; }
            public int OutlineForeColor { get; set; }
            public int BlurRadial { get; set; }
            public int BackColor1 { get; set; }
            public int BackColor2 { get; set; }
            public int ForeColor1 { get; set; }
            public int ForeColor2 { get; set; }
            public int Line { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public bool EnabledSliding { get; set; }
        }

        public static Font TextFont { get; set; }
        public static Color OutlineBackColor { get; set; }
        public static Color OutlineForeColor { get; set; }
        public static int BlurRadial { get; set; }
        public static Color BackColor1 { get; set; }
        public static Color BackColor2 { get; set; }
        public static Color ForeColor1 { get; set; }
        public static Color ForeColor2 { get; set; }
        public static int Line { get; set; }
        public static int X { get; set; }
        public static int Y { get; set; }
        public static int Width { get; set; }
        public static bool EnabledSliding { get; set; }

        public static void SaveConfig(string path)
        {
            var config = new SerializedConfig()
            {
                FontName = TextFont.Name,
                FontStyle = TextFont.Style.ToString(),
                FontSize = TextFont.Size,
                OutlineBackColor = OutlineBackColor.ToArgb(),
                OutlineForeColor = OutlineForeColor.ToArgb(),
                BackColor1 = BackColor1.ToArgb(),
                BackColor2 = BackColor2.ToArgb(),
                ForeColor1 = ForeColor1.ToArgb(),
                ForeColor2 = ForeColor2.ToArgb(),
                BlurRadial = BlurRadial,
                Line = Line,
                X = X,
                Y = Y,
                Width = Width,
                EnabledSliding = EnabledSliding
            };

            var xms = new XmlSerializer(typeof(SerializedConfig));
            var writer = new StringWriter();
            xms.Serialize(writer, config);
            File.WriteAllText(path, writer.ToString(), Encoding.UTF8);
        }

        public static void LoadConfig(string path)
        {
            if (!File.Exists(path))
            {
                TextFont = new Font("微软雅黑", 30);
                OutlineBackColor = Color.LightGreen;
                OutlineForeColor = Color.Magenta;
                BackColor1 = Color.Green;
                BackColor2 = Color.DarkGreen;
                ForeColor1 = Color.Red;
                ForeColor2 = Color.DarkRed;
                BlurRadial = 3;
                Line = 2;
                X = 100;
                Y = 100;
                Width = 300;
                EnabledSliding = true;
            }
            else
            {
                var xmlText = File.ReadAllText(path, Encoding.UTF8);
                var sr = new StringReader(xmlText);
                var xms = new XmlSerializer(typeof(SerializedConfig));
                var config = (SerializedConfig)xms.Deserialize(sr);

                TextFont = new Font(config.FontName, config.FontSize, (FontStyle)Enum.Parse(typeof(FontStyle), config.FontStyle));
                OutlineBackColor = Color.FromArgb(config.OutlineBackColor);
                OutlineForeColor = Color.FromArgb(config.OutlineForeColor);
                BackColor1 = Color.FromArgb(config.BackColor1);
                BackColor2 = Color.FromArgb(config.BackColor2);
                ForeColor1 = Color.FromArgb(config.ForeColor1);
                ForeColor2 = Color.FromArgb(config.ForeColor2);
                BlurRadial = config.BlurRadial;
                Line = config.Line;
                X = config.X;
                Y = config.Y;
                Width = config.Width;
                EnabledSliding = config.EnabledSliding;
            }
        }
    }
}
