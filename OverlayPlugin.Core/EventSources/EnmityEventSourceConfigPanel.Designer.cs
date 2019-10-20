namespace RainbowMage.OverlayPlugin.EventSources
{
    partial class EnmityEventSourceConfigPanel
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnmityEventSourceConfigPanel));
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.label_ScanInterval = new System.Windows.Forms.Label();
            this.nudEnmityScanInterval = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEnmityScanInterval)).BeginInit();
            this.SuspendLayout();
            //
            // tableLayoutPanel7
            //
            resources.ApplyResources(this.tableLayoutPanel7, "tableLayoutPanel7");
            this.tableLayoutPanel7.Controls.Add(this.label_ScanInterval, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.nudEnmityScanInterval, 1, 0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            //
            // label_ScanInterval
            //
            resources.ApplyResources(this.label_ScanInterval, "label_ScanInterval");
            this.label_ScanInterval.Name = "label_ScanInterval";
            //
            // nudEnmityScanInterval
            //
            resources.ApplyResources(this.nudEnmityScanInterval, "nudEnmityScanInterval");
            this.nudEnmityScanInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudEnmityScanInterval.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudEnmityScanInterval.Name = "nudEnmityScanInterval";
            this.nudEnmityScanInterval.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudEnmityScanInterval.ValueChanged += new System.EventHandler(this.nudEnmityScanInterval_ValueChanged);
            //
            // EnmityEventSourceConfigPanel
            //
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel7);
            this.Name = "EnmityEventSourceConfigPanel";
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEnmityScanInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label label_ScanInterval;
        private System.Windows.Forms.NumericUpDown nudEnmityScanInterval;
    }
}
