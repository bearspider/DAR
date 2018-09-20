namespace DAR
{
    partial class CharacterEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterEditor));
            this.groupBoxCESettings = new System.Windows.Forms.GroupBox();
            this.buttonLoadFile = new System.Windows.Forms.Button();
            this.textBoxCECharacter = new System.Windows.Forms.TextBox();
            this.textBoxCEProfile = new System.Windows.Forms.TextBox();
            this.textBoxCELog = new System.Windows.Forms.TextBox();
            this.checkBoxMonitor = new System.Windows.Forms.CheckBox();
            this.labelCharacter = new System.Windows.Forms.Label();
            this.labelProfile = new System.Windows.Forms.Label();
            this.labelLog = new System.Windows.Forms.Label();
            this.groupBoxCEColors = new System.Windows.Forms.GroupBox();
            this.comboBoxTimerBar = new System.Windows.Forms.ComboBox();
            this.labelTextColor = new System.Windows.Forms.Label();
            this.comboBoxTimerFont = new System.Windows.Forms.ComboBox();
            this.comboBoxTextFont = new System.Windows.Forms.ComboBox();
            this.labelTimerBarColor = new System.Windows.Forms.Label();
            this.labelTimerFontColor = new System.Windows.Forms.Label();
            this.groupBoxCEAudio = new System.Windows.Forms.GroupBox();
            this.labelRateValue = new System.Windows.Forms.Label();
            this.labelVolumeValue = new System.Windows.Forms.Label();
            this.buttonSamplePlay = new System.Windows.Forms.Button();
            this.buttonNamePlay = new System.Windows.Forms.Button();
            this.textBoxSample = new System.Windows.Forms.TextBox();
            this.textBoxPhonetic = new System.Windows.Forms.TextBox();
            this.comboBoxVoice = new System.Windows.Forms.ComboBox();
            this.trackBarRate = new System.Windows.Forms.TrackBar();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.labelSample = new System.Windows.Forms.Label();
            this.labelPhonetic = new System.Windows.Forms.Label();
            this.labelRate = new System.Windows.Forms.Label();
            this.labelVoice = new System.Windows.Forms.Label();
            this.labelVolume = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.groupBoxCESettings.SuspendLayout();
            this.groupBoxCEColors.SuspendLayout();
            this.groupBoxCEAudio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxCESettings
            // 
            this.groupBoxCESettings.Controls.Add(this.buttonLoadFile);
            this.groupBoxCESettings.Controls.Add(this.textBoxCECharacter);
            this.groupBoxCESettings.Controls.Add(this.textBoxCEProfile);
            this.groupBoxCESettings.Controls.Add(this.textBoxCELog);
            this.groupBoxCESettings.Controls.Add(this.checkBoxMonitor);
            this.groupBoxCESettings.Controls.Add(this.labelCharacter);
            this.groupBoxCESettings.Controls.Add(this.labelProfile);
            this.groupBoxCESettings.Controls.Add(this.labelLog);
            this.groupBoxCESettings.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxCESettings.Location = new System.Drawing.Point(13, 13);
            this.groupBoxCESettings.Name = "groupBoxCESettings";
            this.groupBoxCESettings.Size = new System.Drawing.Size(479, 116);
            this.groupBoxCESettings.TabIndex = 0;
            this.groupBoxCESettings.TabStop = false;
            this.groupBoxCESettings.Text = "General Settings";
            // 
            // buttonLoadFile
            // 
            this.buttonLoadFile.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLoadFile.Location = new System.Drawing.Point(406, 13);
            this.buttonLoadFile.Name = "buttonLoadFile";
            this.buttonLoadFile.Size = new System.Drawing.Size(40, 20);
            this.buttonLoadFile.TabIndex = 7;
            this.buttonLoadFile.Text = ". . .";
            this.buttonLoadFile.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonLoadFile.UseVisualStyleBackColor = true;
            this.buttonLoadFile.Click += new System.EventHandler(this.ButtonLoadFile_Click);
            // 
            // textBoxCECharacter
            // 
            this.textBoxCECharacter.Location = new System.Drawing.Point(100, 65);
            this.textBoxCECharacter.Name = "textBoxCECharacter";
            this.textBoxCECharacter.Size = new System.Drawing.Size(220, 22);
            this.textBoxCECharacter.TabIndex = 6;
            // 
            // textBoxCEProfile
            // 
            this.textBoxCEProfile.Location = new System.Drawing.Point(100, 39);
            this.textBoxCEProfile.Name = "textBoxCEProfile";
            this.textBoxCEProfile.Size = new System.Drawing.Size(373, 22);
            this.textBoxCEProfile.TabIndex = 5;
            // 
            // textBoxCELog
            // 
            this.textBoxCELog.Location = new System.Drawing.Point(100, 13);
            this.textBoxCELog.Name = "textBoxCELog";
            this.textBoxCELog.Size = new System.Drawing.Size(300, 22);
            this.textBoxCELog.TabIndex = 4;
            // 
            // checkBoxMonitor
            // 
            this.checkBoxMonitor.AutoSize = true;
            this.checkBoxMonitor.Checked = true;
            this.checkBoxMonitor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMonitor.Location = new System.Drawing.Point(6, 93);
            this.checkBoxMonitor.Name = "checkBoxMonitor";
            this.checkBoxMonitor.Size = new System.Drawing.Size(126, 17);
            this.checkBoxMonitor.TabIndex = 3;
            this.checkBoxMonitor.Text = "Monitor on Startup";
            this.checkBoxMonitor.UseVisualStyleBackColor = true;
            // 
            // labelCharacter
            // 
            this.labelCharacter.AutoSize = true;
            this.labelCharacter.Location = new System.Drawing.Point(3, 68);
            this.labelCharacter.Name = "labelCharacter";
            this.labelCharacter.Size = new System.Drawing.Size(88, 13);
            this.labelCharacter.TabIndex = 2;
            this.labelCharacter.Text = "Character Name";
            // 
            // labelProfile
            // 
            this.labelProfile.AutoSize = true;
            this.labelProfile.Location = new System.Drawing.Point(3, 42);
            this.labelProfile.Name = "labelProfile";
            this.labelProfile.Size = new System.Drawing.Size(72, 13);
            this.labelProfile.TabIndex = 1;
            this.labelProfile.Text = "Profile Name";
            // 
            // labelLog
            // 
            this.labelLog.AutoSize = true;
            this.labelLog.Location = new System.Drawing.Point(3, 16);
            this.labelLog.Name = "labelLog";
            this.labelLog.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelLog.Size = new System.Drawing.Size(47, 13);
            this.labelLog.TabIndex = 0;
            this.labelLog.Text = "Log File";
            // 
            // groupBoxCEColors
            // 
            this.groupBoxCEColors.Controls.Add(this.comboBoxTimerBar);
            this.groupBoxCEColors.Controls.Add(this.labelTextColor);
            this.groupBoxCEColors.Controls.Add(this.comboBoxTimerFont);
            this.groupBoxCEColors.Controls.Add(this.comboBoxTextFont);
            this.groupBoxCEColors.Controls.Add(this.labelTimerBarColor);
            this.groupBoxCEColors.Controls.Add(this.labelTimerFontColor);
            this.groupBoxCEColors.Location = new System.Drawing.Point(13, 135);
            this.groupBoxCEColors.Name = "groupBoxCEColors";
            this.groupBoxCEColors.Size = new System.Drawing.Size(479, 116);
            this.groupBoxCEColors.TabIndex = 1;
            this.groupBoxCEColors.TabStop = false;
            this.groupBoxCEColors.Text = "Colors";
            // 
            // comboBoxTimerBar
            // 
            this.comboBoxTimerBar.FormattingEnabled = true;
            this.comboBoxTimerBar.Location = new System.Drawing.Point(122, 75);
            this.comboBoxTimerBar.Name = "comboBoxTimerBar";
            this.comboBoxTimerBar.Size = new System.Drawing.Size(270, 21);
            this.comboBoxTimerBar.TabIndex = 5;
            this.comboBoxTimerBar.SelectedIndexChanged += new System.EventHandler(this.ComboBoxTimerBar_SelectedIndexChanged);
            // 
            // labelTextColor
            // 
            this.labelTextColor.AutoSize = true;
            this.labelTextColor.Location = new System.Drawing.Point(3, 24);
            this.labelTextColor.Name = "labelTextColor";
            this.labelTextColor.Size = new System.Drawing.Size(84, 13);
            this.labelTextColor.TabIndex = 0;
            this.labelTextColor.Text = "Text Font Color";
            // 
            // comboBoxTimerFont
            // 
            this.comboBoxTimerFont.FormattingEnabled = true;
            this.comboBoxTimerFont.Location = new System.Drawing.Point(122, 48);
            this.comboBoxTimerFont.Name = "comboBoxTimerFont";
            this.comboBoxTimerFont.Size = new System.Drawing.Size(270, 21);
            this.comboBoxTimerFont.TabIndex = 4;
            this.comboBoxTimerFont.SelectedIndexChanged += new System.EventHandler(this.ComboBoxTimerFont_SelectedIndexChanged);
            // 
            // comboBoxTextFont
            // 
            this.comboBoxTextFont.FormattingEnabled = true;
            this.comboBoxTextFont.Location = new System.Drawing.Point(122, 21);
            this.comboBoxTextFont.Name = "comboBoxTextFont";
            this.comboBoxTextFont.Size = new System.Drawing.Size(270, 21);
            this.comboBoxTextFont.TabIndex = 3;
            this.comboBoxTextFont.SelectedIndexChanged += new System.EventHandler(this.ComboBoxTextFont_SelectedIndexChanged);
            // 
            // labelTimerBarColor
            // 
            this.labelTimerBarColor.AutoSize = true;
            this.labelTimerBarColor.Location = new System.Drawing.Point(3, 78);
            this.labelTimerBarColor.Name = "labelTimerBarColor";
            this.labelTimerBarColor.Size = new System.Drawing.Size(85, 13);
            this.labelTimerBarColor.TabIndex = 2;
            this.labelTimerBarColor.Text = "Timer Bar Color";
            // 
            // labelTimerFontColor
            // 
            this.labelTimerFontColor.AutoSize = true;
            this.labelTimerFontColor.Location = new System.Drawing.Point(3, 51);
            this.labelTimerFontColor.Name = "labelTimerFontColor";
            this.labelTimerFontColor.Size = new System.Drawing.Size(92, 13);
            this.labelTimerFontColor.TabIndex = 1;
            this.labelTimerFontColor.Text = "Timer Font Color";
            // 
            // groupBoxCEAudio
            // 
            this.groupBoxCEAudio.Controls.Add(this.labelRateValue);
            this.groupBoxCEAudio.Controls.Add(this.labelVolumeValue);
            this.groupBoxCEAudio.Controls.Add(this.buttonSamplePlay);
            this.groupBoxCEAudio.Controls.Add(this.buttonNamePlay);
            this.groupBoxCEAudio.Controls.Add(this.textBoxSample);
            this.groupBoxCEAudio.Controls.Add(this.textBoxPhonetic);
            this.groupBoxCEAudio.Controls.Add(this.comboBoxVoice);
            this.groupBoxCEAudio.Controls.Add(this.trackBarRate);
            this.groupBoxCEAudio.Controls.Add(this.trackBarVolume);
            this.groupBoxCEAudio.Controls.Add(this.labelSample);
            this.groupBoxCEAudio.Controls.Add(this.labelPhonetic);
            this.groupBoxCEAudio.Controls.Add(this.labelRate);
            this.groupBoxCEAudio.Controls.Add(this.labelVoice);
            this.groupBoxCEAudio.Controls.Add(this.labelVolume);
            this.groupBoxCEAudio.Location = new System.Drawing.Point(13, 257);
            this.groupBoxCEAudio.Name = "groupBoxCEAudio";
            this.groupBoxCEAudio.Size = new System.Drawing.Size(479, 215);
            this.groupBoxCEAudio.TabIndex = 2;
            this.groupBoxCEAudio.TabStop = false;
            this.groupBoxCEAudio.Text = "Audio Settings";
            // 
            // labelRateValue
            // 
            this.labelRateValue.AutoSize = true;
            this.labelRateValue.Location = new System.Drawing.Point(327, 99);
            this.labelRateValue.Name = "labelRateValue";
            this.labelRateValue.Size = new System.Drawing.Size(0, 13);
            this.labelRateValue.TabIndex = 13;
            // 
            // labelVolumeValue
            // 
            this.labelVolumeValue.AutoSize = true;
            this.labelVolumeValue.Location = new System.Drawing.Point(327, 22);
            this.labelVolumeValue.Name = "labelVolumeValue";
            this.labelVolumeValue.Size = new System.Drawing.Size(0, 13);
            this.labelVolumeValue.TabIndex = 12;
            // 
            // buttonSamplePlay
            // 
            this.buttonSamplePlay.Location = new System.Drawing.Point(398, 177);
            this.buttonSamplePlay.Name = "buttonSamplePlay";
            this.buttonSamplePlay.Size = new System.Drawing.Size(75, 23);
            this.buttonSamplePlay.TabIndex = 11;
            this.buttonSamplePlay.Text = "Play";
            this.buttonSamplePlay.UseVisualStyleBackColor = true;
            this.buttonSamplePlay.Click += new System.EventHandler(this.ButtonSamplePlay_Click);
            // 
            // buttonNamePlay
            // 
            this.buttonNamePlay.Location = new System.Drawing.Point(317, 149);
            this.buttonNamePlay.Name = "buttonNamePlay";
            this.buttonNamePlay.Size = new System.Drawing.Size(75, 23);
            this.buttonNamePlay.TabIndex = 10;
            this.buttonNamePlay.Text = "Play";
            this.buttonNamePlay.UseVisualStyleBackColor = true;
            this.buttonNamePlay.Click += new System.EventHandler(this.ButtonNamePlay_Click);
            // 
            // textBoxSample
            // 
            this.textBoxSample.Location = new System.Drawing.Point(100, 178);
            this.textBoxSample.Name = "textBoxSample";
            this.textBoxSample.Size = new System.Drawing.Size(292, 22);
            this.textBoxSample.TabIndex = 9;
            // 
            // textBoxPhonetic
            // 
            this.textBoxPhonetic.Location = new System.Drawing.Point(100, 150);
            this.textBoxPhonetic.Name = "textBoxPhonetic";
            this.textBoxPhonetic.Size = new System.Drawing.Size(211, 22);
            this.textBoxPhonetic.TabIndex = 8;
            // 
            // comboBoxVoice
            // 
            this.comboBoxVoice.FormattingEnabled = true;
            this.comboBoxVoice.Location = new System.Drawing.Point(100, 72);
            this.comboBoxVoice.Name = "comboBoxVoice";
            this.comboBoxVoice.Size = new System.Drawing.Size(220, 21);
            this.comboBoxVoice.TabIndex = 7;
            this.comboBoxVoice.SelectedIndexChanged += new System.EventHandler(this.ComboBoxVoice_SelectedIndexChanged);
            // 
            // trackBarRate
            // 
            this.trackBarRate.Location = new System.Drawing.Point(100, 99);
            this.trackBarRate.Minimum = -10;
            this.trackBarRate.Name = "trackBarRate";
            this.trackBarRate.Size = new System.Drawing.Size(220, 45);
            this.trackBarRate.TabIndex = 6;
            this.trackBarRate.Scroll += new System.EventHandler(this.TrackBarRate_Scroll);
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.Location = new System.Drawing.Point(100, 21);
            this.trackBarVolume.Maximum = 100;
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(220, 45);
            this.trackBarVolume.TabIndex = 5;
            this.trackBarVolume.Value = 90;
            this.trackBarVolume.Scroll += new System.EventHandler(this.TrackBarVolume_Scroll);
            // 
            // labelSample
            // 
            this.labelSample.AutoSize = true;
            this.labelSample.Location = new System.Drawing.Point(3, 181);
            this.labelSample.Name = "labelSample";
            this.labelSample.Size = new System.Drawing.Size(44, 13);
            this.labelSample.TabIndex = 4;
            this.labelSample.Text = "Sample";
            // 
            // labelPhonetic
            // 
            this.labelPhonetic.AutoSize = true;
            this.labelPhonetic.Location = new System.Drawing.Point(3, 153);
            this.labelPhonetic.Name = "labelPhonetic";
            this.labelPhonetic.Size = new System.Drawing.Size(84, 13);
            this.labelPhonetic.TabIndex = 3;
            this.labelPhonetic.Text = "Phonetic Name";
            // 
            // labelRate
            // 
            this.labelRate.AutoSize = true;
            this.labelRate.Location = new System.Drawing.Point(3, 99);
            this.labelRate.Name = "labelRate";
            this.labelRate.Size = new System.Drawing.Size(69, 13);
            this.labelRate.TabIndex = 2;
            this.labelRate.Text = "Voice Speed";
            // 
            // labelVoice
            // 
            this.labelVoice.AutoSize = true;
            this.labelVoice.Location = new System.Drawing.Point(3, 75);
            this.labelVoice.Name = "labelVoice";
            this.labelVoice.Size = new System.Drawing.Size(34, 13);
            this.labelVoice.TabIndex = 1;
            this.labelVoice.Text = "Voice";
            // 
            // labelVolume
            // 
            this.labelVolume.AutoSize = true;
            this.labelVolume.Location = new System.Drawing.Point(3, 21);
            this.labelVolume.Name = "labelVolume";
            this.labelVolume.Size = new System.Drawing.Size(45, 13);
            this.labelVolume.TabIndex = 0;
            this.labelVolume.Text = "Volume";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(421, 486);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(340, 486);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // CharacterEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 521);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBoxCEAudio);
            this.Controls.Add(this.groupBoxCEColors);
            this.Controls.Add(this.groupBoxCESettings);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "CharacterEditor";
            this.ShowInTaskbar = false;
            this.Text = "Character Editor";
            this.Load += new System.EventHandler(this.CharacterEditor_Load);
            this.groupBoxCESettings.ResumeLayout(false);
            this.groupBoxCESettings.PerformLayout();
            this.groupBoxCEColors.ResumeLayout(false);
            this.groupBoxCEColors.PerformLayout();
            this.groupBoxCEAudio.ResumeLayout(false);
            this.groupBoxCEAudio.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxCESettings;
        private System.Windows.Forms.TextBox textBoxCECharacter;
        private System.Windows.Forms.TextBox textBoxCEProfile;
        private System.Windows.Forms.TextBox textBoxCELog;
        private System.Windows.Forms.CheckBox checkBoxMonitor;
        private System.Windows.Forms.Label labelCharacter;
        private System.Windows.Forms.Label labelProfile;
        private System.Windows.Forms.Label labelLog;
        private System.Windows.Forms.GroupBox groupBoxCEColors;
        private System.Windows.Forms.GroupBox groupBoxCEAudio;
        private System.Windows.Forms.Button buttonLoadFile;
        private System.Windows.Forms.Label labelTimerBarColor;
        private System.Windows.Forms.Label labelTimerFontColor;
        private System.Windows.Forms.Label labelTextColor;
        private System.Windows.Forms.ComboBox comboBoxTimerBar;
        private System.Windows.Forms.ComboBox comboBoxTimerFont;
        private System.Windows.Forms.ComboBox comboBoxTextFont;
        private System.Windows.Forms.Label labelSample;
        private System.Windows.Forms.Label labelPhonetic;
        private System.Windows.Forms.Label labelRate;
        private System.Windows.Forms.Label labelVoice;
        private System.Windows.Forms.Label labelVolume;
        private System.Windows.Forms.TextBox textBoxSample;
        private System.Windows.Forms.TextBox textBoxPhonetic;
        private System.Windows.Forms.ComboBox comboBoxVoice;
        private System.Windows.Forms.TrackBar trackBarRate;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.Label labelRateValue;
        private System.Windows.Forms.Label labelVolumeValue;
        private System.Windows.Forms.Button buttonSamplePlay;
        private System.Windows.Forms.Button buttonNamePlay;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
    }
}