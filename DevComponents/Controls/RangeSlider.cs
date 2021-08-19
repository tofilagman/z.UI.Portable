using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Represents Range Slider Control.
    /// </summary>
    [ToolboxBitmap(typeof(RangeSlider), "Controls.RangeSlider.ico"), ToolboxItem(true), DefaultEvent("ValueChanged"), ComVisible(false), Designer("DevComponents.DotNetBar.Design.RangeSliderDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf")]
    public class RangeSlider : BaseItemControl
    {
        #region Private Variables
        private RangeSliderItem _Slider = null;
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
        #endregion

        #region Constructor, Dispose
        public RangeSlider()
        {
            _Slider = new RangeSliderItem();
            _Slider.Style = eDotNetBarStyle.StyleManagerControlled;
            _Slider.ValueChanging += new RangeSliderValueChangingEventHandler(Slider_ValueChanging);
            _Slider.ValueChanged += new EventHandler(Slider_ValueChanged);
            _Slider.RangeTooltipText += new RangeTooltipEventHandler(Slider_RangeTooltipText);
            this.HostItem = _Slider;
        }

        void Slider_RangeTooltipText(object sender, RangeTooltipEventArgs e)
        {
            OnRangeTooltipText(e);
        }

        void Slider_ValueChanged(object sender, EventArgs e)
        {
            OnValueChanged(e);
        }

        void Slider_ValueChanging(object sender, RangeSliderValueChangingEventArgs e)
        {
            OnValueChanging(e);
        }

        /// <summary>
        /// Forces the button to perform internal layout.
        /// </summary>
        public override void RecalcLayout()
        {
            Rectangle r = GetItemBounds();
            _Slider.Width = r.Width;
            _Slider.Height = r.Height;
            base.RecalcLayout();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            this.RecalcLayout();
            base.OnHandleCreated(e);
        }

        //protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        //{
        //    if (keyData == System.Windows.Forms.Keys.Left)
        //    {
        //        m_Slider.Increment(-m_Slider.Step, eEventSource.Keyboard);
        //        return true;
        //    }
        //    else if (keyData == System.Windows.Forms.Keys.Right)
        //    {
        //        m_Slider.Increment(m_Slider.Step, eEventSource.Keyboard);
        //        return true;
        //    }
        //    return base.ProcessCmdKey(ref msg, keyData);
        //}

        /// <summary>
        /// Gets the RangeSliderItem.
        /// </summary>
        internal RangeSliderItem SliderItem
        {
            get { return (_Slider); }
        }

        /// <summary>
        /// Indicates whether clicking the area outside of the range change buttons moves the range change button to the clicked location if possible thus allowing range change.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether clicking the area outside of the range change buttons moves the range change button to the clicked location if possible thus allowing range change.")]
        public bool ClientClickRangeChangeEnabled
        {
            get { return _Slider.ClientClickRangeChangeEnabled; }
            set
            {
                _Slider.ClientClickRangeChangeEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of the range of the control.
        /// </summary>
        [Description("Gets or sets the maximum value of the range of the control."), Category("Behavior"), DefaultValue(10)]
        public int Maximum
        {
            get { return _Slider.Maximum; }
            set
            {
                _Slider.Maximum = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum value of the range of the control.
        /// </summary>
        [Description("Gets or sets the minimum value of the range of the control."), Category("Behavior"), DefaultValue(0)]
        public int Minimum
        {
            get { return _Slider.Minimum; }
            set
            {
                _Slider.Minimum = value;
            }
        }

        /// <summary>
        /// Indicates image to be used as maximum range slider button instead of built-in button. Image is scaled to size set by RangeButtonSize.
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates image to be used as maximum range slider button instead of built-in button. Image is scaled to size set by RangeButtonSize.")]
        public Image MaxRangeSliderImage
        {
            get { return _Slider.MaxRangeSliderImage; }
            set
            {
                _Slider.MaxRangeSliderImage = value;
            }
        }

        /// <summary>
        /// Indicates image to be used as minimum range slider button instead of built-in button. Image is scaled to size set by RangeButtonSize.
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates image to be used as minimum range slider button instead of built-in button. Image is scaled to size set by RangeButtonSize.")]
        public Image MinRangeSliderImage
        {
            get { return _Slider.MinRangeSliderImage; }
            set
            {
                _Slider.MinRangeSliderImage = value;
            }
        }

        /// <summary>
        /// Specifies minimum absolute range that user can select. Absolute range is defined as Abs(Value.Max-Value.Min) Applies to user performed selection through mouse only.
        /// </summary>
        [DefaultValue(0), Category("Behavior"), Description("Specifies minimum absolute range that user can select. Absolute range is defined as Abs(Value.Max-Value.Min) Applies to user performed selection through mouse only.")]
        public int MinimumAbsoluteRange
        {
            get { return _Slider.MinimumAbsoluteRange; }
            set
            {
                _Slider.MinimumAbsoluteRange = value;
            }
        }

        /// <summary>
        /// Gets current part that is pressed using mouse left button.
        /// </summary>
        [Browsable(false)]
        public eRangeSliderPart MouseDownPart
        {
            get { return _Slider.MouseDownPart; }
        }

        /// <summary>
        /// Gets mouse over part.
        /// </summary>
        [Browsable(false)]
        public eRangeSliderPart MouseOverPart
        {
            get { return _Slider.MouseOverPart; }
        }

        /// <summary>
        /// Gets bounds of maximum range sliding button.
        /// </summary>
        [Browsable(false)]
        public Rectangle RangeButtonMaxBounds
        {
            get
            {
                return _Slider.RangeButtonMaxBounds;
            }
        }

        /// <summary>
        /// Gets bounds of minimum range sliding button.
        /// </summary>
        [Browsable(false)]
        public Rectangle RangeButtonMinBounds
        {
            get
            {
                return _Slider.RangeButtonMinBounds;
            }
        }

        /// <summary>
        /// Indicates the size of the range change buttons.
        /// </summary>
        [Description("Indicates the size of the range change buttons."), Category("Appearance")]
        public Size RangeButtonSize
        {
            get { return _Slider.RangeButtonSize; }
            set
            {
                _Slider.RangeButtonSize = value;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeRangeButtonSize()
        {
            return _Slider.ShouldSerializeRangeButtonSize();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetRangeButtonSize()
        {
            _Slider.ResetRangeButtonSize();
        }

        /// <summary>
        /// Specifies the height of the range line
        /// </summary>
        [DefaultValue(7), Category("Appearance"), Description("Specifies the height of the range line")]
        public int RangeLineHeight
        {
            get { return _Slider.RangeLineHeight; }
            set
            {
                _Slider.RangeLineHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the string that is used to format the range value to be displayed while user moves the range buttons. Value set here is used in string.Format(RangeTooltipFormat, Value.Min, Value.Max).
        /// </summary>
        [DefaultValue("{0} - {1}"), Description("Indicate string that is used to format the range value to be displayed while user moves the range buttons. Value set here is used in string.Format(RangeTooltipFormat, Value.Min, Value.Max)."), Localizable(true), Category("Behavior")]
        public string RangeTooltipFormat
        {
            get { return _Slider.RangeTooltipFormat; }
            set
            {
                _Slider.RangeTooltipFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the range value.
        /// </summary>
        [Category("Appearance"), Description("Indicates color of range value.")]
        public Color RangeValueColor
        {
            get { return _Slider.RangeValueColor; }
            set
            {
                _Slider.RangeValueColor = value;
            }
        }
        /// <summary>
        /// Gets whether property should be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeRangeValueColor()
        {
            return _Slider.ShouldSerializeRangeValueColor();
        }
        /// <summary>
        /// Resets property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetRangeValueColor()
        {
            _Slider.ResetRangeValueColor();
        }

        /// <summary>
        /// Specifies whether range tooltip is shown while user is changing the range
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Specifies whether range tooltip is shown while user is changing the range")]
        public bool ShowRangeTooltip
        {
            get { return _Slider.ShowRangeTooltip; }
            set
            {
                _Slider.ShowRangeTooltip = value;
            }
        }

        /// <summary>
        /// Gets or sets the slider orientation. Default value is horizontal.
        /// </summary>
        [DefaultValue(eOrientation.Horizontal), Category("Appearance"), Description("Indicates slider orientation.")]
        public eOrientation SliderOrientation
        {
            get { return _Slider.SliderOrientation; }
            set
            {
                _Slider.SliderOrientation = value;
            }
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

        /// <summary>
        /// Gets tick marks bounds.
        /// </summary>
        [Browsable(false)]
        public Rectangle TicksBounds
        {
            get
            {
                return _Slider.TicksBounds;
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
                return _Slider.TicksBounds2;
            }
        }

        /// <summary>
        /// Specifies the ticks position inside of Range Slider.
        /// </summary>
        [DefaultValue(eTicksPosition.Bottom), Category("Appearance"), Description("Specifies the ticks position inside of Range Slider.")]
        public eTicksPosition TicksPosition
        {
            get { return _Slider.TicksPosition; }
            set
            {
                _Slider.TicksPosition = value;
            }
        }

        /// <summary>
        /// Indicates tick display period
        /// </summary>
        [DefaultValue(1), Category("Appearance"), Description("Indicates tick display period")]
        public int TicksStep
        {
            get { return _Slider.TicksStep; }
            set
            {
                _Slider.TicksStep = value;
            }
        }

        /// <summary>
        /// Indicates whether tick lines are shown
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether tick lines are shown")]
        public bool TicksVisible
        {
            get { return _Slider.TicksVisible; }
            set
            {
                _Slider.TicksVisible = value;
            }
        }

        /// <summary>
        /// Gets or sets the range displayed by the control.
        /// </summary>
        [Description("Specifies range displayed by the control."), Category("Data")]
        public RangeValue Value
        {
            get { return _Slider.Value; }
            set
            {
                _Slider.Value = value;
            }
        }
        #endregion
    }
}
