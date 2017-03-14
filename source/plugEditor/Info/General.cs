using System;
using System.Windows.Forms;
using conf;

namespace plugEditor
{
    public partial class General : UserControl
    {
        #region General

        public General()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBox3.Text = RegEx.MakeReg(this.textBox3.Text, "[a-zA-Z0-9]");
        }

        #endregion
    }
}
