 
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Rendering;

namespace DevComponents.Editors
{
    public class VisualButton : VisualButtonBase
    {
        #region Private Variables

        #endregion

        #region Events
        #endregion

        #region Constructor
        #endregion

        #region Internal Implementation
        public override void PerformLayout(PaintInfo pi)
        {
            Size size = new Size(0, (_Height > 0 ? Dpi.Height(_Height) : pi.AvailableSize.Height));
            Graphics g = pi.Graphics;

            size.Width += Dpi.Width6; // Border 2 pixels on each side and 1 pixels of padding between border and content

            if (_Text.Length > 0)
            {
                Size textSize = TextDrawing.MeasureString(g, _Text, pi.DefaultFont);
                size.Width += textSize.Width;
                if (_Image != null)
                    size.Width += Dpi.Width4; // Padding between text and image
            }

            if (!string.IsNullOrEmpty(_SymbolRealized))
            {
                size.Width += size.Height;
            }
            else if (_Image != null)
            {
                size.Width += Dpi.Width(_Image.Width);
            }

            if (_Text.Length == 0 && _Image == null && string.IsNullOrEmpty(_SymbolRealized))
                size.Width += Dpi.Width9;

            this.Size = size;
            this.CalculatedSize = size;
            base.PerformLayout(pi);
        }

        private int _Height = 0;
        /// <summary>
        /// Gets or sets the fixed button height.
        /// </summary>
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        protected override void OnPaint(PaintInfo p)
        {
            Graphics g = p.Graphics;
            Rectangle r = this.RenderBounds;
            PaintButtonBackground(p);

            Rectangle contentRect = r;
            contentRect.Inflate(-3, -3); // Two pixels border + 1 pixels padding for content
            if (!string.IsNullOrEmpty(_Text))
            {
                TextDrawing.DrawString(g, _Text, p.DefaultFont, GetIsEnabled(p) ? p.ForeColor : p.DisabledForeColor, contentRect, eTextFormat.Default | eTextFormat.VerticalCenter);
            }

            if (!string.IsNullOrEmpty(_SymbolRealized))
            {
                Office2007ButtonItemStateColorTable ct = GetOffice2007StateColorTable(p);
                Color symbolColor=(GetIsEnabled(p) ? p.ForeColor : p.DisabledForeColor);
                if (!_SymbolColor.IsEmpty)
                    symbolColor = _SymbolColor;
                else if (!ct.Text.IsEmpty)
                    symbolColor = ct.Text;
                float symbolSize = Math.Max(1, Math.Min(r.Width, r.Height) * 72 / g.DpiX - 1.5f);
                Rectangle sr = contentRect;
                if (!string.IsNullOrEmpty(_Text))
                    sr = new Rectangle(contentRect.Right - contentRect.Height, contentRect.Y, contentRect.Height, contentRect.Height);
                sr.Inflate(3, 3);
                TextDrawing.DrawStringLegacy(g, _SymbolRealized, Symbols.GetFont(symbolSize, _SymbolSet),
                    symbolColor, sr, eTextFormat.HorizontalCenter | eTextFormat.VerticalCenter);
            }
            else if (_Image != null)
            {
                Image image = GetIsEnabled(p) ? _Image : GetDisabledImage();
                Size imageSize = Dpi.ImageSize(image.Size);
                g.DrawImage(image, new Rectangle(contentRect.Right - (imageSize.Width + (string.IsNullOrEmpty(_Text)?(contentRect.Width - imageSize.Width) / 2:0)), 
                    contentRect.Y + (contentRect.Height - imageSize.Height) / 2, imageSize.Width, imageSize.Height));
            }

            base.OnPaint(p);
        }

        protected virtual void PaintButtonBackground(PaintInfo p)
        {
            PaintButtonBackground(p, GetOffice2007StateColorTable(p));
        }

        protected virtual void PaintButtonBackground(PaintInfo p, Office2007ButtonItemStateColorTable ct)
        {
            Graphics g = p.Graphics;
            Rectangle r = this.RenderBounds;
            if (RenderBackground(p))
                Office2007ButtonItemPainter.PaintBackground(g, ct, r, RoundRectangleShapeDescriptor.RectangleShape);
        }

        private bool RenderBackground(PaintInfo p)
        {
            if (RenderDefaultBackground) return true;

            if (!p.MouseOver && !this.IsMouseDown && !this.IsMouseOver && !this.Checked || !this.GetIsEnabled())
                return false;

            return true;
        }

        protected Office2007ButtonItemStateColorTable GetOffice2007StateColorTable(PaintInfo p)
        {
            if (GlobalManager.Renderer is Office2007Renderer)
            {
                Office2007ColorTable ct = ((Office2007Renderer)GlobalManager.Renderer).ColorTable;
                Office2007ButtonItemColorTable buttonColorTable = ct.ButtonItemColors[Enum.GetName(typeof(eButtonColor), eButtonColor.OrangeWithBackground)];
                if (!this.GetIsEnabled(p))
                    return buttonColorTable.Disabled;
                else if (this.IsMouseDown)
                    return buttonColorTable.Pressed;
                else if (this.IsMouseOver)
                    return buttonColorTable.MouseOver;
                else if (this.Checked)
                    return buttonColorTable.Checked;
                else
                    return buttonColorTable.Default;
            }

            return null;
        }

        protected override void OnMouseEnter()
        {
            if (this.GetIsEnabled())
                this.IsMouseOver = true;
            base.OnMouseEnter();
        }

        protected override void OnMouseLeave()
        {
            this.IsMouseOver = false;
            base.OnMouseLeave();
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.IsMouseDown = true;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this.IsMouseDown = false;
            base.OnMouseUp(e);
        }

        private bool _IsMouseOver = false;
        /// <summary>
        /// Gets whether mouse is over the control.
        /// </summary>
        public bool IsMouseOver
        {
            get { return _IsMouseOver; }
            internal set
            {
                if (_IsMouseOver != value)
                {
                    _IsMouseOver = value;
                    this.InvalidateRender();
                }
            }
        }

        private bool _IsMouseDown = false;
        /// <summary>
        /// Gets whether mouse is pressed on the control.
        /// </summary>
        public bool IsMouseDown
        {
            get { return _IsMouseDown; }
            internal set
            {
                if (_IsMouseDown != value)
                {
                    _IsMouseDown = value;
                    this.InvalidateRender();
                }
            }
        }

        private bool _Checked;
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                if (_Checked != value)
                {
                    _Checked = value;
                    this.InvalidateRender();
                }
            }
        }


        private string _Text = "";
        /// <summary>
        /// Gets or sets the text displayed on the face of the button.
        /// </summary>
        [DefaultValue("")]
        public string Text
        {
            get { return _Text; }
            set
            {
                if (value == null) value = "";
                if (_Text != value)
                {
                    _Text = value;
                    this.InvalidateArrange();
                }
            }
        }

        private Image GetDisabledImage()
        {
            if (_DisabledImage == null && _Image != null)
            {
                _DisabledImage = ImageHelper.CreateGrayScaleImage(_Image as Bitmap);
            }

            return _DisabledImage;
        }

        private Image _DisabledImage = null;
        private Image _Image = null;
        /// <summary>
        /// Gets or sets the image displayed on the face of the button.
        /// </summary>
        [DefaultValue(null)]
        public Image Image
        {
            get { return _Image; }
            set
            {
                if (_Image != value)
                {
                    _Image = value;
                    if (_DisabledImage != null)
                    {
                        _DisabledImage.Dispose();
                        _DisabledImage = null;
                    }
                    this.InvalidateArrange();
                }
            }
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
            this.InvalidateArrange();
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
            this.InvalidateArrange();
        }

        private Color _SymbolColor = Color.Empty;
        /// <summary>
        /// Gets or sets the color of the Symbol.
        /// </summary>
        [Category("Appearance"), Description("Indicates color of the Symbol.")]
        public Color SymbolColor
        {
            get { return _SymbolColor; }
            set { _SymbolColor = value; this.InvalidateRender(); }
        }
        #endregion

    }
}
 

