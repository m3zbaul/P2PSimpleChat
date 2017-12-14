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
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            CustomInitialization();
        }
        private void CustomInitialization()
        {
            this.BackColor = CustomColors.ChatBodyBackColor;
            this.tableLayoutPanel3.BackColor = CustomColors.Lynch;
            this.label1.ForeColor = System.Drawing.Color.White;

            // add topic nodes
            List<string> topics = P2PSimpleChat.DAL.DataAccess.GetHelpTopicList();
            foreach (string topic in topics)
            {
                TreeNode tn = new TreeNode();
                tn.Name = topic;
                tn.Text = topic;
                this.treeView1.Nodes.Add(tn);
            }
            this.treeView1.ExpandAll();
            this.treeView1.SelectedNode = null;

            this.label1.Text = this.label2.Text = "";
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.label1.Text = this.treeView1.SelectedNode.Text;
            this.label2.Text = P2PSimpleChat.DAL.DataAccess.GetDiscussionByTopic(this.treeView1.SelectedNode.Text);
        }
    }
}
