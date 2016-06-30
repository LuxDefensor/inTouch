using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace inTouch
{
    public partial class formMain : Form
    {
        private Font fontTime, fontUserCurrent, fontUserOther, fontMessage;
        private DateTime lastMessageStamp;

        public formMain()
        {
            InitializeComponent();
            lastMessageStamp = DateTime.Now.AddHours(-1);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                this.Visible = true;
                this.ShowInTaskbar = true;
            }
        }

        private void menuShow_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
        }

        private void menuHide_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
        }

        private void menuTrayExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void formMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.ShowInTaskbar = false;
                this.Visible = false;
                notifyIcon1.Visible = true;
            }
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            fontTime = new Font("Courier New", 10, FontStyle.Regular);
            fontUserCurrent = new Font("Courier New", 10, FontStyle.Bold);
            fontUserOther = new Font("Courier New", 10, FontStyle.Bold);
            fontMessage = new Font("Arial", 10, FontStyle.Regular);
            if (DataManager.TestConnection())
            {
                txtActiveUser.Text = DataManager.GetCurrentUser();
                try
                {
                    DataManager.RegisterUser(true);
                }
                catch (Exception ex)
                {
                    lblStatus.Text = DateTime.Now.ToShortTimeString() + " " + ex.Message;
                }
                UpdateList();
            }
            else
            {
                MessageBox.Show("Ошибка подключения к БД", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtActiveUser.Text = "No connection";
            }
            notifyIcon1.Visible = true;
            this.Cursor = Cursors.Default;
        }

        private void menuConnectionSettings_Click(object sender, EventArgs e)
        {
            formConnection frm = new formConnection();
            frm.ShowDialog();
            if (DataManager.TestConnection())
            {
                txtActiveUser.Text = DataManager.GetCurrentUser();
                try
                {
                    DataManager.RegisterUser(true);
                }
                catch (Exception ex)
                {
                    lblStatus.Text = DateTime.Now.ToShortTimeString() + " " + ex.Message;
                }
                UpdateList();
            }
        }

        private void formMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataManager.RegisterUser(false);
        }

        private void txtNewMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtNewMessage.TextLength > 0)
                {
                    try
                    {
                        DataManager.PostMessage(txtActiveUser.Text, txtNewMessage.Text);
                        lastMessageStamp = DateTime.Now;
                        lblStatus.Text= lastMessageStamp.ToShortTimeString() + " OK";
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Text = DateTime.Now.ToShortTimeString() + " " + ex.Message;
                    }
                    UpdateMessages();
                }
                e.SuppressKeyPress = true;
                txtNewMessage.Text = "";

            }
        }

        private void formMain_Activated(object sender, EventArgs e)
        {
            notifyIcon1.Icon = trayNoNew.Icon;
        }

        private void UpdateList()
        {
            lstUsers.Items.Clear();
            try
            {
                DataTable source = DataManager.UsersList();
                foreach (DataRow row in source.Rows)
                {
                    lstUsers.Items.Add(row[0]);
                }
                lblStatus.Text = DateTime.Now.ToShortTimeString() + " OK";
            }
            catch(Exception ex)
            {
                lblStatus.Text = DateTime.Now.ToShortTimeString() + " " + ex.Message;
            }
        }

        private void UpdateMessages()
        {
            int position;
            DataRow row;
            string message, timestamp, username;
            try
            {
                DataTable messages = DataManager.FetchMessages(lastMessageStamp);
                if (messages.Rows.Count > 0)
                {
                    lastMessageStamp = DateTime.Parse(messages.Rows[0]["Stamp"].ToString());
                    lastMessageStamp = lastMessageStamp.AddSeconds(1);
                    for (int i = messages.Rows.Count - 1; i >= 0; i--)
                    {
                        row = messages.Rows[i];
                        position = rtfMessages.TextLength;
                        message = row[2].ToString();
                        username = row[1].ToString();
                        timestamp = DateTime.Parse(row[3].ToString()).ToString("[dd.MM.yyyy HH:mm] ");

                        rtfMessages.AppendText(timestamp);
                        rtfMessages.Select(position, timestamp.Length);
                        rtfMessages.SelectionHangingIndent = 20;
                        rtfMessages.SelectionFont = fontTime;
                        position += timestamp.Length;

                        rtfMessages.AppendText(username);
                        rtfMessages.Select(position, username.Length);
                        if (username == DataManager.GetCurrentUser())
                        {
                            rtfMessages.SelectionFont = fontUserCurrent;
                            rtfMessages.SelectionColor = Color.Red;
                        }
                        else
                        {
                            rtfMessages.SelectionFont = fontUserOther;
                            rtfMessages.SelectionColor = Color.Blue;
                        }
                        rtfMessages.AppendText(" ");
                        position = position + username.Length + 1;

                        rtfMessages.AppendText(message);
                        rtfMessages.Select(position, message.Length);
                        rtfMessages.SelectionFont = fontMessage;
                        rtfMessages.SelectionColor = Color.Black;

                        rtfMessages.AppendText("\n");
                    }
                }
                lblStatus.Text = DateTime.Now.ToShortTimeString() + " OK";
            }
            catch (Exception ex)
            {
                lblStatus.Text = DateTime.Now.ToShortTimeString() + " " + ex.Message;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateList();
            if (DataManager.GetLastStamp() > lastMessageStamp)
            {
                UpdateMessages();
                if (this.Visible == false)
                {
                    notifyIcon1.Icon = trayNew.Icon;
                }
            }
        }
    }
}
