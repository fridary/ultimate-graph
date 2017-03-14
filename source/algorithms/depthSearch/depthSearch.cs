using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using control;
using pluginfo;

namespace depthSearch
{
    public class depthSearch : UserControl, IPlugin
    {
        #region InitializeComponent()

        private Button button1;
        private Timer timer1;
        private IContainer components;
        private Button button2;
        private Button button3;
        private Label label2;
        private Label label3;
        private Label label1;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(38, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 84);
            this.label1.TabIndex = 1;
            this.label1.Text = "начальная вершина: -";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(38, 64);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(38, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Choose";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 196);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 55);
            this.label2.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 183);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Стек:";
            // 
            // depthSearch
            // 
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "depthSearch";
            this.Size = new System.Drawing.Size(141, 251);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Constructor

        event PluginEventHandler UpdatePlugin; // событие, благодаря которому обнавляется плагин

        List<CPoint> dot = new List<CPoint>(); // список всех вершин
        List<CLine> line = new List<CLine>(); // список всех ребер

        string control_name = "Поиск в глубину"; // название плагина
        string control_descr = "Один из методов обхода графа"; // описание плагина
        string control_stripStatusText = "Готово";
        decimal control_timerInterval = 1800;
        bool control_CancelNewEdge = false;
        bool control_CancelNewDot = false;
        int control_radius;
        Cursor control_cursor = Cursors.Default;

        public event PluginEventHandler plugChange
        {
            add { UpdatePlugin += value; }
            remove { UpdatePlugin -= value; }
        }

        public List<CPoint> pDot
        {
            set { dot = value; }
        }

        public List<CLine> pLine
        {
            set { line = value; }
        }

        public string pName
        {
            get { return control_name; }
        }

        public string pDescription
        {
            get { return control_descr; }
        }

        public string stripStatusText
        {
            get { return control_stripStatusText; }
        }

        public decimal pTimerInterval
        {
            set { control_timerInterval = value; }
        }

        public decimal pTimerIntervalDefault
        {
            get { return control_timerInterval; }
        }

        public UserControl MainInterface
        {
            get { return this; }
        }

        public bool CancelNewEdge
        {
            get { return control_CancelNewEdge; }
        }

        public bool CancelNewDot
        {
            get { return control_CancelNewDot; }
        }

        public int Radius
        {
            set { control_radius = value; }
        }

        public Cursor pCursor
        {
            get { return control_cursor; }
        }

        public int control_EdgeSize(int x1, int y1, int x2, int y2)
        {
            int a = Math.Abs(x1 - x1);
            int b = Math.Abs(y1 - y2);
            int c = Convert.ToInt32(Math.Sqrt(a * a + b * b) / 10);
            return c;
        }

        #endregion

        List<int> stack = new List<int>(); // стек
        List<int> stacklines = new List<int>(); // цепочка ребер (из стека), по которым будем идти

        List<int> falseverts = new List<int>(); // список вершин, по которым мы не сможем идтм
        List<int> falselines = new List<int>(); // список ребер из вершин списка falseverts

        Color realVert = Color.GreenYellow;
        Color nextVert = Color.Orchid;
        Color nextLine = Color.Blue;
        Color falseVert = Color.Red;
        Color falseLine = Color.Red;

        Color defaultHighlight;

        // переменные для построениея алгоритма в timer1
        int i, p;
        int vertex = -1;
        int start_pressed;

        int[] flag = new int[1];
        // flag[0] - флаг, определяющий когда выбираем начальную вершину

        public depthSearch()
        {
            // функция для вывода плагина на экран
            InitializeComponent();
        }

        public void control_Initialize()
        {
            timer1.Interval = Convert.ToInt32(this.control_timerInterval); // обнавление интервала timer'a
            Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (stack.Count == 0)
                {
                    for (int a = 0; a < falseverts.Count; a++)
                        dot[falseverts[a]].highlight = false;
                    for (int b = 0; b < falselines.Count; b++)
                        line[falselines[b]].color = Color.Black;

                    dot[vertex].highlight = true;
                    dot[vertex].highlight_color = defaultHighlight;
                    falseverts.Clear();
                    falselines.Clear();

                    this.label1.Text = "алгоритм закончен, так как стек пуст";
                    this.label2.Text = null;
                    control_stripStatusText = "Готово";
                    timer1.Stop(); // останавливаем таймер и алгоритм завершен
                }
                else
                {
                    if (p == 0)
                    {
                        this.label1.Text = "добавляем начальную вершину " + dot[vertex].vertex + " в стек";
                        this.label2.Text = dot[vertex].vertex.ToString();
                        p++;
                    }
                    else if (i == 0)
                    {
                        this.label1.Text = "ищем любую незачеркнутую вершину от вершины " + dot[stack[stack.Count - 1]].vertex;

                        dot[stack[stack.Count - 1]].highlight = true;
                        dot[stack[stack.Count - 1]].highlight_color = realVert;
                        if (stack.Count > 1)
                            if (stack[stack.Count - 2] != vertex)
                                dot[stack[stack.Count - 2]].highlight = false;
                        if (stacklines.Count > 0)
                            line[stacklines[stacklines.Count - 1]].color = Color.Black;

                        i++;
                    }
                    else if (i == 1)
                    {
                        int howmanyadd = 0;
                        for (int a = 0; a < line.Count; a++)
                        {
                            if (dot[stack[stack.Count - 1]].mx == line[a].lx_1 && dot[stack[stack.Count - 1]].my == line[a].ly_1)
                            {
                                for (int b = 0; b < dot.Count; b++)
                                {
                                    if (dot[b].mx == line[a].lx_2 && dot[b].my == line[a].ly_2)
                                    {
                                        if (dot[b].cross == false)
                                        {
                                            stack.Add(b);
                                            stacklines.Add(a);
                                            howmanyadd++;
                                        }
                                        else
                                        {
                                            falseverts.Add(b);
                                            falselines.Add(a);
                                        }
                                        //break;
                                    }
                                }
                                if (howmanyadd > 0)
                                    break;
                            }
                            else if (dot[stack[stack.Count - 1]].mx == line[a].lx_2 && dot[stack[stack.Count - 1]].my == line[a].ly_2)
                            {
                                for (int b = 0; b < dot.Count; b++)
                                {
                                    if (dot[b].mx == line[a].lx_1 && dot[b].my == line[a].ly_1)
                                    {
                                        if (dot[b].cross == false)
                                        {
                                            stack.Add(b);
                                            stacklines.Add(a);
                                            howmanyadd++;
                                        }
                                        else
                                        {
                                            falseverts.Add(b);
                                            falselines.Add(a);
                                        }
                                        //break;
                                    }
                                }
                                if (howmanyadd > 0)
                                    break;
                            }
                        }

                        if (howmanyadd == 0)
                        {
                            string f = null;
                            for (int c = 0; c < falseverts.Count; c++)
                                f += dot[falseverts[c]].vertex + ", ";
                            if (f != null)
                                f = f.Remove(f.Length - 2, 2);

                            this.label1.Text = "от вершины " + dot[stack[stack.Count - 1]].vertex + " исходят(ит) только уже рассмотренные вершины(а), а именно " + f;

                            for (int a = 0; a < falseverts.Count; a++)
                            {
                                dot[falseverts[a]].highlight = true;
                                dot[falseverts[a]].highlight_color = falseVert;
                            }

                            for (int b = 0; b < falselines.Count; b++)
                                line[falselines[b]].color = falseLine;

                            if (stack.Count == 1)
                                stack.RemoveAt(0);

                            i = 5;
                        }
                        else
                        {
                            this.label1.Text = "предположим, что это вершина " + dot[stack[stack.Count - 1]].vertex + " и добавляем ее в стек";

                            string s = null;
                            for (int c = 0; c < stack.Count; c++)
                                s += dot[stack[c]].vertex + ", ";
                            if (s != null)
                                s = s.Remove(s.Length - 2, 2);
                            this.label2.Text = s;

                            dot[stack[stack.Count - 1]].highlight = true;
                            dot[stack[stack.Count - 1]].highlight_color = nextVert;
                            line[stacklines[stacklines.Count - 1]].color = nextLine;

                            for (int a = 0; a < falseverts.Count; a++)
                            {
                                dot[falseverts[a]].highlight = true;
                                dot[falseverts[a]].highlight_color = falseVert;
                            }

                            for (int b = 0; b < falselines.Count; b++)
                                line[falselines[b]].color = falseLine;

                            if (falseverts.Count > 0)
                                i++;
                            else
                                i = 3;
                        }
                    }
                    else if (i == 2)
                    {
                        string f = null;
                        for (int c = 0; c < falseverts.Count; c++)
                            f += dot[falseverts[c]].vertex + ", ";
                        if (f != null)
                            f = f.Remove(f.Length - 2, 2);

                        this.label1.Text = "вершины(у) " + f + " мы не можем выбрать, т. к. они зачеркнуты, то есть мы их уже рассматривали";
                        i++;
                    }
                    else if (i == 3)
                    {
                        for (int a = 0; a < falseverts.Count; a++)
                            dot[falseverts[a]].highlight = false;
                        for (int b = 0; b < falselines.Count; b++)
                            line[falselines[b]].color = Color.Black;

                        dot[vertex].highlight = true;
                        dot[vertex].highlight_color = defaultHighlight;
                        falseverts.Clear();
                        falselines.Clear();

                        dot[stack[stack.Count - 1]].highlight = false;
                        line[stacklines[stacklines.Count - 1]].color = Color.Black;

                        this.label1.Text = "следовательно идем по ребру от " + dot[stack[stack.Count - 2]].vertex
                            + " к " + dot[stack[stack.Count - 1]].vertex + " вершине";

                        if (dot[stack[stack.Count - 2]].cross == false)
                            i++;
                        else
                            i = 0;
                    }
                    else if (i == 4)
                    {
                        this.label1.Text = "..а прошлую вершину " + dot[stack[stack.Count - 2]].vertex + " вычеркиваем";
                        dot[stack[stack.Count - 2]].cross = true;

                        i = 0;
                    }
                    else if (i == 5)
                    {
                        for (int a = 0; a < falseverts.Count; a++)
                            dot[falseverts[a]].highlight = false;
                        for (int b = 0; b < falselines.Count; b++)
                            line[falselines[b]].color = Color.Black;

                        falseverts.Clear();
                        falselines.Clear();

                        if (dot[stack[stack.Count - 1]].cross == false)
                        {
                            this.label1.Text = "..поэтому эту вершину зачеркиваем, убираем из стека и возвращаемся назад к вершине " + dot[stack[stack.Count - 2]].vertex;
                            dot[stack[stack.Count - 1]].cross = true;
                        }
                        else
                        {
                            this.label1.Text = "..поэтому эту вершину убираем из стека и возвращаемся назад к вершине " + dot[stack[stack.Count - 2]].vertex;
                        }
                        dot[stack[stack.Count - 1]].highlight = false;
                        dot[stack[stack.Count - 2]].highlight = true;
                        dot[stack[stack.Count - 2]].highlight_color = realVert;
                        stack.RemoveAt(stack.Count - 1);

                        string s = null;
                        for (int c = 0; c < stack.Count; c++)
                            s += dot[stack[c]].vertex + ", ";
                        if (s != null)
                            s = s.Remove(s.Length - 2, 2);
                        this.label2.Text = s;

                        i = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // если есть ошибки, выводим их в MessageBox'е
                timer1.Stop(); // и останавливаем timer1
                MessageBox.Show("error!\r\n" + ex.Message);
            }

            UpdatePlugin(this, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (start_pressed != 1)
                stack.Add(vertex);

            start_pressed = 1;
            this.button3.Enabled = false;
            control_stripStatusText = "Визуализация Алгоритма...";
            timer1.Start(); // запускаем timer1
        }

        private void button2_Click(object sender, EventArgs e)
        {
            control_stripStatusText = "Остановлено";
            timer1.Stop(); // остановка timer1
            UpdatePlugin(this, e); // обнавлям плагин
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (vertex != -1) // если начальная вершина уже задана,
                dot[vertex].highlight = false; // то убираем эту вершину и ищем новую (щелчком мыши в функции control_MouseClick)
            vertex = -1;
            this.label1.Text = "начальная вершина: -";
            control_cursor = Cursors.Cross;
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            flag[0] = 1;
            UpdatePlugin(this, e); // обнавляем плагин
        }

        #region Mouse Events

        public void control_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // если щелчек мыши был левой кнопкой
            {
                if (flag[0] == 1)
                {
                    for (int i = 0; i < dot.Count; i++) // проходимся циклом по всем вершинам в списке dot
                    {
                        if ((e.X - dot[i].mx) * (e.X - dot[i].mx) + (e.Y - dot[i].my) * (e.Y - dot[i].my) <= control_radius / 2 * control_radius / 2)
                        {
                            // если при щелчке мыши попали по вершине,
                            // то ставим flag[0] = 0 и vertex = i (номер вершины в списке, по которой попали)
                            dot[i].highlight = false;
                            this.label1.Text = "начальная вершина: " + (i + 1);
                            this.button1.Enabled = true;
                            this.button2.Enabled = true;
                            control_CancelNewEdge = true;
                            control_CancelNewDot = true;
                            vertex = i;
                            flag[0] = 0;
                            control_cursor = Cursors.Default;
                        }
                    }
                    if (flag[0] == 0)
                    {
                        dot[vertex].highlight = true;
                        defaultHighlight = dot[vertex].highlight_color;
                    }
                }
                UpdatePlugin(this, e); // обнавляем плагин
            }
        }

        public void control_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag[0] == 1) // если ищем начальную вершину
            {
                // пока двигаем мышкой, проходимся по всем вершинам в списке,
                // и если навели курсором на одну из вершин, то закрашиваем ее
                for (int i = 0; i < dot.Count; i++)
                    if ((e.X - dot[i].mx) * (e.X - dot[i].mx) + (e.Y - dot[i].my) * (e.Y - dot[i].my) <= control_radius / 2 * control_radius / 2)
                        dot[i].highlight = true;
                    else
                        dot[i].highlight = false;
            }
            if (control_CancelNewEdge == true)
                control_CancelNewEdge = false;
            if (control_CancelNewDot == true)
                control_CancelNewDot = false;
        }

        #endregion
    }
}