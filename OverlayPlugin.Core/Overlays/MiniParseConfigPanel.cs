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
    public partial class MiniParseConfigPanel : UserControl
    {
        private MiniParseOverlayConfig config;
        private MiniParseOverlay overlay;

        static readonly List<KeyValuePair<string, GlobalHotkeyType>> hotkeyTypeDict = new List<KeyValuePair<string, GlobalHotkeyType>>()
        {
            new KeyValuePair<string, GlobalHotkeyType>(Localization.GetText(TextItem.ToggleVisible), GlobalHotkeyType.ToggleVisible),
            new KeyValuePair<string, GlobalHotkeyType>(Localization.GetText(TextItem.ToggleClickthru), GlobalHotkeyType.ToggleClickthru),
            new KeyValuePair<string, GlobalHotkeyType>(Localization.GetText(TextItem.ToggleLock), GlobalHotkeyType.ToggleLock)
        };

        public MiniParseConfigPanel(MiniParseOverlay overlay)
        {
            InitializeComponent();

            this.overlay = overlay;
            this.config = overlay.Config;

            SetupControlProperties();
            SetupConfigEventHandlers();
        }

        private void SetupControlProperties()
        {
            this.checkMiniParseVisible.Checked = config.IsVisible;
            this.checkMiniParseClickthru.Checked = config.IsClickThru;
            this.checkLock.Checked = config.IsLocked;
            this.textMiniParseUrl.Text = config.Url;
            this.checkActwsCompatbility.Checked = config.ActwsCompatibility;
            this.nudMaxFrameRate.Value = config.MaxFrameRate;
            this.checkEnableGlobalHotkey.Checked = config.GlobalHotkeyEnabled;
            this.textGlobalHotkey.Enabled = this.checkEnableGlobalHotkey.Checked;
            this.textGlobalHotkey.Text = Util.GetHotkeyString(config.GlobalHotkeyModifiers, config.GlobalHotkey);
            this.comboHotkeyType.DisplayMember = "Key";
            this.comboHotkeyType.ValueMember = "Value";
            this.comboHotkeyType.DataSource = hotkeyTypeDict;
            this.comboHotkeyType.SelectedValue = config.GlobalHotkeyType;
            this.comboHotkeyType.SelectedIndexChanged += ComboHotkeyMode_SelectedIndexChanged;
        }

        private void SetupConfigEventHandlers()
        {
            this.config.VisibleChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkMiniParseVisible.Checked = e.IsVisible;
                });
            };
            this.config.ClickThruChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkMiniParseClickthru.Checked = e.IsClickThru;
                });
            };
            this.config.UrlChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.textMiniParseUrl.Text = e.NewUrl;
                });
            };
            this.config.ActwsCompatibilityChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkActwsCompatbility.Checked = config.ActwsCompatibility;
                });
            };
            this.config.MaxFrameRateChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.nudMaxFrameRate.Value = e.NewFrameRate;
                });
            };
            this.config.GlobalHotkeyEnabledChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkEnableGlobalHotkey.Checked = e.NewGlobalHotkeyEnabled;
                    this.textGlobalHotkey.Enabled = this.checkEnableGlobalHotkey.Checked;
                });
            };
            this.config.GlobalHotkeyChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.textGlobalHotkey.Text = Util.GetHotkeyString(this.config.GlobalHotkeyModifiers, e.NewHotkey);
                });
            };
            this.config.GlobalHotkeyModifiersChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.textGlobalHotkey.Text = Util.GetHotkeyString(e.NewHotkey, this.config.GlobalHotkey);
                });
            };
            this.config.LockChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkLock.Checked = e.IsLocked;
                });
            };
            this.config.GlobalHotkeyTypeChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.comboHotkeyType.SelectedValue = e.NewHotkeyType;
                });
            };
            this.config.ActwsCompatibilityChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkActwsCompatbility.Checked = this.config.ActwsCompatibility;
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

        private void checkWindowVisible_CheckedChanged(object sender, EventArgs e)
        {
            this.config.IsVisible = checkMiniParseVisible.Checked;
        }

        private void checkMouseClickthru_CheckedChanged(object sender, EventArgs e)
        {
            this.config.IsClickThru = checkMiniParseClickthru.Checked;
        }

        private void textUrl_TextChanged(object sender, EventArgs e)
        {
            //this.config.Url = textMiniParseUrl.Text;
        }

        private void textMiniParseUrl_Leave(object sender, EventArgs e)
        {
            this.config.Url = textMiniParseUrl.Text;
        }

        private void ComboHotkeyMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            var value = (GlobalHotkeyType)this.comboHotkeyType.SelectedValue;
            this.config.GlobalHotkeyType = value;
        }

        private void nudMaxFrameRate_ValueChanged(object sender, EventArgs e)
        {
            this.config.MaxFrameRate = (int)nudMaxFrameRate.Value;
        }

        private void buttonReloadBrowser_Click(object sender, EventArgs e)
        {
            this.overlay.Reload();
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.config.Url = new Uri(ofd.FileName).ToString();
            }
        }

        private void buttonMiniParseOpenDevTools_Click(object sender, EventArgs e)
        {
            this.overlay.Overlay.Renderer.showDevTools();
        }

        private void buttonMiniParseOpenDevTools_RClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                this.overlay.Overlay.Renderer.showDevTools(false);
        }

        private void checkBoxEnableGlobalHotkey_CheckedChanged(object sender, EventArgs e)
        {
            this.config.GlobalHotkeyEnabled = this.checkEnableGlobalHotkey.Checked;
            this.textGlobalHotkey.Enabled = this.config.GlobalHotkeyEnabled;
        }

        private void textBoxGlobalHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            var key = Util.RemoveModifiers(e.KeyCode, e.Modifiers);
            this.config.GlobalHotkey = key;
            this.config.GlobalHotkeyModifiers = e.Modifiers;
        }

        private void checkLock_CheckedChanged(object sender, EventArgs e)
        {
            this.config.IsLocked = this.checkLock.Checked;
        }

        private void buttonResetOverlayPosition_Click(object sender, EventArgs e)
        {
            this.overlay.Overlay.Location = new Point(10, 10);
        }

        private void CheckActwsCompatbility_CheckedChanged(object sender, EventArgs e)
        {
            this.config.ActwsCompatibility = checkActwsCompatbility.Checked;
        }

        private void textGlobalHotkey_Enter(object sender, EventArgs e)
        {
            Registry.Resolve<KeyboardHook>().DisableHotKeys();
        }

        private void textGlobalHotkey_Leave(object sender, EventArgs e)
        {
            Registry.Resolve<KeyboardHook>().EnableHotKeys();
        }

        private void cbWhiteBg_CheckedChanged(object sender, EventArgs e)
        {
            var color = this.cbWhiteBg.Checked ? "white" : "transparent";
            this.overlay.ExecuteScript($"document.body.style.backgroundColor = \"{color}\";");
        }
    }
}
