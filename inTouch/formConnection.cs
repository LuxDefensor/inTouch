using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace inTouch
{
    public partial class formConnection : Form
    {

        public formConnection()
        {
            InitializeComponent();
            this.Load += FormConnection_Load;
            btnOK.Click += BtnOK_Click;
            btnLocalServer.Click += BtnLocalServer_Click;
            btnTestConnection.Click += BtnTestConnection_Click;
        }

        private void BtnTestConnection_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            SaveSettings();
            if (DataManager.TestConnection())
                MessageBox.Show("Соединение с базой данных установлено",
                    "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Не удалось установить соединение с базой данных",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            this.Cursor = Cursors.Default;
        }

        private void BtnLocalServer_Click(object sender, EventArgs e)
        {
            txtServer.Text = "localhost";
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void FormConnection_Load(object sender, EventArgs e)
        {
            Properties.Settings settings = new Properties.Settings();
            txtServer.Text = settings.Server;
            txtDB.Text = settings.Database;
            txtLogDepth.Text = settings.LogDepth.ToString();
        }

        private void SaveSettings()
        {
            Properties.Settings settings = new Properties.Settings();
            settings["Server"] = txtServer.Text;
            settings["Database"] = txtDB.Text;
            settings["LogDepth"] = int.Parse(txtLogDepth.Text);
            settings.Save();
        }

    }
}
