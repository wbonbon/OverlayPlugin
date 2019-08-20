namespace RainbowMage.OverlayPlugin
{
    partial class WSConfigPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WSConfigPanel));
            this.ipTxt = new System.Windows.Forms.TextBox();
            this.portTxt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.genSslBtn = new System.Windows.Forms.Button();
            this.sslBox = new System.Windows.Forms.CheckBox();
            this.logDisplay = new System.Windows.Forms.TextBox();
            this.startBtn = new System.Windows.Forms.Button();
            this.stopBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ipTxt
            // 
            this.ipTxt.Location = new System.Drawing.Point(124, 50);
            this.ipTxt.Name = "ipTxt";
            this.ipTxt.Size = new System.Drawing.Size(144, 20);
            this.ipTxt.TabIndex = 0;
            this.ipTxt.Leave += new System.EventHandler(this.ipTxt_Leave);
            // 
            // portTxt
            // 
            this.portTxt.Location = new System.Drawing.Point(124, 77);
            this.portTxt.Name = "portTxt";
            this.portTxt.Size = new System.Drawing.Size(144, 20);
            this.portTxt.TabIndex = 1;
            this.portTxt.Leave += new System.EventHandler(this.portTxt_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP Address:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port:";
            // 
            // genSslBtn
            // 
            this.genSslBtn.Location = new System.Drawing.Point(124, 110);
            this.genSslBtn.Name = "genSslBtn";
            this.genSslBtn.Size = new System.Drawing.Size(178, 23);
            this.genSslBtn.TabIndex = 4;
            this.genSslBtn.Text = "Generate SSL Certificate";
            this.genSslBtn.UseVisualStyleBackColor = true;
            this.genSslBtn.Click += new System.EventHandler(this.genSslBtn_Click);
            // 
            // sslBox
            // 
            this.sslBox.AutoSize = true;
            this.sslBox.Enabled = false;
            this.sslBox.Location = new System.Drawing.Point(21, 114);
            this.sslBox.Name = "sslBox";
            this.sslBox.Size = new System.Drawing.Size(82, 17);
            this.sslBox.TabIndex = 5;
            this.sslBox.Text = "Enable SSL";
            this.sslBox.UseVisualStyleBackColor = true;
            this.sslBox.CheckedChanged += new System.EventHandler(this.sslBox_CheckedChanged);
            // 
            // logDisplay
            // 
            this.logDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logDisplay.BackColor = System.Drawing.SystemColors.Control;
            this.logDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logDisplay.Location = new System.Drawing.Point(21, 164);
            this.logDisplay.Multiline = true;
            this.logDisplay.Name = "logDisplay";
            this.logDisplay.ReadOnly = true;
            this.logDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logDisplay.Size = new System.Drawing.Size(572, 182);
            this.logDisplay.TabIndex = 6;
            this.logDisplay.Text = resources.GetString("logDisplay.Text");
            // 
            // startBtn
            // 
            this.startBtn.Location = new System.Drawing.Point(384, 48);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(75, 23);
            this.startBtn.TabIndex = 7;
            this.startBtn.Text = "Start";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // stopBtn
            // 
            this.stopBtn.Location = new System.Drawing.Point(384, 75);
            this.stopBtn.Name = "stopBtn";
            this.stopBtn.Size = new System.Drawing.Size(75, 23);
            this.stopBtn.TabIndex = 8;
            this.stopBtn.Text = "Stop";
            this.stopBtn.UseVisualStyleBackColor = true;
            this.stopBtn.Click += new System.EventHandler(this.stopBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(341, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Status:";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(396, 115);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(53, 13);
            this.statusLabel.TabIndex = 10;
            this.statusLabel.Text = "Unknown";
            // 
            // WSConfigPanel
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.stopBtn);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.logDisplay);
            this.Controls.Add(this.sslBox);
            this.Controls.Add(this.genSslBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.portTxt);
            this.Controls.Add(this.ipTxt);
            this.Name = "WSConfigPanel";
            this.Size = new System.Drawing.Size(619, 408);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ipTxt;
        private System.Windows.Forms.TextBox portTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button genSslBtn;
        private System.Windows.Forms.CheckBox sslBox;
        private System.Windows.Forms.TextBox logDisplay;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Button stopBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label statusLabel;
    }
}
