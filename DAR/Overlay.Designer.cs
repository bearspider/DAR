namespace DAR
{
    partial class Overlay
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
            this.labelFont = new System.Windows.Forms.Label();
            this.comboBoxFont = new System.Windows.Forms.ComboBox();
            this.labelFontSize = new System.Windows.Forms.Label();
            this.comboBoxFontSize = new System.Windows.Forms.ComboBox();
            this.labelFadeDelay = new System.Windows.Forms.Label();
            this.comboBoxFadeDelay = new System.Windows.Forms.ComboBox();
            this.labelBackground = new System.Windows.Forms.Label();
            this.comboBoxBg = new System.Windows.Forms.ComboBox();
            this.labelFadedBg = new System.Windows.Forms.Label();
            this.comboBoxFadedBg = new System.Windows.Forms.ComboBox();
            this.labelOverlayName = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelFont
            // 
            this.labelFont.AutoSize = true;
            this.labelFont.Location = new System.Drawing.Point(5, 15);
            this.labelFont.Name = "labelFont";
            this.labelFont.Size = new System.Drawing.Size(31, 13);
            this.labelFont.TabIndex = 0;
            this.labelFont.Text = "Font:";
            // 
            // comboBoxFont
            // 
            this.comboBoxFont.FormattingEnabled = true;
            this.comboBoxFont.Location = new System.Drawing.Point(42, 12);
            this.comboBoxFont.Name = "comboBoxFont";
            this.comboBoxFont.Size = new System.Drawing.Size(121, 21);
            this.comboBoxFont.TabIndex = 1;
            this.comboBoxFont.SelectedIndexChanged += new System.EventHandler(this.ComboBoxFont_SelectedIndexChanged);
            // 
            // labelFontSize
            // 
            this.labelFontSize.AutoSize = true;
            this.labelFontSize.Location = new System.Drawing.Point(169, 15);
            this.labelFontSize.Name = "labelFontSize";
            this.labelFontSize.Size = new System.Drawing.Size(30, 13);
            this.labelFontSize.TabIndex = 2;
            this.labelFontSize.Text = "Size:";
            // 
            // comboBoxFontSize
            // 
            this.comboBoxFontSize.FormattingEnabled = true;
            this.comboBoxFontSize.Location = new System.Drawing.Point(206, 11);
            this.comboBoxFontSize.Name = "comboBoxFontSize";
            this.comboBoxFontSize.Size = new System.Drawing.Size(49, 21);
            this.comboBoxFontSize.TabIndex = 3;
            this.comboBoxFontSize.SelectedIndexChanged += new System.EventHandler(this.ComboBoxFontSize_SelectedIndexChanged);
            // 
            // labelFadeDelay
            // 
            this.labelFadeDelay.AutoSize = true;
            this.labelFadeDelay.Location = new System.Drawing.Point(261, 14);
            this.labelFadeDelay.Name = "labelFadeDelay";
            this.labelFadeDelay.Size = new System.Drawing.Size(64, 13);
            this.labelFadeDelay.TabIndex = 4;
            this.labelFadeDelay.Text = "Fade Delay:";
            // 
            // comboBoxFadeDelay
            // 
            this.comboBoxFadeDelay.FormattingEnabled = true;
            this.comboBoxFadeDelay.Location = new System.Drawing.Point(331, 11);
            this.comboBoxFadeDelay.Name = "comboBoxFadeDelay";
            this.comboBoxFadeDelay.Size = new System.Drawing.Size(121, 21);
            this.comboBoxFadeDelay.TabIndex = 5;
            // 
            // labelBackground
            // 
            this.labelBackground.AutoSize = true;
            this.labelBackground.Location = new System.Drawing.Point(458, 14);
            this.labelBackground.Name = "labelBackground";
            this.labelBackground.Size = new System.Drawing.Size(68, 13);
            this.labelBackground.TabIndex = 6;
            this.labelBackground.Text = "Background:";
            // 
            // comboBoxBg
            // 
            this.comboBoxBg.FormattingEnabled = true;
            this.comboBoxBg.Location = new System.Drawing.Point(532, 11);
            this.comboBoxBg.Name = "comboBoxBg";
            this.comboBoxBg.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBg.TabIndex = 7;
            // 
            // labelFadedBg
            // 
            this.labelFadedBg.AutoSize = true;
            this.labelFadedBg.Location = new System.Drawing.Point(659, 15);
            this.labelFadedBg.Name = "labelFadedBg";
            this.labelFadedBg.Size = new System.Drawing.Size(101, 13);
            this.labelFadedBg.TabIndex = 8;
            this.labelFadedBg.Text = "Faded Background:";
            // 
            // comboBoxFadedBg
            // 
            this.comboBoxFadedBg.FormattingEnabled = true;
            this.comboBoxFadedBg.Location = new System.Drawing.Point(766, 12);
            this.comboBoxFadedBg.Name = "comboBoxFadedBg";
            this.comboBoxFadedBg.Size = new System.Drawing.Size(121, 21);
            this.comboBoxFadedBg.TabIndex = 9;
            // 
            // labelOverlayName
            // 
            this.labelOverlayName.AutoSize = true;
            this.labelOverlayName.Location = new System.Drawing.Point(417, 207);
            this.labelOverlayName.Name = "labelOverlayName";
            this.labelOverlayName.Size = new System.Drawing.Size(35, 13);
            this.labelOverlayName.TabIndex = 10;
            this.labelOverlayName.Text = "label1";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(377, 38);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 11;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            // 
            // Overlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 450);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.labelOverlayName);
            this.Controls.Add(this.comboBoxFadedBg);
            this.Controls.Add(this.labelFadedBg);
            this.Controls.Add(this.comboBoxBg);
            this.Controls.Add(this.labelBackground);
            this.Controls.Add(this.comboBoxFadeDelay);
            this.Controls.Add(this.labelFadeDelay);
            this.Controls.Add(this.comboBoxFontSize);
            this.Controls.Add(this.labelFontSize);
            this.Controls.Add(this.comboBoxFont);
            this.Controls.Add(this.labelFont);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Overlay";
            this.Opacity = 0.2D;
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelFont;
        private System.Windows.Forms.ComboBox comboBoxFont;
        private System.Windows.Forms.Label labelFontSize;
        private System.Windows.Forms.ComboBox comboBoxFontSize;
        private System.Windows.Forms.Label labelFadeDelay;
        private System.Windows.Forms.ComboBox comboBoxFadeDelay;
        private System.Windows.Forms.Label labelBackground;
        private System.Windows.Forms.ComboBox comboBoxBg;
        private System.Windows.Forms.Label labelFadedBg;
        private System.Windows.Forms.ComboBox comboBoxFadedBg;
        private System.Windows.Forms.Label labelOverlayName;
        private System.Windows.Forms.Button buttonSave;
    }
}