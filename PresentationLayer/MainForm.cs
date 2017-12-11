using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2PSimpleChat.PL
{
    public partial class Form1 : Form
    {
        P2PSimpleChat.BLL.Server server;
        P2PSimpleChat.BLL.Client client;
        private string saveFilePathPrefix = "D:\\";
        private bool fileSelected = false;

        public Form1()
        {
            InitializeComponent();
            InitializeCustom();
            //this.listBox1.Text = "";
            this.textBox1.Text = this.textBox3.Text = "127.0.0.1";
        }
        private void InitializeCustom()
        {
            this.label7.Text = "";
            this.toolTip1.SetToolTip(this.button5, "Help");
            this.toolTip1.SetToolTip(this.button6, "Settings");
            this.toolTip1.SetToolTip(this.button7, "About");
            this.button5.FlatAppearance.MouseOverBackColor = this.button5.FlatAppearance.MouseDownBackColor = CustomColors.ChatBodyBackColor;
            this.button6.FlatAppearance.MouseOverBackColor = this.button6.FlatAppearance.MouseDownBackColor = CustomColors.ChatBodyBackColor;
            this.button7.FlatAppearance.MouseOverBackColor = this.button7.FlatAppearance.MouseDownBackColor = CustomColors.ChatBodyBackColor;
            this.tableLayoutPanel1.BackColor = CustomColors.ChatBodyBackColor;
            this.button3.BackColor = CustomColors.SendButtonBackColor;
            this.button4.BackColor = CustomColors.SelectFileBackColor;
            richTextBox2.BackColor = CustomColors.SoftWhite;
            this.richTextBox2.BackColor = CustomColors.SoftWhite;
            this.textBox1.BackColor = CustomColors.SoftWhite;
            this.textBox2.BackColor = CustomColors.SoftWhite;
            this.textBox3.BackColor = CustomColors.SoftWhite;
            this.textBox4.BackColor = CustomColors.SoftWhite;
            this.button1.BackColor = this.button2.BackColor = CustomColors.Lynch;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.button1.Text == "Stop")
            {
                this.server.Stop();
                System.Threading.Thread.Sleep(200);
                this.server = null;
                this.button1.Text = "Start";
                return;
            }
            // peer start button click
            if (P2PSimpleChat.BLL.Utility.IsValidIPv4(this.textBox1.Text) && P2PSimpleChat.BLL.Utility.IsNumeric(this.textBox2.Text))
            {
                this.server = new P2PSimpleChat.BLL.Server(textBox1.Text, textBox2.Text);
                System.Threading.Thread.Sleep(200);
                if (P2PSimpleChat.BLL.Server.ServerStartedSuccessfully)
                {
                    //this.label7.Text = "Success!";
                    //this.groupBox1.Enabled = false;
                    this.button1.Text = "Stop";
                    server.ClientDataReceived += Server_ClientDataReceived;
                    this.label7.Text = "Peer started successfully!";
                    this.label7.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    this.AppendText("*** Error: " + P2PSimpleChat.BLL.Server.ErrorMessage + " ***", System.Drawing.Color.Red, true);
                    this.server = null;
                    this.groupBox1.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("Invalid ip address or port number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClientDisconnectAction()
        {
            this.AppendText("*** " + textBox3.Text + ":" + textBox4.Text + " disconnected***", System.Drawing.Color.Red, true);
            this.groupBox2.Enabled = true;
            this.textBox3.Text = "";
            this.textBox4.Text = "";
            this.client.Close();
            this.client = null;
        }
        private void ClientMessageReceivedAction(byte[] data)
        {
            byte[] target = new byte[data.Length - 3];
            Array.Copy(data, 3, target, 0, target.Length);
            this.AppendText("Peer> " + Encoding.ASCII.GetString(target), System.Drawing.Color.RosyBrown, true);
        }
        private void ClientTypingAction()
        {
            this.label7.Text = "Peer is typing...";
            this.label7.ForeColor = System.Drawing.Color.Black;
        }
        private void ClientFileReceivedAction(byte[] data)
        {
            byte[] target = new byte[data.Length - 4];
            string ext = "";
            Array.Copy(data, 4, target, 0, target.Length);
            if (data[3] == 1)
            {
                ext = "png";
            }
            else if (data[3] == 2)
            {
                ext = "txt";
            }
            else
            {
                ext = "unknown";
            }
            string fileName = GetTimestamp(DateTime.Now) + "." + ext;
            try
            {
                string base64 = Encoding.UTF8.GetString(target);
                File.WriteAllBytes(this.saveFilePathPrefix + fileName, Convert.FromBase64String(base64));
                this.AppendText("*** File received: " + this.saveFilePathPrefix + fileName + " ***", System.Drawing.Color.BlueViolet, true);
            }
            catch (Exception ex)
            {
                this.label7.Text = "Error: couldn't store received file: " + ex.Message + ". "+ fileName;
                this.label7.ForeColor = System.Drawing.Color.Red;
            }
        }

        private string GetTimestamp(DateTime now)
        {
            return now.ToString("yyyyMMddHHmmssff");
        }

        private void ClientRequestReceivedAction(byte[] data)
        {
            //MessageBox.Show("New request");
            if (this.client == null)
            {
                try
                {
                    byte[] d = new byte[data.Length - 3];
                    Array.Copy(data, 3, d, 0, d.Length);
                    string[] s = Encoding.ASCII.GetString(d).Split(':');
                    this.client = P2PSimpleChat.BLL.Client.GetClientObject(s[0], s[1]);
                    this.textBox3.Text = s[0];
                    this.textBox4.Text = s[1];
                    this.groupBox2.Enabled = false;
                    this.label7.Text = "Connected to peer successfully!";
                }
                catch (Exception ex)
                {
                    this.client = null;
                    this.groupBox2.Enabled = true;
                    this.label7.Text = "Error occured! " + ex.Message;
                    this.label7.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
        private void ClientDataProcessor(byte[] data)
        {
            if (data[2] == 2) // message
            {
                ClientMessageReceivedAction(data);
            }
            else if (data[2] == 1)
            {
                ClientRequestReceivedAction(data);
            }
            else if (data[2] == 3) // file received
            {
                ClientFileReceivedAction(data);
            }
            else if (data[2] == 4) // typing
            {
                ClientTypingAction();
            }
            else if (data[2] == 9) // disconnect
            {
                ClientDisconnectAction();
            }
        }
        private void Server_ClientDataReceived(object sender, byte[] data)
        {
            if (this.textBox1.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    ClientDataProcessor(data);
                }));
            }
            else
            {
                ClientDataProcessor(data);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.button2.Text == "Disconnect")
            {
                this.client.Close();
                this.server.CloseClient();
                this.button2.Text = "Connect";
            }
            // connect to peer button click
            if (P2PSimpleChat.BLL.Utility.IsValidIPv4(this.textBox3.Text) && P2PSimpleChat.BLL.Utility.IsNumeric(this.textBox4.Text))
            {
                if (this.server == null)
                {
                    MessageBox.Show("You must start your peer first before connecting to another peer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        this.client = P2PSimpleChat.BLL.Client.GetClientObject(textBox3.Text, textBox4.Text);
                        this.groupBox2.Enabled = false;
                        this.client.SendRequest(this.textBox1.Text, this.textBox2.Text);
                        this.label7.Text = "Connected to peer successfully!";
                        this.label7.ForeColor = System.Drawing.Color.Green;
                        this.button2.Text = "Disconnect";
                    }
                    catch (Exception ex)
                    {
                        this.client = null;
                        this.groupBox2.Enabled = true;
                        this.label7.Text = "Failure! " + ex.Message;
                        this.label7.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid ip address or port number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FileSendAction()
        {
            try
            {
                this.client.SendFile(this.openFileDialog1.FileName);

                this.button4.Text = "Send File";
                this.toolTip1.SetToolTip(this.button4, null);
                fileSelected = false;
                this.richTextBox1.Text = "";
                this.AppendText("*** File sent: " + this.openFileDialog1.FileName + " ***", System.Drawing.Color.Black, true);
                this.openFileDialog1.FileName = null;
            }
            catch (Exception ex)
            {
                this.label7.Text = ex.Message;
                this.label7.ForeColor = System.Drawing.Color.Red;
            }
        }
        private void MessageSendAction()
        {
            this.client.SendMessage(richTextBox1.Text);
            this.AppendText("Me> " + richTextBox1.Text, System.Drawing.Color.Purple, true);
            this.richTextBox1.Text = "";
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            // send message button click
            if (this.client != null)
            {
                try
                {
                    if (fileSelected)
                    {
                        FileSendAction();
                    }
                    else
                    {
                        MessageSendAction();
                    }
                }
                catch (Exception ex)
                {
                    this.label7.Text = ex.Message;
                    this.label7.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                MessageBox.Show("You are not connected to any peer yet.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.server != null)
            {
                this.server.Stop();
            }
            if (this.client != null)
            {
                this.client.Close();
            }
        }
        private void AppendText(string text, Color color, bool addNewLine = false)
        {
            richTextBox2.SuspendLayout();
            richTextBox2.SelectionColor = color;
            richTextBox2.AppendText(addNewLine
                ? $"{text}{Environment.NewLine}"
                : text);
            richTextBox2.ScrollToCaret();
            richTextBox2.ResumeLayout();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            // open file
            this.openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string path = this.openFileDialog1.FileName;
            this.button4.Text = path;
            this.toolTip1.SetToolTip(this.button4, path);
            fileSelected = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) 
                ||P2PSimpleChat.BLL.Utility.IsValidIPv4(textBox1.Text) == false
               )
            {
                this.textBox1.BackColor = CustomColors.ErrorBackColor;
                this.textBox1.ForeColor = System.Drawing.Color.White;
            }
            else
            {
                this.textBox1.BackColor = CustomColors.SoftWhite;
                this.textBox1.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.label7.Text = "";
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.client != null)
            {
                this.client.SendTyping();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text)
                || P2PSimpleChat.BLL.Utility.IsNumeric(textBox2.Text) == false
               )
            {
                this.textBox2.BackColor = CustomColors.ErrorBackColor;
                this.textBox2.ForeColor = System.Drawing.Color.White;
            }
            else
            {
                this.textBox2.BackColor = CustomColors.SoftWhite;
                this.textBox2.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text)
                || P2PSimpleChat.BLL.Utility.IsValidIPv4(textBox3.Text) == false
               )
            {
                this.textBox3.BackColor = CustomColors.ErrorBackColor;
                this.textBox3.ForeColor = System.Drawing.Color.White;
            }
            else
            {
                this.textBox3.BackColor = CustomColors.SoftWhite;
                this.textBox3.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text == ""
                || (P2PSimpleChat.BLL.Utility.IsNumeric(textBox4.Text) == false)
               )
            {
                this.textBox4.BackColor = CustomColors.ErrorBackColor;
                this.textBox4.ForeColor = System.Drawing.Color.White;
            }
            else
            {
                this.textBox4.BackColor = CustomColors.SoftWhite;
                this.textBox4.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new HelpForm().Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new SettingsForm().Show();
        }
    }
}
