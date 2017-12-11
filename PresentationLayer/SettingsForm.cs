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
        public SettingsForm()
        {
            InitializeComponent();
            CustomInitialization();
        }
        public void CustomInitialization()
        {
            this.toolTip1.SetToolTip(this.label1, "Protect this application with a password.");
            this.tableLayoutPanel1.BackColor = CustomColors.ChatBodyBackColor;
        }
    }
}
