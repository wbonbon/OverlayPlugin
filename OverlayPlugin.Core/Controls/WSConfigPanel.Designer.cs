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
            resources.ApplyResources(this.ipTxt, "ipTxt");
            this.ipTxt.Name = "ipTxt";
            this.ipTxt.Leave += new System.EventHandler(this.ipTxt_Leave);
            // 
            // portTxt
            // 
            resources.ApplyResources(this.portTxt, "portTxt");
            this.portTxt.Name = "portTxt";
            this.portTxt.Leave += new System.EventHandler(this.portTxt_Leave);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // genSslBtn
            // 
            resources.ApplyResources(this.genSslBtn, "genSslBtn");
            this.genSslBtn.Name = "genSslBtn";
            this.genSslBtn.UseVisualStyleBackColor = true;
            this.genSslBtn.Click += new System.EventHandler(this.genSslBtn_Click);
            // 
            // sslBox
            // 
            resources.ApplyResources(this.sslBox, "sslBox");
            this.sslBox.Name = "sslBox";
            this.sslBox.UseVisualStyleBackColor = true;
            this.sslBox.CheckedChanged += new System.EventHandler(this.sslBox_CheckedChanged);
            // 
            // logDisplay
            // 
            resources.ApplyResources(this.logDisplay, "logDisplay");
            this.logDisplay.BackColor = System.Drawing.SystemColors.Control;
            this.logDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logDisplay.Name = "logDisplay";
            this.logDisplay.ReadOnly = true;
            // 
            // startBtn
            // 
            resources.ApplyResources(this.startBtn, "startBtn");
            this.startBtn.Name = "startBtn";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // stopBtn
            // 
            resources.ApplyResources(this.stopBtn, "stopBtn");
            this.stopBtn.Name = "stopBtn";
            this.stopBtn.UseVisualStyleBackColor = true;
            this.stopBtn.Click += new System.EventHandler(this.stopBtn_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // statusLabel
            // 
            resources.ApplyResources(this.statusLabel, "statusLabel");
            this.statusLabel.Name = "statusLabel";
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
            resources.ApplyResources(this, "$this");
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
