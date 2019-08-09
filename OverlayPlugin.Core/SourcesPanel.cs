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

namespace RainbowMage.OverlayPlugin
{
    public partial class SourcesPanel : UserControl
    {
        PluginMain pluginMain;
        PluginConfig config;

        public SourcesPanel(PluginMain pluginMain, PluginConfig config)
        {
            InitializeComponent();

            this.pluginMain = pluginMain;
            this.config = config;

            this.menuFollowLatestLog.Checked = this.config.FollowLatestLog;
            this.listViewLog.VirtualListSize = PluginMain.Logger.Logs.Count;
            PluginMain.Logger.Logs.ListChanged += (o, e) =>
            {
                this.listViewLog.BeginUpdate();
                this.listViewLog.VirtualListSize = PluginMain.Logger.Logs.Count;
                if (this.config.FollowLatestLog && this.listViewLog.VirtualListSize > 0)
                {
                    this.listViewLog.EnsureVisible(this.listViewLog.VirtualListSize - 1);
                }
                this.listViewLog.EndUpdate();
            };

            InitializeOverlayConfigTabs();
        }

        private void InitializeOverlayConfigTabs()
        {
            foreach (var source in this.pluginMain.EventSources)
            {
                AddConfigTab(source);
            }
        }

        private void AddConfigTab(IEventSource source)
        {
            var tabPage = new TabPage
            {
                Name = source.Name,
                Text = source.GetType().Name
            };

            var addon = pluginMain.Addons.FirstOrDefault(x => x.EventSourceType == source.GetType());
            if (addon != null)
            {
                var control = addon.CreateEventSourceControlInstance(source);
                if (control != null)
                {
                    control.Dock = DockStyle.Fill;
                    control.BackColor = SystemColors.ControlLightLight;
                    tabPage.Controls.Add(control);

                    this.tabControl.TabPages.Add(tabPage);
                    this.tabControl.SelectTab(tabPage);
                }
            }
        }

        private void menuLogCopy_Click(object sender, EventArgs e)
        {
            if (listViewLog.SelectedIndices.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (int index in listViewLog.SelectedIndices)
                {
                    sb.AppendFormat(
                        "{0}: {1}: {2}",
                        PluginMain.Logger.Logs[index].Time,
                        PluginMain.Logger.Logs[index].Level,
                        PluginMain.Logger.Logs[index].Message);
                    sb.AppendLine();
                }
                Clipboard.SetText(sb.ToString());
            }
        }

        private void listViewLog_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex >= PluginMain.Logger.Logs.Count) 
            {
                e.Item = new ListViewItem();
                return;
            };

            try
            {
                var log = PluginMain.Logger.Logs[e.ItemIndex];
                e.Item = new ListViewItem(log.Time.ToString());
                e.Item.UseItemStyleForSubItems = true;
                e.Item.SubItems.Add(log.Level.ToString());
                e.Item.SubItems.Add(log.Message ?? "Null");

                e.Item.ForeColor = Color.Black;
                if (log.Level == LogLevel.Warning)
                {
                    e.Item.BackColor = Color.LightYellow;
                }
                else if (log.Level == LogLevel.Error)
                {
                    e.Item.BackColor = Color.LightPink;
                }
                else
                {
                    e.Item.BackColor = Color.White;
                }
            } catch(NullReferenceException)
            {
                // We should log this but can't since it'd spam the log like crazy.
            }
        }

        private void menuFollowLatestLog_Click(object sender, EventArgs e)
        {
            this.config.FollowLatestLog = menuFollowLatestLog.Checked;
        }

        private void menuClearLog_Click(object sender, EventArgs e)
        {
            PluginMain.Logger.Logs.Clear();
        }

        private void menuCopyLogAll_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var log in PluginMain.Logger.Logs)
            {
                sb.AppendFormat(
                    "{0}: {1}: {2}",
                    log.Time,
                    log.Level,
                    log.Message);
                sb.AppendLine();
            }
            Clipboard.SetText(sb.ToString());
        }
    }
}
