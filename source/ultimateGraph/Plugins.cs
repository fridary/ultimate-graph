using System;
using System.ComponentModel;
using System.Windows.Forms;
using pluginfo;
using pluginfo.Types;

namespace ultimateGraph
{
    public partial class Plugins : Form
    {
        #region Fields

        bool contextMenuStripEnabled = false;

        #endregion

        #region Plugins

        public Plugins()
        {
            InitializeComponent();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            contextMenuStripEnabled = true;
            this.treeView1.SelectedNode = e.Node;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            this.treeView1.SelectedNode = e.Node;
            this.DialogResult = DialogResult.OK;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.treeView1.SelectedNode == this.treeView1.Nodes[0])
            {
                this.label1.Text = null;
            }
            else
            {
                AvailablePlugin selectedPlugin = Global.Plugins.AvailablePlugins.Find(this.treeView1.SelectedNode.Text);
                if (selectedPlugin != null)
                    this.label1.Text = selectedPlugin.Instance.pDescription;
            }
        }

        private void chooseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (!contextMenuStripEnabled)
            {
                e.Cancel = true;
            }
            return;
        }

        private void Plugins_MouseMove(object sender, MouseEventArgs e)
        {
            if (contextMenuStripEnabled)
                contextMenuStripEnabled = false;
        }

        #endregion
    }
}