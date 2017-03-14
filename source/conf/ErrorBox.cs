using System;
using System.Windows.Forms;
using conf.Properties;

namespace conf
{
    #region Enums

    public enum ErrorBoxButtons
    {
        OkCancel,
        YesNoCancel,
        YesNo
    }

    public enum ErrorBoxIcon
    {
        Error,
        Question,
        Warning,
        Information,
        Delete
    }

    #endregion

    public partial class ErrorBox : Form
    {
        #region ErrorBox

        public ErrorBox()
        {
            InitializeComponent();
        }

        public static DialogResult Show(string text, string title, ErrorBoxButtons buttons, ErrorBoxIcon icon)
        {
            ErrorBox errorbox = new ErrorBox();
            errorbox.errorBox(text, title);
            errorbox.SetButtons(buttons);
            errorbox.SetIcon(icon);

            return errorbox.ShowDialog();
        }

        public void errorBox(string text, string title)
        {
            this.Text = title;
            this.label1.Text = text;

            string temp;
            string what = "\r\n";
            for (int i = 0; i < text.Length - what.Length + 1; i++)
            {
                temp = text;
                temp = temp.Remove(0, i);
                temp = temp.Remove(what.Length, text.Length - what.Length - i);
                if (temp == what)
                {
                    this.label1.Location = new System.Drawing.Point(this.label1.Location.X, this.label1.Location.Y - 10);
                    break;
                }
            }
        }

        public void SetButtons(ErrorBoxButtons buttons)
        {
            if (buttons == ErrorBoxButtons.YesNoCancel)
            {
                button1.Visible = true;
                button1.Text = "Yes";
                button2.Visible = true;
                button2.Text = "No";
                button3.Visible = true;
                button3.Text = "Cancel";
            }
            else if (buttons == ErrorBoxButtons.YesNo)
            {
                button1.Visible = false;
                button2.Visible = true;
                button2.Text = "Yes";
                button2.DialogResult = DialogResult.Yes;
                button3.Visible = true;
                button3.Text = "No";
                button3.DialogResult = DialogResult.No;
            }
            else if (buttons == ErrorBoxButtons.OkCancel)
            {
                button1.Visible = false;
                button2.Visible = true;
                button2.Text = "OK";
                button2.DialogResult = DialogResult.OK;
                button3.Visible = true;
                button3.Text = "Cancel";
                button3.DialogResult = DialogResult.Cancel;
            }
        }

        public void SetIcon(ErrorBoxIcon icon)
        {
            if (icon == ErrorBoxIcon.Information)
                pictureBox1.BackgroundImage = Resources.e_information;
            else if (icon == ErrorBoxIcon.Warning)
                pictureBox1.BackgroundImage = Resources.e_warning;
            else if (icon == ErrorBoxIcon.Error)
                pictureBox1.BackgroundImage = Resources.e_error;
            else if (icon == ErrorBoxIcon.Question)
                pictureBox1.BackgroundImage = Resources.e_question;
            else if (icon == ErrorBoxIcon.Delete)
                pictureBox1.BackgroundImage = Resources.e_delete;
        }

        #endregion
    }
}