using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RainbowMage.OverlayPlugin
{
    public partial class NewOverlayDialog : Form
    {
        public delegate bool ValidateNameDelegate(string name);

        public ValidateNameDelegate NameValidator { get; set; }

        public string OverlayName { get; set; }
        public Type SelectedOverlayType { get; set; }

        private PluginMain pluginMain;

        public NewOverlayDialog(PluginMain pluginMain)
        {
            InitializeComponent();

            this.pluginMain = pluginMain;

            // Default validator
            this.NameValidator = (name) => { return name != null; };

            foreach (var overlayType in Registry.Overlays)
            {
                var name = overlayType.Name;
                if (name.EndsWith("Overlay"))
                {
                    name = name.Substring(0, name.Length - 7);
                }

                comboBox1.Items.Add(new KeyValuePair<string, Type>(name, overlayType));
            }

            comboBox1.DisplayMember = "Name";
            comboBox1.SelectedIndex = 0;

            textBox1.Focus();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (this.NameValidator(this.textBox1.Text))
            {
                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Please select overlay type.");
                    this.DialogResult = System.Windows.Forms.DialogResult.None;
                }
                else
                {
                    this.OverlayName = textBox1.Text;
                    this.SelectedOverlayType = (Type)comboBox1.SelectedItem;
                }
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.None;
            }
        }

        private class ComboItem
        {
            public Type OverlayType { get; set; }
            public string FriendlyName { get; set; }

            public ComboItem(Type overlayType, string friendlyName)
            {
                OverlayType = overlayType;
                FriendlyName = friendlyName;
            }
        }
    }
}
