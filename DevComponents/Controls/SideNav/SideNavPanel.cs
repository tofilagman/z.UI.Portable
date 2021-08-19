using DevComponents.DotNetBar.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Represents a panel which hosts controls for the SideNavItem.
    /// </summary>
    [ToolboxItem(false)]
    public class SideNavPanel : Panel, IScrollBarOverrideSupport
    {
        private ScrollbarSkinner _ScrollSkinner = null;
        public SideNavPanel()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                 | ControlStyles.ResizeRedraw
                 | DisplayHelp.DoubleBufferFlag
                 | ControlStyles.UserPaint
                 | ControlStyles.Opaque
                 , true);

            _ScrollSkinner = new ScrollbarSkinner(this);
        }

        protected override void Dispose(bool disposing)
        {
            _ScrollSkinner.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Color backColor = GetBackColor();
            if (!backColor.IsEmpty)
            {
                using (SolidBrush brush = new SolidBrush(backColor))
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
            base.OnPaint(e);
        }

        private Color _BackColor = Color.Empty;
        protected override void OnBackColorChanged(EventArgs e)
        {
            PropertyInfo prop = typeof (Control).GetProperty("RawBackColor",
                BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null)
            {
                _BackColor = (Color)prop.GetValue(this, null);
            }
            base.OnBackColorChanged(e);
        }
        
        private Color GetBackColor()
        {
            if(!_BackColor.IsEmpty)
                return _BackColor;
            return GetColorTable().PanelBackColor;
        }

        private SideNavColorTable GetColorTable()
        {
            return ((Office2007Renderer)GlobalManager.Renderer).ColorTable.SideNav;
        }

        protected override void WndProc(ref Message m)
        {
            //Console.WriteLine("{0} Message {1}", DateTime.Now, MapMessage(m.Msg));
            if (m.Msg == (int)WinApi.WindowsMessages.WM_VSCROLL || m.Msg == (int)WinApi.WindowsMessages.WM_HSCROLL || m.Msg == (int)WinApi.WindowsMessages.WM_MOUSEWHEEL)
            {
                base.WndProc(ref m);
                OnScrollBarValueChanged(new ScrollValueChangedEventArgs(ScrollbarControl.MapMessageToScrollChange(m.Msg)));
                return;
            }
            else if (m.Msg == (int)WinApi.WindowsMessages.WM_NCCALCSIZE)
            {
                base.WndProc(ref m);
                OnNonClientSizeChanged(EventArgs.Empty);
                return;
            }
            else if (m.Msg == 206 || m.Msg == 8270) // Internal RichTextBox message we use to trigger scroll-bar update
            {
                base.WndProc(ref m);
                OnScrollBarValueChanged(new ScrollValueChangedEventArgs(eScrollBarScrollChange.Horizontal | eScrollBarScrollChange.Vertical));
                return;
            }
            else if (m.Msg == (int)WinApi.WindowsMessages.WM_MOVE) // Internal RichTextBox message we use to trigger scroll-bar update
            {
                base.WndProc(ref m);
                OnControlMoved(EventArgs.Empty);
                return;
            }
            base.WndProc(ref m);
        }

        #region IScrollBarOverrideSupport Members
        [Browsable(false)]
        public event EventHandler NonClientSizeChanged;
        /// <summary>
        /// Raises NonClientSizeChanged event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnNonClientSizeChanged(EventArgs e)
        {
            EventHandler handler = NonClientSizeChanged;
            if (handler != null)
                handler(this, e);
        }

        public event ScrollValueChangedHandler ScrollBarValueChanged;
        /// <summary>
        /// Raises ScrollBarValueChanged event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnScrollBarValueChanged(ScrollValueChangedEventArgs e)
        {
            ScrollValueChangedHandler handler = ScrollBarValueChanged;
            if (handler != null)
                handler(this, e);
        }
        [Browsable(false)]
        public event EventHandler ControlMoved;
        /// <summary>
        /// Raises NonClientSizeChanged event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnControlMoved(EventArgs e)
        {
            EventHandler handler = ControlMoved;
            if (handler != null)
                handler(this, e);
        }
        bool IScrollBarOverrideSupport.DesignMode
        {
            get
            {
                return this.DesignMode;
            }
        }
        #endregion
    }
}
