namespace RainbowMage.OverlayPlugin
{
    partial class GeneralConfigTab
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbErrorReports = new System.Windows.Forms.CheckBox();
            this.cbUpdateCheck = new System.Windows.Forms.CheckBox();
            this.btnUpdateCheck = new System.Windows.Forms.Button();
            this.cbHideOverlaysWhenNotActive = new System.Windows.Forms.CheckBox();
            this.cbHideOverlaysDuringCutscene = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 666F));
            this.tableLayoutPanel1.Controls.Add(this.cbErrorReports, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbUpdateCheck, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnUpdateCheck, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.cbHideOverlaysWhenNotActive, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.cbHideOverlaysDuringCutscene, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(686, 397);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cbErrorReports
            // 
            this.cbErrorReports.AutoSize = true;
            this.cbErrorReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbErrorReports.Location = new System.Drawing.Point(23, 3);
            this.cbErrorReports.Name = "cbErrorReports";
            this.cbErrorReports.Size = new System.Drawing.Size(660, 19);
            this.cbErrorReports.TabIndex = 0;
            this.cbErrorReports.Text = "Automatically report errors";
            this.cbErrorReports.UseVisualStyleBackColor = true;
            // 
            // cbUpdateCheck
            // 
            this.cbUpdateCheck.AutoSize = true;
            this.cbUpdateCheck.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbUpdateCheck.Location = new System.Drawing.Point(23, 28);
            this.cbUpdateCheck.Name = "cbUpdateCheck";
            this.cbUpdateCheck.Size = new System.Drawing.Size(660, 19);
            this.cbUpdateCheck.TabIndex = 1;
            this.cbUpdateCheck.Text = "Automatically check for updates";
            this.cbUpdateCheck.UseVisualStyleBackColor = true;
            // 
            // btnUpdateCheck
            // 
            this.btnUpdateCheck.Location = new System.Drawing.Point(23, 123);
            this.btnUpdateCheck.Name = "btnUpdateCheck";
            this.btnUpdateCheck.Size = new System.Drawing.Size(163, 23);
            this.btnUpdateCheck.TabIndex = 2;
            this.btnUpdateCheck.Text = "Check For Updates";
            this.btnUpdateCheck.UseVisualStyleBackColor = true;
            this.btnUpdateCheck.Click += new System.EventHandler(this.BtnUpdateCheck_Click);
            // 
            // cbHideOverlaysWhenNotActive
            // 
            this.cbHideOverlaysWhenNotActive.AutoSize = true;
            this.cbHideOverlaysWhenNotActive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbHideOverlaysWhenNotActive.Location = new System.Drawing.Point(23, 53);
            this.cbHideOverlaysWhenNotActive.Name = "cbHideOverlaysWhenNotActive";
            this.cbHideOverlaysWhenNotActive.Size = new System.Drawing.Size(660, 19);
            this.cbHideOverlaysWhenNotActive.TabIndex = 3;
            this.cbHideOverlaysWhenNotActive.Text = "Automatically hide overlays when the game is in the background";
            this.cbHideOverlaysWhenNotActive.UseVisualStyleBackColor = true;
            // 
            // cbHideOverlaysDuringCutscene
            // 
            this.cbHideOverlaysDuringCutscene.AutoSize = true;
            this.cbHideOverlaysDuringCutscene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbHideOverlaysDuringCutscene.Location = new System.Drawing.Point(23, 78);
            this.cbHideOverlaysDuringCutscene.Name = "cbHideOverlaysDuringCutscene";
            this.cbHideOverlaysDuringCutscene.Size = new System.Drawing.Size(660, 19);
            this.cbHideOverlaysDuringCutscene.TabIndex = 4;
            this.cbHideOverlaysDuringCutscene.Text = "Automatically hide overlays during cutscenes";
            this.cbHideOverlaysDuringCutscene.UseVisualStyleBackColor = true;
            // 
            // GeneralConfigTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "GeneralConfigTab";
            this.Size = new System.Drawing.Size(686, 397);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox cbErrorReports;
        private System.Windows.Forms.CheckBox cbUpdateCheck;
        private System.Windows.Forms.Button btnUpdateCheck;
        private System.Windows.Forms.CheckBox cbHideOverlaysWhenNotActive;
        private System.Windows.Forms.CheckBox cbHideOverlaysDuringCutscene;
    }
}
