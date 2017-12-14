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
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            CustomInitialization();
        }
        private void CustomInitialization()
        {
            this.BackColor = CustomColors.ChatBodyBackColor;
            this.label4.Text = Application.ProductVersion;
        }
    }
}
