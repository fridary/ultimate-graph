using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace plugEditor
{
    public partial class codeBox : UserControl
    {
        public codeBox()
        {
            InitializeComponent();
            //makePadding();
            //numberLabel.Font = new Font(richTextBox1.Font.FontFamily, richTextBox1.Font.Size);

            //InitalizeKeywords();
            //this.richTextBox1.Text = "int i = 0; //This works !";
            //ColorRtb(richTextBox1);
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                richTextBox1.SelectAll();
                //keyHandled = true;
            }

        }

        //private void makePadding()
        //{
        //    if (this.richTextBox1.Lines.Length < 100)
        //        this.splitContainer1.SplitterDistance = 28;
        //    else if (this.richTextBox1.Lines.Length < 1000)
        //        this.splitContainer1.SplitterDistance = 34;
        //    else if (this.richTextBox1.Lines.Length < 10000)
        //        this.splitContainer1.SplitterDistance = 40;
        //}

        //private void richTextBox1_TextChanged(object sender, EventArgs e)
        //{
            //makePadding();
            //updateNumberLabel();
            //ColorVisibleLines(richTextBox1);
        //}

        //private void updateNumberLabel()
        //{
        //    we get index of first visible char and number of first visible line
        //    Point pos = new Point(0, 0);
        //    int firstIndex = richTextBox1.GetCharIndexFromPosition(pos);
        //    int firstLine = richTextBox1.GetLineFromCharIndex(firstIndex);

        //    now we get index of last visible char and number of last visible line
        //    pos.X = ClientRectangle.Width;
        //    pos.Y = ClientRectangle.Height;
        //    int lastIndex = richTextBox1.GetCharIndexFromPosition(pos);
        //    int lastLine = richTextBox1.GetLineFromCharIndex(lastIndex);

        //    this is point position of last visible char, we'll use its Y value for calculating numberLabel size
        //    pos = richTextBox1.GetPositionFromCharIndex(lastIndex);

        //    finally, renumber label
        //    numberLabel.Text = "";
        //    for (int i = firstLine; i <= lastLine + 1; i++)
        //        numberLabel.Text += i + 1 + "\n";
        //}

        //private void richTextBox1_VScroll(object sender, EventArgs e)
        //{
        //    move location of numberLabel for amount of pixels caused by scrollbar
        //    int d = richTextBox1.GetPositionFromCharIndex(0).Y % (richTextBox1.Font.Height + 1);
        //    numberLabel.Location = new Point(0, d);

        //    updateNumberLabel();
        //}

        //private void richTextBox1_Resize(object sender, EventArgs e)
        //{
        //    richTextBox1_VScroll(null, null);
        //}

        //private void richTextBox1_FontChanged(object sender, EventArgs e)
        //{
        //    updateNumberLabel();
        //    richTextBox1_VScroll(null, null);
        //}

        //----------------------+

        /*

        [DllImport("user32.dll")]
        extern static int SendMessage(int hwnd, int message, int wparam, int lparam);
        [DllImport("user32.dll")]
        extern static int LockWindowUpdate(int hwnd);

        KeyWords[] Words = new KeyWords[13];

        public enum EditMessages
        {
            LineIndex = 187,
            LineFromChar = 201,
            GetFirstVisibleLine = 206,
            CharFromPos = 215,
            PosFromChar = 1062
        }

        public struct KeyWords
        {
            public string Word;
            public Color Color;
        }

        public void ColorVisibleLines(RichTextBox richTextBox1)
        {
            int FirstLine = FirstVisibleLine();
            int LastLine = LastVisibleLine();
            int FirstVisibleChar;
            int i = FirstLine;

            if ((FirstLine == 0) & (LastLine == 0))
            {
                // If there is no text in the control, it will run an error 
                // So, if there isn't any text, just exit 
                return; // TODO: might not be correct. Was : Exit Sub 
            }
            else
            {
                while (i < LastLine)
                {
                    FirstVisibleChar = GetCharFromLineIndex(FirstLine);
                    ColorLineNumber(richTextBox1, FirstLine, FirstVisibleChar);
                    FirstLine += 1;
                    i += 1;
                }
            }
        }

        public int FirstVisibleLine()
        {
            return SendMessage(richTextBox1.Handle.ToInt32(), (int)EditMessages.GetFirstVisibleLine, 0, 0);
        }

        public int GetCharFromLineIndex(int LineIndex)
        {
            return SendMessage(richTextBox1.Handle.ToInt32(), (int)EditMessages.LineIndex, LineIndex, 0);
        }

        public int LastVisibleLine()
        {
            int LastLine = FirstVisibleLine() + (richTextBox1.Height / richTextBox1.Font.Height);
            if (LastLine > richTextBox1.Lines.Length | LastLine == 0)
            {
                LastLine = richTextBox1.Lines.Length;
            }
            return LastLine;
        }

        public void ColorRtb(RichTextBox richTextBox1)
        {
            int FirstVisibleChar;
            int i = 0;

            while (i < richTextBox1.Lines.Length)
            {
                FirstVisibleChar = GetCharFromLineIndex(i);
                ColorLineNumber(richTextBox1, i, FirstVisibleChar);
                i += 1;
            }
        }

        public void ColorLineNumber(RichTextBox richTextBox1, int LineIndex, int lStart)
        {
            string Line = richTextBox1.Lines[LineIndex].ToLower();
            int i = 0;
            int Instance;
            int SelectionAt = richTextBox1.SelectionStart;

            // Lock the update 
            LockWindowUpdate(richTextBox1.Handle.ToInt32());

            // Color the line black to remove any previous coloring 
            richTextBox1.SelectionStart = lStart;
            richTextBox1.SelectionLength = Line.Length;
            richTextBox1.SelectionColor = Color.Black;

            // Find any comments 
            Instance = Line.IndexOf("//") + 1;

            // If there are comments, color them 
            if (Instance != 0)
            {
                richTextBox1.SelectionStart = (lStart + Instance - 1);
                richTextBox1.SelectionLength = (Line.Length - Instance + 1);
                richTextBox1.SelectionColor = Color.Green;
            }

            if (Instance == 1)
            {
                // Unlock the update, restore the start and exit 
                richTextBox1.SelectionStart = SelectionAt;
                richTextBox1.SelectionLength = 0;
                LockWindowUpdate(0);
                return; // TODO: might not be correct. Was : Exit Sub 
            }

            // Loop through all the Keywords 
            while (i < Words.Length)
            {

                // See if the word is in the Line 
                //Instance = string.Compare(Line, Words[i].Word); // InStr(Line, Words[i].Word);
                Instance = Line.IndexOf(Words[i].Word) + 1;

                // If the lines contains the word, color it 
                if (Instance != 0)
                {
                    richTextBox1.SelectionStart = (lStart + Instance - 1);
                    richTextBox1.SelectionLength = Words[i].Word.Length;
                    richTextBox1.SelectionColor = Words[i].Color;
                }

                i += 1;
            }

            // Restore the selectionstart 
            richTextBox1.SelectionStart = SelectionAt;
            richTextBox1.SelectionLength = 0;

            // Unlock the update 
            LockWindowUpdate(0);
        }

        public void InitalizeKeywords()
        {
            //KeyWords[] Words = new KeyWords[13];
            Words[0].Word = "if";
            Words[0].Color = Color.Blue;
            Words[1].Word = "else";
            Words[1].Color = Color.Blue;
            Words[2].Word = "int";
            Words[2].Color = Color.Blue;
            Words[3].Word = "string";
            Words[3].Color = Color.Blue;
            Words[4].Word = "public";
            Words[4].Color = Color.Blue;
            Words[5].Word = "private";
            Words[5].Color = Color.Blue;
            Words[6].Word = "void";
            Words[6].Color = Color.Blue;
            Words[7].Word = "long";
            Words[7].Color = Color.Blue;
            Words[8].Word = "partial";
            Words[8].Color = Color.Blue;
            Words[9].Word = "class";
            Words[9].Color = Color.Blue;
            Words[10].Word = "as";
            Words[10].Color = Color.Blue;
            Words[11].Word = "using";
            Words[11].Color = Color.Blue;
            Words[12].Word = "double";
            Words[12].Color = Color.Blue;
        }
         */
    }
}