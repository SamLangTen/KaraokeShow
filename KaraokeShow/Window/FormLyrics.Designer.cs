namespace MusicBeePlugin.Window
{
    partial class FormLyrics
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormLyrics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(514, 254);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLyrics";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormLyrics";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormLyrics_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormLyrics_MouseDown);
            this.MouseLeave += new System.EventHandler(this.FormLyrics_MouseLeave);
            this.MouseHover += new System.EventHandler(this.FormLyrics_MouseHover);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormLyrics_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormLyrics_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}