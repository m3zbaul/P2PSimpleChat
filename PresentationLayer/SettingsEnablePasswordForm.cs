using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2PSimpleChat.PL
{
    public partial class SettingsEnablePasswordForm : Form
    {
        private SettingsForm settingsForm;
        public SettingsEnablePasswordForm()
        {
            InitializeComponent();

        }
        public SettingsEnablePasswordForm(SettingsForm sf)
        {
            InitializeComponent();
            CustomInitialization();
            this.settingsForm = sf;

        }
        private void CustomInitialization()
        {
            this.BackColor = CustomColors.ChatBodyBackColor;
        }

        private void SettingsEnablePasswordForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == this.textBox2.Text)
            {
                if (this.textBox1.Text == null || this.textBox1.Text == "" || this.textBox3.Text == null || this.textBox3.Text == "")
                {
                    MessageBox.Show("You cannot leave any field empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (P2PSimpleChat.BLL.Utility.IsValidPassword(textBox1.Text))
                {
                    if (P2PSimpleChat.BLL.Utility.IsValidHint(textBox3.Text))
                    {
                        string message = P2PSimpleChat.DAL.DataAccess.SetPassword(this.textBox1.Text, this.textBox3.Text);
                        if (message == null)
                        {
                            this.settingsForm.PasswordUpdatedAction();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Hint contains invalid characters!\nValid characters: a-z, A-Z, 0-9, -, _ and space", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                }
                else
                {
                    MessageBox.Show("Password contains invalid characters!\nValid characters: a-z, A-Z, 0-9, -, _, $", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Password fields do not match!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
