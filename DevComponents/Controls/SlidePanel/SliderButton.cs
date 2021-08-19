using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Represents the slider button that is used by SlidePanel when collapsed to slide it back into the view.
    /// </summary>
    [ToolboxItem(false)]
    public class SliderButton : Control
    {
        #region Events

        #endregion

        #region Constructor
        private SlidePanel _SlidePanel = null;
        /// <summary>
        /// Initializes a new instance of the SliderButton class.
        /// </summary>
        /// <param name="slidePanel"></param>
        public SliderButton(SlidePanel slidePanel)
        {
            _SlidePanel = slidePanel;
            this.Margin = new System.Windows.Forms.Padding();
        }

        protected override void Dispose(bool disposing)
        {
            if (_Style != null)
                _Style.StyleChanged -= this.ElementStyleChanged;
            DestroyTopmostCheckTimer();
            base.Dispose(disposing);
        }
        #endregion

        #region Implementation
        protected override void OnParentChanged(EventArgs e)
        {
            if (this.Parent != null)
                UpdatePosition();
            base.OnParentChanged(e);
        }
        protected override void OnClick(EventArgs e)
        {
            if (!_IsSlidingActive)
                _SlidePanel.IsOpen = true;
            base.OnClick(e);
        }
        private Point _MouseOffset = Point.Empty;
        private Point _MouseDownPoint = Point.Empty;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _MouseDownPoint = Control.MousePosition;
                IsMouseDown = true;
            }
            base.OnMouseDown(e);
        }
        private Point _OriginalSlidePanelLocation = Point.Empty;
        private bool _IsSlidingActive = false;
        private int MaximizedOffset = SystemInformation.FrameBorderSize.Width * 2 - 2;
        private static readonly Point SlideOffsetStart = new Point(4, 4);
        private static readonly Point SlideOutSnapOffset = new Point(96, 96);
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseDown && !_IsSlidingActive)
            {
                eSlideSide side = _SlidePanel.SlideSide;
                if (side == eSlideSide.Left && Control.MousePosition.X - _MouseDownPoint.X > SlideOffsetStart.X)
                    _IsSlidingActive = true;
                else if (side == eSlideSide.Right && _MouseDownPoint.X - Control.MousePosition.X > SlideOffsetStart.X)
                    _IsSlidingActive = true;
                else if (side == eSlideSide.Top && Control.MousePosition.Y - _MouseDownPoint.Y > SlideOffsetStart.Y)
                    _IsSlidingActive = true;
                else if (side == eSlideSide.Bottom && _MouseDownPoint.Y - Control.MousePosition.Y > SlideOffsetStart.Y)
                    _IsSlidingActive = true;
            }
            if (IsMouseDown && _IsSlidingActive)
            {
                if (!this.Capture)
                    this.Capture = true;
                Point p = Control.MousePosition;
                eSlideSide side = _SlidePanel.SlideSide;
                if (side == eSlideSide.Left)
                    _MouseOffset = new Point(Math.Max(0, p.X - _MouseDownPoint.X), 0);
                else if (side == eSlideSide.Right)
                    _MouseOffset = new Point(Math.Max(0, _MouseDownPoint.X - p.X), 0);
                else if (side == eSlideSide.Top)
                    _MouseOffset = new Point(0, Math.Max(0, p.Y - _MouseDownPoint.Y));
                else if (side == eSlideSide.Bottom)
                    _MouseOffset = new Point(0, Math.Max(0, _MouseDownPoint.Y - p.Y));

                if (!_MouseOffset.IsEmpty)
                {
                    UpdatePosition();

                    // Slide panel out
                    if (!_SlidePanel.Visible)
                    {
                        _SlidePanel.Visible = true;
                        _OriginalSlidePanelLocation = _SlidePanel.Location;
                    }
                    if (side == eSlideSide.Left)
                        _SlidePanel.Left = _OriginalSlidePanelLocation.X + _MouseOffset.X;
                    else if (side == eSlideSide.Right)
                        _SlidePanel.Left = _OriginalSlidePanelLocation.X - _MouseOffset.X;
                    else if (side == eSlideSide.Top)
                        _SlidePanel.Top = _OriginalSlidePanelLocation.Y + _MouseOffset.Y;
                    else if (side == eSlideSide.Bottom)
                        _SlidePanel.Top = _OriginalSlidePanelLocation.Y - _MouseOffset.Y;
                }
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                Point mousePosition = Control.MousePosition;
                if (this.Capture) this.Capture = false;

                eSlideSide side = _SlidePanel.SlideSide;
                if (side == eSlideSide.Left && mousePosition.X - _MouseDownPoint.X > SlideOutSnapOffset.X)
                    _SlidePanel.IsOpen = true;
                else if (side == eSlideSide.Right && _MouseDownPoint.X - mousePosition.X > SlideOutSnapOffset.X)
                    _SlidePanel.IsOpen = true;
                else if (side == eSlideSide.Top && mousePosition.Y - _MouseDownPoint.Y > SlideOutSnapOffset.Y)
                    _SlidePanel.IsOpen = true;
                else if (side == eSlideSide.Bottom && _MouseDownPoint.Y - mousePosition.Y > SlideOutSnapOffset.Y)
                    _SlidePanel.IsOpen = true;
                else if (_IsSlidingActive)
                {
                    _SlidePanel.Location = _OriginalSlidePanelLocation;
                    _SlidePanel.Visible = false;
                }

                IsMouseDown = false;
                UpdatePosition();
            }
            base.OnMouseUp(e);
        }
        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            if (this.IsMouseDown && !this.Capture)
            {
            }
            base.OnMouseCaptureChanged(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            Size newSize = ActiveSliderSize;
            if (newSize != this.Size)
            {
                this.Size = newSize;
                UpdatePosition();
            }
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            Size newSize = this.SliderSize;
            if (newSize != this.Size)
            {
                this.Size = newSize;
                UpdatePosition();
            }
            base.OnMouseLeave(e);
        }
        protected override void OnResize(EventArgs e)
        {
            this.Invalidate();
            base.OnResize(e);
        }
        internal void UpdatePosition()
        {
            Control parent = this.Parent;
            if (parent == null) return;
            eSlideSide side = _SlidePanel.SlideSide;
            Point loc = new Point();
            //bool maximized = false;
            //if (this.Parent is DevComponents.DotNetBar.Metro.MetroAppForm && ((Form)this.Parent).WindowState == FormWindowState.Maximized) maximized = true;
            if (side == eSlideSide.Left)
            {
                loc.X = this.Margin.Left + _MouseOffset.X;
                loc.Y = (parent.ClientRectangle.Height - this.Height) / 2 + this.Margin.Top;
                //if (maximized) loc.X += MaximizedOffset;
            }
            else if (side == eSlideSide.Right)
            {
                loc.X = parent.ClientRectangle.Width - this.Margin.Right - this.Width - _MouseOffset.X;
                loc.Y = (parent.ClientRectangle.Height - this.Height) / 2 + this.Margin.Top;
                //if (maximized) loc.X -= MaximizedOffset;
            }
            else if (side == eSlideSide.Top)
            {
                loc.X = (parent.ClientRectangle.Width - this.Width) / 2 + this.Margin.Top;
                loc.Y = this.Margin.Top + _MouseOffset.Y;
            }
            else if (side == eSlideSide.Bottom)
            {
                loc.X = (parent.ClientRectangle.Width - this.Width) / 2 + this.Margin.Top;
                loc.Y = parent.ClientRectangle.Height - this.Margin.Bottom - this.Height - _MouseOffset.Y;
            }

            this.Location = loc;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (_Style != null)
            {
                ElementStyleDisplayInfo di = new ElementStyleDisplayInfo(_Style, g, this.ClientRectangle);
                ElementStyleDisplay.Paint(di);
            }

            if (!string.IsNullOrEmpty(_SymbolRealized))
            {
                Font symFont = Symbols.GetFont(_SymbolSize, _SymbolSet);
                Color foreColor = this.ForeColor;
                bool disposeStyle = false;
                ElementStyle style = null;
                if (_Style != null)
                {
                    style = ElementStyleDisplay.GetElementStyle(_Style, out disposeStyle);
                }
                if (style != null && !style.TextColor.IsEmpty) foreColor = style.TextColor;
                if(disposeStyle)
                    style.Dispose();
                TextDrawing.DrawStringLegacy(g, _SymbolRealized, symFont, _SymbolColor.IsEmpty ? foreColor : _SymbolColor,
                        this.ClientRectangle,
                        eTextFormat.HorizontalCenter | eTextFormat.NoClipping | eTextFormat.VerticalCenter);
            }

            base.OnPaint(e);
        }


        protected override void OnHandleCreated(EventArgs e)
        {
            SetupTopmostCheckTimer();
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            DestroyTopmostCheckTimer();
            base.OnHandleDestroyed(e);
        }
        private Timer _Timer = null;
        private void SetupTopmostCheckTimer()
        {
            if (_Timer != null || !_AutoTopMostEnabled) return;
            _Timer = new Timer();
            _Timer.Interval = 800;
            _Timer.Tick += new EventHandler(CheckAutoTopMostTimerTick);
            _Timer.Start();
        }
        void CheckAutoTopMostTimerTick(object sender, EventArgs e)
        {
            if (!this.Visible || this.Parent == null) return;

            Control parent = this.Parent;
            if (parent.Controls.IndexOf(this) != 0)
            {
                parent.Controls.SetChildIndex(this, 0);
            }
        }
        private void DestroyTopmostCheckTimer()
        {
            Timer timer = _Timer;
            _Timer = null;
            if (timer == null) return;
            timer.Stop();
            timer.Dispose();
        }

        private bool _AutoTopMostEnabled = true;
        /// <summary>
        /// Gets or sets whether slider button automatically checks whether its top-most control on the form, i.e. visible at all times on top of other controls
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether slider button automatically checks whether its top-most control on the form, i.e. visible at all times on top of other controls")]
        public bool AutoTopMostEnabled
        {
            get { return _AutoTopMostEnabled; }
            set
            {
                if (value != _AutoTopMostEnabled)
                {
                    bool oldValue = _AutoTopMostEnabled;
                    _AutoTopMostEnabled = value;
                    OnAutoTopMostEnabledChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when AutoTopMostEnabled property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnAutoTopMostEnabledChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("AutoTopMostEnabled"));
            if (!newValue)
                DestroyTopmostCheckTimer();
            else if (this.IsHandleCreated)
                SetupTopmostCheckTimer();
        }

        internal static readonly Size DefaultSliderSize = new Size(16, 36);
        private Size _SliderSize = DefaultSliderSize;
        /// <summary>
        /// Gets or sets the slider size in default state. Notice that size specified here applies to Left and Right SlidePanel positions. For Top and Bottom positions the Width and Height are interchaged.
        /// </summary>
        [Category("Appearance"), Description("Indicates slider size in default state. Notice that size specified here applies to Left and Right SlidePanel positions. For Top and Bottom positions the Width and Height are interchaged.")]
        public Size SliderSize
        {
            get { return _SliderSize; }
            set
            {
                if (value != _SliderSize)
                {
                    Size oldValue = _SliderSize;
                    _SliderSize = value;
                    OnSliderSizeChanged(oldValue, value);
                }
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeSliderSize()
        {
            return _SliderSize != DefaultSliderSize;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetSliderSize()
        {
            SliderSize = DefaultSliderSize;
        }
        /// <summary>
        /// Called when SliderSize property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnSliderSizeChanged(Size oldValue, Size newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("SliderSize"));
            this.Size = newValue;
        }

        internal static readonly Size DefaultActiveSliderSize = new Size(24, 36);
        private Size _ActiveSliderSize = DefaultActiveSliderSize;
        /// <summary>
        /// Gets or sets the slider size in active state. Notice that size specified here applies to Left and Right SlidePanel positions. For Top and Bottom positions the Width and Height are interchaged.
        /// </summary>
        [Category("Appearance"), Description("Indicates slider size in active state. Notice that size specified here applies to Left and Right SlidePanel positions. For Top and Bottom positions the Width and Height are interchaged.")]
        public Size ActiveSliderSize
        {
            get { return _ActiveSliderSize; }
            set
            {
                if (value != _ActiveSliderSize)
                {
                    Size oldValue = _ActiveSliderSize;
                    _ActiveSliderSize = value;
                    OnActiveSliderSizeChanged(oldValue, value);
                }
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeActiveSliderSize()
        {
            return _ActiveSliderSize != DefaultActiveSliderSize;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetActiveSliderSize()
        {
            ActiveSliderSize = DefaultActiveSliderSize;
        }
        /// <summary>
        /// Called when ActiveSliderSize property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnActiveSliderSizeChanged(Size oldValue, Size newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("ActiveSliderSize"));

        }

        private bool _IsActive;
        /// <summary>
        /// Gets or sets whether slider button is in active state.
        /// </summary>
        [DefaultValue(false), Category("Behavior"), Description("Indicates  whether slider button is in active state.")]
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                if (value != _IsActive)
                {
                    bool oldValue = _IsActive;
                    _IsActive = value;
                    OnIsActiveChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when IsActive property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnIsActiveChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("IsActive"));

        }

        protected override Size DefaultSize
        {
            get
            {
                return GetSliderSize();
            }
        }
        private Size GetSliderSize()
        {
            Size sliderSize = this.IsActive ? ActiveSliderSize : SliderSize;

            if (_SlidePanel != null)
            {
                eSlideSide slideSide = _SlidePanel.SlideSide;
                if (slideSide == eSlideSide.Left || slideSide == eSlideSide.Right)
                    return sliderSize;
                else
                    return new Size(sliderSize.Height, sliderSize.Width);
            }

            return sliderSize;
        }

        private bool _IsMouseDown = false;
        internal bool IsMouseDown
        {
            get { return _IsMouseDown; }
            set
            {
                if (value != _IsMouseDown)
                {
                    bool oldValue = _IsMouseDown;
                    _IsMouseDown = value;
                    OnIsMouseDownChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when IsMouseDown property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnIsMouseDownChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("IsMouseDown"));
            if (!newValue)
            {
                _IsSlidingActive = false;
                _MouseOffset = Point.Empty;
            }
            this.Invalidate();
        }

        private ElementStyle _Style = null;
        /// <summary>
        /// Gets or sets the style associated with the control.
        /// </summary>
        public ElementStyle Style
        {
            get { return _Style; }
            set
            {
                if (_Style != null)
                    _Style.StyleChanged -= this.ElementStyleChanged;
                _Style = value;
                if(_Style!=null)
                    _Style.StyleChanged += ElementStyleChanged;
                this.Invalidate();
            }
        }

        void ElementStyleChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private Color _SymbolColor = Color.Empty;
        /// <summary>
        /// Gets or sets the color of the Symbol.
        /// </summary>
        [Category("Appearance"), Description("Indicates color of the Symbol.")]
        public Color SymbolColor
        {
            get { return _SymbolColor; }
            set { _SymbolColor = value; this.Invalidate(); }
        }
        /// <summary>
        /// Gets whether property should be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeSymbolColor()
        {
            return !_SymbolColor.IsEmpty;
        }
        /// <summary>
        /// Resets property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetSymbolColor()
        {
            this.SymbolColor = Color.Empty;
        }

        /// <summary>
        /// Gets the realized symbol string.
        /// </summary>
        [Browsable(false)]
        public string SymbolRealized
        {
            get { return _SymbolRealized; }
        }
        private string _Symbol = "", _SymbolRealized = "";
        /// <summary>
        /// Indicates the symbol displayed on face of the button instead of the image. Setting the symbol overrides the image setting.
        /// </summary>
        [DefaultValue(""), Category("Appearance"), Description("Indicates the symbol displayed on face of the button instead of the image. Setting the symbol overrides the image setting.")]
        [Editor("DevComponents.DotNetBar.Design.SymbolTypeEditor, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf", typeof(System.Drawing.Design.UITypeEditor))]
        public string Symbol
        {
            get { return _Symbol; }
            set
            {
                if (value != _Symbol)
                {
                    string oldValue = _Symbol;
                    _Symbol = value;
                    OnSymbolChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when Symbol property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnSymbolChanged(string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
                _SymbolRealized = "";
            else
                _SymbolRealized = Symbols.GetSymbol(newValue);
            this.Invalidate();
        }

        private eSymbolSet _SymbolSet = eSymbolSet.Awesome;
        /// <summary>
        /// Gets or sets the symbol set used to represent the Symbol.
        /// </summary>
        [Browsable(false), DefaultValue(eSymbolSet.Awesome)]
        public eSymbolSet SymbolSet
        {
            get { return _SymbolSet; }
            set
            {
                if (_SymbolSet != value)
                {
                    eSymbolSet oldValue = _SymbolSet;
                    _SymbolSet = value;
                    OnSymbolSetChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when SymbolSet property value changes.
        /// </summary>
        /// <param name="oldValue">Indciates old value</param>
        /// <param name="newValue">Indicates new value</param>
        protected virtual void OnSymbolSetChanged(eSymbolSet oldValue, eSymbolSet newValue)
        {
            this.Invalidate();
            this.Refresh();
        }


        private float _SymbolSize = 10f;
        /// <summary>
        /// Indicates the size of the symbol in points.
        /// </summary>
        [DefaultValue(10f), Category("Appearance"), Description("Indicates the size of the symbol in points.")]
        public float SymbolSize
        {
            get { return _SymbolSize; }
            set
            {
                if (value != _SymbolSize)
                {
                    float oldValue = _SymbolSize;
                    _SymbolSize = value;
                    OnSymbolSizeChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when SymbolSize property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnSymbolSizeChanged(float oldValue, float newValue)
        {
            this.Invalidate();
        }
        #endregion
    }
}
