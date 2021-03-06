﻿using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace FileMasta.Windows
{
    public partial class OptionsWindow : Form
    {
        public OptionsWindow()
        {
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            LoadUserSettings();
        }

        /// <summary>
        /// Enables/Disables Proxy Settings
        /// </summary>
        private void checkBoxConnectionDefault_CheckedChanged(object sender, EventArgs e)
        {
            // Connection (Enabled/Disabled)
            textBoxConnectionAddress.Enabled = Properties.Settings.Default.proxyUseCustom;
            textBoxConnectionPort.Enabled = Properties.Settings.Default.proxyUseCustom;
            textBoxConnectionUsername.Enabled = Properties.Settings.Default.proxyUseCustom;
            textBoxConnectionPassword.Enabled = Properties.Settings.Default.proxyUseCustom;
        }

        /// <summary>
        /// Set UI/WebClient/WebRequest Properties
        /// </summary>
        public void LoadUserSettings()
        {
            // General
            checkBoxGeneralClearDataOnClose.Checked = Properties.Settings.Default.clearDataOnClose;

            // Connection (Enabled/Disabled)
            textBoxConnectionAddress.Enabled = Properties.Settings.Default.proxyUseCustom;
            textBoxConnectionPort.Enabled = Properties.Settings.Default.proxyUseCustom;
            textBoxConnectionUsername.Enabled = Properties.Settings.Default.proxyUseCustom;
            textBoxConnectionPassword.Enabled = Properties.Settings.Default.proxyUseCustom;

            // Connection
            checkBoxConnectionDefault.Checked = Properties.Settings.Default.proxyUseCustom;
            textBoxConnectionAddress.Text = Properties.Settings.Default.proxyAddress;
            textBoxConnectionPort.Text = Convert.ToString(Properties.Settings.Default.proxyPort);
            textBoxConnectionUsername.Text = Properties.Settings.Default.proxyUsername;
            textBoxConnectionPassword.Text = Properties.Settings.Default.proxyPassword;

            // Set Proxy Settings
            if (MainForm.UseWebProxyCustom)
            {
                if (Uri.TryCreate(Properties.Settings.Default.proxyAddress + ":" + Properties.Settings.Default.proxyPort, UriKind.RelativeOrAbsolute, out Uri result))
                {
                    MainForm._webClient.Proxy = new WebProxy(result);

                    if (Properties.Settings.Default.proxyUsername == "" && Properties.Settings.Default.proxyPassword == "")
                        MainForm._webClient.UseDefaultCredentials = true;
                    else
                    {
                        MainForm._webClient.UseDefaultCredentials = false;
                        MainForm._webClient.Credentials = new NetworkCredential(Properties.Settings.Default.proxyUsername, Properties.Settings.Default.proxyPassword);
                    }
                }
                else
                    MessageBox.Show(this, "Invalid Address/Port");
            }
            else
            {
                MainForm._webClient.UseDefaultCredentials = true;
#pragma warning disable CS0618 // Type or member is obsolete
                MainForm._webClient.Proxy = WebProxy.GetDefaultProxy();
#pragma warning restore CS0618 // Type or member is obsolete
                MainForm._webClient.Credentials = CredentialCache.DefaultCredentials;
            }
        }

        private void buttonRestore_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

            Thread.Sleep(500);
            LoadUserSettings();
            Thread.Sleep(500);
            MessageBox.Show("Settings restored to default.");
            Program.log.Info("Restored user settings");
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // General
            Properties.Settings.Default.clearDataOnClose = checkBoxGeneralClearDataOnClose.Checked;

            // Connection
            Properties.Settings.Default.proxyUseCustom = checkBoxConnectionDefault.Checked;
            Properties.Settings.Default.proxyAddress = textBoxConnectionAddress.Text;
            Properties.Settings.Default.proxyPort = Convert.ToInt32(textBoxConnectionPort.Text);
            Properties.Settings.Default.proxyUsername = textBoxConnectionUsername.Text;
            Properties.Settings.Default.proxyPassword = textBoxConnectionPassword.Text;

            MainForm.UseWebProxyCustom = Properties.Settings.Default.proxyUseCustom;

            Thread.Sleep(500);
            Properties.Settings.Default.Save();
            Thread.Sleep(500);
            LoadUserSettings();
            MessageBox.Show("Settings saved.");
            Program.log.Info("Saved user settings");
        }
    }
}
