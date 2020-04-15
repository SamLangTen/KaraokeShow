using MusicBeePlugin.Internationalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MusicBeePlugin.Config
{
    public partial class ConfigWindow : Form
    {
        public Font TextFont
        {
            get => (Font)button1.Tag;
            set
            {
                button1.Tag = value;
                textBox1.Text = $"{value.Name}, {value.Size.ToString()}pt";
            }
        }
        public Color OutlineBackColor { get => button5.BackColor; set => button5.BackColor = value; }
        public Color OutlineForeColor { get => button2.BackColor; set => button2.BackColor = value; }
        public int BlurRadial { get => (int)numericUpDown2.Value; set => numericUpDown2.Value = value; }
        public int OutlineWidth { get => (int)numericUpDown3.Value; set => numericUpDown3.Value = value; }
        public Color BackColor1 { get => button6.BackColor; set => button6.BackColor = value; }
        public Color BackColor2 { get => button7.BackColor; set => button7.BackColor = value; }
        public Color ForeColor1 { get => button3.BackColor; set => button3.BackColor = value; }
        public Color ForeColor2 { get => button4.BackColor; set => button4.BackColor = value; }
        public int Line { get => (int)numericUpDown1.Value; set => numericUpDown1.Value = value; }
        public bool EnabledSliding { get => checkBox1.Checked; set => checkBox1.Checked = value; }

        public ConfigWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var font = new FontDialog();
            font.Font = (Font)button1.Tag;
            if (font.ShowDialog() == DialogResult.OK)
            {
                button1.Tag = font.Font;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var color = new ColorDialog();
            color.Color = button.BackColor;
            if (color.ShowDialog() == DialogResult.OK)
            {
                button.BackColor = color.Color;
            }
        }

        private void ConfigWindow_Load(object sender, EventArgs e)
        {
            InternationalizationManager.EnableLanguage();
            InternationalizationManager.ApplyResourceToWinForm(this);
        }
    }
}
