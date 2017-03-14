using System;
using System.Windows.Forms;
using conf;

namespace plugEditor
{
    public partial class NewPlug : Form
    {
        #region Fields

        public string assembly;

        #endregion

        #region NewPlug

        public NewPlug()
        {
            InitializeComponent();

            string name = "MyPlugin";
            string full_name = "";
            for (int i = 1; i < 100; i++)
            {
                if (!System.IO.Directory.Exists(Application.StartupPath + @"/plugins/" + name + i))
                {
                    full_name = "My Plugin " + i;
                    break;
                }
            }
            this.textBox1.Text = full_name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "")
            {
                //ErrorBox.Show("Name is empty!", Application.ProductName, ErrorBoxButtons.OkCancel, ErrorBoxIcon.Error);
                MessageBox.Show("Name is empty!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
            else
            {
                assembly = RegEx.MakeReg(this.textBox1.Text, "[a-zA-Z0-9]");
                if (assembly == null)
                {
                    //ErrorBox.Show("Invalid name!", Application.ProductName, ErrorBoxButtons.OkCancel, ErrorBoxIcon.Error);
                    MessageBox.Show("Invalid name!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                }
            }
        }

        #endregion
    }
}