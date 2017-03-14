using System;
using System.Collections.Generic;
using System.Windows.Forms;
using control;

namespace pluginfo
{
    public delegate void PluginEventHandler(object sender, EventArgs e);
    public interface IPlugin
    {
        #region IPlugin

        event PluginEventHandler plugChange;

        List<CPoint> pDot { set; }
        List<CLine> pLine { set; }

        string pName { get; }
        string pDescription { get; }
        string stripStatusText { get; }

        decimal pTimerInterval { set; }
        decimal pTimerIntervalDefault { get; }

        bool CancelNewEdge { get; }
        bool CancelNewDot { get; }

        int Radius { set; }

        void control_Initialize();
        void control_MouseClick(object sender, MouseEventArgs e);
        void control_MouseMove(object sender, MouseEventArgs e);

        Cursor pCursor { get; }
        UserControl MainInterface { get; }

        #endregion
    }
}
