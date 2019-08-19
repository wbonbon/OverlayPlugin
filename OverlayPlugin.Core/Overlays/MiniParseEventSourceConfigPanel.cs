using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RainbowMage.OverlayPlugin.Overlays
{
    partial class MiniParseEventSourceConfigPanel : UserControl
    {
        private MiniParseEventSourceConfig config;

        public MiniParseEventSourceConfigPanel(MiniParseEventSource source)
        {
            InitializeComponent();

            this.config = source.Config;

            SetupControlProperties();
            SetupConfigEventHandlers();
        }

        private void SetupControlProperties()
        {
            
        }

        private void SetupConfigEventHandlers()
        {
            
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

        private void buttonCopyActXiv_Click(object sender, EventArgs e)
        {
            /*var json = overlay.CreateJsonData();
            if (!string.IsNullOrWhiteSpace(json))
            {
                Clipboard.SetText(json);
            }*/
        }
    }
}
