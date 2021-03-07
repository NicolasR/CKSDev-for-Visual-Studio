namespace CKS.Dev.VisualStudio.SharePoint.Explorer.Dialogs
{
    partial class DeveloperDashboardSettingsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpDisplayLevel = new System.Windows.Forms.GroupBox();
            this.radDisplayLevelOff = new System.Windows.Forms.RadioButton();
            this.radDisplayLevelOnDemand = new System.Windows.Forms.RadioButton();
            this.radDisplayLevelOn = new System.Windows.Forms.RadioButton();
            this.grpDisplayLevel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(12, 113);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(93, 113);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // grpDisplayLevel
            // 
            this.grpDisplayLevel.Controls.Add(this.radDisplayLevelOn);
            this.grpDisplayLevel.Controls.Add(this.radDisplayLevelOnDemand);
            this.grpDisplayLevel.Controls.Add(this.radDisplayLevelOff);
            this.grpDisplayLevel.Location = new System.Drawing.Point(12, 12);
            this.grpDisplayLevel.Name = "grpDisplayLevel";
            this.grpDisplayLevel.Size = new System.Drawing.Size(156, 95);
            this.grpDisplayLevel.TabIndex = 2;
            this.grpDisplayLevel.TabStop = false;
            this.grpDisplayLevel.Text = "Display Level";
            // 
            // radDisplayLevelOff
            // 
            this.radDisplayLevelOff.AutoSize = true;
            this.radDisplayLevelOff.Location = new System.Drawing.Point(7, 20);
            this.radDisplayLevelOff.Name = "radDisplayLevelOff";
            this.radDisplayLevelOff.Size = new System.Drawing.Size(39, 17);
            this.radDisplayLevelOff.TabIndex = 0;
            this.radDisplayLevelOff.TabStop = true;
            this.radDisplayLevelOff.Text = "Off";
            this.radDisplayLevelOff.UseVisualStyleBackColor = true;
            // 
            // radDisplayLevelOnDemand
            // 
            this.radDisplayLevelOnDemand.AutoSize = true;
            this.radDisplayLevelOnDemand.Location = new System.Drawing.Point(7, 44);
            this.radDisplayLevelOnDemand.Name = "radDisplayLevelOnDemand";
            this.radDisplayLevelOnDemand.Size = new System.Drawing.Size(82, 17);
            this.radDisplayLevelOnDemand.TabIndex = 1;
            this.radDisplayLevelOnDemand.TabStop = true;
            this.radDisplayLevelOnDemand.Text = "On Demand";
            this.radDisplayLevelOnDemand.UseVisualStyleBackColor = true;
            // 
            // radDisplayLevelOn
            // 
            this.radDisplayLevelOn.AutoSize = true;
            this.radDisplayLevelOn.Location = new System.Drawing.Point(7, 68);
            this.radDisplayLevelOn.Name = "radDisplayLevelOn";
            this.radDisplayLevelOn.Size = new System.Drawing.Size(39, 17);
            this.radDisplayLevelOn.TabIndex = 2;
            this.radDisplayLevelOn.TabStop = true;
            this.radDisplayLevelOn.Text = "On";
            this.radDisplayLevelOn.UseVisualStyleBackColor = true;
            // 
            // DeveloperDashboardSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(181, 142);
            this.Controls.Add(this.grpDisplayLevel);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DeveloperDashboardSettingsDialog";
            this.Text = "Developer Dashboard Settings";
            this.Load += new System.EventHandler(this.DeveloperDashboardSettingsDialog_Load);
            this.grpDisplayLevel.ResumeLayout(false);
            this.grpDisplayLevel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpDisplayLevel;
        private System.Windows.Forms.RadioButton radDisplayLevelOn;
        private System.Windows.Forms.RadioButton radDisplayLevelOnDemand;
        private System.Windows.Forms.RadioButton radDisplayLevelOff;
    }
}