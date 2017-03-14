using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using conf;
using pluginfo;
using pluginfo.Types;

namespace plugEditor
{
    public partial class EditorForm : Form
    {
        #region Fields

        Plugins listalg;

        string plug_path;
        string plug_name;
        string plug_descr;
        string assembly;
        string codeStart;
        string codeEnd;

        int saved;

        #endregion

        #region EditorForm

        public EditorForm()
        {
            InitializeComponent();
            ToolStripManager.Renderer = new SkinRenderer();
        }

        private string textBoxCode
        {
            get { return codeBox1.richTextBox1.Text; }
            set
            {
                this.codeBox1.richTextBox1.Text = value;
                this.codeBox1.richTextBox1.SelectionLength = 0;
            }
        }

        #endregion

        #region toolStrip

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewPlug newp = new NewPlug();
            newp.ShowDialog();

            if (newp.DialogResult == DialogResult.OK)
            {
                int get_it = 0;
                if (Directory.Exists(Application.StartupPath + @"/plugins/" + newp.assembly))
                {
                    DialogResult dr = ErrorBox.Show("The plugin \"" + newp.textBox1.Text + "\" is already exists.\r\nContinue?",
                        Application.ProductName, ErrorBoxButtons.YesNo, ErrorBoxIcon.Question);

                    if (dr != DialogResult.Yes)
                        get_it = 1;
                }

                if (get_it != 1)
                {
                    this.optionsToolStripMenuItem.Enabled = true;
                    this.startCompilingToolStripMenuItem.Enabled = true;
                    textBoxCode = null;
                    this.codeBox1.richTextBox1.ReadOnly = false;
                    this.codeBox1.richTextBox1.BackColor = SystemColors.Window;
                    this.Text = Application.ProductName + " - " + newp.textBox1.Text;

                    plug_name = newp.textBox1.Text;
                    plug_descr = newp.textBox2.Text;
                    assembly = newp.assembly;

                    Assembly _assembly = Assembly.GetExecutingAssembly();
                    StreamReader _textStreamReader;

                    //-----+

                    _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("plugEditor.defaultcode.defaultcode.txt"));
                    if (_textStreamReader.Peek() != -1)
                        textBoxCode = _textStreamReader.ReadToEnd();

                    textBoxCode = textBoxCode.Replace("plug_name", plug_name);
                    textBoxCode = textBoxCode.Replace("plug_descr", plug_descr);
                    textBoxCode = textBoxCode.Replace("assembly_name", assembly);

                    //-----+

                    codeEnd = "}";
                    codeEnd += "}";

                    //-----+
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listalg = new Plugins();
            Global.Plugins.ClosePlugins();
            Global.Plugins.FindPlugins(Application.StartupPath + @"\plugins");

            foreach (AvailablePlugin pluginOn in Global.Plugins.AvailablePlugins)
            {
                TreeNode newNode = new TreeNode(pluginOn.Instance.pName);
                listalg.treeView1.Nodes.Add(newNode);
                newNode = null;
            }

            for (int i = 0; i < listalg.treeView1.Nodes.Count; i++)
            {
                if (plug_name == listalg.treeView1.Nodes[i].ToString())
                {
                    listalg.treeView1.SelectedNode = listalg.treeView1.Nodes[i];
                    break;
                }
            }

            listalg.ShowDialog();

            if (listalg.DialogResult == DialogResult.OK)
            {
                AvailablePlugin selectedPlugin = Global.Plugins.AvailablePlugins.Find(listalg.treeView1.SelectedNode.Text);
                if (selectedPlugin != null)
                {
                    plug_name = selectedPlugin.Instance.pName;
                    plug_path = selectedPlugin.AssemblyPath;
                    this.Text = Application.ProductName + " - " + plug_name;
                    for (int i = 0; i < plug_path.Length; i++)
                        if (plug_path[i] == '\\')
                            assembly = plug_path.Remove(0, i + 1);

                    assembly = assembly.Remove(assembly.Length - 4, 4);

                    int stop = 0;
                    try
                    {
                        StreamReader freader = File.OpenText(Application.StartupPath + @"/plugins/" + assembly + @"/" + assembly + ".txt");
                        this.codeBox1.richTextBox1.Text = freader.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        stop = 1;
                        MessageBox.Show("error opening txt file!\r\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (stop != 1)
                    {
                        this.Text = Application.ProductName + " - " + plug_name;
                        this.optionsToolStripMenuItem.Enabled = true;
                        this.startCompilingToolStripMenuItem.Enabled = true;
                        this.codeBox1.richTextBox1.ReadOnly = false;
                        this.codeBox1.richTextBox1.BackColor = SystemColors.Window;

                        saved = 1;
                    }
                }
            }
            else
            {
                Global.Plugins.ClosePlugins();
            }
        }

        private void startCompilingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBoxCode != null)
            {
                CompilerResults results;
                Cursor = Cursors.WaitCursor;
                this.statusStrip1.Text = "Компилирую...";

                //string plug_name_temp = FindUntilForward("string control_name", textBoxCode, '"', '"');
                //string plug_name_temp_ass = RegEx.MakeReg(plug_name_temp, "[a-zA-Z0-9]");
                //string temp_plug = RegEx.MakeReg(plug_name, "[a-zA-Z0-9]");

                Assembly _assembly = Assembly.GetExecutingAssembly();
                StreamReader _textStreamReader;
                _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("plugEditor.defaultcode.defaultstart.txt"));
                if (_textStreamReader.Peek() != -1)
                    codeStart = _textStreamReader.ReadToEnd();

                codeStart = codeStart.Replace("assembly_name", assembly);
                codeStart = codeStart.Replace("plug_name", plug_name);
                codeStart = codeStart.Replace("plug_descr", plug_descr);

                codeStart = codeStart.Replace("UpdatePlugin", FindUntil("eventPluginEventHandler", textBoxCode.Replace(" ", ""), ';'));
                codeStart = codeStart.Replace("dot_name", FindUntil("List<CPoint>", textBoxCode.Replace(" ", ""), '='));
                codeStart = codeStart.Replace("line_name", FindUntil("List<CLine>", textBoxCode.Replace(" ", ""), '='));

                plug_name = FindUntilForward("string control_name", textBoxCode, '"', '"');
                plug_descr = FindUntilForward("string control_descr", textBoxCode, '"', '"');

                if (!isFind("control_MouseClick", textBoxCode))
                    codeStart += "public void control_MouseClick(object sender, MouseEventArgs e){ }";
                if (!isFind("control_MouseMove", textBoxCode))
                    codeStart += "public void control_MouseMove(object sender, MouseEventArgs e){ }";

                this.Text = Application.ProductName + " - " + plug_name;

                int sum = 0;
                string what = "\r\n";
                string temp;
                for (int i = 0; i < codeStart.Length - what.Length + 1; i++)
                {
                    temp = codeStart;
                    temp = temp.Remove(0, i);
                    temp = temp.Remove(what.Length, codeStart.Length - what.Length - i);
                    if (temp == what)
                        sum++;
                }

                listView1.Items.Clear();
                results = CompileCode.CompileScript(codeStart + textBoxCode + codeEnd, assembly, CompileCode.Languages.CSharp);

                if (results.Errors.Count > 0)
                {
                    ListViewItem l;
                    string err_line;
                    foreach (CompilerError err in results.Errors)
                    {
                        if (err.Line - sum < 0)
                            err_line = "";
                        else
                            err_line = Convert.ToString(err.Line - sum);
                        l = new ListViewItem(err.ErrorText);
                        l.SubItems.Add(err_line);
                        listView1.Items.Add(l);
                    }

                    if (saved == 0 && Directory.Exists(Application.StartupPath + @"/plugins/" + assembly))
                        Directory.Delete(Application.StartupPath + @"/plugins/" + assembly);

                    MessageBox.Show("Compile failed with " + results.Errors.Count.ToString() + " errors.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    StreamWriter MyStream = null;
                    string path_code = Application.StartupPath + @"\plugins\" + assembly + @"\" + assembly + ".txt";

                    int stop = 0;
                    string error = "";

                    try
                    {
                        MyStream = File.CreateText(path_code);
                        MyStream.Write(textBoxCode);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex);
                        error = ex.Message;
                        stop = 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        error = ex.Message;
                        stop = 1;
                    }
                    finally
                    {
                        if (MyStream != null)
                            MyStream.Close();
                        else
                            stop = 1;
                    }

                    if (stop == 1)
                        MessageBox.Show("ошибка:\n" + error,
                            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("Скомпилировано успешно!",
                            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //if (Directory.Exists(Application.StartupPath + @"/plugins/" + temp_plug) && temp_plug != plug_name_temp_ass)
                    //{
                    //    Directory.Delete(Application.StartupPath + @"/plugins/" + temp_plug);
                    //    assembly = plug_name_temp_ass;
                    //}

                    saved = 1;
                }

                Cursor = Cursors.Default;
                this.statusStrip1.Text = "Compiled";
            }
            else
            {
                MessageBox.Show("no code was compiled!",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            int l;
            int i, pos;

            if (listView1.SelectedItems[0].SubItems[1].Text == "")
                l = 0;
            else
                l = Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text);

            if (l != 0)
            {
                i = 1;
                pos = 0;
                while (i < l)
                {
                    pos = this.codeBox1.richTextBox1.Text.IndexOf(Environment.NewLine, pos + 1);
                    i++;
                }
                try
                {
                    this.codeBox1.richTextBox1.SelectionStart = pos;
                    this.codeBox1.richTextBox1.SelectionLength = this.codeBox1.richTextBox1.Text.IndexOf(Environment.NewLine, pos + 1) - pos;
                }
                catch
                {
                    MessageBox.Show("can't read line");
                }
            }

            this.codeBox1.richTextBox1.Focus();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Info inf = new Info();

            inf.general1.textBox1.Text = FindUntilForward("string control_name", textBoxCode, '"', '"');
            inf.general1.textBox2.Text = FindUntilForward("string control_descr", textBoxCode, '"', '"');
            inf.general1.textBox3.Text = assembly;

            inf.ShowDialog();

            if (inf.DialogResult == DialogResult.OK)
            {
                //codeStart = codeStart.Replace(assembly, inf.general1.textBox3.Text);
                textBoxCode = textBoxCode.Replace(assembly, inf.general1.textBox3.Text);

                plug_name = inf.general1.textBox1.Text;
                plug_descr = inf.general1.textBox2.Text;
                assembly = inf.general1.textBox3.Text;

                string cont_descr = FindUntilForwardInsert("string control_descr", plug_descr, textBoxCode, '"', '"');
                if (cont_descr != null)
                    textBoxCode = cont_descr;
                else
                    textBoxCode = textBoxCode.Insert(0, "string control_descr = \"" + plug_descr + "\";\r\n");

                string cont_name = FindUntilForwardInsert("string control_name", plug_name, textBoxCode, '"', '"');
                if (cont_name != null)
                    textBoxCode = cont_name;
                else
                    textBoxCode = textBoxCode.Insert(0, "string control_name = \"" + plug_name + "\";\r\n\r\n");

                this.Text = Application.ProductName + " - " + plug_name;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (textBoxCode.Length > 0)
            //{
            //    DialogResult dr = ErrorBox.Show("Are you sure?", Application.ProductName, ErrorBoxButtons.YesNo, ErrorBoxIcon.Question);
            //    if (dr == DialogResult.Yes)
            //    {
            //        this.Close();
            //    }
            //}
            //else
            //{
                this.Close();
            //}
        }

        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Utility.OpenFindDialog(this.textBox1.Controls);
        }

        private void findAgainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Utility.FindNext(Controls);
        }

        private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (textBoxCode.Length > 0)
            //{
            //    DialogResult dr = ErrorBox.Show("Are you sure?", Application.ProductName, ErrorBoxButtons.YesNo, ErrorBoxIcon.Question);
            //    if (dr == DialogResult.No)
            //    {
            //        e.Cancel = true;
            //    }
            //}
        }

        #endregion

        #region Find / Replace / Until

        private string FindReplace(string what, string to, string where)
        {
            string temp;
            for (int i = 0; i < where.Length - what.Length + 1; i++)
            {
                temp = where;
                temp = temp.Remove(0, i);
                temp = temp.Remove(what.Length, where.Length - what.Length - i);
                if (temp == what)
                {
                    where = where.Remove(i, what.Length);
                    where = where.Insert(i, to);
                }
            }

            return where;
        }

        private bool isFind(string what, string where)
        {
            string temp;
            bool result = false;
            for (int i = 0; i < where.Length - what.Length + 1; i++)
            {
                temp = where;
                temp = temp.Remove(0, i);
                temp = temp.Remove(what.Length, where.Length - what.Length - i);
                if (temp == what)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private string FindUntil(string what, string temp_trim, char until)
        {
            string temp;
            string result = null;
            for (int i = 0; i < temp_trim.Length - what.Length + 1; i++)
            {
                temp = temp_trim;
                temp = temp.Remove(0, i);
                temp = temp.Remove(what.Length, temp_trim.Length - what.Length - i);
                if (temp == what)
                {
                    int j = 0;
                    int ok = 0;
                    for (int k = i + what.Length; k < temp_trim.Length; k++)
                    {
                        if (temp_trim[k] == until)
                        {
                            temp_trim = temp_trim.Remove(0, i + what.Length);
                            temp_trim = temp_trim.Remove(j, temp_trim.Length - j);
                            result = temp_trim;
                            ok = 1;
                            break;
                        }
                        j++;
                    }
                    if (ok == 1)
                        break;
                }
            }

            return result;
        }

        private string FindUntilForward(string what, string content, char until, char forward)
        {
            string temp;
            string result = null;
            for (int i = 0; i < content.Length - what.Length + 1; i++)
            {
                temp = content;
                temp = temp.Remove(0, i);
                temp = temp.Remove(what.Length, content.Length - what.Length - i);
                if (temp == what)
                {
                    int ok = 0;
                    for (int k = i + what.Length; k < content.Length; k++)
                    {
                        if (content[k] == until)
                        {
                            for (int m = k + 1; m < content.Length; m++)
                            {
                                if (content[m] == forward)
                                {
                                    content = content.Remove(0, k + 1);
                                    content = content.Remove(m - k - 1, content.Length - m + k + 1);
                                    result = content;
                                    ok = 1;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    if (ok == 1)
                        break;
                }
            }

            return result;
        }

        private string FindUntilForwardInsert(string what, string insert, string content, char until, char forward)
        {
            string temp;
            string result = null;
            for (int i = 0; i < content.Length - what.Length + 1; i++)
            {
                temp = content;
                temp = temp.Remove(0, i);
                temp = temp.Remove(what.Length, content.Length - what.Length - i);
                if (temp == what)
                {
                    int j = 0;
                    int ok = 0;
                    for (int k = i + what.Length; k < content.Length; k++)
                    {
                        if (content[k] == until)
                        {
                            for (int m = k + 1; m < content.Length; m++)
                            {
                                if (content[m] == forward)
                                {
                                    content = content.Remove(k + 1, m - k - 1);
                                    content = content.Insert(k + 1, insert);
                                    result = content;
                                    ok = 1;
                                    break;
                                }
                            }
                            break;
                        }
                        j++;
                    }
                    if (ok == 1)
                        break;
                }
            }

            return result;
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
            Application.Run(new EditorForm());
        }
    }

    #endregion
}