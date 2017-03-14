using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using conf;
using control;
using pluginfo;
using pluginfo.Types;
using plugEditor;

namespace ultimateGraph
{
    public partial class AppForm : Form
    {
        #region Fields

        List<CPoint> dot;
        List<CLine> line;

        CPoint MyPoint;
        CLine MyLine;

        Options options;
        Plugins pluglist;

        string plug_name;
        string assembly;
        string plug_path;
        string file_name;
        string file_path;

        int open, exit, newfile, saved;

        #endregion

        #region AppForm

        public AppForm()
        {
            InitializeComponent();
            this.Graph1.wControlChange += new GraphEventHandler(GraphChange);

            this.Graph1.mClickChange += new MouseClickEventHandler(MouseClickChange);
            this.Graph1.mMoveChange += new MouseMoveEventHandler(MouseMoveChange);

            dot = Graph1.dot;
            line = Graph1.line;

            if (plug_name == null)
            {
                this.pName.Visible = false;
                this.pDesc.Visible = false;
            }

            ToolStripManager.Renderer = new SkinRenderer();
        }

        private void GraphChange(object sender, EventArgs e)
        {
            saved = 0;
            this.toolStripStatusLabel2.Text = "Вершин: " + dot.Count;
            this.toolStripStatusLabel3.Text = "Ребер: " + line.Count;

            AvailablePlugin selectedPlugin = Global.Plugins.AvailablePlugins.Find(plug_name);
            if (selectedPlugin != null)
            {
                selectedPlugin.Instance.pDot = dot;
                selectedPlugin.Instance.pLine = line;
                selectedPlugin.Instance.control_Initialize();
            }
        }

        private void PluginChange(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = "Вершин: " + dot.Count;
            this.toolStripStatusLabel3.Text = "Ребер: " + line.Count;

            AvailablePlugin selectedPlugin = Global.Plugins.AvailablePlugins.Find(plug_name);
            if (selectedPlugin != null)
            {
                //selectedPlugin.Instance.pTimerInterval = Graph1.timerInterval;
                this.toolStripStatusLabel1.Text = selectedPlugin.Instance.stripStatusText;
                this.Graph1.CancelNewEdge = selectedPlugin.Instance.CancelNewEdge;
                this.Graph1.CancelNewDot = selectedPlugin.Instance.CancelNewDot;
                Cursor = selectedPlugin.Instance.pCursor;
                selectedPlugin.Instance.control_Initialize();
            }

            Refresh();
        }

        private void MouseClickChange(object sender, MouseEventArgs e)
        {
            AvailablePlugin selectedPlugin = Global.Plugins.AvailablePlugins.Find(plug_name);
            if (selectedPlugin != null)
            {
                selectedPlugin.Instance.control_MouseClick(this, e);
            }
        }

        private void MouseMoveChange(object sender, MouseEventArgs e)
        {
            AvailablePlugin selectedPlugin = Global.Plugins.AvailablePlugins.Find(plug_name);
            if (selectedPlugin != null)
            {
                selectedPlugin.Instance.control_MouseMove(this, e);
            }
        }

        #endregion

        #region menuStrip

        private void SaveChanges(object sender, EventArgs e, string what)
        {
            DialogResult dr = ErrorBox.Show("Save graph changes?", Application.ProductName,
                ErrorBoxButtons.YesNoCancel, ErrorBoxIcon.Question);

            if (what == "new")
            {
                if (dr == DialogResult.Yes)
                {
                    newfile = 1;
                    saveAsToolStripMenuItem_Click(this, e);
                }
                else if (dr == DialogResult.No)
                {
                    file_path = "";
                    if (plug_name != null)
                        this.Text = Application.ProductName;
                    dot.Clear();
                    line.Clear();
                    Graph1.count = 0;
                }
            }
            else if (what == "exit")
            {
                if (dr == DialogResult.Yes)
                {
                    saveAsToolStripMenuItem_Click(this, e);
                }
                else if (dr == DialogResult.No)
                {
                    this.Close();
                }
                else if (dr == DialogResult.Cancel)
                {
                    exit = 0;
                }
            }
            else if (what == "open")
            {
                if (dr == DialogResult.Yes)
                {
                    open = 1;
                    saveAsToolStripMenuItem_Click(this, e);
                }
                else if (dr == DialogResult.No)
                {
                    open = 1;
                    openToolStripMenuItem_Click(this, e);
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (dot.Count > 0 && saved == 0)
            {
                this.SaveChanges(this, e, "new");
            }
            */

            file_path = "";
            if (plug_name != null)
                this.Text = Application.ProductName;
            Graph1.count = 0;

            int what = 1;
            Graph1.GraphChange(this, e, what);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (dot.Count > 0 && open != 1 && saved == 0)
            //{
            //    this.SaveChanges(this, e, "open");
            //}
            //else
            //{
                Stream myStream = null;
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = Application.StartupPath + @"\saved";
                openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    myStream = openFileDialog1.OpenFile();
                    StreamReader reader = new StreamReader(myStream);
                    try
                    {
                        if (myStream != null)
                        {
                            using (myStream)
                            {
                                dot.Clear();
                                line.Clear();
                                int k = Convert.ToInt32(reader.ReadLine());
                                int p = Convert.ToInt32(reader.ReadLine());
                                for (int i = 0; i < k; i++)
                                {
                                    MyPoint = new control.CPoint();
                                    MyPoint.mx = Convert.ToInt32(reader.ReadLine());
                                    MyPoint.my = Convert.ToInt32(reader.ReadLine());
                                    MyPoint.vertex = Convert.ToInt32(reader.ReadLine());
                                    dot.Add(MyPoint);
                                }
                                for (int i = 0; i < p; i++)
                                {
                                    MyLine = new control.CLine();
                                    MyLine.lx_1 = Convert.ToInt32(reader.ReadLine());
                                    MyLine.ly_1 = Convert.ToInt32(reader.ReadLine());
                                    MyLine.lx_2 = Convert.ToInt32(reader.ReadLine());
                                    MyLine.ly_2 = Convert.ToInt32(reader.ReadLine());
                                    MyLine.edge = Convert.ToInt32(reader.ReadLine());
                                    line.Add(MyLine);
                                }
                                Graph1.count = dot[dot.Count - 1].vertex;
                            }
                            file_path = openFileDialog1.FileName;
                            for (int i = 0; i < file_path.Length; i++)
                                if (file_path[i] == '\\')
                                    file_name = file_path.Remove(0, i + 1);
                            file_name = file_name.Remove(file_name.Length - 4, 4);
                            if (plug_name == null)
                                this.Text = Application.ProductName + " - " + file_name;
                            open = 0;
                            saved = 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not read file from the disk! Original error:\n" + ex.Message,
                            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            //}
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file_path == null)
            {
                saveAsToolStripMenuItem_Click(this, e);
            }
            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = file_path;
                Stream myStream = saveFileDialog1.OpenFile();
                StreamWriter writer = new StreamWriter(myStream);
                if (myStream != null)
                {
                    writer.WriteLine(dot.Count);
                    writer.WriteLine(line.Count);
                    foreach (control.CPoint m in dot)
                    {
                        writer.WriteLine(m.mx);
                        writer.WriteLine(m.my);
                        writer.WriteLine(m.vertex);
                    }
                    foreach (control.CLine n in line)
                    {
                        writer.WriteLine(n.lx_1);
                        writer.WriteLine(n.ly_1);
                        writer.WriteLine(n.lx_2);
                        writer.WriteLine(n.ly_2);
                        writer.WriteLine(n.edge);
                    }
                    writer.Close();
                    myStream.Close();
                    file_path = saveFileDialog1.FileName;

                    if (plug_name == null)
                        this.Text = Application.ProductName + " - " + file_name;
                    saved = 1;
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Application.StartupPath + @"\saved";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = saveFileDialog1.OpenFile();
                StreamWriter writer = new StreamWriter(myStream);
                if (myStream != null)
                {
                    writer.WriteLine(dot.Count);
                    writer.WriteLine(line.Count);
                    foreach (control.CPoint m in dot)
                    {
                        writer.WriteLine(m.mx);
                        writer.WriteLine(m.my);
                        writer.WriteLine(m.vertex);
                    }
                    foreach (control.CLine n in line)
                    {
                        writer.WriteLine(n.lx_1);
                        writer.WriteLine(n.ly_1);
                        writer.WriteLine(n.lx_2);
                        writer.WriteLine(n.ly_2);
                        writer.WriteLine(n.edge);
                    }
                    writer.Close();
                    myStream.Close();
                }
                file_path = saveFileDialog1.FileName;
                for (int i = 0; i < file_path.Length; i++)
                    if (file_path[i] == '\\')
                        file_name = file_path.Remove(0, i + 1);
                file_name = file_name.Remove(file_name.Length - 4, 4);

                if (plug_name == null)
                    this.Text = Application.ProductName + " - " + file_name;
                saved = 1;
            }
            else
            {
                exit = 0;
            }

            if (exit == 1)
                this.Close();
            if (open == 1)
                openToolStripButton_Click(this, e);
            if (newfile == 1)
            {
                newfile = 0;
                dot.Clear();
                line.Clear();
            }
        }

        private void plugEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorForm pld = new EditorForm();
            pld.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (dot.Count > 0)
            //{
            //    exit = 1;
            //    this.SaveChanges(this, e, "exit");
            //}
            //else
            //{
                this.Close();
            //}
        }

        private void chooseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pluglist = new Plugins();
            Global.Plugins.ClosePlugins();
            Global.Plugins.FindPlugins(Application.StartupPath + @"\plugins");

            foreach (AvailablePlugin pluginOn in Global.Plugins.AvailablePlugins)
            {
                TreeNode newNode = new TreeNode(pluginOn.Instance.pName);
                pluglist.treeView1.Nodes.Add(newNode);
                newNode = null;
            }

            if (plug_name == null)
            {
                pluglist.treeView1.SelectedNode = pluglist.treeView1.Nodes[0];
            }
            else
            {
                for (int i = 0; i < pluglist.treeView1.Nodes.Count; i++)
                {
                    if (plug_name == pluglist.treeView1.Nodes[i].ToString())
                    {
                        pluglist.treeView1.SelectedNode = pluglist.treeView1.Nodes[i];
                        break;
                    }
                }
            }

            pluglist.ShowDialog();

            if (pluglist.DialogResult == DialogResult.OK)
            {
                if (pluglist.treeView1.SelectedNode == pluglist.treeView1.Nodes[0])
                {
                    if (plug_name != null)
                        this.Text = Application.ProductName;
                    else
                        plug_name = null;
                    this.panelPlugin.Visible = false;
                    this.pDesc.Visible = false;
                    this.pName.Visible = false;
                    Graph1.plug = false;
                }
                else
                {
                    AvailablePlugin selectedPlugin = Global.Plugins.AvailablePlugins.Find(pluglist.treeView1.SelectedNode.Text);
                    if (selectedPlugin != null)
                    {
                        plug_name = selectedPlugin.Instance.pName;
                        plug_path = selectedPlugin.AssemblyPath;
                        Graph1.plug = true;
                        this.Text = Application.ProductName + " - " + plug_name;
                        for (int i = 0; i < plug_path.Length; i++)
                            if (plug_path[i] == '\\')
                                assembly = plug_path.Remove(0, i + 1);
                        this.panelPlugin.Visible = true;
                        this.pDesc.Visible = true;
                        this.pName.Visible = true;

                        this.pName.Text = selectedPlugin.Instance.pName;
                        this.pDesc.Text = selectedPlugin.Instance.pDescription;

                        this.panelPlugin.Controls.Clear();
                        selectedPlugin.Instance.MainInterface.Dock = DockStyle.Fill;
                        this.panelPlugin.Controls.Add(selectedPlugin.Instance.MainInterface);

                        selectedPlugin.Instance.pDot = dot;
                        selectedPlugin.Instance.pLine = line;
                        //selectedPlugin.Instance.pTimerInterval = Graph1.timerInterval;
                        Graph1.timerInterval = selectedPlugin.Instance.pTimerIntervalDefault;
                        selectedPlugin.Instance.Radius = this.Graph1.Radius;
                        selectedPlugin.Instance.control_Initialize();

                        selectedPlugin.Instance.plugChange += new PluginEventHandler(this.PluginChange);
                    }
                }
            }
            else
            {
                Global.Plugins.ClosePlugins();
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int what = 1;
            Graph1.GraphChange(this, e, what);
        }

        private void toolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.toolbarToolStripMenuItem.Checked == true)
            {
                this.toolbarToolStripMenuItem.Checked = false;
                this.toolbarToolStripMenuItem1.Checked = false;
                this.toolStrip1.Visible = false;
            }
            else
            {
                this.toolbarToolStripMenuItem.Checked = true;
                this.toolbarToolStripMenuItem1.Checked = true;
                this.toolStrip1.Visible = true;
            }
        }

        private void toolbarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.toolbarToolStripMenuItem1.Checked == true)
            {
                this.toolbarToolStripMenuItem.Checked = false;
                this.toolbarToolStripMenuItem1.Checked = false;
                this.toolStrip1.Visible = false;
            }
            else
            {
                this.toolbarToolStripMenuItem.Checked = true;
                this.toolbarToolStripMenuItem1.Checked = true;
                this.toolStrip1.Visible = true;
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            options = new Options();
            options.general1.checkBox1.Checked = this.Graph1.checkbox1;
            options.general1.checkBox2.Checked = this.Graph1.checkbox2;
            options.general1.checkBox3.Checked = this.Graph1.checkbox3;
            options.general1.checkBox4.Checked = this.Graph1.checkbox4;
            options.general1.checkBox5.Checked = this.Graph1.checkbox5;
            options.general1.numericUpDown1.Value = this.Graph1.timerInterval;
            options.ShowDialog();

            if (options.DialogResult == DialogResult.OK)
                OptionsChange(this, e);
        }

        private void OptionsChange(object sender, EventArgs e)
        {
            bool temp_checkbox1 = this.Graph1.checkbox1;

            this.Graph1.checkbox1 = options.general1.checkBox1.Checked;
            this.Graph1.checkbox2 = options.general1.checkBox2.Checked;
            this.Graph1.checkbox3 = options.general1.checkBox3.Checked;
            this.Graph1.checkbox4 = options.general1.checkBox4.Checked;
            this.Graph1.checkbox5 = options.general1.checkBox5.Checked;
            this.Graph1.timerInterval = options.general1.numericUpDown1.Value;
            if (temp_checkbox1 != this.Graph1.checkbox1)
                if (this.Graph1.checkbox1)
                    this.Graph1.Radius = this.Graph1.RadiusVertex;
                else
                    this.Graph1.Radius = this.Graph1.RadiusVertex - this.Graph1.radiusDifference;

            if (plug_name != null)
            {
                AvailablePlugin selectedPlugin = Global.Plugins.AvailablePlugins.Find(pluglist.treeView1.SelectedNode.Text);
                if (selectedPlugin != null)
                {
                    selectedPlugin.Instance.Radius = this.Graph1.Radius;
                    selectedPlugin.Instance.pTimerInterval = this.Graph1.timerInterval;
                    selectedPlugin.Instance.control_Initialize();
                }
            }

            //Refresh();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About ab = new About();
            ab.ShowDialog();
        }

        #endregion

        #region toolStrip

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            newToolStripMenuItem_Click(this, e);
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(this, e);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(this, e);
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            int what = 1;
            Graph1.GraphChange(this, e, what);
        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            aboutToolStripMenuItem_Click(this, e);
        }

        private void AppForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            if (dot.Count > 0 && saved == 0)
            {
                DialogResult dr = ErrorBox.Show("Save graph changes?", Application.ProductName,
                    ErrorBoxButtons.YesNoCancel, ErrorBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {
                    saveAsToolStripMenuItem_Click(this, e);
                    exit = 1;
                    e.Cancel = true;
                }
                else if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            */
        }

        #endregion
    }

    #region Program

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppForm());
        }
    }

    #endregion
}