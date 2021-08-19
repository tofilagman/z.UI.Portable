using DevComponents.DotNetBar.Rendering;
using DevComponents.Editors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar.Controls
{
    [ToolboxBitmap(typeof(CollapsibleSplitContainer), "Controls.CollapsibleSplitContainer.ico")]
    public class CollapsibleSplitContainer : SplitContainer
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the CollapsibleSplitContainer class.
        /// </summary>
        public CollapsibleSplitContainer()
        {
            ControlStyles cs = ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer;
            this.SetStyle(cs, true);
            object[] objArgs = new object[] { cs, true };
            MethodInfo mi = typeof(Control).GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi != null)
            {
                mi.Invoke(this.Panel1, objArgs);
                mi.Invoke(this.Panel2, objArgs);
            }
            this.SplitterWidth = 20;
            _OverrideCursorPropInfo = typeof(SplitContainer).GetProperty("OverrideCursor", BindingFlags.NonPublic | BindingFlags.Instance);

            this.SplitterMoved += SplitterMovedHandler;
        }

        protected override void Dispose(bool disposing)
        {
            this.SplitterMoved -= SplitterMovedHandler;
            base.Dispose(disposing);
        }
        #endregion

        #region Implementation
        private Rectangle _NearCollapseButton = Rectangle.Empty;
        private Rectangle _FarCollapseButton = Rectangle.Empty;
        private eButtonState _NearCollapseButtonState = eButtonState.Normal, _FarCollapseButtonState = eButtonState.Normal;
        private const int ButtonSpacing = 2;

        private void UpdateButtonLayout()
        {
            Rectangle r = this.SplitterRectangle;
            Size buttonSize = new Size(this.SplitterWidth, this.SplitterWidth);
            if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                if (_ButtonPosition == eSplitterButtonPosition.Near)
                {
                    _NearCollapseButton = new Rectangle(r.X, r.Y, buttonSize.Width, buttonSize.Height);
                    _FarCollapseButton = new Rectangle(_NearCollapseButton.Right + ButtonSpacing, r.Y, buttonSize.Width, buttonSize.Height);
                }
                else if (_ButtonPosition == eSplitterButtonPosition.Center)
                {
                    _NearCollapseButton = new Rectangle(r.X + (r.Width - (buttonSize.Width * 2 + ButtonSpacing)) / 2, r.Y, buttonSize.Width, buttonSize.Height);
                    _FarCollapseButton = new Rectangle(_NearCollapseButton.Right + ButtonSpacing, r.Y, buttonSize.Width, buttonSize.Height);
                }
                else if (_ButtonPosition == eSplitterButtonPosition.Far)
                {
                    _NearCollapseButton = new Rectangle(r.Right - buttonSize.Width * 2 - ButtonSpacing, r.Y, buttonSize.Width, buttonSize.Height);
                    _FarCollapseButton = new Rectangle(_NearCollapseButton.Right + ButtonSpacing, r.Y, buttonSize.Width, buttonSize.Height);
                }
            }
            else
            {
                if (_ButtonPosition == eSplitterButtonPosition.Near)
                {
                    _NearCollapseButton = new Rectangle(r.X, r.Y, buttonSize.Width, buttonSize.Height);
                    _FarCollapseButton = new Rectangle(r.X, _NearCollapseButton.Bottom + ButtonSpacing, buttonSize.Width, buttonSize.Height);
                }
                else if (_ButtonPosition == eSplitterButtonPosition.Center)
                {
                    _NearCollapseButton = new Rectangle(r.X, r.Y + (r.Height - (buttonSize.Height * 2 + ButtonSpacing)) / 2, buttonSize.Width, buttonSize.Height);
                    _FarCollapseButton = new Rectangle(r.X, _NearCollapseButton.Bottom + ButtonSpacing, buttonSize.Width, buttonSize.Height);
                }
                else if (_ButtonPosition == eSplitterButtonPosition.Far)
                {
                    _NearCollapseButton = new Rectangle(r.X, r.Bottom - buttonSize.Height * 2 - ButtonSpacing, buttonSize.Width, buttonSize.Height);
                    _FarCollapseButton = new Rectangle(r.X, _NearCollapseButton.Bottom + ButtonSpacing, buttonSize.Width, buttonSize.Height);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            UpdateButtonLayout();

            base.OnPaint(e);

            Graphics g = e.Graphics;
            bool isHorizontal = this.Orientation == System.Windows.Forms.Orientation.Horizontal;

            if (_NearCollapseButtonState != eButtonState.Hidden)
            {
                Office2007ButtonItemStateColorTable ct = GetOffice2007StateColorTable(_NearCollapseButtonState);
                Office2007ButtonItemPainter.PaintBackground(g, ct, _NearCollapseButton, RoundRectangleShapeDescriptor.RectangleShape);
                Rectangle r = _NearCollapseButton;
                r.Inflate(-2, -2);
                TextDrawing.DrawStringLegacy(g, isHorizontal ? "\uf077" : "\uf053", Symbols.GetFont(9f, eSymbolSet.Awesome),
                    ct.Text, r, eTextFormat.HorizontalCenter | eTextFormat.VerticalCenter);
            }
            if (_FarCollapseButtonState != eButtonState.Hidden)
            {
                Office2007ButtonItemStateColorTable ct = GetOffice2007StateColorTable(_FarCollapseButtonState);
                Office2007ButtonItemPainter.PaintBackground(g, ct, _FarCollapseButton, RoundRectangleShapeDescriptor.RectangleShape);
                Rectangle r = _FarCollapseButton;
                r.Inflate(-2, -2);
                TextDrawing.DrawStringLegacy(g, isHorizontal ? "\uf078" : "\uf054", Symbols.GetFont(9f, eSymbolSet.Awesome),
                    ct.Text, r, eTextFormat.HorizontalCenter | eTextFormat.VerticalCenter);
            }

            
        }

        protected Office2007ButtonItemStateColorTable GetOffice2007StateColorTable(eButtonState state)
        {
            if (GlobalManager.Renderer is Office2007Renderer)
            {
                Office2007ColorTable ct = ((Office2007Renderer)GlobalManager.Renderer).ColorTable;
                Office2007ButtonItemColorTable buttonColorTable = ct.ButtonItemColors[Enum.GetName(typeof(eButtonColor), eButtonColor.OrangeWithBackground)];
                if (!this.Enabled || state == eButtonState.Disabled)
                    return buttonColorTable.Disabled;
                else if (state == eButtonState.MouseDownLeft)
                    return buttonColorTable.Pressed;
                else if (state == eButtonState.MouseOver)
                    return buttonColorTable.MouseOver;
                else
                    return buttonColorTable.Default;
            }

            return null;
        }

        private Orientation _CurrentOrientation = Orientation.Vertical;
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (_CurrentOrientation != this.Orientation)
            {
                _CurrentOrientation = this.Orientation;
                UpdateButtonLayout();
            }
            Invalidate();
        }

        private bool IsButtonActive(eButtonState state)
        {
            return state != eButtonState.Hidden && state != eButtonState.Disabled;
        }
        private Cursor _OriginalCursor = null;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsButtonActive(_NearCollapseButtonState))
            {
                if (_NearCollapseButton.Contains(e.Location))
                {
                    if (_NearCollapseButtonState == eButtonState.Normal)
                    {
                        _NearCollapseButtonState = eButtonState.MouseOver;
                        this.Invalidate();
                    }
                }
                else
                {
                    _NearCollapseButtonState = eButtonState.Normal;
                    this.Invalidate();
                }
            }
            if (IsButtonActive(_FarCollapseButtonState))
            {
                if (_FarCollapseButton.Contains(e.Location))
                {
                    if (_FarCollapseButtonState == eButtonState.Normal)
                    {
                        _FarCollapseButtonState = eButtonState.MouseOver;
                        this.Invalidate();
                    }
                }
                else
                {
                    _FarCollapseButtonState = eButtonState.Normal;
                    this.Invalidate();
                }
            }
            if (_NearCollapseButton.Contains(e.Location) || _FarCollapseButton.Contains(e.Location))
            {
                if (_OriginalCursor == null && _OverrideCursorPropInfo != null)
                {
                    _OriginalCursor = (Cursor)_OverrideCursorPropInfo.GetValue(this, null);
                    _OverrideCursorPropInfo.SetValue(this, Cursors.Default, null);
                }
                return;
            }

            RestoreOriginalCursor();

            base.OnMouseMove(e);
        }

        private PropertyInfo _OverrideCursorPropInfo = null;
        private void RestoreOriginalCursor()
        {
            if (_OriginalCursor != null)
            {
                _OverrideCursorPropInfo.SetValue(this, _OriginalCursor, null); ;
                _OriginalCursor = null;
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (IsButtonActive(_NearCollapseButtonState))
            {
                if (_NearCollapseButton.Contains(e.Location) && e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    _NearCollapseButtonState = eButtonState.MouseDownLeft;
                    this.Invalidate();
                }
            }
            if (IsButtonActive(_FarCollapseButtonState))
            {
                if (_FarCollapseButton.Contains(e.Location) && e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    _FarCollapseButtonState = eButtonState.MouseDownLeft;
                    this.Invalidate();
                }
            }
            if (_NearCollapseButton.Contains(e.Location) || _FarCollapseButton.Contains(e.Location))
                return;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (IsButtonActive(_NearCollapseButtonState))
            {
                if (_NearCollapseButton.Contains(e.Location))
                {
                    if (_NearCollapseButtonState == eButtonState.MouseDownLeft)
                    {
                        _NearCollapseButtonState = eButtonState.MouseOver;
                        // Trigger button action
                        NearCollapseButtonClick();
                    }
                    this.Invalidate();
                }
            }
            if (IsButtonActive(_FarCollapseButtonState))
            {
                if (_FarCollapseButton.Contains(e.Location))
                {
                    if (_FarCollapseButtonState == eButtonState.MouseDownLeft)
                    {
                        _FarCollapseButtonState = eButtonState.MouseOver;
                        // Trigger button action
                        FarCollapseButtonClick();
                    }
                    this.Invalidate();
                }
            }

            if (_NearCollapseButton.Contains(e.Location) || _FarCollapseButton.Contains(e.Location))
                return;

            base.OnMouseUp(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            this.Invalidate();
        }

        // <summary>
        /// Occurs before near collapse button is clicked and allows you to cancel its action.
        /// </summary>
        [Description("Occurs before near collapse button is clicked and allows you to cancel its action.")]
        public event CancelEventHandler BeforeNearCollapseButtonClick;
        /// <summary>
        /// Raises BeforeNearCollapseButtonClick event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnBeforeNearCollapseButtonClick(CancelEventArgs e)
        {
            CancelEventHandler handler = BeforeNearCollapseButtonClick;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Occurs after near collapse button is clicked.
        /// </summary>
        [Description("Occurs after near collapse button is clicked.")]
        public event EventHandler NearCollapseButtonClicked;
        /// <summary>
        /// Raises NearCollapseButtonClick event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnNearCollapseButtonClicked(EventArgs e)
        {
            EventHandler handler = NearCollapseButtonClicked;
            if (handler != null)
                handler(this, e);
        }

        private int _OriginalSplitterDistance = 0;
        private bool _IsPanel1Minimized = false;
        private bool _IsPanel2Minimized = false;
        private bool _InternalDistanceChange = false;
        private void NearCollapseButtonClick()
        {
            CancelEventArgs e = new CancelEventArgs();
            OnBeforeNearCollapseButtonClick(e);
            if (e.Cancel) return;

            if (_CollapseMode == eCollapseMode.PanelMinSize)
            {
                _InternalDistanceChange = true;
                try
                {
                    if (_IsPanel2Minimized)
                    {
                        this.SplitterDistance = _OriginalSplitterDistance;
                        _IsPanel2Minimized = false;
                        _FarCollapseButtonState = eButtonState.Normal;
                    }
                    else if (!_IsPanel1Minimized)
                    {
                        _OriginalSplitterDistance = this.SplitterDistance;
                        this.SplitterDistance = this.Panel1MinSize;
                        _IsPanel1Minimized = true;
                        _NearCollapseButtonState = eButtonState.Disabled;
                    }
                }
                finally
                {
                    _InternalDistanceChange = false;
                }
            }
            else
            {
                this.Panel1Collapsed = true;
            }

            OnNearCollapseButtonClicked(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs before far collapse button is clicked and allows you to cancel its action.
        /// </summary>
        [Description("Occurs before far collapse button is clicked and allows you to cancel its action.")]
        public event CancelEventHandler BeforeFarCollapseButtonClick;
        /// <summary>
        /// Raises BeforeFarCollapseButtonClick event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnBeforeFarCollapseButtonClick(CancelEventArgs e)
        {
            CancelEventHandler handler = BeforeFarCollapseButtonClick;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Occurs after far collapse button is clicked.
        /// </summary>
        [Description("Occurs after far collapse button is clicked.")]
        public event EventHandler FarCollapseButtonClicked;
        /// <summary>
        /// Raises FarCollapseButtonClick event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnFarCollapseButtonClicked(EventArgs e)
        {
            EventHandler handler = FarCollapseButtonClicked;
            if (handler != null)
                handler(this, e);
        }

        private void FarCollapseButtonClick()
        {
            CancelEventArgs e=new CancelEventArgs();
            OnBeforeFarCollapseButtonClick(e);
            if (e.Cancel) return;

            if (_CollapseMode == eCollapseMode.PanelMinSize)
            {
                _InternalDistanceChange = true;

                try
                {
                    if (_IsPanel1Minimized)
                    {
                        this.SplitterDistance = _OriginalSplitterDistance;
                        _IsPanel1Minimized = false;
                        _NearCollapseButtonState = eButtonState.Normal;
                    }
                    else if (!_IsPanel2Minimized)
                    {
                        _OriginalSplitterDistance = this.SplitterDistance;
                        if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                            this.SplitterDistance = this.Width - this.Panel2MinSize;
                        else
                            this.SplitterDistance = this.Height - this.Panel2MinSize;
                        _IsPanel2Minimized = true;
                        _FarCollapseButtonState = eButtonState.Disabled;
                    }
                }
                finally
                {
                    _InternalDistanceChange = false;
                }
            }
            else
            {
                this.Panel2Collapsed = true;
            }

            OnFarCollapseButtonClicked(EventArgs.Empty);
        }

        private void SplitterMovedHandler(object sender, SplitterEventArgs e)
        {
            if (_InternalDistanceChange) return;
            if(_IsPanel1Minimized)
            {
                _IsPanel1Minimized = false;
                _NearCollapseButtonState = eButtonState.Normal;
            }
            else if (_IsPanel2Minimized)
            {
                _IsPanel2Minimized = false;
                _FarCollapseButtonState = eButtonState.Normal;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsButtonActive(_NearCollapseButtonState))
            {
                _NearCollapseButtonState = eButtonState.Normal;
            }
            if (IsButtonActive(_FarCollapseButtonState))
            {
                _FarCollapseButtonState = eButtonState.Normal;
            }
            RestoreOriginalCursor();
            Invalidate();
            base.OnMouseLeave(e);
        }

        private eSplitterButtonPosition _ButtonPosition = eSplitterButtonPosition.Near;
        /// <summary>
        /// Indicates position of buttons inside container.
        /// </summary>
        [DefaultValue(eSplitterButtonPosition.Near), Category("Appearance"), Description("Indicates position of buttons inside container.")]
        public eSplitterButtonPosition ButtonPosition
        {
            get { return _ButtonPosition; }
            set
            {
                if (value != _ButtonPosition)
                {
                    eSplitterButtonPosition oldValue = _ButtonPosition;
                    _ButtonPosition = value;
                    OnButtonPositionChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when ButtonPosition property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnButtonPositionChanged(eSplitterButtonPosition oldValue, eSplitterButtonPosition newValue)
        {
            UpdateButtonLayout();
            this.Invalidate();
            //OnPropertyChanged(new PropertyChangedEventArgs("ButtonPosition"));
        }

        private eCollapseMode _CollapseMode = eCollapseMode.PanelMinSize;
        /// <summary>
        /// Specifies how panels are collapsed when collapse buttons are pressed.
        /// </summary>
        [DefaultValue(eCollapseMode.PanelMinSize), Category("Behavior"), Description("Specifies how panels are collapsed when collapse buttons are pressed.")]
        public eCollapseMode CollapseMode
        {
            get { return _CollapseMode; }
            set
            {
                if (value !=_CollapseMode)
                {
                    eCollapseMode oldValue = _CollapseMode;
                    _CollapseMode = value;
                    OnCollapseModeChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when CollapseMode property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnCollapseModeChanged(eCollapseMode oldValue, eCollapseMode newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("CollapseMode"));
        }
        #endregion
    }

    /// <summary>
    /// Defines available positions for buttons inside CollapsibleSplitterContainer.
    /// </summary>
    public enum eSplitterButtonPosition
    {
        /// <summary>
        /// Buttons are positioned on left or top side depending on orientation.
        /// </summary>
        Near,
        /// <summary>
        /// Buttons are positioned in center of container.
        /// </summary>
        Center,
        /// <summary>
        /// Buttons are positioned on right or bottom side depending on orientation.
        /// </summary>
        Far,
    }

    /// <summary>
    /// Defines collapse mode for the CollapsibleSplitContainer control.
    /// </summary>
    public enum eCollapseMode
    {
        /// <summary>
        /// When buttons are pressed the splitter is positioned at the PanelMinSize.
        /// </summary>
        PanelMinSize,
        /// <summary>
        /// When buttons are pressed associated panel is collapsed through Panel1Collapsed or Panel2Collapsed properties.
        /// </summary>
        PanelCollapse
    }
}
