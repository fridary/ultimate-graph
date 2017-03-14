using System;
using System.ComponentModel;
using System.Windows.Forms;
using pluginfo;
using pluginfo.Types;
using conf;

namespace plugEditor
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
            AvailablePlugin selectedPlugin = Global.Plugins.AvailablePlugins.Find(this.treeView1.SelectedNode.Text);
            if (selectedPlugin != null)
                this.label1.Text = selectedPlugin.Instance.pDescription;
        }

        private void chooseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = ErrorBox.Show("The plugin \"" + this.treeView1.SelectedNode.Text + "\" will be\r\nremoved completely. Are you sure?",
                Application.ProductName, ErrorBoxButtons.YesNo, ErrorBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    AvailablePlugin selectedPlugin = Global.Plugins.AvailablePlugins.Find(this.treeView1.SelectedNode.Text);
                    if (selectedPlugin != null)
                    {
                        string assembly = null;
                        for (int i = 0; i < selectedPlugin.AssemblyPath.Length; i++)
                            if (selectedPlugin.AssemblyPath[i] == '\\')
                                assembly = selectedPlugin.AssemblyPath.Remove(0, i + 1);

                        assembly = assembly.Remove(assembly.Length - 4, 4);
                        Global.Plugins.ClosePlugins();

                        //System.IO.File.Delete(Application.StartupPath + @"/plugins/" + assembly + @"/" + assembly + ".txt");
                        System.IO.File.Delete(Application.StartupPath + @"/plugins/" + assembly + @"/" + assembly + ".dll");
                        //System.IO.Directory.Delete(Application.StartupPath + @"/plugins/" + assembly);
                    }
                    else
                    {
                        MessageBox.Show("Can't find plugin directory!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error removing plugin! Original error:\r\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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