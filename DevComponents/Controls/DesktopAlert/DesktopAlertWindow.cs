using DevComponents.DotNetBar.Rendering;
using DevComponents.DotNetBar.TextMarkup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using DevComponents.DotNetBar.Metro.Helpers;

namespace DevComponents.DotNetBar.Controls
{
    public class DesktopAlertWindow : Form
    {
        #region Constructor
        public DesktopAlertWindow()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque |
                ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer, true);

            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Alert;
            this.ClientSize = DefaultAlertSizeValue;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = FormStartPosition.Manual;
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Font = new Font("Segoe UI", 10.125F);
        }

        protected override void Dispose(bool disposing)
        {
            DestroyAutoCloseTimer();
            base.Dispose(disposing);
        }
        #endregion

        #region Implementation
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Color backColor = this.BackColor;
            Color foreColor = this.ForeColor;

            using (SolidBrush brush = new SolidBrush(backColor))
                g.FillRectangle(brush, this.ClientRectangle);

            if (_CloseButtonVisible)
            {
                Font symFont = Symbols.GetFont(12f, eSymbolSet.Material);
                TextDrawing.DrawStringLegacy(g, "\uE14C", symFont,
                    (_CloseButtonMouseOver ? ColorHelpers.GetShadeColor(foreColor) : foreColor),
                        _CloseButtonBounds,
                        eTextFormat.Default | eTextFormat.NoClipping);
            }

            if (!string.IsNullOrEmpty(_SymbolRealized))
            {
                Font symFont = Symbols.GetFont(_SymbolSize, _SymbolSet);

                TextDrawing.DrawStringLegacy(g, _SymbolRealized, symFont, _SymbolColor.IsEmpty ? foreColor : _SymbolColor,
                        _ImageBounds,
                        eTextFormat.Default | eTextFormat.NoClipping | eTextFormat.VerticalCenter);
            }
            else if (_Image != null)
            {
                g.DrawImage(_Image, _ImageBounds);
            }

            Rectangle r = _TextBounds;
            if (r.Bottom > this.ClientRectangle.Bottom - this.Padding.Bottom)
                r.Height -= (r.Bottom - (this.ClientRectangle.Bottom - this.Padding.Bottom));
            eTextFormat format = TextFormat;
            if (_TextMarkup == null)
            {
                if (this.RightToLeft == RightToLeft.Yes) format |= eTextFormat.RightToLeft;
                if (UseTextRenderer)
                {
                    TextRenderer.DrawText(g, Text, Font, r, foreColor, backColor,
                        TextDrawing.GetTextFormatFlags(format));
                }
                else
                    TextDrawing.DrawString(g, Text, this.Font, foreColor, r, format);
            }
            else
            {
                TextMarkup.MarkupDrawContext d = new TextMarkup.MarkupDrawContext(g, this.Font, foreColor,
                    (this.RightToLeft == RightToLeft.Yes), r, true);
                Size markupSize = _TextMarkup.Bounds.Size;
                if (!markupSize.IsEmpty && (format & eTextFormat.VerticalCenter) == eTextFormat.VerticalCenter && r.Height>markupSize.Height)
                {
                    r.Y += (r.Height - markupSize.Height) / 2;
                    r.Height = markupSize.Height;
                }
                _TextMarkup.Arrange(r, d);
                _TextMarkup.Render(d);
            }

            base.OnPaint(e);
        }
        private eTextFormat TextFormat
        {
            get
            {
                return eTextFormat.Default | eTextFormat.WordBreak | eTextFormat.VerticalCenter | eTextFormat.EndEllipsis;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (_AutoClose && _AutoCloseTimer != null)
                _AutoCloseTimer.Enabled = false;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_TextMarkup != null)
                _TextMarkup.MouseLeave(this);
            if (_AutoClose && _AutoCloseTimer != null)
                _AutoCloseTimer.Enabled = true;
            base.OnMouseLeave(e);
        }

        private bool _CloseButtonMouseOver = false;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_CloseButtonVisible && _CloseButtonBounds.Contains(e.Location))
            {
                _CloseButtonMouseOver = true;
                Invalidate(_CloseButtonBounds);
            }
            else if (_CloseButtonMouseOver)
            {
                _CloseButtonMouseOver = false;
                Invalidate(_CloseButtonBounds);
            }

            if (_TextMarkup != null)
                _TextMarkup.MouseMove(this, e);

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_TextMarkup != null)
                _TextMarkup.MouseDown(this, e);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_TextMarkup != null)
                _TextMarkup.MouseUp(this, e);

            if (_CloseButtonMouseOver && e.Button == MouseButtons.Left)
                this.HideAlert(eAlertClosureSource.CloseButton);
            base.OnMouseUp(e);
        }

        private bool _AutoSize = true;
        [DefaultValue(true)]
        public override bool AutoSize
        {
            get
            {
                return _AutoSize;
            }
            set
            {
                _AutoSize = value;
            }
        }

        private Rectangle _ImageBounds = Rectangle.Empty;
        private Rectangle _TextBounds = Rectangle.Empty;
        private Rectangle _CloseButtonBounds = Rectangle.Empty;
        /// <summary>
        /// Sets alert size based on its content, it respects MinimumSize and MaximumSize property settings.
        /// </summary>
        public void PerformAutoSize()
        {
            Size size = Dpi.Size(_DefaultAlertSize);
            if (!this.MinimumSize.IsEmpty)
                size = Dpi.Size(this.MinimumSize);

            if (!this.IsHandleCreated)
                this.CreateHandle();

            Rectangle r = new Rectangle(Point.Empty, size);
            r.X += Dpi.Width(this.Padding.Left);
            r.Width -= Dpi.Width(this.Padding.Horizontal);
            r.Y += Dpi.Height(this.Padding.Top);
            r.Height -= Dpi.Height(this.Padding.Vertical);

            if (!string.IsNullOrEmpty(Text) || !string.IsNullOrEmpty(_SymbolRealized) || _Image != null)
            {
                using (Graphics g = BarFunctions.CreateGraphics(this))
                {
                    if (_CloseButtonVisible)
                    {
                        Font symFont = Symbols.GetFont(12f, eSymbolSet.Material);
                        Size closeSize = TextDrawing.MeasureString(g, "\uE14C", symFont); // Need to do this to get consistent size for the symbol since they are not all the same width we pick widest
                        _CloseButtonBounds = new Rectangle(r.Right - closeSize.Width - Dpi.Width4, r.Y + Dpi.Height4, closeSize.Width, closeSize.Height);
                        r.Width -= closeSize.Width + Dpi.Width8;
                    }


                    _ImageBounds = Rectangle.Empty;
                    if (!string.IsNullOrEmpty(_SymbolRealized))
                    {
                        Size symSize = GetSymbolSize(g);
                        symSize.Width += Dpi.Width2;
                        _ImageBounds = new Rectangle(r.X, r.Y, symSize.Width, symSize.Height);
                        r.X += symSize.Width + _ImageTextPadding;
                        r.Width -= symSize.Width + _ImageTextPadding;
                        if (symSize.Height > r.Height)
                        {
                            size.Height += (symSize.Height - r.Height);
                            r.Height = symSize.Height;
                        }
                        else
                        {
                            _ImageBounds.Height = r.Height;
                        }
                    }
                    else if (_Image != null)
                    {
                        _ImageBounds = new Rectangle(r.X, r.Y, _Image.Width, _Image.Height);
                        r.X += _Image.Width + _ImageTextPadding;
                        r.Width -= _Image.Width + _ImageTextPadding;
                        if (_Image.Height > r.Height)
                        {
                            size.Height += (_Image.Height - r.Height);
                            r.Height = _Image.Height;
                        }
                    }

                    _TextBounds = Rectangle.Empty;

                    if (_TextMarkup != null)
                    {
                        MarkupDrawContext dc = GetMarkupDrawContext(g);
                        _TextMarkup.Measure(r.Size, dc);
                        Size sz = _TextMarkup.Bounds.Size;
                        _TextMarkup.Arrange(new Rectangle(r.Location, sz), dc);
                        if (sz.Width > r.Width)
                        {
                            size.Width += (sz.Width - r.Width);
                            r.Width = size.Width;
                        }
                        if (sz.Height > r.Height)
                        {
                            size.Height += (sz.Height - r.Height);
                            r.Height = size.Height;
                        }
                        _TextBounds = r;
                    }
                    else if (!string.IsNullOrEmpty(this.Text))
                    {
                        Size sz = (UseTextRenderer ? TextRenderer.MeasureText(g, this.Text, this.Font, r.Size, TextDrawing.GetTextFormatFlags(TextFormat)) : TextDrawing.MeasureString(g, this.Text, this.Font, size, TextFormat));
                        if (sz.Width > r.Width)
                        {
                            size.Width += (sz.Width - r.Width);
                            r.Width = size.Width;
                        }
                        if (sz.Height > r.Height)
                        {
                            size.Height += (sz.Height - r.Height);
                            r.Height = size.Height;
                        }
                        _TextBounds = r;
                    }
                }
            }

            Size maximumSize = Dpi.Size(this.MaximumSize);
            if (maximumSize.Width > 0 && size.Width > maximumSize.Width)
                size.Width = maximumSize.Width;
            if (maximumSize.Height > 0 && size.Height > maximumSize.Height)
                size.Height = maximumSize.Height;

            this.Size = size;
        }

        private MarkupDrawContext GetMarkupDrawContext(Graphics g)
        {
            return new MarkupDrawContext(g, this.Font, this.ForeColor, this.RightToLeft == RightToLeft.Yes);
        }
        private Size GetSymbolSize(Graphics g)
        {
            Size symbolSize = Size.Empty;
            if (g == null || string.IsNullOrEmpty(_Symbol)) return symbolSize;
            Font symFont = Symbols.GetFont(this.SymbolSize, this.SymbolSet);
            symbolSize = TextDrawing.MeasureString(g, "\uF00A", symFont); // Need to do this to get consistent size for the symbol since they are not all the same width we pick widest
            //int descent = (int)Math.Ceiling((symFont.FontFamily.GetCellDescent(symFont.Style) *
            //    symFont.Size / symFont.FontFamily.GetEmHeight(symFont.Style)));
            //symbolSize.Height -= descent;
            return symbolSize;
        }

        //private Color[] _BackColors = null;
        ///// <summary>
        ///// Indicates the array of colors that when set are used to draw the background of the alert.
        ///// </summary>
        //[DefaultValue(null), Category("Appearance"), Description("Indicates the array of colors that when set are used to draw the background of the alert."), TypeConverter(typeof(ArrayConverter))]
        //public Color[] BackColors
        //{
        //    get
        //    {
        //        return _BackColors;
        //    }
        //    set
        //    {
        //        if (_BackColors != value)
        //        {
        //            _BackColors = value;
        //            this.Invalidate();
        //        }
        //    }
        //}

        //private Color[] _BorderColors = null;
        ///// <summary>
        ///// Indicates the array of colors that when set are used to draw the border of the alert.
        ///// </summary>
        //[DefaultValue(null), Category("Appearance"), Description("Indicates the array of colors that when set are used to draw the border of the alert."), TypeConverter(typeof(ArrayConverter))]
        //public Color[] BorderColors
        //{
        //    get
        //    {
        //        return _BorderColors;
        //    }
        //    set
        //    {
        //        if (_BorderColors != value)
        //        {
        //            _BorderColors = value;
        //            //OnPropertyChanged(new PropertyChangedEventArgs("Colors"));
        //            this.Invalidate();
        //        }
        //    }
        //}

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


        private float _SymbolSize = 22f;
        /// <summary>
        /// Indicates the size of the symbol in points.
        /// </summary>
        [DefaultValue(22f), Category("Appearance"), Description("Indicates the size of the symbol in points.")]
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

        private Image _Image = null;
        /// <summary>
        /// Indicates image displayed on alert.
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates image displayed on alert.")]
        public Image Image
        {
            get { return _Image; }
            set
            {
                _Image = value;
                this.Invalidate();
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            // Markup support
            MarkupTextChanged();
            base.OnTextChanged(e);
        }

        //private string _TitleText = "";
        ///// <summary>
        ///// Gets or sets the tile title text displayed by default in lower left corner.
        ///// </summary>
        //[DefaultValue(""), Category("Appearance"), Description("Indicates tile title text displayed by default in lower left corner"), Editor("DevComponents.DotNetBar.Design.TextMarkupUIEditor, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf", typeof(System.Drawing.Design.UITypeEditor))]
        //public string TitleText
        //{
        //    get { return _TitleText; }
        //    set
        //    {
        //        if (value != _TitleText)
        //        {
        //            string oldValue = _TitleText;
        //            _TitleText = value;
        //            OnTitleTextChanged(oldValue, value);
        //        }
        //    }
        //}
        ///// <summary>
        ///// Called when TitleText property has changed.
        ///// </summary>
        ///// <param name="oldValue">Old property value</param>
        ///// <param name="newValue">New property value</param>
        //protected virtual void OnTitleTextChanged(string oldValue, string newValue)
        //{
        //    TitleTextMarkupUpdate();
        //    this.Invalidate();
        //    //OnPropertyChanged(new PropertyChangedEventArgs("TitleText"));
        //}
        ///// <summary>
        ///// Gets reference to parsed markup body element if text was markup otherwise returns null.
        ///// </summary>
        //internal TextMarkup.BodyElement TitleTextMarkupBody
        //{
        //    get { return _TitleTextMarkup; }
        //}
        //private TextMarkup.BodyElement _TitleTextMarkup = null;
        //private void TitleTextMarkupUpdate()
        //{
        //    if (_TitleTextMarkup != null)
        //        _TitleTextMarkup.HyperLinkClick -= TitleTextMarkupLinkClicked;
        //    _TitleTextMarkup = null;

        //    if (!_TextMarkupEnabled)
        //        return;

        //    if (!TextMarkup.MarkupParser.IsMarkup(ref _TitleText))
        //        return;

        //    _TitleTextMarkup = TextMarkup.MarkupParser.Parse(_TitleText);

        //    if (_TitleTextMarkup != null)
        //        _TitleTextMarkup.HyperLinkClick += TitleTextMarkupLinkClicked;
        //}
        //private void TitleTextMarkupLinkClicked(object sender, EventArgs e)
        //{
        //    DevComponents.DotNetBar.TextMarkup.HyperLink link = sender as DevComponents.DotNetBar.TextMarkup.HyperLink;

        //    if (link != null)
        //        OnTitleTextMarkupLinkClick(new MarkupLinkClickEventArgs(link.Name, link.HRef));
        //    else
        //        OnTitleTextMarkupLinkClick(e);
        //}
        ///// <summary>
        ///// Occurs when an hyperlink in title text markup is clicked.
        ///// </summary>
        //public event EventHandler TitleTextMarkupLinkClick;
        ///// <summary>
        ///// Raises TitleTextMarkupLinkClick event.
        ///// </summary>
        ///// <param name="e">Provides event arguments.</param>
        //protected virtual void OnTitleTextMarkupLinkClick(EventArgs e)
        //{
        //    EventHandler handler = TitleTextMarkupLinkClick;
        //    if (handler != null)
        //        handler(this, e);
        //}


        //private Font _TitleTextFont = null;
        ///// <summary>
        ///// Gets or sets the title text font.
        ///// </summary>
        //[DefaultValue(null), Category("Appearance"), Description("Gets or sets the title text font.")]
        //public Font TitleTextFont
        //{
        //    get { return _TitleTextFont; }
        //    set
        //    {
        //        if (value != _TitleTextFont)
        //        {
        //            Font oldValue = _TitleTextFont;
        //            _TitleTextFont = value;
        //            OnTitleTextFontChanged(oldValue, value);
        //        }
        //    }
        //}
        ///// <summary>
        ///// Called when TitleTextFont property has changed.
        ///// </summary>
        ///// <param name="oldValue">Old property value</param>
        ///// <param name="newValue">New property value</param>
        //protected virtual void OnTitleTextFontChanged(Font oldValue, Font newValue)
        //{
        //    this.Invalidate();
        //    //OnPropertyChanged(new PropertyChangedEventArgs("TitleTextFont"));
        //}
        //private Rectangle _TitleTextBounds = Rectangle.Empty;
        //internal Rectangle TitleTextBounds
        //{
        //    get
        //    {
        //        return _TitleTextBounds;
        //    }
        //    set
        //    {
        //        _TitleTextBounds = value;
        //    }
        //}

        //private Color _TitleTextColor = Color.Empty;
        ///// <summary>
        ///// Gets or sets the color of the title text.
        ///// </summary>
        //[Category("Columns"), Description("Indicates color of title text.")]
        //public Color TitleTextColor
        //{
        //    get { return _TitleTextColor; }
        //    set
        //    {
        //        if (_TitleTextColor != value)
        //        {
        //            Color oldValue = _TitleTextColor;
        //            _TitleTextColor = value;
        //            OnTitleTextColorChanged(oldValue, value);
        //        }
        //    }
        //}
        ///// <summary>
        ///// Called when TitleTextColor property has changed.
        ///// </summary>
        ///// <param name="oldValue">Old property value</param>
        ///// <param name="newValue">New property value</param>
        //protected virtual void OnTitleTextColorChanged(Color oldValue, Color newValue)
        //{
        //    this.Invalidate();
        //    //OnPropertyChanged(new PropertyChangedEventArgs("TitleTextColor"));
        //}
        ///// <summary>
        ///// Gets whether property should be serialized.
        ///// </summary>
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public bool ShouldSerializeTitleTextColor()
        //{
        //    return !_TitleTextColor.IsEmpty;
        //}
        ///// <summary>
        ///// Resets property to its default value.
        ///// </summary>
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public void ResetTitleTextColor()
        //{
        //    this.TitleTextColor = Color.Empty;
        //}

        //private ContentAlignment _TitleTextAlignment = ContentAlignment.BottomLeft;
        ///// <summary>
        ///// Gets or sets title text alignment.
        ///// </summary>
        //[DefaultValue(ContentAlignment.BottomLeft), Category("Appearance"), Description("Indicates title text alignment.")]
        //public ContentAlignment TitleTextAlignment
        //{
        //    get { return _TitleTextAlignment; }
        //    set
        //    {
        //        if (value != _TitleTextAlignment)
        //        {
        //            ContentAlignment oldValue = _TitleTextAlignment;
        //            _TitleTextAlignment = value;
        //            OnTitleTextAlignmentChanged(oldValue, value);
        //        }
        //    }
        //}
        ///// <summary>
        ///// Called when TitleTextAlignment property has changed.
        ///// </summary>
        ///// <param name="oldValue">Old property value</param>
        ///// <param name="newValue">New property value</param>
        //protected virtual void OnTitleTextAlignmentChanged(ContentAlignment oldValue, ContentAlignment newValue)
        //{
        //    //OnPropertyChanged(new PropertyChangedEventArgs("TitleTextAlignment"));
        //    this.Invalidate();
        //}

        protected override void OnClick(EventArgs e)
        {
            if (_TextMarkup != null && _TextMarkup.MouseOverElement is HyperLink)
            {
                _TextMarkup.Click(this);
                base.OnClick(e);
                return;
            }
            else if (_CloseButtonMouseOver)
            {
                base.OnClick(e);
                return;
            }

            if (_ClickAction != null)
            {
                _ClickAction.Invoke(_AlertId);
                this.HideAlert(eAlertClosureSource.AlertClicked);
            }
            base.OnClick(e);
        }

        private long _AlertId = 0;
        /// <summary>
        /// Indicates optional Alert Id.
        /// </summary>
        [Browsable(false), DefaultValue(0)]
        public long AlertId {
            get { return _AlertId; }
            set { _AlertId = value; }
        }

        private Action<long> _ClickAction = null;
        /// <summary>
        /// Indicates the method that will be invoked if user clicks the alert.
        /// </summary>
        [Browsable(false), DefaultValue(null)]
        public Action<long> ClickAction
        {
            get { return _ClickAction; }
            set { _ClickAction = value; }
        }

        private bool _TextMarkupEnabled = true;
        /// <summary>
        /// Gets or sets whether text-markup can be used in Text property.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether text-markup can be used in Text property.")]
        public bool TextMarkupEnabled
        {
            get { return _TextMarkupEnabled; }
            set
            {
                if (value != _TextMarkupEnabled)
                {
                    bool oldValue = _TextMarkupEnabled;
                    _TextMarkupEnabled = value;
                    OnTextMarkupEnabledChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when TextMarkupEnabled property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnTextMarkupEnabledChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("TextMarkupEnabled"));
            this.Invalidate();
        }
        #endregion

        #region Markup Implementation
        private TextMarkup.BodyElement _TextMarkup = null;

        private void MarkupTextChanged()
        {
            if (_TextMarkup != null)
                _TextMarkup.HyperLinkClick -= TextMarkupLinkClick;

            _TextMarkup = null;

            if (!_TextMarkupEnabled)
                return;
            string text = this.Text;
            if (!TextMarkup.MarkupParser.IsMarkup(ref text))
                return;

            _TextMarkup = TextMarkup.MarkupParser.Parse(text);

            if (_TextMarkup != null)
                _TextMarkup.HyperLinkClick += TextMarkupLinkClick;
        }

        /// <summary>
        /// Occurs when text markup link is clicked.
        /// </summary>
        protected virtual void TextMarkupLinkClick(object sender, EventArgs e)
        {
            TextMarkup.HyperLink link = sender as TextMarkup.HyperLink;
            if (link != null)
                OnMarkupLinkClick(new MarkupLinkClickEventArgs(link.Name, link.HRef));
        }
        /// <summary>
        /// Occurs when text markup link is clicked. Markup links can be created using "a" tag, for example:
        /// <a name="MyLink">Markup link</a>
        /// </summary>
        [Description("Occurs when text markup link is clicked. Markup links can be created using a tag.")]
        public event MarkupLinkClickEventHandler MarkupLinkClick;
        /// <summary>
        /// Raises MarkupLinkClick event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnMarkupLinkClick(MarkupLinkClickEventArgs e)
        {
            MarkupLinkClickEventHandler handler = MarkupLinkClick;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Gets reference to parsed markup body element if text was markup otherwise returns null.
        /// </summary>
        internal TextMarkup.BodyElement TextMarkupBody
        {
            get { return _TextMarkup; }
        }

        internal static Size MeasureText(DesktopAlertWindow item, Graphics g, int containerWidth, Font font, eTextFormat stringFormat, bool rightToLeft)
        {
            if (item.Text == "" && item.TextMarkupBody == null) return Size.Empty;

            Size textSize = Size.Empty;

            if (item.TextMarkupBody == null)
            {
                textSize = TextDrawing.MeasureString(g, ButtonItemPainter.GetDrawText(item.Text), font, containerWidth, stringFormat);
            }
            else
            {
                Size availSize = new Size(containerWidth, 1);
                if (containerWidth == 0)
                    availSize.Width = 1600;
                TextMarkup.MarkupDrawContext d = new TextMarkup.MarkupDrawContext(g, font, Color.Empty, false);
                item.TextMarkupBody.Measure(availSize, d);
                availSize = item.TextMarkupBody.Bounds.Size;
                if (containerWidth != 0)
                    availSize.Width = containerWidth;
                d.RightToLeft = rightToLeft;
                item.TextMarkupBody.Arrange(new Rectangle(0, 0, availSize.Width, availSize.Height), d);

                textSize = item.TextMarkupBody.Bounds.Size;
            }

            return textSize;
        }

        private static readonly Size DefaultAlertSizeValue = new Size(360, 64);
        private Size _DefaultAlertSize = DefaultAlertSizeValue;
        /// <summary>
        /// Gets or sets the default alert size.
        /// </summary>
        [Category("Appearance"), Description("Indicates default alert size.")]
        public Size DefaultAlertSize
        {
            get { return _DefaultAlertSize; }
            set
            {
                if (value != _DefaultAlertSize)
                {
                    Size oldValue = _DefaultAlertSize;
                    _DefaultAlertSize = value;
                    OnDefaultAlertSizeChanged(oldValue, value);
                }
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDefaultAlertSize()
        {
            return _DefaultAlertSize.Width != DefaultAlertSizeValue.Width || _DefaultAlertSize.Height != DefaultAlertSizeValue.Height;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetDefaultAlertSize()
        {
            DefaultAlertSize = DefaultAlertSizeValue;
        }
        private void OnDefaultAlertSizeChanged(Size oldValue, Size newValue)
        {
            if (_AutoSize && this.IsHandleCreated)
                PerformAutoSize();
        }

        //private eAlertAnimation _AlertAnimation = eAlertAnimation.RightToLeft;
        ///// <summary>
        ///// Gets or sets the animation type used to display Alert.
        ///// </summary>
        //[Browsable(true), Description("Gets or sets the animation type used to display Alert."), Category("Behavior"), DefaultValue(eAlertAnimation.RightToLeft)]
        //public eAlertAnimation AlertAnimation
        //{
        //    get { return _AlertAnimation; }
        //    set { _AlertAnimation = value; }
        //}

        private int _AlertAnimationDuration = 200;
        /// <summary>
        /// Gets or sets the total time in milliseconds alert animation takes.
        /// Default value is 200.
        /// </summary>
        [Browsable(true), Description("Gets or sets the total time in milliseconds alert animation takes."), Category("Behavior"), DefaultValue(200)]
        public int AlertAnimationDuration
        {
            get { return _AlertAnimationDuration; }
            set { _AlertAnimationDuration = value; }
        }

        private bool _AutoClose = true;
        /// <summary>
        /// Gets or sets whether balloon will close automatically when user click the close button.
        /// </summary>
        [Description("Indicates whether balloon will close automatically when user click the close button."), Category("Behavior"), DefaultValue(true)]
        public bool AutoClose
        {
            get { return _AutoClose; }
            set { _AutoClose = value; }
        }

        private int _AutoCloseTimeOut = 6;
        /// <summary>
        /// Gets or sets time period in seconds after alert closes automatically.
        /// </summary>
        [Description("Indicates time period in seconds after balloon closes automatically."), Category("Behavior"), DefaultValue(6)]
        public int AutoCloseTimeOut
        {
            get { return _AutoCloseTimeOut; }
            set
            {
                _AutoCloseTimeOut = value;
                OnAutoCloseTimeOutChanged();
            }
        }

        private System.Windows.Forms.Timer _AutoCloseTimer = null;
        protected void OnAutoCloseTimeOutChanged()
        {
            if (_AutoCloseTimeOut > 0 && !this.DesignMode)
            {
                StartAutoCloseTimer();
            }
            else
            {
                DestroyAutoCloseTimer();
            }
        }

        private void StartAutoCloseTimer()
        {
            if (_AutoCloseTimer == null)
            {
                _AutoCloseTimer = new System.Windows.Forms.Timer();
                _AutoCloseTimer.Enabled = false;
                _AutoCloseTimer.Tick += new EventHandler(this.AutoCloseTimeOutEllapsed);
            }
            _AutoCloseTimer.Interval = _AutoCloseTimeOut * 1000;
            if (this.Visible)
            {
                _AutoCloseTimer.Enabled = true;
                _AutoCloseTimer.Start();
            }
        }

        protected virtual void AutoCloseTimeOutEllapsed(object sender, EventArgs e)
        {
            if (this.IsDisposed)
                return;
            DestroyAutoCloseTimer();
            this.HideAlert(eAlertClosureSource.Timeout);
            this.Close();
        }

        private void DestroyAutoCloseTimer()
        {
            if (_AutoCloseTimer != null)
            {
                _AutoCloseTimer.Enabled = false;
                _AutoCloseTimer.Tick -= new EventHandler(this.AutoCloseTimeOutEllapsed);
                _AutoCloseTimer.Dispose();
                _AutoCloseTimer = null;
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {
                if (_AutoCloseTimeOut > 0)
                    StartAutoCloseTimer();

                if (_AutoCloseTimer != null && !_AutoCloseTimer.Enabled)
                {
                    _AutoCloseTimer.Enabled = true;
                    _AutoCloseTimer.Start();
                }
            }
            else
            {
                if (_AutoCloseTimer != null && _AutoCloseTimer.Enabled)
                {
                    _AutoCloseTimer.Stop();
                    _AutoCloseTimer.Enabled = false;
                }
            }
        }

        private Control _ReferenceControl = null;
        /// <summary>
         /// Specifies the reference control which is used to find which screen the alert is displayed on. If not specified alert is displayed on primary screen.
        /// </summary>
        [DefaultValue(null), Browsable(false)]
        public Control ReferenceControl
        {
            get { return _ReferenceControl; }
            set { _ReferenceControl = value; }
        }

        private static Semaphore _DisplayPositionsSemaphore = new Semaphore(1, 1);
        private static List<Rectangle> _DisplayPositions = new List<Rectangle>();
        private Rectangle GetAlertBounds()
        {
            Rectangle r = this.Bounds;

            ScreenInformation si = null;
            if (_ReferenceControl == null)
                si = BarFunctions.PrimaryScreen;
            else
                si = BarFunctions.ScreenFromControl(_ReferenceControl);
            
            if (_AlertPosition == eAlertPosition.BottomRight)
            {
                r = new Rectangle(si.WorkingArea.Right - this.Width, si.WorkingArea.Bottom - this.Height - Dpi.Height12, this.Width, this.Height);
            }
            else if (_AlertPosition == eAlertPosition.BottomLeft)
            {
                r = new Rectangle(si.WorkingArea.Left, si.WorkingArea.Bottom - this.Height - Dpi.Height12, this.Width, this.Height);
            }
            else if (_AlertPosition == eAlertPosition.TopRight)
            {
                r = new Rectangle(si.WorkingArea.Right - this.Width, si.WorkingArea.Top + Dpi.Height12, this.Width, this.Height);
            }
            else if (_AlertPosition == eAlertPosition.TopLeft)
            {
                r = new Rectangle(si.WorkingArea.Left, si.WorkingArea.Top + Dpi.Height12, this.Width, this.Height);
            }

            // Now adjust display position
            if (_DisplayPositions.Count > 0)
            {
                if(_AlertPosition == eAlertPosition.TopLeft || _AlertPosition == eAlertPosition.TopRight)
                    _DisplayPositions.Sort(_RectangleComparer);
                else
                    _DisplayPositions.Sort(_RectangleReverseComparer);
                Rectangle originalBounds = r;
                for (int i = 0; i < _DisplayPositions.Count; i++)
                {
                    if (_DisplayPositions[i].IntersectsWith(r))
                    {
                        if (_AlertPosition == eAlertPosition.BottomLeft || _AlertPosition == eAlertPosition.BottomRight)
                            r.Y -= _DisplayPositions[i].Height + _AlertsSpacing;
                        else
                            r.Y += _DisplayPositions[i].Height + _AlertsSpacing;
                    }
                    else
                        break;
                    if (!si.WorkingArea.Contains(r))
                    {
                        r = originalBounds;
                        break;
                    }
                    if (i == _DisplayPositions.Count - 1)
                        break;
                }
            }

            _DisplayPositionsSemaphore.WaitOne();
            try
            {
                _DisplayPositions.Add(r);
            }
            finally
            {
                _DisplayPositionsSemaphore.Release();
            }
            return r;
        }

        private static readonly RectangleComparer _RectangleComparer = new RectangleComparer();
        private static readonly RectangleReverseComparer _RectangleReverseComparer = new RectangleReverseComparer();
        private class RectangleComparer : IComparer<Rectangle>
        {
            public int Compare(Rectangle x, Rectangle y)
            {
                return x.Y - y.Y;
            }
        }
        private class RectangleReverseComparer : IComparer<Rectangle>
        {
            public int Compare(Rectangle x, Rectangle y)
            {
                return y.Y - x.Y;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _DisplayPositionsSemaphore.WaitOne();
            try
            {
                for (int i = _DisplayPositions.Count - 1; i >= 0; i--)
                {
                    if (_DisplayPositions[i] == this.Bounds)
                    {
                        _DisplayPositions.RemoveAt(i);
                        break;
                    }
                }
            }
            finally
            {
                _DisplayPositionsSemaphore.Release();
            }
            base.OnClosed(e);
        }

        /// <summary>
        /// Display balloon.
        /// </summary>
        /// <param name="focusAlert">Indicates whether alert receives input focus upon showing.</param>
        public void Show(bool focusAlert)
        {
            PerformAutoSize();

            this.Bounds = GetAlertBounds();
            Rectangle rEnd = this.Bounds;

            DesktopAlert.OnBeforeAlertDisplayed(this, EventArgs.Empty);
            if(this.IsDisposed)
                return;

            if (ShouldAnimate())
            {
                try
                {
                    _AnimationInProgress = true;
                    eAlertAnimation alertAnimation = GetAlertAnimation();
                    if (alertAnimation == eAlertAnimation.RightToLeft)
                        NativeFunctions.AnimateWindow(this.Handle, _AlertAnimationDuration, (NativeFunctions.AW_SLIDE | NativeFunctions.AW_HOR_NEGATIVE));
                    else if (alertAnimation == eAlertAnimation.LeftToRight)
                        NativeFunctions.AnimateWindow(this.Handle, _AlertAnimationDuration, (NativeFunctions.AW_SLIDE | NativeFunctions.AW_HOR_POSITIVE));
                    else if (alertAnimation == eAlertAnimation.BottomToTop)
                        NativeFunctions.AnimateWindow(this.Handle, _AlertAnimationDuration, (NativeFunctions.AW_SLIDE | NativeFunctions.AW_VER_NEGATIVE));
                    else if (alertAnimation == eAlertAnimation.TopToBottom)
                        NativeFunctions.AnimateWindow(this.Handle, _AlertAnimationDuration, (NativeFunctions.AW_SLIDE | NativeFunctions.AW_VER_POSITIVE));
                }
                finally
                {
                    _AnimationInProgress = false;
                }
            }
            base.Show();

            if (_TopMost)
                NativeFunctions.SetWindowPos(this.Handle,
                    new IntPtr(NativeFunctions.HWND_TOPMOST), 0, 0, 0, 0,
                    NativeFunctions.SWP_NOACTIVATE | NativeFunctions.SWP_NOMOVE | NativeFunctions.SWP_NOSIZE);
            //this.Visible = true;

            if (_PlaySound)
                System.Media.SystemSounds.Beep.Play();
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        private eAlertAnimation GetAlertAnimation()
        {
            if (_AlertPosition == eAlertPosition.TopLeft || _AlertPosition == eAlertPosition.BottomLeft)
                return eAlertAnimation.LeftToRight;
            else
                return eAlertAnimation.RightToLeft;
        }

        /// <summary>
        /// Displays balloon.
        /// </summary>
        public new void Show()
        {
            this.Show(false);
        }

        private bool _AnimationInProgress = false;
        /// <summary>
        /// Called when alert needs to be hidden.
        /// </summary>
        protected virtual void HideAlert(eAlertClosureSource source)
        {
            DestroyAutoCloseTimer();
            if (ShouldAnimate())
            {
                Rectangle rStart = this.Bounds;
                //Rectangle rEnd = GetAnimationRectangle();
                try
                {
                    _AnimationInProgress = true;
                    eAlertAnimation alertAnimation = GetAlertAnimation();
                    if (alertAnimation == eAlertAnimation.RightToLeft)
                        NativeFunctions.AnimateWindow(this.Handle, _AlertAnimationDuration, (NativeFunctions.AW_SLIDE | NativeFunctions.AW_HOR_POSITIVE | NativeFunctions.AW_HIDE));
                    else if (alertAnimation == eAlertAnimation.LeftToRight)
                        NativeFunctions.AnimateWindow(this.Handle, _AlertAnimationDuration, (NativeFunctions.AW_SLIDE | NativeFunctions.AW_HOR_NEGATIVE | NativeFunctions.AW_HIDE));
                    else if (alertAnimation == eAlertAnimation.BottomToTop)
                        NativeFunctions.AnimateWindow(this.Handle, _AlertAnimationDuration, (NativeFunctions.AW_SLIDE | NativeFunctions.AW_VER_POSITIVE | NativeFunctions.AW_HIDE));
                    else if (alertAnimation == eAlertAnimation.TopToBottom)
                        NativeFunctions.AnimateWindow(this.Handle, _AlertAnimationDuration, (NativeFunctions.AW_SLIDE | NativeFunctions.AW_VER_NEGATIVE | NativeFunctions.AW_HIDE));
                }
                finally
                {
                    _AnimationInProgress = false;
                }
            }
            else
                base.Hide();
            this.Close();
            DesktopAlert.OnAlertClosed(this, new AlertClosedEventArgs(source));
            _ClickAction = null;
            this.Dispose();
        }

        /// <summary>
        /// Hides balloon.
        /// </summary>
        public new void Hide()
        {
            HideAlert(eAlertClosureSource.Timeout);
        }

        private bool ShouldAnimate()
        {
            if (_AlertAnimationDuration > 0 && !this.DesignMode && !this.IsDisposed)
                return true;
            return false;
        }

        private int _AlertsSpacing = 8;
        /// <summary>
        /// Indicates spacing between alerts on the screen
        /// </summary>
        [DefaultValue(8), Category("Behavior"), Description("Indicates spacing between alerts on the screen")]
        public int AlertsSpacing
        {
            get { return _AlertsSpacing; }
            set
            {
                if (_AlertsSpacing != value)
                {
                    int oldValue = _AlertsSpacing;
                    _AlertsSpacing = value;
                    OnAlertsSpacingChanged(value, oldValue);
                }
            }
        }

        protected virtual void OnAlertsSpacingChanged(int newValue, int oldValue)
        {
            throw new NotImplementedException();
        }

        //private Rectangle GetAnimationRectangle()
        //{
        //    Rectangle r = new Rectangle(this.Location, this.Size);
        //    if (_AlertAnimation == eAlertAnimation.BottomToTop)
        //    {
        //        r.Y = r.Bottom - 1;
        //        r.Height = 1;
        //    }
        //    else if (_AlertAnimation == eAlertAnimation.TopToBottom)
        //    {
        //        r.Height = 1;
        //    }
        //    else if (_AlertAnimation == eAlertAnimation.LeftToRight)
        //    {
        //        r.Width = 2;
        //    }
        //    else if (_AlertAnimation == eAlertAnimation.RightToLeft)
        //    {
        //        r.X = r.Right - 1;
        //        r.Width = 1;
        //    }
        //    return r;
        //}

        private eAlertPosition _AlertPosition = eAlertPosition.BottomRight;
        /// <summary>
        /// Indicates the request screen position for the alert
        /// </summary>
        [DefaultValue(eAlertPosition.BottomRight), Category("Behavior"), Description("Indicates the request screen position for the alert")]
        public eAlertPosition AlertPosition
        {
            get { return _AlertPosition; }
            set
            {
                if (_AlertPosition != value)
                {
                    eAlertPosition oldValue = _AlertPosition;
                    _AlertPosition = value;
                    OnAlertPositionChanged(value, oldValue);
                }
            }
        }
        protected virtual void OnAlertPositionChanged(eAlertPosition newValue, eAlertPosition oldValue)
        {
            //throw new NotImplementedException();
        }

        private bool _TopMost = true;
        [DefaultValue(true)]
        public new bool TopMost
        {
            get { return _TopMost; }
            set
            {
                if (_TopMost != value)
                {
                    bool oldValue = _TopMost;
                    _TopMost = value;
                    OnTopMostChanged(value, oldValue);
                }
            }
        }

        protected virtual void OnTopMostChanged(bool newValue, bool oldValue)
        {
            if (IsHandleCreated && Visible)
                NativeFunctions.SetWindowPos(this.Handle,
                    new IntPtr(newValue ? NativeFunctions.HWND_TOPMOST : NativeFunctions.HWND_NOTOPMOST), 0, 0, 0, 0,
                    NativeFunctions.SWP_NOACTIVATE | NativeFunctions.SWP_NOMOVE | NativeFunctions.SWP_NOSIZE);
        }

        private int _ImageTextPadding = 11;
        /// <summary>
        /// Indicates spacing in pixels between image and text
        /// </summary>
        [DefaultValue(11), Category("Appearance"), Description("Indicates spacing in pixels between image and text")]
        public int ImageTextPadding
        {
            get { return _ImageTextPadding; }
            set
            {
                if (_ImageTextPadding != value)
                {
                    int oldValue = _ImageTextPadding;
                    _ImageTextPadding = value;
                    OnImageTextPaddingChanged(value, oldValue);
                }
            }
        }

        protected virtual void OnImageTextPaddingChanged(int newValue, int oldValue)
        {

        }

        private bool UseTextRenderer
        {
            get { return true; /*_BackColors == null || _BackColors.Length < 2;*/ }
        }

        private bool _PlaySound = true;
        /// <summary>
        /// Indicates whether alert plays exclamation sound when shown.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether alert plays exclamation sound when shown.")]
        public bool PlaySound
        {
            get { return _PlaySound; }
            set
            {
                if (_PlaySound != value)
                {
                    bool oldValue = _PlaySound;
                    _PlaySound = value;
                    OnPlaySoundChanged(value, oldValue);
                }
            }
        }

        protected virtual void OnPlaySoundChanged(bool newValue, bool oldValue)
        {
            //throw new NotImplementedException();
        }

        private bool _CloseButtonVisible = true;
        /// <summary>
        /// Indicates whether close button on alert is visible
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether close button on alert is visible")]
        public bool CloseButtonVisible
        {
            get { return _CloseButtonVisible; }
            set
            {
                if (_CloseButtonVisible != value)
                {
                    bool oldValue = _CloseButtonVisible;
                    _CloseButtonVisible = value;
                    OnCloseButtonVisibleChanged(value, oldValue);
                }
            }
        }

        protected virtual void OnCloseButtonVisibleChanged(bool newValue, bool oldValue)
        {
            //throw new NotImplementedException();
        }
        #endregion
    }

    /// <summary>
    /// Defines the alert positions.
    /// </summary>
    public enum eAlertPosition
    {
        /// <summary>
        /// Top-left screen position.
        /// </summary>
        TopLeft,
        /// <summary>
        /// Top-right screen position.
        /// </summary>
        TopRight,
        /// <summary>
        /// Bottom-right screen position.
        /// </summary>
        BottomRight,
        /// <summary>
        /// Bottom left screen position.
        /// </summary>
        BottomLeft
    }
    /// <summary>
    /// Defines closure sources for the alert.
    /// </summary>
    public enum eAlertClosureSource
    {
        /// <summary>
        /// Alert is closed becouse of timeout
        /// </summary>
        Timeout,
        /// <summary>
        /// Alert is closed becouse user clicked close button.
        /// </summary>
        CloseButton,
        /// <summary>
        /// Alert is closed becouse user clicked on the alert.
        /// </summary>
        AlertClicked
    }
}
