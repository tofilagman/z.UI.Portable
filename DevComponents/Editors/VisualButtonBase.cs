 
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace DevComponents.Editors
{
    public class VisualButtonBase : VisualItem
    {
        #region Private Variables
        #endregion

        #region Events

        #endregion

        #region Constructor
        #endregion

        #region Internal Implementation
        private bool _ClickAutoRepeat = false;
        /// <summary>
        /// Gets or sets whether button automatically gets Click events repeated when mouse is kept pressed on the button. Default value is false.
        /// </summary>
        [DefaultValue(false)]
        public bool ClickAutoRepeat
        {
            get { return _ClickAutoRepeat; }
            set
            {
                _ClickAutoRepeat = value;
            }
        }

        private Timer _ClickTimer = null;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.GetIsEnabled() && _ClickAutoRepeat)
            {
                if (_ClickTimer == null)
                    _ClickTimer = new Timer();
                _ClickTimer.Interval = ClickRepeatInterval;
                _ClickTimer.Tick += new EventHandler(this.ClickTimerTick);
                _ClickTimer.Start();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            DisposeAutoClickTimer();
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave()
        {
            HideTooltip();
            DisposeAutoClickTimer();
            base.OnMouseLeave();
        }

        private void DisposeAutoClickTimer()
        {
            if (_ClickTimer != null)
            {
                Timer t = _ClickTimer;
                _ClickTimer = null;
                t.Stop();
                t.Tick -= new EventHandler(this.ClickTimerTick);
                t.Dispose();
            }
        }
        private void ClickTimerTick(object sender, EventArgs e)
        {
            this.ProcessClick();
            this.OnClickAutoRepeat();
        }

        protected virtual void OnClickAutoRepeat()
        {

        }

        private int _ClickRepeatInterval = 300;
        /// <summary>
        /// Gets or sets the auto-repeat interval for the click event when mouse button is kept pressed over the item.
        /// </summary>
        [Browsable(true), DefaultValue(300), Category("Behavior"), Description("Gets or sets the auto-repeat interval for the click event when mouse button is kept pressed over the item.")]
        public virtual int ClickRepeatInterval
        {
            get
            {
                return _ClickRepeatInterval;
            }
            set
            {
                _ClickRepeatInterval = value;
            }
        }

        private bool _RenderDefaultBackground = true;
        /// <summary>
        /// Gets or sets whether default button background is rendered when mouse is not over the host control. Default value is true.
        /// </summary>
        public bool RenderDefaultBackground
        {
            get { return _RenderDefaultBackground; }
            set
            {
                if (_RenderDefaultBackground != value)
                {
                    _RenderDefaultBackground = value;
                    this.InvalidateRender();
                }
            }
        }

        private DevComponents.DotNetBar.eShortcut _Shortcut = DevComponents.DotNetBar.eShortcut.None;
        /// <summary>
        /// Gets or sets the shortcut key which when pressed triggers button click event or its default function.
        /// </summary>
        [DefaultValue(DevComponents.DotNetBar.eShortcut.None), Description("Indicates shortcut key which when pressed triggers button click event or its default function.")]
        public DevComponents.DotNetBar.eShortcut Shortcut
        {
            get { return _Shortcut; }
            set
            {
                _Shortcut = value;
            }
        }

        protected override bool OnCmdKey(ref Message msg, Keys keyData)
        {
            if (_Shortcut != DevComponents.DotNetBar.eShortcut.None)
            {
                if (keyData == (Keys)_Shortcut)
                {
                    OnClick(new KeyEventArgs(keyData));
                    return true;
                }
            }
            return base.OnCmdKey(ref msg, keyData);
        }

        private string _Tooltip = "";
        /// <summary>
        /// Gets or sets the button tooltip.
        /// </summary>
        public string Tooltip
        {
            get { return _Tooltip; }
            set
            {
                _Tooltip = value;
            }
        }

        private DevComponents.DotNetBar.ToolTip _TooltipWnd = null;
        protected override void OnMouseHover(EventArgs e)
        {
            ShowTooltip();
            base.OnMouseHover(e);
        }


        private void ShowTooltip()
        {
            if (!string.IsNullOrEmpty(_Tooltip))
            {
                if (_TooltipWnd == null)
                    _TooltipWnd = new DevComponents.DotNetBar.ToolTip();
                _TooltipWnd.Style = DevComponents.DotNetBar.StyleManager.GetEffectiveStyle();
                _TooltipWnd.Text = _Tooltip;
                _TooltipWnd.ShowToolTip();
            }
        }
        private void HideTooltip()
        {
            if (_TooltipWnd != null)
            {
                try
                {
                    if (_TooltipWnd != null)
                    {
                        _TooltipWnd.Hide();
                        _TooltipWnd.Dispose();
                        _TooltipWnd = null;
                        ResetHover();
                    }
                }
                catch { }
            }
        }
        #endregion

    }
}
 

