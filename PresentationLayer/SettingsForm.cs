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
    public partial class SettingsForm : Form
    {
        private Form1 mainForm;
        public Form1 MainForm
        {
            get { return this.mainForm; }
        }
        public SettingsForm(Form1 mf)
        {
            InitializeComponent();
            this.mainForm = mf;
            CustomInitialization();
        }
        public void CustomInitialization()
        {
            this.BackColor = CustomColors.ChatBodyBackColor;
            this.toolTip1.SetToolTip(this.label1, "Protect this application with a password.");
            this.tableLayoutPanel1.BackColor = CustomColors.ChatBodyBackColor;
            if (this.mainForm.WelcomeForm.Password != null)
            {
                PasswordSetAction();
            }
            else
            {
                PasswordNotSetAction();
            }
        }
        public void PasswordUpdatedAction()
        {
            string p = P2PSimpleChat.DAL.DataAccess.GetPassword();
            if (p == null)
            {
                PasswordNotSetAction();
                return;
            }
            string h = P2PSimpleChat.DAL.DataAccess.GetHint();
            this.mainForm.WelcomeForm.Password = p;
            this.mainForm.WelcomeForm.Hint = h;
            PasswordSetAction();
        }
        private void PasswordSetAction()
        {
            this.label2.Text = "Enabled";
            this.button1.Text = "Disable";
        }
        private void PasswordNotSetAction()
        {
            this.label2.Text = "Disabled";
            this.button1.Text = "Enable";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.button1.Text == "Enable")
            {
                new SettingsEnablePasswordForm(this).ShowDialog();
            }
            else
            {
                new SettingsDisablePasswordForm(this).ShowDialog();
            }
        }
    }
}
