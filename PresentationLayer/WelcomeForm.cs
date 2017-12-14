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
    public partial class WelcomeForm : Form
    {
        private string password;
        private string hint;
        private Form1 mainForm;

        public string Hint
        {
            get { return this.hint; }
            set { this.hint = value; }
        }
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }
        public WelcomeForm()
        {
            InitializeComponent();
            CustomInitialization();
        }
        public void CustomInitialization()
        {
            this.BackColor = CustomColors.ChatBodyBackColor;
            try
            {
                password = P2PSimpleChat.DAL.DataAccess.GetPassword();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (password == null)
            {
                PasswordNotSetAction();
            }
            else
            {
                hint = P2PSimpleChat.DAL.DataAccess.GetHint();
                PasswordSetAction();
            }
            this.button1.BackColor = CustomColors.Lynch;
            this.button2.BackColor = CustomColors.Lynch;
        }
        private void PasswordSetAction()
        {
            this.label3.Show();
            this.tableLayoutPanel5.Show();
            this.tableLayoutPanel6.Show();
            this.button3.Hide();
        }
        private void PasswordNotSetAction()
        {
            this.label3.Hide();
            this.tableLayoutPanel5.Hide();
            this.tableLayoutPanel6.Hide();
            this.button3.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hint: " + this.hint, "Password Hint", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != null && textBox1.Text == this.password)
            {
                this.button3.Show();
                this.PasswordNotSetAction();
            }
            else
            {
                MessageBox.Show("Wrong password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.PasswordSetAction();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.mainForm == null)
            {
                this.mainForm = new Form1(this);
            }
            this.mainForm.Show();
            this.Hide();
        }
    }
}
