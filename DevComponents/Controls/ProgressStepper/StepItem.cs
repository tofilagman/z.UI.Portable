using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Represents a step item which is used to show single step in multi-step progress control.
    /// </summary>
    [ToolboxItem(false), DefaultEvent("Click")]
    public class StepItem : BaseItem
    {
        #region Constructor, Copy
        /// <summary>
        /// Creates new instance of StepItem.
        /// </summary>
        public StepItem() : this("", "") { }
        /// <summary>
        /// Creates new instance of StepItem and assigns the name to it.
        /// </summary>
        /// <param name="sItemName">Item name.</param>
        public StepItem(string sItemName) : this(sItemName, "") { }
        /// <summary>
        /// Creates new instance of StepItem and assigns the name and text to it.
        /// </summary>
        /// <param name="sItemName">Item name.</param>
        /// <param name="ItemText">item text.</param>
        public StepItem(string sItemName, string ItemText)
            : base(sItemName, ItemText)
        {
            //this.ClickRepeatInterval = 200;
            this.MouseUpNotification = true;
            //this.MouseDownCapture = true;
            _Padding.PropertyChanged += PaddingPropertyChanged;
        }

        /// <summary>
        /// Returns copy of the item.
        /// </summary>
        public override BaseItem Copy()
        {
            StepItem objCopy = new StepItem(m_Name);
            this.CopyToItem(objCopy);
            return objCopy;
        }

        /// <summary>
        /// Copies the StepItem specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New StepItem instance.</param>
        internal void InternalCopyToItem(StepItem copy)
        {
            CopyToItem(copy);
        }

        /// <summary>
        /// Copies the StepItem specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New StepItem instance.</param>
        protected override void CopyToItem(BaseItem copy)
        {
            StepItem c = copy as StepItem;
            c.Symbol = _Symbol;
            c.SymbolSet = _SymbolSet;
            c.SymbolColor = _SymbolColor;
            c.SymbolSize = _SymbolSize;

            base.CopyToItem(c);


        }

        protected override void Dispose(bool disposing)
        {
            if (_ItemPath != null)
            {
                _ItemPath.Dispose();
                _ItemPath = null;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Internal Implementation
        public override void Paint(ItemPaintArgs p)
        {
            Rendering.BaseRenderer renderer = p.Renderer;
            if (renderer != null)
            {
                StepItemRendererEventArgs e = new StepItemRendererEventArgs(this, p.Graphics);
                e.ItemPaintArgs = p;
                renderer.DrawStepItem(e);
            }
            else
            {
                Rendering.StepItemPainter painter = PainterFactory.CreateStepItemPainter(this);
                if (painter != null)
                {
                    StepItemRendererEventArgs e = new StepItemRendererEventArgs(this, p.Graphics);
                    e.ItemPaintArgs = p;
                    painter.Paint(e);
                }
            }

            if (this.DesignMode && this.Focused)
            {
                Rectangle r = Rectangle.Round(_ItemPath.GetBounds());
                r.Inflate(-1, -1);
                DesignTime.DrawDesignTimeSelection(p.Graphics, r, p.Colors.ItemDesignTimeBorder);
            }

            this.DrawInsertMarker(p.Graphics);
        }

        public override void RecalcSize()
        {
            Font font = GetFont(null);
            Size size = Size.Empty;

            Control objCtrl = this.ContainerControl as Control;
            if (objCtrl == null || objCtrl.Disposing || objCtrl.IsDisposed)
                return;
            Graphics g = BarFunctions.CreateGraphics(objCtrl);
            if (g == null) return;
            try
            {
                if (!string.IsNullOrEmpty(_Symbol))
                {
                    _ActualSymbolSize = GetSymbolSize(g);
                    size = _ActualSymbolSize;
                }
                else if (_Image != null)
                {
                    size = _Image.Size;
                }
                if (!string.IsNullOrEmpty(this.Text))
                {
                    Size textSize = ButtonItemLayout.MeasureItemText(this, g, 0, objCtrl.Font, eTextFormat.Default, objCtrl.RightToLeft == RightToLeft.Yes);
                    size.Width += textSize.Width;
                    size.Height = Math.Max(size.Height, textSize.Height);
                    if (_Image != null || !string.IsNullOrEmpty(_Symbol))
                        size.Width += Dpi.Width(_ImageTextSpacing);
                }
                else if (string.IsNullOrEmpty(_Symbol) && _Image == null)
                    size = new System.Drawing.Size(Dpi.Width16, Dpi.Height16);

                size.Width += GetPointerSize();
                if(!_IsFirst && !_IsLast)
                    size.Width += Dpi.Width(GetPointerSize());

                size.Width += Dpi.Width(_Padding.Horizontal);
                size.Height += Dpi.Height(_Padding.Vertical);
                
                base.RecalcSize();
            }
            finally
            {
                g.Dispose();
            }

            if (!_MinimumSize.IsEmpty)
            {
                if (size.Width < _MinimumSize.Width) size.Width = _MinimumSize.Width;
                if (size.Height < _MinimumSize.Height) size.Height = _MinimumSize.Height;
            }

            m_Rect.Size = size;
        }

        internal int GetPointerSize()
        {
            if (this.Parent is StepItemContainer)
                return ((StepItemContainer)this.Parent).PointerSize;
            return 10;
        }

        private Size GetSymbolSize(Graphics g)
        {
            Size symbolSize = Size.Empty;
            if (g == null || string.IsNullOrEmpty(_Symbol)) return symbolSize;
            Font symFont = Symbols.GetFont(this.SymbolSize, this.SymbolSet);
            symbolSize = TextDrawing.MeasureString(g, "\uF00A", symFont); // Need to do this to get consistent size for the symbol since they are not all the same width we pick widest
            int descent = (int)Math.Ceiling((symFont.FontFamily.GetCellDescent(symFont.Style) *
                symFont.Size / symFont.FontFamily.GetEmHeight(symFont.Style)));
            symbolSize.Height -= descent;
            return symbolSize;
        }

        private Size _ActualSymbolSize = Size.Empty;
        internal Size ActualSymbolSize
        {
            get
            {
                return _ActualSymbolSize;
            }
        }

        /// <summary>
        /// Returns the Font object to be used for drawing the item text.
        /// </summary>
        /// <returns>Font object.</returns>
        private Font GetFont(ItemPaintArgs pa)
        {
            System.Drawing.Font font = null;

            if (pa != null)
                font = pa.Font;

            if (font == null)
            {
                System.Windows.Forms.Control objCtrl = null;
                if (pa != null)
                    objCtrl = pa.ContainerControl;
                if (objCtrl == null)
                    objCtrl = this.ContainerControl as System.Windows.Forms.Control;
                if (objCtrl != null && objCtrl.Font != null)
                    font = (Font)objCtrl.Font;
                else
                    font = SystemFonts.DefaultFont; // (Font)System.Windows.Forms.SystemInformation.MenuFont;
            }

            return font;
        }

        private GraphicsPath _ItemPath = null;
        /// <summary>
        /// Gets the render path of the item.
        /// </summary>
        [Browsable(false)]
        public GraphicsPath ItemPath
        {
            get { return _ItemPath; }
            internal set
            {
                if (_ItemPath != null)
                    _ItemPath.Dispose();
                _ItemPath = value;
            }
        }

        private bool _IsFirst;
        private int _Minimum = 0;
        /// <summary>
        /// Gets or sets the minimum value of the range of the control.
        /// </summary>
        [Browsable(true), Description("Gets or sets the minimum value of the range of the control."), Category("Behavior"), DefaultValue(0)]
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
            this.OnAppearanceChanged();
            this.Refresh();
        }

        private int _Maximum = 100;
        [Browsable(true), Description("Gets or sets the maximum value of the range of the control."), Category("Behavior"), DefaultValue(100)]
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
            this.OnAppearanceChanged();
            this.Refresh();
        }

        private int _Value = 0;
        [Browsable(true), Description("Gets or sets the current position of the progress bar."), Category("Behavior"), DefaultValue(0)]
        public int Value
        {
            get { return _Value; }
            set
            {
                if (value != _Value)
                {
                    int oldValue = _Value;
                    _Value = value;
                    OnValueChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when Value property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnValueChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Value"));
            this.OnAppearanceChanged();
            this.Refresh();
        }

        private Color _SymbolColor = Color.Empty;
        /// <summary>
        /// Gets or sets the color of the Symbol.
        /// </summary>
        [Category("Appearance"), Description("Indicates color of the Symbol.")]
        public Color SymbolColor
        {
            get { return _SymbolColor; }
            set { _SymbolColor = value; this.Refresh(); }
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
            //OnPropertyChanged(new PropertyChangedEventArgs("Symbol"));
            NeedRecalcSize = true;
            OnAppearanceChanged();
            this.Refresh();
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
            NeedRecalcSize = true;
            OnAppearanceChanged();
            this.Refresh();
        }

        private float _SymbolSize = 13f;
        /// <summary>
        /// Indicates the size of the symbol in points.
        /// </summary>
        [DefaultValue(12f), Category("Appearance"), Description("Indicates the size of the symbol in points.")]
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
            //OnPropertyChanged(new PropertyChangedEventArgs("SymbolSize"));
            NeedRecalcSize = true;
            OnAppearanceChanged();
            this.Refresh();
        }

        private Image _Image = null;
        /// <summary>
        /// Indicates the image that is displayed next to the item text label.
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates the image that is displayed next to the item text label.")]
        public Image Image
        {
            get { return _Image; }
            set
            {
                if (value != _Image)
                {
                    Image oldValue = _Image;
                    _Image = value;
                    OnImageChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when Image property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnImageChanged(Image oldValue, Image newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Image"));
            NeedRecalcSize = true;
            OnAppearanceChanged();
            this.Refresh();
        }

        /// <summary>
        /// Gets or sets whether this is first item in StepControl.
        /// </summary>
        internal bool IsFirst
        {
            get { return _IsFirst; }
            set
            {
                _IsFirst = value;
            }
        }

        private bool _IsLast = false;
        /// <summary>
        /// Gets or sets whether this is laste item in StepControl.
        /// </summary>
        internal bool IsLast
        {
            get { return _IsLast; }
            set
            {
                _IsLast = value;
            }
        }

        private Size _MinimumSize = Size.Empty;
        /// <summary>
        /// Indicates minimum size of the item
        /// </summary>
        [Category("Appearance"), Description("Indicates minimum size of the item")]
        public Size MinimumSize
        {
            get { return _MinimumSize; }
            set
            {
                if (value != _MinimumSize)
                {
                    Size oldValue = _MinimumSize;
                    _MinimumSize = value;
                    OnMinimumSizeChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when MinimumSize property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnMinimumSizeChanged(Size oldValue, Size newValue)
        {
            // OnPropertyChanged(new PropertyChangedEventArgs("MinimumSize"));
            NeedRecalcSize = true;
            OnAppearanceChanged();
            this.Refresh();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeMinimumSize()
        {
            return !_MinimumSize.IsEmpty;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetMinimumSize()
        {
            this.Size = Size.Empty;
        }

        private bool _HotTracking = true;
        /// <summary>
        /// Specifies whether item changes its appearance when mouse is moved over the item
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Specifies whether item changes its appearance when mouse is moved over the item")]
        public bool HotTracking
        {
            get { return _HotTracking; }
            set
            {
                if (value != _HotTracking)
                {
                    bool oldValue = _HotTracking;
                    _HotTracking = value;
                    OnHotTrackingChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when HotTracking property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnHotTrackingChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("HotTracking"));
        }

        private bool _MouseOver = false, _MouseDown = false;
        public override void InternalMouseEnter()
        {
            base.InternalMouseEnter();
            if (!this.DesignMode)
            {
                _MouseOver = true;
                if (this.GetEnabled() && _HotTracking)
                    this.Refresh();
            }
        }

        public override void InternalMouseLeave()
        {
            base.InternalMouseLeave();
            if (!this.DesignMode)
            {
                _MouseOver = false;
                _MouseDown = false;
                if (this.GetEnabled() && _HotTracking)
                    this.Refresh();
            }
        }

        public override void InternalMouseDown(MouseEventArgs objArg)
        {
            base.InternalMouseDown(objArg);
            if (objArg.Button == MouseButtons.Left && !this.DesignMode)
            {
                _MouseDown = true;
                if (this.GetEnabled() && _HotTracking)
                    this.Refresh();
            }
        }

        public override void InternalMouseUp(MouseEventArgs objArg)
        {
            base.InternalMouseUp(objArg);

            if (_MouseDown && !this.DesignMode)
            {
                _MouseDown = false;
                if (this.GetEnabled() && _HotTracking)
                    this.Refresh();
            }
        }

        /// <summary>
        /// Gets whether mouse is over the item.
        /// </summary>
        [Browsable(false)]
        public bool IsMouseOver
        {
            get { return _MouseOver; }
            internal set { _MouseOver = value; }
        }

        /// <summary>
        /// Gets whether left mouse button is pressed on the item.
        /// </summary>
        [Browsable(false)]
        public bool IsMouseDown
        {
            get { return _MouseDown; }
            internal set { _MouseDown = value; }
        }


        private int _ImageTextSpacing = 4;
        /// <summary>
        /// Indicates the spacing between image and text.
        /// </summary>
        [DefaultValue(4), Category("Appearance"), Description("Indicates the spacing between image and text.")]
        public int ImageTextSpacing
        {
            get { return _ImageTextSpacing; }
            set
            {
                if (value != _ImageTextSpacing)
                {
                    int oldValue = _ImageTextSpacing;
                    _ImageTextSpacing = value;
                    OnImageTextSpacingChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when ImageTextSpacing property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnImageTextSpacingChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("ImageTextSpacing"));
            NeedRecalcSize = true;
            OnAppearanceChanged();
            this.Refresh();
        }

        private const int DefaultPadding = 4;
        private Padding _Padding = new Padding(DefaultPadding);
        /// <summary>
        /// Gets or sets padding around content of the item.
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("Gets or sets padding around content of the item."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Padding Padding
        {
            get { return _Padding; }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializePadding()
        {
            return _Padding.Bottom != DefaultPadding || _Padding.Top != DefaultPadding || _Padding.Left != DefaultPadding || _Padding.Right != DefaultPadding;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetPadding()
        {
            _Padding.All = DefaultPadding;
        }
        private void PaddingPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NeedRecalcSize = true;
            this.Refresh();
        }

        private Color[] _ProgressColors = null;
        /// <summary>
        /// Indicates the array of colors that when set are used to draw the current progress, i.e. Value>Minimum
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates the array of colors that when set are used to draw the current progress, i.e. Value>Minimum"), TypeConverter(typeof(ArrayConverter))]
        public Color[] ProgressColors
        {
            get
            {
                return _ProgressColors;
            }
            set
            {
                if (_ProgressColors != value)
                {
                    _ProgressColors = value;
                    //OnPropertyChanged(new PropertyChangedEventArgs("Colors"));
                    this.Refresh();
                }
            }
        }

        private Color[] _BackColors = null;
        /// <summary>
        /// Indicates the array of colors that when set are used to draw the background of the item.
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates the array of colors that when set are used to draw the background of the item."), TypeConverter(typeof(ArrayConverter))]
        public Color[] BackColors
        {
            get
            {
                return _BackColors;
            }
            set
            {
                if (_BackColors != value)
                {
                    _BackColors = value;
                    //OnPropertyChanged(new PropertyChangedEventArgs("Colors"));
                    this.Refresh();
                }
            }
        }

        private eButtonTextAlignment _TextAlignment = eButtonTextAlignment.Left;
        /// <summary>
        /// Gets or sets the text alignment. Default value is left.
        /// </summary>
        [Browsable(true), DefaultValue(eButtonTextAlignment.Left), Category("Appearance"), Description("Indicates text alignment.")]
        public eButtonTextAlignment TextAlignment
        {
            get { return _TextAlignment; }
            set
            {
                _TextAlignment = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the text associated with this item.
        /// </summary>
        [System.ComponentModel.Browsable(true), DevCoBrowsable(true), Editor("DevComponents.DotNetBar.Design.TextMarkupUIEditor, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf", typeof(System.Drawing.Design.UITypeEditor)), System.ComponentModel.Category("Appearance"), System.ComponentModel.Description("The text contained in the item."), System.ComponentModel.Localizable(true), System.ComponentModel.DefaultValue("")]
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

        private Color _TextColor = Color.Empty;
        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        [Category("Columns"), Description("Indicates color of text.")]
        public Color TextColor
        {
            get { return _TextColor; }
            set { _TextColor = value; this.Refresh(); }
        }
        /// <summary>
        /// Gets whether property should be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeTextColor()
        {
            return !_TextColor.IsEmpty;
        }
        /// <summary>
        /// Resets property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetTextColor()
        {
            this.TextColor = Color.Empty;
        }
        #endregion

        #region Markup Implementation
        /// <summary>
        /// Gets whether item supports text markup. Default is false.
        /// </summary>
        protected override bool IsMarkupSupported
        {
            get { return _EnableMarkup; }
        }

        private bool _EnableMarkup = true;
        /// <summary>
        /// Gets or sets whether text-markup support is enabled for items Text property. Default value is true.
        /// Set this property to false to display HTML or other markup in the item instead of it being parsed as text-markup.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether text-markup support is enabled for items Text property.")]
        public bool EnableMarkup
        {
            get { return _EnableMarkup; }
            set
            {
                if (_EnableMarkup != value)
                {
                    _EnableMarkup = value;
                    NeedRecalcSize = true;
                    OnTextChanged();
                }
            }
        }
        #endregion
    }
}
