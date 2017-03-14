using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using control;
using pluginfo;

namespace breadthSearch
{
    public class breadthSearch : UserControl, IPlugin
    {
        #region InitializeComponent()

        private Button button1;
        private Timer timer1;
        private IContainer components;
        private Button button2;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button button3;
        private Label label5;
        private Label label1;
        private Button button4;
        private Button button5;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(38, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Старт";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 246);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 25);
            this.label1.TabIndex = 1;
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
            this.button2.Text = "Стоп";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 288);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 32);
            this.label2.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 226);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Очередь:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 267);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Рассмотренные:";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(38, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Выбрать";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(38, 93);
            this.button4.Name = "button4";
            this.button4.Enabled = false;
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 8;
            this.button4.Text = "Очистить";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(38, 122);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 9;
            this.button5.Text = "Log";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(3, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 54);
            this.label5.TabIndex = 7;
            this.label5.Text = "начальная вершина: -";
            // 
            // breadthSearch
            // 
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button5);
            this.Name = "breadthSearch";
            this.Size = new System.Drawing.Size(150, 251);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Constructor

        event PluginEventHandler UpdatePlugin; // событие, благодаря которому обнавляется плагин

        List<CPoint> dot = new List<CPoint>(); // список всех вершин
        List<CLine> line = new List<CLine>(); // список всех ребер

        string control_name = "Поиск в ширину"; // название плагина
        string control_descr = "Обозначения вершин:\nЗеленая - текущая\nСиняя - по ней можно сделать переход\nКрасная - по ней переход делать нельзя"; // описание плагина
        string control_stripStatusText = "Готово";
        decimal control_timerInterval = 180;
        bool control_CancelNewEdge = false;
        bool control_CancelNewDot = false;
        int control_radius;
        Cursor control_cursor = Cursors.Default;

        string log = "";

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

        List<int> wlist = new List<int>(); // очередь
        List<int> past = new List<int>(); // уже рассмотренные вершины

        List<int> nextverts = new List<int>(); // список следующих вершин в очереди
        List<int> nextlines = new List<int>(); // список ребер для следующих вершин в очереди
        List<int> falseverts = new List<int>(); // список вершин, которые не будут добавлены в очереди
        List<int> falselines = new List<int>(); // список ребер из вершин списка falseverts

        Color realVert = Color.GreenYellow;
        Color trueHighlight = Color.CornflowerBlue;
        Color trueLine = Color.CornflowerBlue;
        Color falseHighlight = Color.Red;
        Color falseLine = Color.Red;

        Color defaultHighlight;

        // переменные для построениея алгоритма в timer1
        int i, j;
        int vertex = -1;

        int[] flag = new int[1];
        // flag[0] - флаг, определяющий когда выбираем начальную вершину

        public breadthSearch()
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
                if (i == 0)
                {
                    this.label5.Text = "добавляем начальную вершину " + dot[vertex].vertex + " в очередь";
                    log += "добавляем начальную вершину " + dot[vertex].vertex + " в очередь@";
                    this.label1.Text += dot[vertex].vertex + " ";
                    wlist.Add(vertex);
                    i++;
                }
                else
                {
                    if (j == 0)
                    {
                        this.label5.Text = "смотрим, какие вершины отходят от вершины " + dot[wlist[0]].vertex + " и не входят в очередь";
                        log += "смотрим, какие вершины отходят от вершины " + dot[wlist[0]].vertex + " и не входят в очередь@";
                        if (wlist[0] != vertex)
                        {
                            dot[wlist[0]].highlight = true;
                            dot[wlist[0]].highlight_color = realVert;
                        }
                        j++;
                    }
                    else if (j == 1)
                    {
                        int howmanyadd = 0;
                        for (int a = 0; a < line.Count; a++)
                        {
                            if (dot[wlist[0]].mx == line[a].lx_1 && dot[wlist[0]].my == line[a].ly_1)
                            {
                                for (int b = 0; b < dot.Count; b++)
                                {
                                    if (dot[b].mx == line[a].lx_2 && dot[b].my == line[a].ly_2)
                                    {
                                        if (dot[b].cross == false)
                                        {
                                            int ok = 1;
                                            for (int f = 0; f < wlist.Count; f++)
                                            {
                                                if (wlist[f] == b)
                                                {
                                                    ok = 0;
                                                    break;
                                                }
                                            }

                                            if (ok == 1)
                                            {
                                                nextverts.Add(b);
                                                nextlines.Add(a);
                                                wlist.Add(b);
                                                dot[b].highlight = true;
                                                dot[b].highlight_color = trueHighlight;
                                                howmanyadd++;
                                            }
                                            else
                                            {
                                                falseverts.Add(b);
                                                falselines.Add(a);
                                                dot[b].highlight = true;
                                                dot[b].highlight_color = falseHighlight;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            falseverts.Add(b);
                                            falselines.Add(a);
                                            dot[b].highlight = true;
                                            dot[b].highlight_color = falseHighlight;
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (dot[wlist[0]].mx == line[a].lx_2 && dot[wlist[0]].my == line[a].ly_2)
                            {
                                for (int b = 0; b < dot.Count; b++)
                                {
                                    if (dot[b].mx == line[a].lx_1 && dot[b].my == line[a].ly_1)
                                    {
                                        if (dot[b].cross == false)
                                        {
                                            int ok = 1;
                                            for (int f = 0; f < wlist.Count; f++)
                                            {
                                                if (wlist[f] == b)
                                                {
                                                    ok = 0;
                                                    break;
                                                }
                                            }

                                            if (ok == 1)
                                            {
                                                nextverts.Add(b);
                                                nextlines.Add(a);
                                                wlist.Add(b);
                                                dot[b].highlight = true;
                                                dot[b].highlight_color = trueHighlight;
                                                howmanyadd++;
                                            }
                                            else
                                            {
                                                falseverts.Add(b);
                                                falselines.Add(a);
                                                dot[b].highlight = true;
                                                dot[b].highlight_color = falseHighlight;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            falseverts.Add(b);
                                            falselines.Add(a);
                                            dot[b].highlight = true;
                                            dot[b].highlight_color = falseHighlight;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (howmanyadd == 0)
                        {
                            string f = null;
                            for (int c = 0; c < falseverts.Count; c++)
                                f += dot[falseverts[c]].vertex + ", ";
                            if (f != null)
                                f = f.Remove(f.Length - 2, 2);

                            this.label5.Text = "вершины(а) " + f + ", соединявшие " + dot[wlist[0]].vertex + " вершину, находятся в очереди или уже рассмотрены";
                            log += "вершины(а) " + f + ", соединявшие " + dot[wlist[0]].vertex + " вершину, находятся в очереди или уже рассмотрены@";
                            j = 4;
                        }
                        else
                        {
                            string v = null;
                            for (int c = 0; c < nextverts.Count; c++)
                                v += dot[nextverts[c]].vertex + ", ";
                            v = v.Remove(v.Length - 2, 2);
                            this.label5.Text = "это вершины(а) " + v;
                            log += "это вершины(а) " + v + "@";
                            for (int d = 0; d < nextlines.Count; d++)
                                line[nextlines[d]].color = trueLine;
                            j++;
                        }

                        for (int h = 0; h < falselines.Count; h++)
                            line[falselines[h]].color = falseLine;
                    }
                    else if (j == 2)
                    {
                        this.label5.Text = "следовательно добавляем их в очередь";
                        log += "следовательно добавляем их в очередь@";
                        string v = null;
                        for (int c = 0; c < wlist.Count; c++)
                            v += dot[wlist[c]].vertex + ", ";
                        v = v.Remove(v.Length - 2, 2);
                        this.label1.Text = v;

                        if (falseverts.Count > 0)
                            j = 5;
                        else
                            j++;
                    }
                    else if (j == 3)
                    {
                        if (wlist[0] != vertex)
                            dot[wlist[0]].highlight = false;
                        dot[wlist[0]].sup = null;

                        for (int d = 0; d < nextlines.Count; d++)
                            line[nextlines[d]].color = Color.Black;
                        for (int g = 0; g < nextverts.Count; g++)
                            dot[nextverts[g]].highlight = false;
                        nextverts.Clear();
                        nextlines.Clear();


                        for (int a = 0; a < falseverts.Count; a++)
                            dot[falseverts[a]].highlight = false;
                        for (int b = 0; b < falselines.Count; b++)
                            line[falselines[b]].color = Color.Black;

                        falseverts.Clear();
                        falselines.Clear();
                        dot[vertex].highlight = true;
                        dot[vertex].highlight_color = defaultHighlight;


                        this.label5.Text = "..а прошлую вершину " + dot[wlist[0]].vertex + " помечаем как зачеркнутую и убираем из очереди";
                        log += "..а прошлую вершину " + dot[wlist[0]].vertex + " помечаем как зачеркнутую и убираем из очереди@";
                        dot[wlist[0]].cross = true;
                        past.Add(wlist[0]);
                        wlist.RemoveAt(0);

                        string s = null;
                        for (int c = 0; c < wlist.Count; c++)
                            s += dot[wlist[c]].vertex + ", ";
                        if (s != null)
                            s = s.Remove(s.Length - 2, 2);
                        this.label1.Text = s;

                        string v = null;
                        for (int c = 0; c < past.Count; c++)
                            v += dot[past[c]].vertex + ", ";
                        if (v != null)
                            v = v.Remove(v.Length - 2, 2);
                        this.label2.Text = v;

                        j = 0;
                    }
                    else if (j == 4)
                    {
                        if (wlist[0] != vertex)
                            dot[wlist[0]].highlight = false;

                        this.label5.Text = "..поэтому мы их сразу убираем из очереди и идем дальше";
                        log += "..поэтому мы их сразу убираем из очереди и идем дальше@";
                        dot[wlist[0]].cross = true;
                        past.Add(wlist[0]);
                        wlist.RemoveAt(0);

                        string s = null;
                        for (int c = 0; c < wlist.Count; c++)
                            s += dot[wlist[c]].vertex + ", ";
                        if (s != null)
                            s = s.Remove(s.Length - 2, 2);
                        this.label1.Text = s;

                        string v = null;
                        for (int c = 0; c < past.Count; c++)
                            v += dot[past[c]].vertex + ", ";
                        if (v != null)
                            v = v.Remove(v.Length - 2, 2);
                        this.label2.Text = v;

                        for (int a = 0; a < falseverts.Count; a++)
                            dot[falseverts[a]].highlight = false;
                        for (int b = 0; b < falselines.Count; b++)
                            line[falselines[b]].color = Color.Black;

                        falseverts.Clear();
                        falselines.Clear();
                        dot[vertex].highlight = true;
                        dot[vertex].highlight_color = defaultHighlight;

                        j = 0;
                    }
                    else if (j == 5)
                    {
                        string f = null;
                        for (int c = 0; c < falseverts.Count; c++)
                            f += dot[falseverts[c]].vertex + ", ";
                        if (f != null)
                            f = f.Remove(f.Length - 2, 2);

                        this.label5.Text = "вершины(а) " + f + " находятся либо в очереди, либо уже рассмотрены; поэтому мы их пропускаем";
                        log += "вершины(а) " + f + " находятся либо в очереди, либо уже рассмотрены; поэтому мы их пропускаем@";

                        j = 3;
                    }
                }

                if (wlist.Count == 0) // если очередь пуста
                {
                    this.label5.Text = "очередь пуста, поэтому конец алгоритма";
                    log += "очередь пуста, поэтому конец алгоритма@";
                    control_stripStatusText = "Готово";
                    this.button2.Enabled = false;
                    this.button4.Enabled = true;
                    timer1.Stop(); // останавливаем таймер и алгоритм завершен


                    string path = @"log.txt";
                    if (!File.Exists(path)) 
                    {
                        this.label5.Text = "Ошибка: файла log.txt не существует!";	
                    }

                    using (StreamWriter sw = File.AppendText(path)) 
                    {
                        sw.WriteLine("[Алгоритм поиска в ширину - " + DateTime.Now.ToString("M/d/yyyy hh:mm") + "]");
                        log = log.Replace("@", Environment.NewLine);
                        sw.WriteLine(log);
                        sw.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                // если есть ошибки, выводим их в MessageBox'е
                timer1.Stop(); // и останавливаем timer1
                MessageBox.Show("error!\r\n" + ex.Message); 
            }

            this.UpdatePlugin(this, e); // обнавляем плагин
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            this.button2.Enabled = true;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            control_stripStatusText = "Визуализация Алгоритма...";
            timer1.Start(); // запускаем timer1
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.button2.Enabled = false;
            this.button1.Enabled = true;
            control_stripStatusText = "Остановлено";
            timer1.Stop(); // остановка timer1
            UpdatePlugin(this, e); // обнавлям плагин
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (vertex != -1) // если начальная вершина уже задана,
                dot[vertex].highlight = false; // то убираем эту вершину и ищем новую (щелчком мыши в функции control_MouseClick)
            vertex = -1;
            this.label5.Text = "начальная вершина: -";
            control_cursor = Cursors.Cross;
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            flag[0] = 1;
            UpdatePlugin(this, e); // обнавляем плагин
        }

        private void button4_Click(object sender, EventArgs e)
        {
            wlist.Clear();
            past.Clear();

            nextverts.Clear();
            nextlines.Clear();
            falseverts.Clear();
            falselines.Clear();

            this.label5.Text = "начальная вершина: -";
            this.label1.Text = "";
            this.label2.Text = "";
            this.button3.Enabled = true;
            control_CancelNewEdge = false;
            control_CancelNewDot = false;
            i = 0;
            j = 0;

            for (int b = 0; b < dot.Count; b++)
            {
                dot[b].cross = false;
                dot[b].highlight = false;
                dot[b].highlight_color = defaultHighlight;
            }

            this.UpdatePlugin(this, e);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start(@"log.txt");
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
                            this.label5.Text = "начальная вершина: " + (i + 1);
                            this.button1.Enabled = true;
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