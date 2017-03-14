﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using control;
using pluginfo;

namespace dijkstra
{
    public class dijkstra : UserControl, IPlugin
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
            this.label1.Location = new System.Drawing.Point(3, 166);
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
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 213);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 32);
            this.label2.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Очередь:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 191);
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
            this.button3.Text = "Choose";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(3, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 54);
            this.label5.TabIndex = 7;
            this.label5.Text = "начальная вершина: -";
            // 
            // dijkstra
            // 
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "dijkstra";
            this.Size = new System.Drawing.Size(150, 251);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Constructor

        event PluginEventHandler UpdatePlugin; // событие, благодаря которому обнавляется плагин

        List<CPoint> dot = new List<CPoint>(); // список всех вершин
        List<CLine> line = new List<CLine>(); // список всех ребер

        string control_name = "Алгоритм Дейкстры"; // название плагина
        string control_descr = "Поиск кратчайшего пути до всех вершин от заданной"; // описание плагина
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

        List<int> wlist = new List<int>(); // очередь
        List<int> past = new List<int>(); // уже рассмотренные вершины

        List<int> nextverts = new List<int>(); // список следующих вершин в очереди
        List<int> nextlines = new List<int>(); // список ребер для следующих вершин в очереди
        List<int> falseverts = new List<int>(); // список вершин, которые не будут добавлены в очереди
        List<int> falselines = new List<int>(); // список ребер из вершин списка falseverts

        List<int> nolines = new List<int>(); // список вершин без ребер
        List<int> vertcon_x = new List<int>(); // список вершин с координатами X, соединенные ребрами и с начальной вершиной
        List<int> vertcon_y = new List<int>(); // список вершин с координатами Y, соединенные ребрами и с начальной вершиной
        List<int> novert_dot = new List<int>(); // список вершин с ребрами, которые не соединены никак с начальной вершиной

        Color realVert = Color.GreenYellow;
        Color trueHighlight = Color.Orchid;
        Color trueLine = Color.Blue;
        Color falseHighlight = Color.Red;
        Color falseLine = Color.Red;

        Color defaultHighlight;

        // переменные для построениея алгоритма в timer1
        int i, j;
        int vertex = -1;
        int addnext;
        int nosup;

        int start_pressed;

        int[] flag = new int[1];
        // flag[0] - флаг, определяющий когда выбираем начальную вершину

        public dijkstra()
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
                this.label5.Text = "отмечаем все вершины знаком бесконечности";

                for (int l = 0; l < dot.Count; l++)
                {
                    dot[l].sup = "infinity";
                    dot[l].sup_color = Color.Red;
                }

                wlist.Add(vertex);
                i++;
            }
            else if (i == 1)
            {
                this.label5.Text = "добавляем начальную вершину " + dot[vertex].vertex + " в очередь и приравниваем ей начальное расстояние 0";
                this.label1.Text += dot[vertex].vertex + " ";

                dot[vertex].sup = "0";
                dot[vertex].sup_color = Color.DarkGreen;

                i++;
            }
            else
            {
                if (j == 0)
                {
                    this.label5.Text = "смотрим, какие вершины отходят от вершины " + dot[wlist[0]].vertex + ", не входят в очередь и по одной их рассматриваем";
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
                    if (addnext != 1)
                    {
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

                                            if (ok == 0)
                                            {
                                                if (Convert.ToInt32(dot[b].sup) > Convert.ToInt32(dot[wlist[0]].sup) + line[a].edge)
                                                {
                                                    nextverts.Add(b);
                                                    nextlines.Add(a);
                                                    //wlist.Add(b);
                                                    howmanyadd++;
                                                }
                                                else
                                                {
                                                    falseverts.Add(b);
                                                    falselines.Add(a);
                                                }
                                            }
                                            else if (ok == 1)
                                            {
                                                nextverts.Add(b);
                                                nextlines.Add(a);
                                                wlist.Add(b);
                                                howmanyadd++;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt32(dot[b].sup) > Convert.ToInt32(dot[wlist[0]].sup) + line[a].edge)
                                            {
                                                nextverts.Add(b);
                                                nextlines.Add(a);
                                                wlist.Add(b);
                                                howmanyadd++;
                                            }
                                            else
                                            {
                                                falseverts.Add(b);
                                                falselines.Add(a);
                                            }
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

                                            if (ok == 0)
                                            {
                                                if (Convert.ToInt32(dot[b].sup) > Convert.ToInt32(dot[wlist[0]].sup) + line[a].edge)
                                                {
                                                    nextverts.Add(b);
                                                    nextlines.Add(a);
                                                    //wlist.Add(b);
                                                    howmanyadd++;
                                                }
                                                else
                                                {
                                                    falseverts.Add(b);
                                                    falselines.Add(a);
                                                }
                                            }
                                            else if (ok == 1)
                                            {
                                                nextverts.Add(b);
                                                nextlines.Add(a);
                                                wlist.Add(b);
                                                howmanyadd++;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt32(dot[b].sup) > Convert.ToInt32(dot[wlist[0]].sup) + line[a].edge)
                                            {
                                                nextverts.Add(b);
                                                nextlines.Add(a);
                                                wlist.Add(b);
                                                howmanyadd++;
                                            }
                                            else
                                            {
                                                falseverts.Add(b);
                                                falselines.Add(a);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (howmanyadd == 0 && addnext != 1)
                    {
                        string f = null;
                        for (int c = 0; c < falseverts.Count; c++)
                            f += dot[falseverts[c]].vertex + ", ";
                        if (f != null)
                            f = f.Remove(f.Length - 2, 2);

                        for (int w = 0; w < falseverts.Count; w++)
                        {
                            dot[falseverts[w]].highlight = true;
                            dot[falseverts[w]].highlight_color = falseHighlight;
                        }
                        for (int y = 0; y < falselines.Count; y++)
                            line[falselines[y]].color = falseLine;

                        this.label5.Text = "вершины(а) " + f + ", соединявшие " + dot[wlist[0]].vertex + " вершину, находятся в очереди или уже рассмотрены";
                        j = 5;
                    }
                    else
                    {
                        int get_it = 0;
                        if (dot[nextverts[0]].sup != "infinity" && dot[nextverts[0]].sup != "?")
                            get_it = 1;

                        addnext = 1;
                        this.label5.Text = "идем к первой попавшейся вершине " + dot[nextverts[0]].vertex + " от вершины " + dot[wlist[0]].vertex;

                        line[nextlines[0]].color = trueLine;
                        dot[nextverts[0]].highlight = true;
                        dot[nextverts[0]].highlight_color = trueHighlight;

                        if (get_it != 1)
                        {
                            dot[nextverts[0]].sup = "?";
                            dot[nextverts[0]].sup_color = Color.DarkCyan;
                        }

                        j++;
                    }

                    //for (int h = 0; h < falselines.Count; h++)
                    //    line[falselines[h]].color = falseLine;
                }
                else if (j == 2)
                {
                    int get_it = 0;
                    for (int f = 0; f < wlist.Count; f++)
                    {
                        if (wlist[f] == nextverts[0])
                        {
                            get_it = 1;
                            break;
                        }
                    }

                    if (get_it == 1)
                    {
                        this.label5.Text = "и складываем путь прошлой вершины " + dot[wlist[0]].sup + " с весом ребра "
                            + line[nextlines[0]].edge + " следующей вершины";
                    }
                    else
                    {
                        this.label5.Text = "добавляем ее в очередь и складываем путь прошлой вершины " + dot[wlist[0]].sup + " с весом ребра "
                            + line[nextlines[0]].edge + " следующей вершины";
                        this.label1.Text += ", " + dot[nextverts[0]].vertex;
                    }

                    //int ok = 0;
                    //for (int f = 0; f < wlist.Count; f++)
                    //{
                    //    if (wlist[f] == nextverts[0])
                    //    {
                    //        ok = 1;
                    //        break;
                    //    }
                    //}

                    //if (ok == 0 && dot[nextverts[0]].cross == true)
                    //    ok = 1;

                    int ok = 0;

                    if (dot[nextverts[0]].sup != "infinity" && dot[nextverts[0]].sup != "?")
                        if (Convert.ToInt32(dot[nextverts[0]].sup) > Convert.ToInt32(dot[wlist[0]].sup) + line[nextlines[0]].edge)
                            ok = 1;

                    if (ok == 1)
                    {
                        j = 6;
                    }
                    else
                    {
                        dot[nextverts[0]].sup = dot[wlist[0]].sup + " + " + line[nextlines[0]].edge.ToString();
                        dot[nextverts[0]].sup_color = Color.DarkGreen;
                        j++;
                    }
                }
                else if (j == 3)
                {
                    line[nextlines[0]].color = Color.Black;
                    dot[nextverts[0]].highlight = false;

                    if (nosup != 1)
                    {
                        dot[nextverts[0]].sup = Convert.ToString(Convert.ToInt32(dot[wlist[0]].sup) + line[nextlines[0]].edge);
                        dot[nextverts[0]].sup_color = Color.DarkGreen;
                    }
                    else
                    {
                        nosup = 0;
                    }

                    this.label5.Text = "следовательно кратчайший путь от начальной вершины " + dot[vertex].vertex + " до вершины "
                        + dot[nextverts[0]].vertex + " будет равно " + dot[nextverts[0]].sup;

                    nextverts.RemoveAt(0);
                    nextlines.RemoveAt(0);

                    if (nextverts.Count == 0)
                    {
                        addnext = 0;
                        j++;
                    }
                    else
                    {
                        j = 1;
                    }
                }
                else if (j == 4)
                {
                    if (wlist[0] != vertex)
                        dot[wlist[0]].highlight = false;
                    //dot[wlist[0]].sup = null;

                    //for (int d = 0; d < nextlines.Count; d++)
                    //    line[nextlines[d]].color = Color.Black;
                    //for (int g = 0; g < nextverts.Count; g++)
                    //    dot[nextverts[g]].highlight = false;
                    //nextverts.Clear();
                    //nextlines.Clear();


                    for (int a = 0; a < falseverts.Count; a++)
                        dot[falseverts[a]].highlight = false;
                    for (int b = 0; b < falselines.Count; b++)
                        line[falselines[b]].color = Color.Black;

                    falseverts.Clear();
                    falselines.Clear();
                    dot[vertex].highlight = true;
                    dot[vertex].highlight_color = defaultHighlight;


                    this.label5.Text = "..а прошлую вершину " + dot[wlist[0]].vertex + " помечаем как зачеркнутую и убираем из очереди";
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
                else if (j == 5)
                {
                    if (wlist[0] != vertex)
                        dot[wlist[0]].highlight = false;

                    this.label5.Text = "..поэтому мы их сразу убираем из очереди и идем дальше";
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
                else if (j == 6)
                {
                    this.label5.Text = "..но так как длина пути " + Convert.ToInt32(dot[nextverts[0]].sup)
                        + " больше " + Convert.ToInt32(dot[wlist[0]].sup) + " + " + line[nextlines[0]].edge;

                    dot[nextverts[0]].sup += " > " + dot[wlist[0]].sup + " + " + line[nextlines[0]].edge;
                    j++;
                }
                else if (j == 7)
                {
                    this.label5.Text = ", то  мы присваиваем тот путь, который меньше, то есть " + dot[wlist[0]].sup
                        + " + " + line[nextlines[0]].edge + " = " + (Convert.ToInt32(dot[wlist[0]].sup) + line[nextlines[0]].edge);

                    dot[nextverts[0]].sup = Convert.ToString((Convert.ToInt32(dot[wlist[0]].sup) + line[nextlines[0]].edge));

                    nosup = 1;
                    j = 3;
                }
            }

            if (wlist.Count == 0) // если очередь пуста
            {
                this.label5.Text = "очередь пуста, все вершины вычеркнуты, конец алгоритма";
                control_stripStatusText = "Готово";
                this.button3.Enabled = true;
                timer1.Stop(); // останавливаем таймер и алгоритм завершен
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
            if (start_pressed != 1)
            {
                nextverts.Clear();
                nextlines.Clear();
                nolines.Clear();
                novert_dot.Clear();

                // добавляем начальную вершину, так как из нее буду выходить все остальные
                nextverts.Add(vertex);

                for (int n = 0; n < dot.Count; n++)
                {
                    // проходим циклом по всем ребрам в списке line
                    for (int m = 0; m < line.Count; m++)
                    {
                        if (dot[n].mx != line[m].lx_1 && dot[n].my != line[m].lx_1)
                        {
                            if (dot[n].mx != line[m].lx_2 && dot[n].my != line[m].lx_2)
                            {
                                if (m == line.Count - 1)
                                {
                                    nolines.Add(n); // добавляем в список вершину, которая не соединена ребром
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // добавление в список vertcon_x и vertcon_y координат вершин, соединенные ребрами с начальной вершиной
                vertcon_x.Add(dot[vertex].mx);
                vertcon_y.Add(dot[vertex].my);
                for (int s = 0; s < line.Count; s++)
                {
                    for (int b = 0; b < vertcon_x.Count; b++)
                    {
                        if (line[s].lx_1 == vertcon_x[b] && line[s].ly_1 == vertcon_y[b])
                        {
                            int get_it = 1;
                            for (int i = 0; i < vertcon_x.Count; i++)
                            {
                                if (vertcon_x[i] == line[s].lx_2)
                                    get_it = 0;
                                if (vertcon_y[i] == line[s].ly_2)
                                    get_it = 0;
                            }
                            if (get_it == 1)
                            {
                                vertcon_x.Add(line[s].lx_2);
                                vertcon_y.Add(line[s].ly_2);
                                break;
                            }
                        }
                        if (line[s].lx_2 == vertcon_x[b] && line[s].ly_2 == vertcon_y[b])
                        {
                            int get_it = 1;
                            for (int i = 0; i < vertcon_x.Count; i++)
                            {
                                if (vertcon_x[i] == line[s].lx_1)
                                    get_it = 0;
                                if (vertcon_y[i] == line[s].ly_1)
                                    get_it = 0;
                            }
                            if (get_it == 1)
                            {
                                vertcon_x.Add(line[s].lx_1);
                                vertcon_y.Add(line[s].ly_1);
                                break;
                            }
                        }
                    }
                }

                for (int d = 0; d < dot.Count; d++)
                {
                    for (int x = 0; x < vertcon_x.Count; x++)
                    {
                        if (dot[d].mx != vertcon_x[x] && dot[d].my != vertcon_y[x])
                        {
                            if (x == vertcon_x.Count - 1)
                            {
                                for (int g = 0; g < nolines.Count; g++)
                                {
                                    if (nolines[g] != d)
                                    {
                                        novert_dot.Add(d); // добавляем в список вершину, ребра которой не соединены с начальной вершиной
                                        dot[d].sup = null; // убираем у данной вершины знак "?"
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // убираем схожие вершины в списках novert_dot и nolines
                for (int w = 0; w < novert_dot.Count; w++)
                    for (int q = 0; q < nolines.Count; q++)
                        if (novert_dot[w] == nolines[q])
                            novert_dot.Remove(novert_dot[w]);

                nextverts.Clear();
            }

            if (nolines.Count != 0 && novert_dot.Count != 0)
            {
                string v = null;
                for (int c = 0; c < nolines.Count; c++)
                    v += dot[nolines[c]].vertex + ", ";
                if (v != null)
                    v = v.Remove(v.Length - 2, 2);

                string n = null;

                for (int c = 0; c < novert_dot.Count; c++)
                    n += dot[novert_dot[c]].vertex + ", ";
                if (n != null)
                    n = n.Remove(n.Length - 2, 2);

                this.label1.Text = "nolines: " + v + "\r\nnoverts: " + n;
            }
            else
            {
                start_pressed = 1;
                this.button3.Enabled = false;
                control_stripStatusText = "Визуализация Алгоритма...";
                timer1.Start(); // запускаем timer1
            }
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
            this.label5.Text = "начальная вершина: -";
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
                            this.label5.Text = "начальная вершина: " + (i + 1);
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

int i = 0;

private void timer1_Tick(object sender, EventArgs e) {
    if (i != dot.Count)
    {
        Random rd = new Random();
        dot[i].highlight = true;
        dot[i].highlight_color = Color.FromArgb(rd.Next(255), rd.Next(255), rd.Next(255));
        this.label1.Text = "зарисовываю " + (i + 1) + " вершину";
        i++;
    }
    else
    {
        control_stripStatusText = "Готово";
        this.label1.Text = "Готово";
        timer1.Stop(); // останавливаем таймер
    }

    UpdatePlugin(this, e); // обновляем плагин
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