using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using RainbowMage.OverlayPlugin;
using System.Diagnostics;

namespace RainbowMage.OverlayPlugin.EventSources
{
    public partial class EnmityEventSourceConfigPanel : UserControl
    {
        private EnmityEventSource overlay;
        private EnmityEventSourceConfig config;

        public EnmityEventSourceConfigPanel(EnmityEventSource overlay)
        {
            InitializeComponent();

            this.overlay = overlay;
            this.config = overlay.Config;

            SetupControlProperties();
            SetupConfigEventHandlers();
        }

        private void SetupControlProperties()
        {
            this.nudEnmityScanInterval.Value = this.config.ScanInterval;
        }

        private void SetupConfigEventHandlers()
        {
            this.config.ScanIntervalChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.nudEnmityScanInterval.Value = this.config.ScanInterval;
                });
            };
        }

        private void InvokeIfRequired(Action action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void nudEnmityScanInterval_ValueChanged(object sender, EventArgs e)
        {
            this.config.ScanInterval = (int)nudEnmityScanInterval.Value;
        }
    }
}
