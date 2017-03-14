using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace control
{
    #region CPoint

    public class CPoint
    {
        public List<int> neighbour = new List<int>();

        public Color color = Color.Black;
        public Color cross_color = Color.Blue;
        public Color sup_color = Color.DarkGreen;
        public Color highlight_color = Color.Orange;
        public Color cord_color = Color.Gray;
        public Color vertex_color = Color.Black;
        public Color outline = Color.Transparent;

        public string sup;
        public int mx, my, sx, sy, vertex;
        public bool cross, highlight;

        public static int FindDot(int x, int y, List<CPoint> dot)
        {
            //string t = null;
            //for (int s = 0; s < dot.Count; s++)
            //{
            //    t += "dot[" + s + "]; count: " + dot[s].neighbour.Count + "; vertex: " + dot[s].vertex + "\r\n";
            //    t += "neightbourhood: ";
            //    for (int g = 0; g < dot[s].neighbour.Count; g++)
            //        t += dot[s].neighbour[g] + ", ";
            //    t += "\r\n\r\n";
            //}
            //MessageBox.Show(t);

            int result = -1;
            for (int i = 0; i < dot.Count; i++)
            {
                if (x == dot[i].mx && y == dot[i].my)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        internal void DrawDot(Graphics gR, int r)
        {
            gR.DrawEllipse(new Pen(color), mx - r / 2, my - r / 2, r, r);
        }

        internal void DrawNonTransparency(Graphics gR, int r)
        {
            gR.FillEllipse(new SolidBrush(Color.White), mx - r / 2, my - r / 2, r, r);
        }

        internal void DrawCoordinates(Graphics gR, int r)
        {
            int rad, cord_long;
            if (r > 15)
            {
                rad = r / 3;
                cord_long = r / 5;
            }
            else
            {
                rad = r - 4;
                cord_long = r / 4;
            }
            gR.DrawString("(" + mx + "; " + my + ")", new Font("Arial", rad), new SolidBrush(cord_color),
                mx + r / 2 + cord_long, my - r / 2 + cord_long + 10);
        }

        internal void DrawCross(Graphics gR, int r)
        {
            if (cross)
            {
                int line_long = 4;
                using (Pen pen = new Pen(cross_color, 2))
                {
                    gR.DrawLine(pen, mx + r / 2 + line_long, my - r / 2 - line_long,
                        mx - r / 2 - line_long, my + r / 2 + line_long);
                    gR.DrawLine(pen, mx - r / 2 - line_long, my - r / 2 - line_long,
                        mx + r / 2 + line_long, my + r / 2 + line_long);
                }
            }
        }

        internal void DrawHighLight(Graphics gR, int r)
        {
            if (highlight)
            {
                gR.FillEllipse(new SolidBrush(highlight_color), mx - r / 2, my - r / 2, r, r);
            }
        }

        internal void DrawOutline(Graphics gR, int r)
        {
            int size = 5;
            gR.DrawEllipse(new Pen(outline, 2), mx - r / 2 - size, my - r / 2 - size, r + 2, r + 2);
        }

        internal void DrawSup(Graphics gR, int r)
        {
            if (sup != null)
            {
                int rad, sup_long;
                if (r > 15)
                {
                    rad = r / 2;
                    sup_long = r / 5;
                }
                else
                {
                    rad = r;
                    sup_long = r / 5;
                }
                if (sup == "infinity")
                {
                    StringFormat myFormat = new StringFormat(StringFormatFlags.DirectionVertical);
                    gR.DrawString("8", new Font("Arial", rad + 3), new SolidBrush(sup_color),
                        mx + r / 2 + sup_long, my - r / 2 - sup_long - 4, myFormat);
                }
                else
                {
                    gR.DrawString(sup, new Font("Arial", rad), new SolidBrush(sup_color),
                        mx + r / 2 + sup_long, my - r / 2 - sup_long - 4);
                }
            }
        }

        internal void DrawDotVertex(Graphics gR, int r)
        {
            Color v_color;
            int minus = 0;
            int size = r / 2;

            if (vertex_color == Color.Black)
                v_color = color;
            else
                v_color = vertex_color;

            gR.DrawEllipse(new Pen(color), mx - r / 2, my - r / 2, r, r);
            if (vertex > 9)
                minus = r / 5;
            gR.DrawString(Convert.ToString(vertex),
                new Font("Arial", size), new SolidBrush(v_color), mx - size / 2 - minus - 1, my - r / 3 - 1);
        }
    }

    #endregion

    #region CLine

    public class CLine
    {
        public static Color temp_color = Color.Black;
        public Color color = Color.Black;
        public int width = 1;
        public int first, second, lx_1, ly_1, lx_2, ly_2, edge;

        int newr;
        int minusX_1 = 0;
        int minusY_1 = 0;
        int minusX_2 = 0;
        int minusY_2 = 0;
        double p = Math.PI;

        internal void DrawRoundDot(int r)
        {
            newr = r / 2;

            #region Drawing Round Dot

            if (lx_1 == lx_2)
            {
                if (ly_1 > ly_2)
                {
                    minusY_1 = -newr;
                    minusY_2 = newr;
                }
                else
                {
                    minusY_1 = newr;
                    minusY_2 = -newr;
                }
            }
            else if (ly_1 == ly_2)
            {
                if (lx_1 > lx_2)
                {
                    minusX_1 = newr;
                    minusX_2 = -newr;
                }
                else
                {
                    minusX_1 = -newr;
                    minusX_2 = newr;
                }
            }
            else
            {
                if (ly_1 > ly_2 && lx_1 < lx_2)
                {
                    double tga = Math.Atan(Convert.ToDouble(ly_1 - ly_2) / Convert.ToDouble(lx_2 - lx_1));
                    double sinb = Math.Sin(p / 2 - tga);
                    double cosb = Math.Cos(p / 2 - tga);
                    minusX_1 = -Convert.ToInt32(newr * sinb);
                    minusY_1 = -Convert.ToInt32(newr * cosb);
                    minusX_2 = Convert.ToInt32(newr * sinb);
                    minusY_2 = Convert.ToInt32(newr * cosb);
                }
                else if (ly_1 > ly_2 && lx_1 > lx_2)
                {
                    double tga = Math.Atan(Convert.ToDouble(ly_1 - ly_2) / Convert.ToDouble(lx_1 - lx_2));
                    double sinb = Math.Sin(p / 2 - tga);
                    double cosb = Math.Cos(p / 2 - tga);
                    minusX_1 = Convert.ToInt32(newr * sinb);
                    minusY_1 = -Convert.ToInt32(newr * cosb);
                    minusX_2 = -Convert.ToInt32(newr * sinb);
                    minusY_2 = Convert.ToInt32(newr * cosb);
                }
                else if (ly_1 < ly_2 && lx_1 > lx_2)
                {
                    double tga = Math.Atan(Convert.ToDouble(ly_1 - ly_2) / Convert.ToDouble(lx_1 - lx_2));
                    double sinb = Math.Sin(p / 2 - tga);
                    double cosb = Math.Cos(p / 2 - tga);
                    minusX_1 = Convert.ToInt32(newr * sinb);
                    minusY_1 = -Convert.ToInt32(newr * cosb);
                    minusX_2 = -Convert.ToInt32(newr * sinb);
                    minusY_2 = Convert.ToInt32(newr * cosb);
                }
                else if (ly_1 < ly_2 && lx_1 < lx_2)
                {
                    double tga = Math.Atan(Convert.ToDouble(ly_1 - ly_2) / Convert.ToDouble(lx_1 - lx_2));
                    double sinb = Math.Sin(p / 2 - tga);
                    double cosb = Math.Cos(p / 2 - tga);
                    minusX_1 = -Convert.ToInt32(newr * sinb);
                    minusY_1 = Convert.ToInt32(newr * cosb);
                    minusX_2 = Convert.ToInt32(newr * sinb);
                    minusY_2 = -Convert.ToInt32(newr * cosb);
                }
            }

            #endregion
        }

        internal void DrawLine(Graphics gR)
        {
            gR.DrawLine(new Pen(color, width), lx_1 - minusX_1, ly_1 + minusY_1,
                    lx_2 - minusX_2, ly_2 + minusY_2);
        }

        internal void DrawArrow(Graphics gR)
        {
            int angle = 15;
            int a_long = 15;

            int arrowX_1 = 0;
            int arrowY_1 = 0;
            int arrowX_2 = 0;
            int arrowY_2 = 0;

            #region Drawing Arrow

            if (lx_1 != lx_2 || ly_1 != ly_2)
            {
                if (ly_1 >= ly_2 && lx_1 <= lx_2)
                {
                    double tga = Math.Atan(Convert.ToDouble(ly_1 - ly_2) / Convert.ToDouble(lx_2 - lx_1));
                    double sinb1 = Math.Sin(p / 2 - tga + (p * angle) / 180);
                    double cosb1 = Math.Cos(p / 2 - tga + (p * angle) / 180);
                    double sinb2 = Math.Sin(p / 2 - tga - (p * angle) / 180);
                    double cosb2 = Math.Cos(p / 2 - tga - (p * angle) / 180);
                    arrowX_1 = Convert.ToInt32((newr + a_long) * sinb1);
                    arrowY_1 = Convert.ToInt32((newr + a_long) * cosb1);
                    arrowX_2 = Convert.ToInt32((newr + a_long) * sinb2);
                    arrowY_2 = Convert.ToInt32((newr + a_long) * cosb2);
                }
                else if (ly_1 >= ly_2 && lx_1 >= lx_2)
                {
                    double tga = Math.Atan(Convert.ToDouble(ly_1 - ly_2) / Convert.ToDouble(lx_1 - lx_2));
                    double sinb1 = Math.Sin(p / 2 - tga - (p * angle) / 180);
                    double cosb1 = Math.Cos(p / 2 - tga - (p * angle) / 180);
                    double sinb2 = Math.Sin(p / 2 - tga + (p * angle) / 180);
                    double cosb2 = Math.Cos(p / 2 - tga + (p * angle) / 180);
                    arrowX_1 = -Convert.ToInt32((newr + a_long) * sinb1);
                    arrowY_1 = Convert.ToInt32((newr + a_long) * cosb1);
                    arrowX_2 = -Convert.ToInt32((newr + a_long) * sinb2);
                    arrowY_2 = Convert.ToInt32((newr + a_long) * cosb2);
                }
                else if (ly_1 <= ly_2 && lx_1 >= lx_2)
                {
                    double tga = Math.Atan(Convert.ToDouble(ly_1 - ly_2) / Convert.ToDouble(lx_1 - lx_2));
                    double sinb1 = Math.Sin(p / 2 - tga - (p * angle) / 180);
                    double cosb1 = Math.Cos(p / 2 - tga - (p * angle) / 180);
                    double sinb2 = Math.Sin(p / 2 - tga + (p * angle) / 180);
                    double cosb2 = Math.Cos(p / 2 - tga + (p * angle) / 180);
                    arrowX_1 = -Convert.ToInt32((newr + a_long) * sinb1);
                    arrowY_1 = Convert.ToInt32((newr + a_long) * cosb1);
                    arrowX_2 = -Convert.ToInt32((newr + a_long) * sinb2);
                    arrowY_2 = Convert.ToInt32((newr + a_long) * cosb2);
                }
                else if (ly_1 <= ly_2 && lx_1 <= lx_2)
                {
                    double tga = Math.Atan(Convert.ToDouble(ly_1 - ly_2) / Convert.ToDouble(lx_1 - lx_2));
                    double sinb1 = Math.Sin(p / 2 - tga - (p * angle) / 180);
                    double cosb1 = Math.Cos(p / 2 - tga - (p * angle) / 180);
                    double sinb2 = Math.Sin(p / 2 - tga + (p * angle) / 180);
                    double cosb2 = Math.Cos(p / 2 - tga + (p * angle) / 180);
                    arrowX_1 = Convert.ToInt32((newr + a_long) * sinb1);
                    arrowY_1 = -Convert.ToInt32((newr + a_long) * cosb1);
                    arrowX_2 = Convert.ToInt32((newr + a_long) * sinb2);
                    arrowY_2 = -Convert.ToInt32((newr + a_long) * cosb2);
                }
            }

            gR.DrawLine(new Pen(color, 1), lx_2 - minusX_2, ly_2 + minusY_2,
                lx_2 - arrowX_1, ly_2 + arrowY_1);

            gR.DrawLine(new Pen(color, 1), lx_2 - minusX_2, ly_2 + minusY_2,
                lx_2 - arrowX_2, ly_2 + arrowY_2);

            #endregion
        }

        internal void DrawEdge(Graphics gR)
        {
            /*
            int size = 10;
            int a = Math.Abs(lx_1 - lx_2);
            int b = Math.Abs(ly_1 - ly_2);

            int c;
            int d;

            if (lx_1 > lx_2)
                c = lx_2;
            else
                c = lx_1;
            if (ly_1 > ly_2)
                d = ly_2;
            else
                d = ly_1;

            int x = c + a / 2;
            int y = d + b / 2;

            gR.DrawString(Convert.ToString(edge),
                new Font("Arial", size), new SolidBrush(color), x, y);
            */

            int size = 10;
            int a = Math.Abs(lx_1 - lx_2);
            int b = Math.Abs(ly_1 - ly_2);

            int c;
            int d;

            if (lx_1 > lx_2)
                c = lx_2;
            else
                c = lx_1;
            if (ly_1 > ly_2)
                d = ly_2;
            else
                d = ly_1;

            int x = c + a / 2;
            int y = d + b / 2;

            gR.DrawString(Convert.ToString(edge),
                new Font("Arial", size), new SolidBrush(color), x, y);
        }
    }

    #endregion
}
