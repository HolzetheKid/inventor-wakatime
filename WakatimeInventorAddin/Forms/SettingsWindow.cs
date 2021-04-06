using System;
using System.Windows.Forms;
using WakaTime;

namespace WakatimeInventorAddIn.Forms
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();
            this.txtAPIKey.Text = WakaTimeConfigFile.ApiKey;
            this.txtBoxProxy.Text = WakaTimeConfigFile.Proxy;
            this.ckboxDebug.Checked = WakaTimeConfigFile.Debug;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Guid apiKey;
            var parse = Guid.TryParse(txtAPIKey.Text.Trim(), out apiKey);
            if (parse)
            {
                WakaTimeConfigFile.Debug = ckboxDebug.Checked;
                WakaTimeConfigFile.ApiKey = txtAPIKey.Text.Trim();
                WakaTimeConfigFile.Proxy = txtBoxProxy.Text.Trim();
                WakaTimeConfigFile.Save();
            }
            else
            {
                MessageBox.Show("Please enter valid Api Key.");
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}