using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevComponents.DotNetBar.Events;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Represents the slider item which allows you to select a value from predefined range.
    /// </summary>
    [Browsable(false), Designer("DevComponents.DotNetBar.Design.SimpleItemDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf")]
    public class RangeSliderItem : BaseItem
    {
        #region Constructor
        /// <summary>
        /// Creates new instance of SliderItem.
        /// </summary>
        public RangeSliderItem() : this("", "") { }
        /// <summary>
        /// Creates new instance of SliderItem and assigns the name to it.
        /// </summary>
        /// <param name="sItemName">Item name.</param>
        public RangeSliderItem(string sItemName) : this(sItemName, "") { }
        /// <summary>
        /// Creates new instance of SliderItem and assigns the name and text to it.
        /// </summary>
        /// <param name="sItemName">Item name.</param>
        /// <param name="ItemText">item text.</param>
        public RangeSliderItem(string sItemName, string ItemText)
            : base(sItemName, ItemText)
        {
            this.MouseUpNotification = true;
            this.MouseDownCapture = true;
        }

        /// <summary>
        /// Returns copy of the item.
        /// </summary>
        public override BaseItem Copy()
        {
            RangeSliderItem objCopy = new RangeSliderItem(m_Name);
            this.CopyToItem(objCopy);
            return objCopy;
        }

        /// <summary>
        /// Copies the SliderItem specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New SliderItem instance.</param>
        internal void InternalCopyToItem(SliderItem copy)
        {
            CopyToItem(copy);
        }

        /// <summary>
        /// Copies the SliderItem specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New SliderItem instance.</param>
        protected override void CopyToItem(BaseItem copy)
        {
            RangeSliderItem c = copy as RangeSliderItem;
            base.CopyToItem(c);

            c.Maximum = this.Maximum;
            c.Minimum = this.Minimum;
        }

        protected override void Dispose(bool disposing)
        {
            HideRangeTooltip();
            base.Dispose(disposing);
        }

        #endregion

        #region Events
        /// <summary>
        /// Occurs before Value has changed and allow the cancellation of the change.
        /// </summary>
        [Description("Occurs before Value has changed and allow the cancellation of the change.")]
        public event RangeSliderValueChangingEventHandler ValueChanging;
        /// <summary>
        /// Raises ValueChanging event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnValueChanging(RangeSliderValueChangingEventArgs e)
        {
            RangeSliderValueChangingEventHandler handler = ValueChanging;
            if (handler != null)
                handler(this, e);
        }
        /// <summary>
        /// Occurs after Value property has changed
        /// </summary>
        [Description("Occurs after Value property has changed.")]
        public event EventHandler ValueChanged;
        /// <summary>
        /// Raises ValueChanged event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnValueChanged(EventArgs e)
        {
            EventHandler handler = ValueChanged;
            if (handler != null)
                handler(this, e);
        }
        #endregion

        #region Implementation
        public override void Paint(ItemPaintArgs p)
        {
            //SmoothingMode sm = p.Graphics.SmoothingMode;
            //p.Graphics.SmoothingMode = SmoothingMode.None;
            //p.Graphics.FillRectangle(Brushes.LightGray, m_Rect);
            //p.Graphics.SmoothingMode = sm;

            Rendering.BaseRenderer renderer = p.Renderer;
            if (renderer != null)
            {
                RangeSliderItemRendererEventArgs e = new RangeSliderItemRendererEventArgs(this, p.Graphics);
                e.ItemPaintArgs = p;
                renderer.DrawRangeSliderItem(e);
            }
            else
            {
                Rendering.RangeSliderPainter painter = PainterFactory.CreateRangeSliderPainter(this);
                if (painter != null)
                {
                    RangeSliderItemRendererEventArgs e = new RangeSliderItemRendererEventArgs(this, p.Graphics);
                    e.ItemPaintArgs = p;
                    painter.Paint(e);
                }
            }

            if (this.DesignMode && this.Focused)
            {
                Rectangle r = m_Rect;
                r.Inflate(-1, -1);
                DesignTime.DrawDesignTimeSelection(p.Graphics, r, p.Colors.ItemDesignTimeBorder);
            }
        }
        private int _TicksHeight = 4;
        public override void RecalcSize()
        {
            m_Rect.Size = new Size(_Width, _Height);
            UpdateTicksBounds();

            UpdateRangeButtonBounds();
            base.RecalcSize();
        }

        private void UpdateRangeButtonBounds()
        {
            Size buttonSize = Dpi.Size(_RangeButtonSize);

            int ticksHeight = Dpi.Height(_TicksHeight);
            int ticksRangeButtonSpacing = Dpi.Height(_TicksRangeButtonSpacing);
            if (_SliderOrientation == eOrientation.Horizontal)
            {
                int itemHeight = _Height;// m_Rect.Height;
                if (_TicksPosition == eTicksPosition.Top)
                {
                    _RangeButtonMinBounds = new Rectangle(0,
                                       ticksHeight + ticksRangeButtonSpacing + Math.Max(0, (itemHeight - (ticksHeight + buttonSize.Height + ticksRangeButtonSpacing)) / 2),
                                       buttonSize.Width,
                                       buttonSize.Height);
                }
                else if (_TicksPosition == eTicksPosition.Bottom)
                {
                    _RangeButtonMinBounds = new Rectangle(0,
                                       Math.Max(0, (itemHeight - (ticksHeight + buttonSize.Height + ticksRangeButtonSpacing)) / 2),
                                       buttonSize.Width,
                                       buttonSize.Height);
                }
                else if (_TicksPosition == eTicksPosition.TopAndBottom)
                {
                    _RangeButtonMinBounds = new Rectangle(0,
                                       ticksHeight + ticksRangeButtonSpacing + (itemHeight - (ticksHeight * 2 + buttonSize.Height + ticksRangeButtonSpacing * 2)) / 2,
                                       buttonSize.Width,
                                       buttonSize.Height);
                }
                _RangeButtonMaxBounds = _RangeButtonMinBounds;

                RangeValue minmax = GetNormalizedMinMaxValues();
                RangeValue value = GetNormalizedRangeValue();
                double xstep = GetXStep(_TicksBounds.Width, minmax.Min, minmax.Max, _TicksVisible ? _TicksStep : 1);

                _RangeButtonMinBounds.X = _TicksBounds.X + (int)((Math.Abs(value.Min - minmax.Min)/_TicksStep) * xstep) - buttonSize.Width;
                _RangeButtonMaxBounds.X = _TicksBounds.X + (int)((Math.Abs((value.Max - minmax.Min)/_TicksStep)) * xstep);
                if (value.Max == minmax.Max)
                    _RangeButtonMaxBounds.X = _TicksBounds.Right - 1;
            }
            else
            {
                int itemWidth = _Width; // m_Rect.Width;
                if (_TicksPosition == eTicksPosition.Top)
                {
                    _RangeButtonMinBounds = new Rectangle(ticksHeight + ticksRangeButtonSpacing + Math.Max(0, (itemWidth - (ticksHeight + buttonSize.Height + ticksRangeButtonSpacing)) / 2),
                                       0,
                                       buttonSize.Height,
                                       buttonSize.Width);
                }
                else if (_TicksPosition == eTicksPosition.Bottom)
                {
                    _RangeButtonMinBounds = new Rectangle(Math.Max(0, (itemWidth - (ticksHeight + buttonSize.Height + ticksRangeButtonSpacing)) / 2),
                                       0,
                                       buttonSize.Height,
                                       buttonSize.Width);
                }
                else if (_TicksPosition == eTicksPosition.TopAndBottom)
                {
                    _RangeButtonMinBounds = new Rectangle(ticksHeight + ticksRangeButtonSpacing + (itemWidth - (ticksHeight * 2 + buttonSize.Height + ticksRangeButtonSpacing * 2)) / 2,
                                       0,
                                       buttonSize.Height,
                                       buttonSize.Width);
                }
                _RangeButtonMaxBounds = _RangeButtonMinBounds;

                RangeValue minmax = GetNormalizedMinMaxValues();
                RangeValue value = GetNormalizedRangeValue();
                double ystep = GetXStep(_TicksBounds.Height, minmax.Min, minmax.Max, _TicksStep);

                _RangeButtonMinBounds.Y = _TicksBounds.Y + (int)((Math.Abs(value.Min - minmax.Min) / _TicksStep) * ystep) - buttonSize.Width;
                _RangeButtonMaxBounds.Y = _TicksBounds.Y + (int)((Math.Abs((value.Max - minmax.Min) / _TicksStep)) * ystep);
                if (value.Max == minmax.Max)
                    _RangeButtonMaxBounds.Y = _TicksBounds.Bottom - 1;
            }
        }
        private int _TicksRangeButtonSpacing = 2;
        private void UpdateTicksBounds()
        {
            _TicksBounds = Rectangle.Empty;
            _TicksBounds2 = Rectangle.Empty;

            Size rangeButtonSize = Dpi.Size(_RangeButtonSize);
            int ticksRangeButtonSpacing = Dpi.Height(_TicksRangeButtonSpacing);
            int ticksHeight = Dpi.Height(_TicksHeight);
            if (_SliderOrientation == eOrientation.Horizontal)
            {
                if (_TicksPosition == eTicksPosition.Top)
                {
                    _TicksBounds = new Rectangle(rangeButtonSize.Width,
                                       (m_Rect.Height - (ticksHeight + rangeButtonSize.Height + ticksRangeButtonSpacing)) / 2,
                                       m_Rect.Width - rangeButtonSize.Width * 2,
                                       ticksHeight);
                }
                else if (_TicksPosition == eTicksPosition.Bottom)
                {
                    _TicksBounds = new Rectangle(rangeButtonSize.Width,
                                       rangeButtonSize.Height + ticksRangeButtonSpacing + (m_Rect.Height - (ticksHeight + rangeButtonSize.Height + ticksRangeButtonSpacing)) / 2,
                                       m_Rect.Width - rangeButtonSize.Width * 2,
                                       ticksHeight);
                }
                else if (_TicksPosition == eTicksPosition.TopAndBottom)
                {
                    _TicksBounds = new Rectangle(rangeButtonSize.Width,
                                       (m_Rect.Height - (ticksHeight * 2 + rangeButtonSize.Height + ticksRangeButtonSpacing * 2)) / 2,
                                       m_Rect.Width - rangeButtonSize.Width * 2,
                                       ticksHeight);
                    _TicksBounds2 = new Rectangle(rangeButtonSize.Width,
                                       rangeButtonSize.Height + ticksHeight + ticksRangeButtonSpacing * 2 + (m_Rect.Height - (ticksHeight * 2 + rangeButtonSize.Height + ticksRangeButtonSpacing * 2)) / 2,
                                       m_Rect.Width - rangeButtonSize.Width * 2,
                                       ticksHeight);
                }
            }
            else
            {
                if (_TicksPosition == eTicksPosition.Top)
                {
                    _TicksBounds = new Rectangle((m_Rect.Width - (ticksHeight + rangeButtonSize.Height + ticksRangeButtonSpacing)) / 2,
                                       rangeButtonSize.Width,
                                       ticksHeight,
                                       m_Rect.Height - rangeButtonSize.Width * 2);
                }
                else if (_TicksPosition == eTicksPosition.Bottom)
                {
                    _TicksBounds = new Rectangle(rangeButtonSize.Height + ticksRangeButtonSpacing + (m_Rect.Width - (ticksHeight + rangeButtonSize.Height + ticksRangeButtonSpacing)) / 2,
                                       rangeButtonSize.Width,
                                       ticksHeight,
                                       m_Rect.Height - rangeButtonSize.Width * 2);
                }
                else if (_TicksPosition == eTicksPosition.TopAndBottom)
                {
                    _TicksBounds = new Rectangle((m_Rect.Width - (ticksHeight * 2 + rangeButtonSize.Height + ticksRangeButtonSpacing * 2)) / 2,
                                       rangeButtonSize.Width,
                                       ticksHeight,
                                       m_Rect.Height - rangeButtonSize.Width * 2);
                    _TicksBounds2 = new Rectangle(rangeButtonSize.Height + ticksHeight + ticksRangeButtonSpacing * 2 + (m_Rect.Width - (ticksHeight * 2 + rangeButtonSize.Height + ticksRangeButtonSpacing * 2)) / 2,
                                       rangeButtonSize.Width,
                                       ticksHeight,
                                       m_Rect.Height - rangeButtonSize.Width * 2);
                }
            }
        }
        internal static double GetXStep(int ticksBoundsWidth, int min, int max, int tickSteps)
        {
            if (max == min) return 1;
            double xstep = ticksBoundsWidth / ((double)Math.Abs(max - min) / tickSteps);
            return xstep;
        }
        internal RangeValue GetNormalizedRangeValue()
        {
            RangeValue value = this.Value;
            if (value.Min < this.Minimum)
                value.Min = this.Minimum;
            if (value.Max > this.Maximum)
                value.Max = this.Maximum;

            if (value.Max < value.Min)
                value.Max = value.Min;

            return value;
        }
        internal RangeValue GetNormalizedMinMaxValues()
        {
            int min = this.Minimum;
            int max = this.Maximum;
            if (max < min) max = min + 1;
            return new RangeValue(min, max);
        }

        private eRangeSliderPart _MouseOverPart = eRangeSliderPart.None;
        /// <summary>
        /// Gets mouse over part.
        /// </summary>
        [Browsable(false)]
        public eRangeSliderPart MouseOverPart
        {
            get { return _MouseOverPart; }
            private set
            {
                if (_MouseOverPart != value)
                {
                    _MouseOverPart = value;
                    this.Refresh();
                }
            }
        }
        /// <summary>
        /// Gets current part that is pressed using mouse left button.
        /// </summary>
        [Browsable(false)]
        public eRangeSliderPart MouseDownPart
        {
            get { return _MouseDownPart; }
            private set
            {
                if (_MouseDownPart != value)
                {
                    _MouseDownPart = value;
                    this.Refresh();
                }
            }
        }

        private eRangeSliderPart _MouseDownPart = eRangeSliderPart.None;
        public override void InternalMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && !this.DesignMode)
            {
                MouseDownPart = HitTest(e.Location);
                if (MouseDownPart == eRangeSliderPart.TickArea && _ClientClickRangeChangeEnabled)
                {
                    int v = 0;
                    RangeValue minmax = GetNormalizedMinMaxValues();
                    // Try to calculate the new range, first get click value
                    if (_SliderOrientation == eOrientation.Horizontal)
                    {
                        int x = e.X - (_TicksBounds.X + this.Bounds.X);
                        v = minmax.Min + (int)(x / GetXStep(_TicksBounds.Width, minmax.Min, minmax.Max, _TicksVisible ? _TicksStep : 1)) * _TicksStep;
                    }
                    else
                    {
                        int y = e.Y - (_TicksBounds.Y + this.Bounds.Y);
                        v = minmax.Min + (int)(y / GetXStep(_TicksBounds.Height, minmax.Min, minmax.Max, _TicksVisible ? _TicksStep : 1)) * _TicksStep;
                    }

                    if (v > minmax.Max)
                        v = minmax.Max;
                    if (v < minmax.Min)
                        v = minmax.Min;
                    RangeValue newValue = this.Value;
                    if (v > this.Value.Max)
                        newValue= new RangeValue(this.Value.Min, v);
                    else if (v < this.Value.Min)
                        newValue = new RangeValue(v, this.Value.Max);
                    else if(Math.Abs(v - this.Value.Min) < Math.Abs(v - this.Value.Max) && v<=this.Value.Max)
                        newValue = new RangeValue(v, this.Value.Max);
                    else if(v>=this.Value.Min)
                        newValue = new RangeValue(this.Value.Min, v);

                    if (Math.Abs(newValue.Max - newValue.Min) >= _MinimumAbsoluteRange)
                        SetValue(newValue, eEventSource.Mouse);
                }
            }

            base.InternalMouseDown(e);
        }
        public override void InternalMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && !this.DesignMode)
            {
                if (_MouseDownPart == eRangeSliderPart.MinRangeSlider)
                {
                    if (_SliderOrientation == eOrientation.Horizontal)
                    {
                        int x = e.X - (_TicksBounds.X + this.Bounds.X);
                        RangeValue minmax = GetNormalizedMinMaxValues();
                        double xStep = GetXStep(_TicksBounds.Width, minmax.Min, minmax.Max, _TicksStep);
                        int v = minmax.Min + (int)(x / xStep) * _TicksStep;
                        if (v > this.Value.Max)
                            v = this.Value.Max;
                        if (v < minmax.Min)
                            v = minmax.Min;

                        RangeValue newValue = new RangeValue(v, this.Value.Max);
                        if (Math.Abs(newValue.Max - newValue.Min) >= _MinimumAbsoluteRange)
                            SetValue(newValue, eEventSource.Mouse);
                    }
                    else
                    {
                        int y = e.Y - (_TicksBounds.Y + this.Bounds.Y);
                        RangeValue minmax = GetNormalizedMinMaxValues();
                        double xStep = GetXStep(_TicksBounds.Height, minmax.Min, minmax.Max, _TicksStep);
                        int v = minmax.Min + (int)(y / xStep) * _TicksStep;
                        if (v > this.Value.Max)
                            v = this.Value.Max;
                        if (v < minmax.Min)
                            v = minmax.Min;

                        RangeValue newValue = new RangeValue(v, this.Value.Max);
                        if (Math.Abs(newValue.Max - newValue.Min) >= _MinimumAbsoluteRange)
                            SetValue(newValue, eEventSource.Mouse);
                    }
                }
                else if (_MouseDownPart == eRangeSliderPart.MaxRangeSlider)
                {
                    if (_SliderOrientation == eOrientation.Horizontal)
                    {
                        int x = e.X - (_TicksBounds.X + this.Bounds.X);
                        RangeValue minmax = GetNormalizedMinMaxValues();
                        double xStep = GetXStep(_TicksBounds.Width, minmax.Min, minmax.Max, _TicksStep);
                        int v = minmax.Min + (int)(x / xStep) * _TicksStep;
                        if (v < this.Value.Min)
                            v = this.Value.Min;
                        if (v > minmax.Max)
                            v = minmax.Max;
                        RangeValue newValue = new RangeValue(this.Value.Min, v);
                        if (Math.Abs(newValue.Max - newValue.Min) >= _MinimumAbsoluteRange)
                            SetValue(newValue, eEventSource.Mouse);
                    }
                    else
                    {
                        int y = e.Y - (_TicksBounds.Y + this.Bounds.Y);
                        RangeValue minmax = GetNormalizedMinMaxValues();
                        int v = minmax.Min + (int)(y / GetXStep(_TicksBounds.Height, minmax.Min, minmax.Max, _TicksStep)) * _TicksStep;
                        if (v < this.Value.Min)
                            v = this.Value.Min;
                        if (v > minmax.Max)
                            v = minmax.Max;
                        RangeValue newValue = new RangeValue(this.Value.Min, v);
                        if (Math.Abs(newValue.Max - newValue.Min) >= _MinimumAbsoluteRange)
                            SetValue(newValue, eEventSource.Mouse);
                    }
                }
            }
            else if (e.Button == MouseButtons.None && !this.DesignMode)
            {
                this.MouseOverPart = HitTest(e.Location);
            }

            base.InternalMouseMove(e);
        }
        public override void InternalMouseUp(System.Windows.Forms.MouseEventArgs objArg)
        {
            MouseDownPart = eRangeSliderPart.None;
            HideRangeTooltip();
            base.InternalMouseUp(objArg);
        }
        public override void InternalMouseLeave()
        {
            this.MouseOverPart = eRangeSliderPart.None;
            base.InternalMouseLeave();
        }

        private ToolTip _RangeTooltip = null;
        private void UpdateAndShowRangeTooltip()
        {
            if (_RangeTooltip == null)
            {
                _RangeTooltip = new ToolTip();
                _RangeTooltip.FixedLocation = GetRangeTooltipLocation();
                _RangeTooltip.ShowDropShadow = false;
                _RangeTooltip.Style = EffectiveStyle;
                _RangeTooltip.Text = GetRangeTooltipText();
                _RangeTooltip.ShowToolTip();
            }
            else
            {
                _RangeTooltip.FixedLocation = GetRangeTooltipLocation();
                _RangeTooltip.Text = GetRangeTooltipText();
            }
        }
        private Point GetRangeTooltipLocation()
        {
            Point p = Point.Empty;
            Control c = this.ContainerControl as Control;
            if (c != null)
            {
                Rectangle r = _RangeButtonMinBounds;
                if (_MouseDownPart == eRangeSliderPart.MaxRangeSlider)
                    r = _RangeButtonMaxBounds;
                r.Offset(this.Bounds.Location);
                Point pb = c.PointToScreen(r.Location);
                Point pi = c.PointToScreen(this.Bounds.Location);
                if (_SliderOrientation == eOrientation.Horizontal)
                {
                    p.X = pb.X;
                    if (_TicksPosition == eTicksPosition.Top)
                        p.Y = pi.Y - 18;
                    else
                        p.Y = pi.Y + this.Bounds.Height + 3;
                }
                else
                {
                    p.Y = pb.Y;
                    p.X = pi.X + this.Bounds.Width + 3;
                }
            }
            return p;
        }
        private string GetRangeTooltipText()
        {
            string s = string.Format(_RangeTooltipFormat, Value.Min, Value.Max);
            RangeTooltipEventArgs e = new RangeTooltipEventArgs(s);
            OnRangeTooltipText(e);
            return e.Tooltip;
        }
        /// <summary>
        /// Occurs when control is about to display the range tooltip and it allows you to customize tooltip
        /// </summary>
        [Description("Occurs when control is about to display the range tooltip and it allows you to customize tooltip")]
        public event RangeTooltipEventHandler RangeTooltipText;
        /// <summary>
        /// Raises GetRangeTooltipText event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnRangeTooltipText(RangeTooltipEventArgs e)
        {
            RangeTooltipEventHandler handler = RangeTooltipText;
            if (handler != null)
                handler(this, e);
        }
        private void HideRangeTooltip()
        {
            if (_RangeTooltip == null) return;
            _RangeTooltip.Hide();
            _RangeTooltip.Dispose();
            _RangeTooltip = null;
        }
        protected override bool ShowToolTips
        {
            get
            {
                return (_RangeTooltip == null);
            }
        }
        /// <summary>
        /// Gets the range part that is under specified client location.
        /// </summary>
        /// <param name="p">Location in parent control client coordinates.</param>
        /// <returns>Range part at specified location.</returns>
        public eRangeSliderPart HitTest(Point p)
        {
            Rectangle r = _RangeButtonMinBounds;
            r.Offset(this.Bounds.Location);
            if (r.Contains(p))
                return eRangeSliderPart.MinRangeSlider;

            r = _RangeButtonMaxBounds;
            r.Offset(this.Bounds.Location);
            if (r.Contains(p))
                return eRangeSliderPart.MaxRangeSlider;

            r = _TicksBounds;
            r.Offset(this.Bounds.Location);

            if (_SliderOrientation == eOrientation.Horizontal && p.X >= r.X && p.X <= r.Right)
                return eRangeSliderPart.TickArea;
            if (_SliderOrientation == eOrientation.Vertical && p.Y >= r.Y && p.Y <= r.Bottom)
                return eRangeSliderPart.TickArea;

            return eRangeSliderPart.None;
        }
        private int _Minimum = 0;
        /// <summary>
        /// Gets or sets the minimum value of the range of the control.
        /// </summary>
        [Description("Gets or sets the minimum value of the range of the control."), Category("Behavior"), DefaultValue(0)]
        public int Minimum
        {
            get { return _Minimum; }
            set
            {
                if (value != _Minimum)
                {
                    int oldValue = _Minimum;
                    _Minimum = value;
                    OnMinimumChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when Minimum property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnMinimumChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Minimum"));
            this.Refresh();
            OnAppearanceChanged();
        }

        private int _Maximum = 10;
        /// <summary>
        /// Gets or sets the maximum value of the range of the control.
        /// </summary>
        [Description("Gets or sets the maximum value of the range of the control."), Category("Behavior"), DefaultValue(10)]
        public int Maximum
        {
            get { return _Maximum; }
            set
            {
                if (value != _Maximum)
                {
                    int oldValue = _Maximum;
                    _Maximum = value;
                    OnMaximumChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when Maximum property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnMaximumChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Maximum"));
            this.Refresh();
            OnAppearanceChanged();
        }

        private static readonly RangeValue _DefaultRangeValue = new RangeValue(0, 10);
        private RangeValue _Value = _DefaultRangeValue;
        /// <summary>
        /// Gets or sets the range displayed by the control.
        /// </summary>
        [Description("Specifies range displayed by the control."), Category("Data")]
        public RangeValue Value
        {
            get { return _Value; }
            set
            {
                if (value.Min != _Value.Min || value.Max != _Value.Max)
                {
                    SetValue(value, eEventSource.Code);
                }
            }
        }
        /// <summary>
        /// Sets the range value.
        /// </summary>
        /// <param name="value">New Range value.</param>
        /// <param name="source">Source of the value change.</param>
        public bool SetValue(RangeValue value, eEventSource source)
        {
            RangeSliderValueChangingEventArgs ce = new RangeSliderValueChangingEventArgs(source, value);
            OnValueChanging(ce);
            if (ce.Cancel)
                return false;

            RangeValue oldValue = _Value;
            _Value = value;
            UpdateRangeButtonBounds();
            OnValueChanged(oldValue, value);
            OnValueChanged(new EventSourceArgs(source));
            this.Refresh();
            OnAppearanceChanged();

            if (_MouseDownPart != eRangeSliderPart.None && _ShowRangeTooltip)
                UpdateAndShowRangeTooltip();

            return true;
        }
        /// <summary>
        /// Called when Value property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnValueChanged(RangeValue oldValue, RangeValue newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Value"));
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeValue()
        {
            return _Value.Min != _DefaultRangeValue.Min || _Value.Max != _DefaultRangeValue.Max;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetValue()
        {
            Value = _DefaultRangeValue;
        }

        private bool _TicksVisible = true;
        /// <summary>
        /// Indicates whether tick lines are shown
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether tick lines are shown")]
        public bool TicksVisible
        {
            get { return _TicksVisible; }
            set
            {
                if (value != _TicksVisible)
                {
                    bool oldValue = _TicksVisible;
                    _TicksVisible = value;
                    OnTicksVisibleChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when TicksVisible property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnTicksVisibleChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("TicksVisible"));
            this.OnAppearanceChanged();
            this.Refresh();
        }

        private Rectangle _TicksBounds = Rectangle.Empty;
        /// <summary>
        /// Gets tick marks bounds.
        /// </summary>
        [Browsable(false)]
        public Rectangle TicksBounds
        {
            get
            {
                return _TicksBounds;
            }
        }
        private Rectangle _TicksBounds2 = Rectangle.Empty;
        /// <summary>
        /// Gets tick marks bounds for second marker is visible.
        /// </summary>
        [Browsable(false)]
        public Rectangle TicksBounds2
        {
            get
            {
                return _TicksBounds2;
            }
        }

        private Rectangle _RangeButtonMinBounds = Rectangle.Empty;
        /// <summary>
        /// Gets bounds of minimum range sliding button.
        /// </summary>
        [Browsable(false)]
        public Rectangle RangeButtonMinBounds
        {
            get
            {
                return _RangeButtonMinBounds;
            }
        }
        private Rectangle _RangeButtonMaxBounds = Rectangle.Empty;
        /// <summary>
        /// Gets bounds of maximum range sliding button.
        /// </summary>
        [Browsable(false)]
        public Rectangle RangeButtonMaxBounds
        {
            get
            {
                return _RangeButtonMaxBounds;
            }
        }

        private int _TicksStep = 1;
        /// <summary>
        /// Indicates tick display period
        /// </summary>
        [DefaultValue(1), Category("Appearance"), Description("Indicates tick display period")]
        public int TicksStep
        {
            get { return _TicksStep; }
            set
            {
                if (value != _TicksStep)
                {
                    int oldValue = _TicksStep;
                    _TicksStep = value;
                    OnTicksStepChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when TicksStep property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnTicksStepChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("TicksStep"));
            this.OnAppearanceChanged();
            this.Refresh();
        }

        private eOrientation _SliderOrientation = eOrientation.Horizontal;
        /// <summary>
        /// Gets or sets the slider orientation. Default value is horizontal.
        /// </summary>
        [DefaultValue(eOrientation.Horizontal), Category("Appearance"), Description("Indicates slider orientation.")]
        public eOrientation SliderOrientation
        {
            get { return _SliderOrientation; }
            set
            {
                if (_SliderOrientation != value)
                {
                    _SliderOrientation = value;
                    NeedRecalcSize = true;
                    this.Refresh();
                }
            }
        }

        private int _Width = 140;
        /// <summary>
        /// Gets or sets the width of the range slider in pixels. Value must be greater than 0. Default value is 140.
        /// </summary>
        [Browsable(true), DefaultValue(140), Category("Layout"), Description("Indicates the width of range slider in pixels.")]
        public int Width
        {
            get
            {
                return _Width;
            }
            set
            {
                if (_Width == value || value <= 0)
                    return;
                _Width = value;
                NeedRecalcSize = true;
                OnAppearanceChanged();
                this.Refresh();
            }
        }
        private int _Height = 24;
        /// <summary>
        /// Gets or sets the Height of the range slider in pixels. Value must be greater than 0. Default value is 24.
        /// </summary>
        [Browsable(true), DefaultValue(24), Category("Layout"), Description("Indicates the Height of range slider in pixels.")]
        public int Height
        {
            get
            {
                return _Height;
            }
            set
            {
                if (_Height == value || value <= 0)
                    return;
                _Height = value;
                NeedRecalcSize = true;
                OnAppearanceChanged();
                this.Refresh();
            }
        }

        private static readonly Size DefaultRangeButtonSize = new Size(7, 17);
        private Size _RangeButtonSize = DefaultRangeButtonSize;
        /// <summary>
        /// Indicates the size of the range change buttons.
        /// </summary>
        [Description("Indicates the size of the range change buttons."), Category("Appearance")]
        public Size RangeButtonSize
        {
            get { return _RangeButtonSize; }
            set
            {
                if (value != _RangeButtonSize)
                {
                    Size oldValue = _RangeButtonSize;
                    _RangeButtonSize = value;
                    OnRangeButtonSizeChanged(oldValue, value);
                }
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeRangeButtonSize()
        {
            return _RangeButtonSize != DefaultRangeButtonSize;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetRangeButtonSize()
        {
            RangeButtonSize = DefaultRangeButtonSize;
        }
        /// <summary>
        /// Called when RangeButtonSize property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnRangeButtonSizeChanged(Size oldValue, Size newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("RangeButtonSize"));
            NeedRecalcSize = true;
            OnAppearanceChanged();
            this.Refresh();
        }

        private eTicksPosition _TicksPosition = eTicksPosition.Bottom;
        /// <summary>
        /// Specifies the ticks position inside of Range Slider.
        /// </summary>
        [DefaultValue(eTicksPosition.Bottom), Category("Appearance"), Description("Specifies the ticks position inside of Range Slider.")]
        public eTicksPosition TicksPosition
        {
            get { return _TicksPosition; }
            set
            {
                if (value != _TicksPosition)
                {
                    eTicksPosition oldValue = _TicksPosition;
                    _TicksPosition = value;
                    OnTicksPositionChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when TicksPosition property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnTicksPositionChanged(eTicksPosition oldValue, eTicksPosition newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("TicksPosition"));
            NeedRecalcSize = true;
            OnAppearanceChanged();
            this.Refresh();
        }

        [Browsable(false)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        private int _MinimumAbsoluteRange = 0;
        /// <summary>
        /// Specifies minimum absolute range that user can select. Absolute range is defined as Abs(Value.Max-Value.Min) Applies to user performed selection through mouse only.
        /// </summary>
        [DefaultValue(0), Category("Behavior"), Description("Specifies minimum absolute range that user can select. Absolute range is defined as Abs(Value.Max-Value.Min) Applies to user performed selection through mouse only.")]
        public int MinimumAbsoluteRange
        {
            get { return _MinimumAbsoluteRange; }
            set
            {
                if (value != _MinimumAbsoluteRange)
                {
                    int oldValue = _MinimumAbsoluteRange;
                    _MinimumAbsoluteRange = value;
                    OnMinimumAbsoluteRangeChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when MinimumAbsoluteRange property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnMinimumAbsoluteRangeChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("MinimumAbsoluteRange"));
        }

        private string _RangeTooltipFormat = "Min: {0} - Max: {1}";
        /// <summary>
        /// Gets or sets the string that is used to format the range value to be displayed while user moves the range buttons. Value set here is used in string.Format(RangeTooltipFormat, Value.Min, Value.Max).
        /// </summary>
        [DefaultValue("{0} - {1}"), Description("Indicate string that is used to format the range value to be displayed while user moves the range buttons. Value set here is used in string.Format(RangeTooltipFormat, Value.Min, Value.Max)."), Localizable(true), Category("Behavior")]
        public string RangeTooltipFormat
        {
            get { return _RangeTooltipFormat; }
            set
            {
                if (value != _RangeTooltipFormat)
                {
                    string oldValue = _RangeTooltipFormat;
                    _RangeTooltipFormat = value;
                    OnRangeTooltipFormatChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when RangeTooltipFormat property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnRangeTooltipFormatChanged(string oldValue, string newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("RangeTooltipFormat"));
        }

        private bool _ShowRangeTooltip = true;
        /// <summary>
        /// Specifies whether range tooltip is shown while user is changing the range
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Specifies whether range tooltip is shown while user is changing the range")]
        public bool ShowRangeTooltip
        {
            get { return _ShowRangeTooltip; }
            set
            {
                _ShowRangeTooltip = value;
            }
        }

        private Image _MinRangeSliderImage = null;
        /// <summary>
        /// Indicates image to be used as minimum range slider button instead of built-in button. Image is scaled to size set by RangeButtonSize.
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates image to be used as minimum range slider button instead of built-in button. Image is scaled to size set by RangeButtonSize.")]
        public Image MinRangeSliderImage
        {
            get { return _MinRangeSliderImage; }
            set
            {
                if (value != _MinRangeSliderImage)
                {
                    Image oldValue = _MinRangeSliderImage;
                    _MinRangeSliderImage = value;
                    OnMinRangeSliderImageChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when MinRangeSliderImage property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnMinRangeSliderImageChanged(Image oldValue, Image newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("MinRangeSliderImage"));
            this.OnAppearanceChanged();
            this.Refresh();
        }

        private Image _MaxRangeSliderImage = null;
        /// <summary>
        /// Indicates image to be used as maximum range slider button instead of built-in button. Image is scaled to size set by RangeButtonSize.
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates image to be used as maximum range slider button instead of built-in button. Image is scaled to size set by RangeButtonSize.")]
        public Image MaxRangeSliderImage
        {
            get { return _MaxRangeSliderImage; }
            set
            {
                if (value != _MaxRangeSliderImage)
                {
                    Image oldValue = _MaxRangeSliderImage;
                    _MaxRangeSliderImage = value;
                    OnMaxRangeSliderImageChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when MaxRangeSliderImage property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnMaxRangeSliderImageChanged(Image oldValue, Image newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("MinRangeSliderImage"));
            this.OnAppearanceChanged();
            this.Refresh();
        }

        private int _RangeLineHeight = 7;
        /// <summary>
        /// Specifies the height of the range line
        /// </summary>
        [DefaultValue(7), Category("Appearance"), Description("Specifies the height of the range line")]
        public int RangeLineHeight
        {
            get { return _RangeLineHeight; }
            set
            {
                if (value != _RangeLineHeight)
                {
                    int oldValue = _RangeLineHeight;
                    _RangeLineHeight = value;
                    OnRangeLineHeightChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when RangeLineHeight property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnRangeLineHeightChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("RangeLineHeight"));
            this.OnAppearanceChanged();
            this.Refresh();
        }

        private bool _ClientClickRangeChangeEnabled = true;
        /// <summary>
        /// Indicates whether clicking the area outside of the range change buttons moves the range change button to the clicked location if possible thus allowing range change.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether clicking the area outside of the range change buttons moves the range change button to the clicked location if possible thus allowing range change.")]
        public bool ClientClickRangeChangeEnabled
        {
            get { return _ClientClickRangeChangeEnabled; }
            set
            {
                _ClientClickRangeChangeEnabled = value;
            }
        }

        private Color _RangeValueColor = Color.Empty;
        /// <summary>
        /// Gets or sets the color of the range value.
        /// </summary>
        [Category("Appearance"), Description("Indicates color of range value.")]
        public Color RangeValueColor
        {
            get { return _RangeValueColor; }
            set
            {
                _RangeValueColor = value;
                this.OnAppearanceChanged();
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets whether property should be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeRangeValueColor()
        {
            return !_RangeValueColor.IsEmpty;
        }
        /// <summary>
        /// Resets property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetRangeValueColor()
        {
            this.RangeValueColor = Color.Empty;
        }
        #endregion
    }

    #region eTicksPosition
    /// <summary>
    /// Specifies the ticks position inside of Range Slider.
    /// </summary>
    public enum eTicksPosition
    {
        /// <summary>
        /// Ticks are displayed on top of range sliders.
        /// </summary>
        Top,
        /// <summary>
        /// Ticks are displayed on bottom of range sliders.
        /// </summary>
        Bottom,
        /// <summary>
        /// Ticks are displayed on top and bottom of range sliders.
        /// </summary>
        TopAndBottom
    }
    #endregion

    #region RangeValue
    /// <summary>
    /// Defines value for the range slider controls.
    /// </summary>
    [Serializable, TypeConverter(typeof(RangeValueConverter))]
    public struct RangeValue
    {
        /// <summary>
        /// Gets or sets the range minimum value.
        /// </summary>
        [DefaultValue(0)]
        public int Min;
        /// <summary>
        /// Gets or sets the range maximum value.
        /// </summary>
        [DefaultValue(0)]
        public int Max;

        public RangeValue(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return string.Format("Min={0}, Max={1}", Min, Max);
        }
    }
    #endregion

    #region RangeValueConverter
    public class RangeValueConverter : TypeConverter
    {
        // Methods
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            if (str == null)
            {
                return base.ConvertFrom(context, culture, value);
            }
            string str2 = str.Trim();
            if (str2.Length == 0)
            {
                return null;
            }
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            char ch = culture.TextInfo.ListSeparator[0];
            string[] strArray = str2.Split(new char[] { ch });
            int[] numArray = new int[strArray.Length];
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = (int)converter.ConvertFromString(context, culture, strArray[i]);
            }
            if (numArray.Length != 2)
            {
                throw new ArgumentException("Text Parsing Failed, only 2 comma separated values are accepted");
            }
            return new RangeValue(numArray[0], numArray[1]);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (value is RangeValue)
            {
                if (destinationType == typeof(string))
                {
                    RangeValue range = (RangeValue)value;
                    if (culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }
                    string separator = culture.TextInfo.ListSeparator + " ";
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
                    string[] strArray = new string[2];
                    int num = 0;
                    strArray[num++] = converter.ConvertToString(context, culture, range.Min);
                    strArray[num++] = converter.ConvertToString(context, culture, range.Max);
                    return string.Join(separator, strArray);
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    RangeValue range2 = (RangeValue)value;
                    ConstructorInfo constructor = typeof(RangeValue).GetConstructor(new Type[] { typeof(int), typeof(int) });
                    if (constructor != null)
                    {
                        return new InstanceDescriptor(constructor, new object[] { range2.Min, range2.Max });
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues");
            }
            object obj2 = propertyValues["Min"];
            object obj3 = propertyValues["Max"];
            if (((obj2 == null) || (obj3 == null)) || (!(obj2 is int) || !(obj3 is int)))
            {
                throw new ArgumentException("Property Value Invalid Entry");
            }
            return new RangeValue((int)obj2, (int)obj3);
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(RangeValue), attributes).Sort(new string[] { "Min", "Max" });
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    #endregion

    #region RangeSliderItemRendererEventArgs
    /// <summary>
    /// Provides data for the Slider item rendering events.
    /// </summary>
    public class RangeSliderItemRendererEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the reference to the item being rendered.
        /// </summary>
        public RangeSliderItem SliderItem = null;

        /// <summary>
        /// Gets or sets the reference to graphics object.
        /// </summary>
        public Graphics Graphics = null;

        internal ItemPaintArgs ItemPaintArgs = null;

        /// <summary>
        /// Creates new instance of the object and initializes it with default values.
        /// </summary>
        /// <param name="item">Reference to the Range Slider item being rendered.</param>
        /// <param name="g">Reference to the graphics object.</param>
        public RangeSliderItemRendererEventArgs(RangeSliderItem item, Graphics g)
        {
            this.SliderItem = item;
            this.Graphics = g;
        }
    }
    #endregion

    #region RangeSliderValueChangingEventArgs
    /// <summary>
    /// Defines delegate for RangeSliderValueChanging event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RangeSliderValueChangingEventHandler(object sender, RangeSliderValueChangingEventArgs e);
    /// <summary>
    /// Provides information for RangeSliderValueChanging event.
    /// </summary>
    public class RangeSliderValueChangingEventArgs : EventSourceArgs
    {
        /// <summary>
        ///  New range value.
        /// </summary>
        public readonly RangeValue NewValue;
        /// <summary>
        /// Set to true to cancel the value changing.
        /// </summary>
        public bool Cancel = false;
        /// <summary>
        /// Initializes a new instance of the RangeSliderValueChangingEventArgs class.
        /// </summary>
        /// <param name="newValue"></param>
        public RangeSliderValueChangingEventArgs(eEventSource source, RangeValue newValue)
            : base(source)
        {
            NewValue = newValue;
        }
    }
    #endregion

    #region eRangeSliderPart
    /// <summary>
    /// Defines the slider item parts.
    /// </summary>
    public enum eRangeSliderPart
    {
        /// <summary>
        /// Indicates no part.
        /// </summary>
        None,
        /// <summary>
        /// Indicates the minimum range slider button.
        /// </summary>
        MinRangeSlider,
        /// <summary>
        /// Indicates the maximum range slider button.
        /// </summary>
        MaxRangeSlider,
        /// <summary>
        /// Indicates the track area part of the control.
        /// </summary>
        TickArea
    }
    #endregion


    #region RangeTooltipEventHandler
    /// <summary>
    /// Defines delegate for RangeSliderValueChanging event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RangeTooltipEventHandler(object sender, RangeTooltipEventArgs e);
    /// <summary>
    /// Provides information for GetRangeTooltip event.
    /// </summary>
    public class RangeTooltipEventArgs : EventArgs
    {
        /// <summary>
        ///  Gets or sets the tooltip to display.
        /// </summary>
        public string Tooltip;
        /// <summary>
        /// Initializes a new instance of the RangeTooltipEventArgs class.
        /// </summary>
        /// <param name="tooltip"></param>
        public RangeTooltipEventArgs(string tooltip)
        {
            Tooltip = tooltip;
        }
    }
    #endregion

}
