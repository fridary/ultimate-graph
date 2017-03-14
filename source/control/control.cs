using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace control
{
    public delegate void GraphEventHandler(object sender, EventArgs e);
    public delegate void MouseClickEventHandler(object sender, MouseEventArgs e);
    public delegate void MouseMoveEventHandler(object sender, MouseEventArgs e);
    public partial class Graph : UserControl
    {
        #region Fields

        public event GraphEventHandler wControlChange;

        public event MouseClickEventHandler mClickChange;
        public event MouseMoveEventHandler mMoveChange;

        public List<CPoint> dot;
        public List<CLine> line;

        CPoint MyPoint;
        CLine MyLine;

        ArrayList index = new ArrayList();

        int[] flag = new int[2];

        // flag[0] - dots movements (drag and drop)
        // flag[1] - when we want to build the line between dots

        public bool checkbox1 = true;
        public bool checkbox2 = false;
        public bool checkbox3 = false;
        public bool checkbox4 = true;
        public bool checkbox5 = false;

        public decimal timerInterval = 1500;

        bool contextMenuStripEnabled = true;

        // checkbox1 - vertex
        // checkbox2 - arrow
        // checkbox3 - edge
        // checkbox4 - transparence
        // checkbox5 - coordinates

        int eMouseX, eMouseY;
        int lx_temp, ly_temp;
        int lx_mov, ly_mov;
        int line_in_dot;

        public int radiusDifference;
        public int count;

        private int r = 12;
        private int rVertex = 24;

        //-----+ Plugin

        public bool plug;

        public bool CancelNewEdge;
        public bool CancelNewDot;

        //-----+

        #endregion

        #region control

        public Graph()
        {
            InitializeComponent();
            dot = new List<CPoint>();
            line = new List<CLine>();

            radiusDifference = RadiusVertex - Radius;

            if (checkbox1)
                Radius = RadiusVertex;

            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, true);

            DockPadding.Top = 1;
            DockPadding.Bottom = 1;
            DockPadding.Right = 1;
            DockPadding.Left = 1;
        }

        public void GraphChange(object sender, EventArgs e, int what)
        {
            if (what == 1)
            {
                dot.Clear();
                line.Clear();
                count = 0;

                this.wControlChange(this, e);
            }
            else if (what == 2)
            {
                for (int i = 0; i < 10; i++)
                {
                    Random randObj = new Random();
                    int randomik = randObj.Next(10, 300);

                    MyPoint = new CPoint();
                    MyPoint.mx = randomik;
                    MyPoint.my = randomik;
                    dot.Add(MyPoint);
                    Refresh();
                }
            }

            Refresh();
        }

        private void Graph_Paint(object sender, PaintEventArgs e)
        {
            Rectangle borderRc = ClientRectangle;
            borderRc.Width--;
            borderRc.Height--;

            e.Graphics.DrawRectangle(SystemPens.ControlDark, borderRc);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int r;
            if (checkbox1)
                r = RadiusVertex;
            else
                r = Radius;

            for (int i = 0; i < dot.Count; i++)
            {
                if (!checkbox4)
                    dot[i].DrawNonTransparency(e.Graphics, r);
                if (checkbox5)
                    dot[i].DrawCoordinates(e.Graphics, r);
                dot[i].DrawHighLight(e.Graphics, r);
                dot[i].DrawOutline(e.Graphics, r);
                dot[i].DrawSup(e.Graphics, r);
                if (checkbox1)
                    dot[i].DrawDotVertex(e.Graphics, r);
                else
                    dot[i].DrawDot(e.Graphics, r);
                dot[i].DrawCross(e.Graphics, r);
            }

            for (int i = 0; i < line.Count; i++)
            {
                line[i].DrawRoundDot(Radius);
                line[i].DrawLine(e.Graphics);
            }

            if (checkbox2)
                for (int i = 0; i < line.Count; i++)
                    line[i].DrawArrow(e.Graphics);

            if (checkbox3)
                for (int i = 0; i < line.Count; i++)
                    line[i].DrawEdge(e.Graphics);

            if (flag[1] == 1 && CancelNewEdge == false)
                e.Graphics.DrawLine(new Pen(CLine.temp_color, 1),
                    lx_temp - lx_mov, ly_temp + ly_mov, eMouseX, eMouseY);
        }

        private void Graph_MouseClick(object sender, MouseEventArgs e)
        {
            if (plug)
                this.mClickChange(this, e);
            if (e.Button == MouseButtons.Left)
            {
                if (flag[1] == 1)
                {
                    for (int i = 0; i < dot.Count; i++)
                    {
                        if ((e.X - dot[i].mx) * (e.X - dot[i].mx) + (e.Y - dot[i].my) * (e.Y - dot[i].my) <= Radius / 2 * Radius / 2)
                        {
                            int stop = 0;
                            int j;
                            for (j = 0; j < line.Count; j++)
                            {
                                if (line[j].lx_1 == lx_temp && line[j].ly_1 == ly_temp
                                    && line[j].lx_2 == dot[i].mx && line[j].ly_2 == dot[i].my)
                                {
                                    stop = 1;
                                }
                                else if (line[j].lx_1 == dot[i].mx && line[j].ly_1 == dot[i].my
                                        && line[j].lx_2 == lx_temp && line[j].ly_2 == ly_temp)
                                    {
                                        stop = 1;
                                    }
                            }
                            if (dot[i].mx == lx_temp && dot[i].my == ly_temp)
                                stop = 1;
                            if (stop != 1)
                            {
                                int a = Math.Abs(lx_temp - dot[i].mx);
                                int b = Math.Abs(ly_temp - dot[i].my);
                                int c = Convert.ToInt32(Math.Sqrt(a * a + b * b) / 10);
                                MyLine = new CLine();
                                MyLine.lx_1 = lx_temp;
                                MyLine.ly_1 = ly_temp;
                                MyLine.lx_2 = dot[i].mx;
                                MyLine.ly_2 = dot[i].my;
                                MyLine.first = j;
                                MyLine.second = i;
                                MyLine.edge = c;
                                line.Add(MyLine);
                                dot[CPoint.FindDot(lx_temp, ly_temp, dot)].neighbour.Add(CPoint.FindDot(dot[i].mx, dot[i].my, dot));
                                dot[CPoint.FindDot(dot[i].mx, dot[i].my, dot)].neighbour.Add(CPoint.FindDot(lx_temp, ly_temp, dot));
                            }
                            flag[1] = 0;
                            eMouseX = lx_temp;
                            eMouseY = ly_temp;
                        }
                        else
                        {
                            if (i + 1 == dot.Count && flag[1] == 1)
                            {
                                flag[1] = 0;
                                eMouseX = lx_temp;
                                eMouseY = ly_temp;
                            }
                        }
                    }
                    lx_mov = 0;
                    ly_mov = 0;
                }
                else
                {
                    int get_it = 0;
                    for (int i = 0; i < dot.Count; i++)
                    {
                        if ((e.X - dot[i].mx) * (e.X - dot[i].mx) + (e.Y - dot[i].my) * (e.Y - dot[i].my) <= Radius / 2 * Radius / 2)
                        {
                            get_it = 1;
                        }
                    }
                    if (CancelNewEdge == false)
                    {
                        for (int i = 0; i < dot.Count; i++)
                        {
                            if ((e.X - dot[i].mx) * (e.X - dot[i].mx) + (e.Y - dot[i].my) * (e.Y - dot[i].my) <= Radius / 2 * Radius / 2)
                            {
                                get_it = 0;
                                flag[1] = 1;
                                lx_temp = dot[i].mx;
                                ly_temp = dot[i].my;
                                eMouseX = lx_temp;
                                eMouseY = ly_temp;
                            }
                        }
                    }
                    if (flag[1] != 1 && CancelNewDot == false && get_it != 1)
                    {
                        count++;
                        MyPoint = new CPoint();
                        MyPoint.mx = e.X;
                        MyPoint.my = e.Y;
                        MyPoint.vertex = count;
                        dot.Add(MyPoint);
                    }
                }
            }

            Refresh();
        }

        private void Graph_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag[1] == 1)
            {
                eMouseX = e.X;
                eMouseY = e.Y;
                int newr = Radius / 2;
                double p = Math.PI;

                if ((eMouseX - lx_temp) * (eMouseX - lx_temp) + (eMouseY - ly_temp) * (eMouseY - ly_temp) <= Radius / 2 * Radius / 2)
                {
                    eMouseX = lx_temp;
                    eMouseY = ly_temp;
                    lx_mov = 0;
                    ly_mov = 0;
                }
                else
                {
                    if (eMouseY > ly_temp && eMouseX < lx_temp)
                    {
                        double tga = Math.Atan(Convert.ToDouble(eMouseY - ly_temp) / Convert.ToDouble(lx_temp - eMouseX));
                        double sinb = Math.Sin(p / 2 - tga);
                        double cosb = Math.Cos(p / 2 - tga);
                        lx_mov = Convert.ToInt32(newr * sinb);
                        ly_mov = Convert.ToInt32(newr * cosb);
                    }
                    else if (eMouseY > ly_temp && eMouseX > lx_temp)
                    {
                        double tga = Math.Atan(Convert.ToDouble(eMouseY - ly_temp) / Convert.ToDouble(eMouseX - lx_temp));
                        double sinb = Math.Sin(p / 2 - tga);
                        double cosb = Math.Cos(p / 2 - tga);
                        lx_mov = -Convert.ToInt32(newr * sinb);
                        ly_mov = Convert.ToInt32(newr * cosb);
                    }
                    else if (eMouseY < ly_temp && eMouseX > lx_temp)
                    {
                        double tga = Math.Atan(Convert.ToDouble(eMouseY - ly_temp) / Convert.ToDouble(eMouseX - lx_temp));
                        double sinb = Math.Sin(p / 2 - tga);
                        double cosb = Math.Cos(p / 2 - tga);
                        lx_mov = -Convert.ToInt32(newr * sinb);
                        ly_mov = Convert.ToInt32(newr * cosb);
                    }
                    else if (eMouseY < ly_temp && eMouseX < lx_temp)
                    {
                        double tga = Math.Atan(Convert.ToDouble(eMouseY - ly_temp) / Convert.ToDouble(eMouseX - lx_temp));
                        double sinb = Math.Sin(p / 2 - tga);
                        double cosb = Math.Cos(p / 2 - tga);
                        lx_mov = Convert.ToInt32(newr * sinb);
                        ly_mov = -Convert.ToInt32(newr * cosb);
                    }
                }
            }

            if (flag[0] == 1)
            {
                contextMenuStripEnabled = false;
                foreach (int i in index)
                {
                    for (int j = 0; j < line.Count; j++)
                    {
                        if (dot[i].mx == line[j].lx_1 && dot[i].my == line[j].ly_1)
                        {
                            line[j].lx_1 = e.X + dot[i].sx;
                            line[j].ly_1 = e.Y + dot[i].sy;
                        }
                        else if (dot[i].mx == line[j].lx_2 && dot[i].my == line[j].ly_2)
                        {
                            line[j].lx_2 = e.X + dot[i].sx;
                            line[j].ly_2 = e.Y + dot[i].sy;
                        }
                        int a = Math.Abs(line[j].lx_1 - line[j].lx_2);
                        int b = Math.Abs(line[j].ly_1 - line[j].ly_2);
                        int c = Convert.ToInt32(Math.Sqrt(a * a + b * b) / 10);
                        line[j].edge = c;
                    }

                    dot[i].mx = e.X + dot[i].sx;
                    dot[i].my = e.Y + dot[i].sy;
                    if (line_in_dot == 1)
                    {
                        lx_temp = dot[i].mx;
                        ly_temp = dot[i].my;
                    }
                }
            }
            else
            {
                contextMenuStripEnabled = true;
            }

            int enabled = 0;
            for (int i = 0; i < dot.Count; i++)
            {
                if ((e.X - dot[i].mx) * (e.X - dot[i].mx) + (e.Y - dot[i].my) * (e.Y - dot[i].my) <= Radius / 2 * Radius / 2)
                {
                    enabled = 1;
                }
            }

            if (enabled == 1)
                contextMenuStripEnabled = false;

            if (plug)
                this.mMoveChange(this, e);
            Refresh();
        }

        private void Graph_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < dot.Count; i++)
                {
                    if ((e.X - dot[i].mx) * (e.X - dot[i].mx) + (e.Y - dot[i].my) * (e.Y - dot[i].my) <= Radius / 2 * Radius / 2)
                    {
                        flag[0] = 1;
                        dot[i].sx = dot[i].mx - e.X;
                        dot[i].sy = dot[i].my - e.Y;
                        index.Add(i);
                        if (flag[1] == 1 && lx_temp == dot[i].mx && ly_temp == dot[i].my && line_in_dot == 0)
                        {
                            line_in_dot = 1;
                        }
                    }
                }
            }
        }

        private void Graph_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < dot.Count; i++)
                {
                    if ((e.X - dot[i].mx) * (e.X - dot[i].mx) + (e.Y - dot[i].my) * (e.Y - dot[i].my) <= Radius / 2 * Radius / 2)
                    {
                        if (dot[dot.Count - 1].vertex == dot[i].vertex)
                            count--;
                        dot.RemoveAt(i);
                        i--;

                        for (int a = 0; a < dot.Count; a++)
                        {
                            for (int b = 0; b < dot[a].neighbour.Count; b++)
                            {
                                if (dot[a].neighbour[b] == i)
                                {
                                    dot[a].neighbour.RemoveAt(b);
                                    break;
                                }
                            }
                        }
                    }
                }
                for (int j = 0; j < line.Count; j++)
                {
                    if ((e.X - line[j].lx_1) * (e.X - line[j].lx_1) + (e.Y - line[j].ly_1) * (e.Y - line[j].ly_1) <= Radius / 2 * Radius / 2
                        || (e.X - line[j].lx_2) * (e.X - line[j].lx_2) + (e.Y - line[j].ly_2) * (e.Y - line[j].ly_2) <= Radius / 2 * Radius / 2)
                    {
                        line.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        private void Graph_MouseUp(object sender, MouseEventArgs e)
        {
            if (line_in_dot == 1)
                line_in_dot = 0;

            flag[0] = 0;
            index.Clear();

            this.wControlChange(this, e);
        }

        #endregion

        #region contextMenuStrip

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (!contextMenuStripEnabled)
            {
                e.Cancel = true;
            }
            return;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dot.Clear();
            line.Clear();
            count = 0;

            this.wControlChange(this, e);
        }

        #endregion

        #region Props

        [Category("Configurations"), Description("Graph radius.")]
        public int Radius
        {
            get { return r; }
            set { r = value; }
        }

        [Category("Configurations"), Description("Graph radius with vertex.")]
        public int RadiusVertex
        {
            get { return rVertex; }
            set { rVertex = value; }
        }

        #endregion
    }
}
