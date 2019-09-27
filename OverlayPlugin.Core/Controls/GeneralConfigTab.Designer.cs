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
            this.tableLayoutPanel1.Controls.Add(this.btnUpdateCheck, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(686, 397);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cbErrorReports
            // 
            this.cbErrorReports.AutoSize = true;
            this.cbErrorReports.Location = new System.Drawing.Point(23, 3);
            this.cbErrorReports.Name = "cbErrorReports";
            this.cbErrorReports.Size = new System.Drawing.Size(153, 17);
            this.cbErrorReports.TabIndex = 0;
            this.cbErrorReports.Text = "Automatically Report Errors";
            this.cbErrorReports.UseVisualStyleBackColor = true;
            // 
            // cbUpdateCheck
            // 
            this.cbUpdateCheck.AutoSize = true;
            this.cbUpdateCheck.Location = new System.Drawing.Point(23, 28);
            this.cbUpdateCheck.Name = "cbUpdateCheck";
            this.cbUpdateCheck.Size = new System.Drawing.Size(183, 17);
            this.cbUpdateCheck.TabIndex = 1;
            this.cbUpdateCheck.Text = "Automatically Check For Updates";
            this.cbUpdateCheck.UseVisualStyleBackColor = true;
            // 
            // btnUpdateCheck
            // 
            this.btnUpdateCheck.Location = new System.Drawing.Point(23, 51);
            this.btnUpdateCheck.Name = "btnUpdateCheck";
            this.btnUpdateCheck.Size = new System.Drawing.Size(163, 23);
            this.btnUpdateCheck.TabIndex = 2;
            this.btnUpdateCheck.Text = "Check For Updates";
            this.btnUpdateCheck.UseVisualStyleBackColor = true;
            this.btnUpdateCheck.Click += new System.EventHandler(this.BtnUpdateCheck_Click);
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
    }
}
