using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace plugEditor
{
    public partial class Info : Form
    {
        #region Info

        public Info()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.general1.textBox1.Text == null)
            {
                MessageBox.Show("Name is empty!");
                this.DialogResult = DialogResult.None;
            }
        }

        #endregion
    }
}