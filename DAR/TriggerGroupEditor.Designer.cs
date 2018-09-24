namespace DAR
{
    partial class TriggerGroupEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriggerGroupEditor));
            this.groupBoxGS = new System.Windows.Forms.GroupBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxComments = new System.Windows.Forms.TextBox();
            this.labelComments = new System.Windows.Forms.Label();
            this.checkBoxEnable = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.groupBoxGS.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxGS
            // 
            this.groupBoxGS.Controls.Add(this.checkBoxEnable);
            this.groupBoxGS.Controls.Add(this.labelComments);
            this.groupBoxGS.Controls.Add(this.textBoxComments);
            this.groupBoxGS.Controls.Add(this.labelName);
            this.groupBoxGS.Controls.Add(this.textBoxName);
            this.groupBoxGS.Location = new System.Drawing.Point(12, 12);
            this.groupBoxGS.Name = "groupBoxGS";
            this.groupBoxGS.Size = new System.Drawing.Size(532, 192);
            this.groupBoxGS.TabIndex = 0;
            this.groupBoxGS.TabStop = false;
            this.groupBoxGS.Text = "General Settings";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(114, 19);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(386, 20);
            this.textBoxName.TabIndex = 0;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(6, 22);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(103, 13);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Trigger Group Name";
            // 
            // textBoxComments
            // 
            this.textBoxComments.Location = new System.Drawing.Point(114, 45);
            this.textBoxComments.Multiline = true;
            this.textBoxComments.Name = "textBoxComments";
            this.textBoxComments.Size = new System.Drawing.Size(386, 102);
            this.textBoxComments.TabIndex = 2;
            // 
            // labelComments
            // 
            this.labelComments.AutoSize = true;
            this.labelComments.Location = new System.Drawing.Point(52, 48);
            this.labelComments.Name = "labelComments";
            this.labelComments.Size = new System.Drawing.Size(56, 13);
            this.labelComments.TabIndex = 3;
            this.labelComments.Text = "Comments";
            // 
            // checkBoxEnable
            // 
            this.checkBoxEnable.AutoSize = true;
            this.checkBoxEnable.Checked = true;
            this.checkBoxEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEnable.Location = new System.Drawing.Point(114, 153);
            this.checkBoxEnable.Name = "checkBoxEnable";
            this.checkBoxEnable.Size = new System.Drawing.Size(183, 17);
            this.checkBoxEnable.TabIndex = 4;
            this.checkBoxEnable.Text = "Enable For Characters By Default";
            this.checkBoxEnable.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(475, 216);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(68, 28);
            this.button1.TabIndex = 1;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(401, 216);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(68, 28);
            this.buttonSave.TabIndex = 2;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // TriggerGroupEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 258);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBoxGS);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TriggerGroupEditor";
            this.Text = "Trigger Group Editor";
            this.groupBoxGS.ResumeLayout(false);
            this.groupBoxGS.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxGS;
        private System.Windows.Forms.CheckBox checkBoxEnable;
        private System.Windows.Forms.Label labelComments;
        private System.Windows.Forms.TextBox textBoxComments;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonSave;
    }
}