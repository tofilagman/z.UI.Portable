using DevComponents.DotNetBar.Primitives;
using DevComponents.DotNetBar.Rendering;
using DevComponents.DotNetBar.TextMarkup;
using DevComponents.Editors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar.Controls
{
    [ToolboxItem(true), ToolboxBitmap(typeof(TokenEditor), "Controls.TokenEditor.ico")]
    [Designer("DevComponents.DotNetBar.Design.TokenEditorDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf")]
    [DefaultEvent("SelectedTokensChanged")]
    public class TokenEditor : Control, IMessageHandlerClient
    {
        #region Constructor
        private TextBoxX _EditBox;
        private VScrollBarAdv _VScrollBar = null;
        /// <summary>
        /// Initializes a new instance of the TokenEditor class.
        /// </summary>
        public TokenEditor()
        {
            _SelectedTokens = new CustomCollection<EditToken>();
            _SelectedTokens.CollectionChanged += SelectedTokensCollectionChanged;
            _Tokens = new CustomCollection<EditToken>();
            _Tokens.CollectionChanged += TokensCollectionChanged;
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
            ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.Selectable |
            ControlStyles.StandardDoubleClick | DisplayHelp.DoubleBufferFlag, true);
            _BackgroundStyle = new ElementStyle();
            _BackgroundStyle.Class = ElementStyleClassKeys.DateTimeInputBackgroundKey;
            _BackgroundStyle.StyleChanged += new EventHandler(this.VisualPropertyChanged);

            _EditBox = new TextBoxX();
            _EditBox.BorderStyle = BorderStyle.None;
            _EditBox.Visible = false;
            _EditBox.TextChanged += EditBoxTextChanged;
            _EditBox.KeyDown += EditBoxKeyDown;
            _EditBox.GotFocus += EditBoxGotFocus;
            this.Controls.Add(_EditBox);
            _EditBox.SetAutoHeight();

            _VScrollBar = new VScrollBarAdv();
            _VScrollBar.Visible = false;
            _VScrollBar.Scroll += VScrollBarScroll;
            this.Controls.Add(_VScrollBar);

            VisualGroup group = new VisualGroup();
            group.HorizontalItemSpacing = 0;
            group.ArrangeInvalid += ButtonGroupArrangeInvalid;
            group.RenderInvalid += ButtonGroupRenderInvalid;
            group.ResetMouseHover += ButtonGroupResetMouseHover;
            _ButtonGroup = group;
            CreateButtons();
        }

        protected override void Dispose(bool disposing)
        {
            if (_MessageHandlerInstalled)
            {
                MessageHandler.UnregisterMessageClient(this);
                _MessageHandlerInstalled = false;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Implementation

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            if (Dpi.RecordScalePerControl)
                Dpi.SetScaling(factor);
            base.ScaleControl(factor, specified);
            _EditBox.SetAutoHeight();
            LayoutTokens();
        }

        private void EditBoxGotFocus(object sender, EventArgs e)
        {
            FocusToken = null;
        }
        private TokenEditorColorTable GetTokenColorTable()
        {
            return ((Office2007Renderer)GlobalManager.Renderer).ColorTable.TokenEditor;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if ((this.BackColor.IsEmpty || this.BackColor == Color.Transparent))
            {
                base.OnPaintBackground(e);
            }

            if (_BackgroundStyle != null)
                _BackgroundStyle.SetColorScheme(this.GetColorScheme());
            TokenEditorColorTable colorTable = GetTokenColorTable();
            PaintBackground(e);
            Rectangle clientBounds = GetClientBounds();
            e.Graphics.SetClip(clientBounds);

            if (this.WatermarkEnabled && this.WatermarkText.Length > 0 && this.IsWatermarkRendered)
            {
                DrawWatermark(e.Graphics);
            }

            foreach (EditToken token in _SelectedTokens)
            {
                PaintToken(token, e, colorTable);
            }

            PaintButtons(e.Graphics);

            e.Graphics.ResetClip();
            base.OnPaint(e);
        }

        private void PaintToken(EditToken token, PaintEventArgs e, TokenEditorColorTable colorTable)
        {
            Graphics g = e.Graphics;
            Rectangle r = token.Bounds;
            r.Offset(_AutoScrollPosition);

            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.HighQuality;

            Color textColor = colorTable.Normal.TextColor;
            LinearGradientColorTable background = colorTable.Normal.Background;

            if (token.IsFocused)
            {
                textColor = colorTable.Focused.TextColor;
                background = colorTable.Focused.Background;
            }
            else if (token.MouseOverPart == eTokenPart.Token || token.MouseOverPart == eTokenPart.Image)
                background = colorTable.MouseOver.Background;

            DisplayHelp.FillRoundedRectangle(g, r, 2, background.Start, background.End, background.GradientAngle);

            g.SmoothingMode = sm;
            string text = GetTokenText(token);
            Rectangle rText = r;

            if (EffectiveRemoveTokenButtonVisible)
            {
                Rectangle removeBounds = new Rectangle(r.X, r.Y + (r.Height - _RemoveTokenButtonSize.Height) / 2,
                    _RemoveTokenButtonSize.Width, _RemoveTokenButtonSize.Height);
                Color closeTextColor = textColor;
                if (token.MouseOverPart == eTokenPart.RemoveButton)
                {
                    Office2007ColorTable ct = ((Office2007Renderer)GlobalManager.Renderer).ColorTable;
                    Office2007ButtonItemColorTable buttonColorTable = ct.ButtonItemColors[Enum.GetName(typeof(eButtonColor), eButtonColor.OrangeWithBackground)];
                    Office2007ButtonItemPainter.PaintBackground(g, buttonColorTable.MouseOver, removeBounds, RoundRectangleShapeDescriptor.RectangleShape);
                    closeTextColor = buttonColorTable.MouseOver.Text;
                    //g.FillRectangle(Brushes.Red, removeBounds);
                }

                float symbolSize = Math.Max(1, Math.Min(removeBounds.Width, removeBounds.Height) * 72 / g.DpiX - 1.5f);
                TextDrawing.DrawStringLegacy(g, "\uf00d", Symbols.GetFont(symbolSize, eSymbolSet.Awesome),
                    closeTextColor, removeBounds, eTextFormat.HorizontalCenter | eTextFormat.VerticalCenter);

                token.RemoveButtonBounds = removeBounds;
                rText.X = removeBounds.Right + _TokenPartSpacing;
                rText.Width -= rText.Right - r.Right;
            }

            if (!string.IsNullOrEmpty(token.SymbolRealized))
            {
                Rectangle imageBounds = new Rectangle(rText.X, r.Y, r.Height, r.Height);
                float symbolSize = Math.Max(1, Math.Min(imageBounds.Width, imageBounds.Height) * 72 / g.DpiX - 1.5f);
                TextDrawing.DrawStringLegacy(g, token.SymbolRealized, Symbols.GetFont(symbolSize, token.SymbolSet),
                    (token.SymbolColor.IsEmpty ? textColor : token.SymbolColor),
                    imageBounds, eTextFormat.HorizontalCenter | eTextFormat.VerticalCenter);
                token.ImageBounds = imageBounds;
                rText.X = imageBounds.Right + _TokenPartSpacing;
                rText.Width -= rText.Right - r.Right;
            }
            else if (token.Image != null)
            {
                Rectangle imageBounds = new Rectangle(rText.X, r.Y, token.ImageBounds.Width, token.Image.Height);
                g.DrawImage(token.Image, imageBounds);
                token.ImageBounds = imageBounds;
                rText.X = imageBounds.Right + _TokenPartSpacing;
                rText.Width -= rText.Right - r.Right;
            }
            TextDrawing.DrawString(g, text, this.Font, textColor, rText, eTextFormat.Default | eTextFormat.VerticalCenter);
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

        protected override void OnResize(EventArgs e)
        {
            LayoutTokens();
            base.OnResize(e);
        }

        /// <summary>
        /// Gets or sets the location of the auto-scroll position.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Description("Indicates location of the auto-scroll position.")]
        public Point AutoScrollPosition
        {
            get
            {
                return _AutoScrollPosition;
            }
            set
            {
                if (_AutoScrollPosition == value) return;
                _AutoScrollPosition = value;
                if (_VScrollBar != null && _VScrollBar.Value != -_AutoScrollPosition.Y)
                    _VScrollBar.Value = Math.Min(_VScrollBar.Maximum, Math.Max(_VScrollBar.Minimum, -_AutoScrollPosition.Y));
                RepositionEditTextBox();
                Invalidate();
            }
        }

        private Rectangle GetClientBounds()
        {
            bool disposeStyle = false;
            ElementStyle style = ElementStyleDisplay.GetElementStyle(_BackgroundStyle, out disposeStyle);
            Rectangle clientRect = this.ClientRectangle;
            clientRect.X += style.PaddingLeft;
            clientRect.Width -= style.PaddingHorizontal;
            clientRect.Y += style.PaddingTop;
            clientRect.Height -= style.PaddingVertical;
            if (disposeStyle)
                style.Dispose();
            style = null;

            return clientRect;
        }

        private Point _AutoScrollPosition = Point.Empty;
        private int _TokenSpacing = 2;
        private int _TokenPartSpacing = 2; // Spacing between different parts of the token, like between image and text and image and close button etc. Its spacing between each part.
        private readonly Size _TokenPadding = new Size(2, 2);
        private void LayoutTokens()
        {
            if (!this.IsHandleCreated || this.IsDisposed || this.Width == 0 || this.Height == 0) return;

            int lineCount = 0;
            Rectangle clientRect = GetClientBounds();
            Graphics g = this.CreateGraphics();
            Size totalSize = LayoutTokens(g, clientRect, out lineCount);

            if (_AutoSizeHeight)
            {
                int heightLines = Math.Min(_MaxHeightLines, lineCount);
                int newHeight = (int)(Math.Ceiling((double)totalSize.Height / lineCount) * heightLines) + (this.Height - clientRect.Height);
                if (_MaxHeightLines == 1)
                    newHeight = GetSingleLineTextBoxHeight();
                if (newHeight != this.Height)
                {
                    g.Dispose();
                    this.Height = newHeight;
                    return;
                }
            }

            if (totalSize.Height > clientRect.Height)
            {
                Rectangle reducedBounds = clientRect;

                int visibleLines = (int)(clientRect.Height / (Math.Ceiling((double)totalSize.Height / lineCount)));
                if (_DropDownButtonVisible && visibleLines <= 2 || visibleLines == 1)
                {
                    // Using scroll button due to space limitation
                    _VScrollBar.Visible = false;
                    _ScrollButton.Visible = true;
                    reducedBounds.Width -= SystemInformation.VerticalScrollBarWidth * (_DropDownButtonVisible ? 2 : 1);
                    if (visibleLines > 1)
                        _ButtonGroup.Orientation = eOrientation.Vertical;
                    else
                        _ButtonGroup.Orientation = eOrientation.Horizontal;
                }
                else
                {
                    int buttonOffset = (_DropDownButtonVisible ? GetButtonHeight() + 1 : 0);
                    // Activate Scroll-bar
                    _VScrollBar.Bounds = new Rectangle(clientRect.Right - SystemInformation.VerticalScrollBarWidth,
                        clientRect.Y + buttonOffset,
                        SystemInformation.VerticalScrollBarWidth,
                        clientRect.Height - buttonOffset);
                    _VScrollBar.Visible = true;
                    _ScrollButton.Visible = false;
                    reducedBounds.Width -= _VScrollBar.Width;
                }

                totalSize = LayoutTokens(g, reducedBounds, out lineCount);

                _VScrollBar.Maximum = totalSize.Height;
                _VScrollBar.LargeChange = clientRect.Height;
                if (this.SelectedTokens.Count > 0)
                    _VScrollBar.SmallChange = this.SelectedTokens[0].Bounds.Height;
                else
                    _VScrollBar.SmallChange = 17;
            }
            else
            {
                _VScrollBar.Visible = false;
                _AutoScrollPosition = Point.Empty;
                _ScrollButton.Visible = false;
            }

            g.Dispose();

            RepositionEditTextBox();
            EnsureTextBoxScrollPosition();
            UpdateButtons();

            this.Invalidate();
        }

        private int GetSingleLineTextBoxHeight()
        {
            Font font = this.Font;
            return font.Height + ((SystemInformation.BorderSize.Height * 4) + 4);
        }
        private Size LayoutTokens(Graphics g, Rectangle clientRect, out int lineCount)
        {
            lineCount = 0;
            if (!this.IsHandleCreated || this.IsDisposed) return Size.Empty;

            clientRect.Inflate(-1, -1);
            Font font = this.Font;
            Size totalSize = Size.Empty;
            int singleLineTextBoxHeight = GetSingleLineTextBoxHeight() - 2; // 2 is for top and bottom border

            if (_SelectedTokens.Count == 0)
            {
                lineCount = 1;
                totalSize = new Size(clientRect.Width, singleLineTextBoxHeight);
                _EditBoxLocation = new Point(clientRect.X, clientRect.Y + (totalSize.Height - _EditBox.Height) / 2);
                _EditBox.Width = clientRect.Width - (_DropDownButtonVisible ? SystemInformation.VerticalScrollBarWidth : 0);

                return totalSize;
            }

            int largestTokenHeight = 0;
            foreach (EditToken token in _SelectedTokens)
            {
                token.RemoveButtonBounds = Rectangle.Empty;
                token.ImageBounds = Rectangle.Empty;
                string text = GetTokenText(token);
                Size size = TextDrawing.MeasureString(g, (string.IsNullOrEmpty(text) ? "A" : text), font);
                size.Width += _TokenPadding.Width * 2;
                size.Height += _TokenPadding.Height * 2;
                if (EffectiveRemoveTokenButtonVisible)
                {
                    size.Width += _RemoveTokenButtonSize.Width + _TokenPartSpacing;
                    if (_RemoveTokenButtonSize.Height > size.Height)
                        size.Height = _RemoveTokenButtonSize.Height;
                }

                if (!string.IsNullOrEmpty(token.SymbolRealized))
                {
                    size.Width += size.Height + _TokenPartSpacing;
                }
                else if (token.Image != null)
                {
                    size.Width += token.Image.Width + _TokenPartSpacing;
                    if (token.Image.Height > size.Height)
                        size.Height = token.Image.Height;
                }
                largestTokenHeight = Math.Max(largestTokenHeight, size.Height);
                token.Bounds = new Rectangle(Point.Empty, size);
            }

            lineCount = 1;
            totalSize.Height = largestTokenHeight;
            Point currentPos = clientRect.Location;

            foreach (EditToken token in _SelectedTokens)
            {
                Rectangle tokenBounds = new Rectangle(currentPos.X, currentPos.Y, token.Bounds.Width, largestTokenHeight);
                if (tokenBounds.Right > clientRect.Right && currentPos.X > clientRect.X)
                {
                    currentPos.Y += largestTokenHeight + _TokenSpacing;
                    currentPos.X = clientRect.X;
                    tokenBounds.Y = currentPos.Y;
                    tokenBounds.X = currentPos.X;
                    totalSize.Height += largestTokenHeight + _TokenSpacing;
                    lineCount++;
                }
                currentPos.X += tokenBounds.Width + _TokenSpacing;
                token.Bounds = tokenBounds;

                if (tokenBounds.Right - clientRect.X > totalSize.Width)
                    totalSize.Width = tokenBounds.Right - clientRect.X;
            }

            if (clientRect.Right - currentPos.X < _TextBoxMinWidth)
            {
                currentPos.Y += largestTokenHeight + _TokenSpacing;
                currentPos.X = clientRect.X;
                lineCount++;
                totalSize.Height += largestTokenHeight + _TokenSpacing;
            }

            if (lineCount == 1)
                totalSize.Height = Math.Max(totalSize.Height, singleLineTextBoxHeight);

            _EditBoxLocation = new Point(currentPos.X, currentPos.Y + (largestTokenHeight - _EditBox.Height) / 2);
            _EditBox.Width = clientRect.Right - currentPos.X - (lineCount == 1 && _DropDownButtonVisible ? SystemInformation.VerticalScrollBarWidth : 0);

            return totalSize;
        }

        private Point _EditBoxLocation = Point.Empty;
        private void RepositionEditTextBox()
        {
            Point loc = _EditBoxLocation;
            loc.Offset(_AutoScrollPosition);
            _EditBox.Location = loc;
        }

        private void VScrollBarScroll(object sender, ScrollEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                _AutoScrollPosition.Y = -e.NewValue;
                RepositionEditTextBox();
                this.Invalidate();
            }
        }
        private const int _TextBoxMinWidth = 36;

        protected override void OnHandleCreated(EventArgs e)
        {
            _EditBox.SetAutoHeight();
            LayoutTokens();
            base.OnHandleCreated(e);
        }

        private string GetTokenText(EditToken token)
        {
            if (!string.IsNullOrEmpty(token.Text))
                return token.Text;
            return token.Value;
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

        private CustomCollection<EditToken> _SelectedTokens;
        /// <summary>
        /// Gets the collection of the selected tokens.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CustomCollection<EditToken> SelectedTokens
        {
            get
            {
                return _SelectedTokens;
            }
        }
        private void SelectedTokensCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    ((EditToken)item).IsSelected = true;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object item in e.OldItems)
                {
                    ((EditToken)item).IsSelected = false;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (object item in e.NewItems)
                {
                    ((EditToken)item).IsSelected = true;
                }
                foreach (object item in e.OldItems)
                {
                    ((EditToken)item).IsSelected = false;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (EditToken item in _Tokens)
                {
                    item.IsSelected = false;
                }
            }

            LayoutTokens();
            UpdateText();
            UpdateEditBoxWatermark();
            OnSelectedTokensChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when SelectedTokens collection changes.
        /// </summary>
        [Description("Occurs when SelectedTokens collection changes.")]
        public event EventHandler SelectedTokensChanged;
        /// <summary>
        /// Raises SelectedTokensChanged event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnSelectedTokensChanged(EventArgs e)
        {
            EventHandler handler = SelectedTokensChanged;
            if (handler != null)
                handler(this, e);
        }

        private CustomCollection<EditToken> _Tokens;
        /// <summary>
        /// Gets the collection of the tokens available for selection.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CustomCollection<EditToken> Tokens
        {
            get
            {
                return _Tokens;
            }
        }
        private void TokensCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private bool EffectiveRemoveTokenButtonVisible
        {
            get
            {
                return _RemoveTokenButtonVisible && !_ReadOnly;
            }
        }
        private Size _RemoveTokenButtonSize = new Size(14, 14);
        private bool _RemoveTokenButtonVisible = true;
        /// <summary>
        /// Indicates whether remove token button is displayed on individual tokens so they can be removed from the selection.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether remove token button is displayed on individual tokens so they can be removed from the selection.")]
        public bool RemoveTokenButtonVisible
        {
            get { return _RemoveTokenButtonVisible; }
            set
            {
                if (value != _RemoveTokenButtonVisible)
                {
                    bool oldValue = _RemoveTokenButtonVisible;
                    _RemoveTokenButtonVisible = value;
                    OnRemoveTokenButtonVisibleChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when RemoveTokenButtonVisible property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnRemoveTokenButtonVisibleChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("RemoveTokenButtonVisible"));
            if (_SelectedTokens.Count > 0)
                LayoutTokens();
        }

        /// <summary>
        /// Returns the token from SelectedTokens at specified position or null/nothing if no token is at given location.
        /// </summary>
        /// <param name="p">Location in client coordinates to test.</param>
        /// <returns>EditToken instance or null/nothing</returns>
        public EditToken GetSelectedTokenAt(Point p)
        {
            foreach (EditToken token in _SelectedTokens)
            {
                Rectangle tokenBounds = token.Bounds;
                tokenBounds.Offset(_AutoScrollPosition);
                if (tokenBounds.Contains(p))
                    return token;
            }
            return null;
        }

        private EditToken _MouseOverToken = null;
        private EditToken MouseOverToken
        {
            get
            {
                return _MouseOverToken;
            }
            set
            {
                if (_MouseOverToken != value)
                {
                    if (_MouseOverToken != null)
                        _MouseOverToken = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Occurs when mouse enters one of the SelectedTokens token.
        /// </summary>
        [Description("Occurs when mouse enters one of the SelectedTokens token.")]
        public event EventHandler TokenMouseEnter;
        /// <summary>
        /// Raises TokenMouseEnter event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnTokenMouseEnter(object sender, EventArgs e)
        {
            EventHandler handler = TokenMouseEnter;
            if (handler != null)
                handler(sender, e);
        }
        /// <summary>
        /// Occurs when mouse leaves one of the SelectedTokens token.
        /// </summary>
        [Description("Occurs when mouse leaves one of the SelectedTokens token.")]
        public event EventHandler TokenMouseLeave;
        /// <summary>
        /// Raises TokenMouseLeave event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnTokenMouseLeave(object sender, EventArgs e)
        {
            HideToolTip();
            EventHandler handler = TokenMouseLeave;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Occurs when mouse clicks one of the SelectedTokens token.
        /// </summary>
        [Description("Occurs when mouse clicks one of the SelectedTokens token.")]
        public event MouseEventHandler TokenMouseClick;
        /// <summary>
        /// Raises TokenMouseClick event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnTokenMouseClick(object sender, MouseEventArgs e)
        {
            MouseEventHandler handler = TokenMouseClick;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Occurs when mouse double clicks one of the SelectedTokens token.
        /// </summary>
        [Description("Occurs when mouse double clicks one of the SelectedTokens token.")]
        public event MouseEventHandler TokenMouseDoubleClick;
        /// <summary>
        /// Raises TokenMouseClick event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnTokenMouseDoubleClick(object sender, MouseEventArgs e)
        {
            MouseEventHandler handler = TokenMouseDoubleClick;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Occurs when mouse hovers one of the SelectedTokens token.
        /// </summary>
        [Description("Occurs when mouse hovers one of the SelectedTokens token.")]
        public event EventHandler TokenMouseHover;
        /// <summary>
        /// Raises TokenMouseHover event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnTokenMouseHover(object sender, EventArgs e)
        {
            EventHandler handler = TokenMouseHover;
            if (handler != null)
                handler(sender, e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            EditToken token = GetSelectedTokenAt(e.Location);
            if (token != _MouseOverToken)
            {
                if (_MouseOverToken != null)
                {
                    _MouseOverToken.MouseOverPart = eTokenPart.None;
                    OnTokenMouseLeave(_MouseOverToken, e);
                }
                _MouseOverToken = token;
                this.Invalidate();
                DevComponents.AdvTree.Interop.WinApi.ResetHover(this);
                if (_MouseOverToken != null)
                    OnTokenMouseEnter(_MouseOverToken, e);
            }
            if (_MouseOverToken != null)
            {
                eTokenPart oldMouseOverPart = _MouseOverToken.MouseOverPart;
                if (EffectiveRemoveTokenButtonVisible && !_MouseOverToken.RemoveButtonBounds.IsEmpty && _MouseOverToken.RemoveButtonBounds.Contains(e.Location))
                    _MouseOverToken.MouseOverPart = eTokenPart.RemoveButton;
                else if (!_MouseOverToken.ImageBounds.IsEmpty && _MouseOverToken.ImageBounds.Contains(e.Location))
                    _MouseOverToken.MouseOverPart = eTokenPart.Image;
                else
                    _MouseOverToken.MouseOverPart = eTokenPart.Token;
                if (oldMouseOverPart != _MouseOverToken.MouseOverPart)
                    this.Invalidate();
            }
            if (_ButtonGroup.Visible)
                _ButtonGroup.ProcessMouseMove(e);
            base.OnMouseMove(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            HideToolTip();
            if (_MouseOverToken == null && !_ReadOnly && !(_ButtonGroup.Visible && _ButtonGroup.RenderBounds.Contains(e.Location)))
            {
                ShowEditTextBox();
            }
            else
                this.Select();
            if (_ButtonGroup.Visible)
                _ButtonGroup.ProcessMouseDown(e);
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_ButtonGroup.Visible)
                _ButtonGroup.ProcessMouseUp(e);
            base.OnMouseUp(e);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (_MouseOverToken != null && _MouseOverToken.MouseOverPart == eTokenPart.RemoveButton)
            {
                EditToken token = _MouseOverToken;
                RemovingTokenEventArgs args = new RemovingTokenEventArgs(token, eEventSource.Mouse);
                OnRemovingToken(args);
                if (args.Cancel) return;

                LeaveMouseOverToken();
                this.SelectedTokens.Remove(token);
            }
            else if (_MouseOverToken != null)
            {
                OnTokenMouseClick(_MouseOverToken, e);
            }
            if (_ButtonGroup.Visible)
                _ButtonGroup.ProcessMouseClick(e);
            base.OnMouseClick(e);
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (_MouseOverToken != null)
            {
                OnTokenMouseDoubleClick(_MouseOverToken, e);
            }
            base.OnMouseDoubleClick(e);
        }
        protected override void OnMouseHover(EventArgs e)
        {
            if (_MouseOverToken != null)
            {
                OnTokenMouseHover(_MouseOverToken, e);
                ShowToolTip(_MouseOverToken);
            }

            if (_ButtonGroup.Visible)
                _ButtonGroup.ProcessMouseHover(e);
            base.OnMouseHover(e);
        }
        /// <summary>
        /// Occurs before token is removed from the SelectedTokens by end user.
        /// </summary>
        [Description("Occurs before token is removed from the SelectedTokens by end user.")]
        public event RemovingTokenEventHandler RemovingToken;
        /// <summary>
        /// Raises RemovingToken event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnRemovingToken(RemovingTokenEventArgs e)
        {
            RemovingTokenEventHandler handler = RemovingToken;
            if (handler != null)
                handler(this, e);
        }
        private void LeaveMouseOverToken()
        {
            HideToolTip();
            if (_MouseOverToken != null)
            {
                _MouseOverToken.MouseOverPart = eTokenPart.None;
                OnTokenMouseLeave(_MouseOverToken, EventArgs.Empty);
                _MouseOverToken = null;
                this.Invalidate();
            }
        }

        private bool _IsMouseOver = false;
        protected override void OnMouseLeave(EventArgs e)
        {
            _IsMouseOver = false;
            LeaveMouseOverToken();
            if (_ButtonGroup.Visible)
                _ButtonGroup.ProcessMouseLeave();
            base.OnMouseLeave(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            _IsMouseOver = true;
            base.OnMouseEnter(e);
        }

        private EditToken _FocusToken = null;
        private EditToken FocusToken
        {
            get { return _FocusToken; }
            set
            {
                if (_FocusToken != value)
                {
                    if (_FocusToken != null)
                        _FocusToken.IsFocused = false;
                    _FocusToken = value;
                    if (_FocusToken != null)
                        _FocusToken.IsFocused = true;
                    this.Invalidate();
                }
            }
        }

        private void FocusPreviousToken()
        {
            if (this.SelectedTokens.Count == 0) return;
            int index = _SelectedTokens.Count - 1;
            if (_FocusToken != null)
                index = Math.Max(0, _SelectedTokens.IndexOf(_FocusToken) - 1);
            FocusToken = _SelectedTokens[index];
        }
        private void FocusNextToken()
        {
            if (this.SelectedTokens.Count == 0) return;
            int index = 0;
            if (_FocusToken != null)
            {
                if (_SelectedTokens.IndexOf(_FocusToken) == _SelectedTokens.Count - 1)
                {
                    FocusToken = null;
                    _EditBox.Select();
                    return;
                }
                index = Math.Min(_SelectedTokens.Count - 1, _SelectedTokens.IndexOf(_FocusToken) + 1);
            }
            FocusToken = _SelectedTokens[index];
        }
        private void DeleteFocusedToken(bool isBackspace, eEventSource eventSource)
        {
            EditToken focusToken = _FocusToken;
            if (focusToken == null) return;
            RemovingTokenEventArgs e = new RemovingTokenEventArgs(focusToken, eventSource);
            OnRemovingToken(e);
            if (e.Cancel) return;
            FocusToken = null;
            int index = Math.Max(0, _SelectedTokens.IndexOf(focusToken) - 1);
            _SelectedTokens.Remove(focusToken);
            if (_SelectedTokens.Count > 0)
            {
                FocusToken = _SelectedTokens[index];
            }
            else
            {
                _EditBox.Select();
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!_EditBox.Focused)
            {
                if (keyData == Keys.Left)
                {
                    FocusPreviousToken();
                    return true;
                }
                else if (keyData == Keys.Right)
                {
                    FocusNextToken();
                    return true;
                }
                else if (keyData == Keys.Back)
                {
                    DeleteFocusedToken(true, eEventSource.Keyboard);
                    return true;
                }
                else if (keyData == Keys.Delete)
                {
                    DeleteFocusedToken(false, eEventSource.Keyboard);
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (_FocusToken != null)
            {
                FocusToken = null;
                _EditBox.Select();
            }
            base.OnKeyDown(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            if (_EditBox.Visible)
            {
                _EditBox.Visible = false;
                if (_ValidateTokenTextOnLostFocus && !string.IsNullOrEmpty(_EditBox.Text))
                {
                    ValidateAndConvertEditText(_EditBox.Text);
                }
            }
            _EditBox.Text = "";
            FocusToken = null;
            if (IsPopupOpen)
                CloseAutoCompleteDropDown();
            HideToolTip();
            base.OnLeave(e);
        }
        protected override void OnEnter(EventArgs e)
        {
            if (!_ReadOnly)
            {
                ShowEditTextBox();
            }
            base.OnEnter(e);
        }

        private bool _ReadOnly = false;
        /// <summary>
        /// Indicates whether tokens can be added or removed by end user. Default value is false.
        /// </summary>
        [DefaultValue(false), Category("Behavior"), Description("Indicates whether tokens can be added or removed by end user. Default value is false.")]
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set
            {
                if (value != _ReadOnly)
                {
                    bool oldValue = _ReadOnly;
                    _ReadOnly = value;
                    OnReadOnlyChanged(oldValue, value);
                }
            }
        }
        private void ShowEditTextBox()
        {
            _EditBox.Visible = true;
            _EditBox.Focus();
            EnsureTextBoxScrollPosition();
        }
        private void EnsureTextBoxScrollPosition()
        {
            if (!_EditBox.Visible) return;
            Rectangle clientBounds = GetClientBounds();
            if (clientBounds.Contains(_EditBox.Bounds))
                return;
            // Scroll content vertically to ensure text-box is withing client bounds
            AutoScrollPosition = new Point(0, AutoScrollPosition.Y + (clientBounds.Bottom - _EditBox.Bounds.Bottom));
        }
        /// <summary>
        /// Called when ReadOnly property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnReadOnlyChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("ReadOnly"));
            if (_ReadOnly)
            {
                if (_EditBox.Visible)
                    _EditBox.Visible = false;
            }
            else if (this.Focused)
            {
                ShowEditTextBox();
                _EditBox.Focus();
            }
            if (_RemoveTokenButtonVisible)
                LayoutTokens();
        }

        private int _DropDownHeight = 120;
        /// <summary>
        /// Indicates the height of the auto-complete drop-down.
        /// </summary>
        [DefaultValue(120), Category("Appearance"), Description("Indicates the height of the auto-complete drop-down")]
        public int DropDownHeight
        {
            get { return _DropDownHeight; }
            set
            {
                if (value != _DropDownHeight)
                {
                    int oldValue = _DropDownHeight;
                    _DropDownHeight = value;
                    OnDropDownHeightChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when DropDownHeight property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnDropDownHeightChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("DropDownHeight"));   
        }

        private int _DropDownWidth = 180;
        /// <summary>
        /// Indicates the width of the auto-complete drop-down.
        /// </summary>
        [DefaultValue(180), Category("Appearance"), Description("Indicates the width of the auto-complete drop-down.")]
        public int DropDownWidth
        {
            get { return _DropDownWidth; }
            set
            {
                if (value != _DropDownWidth)
                {
                    int oldValue = _DropDownWidth;
                    _DropDownWidth = value;
                    OnDropDownWidthChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when DropDownWidth property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnDropDownWidthChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("DropDownWidth"));

        }

        private bool _EnterKeyValidatesToken = true;
        /// <summary>
        /// Indicates whether when token text is entered into the text-box pressing the Enter key attempts to validate the token and converts the text to token.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether when token text is entered into the text-box pressing the Enter key attempts to validate the token and converts the text to token.")]
        public bool EnterKeyValidatesToken
        {
            get
            {
                return _EnterKeyValidatesToken;
            }
            set
            {
                _EnterKeyValidatesToken = value;
            }
        }

        private void EditBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && IsPopupOpen)
            {
                CloseAutoCompleteDropDown();
            }
            else if (e.KeyCode == Keys.Enter && IsPopupOpen && _AutoCompleteListBox != null && _AutoCompleteListBox.SelectedIndex >= 0)
            {
                SelectToken(_Tokens[_AutoCompleteListBox.SelectedIndex]);
            }
            else if (e.KeyCode == Keys.Enter && EnterKeyValidatesToken)
            {
                ValidateAndConvertEditText(_EditBox.Text);
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (IsPopupOpen && _AutoCompleteListBox != null)
                {
                    _AutoCompleteListBox.SelectNextItem();
                    e.Handled = true;
                }
                else if (!IsPopupOpen && string.IsNullOrEmpty(_EditBox.Text))
                {
                    IsPopupOpen = true;
                    e.Handled = true;
                }
            }
            else if (_AutoCompleteListBox != null && e.KeyCode == Keys.Up)
            {
                _AutoCompleteListBox.SelectPreviousItem();
                e.Handled = true;
            }
            else if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.Back) && string.IsNullOrEmpty(_EditBox.Text))
            {
                CloseAutoCompleteDropDown();
                if (this.SelectedTokens.Count > 0)
                {
                    this.Select();
                    FocusPreviousToken();
                }
                e.Handled = true;
            }

        }


        private void AutoCompleteListBoxItemClick(object sender, EventArgs e)
        {
            if (_AutoCompleteListBox.SelectedIndex >= 0)
                SelectToken(_Tokens[_AutoCompleteListBox.SelectedIndex]);
        }

        private bool SelectToken(EditToken token)
        {
            if (token.IsSelected) return false;

            ValidateTokenEventArgs args = new ValidateTokenEventArgs(token, false);
            if (args.IsValid && args.Token != null)
            {
                token = args.Token;
                this.SelectedTokens.Add(token);
                _EditBox.Text = "";
                CloseAutoCompleteDropDown();
                return true;
            }
            return false;
        }

        private bool ValidateAndConvertEditText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            IsPopupOpen = false;
            bool isNewToken = false;
            EditToken token = FindOrCreateToken(text, out isNewToken);
            ValidateTokenEventArgs args = new ValidateTokenEventArgs(token, isNewToken);
            OnValidateToken(args);
            if (args.IsValid && args.Token != null)
            {
                token = args.Token;
                this.SelectedTokens.Add(token);
                _EditBox.Text = "";
                CloseAutoCompleteDropDown();
                return true;
            }
            return false;
        }

        private bool IsSelected(EditToken item)
        {
            return item.IsSelected;

            //foreach (EditToken selectedToken in _SelectedTokens)
            //{
            //    if(selectedToken == item)
            //        return true;
            //}
            //return false;
        }
        private EditToken FindOrCreateToken(string text, out bool isNew)
        {
            isNew = false;
            foreach (EditToken item in _Tokens)
            {
                if (!item.IsSelected && TokenExactMatch(text, item, _TokenFilterBehavior))
                {
                    return item;
                }
            }
            isNew = true;
            return new EditToken(text);
        }

        private void EditBoxTextChanged(object sender, EventArgs e)
        {
            string text = _EditBox.Text;

            if (!_DroppedDown && FilteredTokenExists(text))
            {
                OpenAutoCompleteDropDown();
            }
            if (_AutoCompleteListBox != null)
            {
                FilterAutoCompleteBox(_AutoCompleteListBox, text);
            }

            foreach (string separator in _Separators)
            {
                if (text.EndsWith(separator))
                {
                    text = text.Substring(0, text.Length - separator.Length);
                    ValidateAndConvertEditText(text);
                    break;
                }
            }
        }

        private PopupHostController _PopupController = null;
        private ListBoxAdv _AutoCompleteListBox = null;
        private DateTime _MultiDropDownOpenedAtTime = DateTime.MinValue;
        private DateTime _MultiDropDownClosedAtTime = DateTime.MinValue;

        /// <summary>
        /// Occurs before token auto-complete popup is displayed and allows cancelation of popup display.
        /// </summary>
        [Description("Occurs before token auto-complete popup is displayed and allows cancelation of popup display.")]
        public event CancelEventHandler BeforeAutoCompletePopupOpen;
        /// <summary>
        /// Occurs after auto-complete popup is open.
        /// </summary>
        [Description("Occurs after auto-complete popup is open.")]
        public event EventHandler AutoCompletePopupOpened;
        /// <summary>
        /// Raises AutoCompletePopupOpened event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnAutoCompletePopupOpened(EventArgs e)
        {
            EventHandler handler = AutoCompletePopupOpened;
            if (handler != null)
                handler(this, e);
        }
        /// <summary>
        /// Raises BeforeAutoCompletePopupOpen event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnBeforeAutoCompletePopupOpen(CancelEventArgs e)
        {
            CancelEventHandler handler = BeforeAutoCompletePopupOpen;
            if (handler != null)
                handler(this, e);
        }

        private void OpenAutoCompleteDropDown()
        {
            if (_PopupController == null)
            {
                _PopupController = new PopupHostController();
                _PopupController.Closed += PopupControllerClosed;
            }

            if (_AutoCompleteListBox == null)
            {
                _AutoCompleteListBox = CreateAutoCompleteListBox();
                //_AutoCompleteListBox.Resize += _AutoCompleteListBox_Resize;
                //this.Parent.Controls.Add(_AutoCompleteListBox);
                //_AutoCompleteListBox.Location = new Point(10,100);
            }
            
            if (!_MessageHandlerInstalled && _DropDownButtonVisible)
            {
                MessageHandler.RegisterMessageClient(this);
                _MessageHandlerInstalled = true;
            }

            LoadAutoCompleteBox(_AutoCompleteListBox);

            if (!_PopupController.PopupUserSize || !_PreservePopupSize)
            {
                _AutoCompleteListBox.Height = Dpi.Height(this.DropDownHeight);
                _AutoCompleteListBox.Width = (this.DropDownWidth > 0 ? Dpi.Width(this.DropDownWidth) : this.Width);
                //_AutoCompleteListBox.MinimumSize = _AutoCompleteListBox.Size;
            }

            CancelEventArgs cancelArgs = new CancelEventArgs();
            OnBeforeAutoCompletePopupOpen(cancelArgs);
            if (cancelArgs.Cancel)
                return;

            Point location = PointToScreen(new Point(0, Height));
            TokenEditorPopupEventArgs popupArgs = new TokenEditorPopupEventArgs(location, _AutoCompleteListBox.Size);
            OnBeforePopupOpen(popupArgs);
            location = popupArgs.PopupLocation;
            ePopupResizeEdge resize = _EnablePopupResize ? ePopupResizeEdge.BottomRight : ePopupResizeEdge.None;
            _PopupController.AutoClose = false;

            _PopupController.ParentControlBounds = new Rectangle(PointToScreen(Point.Empty), this.Size);
            _PopupController.Show(_AutoCompleteListBox, location.X, location.Y, 0, 0, resize);

            _EditBox.Focus();
            _EditBox.SelectionLength = 0; // Clear selection that happens upon focus
            if (_EditBox.TextLength > 0)
                _EditBox.SelectionStart = _EditBox.TextLength;
            _PopupController.AutoClose = true;
            _DroppedDown = true;
            _MultiDropDownOpenedAtTime = DateTime.Now;

            OnAutoCompletePopupOpened(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs before the auto-complete popup is displayed and allows you to adjust popup location.
        /// </summary>
        [Description("Occurs before the auto-complete popup is displayed and allows you to adjust popup location.")]
        public event TokenEditorPopupEventHandler BeforePopupOpen;
        /// <summary>
        /// Raises BeforePopupOpen event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnBeforePopupOpen(TokenEditorPopupEventArgs e)
        {
            TokenEditorPopupEventHandler handler = BeforePopupOpen;
            if (handler != null)
                handler(this, e);
        }

        private void LoadAutoCompleteBox(ListBoxAdv listBox)
        {
            listBox.BeginUpdate();
            listBox.Items.Clear();
            listBox.CheckBoxesVisible = _CheckBoxesVisible;
            listBox.ShowToolTips = _ShowToolTips;
            foreach (EditToken item in _Tokens)
            {
                ListBoxItem listItem = new ListBoxItem();
                listItem.Text = GetTokenText(item);
                listItem.Tooltip = item.Tooltip;
                if (_CheckBoxesVisible)
                    listItem.CheckState = (item.IsSelected ? CheckState.Checked : CheckState.Unchecked);
                else
                    listItem.Visible = !item.IsSelected;
                listBox.Items.Add(listItem);
            }
            listBox.EndUpdate();
        }
        private bool FilteredTokenExists(string text)
        {
            foreach (EditToken item in _Tokens)
            {
                if (item.IsSelected) continue;
                if (TokenMatch(text, item, _TokenFilterBehavior))
                    return true;
            }
            return false;
        }

        private eTokenFilterBehavior _TokenFilterBehavior = eTokenFilterBehavior.TextAndValue;
        /// <summary>
        /// Indicates how tokens are filtered based on the entered text
        /// </summary>
        [DefaultValue(eTokenFilterBehavior.TextAndValue), Category("Behavior"), Description("Indicates how tokens are filtered based on the entered text")]
        public eTokenFilterBehavior TokenFilterBehavior
        {
            get { return _TokenFilterBehavior; }
            set { _TokenFilterBehavior = value; }
        }
        private static bool TokenMatch(string filterText, EditToken token, eTokenFilterBehavior behavior)
        {
            filterText = filterText.ToLower();
            if(behavior == eTokenFilterBehavior.Text)
                return !string.IsNullOrEmpty(token.Text) && token.Text.ToLower().Contains(filterText);
            else if(behavior == eTokenFilterBehavior.Value)
                return !string.IsNullOrEmpty(token.Value) && token.Value.ToLower().Contains(filterText);
            return !string.IsNullOrEmpty(token.Text) && token.Text.ToLower().Contains(filterText) || !string.IsNullOrEmpty(token.Value) && token.Value.ToLower().Contains(filterText);
        }
        private static bool TokenExactMatch(string filterText, EditToken token, eTokenFilterBehavior behavior)
        {
            filterText = filterText.ToLower();
            if (behavior == eTokenFilterBehavior.Text)
                return !string.IsNullOrEmpty(token.Text) && token.Text.ToLower() == filterText;
            else if (behavior == eTokenFilterBehavior.Value)
                return !string.IsNullOrEmpty(token.Value) && token.Value.ToLower() == filterText;
            return !string.IsNullOrEmpty(token.Text) && token.Text.ToLower() == filterText || !string.IsNullOrEmpty(token.Value) && token.Value.ToLower() == filterText;
        }
        private void FilterAutoCompleteBox(ListBoxAdv listBox, string filterText)
        {
            bool oneVisible = false;
            listBox.BeginUpdate();
            if (string.IsNullOrEmpty(filterText))
            {
                if (!_CheckBoxesVisible)
                {
                    foreach (ListBoxItem item in listBox.Items)
                    {
                        item.IsSelected = false;
                        item.Visible = !item.IsSelected;
                        if (item.Visible)
                            oneVisible = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < listBox.Items.Count; i++)
                {
                    ListBoxItem item = (ListBoxItem)listBox.Items[i];
                    EditToken token = _Tokens[i];
                    if (!token.IsSelected && TokenMatch(filterText, token, _TokenFilterBehavior))
                    {
                        if (!_CheckBoxesVisible)
                            item.Visible = true;
                        if (!oneVisible)
                            item.IsSelected = true;
                        else
                            item.IsSelected = false;
                        oneVisible = true;
                    }
                    else if (!_CheckBoxesVisible)
                        item.Visible = false;
                    else
                        item.IsSelected = false;
                }
            }

            listBox.EndUpdate();

            if (!oneVisible && !_CheckBoxesVisible)
                CloseAutoCompleteDropDown();
        }

        private ListBoxAdv CreateAutoCompleteListBox()
        {
            ListBoxAdv listBox = new ListBoxAdv();
#if (!TRIAL)
            listBox.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
#endif
            //listBox.BackColor = Color.White;
            listBox.BackgroundStyle.Class = ElementStyleClassKeys.ListBoxAdvKey;
            listBox.ItemClick += AutoCompleteListBoxItemClick;
            listBox.ItemCheck += AutoCompleteListBoxItemCheck;
            listBox.AutoScroll = true;
            //listBox.BackgroundStyle.Reset();
            //listBox.BackgroundStyle.BackColor = SystemColors.Window;
            //listBox.BackgroundStyle.Border = eStyleBorderType.None;
            return listBox;

        }

        void AutoCompleteListBoxItemCheck(object sender, ListBoxAdvItemCheckEventArgs e)
        {
            int index = _AutoCompleteListBox.Items.IndexOf(e.Item);
            EditToken token = _Tokens[index];
            if (e.Item.CheckState == CheckState.Unchecked && token.IsSelected)
                this.SelectedTokens.Remove(token);
            else if (!token.IsSelected)
                this.SelectedTokens.Add(token);
        }
        private void PopupControllerClosed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            _DroppedDown = false;

            //m_SelectedIndexInternal = this.SelectedIndex;
            _MultiDropDownClosedAtTime = DateTime.Now;
            //OnDropDownClosed(e);
        }
        private void CloseAutoCompleteDropDown()
        {
            if (_PopupController != null && _DroppedDown)
            {
                _PopupController.Hide();
            }
        }

        private bool _PreservePopupSize = true;
        /// <summary>
        /// Indicates whether auto-complete popup size is preserved between popup displays if popup is resized by end-user.
        /// </summary>
        [DefaultValue(true), Category("Auto-Complete"), Description("Indicates whether auto-complete popup size is preserved between popup displays if popup is resized by end-user.")]
        public bool PreservePopupSize
        {
            get { return _PreservePopupSize; }
            set
            {
                _PreservePopupSize = value;
            }
        }

        private bool _EnablePopupResize = true;
        /// <summary>
        /// Indicates whether auto-complete popup can be resized by end user.
        /// </summary>
        [DefaultValue(true), Category("Auto-Complete"), Description("Indicates whether auto-complete popup can be resized by end user.")]
        public bool EnablePopupResize
        {
            get { return _EnablePopupResize; }
            set
            {
                _EnablePopupResize = value;
            }
        }

        private bool _PopupCloseButtonVisible = true;
        /// <summary>
        /// Indicates whether multi-column popup close button is visible.
        /// </summary>
        [DefaultValue(true), Category("Auto-Complete"), Description("Indicates whether multi-column popup close button is visible.")]
        public bool PopupCloseButtonVisible
        {
            get { return _PopupCloseButtonVisible; }
            set
            {
                _PopupCloseButtonVisible = value;
            }
        }

        private bool _DroppedDown = false;
        /// <summary>
        /// Gets or sets whether auto-complete popup window is open.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPopupOpen
        {
            get { return _DroppedDown; }
            set
            {
                if (value != _DroppedDown)
                {
                    if (value)
                        OpenAutoCompleteDropDown();
                    else
                        CloseAutoCompleteDropDown();
                }
            }
        }

        private List<string> _Separators = new List<string>();
        /// <summary>
        /// Gets the list of separators which are used to divide entered text into the tokens.
        /// </summary>
        [Category("Behavior"), Description("Gets the list of separators which are used to divide entered text into the tokens."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false), Localizable(true)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version= 2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public List<string> Separators
        {
            get { return _Separators; }
        }

        /// <summary>
        /// Occurs when an token is selected from the auto-complete list or when text entry by end user is parsed into token to validate it.
        /// </summary>
        [Description("Occurs when an token is selected from the auto-complete list or when text entry by end user is parsed into token to validate it.")]
        public event ValidateTokenEventHandler ValidateToken;
        /// <summary>
        /// Raises ValidateToken event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnValidateToken(ValidateTokenEventArgs e)
        {
            ValidateTokenEventHandler handler = ValidateToken;
            if (handler != null)
                handler(this, e);
        }

        private bool _AutoSizeHeight = true;
        /// <summary>
        /// Indicates whether control automatically increases its height as more tokens are selected. MaxHeightLines property controls the maximum number of lines control will grow to before showing scroll-bar.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether control automatically increases its height as more tokens are selected. MaxHeightLines property controls the maximum number of lines control will grow to before showing scroll-bar.")]
        public bool AutoSizeHeight
        {
            get { return _AutoSizeHeight; }
            set
            {
                if (_AutoSizeHeight != value)
                {
                    _AutoSizeHeight = value;
                    if (_AutoSizeHeight)
                        LayoutTokens();
                }
            }
        }

        private int _MaxHeightLines = 5;
        /// <summary>
        /// Indicates maximum number of lines control will grow to when AutoSizeHeight=true. Set to 0 to indicates unlimited growth.
        /// Default value is 5.
        /// </summary>
        [DefaultValue(5), Category("Behavior"), Description("Indicates maximum number of lines control will grow to when AutoSizeHeight=true. Set to 0 to indicates unlimited growth.")]
        public int MaxHeightLines
        {
            get { return _MaxHeightLines; }
            set { _MaxHeightLines = value; }
        }

        /// <summary>
        /// Gets reference to internal text-box control that is used to input the token text.
        /// </summary>
        [Browsable(false)]
        public TextBox EditTextBox
        {
            get
            {
                return _EditBox;
            }
        }

        private bool _ValidateTokenTextOnLostFocus = true;
        /// <summary>
        /// Indicates whether any text entered into the token editor is validated and converted to token when control loses focus.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether any text entered into the token editor is validated and converted to token when control loses focus.")]
        public bool ValidateTokenTextOnLostFocus
        {
            get { return _ValidateTokenTextOnLostFocus; }
            set { _ValidateTokenTextOnLostFocus = value; }
        }

        private string _WatermarkText = "";
        private Font _WatermarkFont = null;
        private Color _WatermarkColor = SystemColors.GrayText;

        private bool _WatermarkEnabled = true;
        /// <summary>
        /// Gets or sets whether watermark text is displayed when control is empty. Default value is true.
        /// </summary>
        [DefaultValue(true), Description("Indicates whether watermark text is displayed when control is empty.")]
        public virtual bool WatermarkEnabled
        {
            get { return _WatermarkEnabled; }
            set { _WatermarkEnabled = value; this.Invalidate(); UpdateEditBoxWatermark(); }
        }

        private Image _WatermarkImage = null;
        /// <summary>
        /// Gets or sets the watermark image displayed inside of the control when Text is not set and control does not have input focus.
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates watermark image displayed inside of the control when Text is not set and control does not have input focus.")]
        public Image WatermarkImage
        {
            get { return _WatermarkImage; }
            set { _WatermarkImage = value; this.Invalidate(); UpdateEditBoxWatermark(); }
        }
        private ContentAlignment _WatermarkImageAlignment = ContentAlignment.MiddleLeft;
        /// <summary>
        /// Gets or sets the watermark image alignment.
        /// </summary>
        [DefaultValue(ContentAlignment.MiddleLeft), Category("Appearance"), Description("Indicates watermark image alignment.")]
        public ContentAlignment WatermarkImageAlignment
        {
            get { return _WatermarkImageAlignment; }
            set { _WatermarkImageAlignment = value; this.Invalidate(); UpdateEditBoxWatermark(); }
        }

        /// <summary>
        /// Gets or sets the watermark (tip) text displayed inside of the control when Text is not set and control does not have input focus. This property supports text-markup.
        /// </summary>
        [Browsable(true), DefaultValue(""), Localizable(true), Category("Appearance"), Description("Indicates watermark text displayed inside of the control when Text is not set and control does not have input focus."), Editor("DevComponents.DotNetBar.Design.TextMarkupUIEditor, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf", typeof(System.Drawing.Design.UITypeEditor))]
        public string WatermarkText
        {
            get { return _WatermarkText; }
            set
            {
                if (value == null) value = "";
                _WatermarkText = value;
                MarkupTextChanged();
                UpdateEditBoxWatermark();
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the watermark font.
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("Indicates watermark font."), DefaultValue(null)]
        public Font WatermarkFont
        {
            get { return _WatermarkFont; }
            set { _WatermarkFont = value; this.Invalidate(); UpdateEditBoxWatermark(); }
        }

        /// <summary>
        /// Gets or sets the watermark text color.
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("Indicates watermark text color.")]
        public Color WatermarkColor
        {
            get { return _WatermarkColor; }
            set { _WatermarkColor = value; this.Invalidate(); UpdateEditBoxWatermark(); }
        }
        /// <summary>
        /// Indicates whether property should be serialized by Windows Forms designer.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeWatermarkColor()
        {
            return _WatermarkColor != SystemColors.GrayText;
        }
        /// <summary>
        /// Resets the property to default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetWatermarkColor()
        {
            this.WatermarkColor = SystemColors.GrayText;
        }

        protected virtual bool IsWatermarkRendered
        {
            get
            {
                return (!this.Focused || _WatermarkBehavior == eWatermarkBehavior.HideNonEmpty) && this.SelectedTokens.Count == 0;
            }
        }

        private eWatermarkBehavior _WatermarkBehavior = eWatermarkBehavior.HideOnFocus;
        /// <summary>
        /// Gets or sets the watermark hiding behaviour. Default value indicates that watermark is hidden when control receives input focus.
        /// </summary>
        [DefaultValue(eWatermarkBehavior.HideOnFocus), Category("Behavior"), Description("Indicates watermark hiding behaviour.")]
        public eWatermarkBehavior WatermarkBehavior
        {
            get { return _WatermarkBehavior; }
            set { _WatermarkBehavior = value; this.Invalidate();
                UpdateEditBoxWatermark();
            }
        }
        private void UpdateEditBoxWatermark()
        {
            _EditBox.WatermarkText = _WatermarkText;
            _EditBox.WatermarkColor = _WatermarkColor;
            _EditBox.WatermarkFont = _WatermarkFont;
            _EditBox.WatermarkImage = _WatermarkImage;
            _EditBox.WatermarkImageAlignment = _WatermarkImageAlignment;
            if (this.SelectedTokens.Count > 0 || !_WatermarkEnabled)
                _EditBox.WatermarkEnabled = false;
            else if (_WatermarkBehavior == eWatermarkBehavior.HideNonEmpty)
            {
                _EditBox.WatermarkEnabled = true;
                _EditBox.WatermarkBehavior = eWatermarkBehavior.HideNonEmpty;
            }
            else
            {
                _EditBox.WatermarkBehavior = eWatermarkBehavior.HideOnFocus;
            }

        }

        private TextMarkup.BodyElement _TextMarkup = null;
        private void MarkupTextChanged()
        {
            _TextMarkup = null;

            if (!TextMarkup.MarkupParser.IsMarkup(ref _WatermarkText))
                return;

            _TextMarkup = TextMarkup.MarkupParser.Parse(_WatermarkText);
            ResizeMarkup();
        }

        private MarkupDrawContext GetMarkupDrawContext(Graphics g)
        {
            return new MarkupDrawContext(g, (_WatermarkFont == null ? this.Font : _WatermarkFont), _WatermarkColor, this.RightToLeft == RightToLeft.Yes);
        }
        private void ResizeMarkup()
        {
            if (_TextMarkup != null)
            {
                using (Graphics g = this.CreateGraphics())
                {
                    MarkupDrawContext dc = GetMarkupDrawContext(g);
                    _TextMarkup.Measure(GetWatermarkBounds().Size, dc);
                    Size sz = _TextMarkup.Bounds.Size;
                    _TextMarkup.Arrange(new Rectangle(GetWatermarkBounds().Location, sz), dc);
                }
            }
        }

        private Rectangle GetWatermarkBounds()
        {
            Rectangle r = this.ClientRectangle;
            r.Inflate(-1, 0);
            r.X += 2;
            r.Width -= 2;
            return r;
        }

        private void DrawWatermark(Graphics g)
        {

            Rectangle bounds = GetWatermarkBounds();
            if (_WatermarkImage != null)
            {
                Rectangle imageBounds = new Rectangle(Point.Empty, _WatermarkImage.Size);
                if (_WatermarkImageAlignment == ContentAlignment.BottomCenter)
                    imageBounds.Location = new Point(bounds.X + (bounds.Width - imageBounds.Width) / 2, bounds.Bottom - imageBounds.Height);
                else if (_WatermarkImageAlignment == ContentAlignment.BottomLeft)
                    imageBounds.Location = new Point(bounds.X, bounds.Bottom - imageBounds.Height);
                else if (_WatermarkImageAlignment == ContentAlignment.BottomRight)
                    imageBounds.Location = new Point(bounds.Right - imageBounds.Width, bounds.Bottom - imageBounds.Height);
                else if (_WatermarkImageAlignment == ContentAlignment.MiddleCenter)
                    imageBounds.Location = new Point(bounds.X + (bounds.Width - imageBounds.Width) / 2, bounds.Y + (bounds.Height - imageBounds.Height) / 2);
                else if (_WatermarkImageAlignment == ContentAlignment.MiddleLeft)
                    imageBounds.Location = new Point(bounds.X, bounds.Y + (bounds.Height - imageBounds.Height) / 2);
                else if (_WatermarkImageAlignment == ContentAlignment.MiddleRight)
                    imageBounds.Location = new Point(bounds.Right - imageBounds.Width, bounds.Y + (bounds.Height - imageBounds.Height) / 2);
                else if (_WatermarkImageAlignment == ContentAlignment.TopCenter)
                    imageBounds.Location = new Point(bounds.X + (bounds.Width - imageBounds.Width) / 2, bounds.Y);
                else if (_WatermarkImageAlignment == ContentAlignment.TopLeft)
                    imageBounds.Location = new Point(bounds.X, bounds.Y);
                else if (_WatermarkImageAlignment == ContentAlignment.TopRight)
                    imageBounds.Location = new Point(bounds.Right - imageBounds.Width, bounds.Y);
                g.DrawImage(_WatermarkImage, imageBounds);

                if (_WatermarkImageAlignment == ContentAlignment.BottomLeft || _WatermarkImageAlignment == ContentAlignment.MiddleLeft || _WatermarkImageAlignment == ContentAlignment.TopLeft)
                {
                    bounds.X = imageBounds.Right;
                    bounds.Width = Math.Max(0, bounds.Width - imageBounds.Width);
                }
                else if (_WatermarkImageAlignment == ContentAlignment.BottomRight || _WatermarkImageAlignment == ContentAlignment.MiddleRight || _WatermarkImageAlignment == ContentAlignment.TopRight)
                {
                    bounds.Width = Math.Max(0, bounds.Width - imageBounds.Width);
                }
            }

            if (bounds.Width <= 0) return;

            if (_TextMarkup != null)
            {
                MarkupDrawContext dc = GetMarkupDrawContext(g);
                if (_TextMarkup.Bounds.Height < bounds.Height)
                    _TextMarkup.Bounds = new Rectangle(bounds.X, bounds.Y + (bounds.Height - _TextMarkup.Bounds.Height) / 2, _TextMarkup.Bounds.Width, _TextMarkup.Bounds.Height);
                _TextMarkup.Render(dc);
            }
            else
            {
                eTextFormat tf = eTextFormat.Left | eTextFormat.VerticalCenter;

                if (this.RightToLeft == RightToLeft.Yes) tf |= eTextFormat.RightToLeft;

                tf |= eTextFormat.EndEllipsis;
                tf |= eTextFormat.WordBreak;
                TextDrawing.DrawString(g, _WatermarkText, (_WatermarkFont == null ? this.Font : _WatermarkFont),
                    _WatermarkColor, bounds, tf);
            }

        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (!_UpdatingText)
            {
                this.SelectedTokens.Clear();
                if (!string.IsNullOrEmpty(this.Text))
                {
                    if (!string.IsNullOrEmpty(_TextSeparator))
                    {
                        string[] tokens = this.Text.Split(new string[] { _TextSeparator }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string item in tokens)
                        {
                            bool isNew = false;
                            SelectToken(FindOrCreateToken(this.Text, out isNew));
                        }
                    }
                    else
                    {
                        bool isNew = false;
                        SelectToken(FindOrCreateToken(this.Text, out isNew));
                    }
                }
            }

            base.OnTextChanged(e);
        }

        private bool _UpdatingText = false;
        private void UpdateText()
        {
            _UpdatingText = true;
            string text = "";
            for (int i = 0; i < _SelectedTokens.Count; i++)
            {
                text += GetTokenText(_SelectedTokens[i]);
                if (i < _SelectedTokens.Count - 1)
                    text += _TextSeparator;
            }

            this.Text = text;

            _UpdatingText = false;
        }

        private string _TextSeparator = ",";
        /// <summary>
        /// Indicates the character separator that is used to separate tokens when controls Text property is updated or parsed.
        /// </summary>
        [DefaultValue(","), Category("Behavior"), Description("Indicates the character separator that is used to separate tokens when controls Text property is updated or parsed.")]
        public string TextSeparator
        {
            get { return _TextSeparator; }
            set
            {
                if (value != _TextSeparator)
                {
                    string oldValue = _TextSeparator;
                    _TextSeparator = value;
                    OnTextSeparatorChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when TextSeparator property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnTextSeparatorChanged(string oldValue, string newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("TextSeparator"));
            UpdateText();
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        private bool _DropDownButtonVisible = false;
        /// <summary>
        /// Indicates whether drop-down button which shows available token popup is displayed
        /// </summary>
        [DefaultValue(false), Category("Appearance"), Description("Indicates whether drop-down button which shows available token popup is displayed")]
        public bool DropDownButtonVisible
        {
            get { return _DropDownButtonVisible; }
            set
            {
                if (value != _DropDownButtonVisible)
                {
                    bool oldValue = _DropDownButtonVisible;
                    _DropDownButtonVisible = value;
                    OnDropDownButtonVisibleChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when DropDownButtonVisible property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnDropDownButtonVisibleChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("DropDownButtonVisible"));
            UpdateButtons();
            LayoutTokens();
            Invalidate();
        }

        private bool _CheckBoxesVisible = false;
        /// <summary>
        /// Indicates whether check-boxes are displayed on popup token selection list and used for token selection.
        /// </summary>
        [DefaultValue(false), Category("Behavior"), Description("Indicates whether check-boxes are displayed on popup token selection list and used for token selection.")]
        public bool CheckBoxesVisible
        {
            get { return _CheckBoxesVisible; }
            set
            {
                if (value != _CheckBoxesVisible)
                {
                    bool oldValue = _CheckBoxesVisible;
                    _CheckBoxesVisible = value;
                    OnCheckBoxesVisibleChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when CheckBoxesVisible property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnCheckBoxesVisibleChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("CheckBoxesVisible"));

        }

        private VisualGroup _ButtonGroup = null;
        private VisualDropDownButton _DropDownButton = null;
        private VisualUpDownButton _ScrollButton = null;
        private int GetButtonHeight()
        {
            return GetSingleLineTextBoxHeight() - 4;
        }
        private void UpdateButtons()
        {
            if (!_DropDownButtonVisible && !_ScrollButton.Visible)
                _ButtonGroup.Visible = false;
            else
            {
                _ButtonGroup.Visible = true;
                _DropDownButton.Height = GetButtonHeight();
            }
            _DropDownButton.Visible = _DropDownButtonVisible;
            _ButtonGroup.InvalidateArrange();
        }

        protected virtual void CreateButtons()
        {
            _DropDownButton = new VisualDropDownButton();
            _DropDownButton.MouseClick += DropDownButtonClick;
            _DropDownButton.Visible = false;
            _ButtonGroup.Items.Add(_DropDownButton);

            _ScrollButton = new VisualUpDownButton();
            _ScrollButton.UpClick += ScrollButtonUpClick;
            _ScrollButton.DownClick += ScrollButtonDownClick;
            _ScrollButton.Visible = false;
            _ButtonGroup.Items.Add(_ScrollButton);
        }

        void ScrollButtonDownClick(object sender, EventArgs e)
        {
            if (Math.Abs(_AutoScrollPosition.Y) < (_VScrollBar.Maximum - _VScrollBar.LargeChange - 1))
                AutoScrollPosition = new Point(_AutoScrollPosition.X, _AutoScrollPosition.Y - _VScrollBar.SmallChange);
        }

        void ScrollButtonUpClick(object sender, EventArgs e)
        {
            if (_AutoScrollPosition.Y < 0)
                AutoScrollPosition = new Point(_AutoScrollPosition.X, Math.Min(0, _AutoScrollPosition.Y + _VScrollBar.SmallChange));
        }

        void DropDownButtonClick(object sender, MouseEventArgs e)
        {
            IsPopupOpen = !IsPopupOpen;
        }
        private void ButtonGroupRenderInvalid(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void ButtonGroupArrangeInvalid(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void ButtonGroupResetMouseHover(object sender, EventArgs e)
        {
            DevComponents.AdvTree.Interop.WinApi.ResetHover(this);
        }

        private void PaintButtons(Graphics g)
        {
            PaintInfo p = CreatePaintInfo(g);
            if (!_ButtonGroup.IsLayoutValid)
            {
                _ButtonGroup.PerformLayout(p);
            }
            bool disposeStyle = false;
            ElementStyle style = ElementStyleDisplay.GetElementStyle(_BackgroundStyle, out disposeStyle);
            _ButtonGroup.RenderBounds = new Rectangle(this.Width - (ElementStyleLayout.RightWhiteSpace(style, eSpacePart.Border) + 1) - _ButtonGroup.Size.Width, ElementStyleLayout.TopWhiteSpace(style, eSpacePart.Border) + 1,
                _ButtonGroup.Size.Width, _ButtonGroup.Size.Height);
            _ButtonGroup.ProcessPaint(p);
            if (disposeStyle) style.Dispose();
        }
        private PaintInfo CreatePaintInfo(Graphics g)
        {
            PaintInfo p = new PaintInfo();
            p.Graphics = g;
            p.DefaultFont = this.Font;
            p.ForeColor = this.ForeColor;
            p.RenderOffset = new System.Drawing.Point();
            Size s = this.Size;
            bool disposeStyle = false;
            ElementStyle style = ElementStyleDisplay.GetElementStyle(_BackgroundStyle, out disposeStyle);
            s.Height -= (ElementStyleLayout.TopWhiteSpace(style, eSpacePart.Border) + ElementStyleLayout.BottomWhiteSpace(style, eSpacePart.Border)) + 2;
            s.Width -= (ElementStyleLayout.LeftWhiteSpace(style, eSpacePart.Border) + ElementStyleLayout.RightWhiteSpace(style, eSpacePart.Border)) + 2;
            p.AvailableSize = s;
            p.ParentEnabled = this.Enabled;
            p.MouseOver = _IsMouseOver || this.Focused;
            if (disposeStyle) style.Dispose();
            return p;
        }
        #endregion

        #region Tooltip Support
        private DevComponents.DotNetBar.ToolTip _ToolTipWnd = null;
        /// <summary>
        /// Shows tooltip for this item.
        /// </summary>
        public virtual void ShowToolTip(EditToken token)
        {
            if (this.DesignMode || token==null || !token.IsSelected)
                return;

            if (this.Visible && this.ShowToolTips )
            {
                string tooltip = token.Tooltip;
                if (!string.IsNullOrEmpty(tooltip))
                {
                    if (_ToolTipWnd == null)
                        _ToolTipWnd = new DevComponents.DotNetBar.ToolTip();
                    _ToolTipWnd.Style = StyleManager.GetEffectiveStyle();
                    _ToolTipWnd.Text = tooltip;
                    _ToolTipWnd.ReferenceRectangle = new Rectangle(PointToScreen(token.Bounds.Location), token.Bounds.Size);

                    OnToolTipVisibleChanged(new EventArgs());
                    _ToolTipWnd.ShowToolTip();
                }
            }
        }
        /// <summary>
        /// Destroys tooltip window.
        /// </summary>
        internal protected void HideToolTip()
        {
            if (_ToolTipWnd != null)
            {
                System.Drawing.Rectangle tipRect = _ToolTipWnd.Bounds;
                tipRect.Width += 5;
                tipRect.Height += 6;

                OnToolTipVisibleChanged(new EventArgs());
                try
                {
                    if (_ToolTipWnd != null)
                    {
                        _ToolTipWnd.Hide();
                        _ToolTipWnd.Dispose();
                        _ToolTipWnd = null;
                    }
                }
                catch { }
                this.Invalidate();
            }
        }
        /// <summary>
        /// Occurs when item's tooltip visibility has changed.
        /// </summary>
        [System.ComponentModel.Description("Occurs when item's tooltip visibility has changed.")]
        public event EventHandler ToolTipVisibleChanged;
        private void OnToolTipVisibleChanged(EventArgs eventArgs)
        {
            EventHandler h = ToolTipVisibleChanged;
            if (h != null)
                ToolTipVisibleChanged(this, eventArgs);
        }

        private bool _ShowToolTips = true;
        /// <summary>
        /// Gets or sets whether tooltips are shown when mouse is over the selected token when Tooltip property is set.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether tooltips are shown when mouse is over the cell when Tooltip property is set.")]
        public bool ShowToolTips
        {
            get { return _ShowToolTips; }
            set
            {
                _ShowToolTips = value;
            }
        }
        
        #endregion

        #region IMessageHandlerClient
        private bool _MessageHandlerInstalled = false;
        bool IMessageHandlerClient.OnSysKeyDown(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
        {
            return false;
        }

        bool IMessageHandlerClient.OnSysKeyUp(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
        {
            return false;
        }

        bool IMessageHandlerClient.OnKeyDown(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
        {
            return false;
        }

        bool IMessageHandlerClient.OnMouseDown(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
        {
            if (IsPopupOpen && _PopupController != null)
            {
                if (!_PopupController.Bounds.Contains(Control.MousePosition))
                {
                    IsPopupOpen = false;
                }
            }
            return false;
        }

        bool IMessageHandlerClient.OnMouseMove(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
        {
            return false;
        }

        bool IMessageHandlerClient.OnMouseWheel(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
        {
            return false;
        }

        bool IMessageHandlerClient.IsModal
        {
            get { return false; }
        }
        #endregion
    }

    /// <summary>
    /// Delegate for the ValidateTokenEvent event.
    /// </summary>
    public delegate void ValidateTokenEventHandler(object sender, ValidateTokenEventArgs ea);
    /// <summary>
    /// Arguments for the ValidateTokenEvent event.
    /// </summary>
    public class ValidateTokenEventArgs : EventArgs
    {
        /// <summary>
        /// Indicates whether validated token is valid. Default value is true. When you set this property to false the token being validated will be discared.
        /// </summary>
        public bool IsValid = true;
        /// <summary>
        /// Indicates the Token that will be accepted by the control if IsValid=true.
        /// </summary>
        public EditToken Token = null;
        /// <summary>
        /// Indicates whether token is newly created. When false it means that token was taken from Tokens collection.
        /// </summary>
        public readonly bool IsNewToken;
        /// <summary>
        /// Initializes a new instance of the ValidateTokenEventArgs class.
        /// </summary>
        /// <param name="token"></param>
        public ValidateTokenEventArgs(EditToken token, bool isNewToken)
        {
            Token = token;
            IsNewToken = isNewToken;
        }
    }
    /// <summary>
    /// Delegate for RemovingToken event.
    /// </summary>
    public delegate void RemovingTokenEventHandler(object sender, RemovingTokenEventArgs ea);
    /// <summary>
    /// Defines event arguments for RemovingToken event.
    /// </summary>
    public class RemovingTokenEventArgs : EventArgs
    {
        /// <summary>
        /// Indicates the Token that will be removed.
        /// </summary>
        public readonly EditToken Token;
        /// <summary>
        /// Set to true to cancel removal of the token.
        /// </summary>
        public bool Cancel = false;
        /// <summary>
        /// Indicates the source of the event.
        /// </summary>
        public readonly eEventSource EventSource;
        /// <summary>
        /// Initializes a new instance of the RemovingTokenEventArgs class.
        /// </summary>
        /// <param name="token"></param>
        public RemovingTokenEventArgs(EditToken token, eEventSource eventSource)
        {
            Token = token;
            EventSource = eventSource;
        }
    }

    /// <summary>
    /// Specifies the filter behavior on token editor popup list.
    /// </summary>
    public enum eTokenFilterBehavior
    {
        /// <summary>
        /// Token text is searched for the match.
        /// </summary>
        Text,
        /// <summary>
        /// Token value is searched for the match.
        /// </summary>
        Value,
        /// <summary>
        /// Both token text and value are searched for the match.
        /// </summary>
        TextAndValue
    }

    /// <summary>
    /// Delegate for TokenEditor.BeforePopupOpen event.
    /// </summary>
    public delegate void TokenEditorPopupEventHandler(object sender, TokenEditorPopupEventArgs ea);
    /// <summary>
    /// Defines event arguments for BeforePopupOpen event.
    /// </summary>
    public class TokenEditorPopupEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the screen location of the popup in relation to the TokenEditor control.
        /// </summary>
        public Point PopupLocation = Point.Empty;
        /// <summary>
        /// Gets the suggested popup size.
        /// </summary>
        public readonly Size PopupSize;
        /// <summary>
        /// Initializes a new instance of the TokenEditorPopupEventArgs class.
        /// </summary>
        /// <param name="token"></param>
        public TokenEditorPopupEventArgs(Point popupLocation, Size popupSize)
        {
            PopupLocation = popupLocation;
            PopupSize = popupSize;
        }
    }
}
