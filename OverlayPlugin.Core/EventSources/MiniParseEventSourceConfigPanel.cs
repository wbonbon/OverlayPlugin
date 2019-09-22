using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RainbowMage.OverlayPlugin.EventSources
{
    partial class MiniParseEventSourceConfigPanel : UserControl
    {
        private MiniParseEventSourceConfig config;

        static readonly List<KeyValuePair<string, string>> sortKeyDict = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("None", "null"),
            new KeyValuePair<string, string>("DPS", "encdps"),
            new KeyValuePair<string, string>("HPS", "enchps"),
        };

        public MiniParseEventSourceConfigPanel(MiniParseEventSource source)
        {
            InitializeComponent();

            this.config = source.Config;

            SetupControlProperties();
            SetupConfigEventHandlers();
        }

        private void SetupControlProperties()
        {
            this.textUpdateInterval.Text = "" + config.UpdateInterval;

            this.comboSortKey.DisplayMember = "Key";
            this.comboSortKey.ValueMember = "Value";
            this.comboSortKey.DataSource = sortKeyDict;
            this.comboSortKey.SelectedValue = config.SortKey ?? "null";
            this.comboSortKey.SelectedIndexChanged += comboSortKey_SelectedIndexChanged;

            this.checkSortDesc.Checked = config.SortDesc;
        }

        private void SetupConfigEventHandlers()
        {
            this.config.UpdateIntervalChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.textUpdateInterval.Text = "" + config.UpdateInterval;
                });
            };

            this.config.SortKeyChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.comboSortKey.SelectedValue = config.SortKey;
                });
            };

            this.config.SortDescChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkSortDesc.Checked = config.SortDesc;
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

        private void comboSortKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.config.SortKey = (string)this.comboSortKey.SelectedValue;
            if (this.config.SortKey == "null") this.config.SortKey = null;
        }

        private void TextUpdateInterval_Leave(object sender, EventArgs e)
        {
            if (int.TryParse(this.textUpdateInterval.Text, out int value))
            {
                this.config.UpdateInterval = value;
            } else
            {
                this.textUpdateInterval.Text = "" + this.config.UpdateInterval;
            }
        }

        private void CheckSortDesc_CheckedChanged(object sender, EventArgs e)
        {
            this.config.SortDesc = this.checkSortDesc.Checked;
        }
    }
}
