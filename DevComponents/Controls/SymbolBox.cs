using DevComponents.DotNetBar.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Represents the control which displays symbol from symbols library.
    /// </summary>
   [ToolboxBitmap(typeof(SymbolBox), "Controls.SymbolBox.ico"), ToolboxItem(true)]
    public class SymbolBox : Control
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the SymbolBox class.
        /// </summary>
        public SymbolBox()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
            ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor |
            ControlStyles.StandardDoubleClick | DisplayHelp.DoubleBufferFlag, true);
            _BackgroundStyle = new ElementStyle();
            _BackgroundStyle.StyleChanged += new EventHandler(this.VisualPropertyChanged);
        }
        #endregion

        #region Implementation
        protected override void OnPaint(PaintEventArgs e)
        {
            if ((this.BackColor.IsEmpty || this.BackColor == Color.Transparent))
            {
                base.OnPaintBackground(e);
            }

            if (_BackgroundStyle != null)
                _BackgroundStyle.SetColorScheme(this.GetColorScheme());

            PaintBackground(e);
            PaintContent(e);

            base.OnPaint(e);
        }

        private void PaintContent(PaintEventArgs e)
        {
            float symbolSize = _SymbolSize;
            Graphics g = e.Graphics;

            Rectangle r = ElementStyleLayout.GetInnerRect(_BackgroundStyle, this.ClientRectangle);

            if (symbolSize <= 0)
                symbolSize = Math.Max(1, Math.Min(r.Width, r.Height) * 72 / g.DpiX - 1);

            Color symbolColor = _SymbolColor;
            if (_SymbolColor.IsEmpty)
            {
                if (!_BackgroundStyle.TextColor.IsEmpty)
                    symbolColor = _BackgroundStyle.TextColor;
                else if (!this.ForeColor.IsEmpty)
                    symbolColor = this.ForeColor;
                else
                    symbolColor = SystemColors.ControlText;
            }
            TextDrawing.DrawStringLegacy(g, _SymbolRealized, Symbols.GetFont(symbolSize, _SymbolSet),
                symbolColor, r, eTextFormat.HorizontalCenter | eTextFormat.VerticalCenter);
        }

        protected virtual void PaintBackground(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle r = this.ClientRectangle;
            ElementStyle style = _BackgroundStyle;

            if (!this.BackColor.IsEmpty && this.BackColor != Color.Transparent)
            {
                DisplayHelp.FillRectangle(g, r, this.BackColor);
            }

            if (this.BackgroundImage != null)
                base.OnPaintBackground(e);

            if (style.Custom)
            {
                SmoothingMode sm = g.SmoothingMode;
                //if (m_AntiAlias)
                //    g.SmoothingMode = SmoothingMode.HighQuality;
                ElementStyleDisplayInfo displayInfo = new ElementStyleDisplayInfo(style, e.Graphics, r);
                ElementStyleDisplay.Paint(displayInfo);
                //if (m_AntiAlias)
                //    g.SmoothingMode = sm;
            }
        }

        /// <summary>
        /// Returns the color scheme used by control. Color scheme for Office2007 style will be retrieved from the current renderer instead of
        /// local color scheme referenced by ColorScheme property.
        /// </summary>
        /// <returns>An instance of ColorScheme object.</returns>
        protected virtual ColorScheme GetColorScheme()
        {
            BaseRenderer r = Rendering.GlobalManager.Renderer;
            if (r is Office2007Renderer)
                return ((Office2007Renderer)r).ColorTable.LegacyColors;
            return new ColorScheme();
        }

        private ElementStyle _BackgroundStyle = null;
        /// <summary>
        /// Specifies the background style of the control.
        /// </summary>
        [Browsable(true), Category("Style"), Description("Gets or sets bar background style."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ElementStyle BackgroundStyle
        {
            get { return _BackgroundStyle; }
        }

        /// <summary>
        /// Resets style to default value. Used by windows forms designer.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetBackgroundStyle()
        {
            _BackgroundStyle.StyleChanged -= new EventHandler(this.VisualPropertyChanged);
            _BackgroundStyle = new ElementStyle();
            _BackgroundStyle.StyleChanged += new EventHandler(this.VisualPropertyChanged);
            this.Invalidate();
        }
        private void VisualPropertyChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        /// <summary>
        /// Gets the realized symbol string.
        /// </summary>
        [Browsable(false)]
        public string SymbolRealized
        {
            get { return _SymbolRealized; }
        }
        private string _Symbol = "\uf015", _SymbolRealized = "\uf015";
        /// <summary>
        /// Indicates the symbol displayed on face of the button instead of the image. Setting the symbol overrides the image setting.
        /// </summary>
        [DefaultValue("\uf015"), Category("Appearance"), Description("Indicates the symbol displayed on face of the button instead of the image. Setting the symbol overrides the image setting.")]
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

        private float _SymbolSize = 0f;
        /// <summary>
        /// Indicates the size of the symbol in points.
        /// </summary>
        [DefaultValue(0f), Category("Appearance"), Description("Indicates the size of the symbol in points.")]
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
            this.Invalidate();
        }
        #endregion
    }
}
