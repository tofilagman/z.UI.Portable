using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Represents the ListBoxAdv item for internal use. Not for public usage.
    /// </summary>
    public class ListBoxItem : BaseItem
    {
        #region Constructor

        #endregion

        #region Implementation
        /// <summary>
        /// Returns copy of the item.
        /// </summary>
        public override BaseItem Copy()
        {
            ListBoxItem copy = new ListBoxItem();
            this.CopyToItem(copy);
            return copy;
        }

        /// <summary>
        /// Copies the ListBoxItem specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New ListBoxItem instance.</param>
        internal void InternalCopyToItem(ListBoxItem copy)
        {
            CopyToItem(copy);
        }

        /// <summary>
        /// Copies the ListBoxItem specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New item instance.</param>
        protected override void CopyToItem(BaseItem copy)
        {
            base.CopyToItem(copy);

            ListBoxItem item = copy as ListBoxItem;
            item.IsSelected = _IsSelected;
            item.Symbol = _Symbol;
            item.SymbolSet = _SymbolSet;
            item.SymbolColor = _SymbolColor;
            item.SymbolSize = _SymbolSize;
        }

        public override void Paint(ItemPaintArgs p)
        {
            Rendering.BaseRenderer renderer = p.Renderer;
            if (renderer != null)
            {
                ListBoxItemRendererEventArgs e = new ListBoxItemRendererEventArgs(this, p.Graphics);
                e.ItemPaintArgs = p;
                renderer.DrawListBoxItem(e);
            }
            else
            {
                Rendering.ListBoxItemPainter painter = PainterFactory.CreateListBoxItemPainter(this);
                if (painter != null)
                {
                    ListBoxItemRendererEventArgs e = new ListBoxItemRendererEventArgs(this, p.Graphics);
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
            else if (this.Focused)
            {
                Rectangle r = m_Rect;
                r.Inflate(-1, -1);
                Color c = SystemColors.Control;
                if (renderer is Rendering.Office2007Renderer) c = ((Rendering.Office2007Renderer)renderer).ColorTable.ListBoxItem.Default.TextColor;
                using (Pen pen = new Pen(c, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    System.Drawing.Drawing2D.SmoothingMode sm = p.Graphics.SmoothingMode;
                    p.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    p.Graphics.DrawRectangle(pen, r);
                    p.Graphics.SmoothingMode = sm;
                }
            }

            this.DrawInsertMarker(p.Graphics);

        }

        private CheckState _CheckState = CheckState.Unchecked;
        private bool _HotTracking;
        private Size _CheckBoxSize = new Size(13, 13);
        private const int DefaultPadding = 2;
        private Padding _Padding = new Padding(DefaultPadding);
        internal const int ImageTextSpacing = 4;
        internal const int CheckBoxTextSpacing = 6;
        public override void RecalcSize()
        {
            ListBoxAdv cont = this.ContainerControl as ListBoxAdv;
            if (cont == null || cont.Disposing || cont.IsDisposed) return;

            bool checkBox = cont.CheckBoxesVisible;
            int itemHeight = Dpi.Height(cont.ItemHeight);
            Size size = Size.Empty;

            Graphics g = BarFunctions.CreateGraphics(cont);
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
                    size = Dpi.ImageSize(_Image.Size);
                }
                if (!string.IsNullOrEmpty(this.Text))
                {
                    Size textSize = ButtonItemLayout.MeasureItemText(this, g, 0, cont.Font, eTextFormat.Default, cont.RightToLeft == RightToLeft.Yes);
                    size.Width += textSize.Width;
                    size.Height = Math.Max(size.Height, textSize.Height);
                    if (_Image != null || !string.IsNullOrEmpty(_Symbol))
                        size.Width += Dpi.Width(ImageTextSpacing);
                }
                else if (string.IsNullOrEmpty(_Symbol) && _Image == null)
                    size = new System.Drawing.Size(Dpi.Width16, Dpi.Height16);

                size.Width += _Padding.Horizontal;
                size.Height += _Padding.Vertical;

                base.RecalcSize();
            }
            finally
            {
                g.Dispose();
            }

            if (checkBox)
            {
                size.Width += Dpi.Width(_CheckBoxSize.Width + CheckBoxTextSpacing);
                size.Height = Math.Max(Dpi.Height(_CheckBoxSize.Height), size.Height);
            }

            if (itemHeight > 0) size.Height = itemHeight;

            _CheckBoxBounds = Rectangle.Empty;
            m_Rect.Size = size;

            base.RecalcSize();
        }

        private Rectangle _CheckBoxBounds = Rectangle.Empty;
        internal Rectangle CheckBoxBounds
        {
            get
            {
                return _CheckBoxBounds;
            }
            set
            {
                _CheckBoxBounds = value;
            }
        }

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
                CheckBoxMouseState = eMouseState.None;
                if (this.GetEnabled() && _HotTracking)
                    this.Refresh();
            }
        }

        public override void InternalMouseMove(MouseEventArgs objArg)
        {
            if (_CheckBoxBounds.Contains(objArg.Location))
                CheckBoxMouseState = eMouseState.Hot;
            else
                CheckBoxMouseState = eMouseState.None;
            base.InternalMouseMove(objArg);
        }

        public override void InternalMouseDown(MouseEventArgs objArg)
        {
            if (objArg.Button == MouseButtons.Left && !_CheckBoxBounds.IsEmpty && _CheckBoxBounds.Contains(objArg.Location))
                CheckBoxMouseState = eMouseState.Down;
            base.InternalMouseDown(objArg);
        }

        public override void InternalMouseUp(MouseEventArgs objArg)
        {
            if (objArg.Button == MouseButtons.Left && !_CheckBoxBounds.IsEmpty && _CheckBoxBounds.Contains(objArg.Location))
            {
                this.CheckState = this.CheckState == System.Windows.Forms.CheckState.Checked ? System.Windows.Forms.CheckState.Unchecked : System.Windows.Forms.CheckState.Checked;
                CheckBoxMouseState = eMouseState.Hot;
            }
            else if (objArg.Button == MouseButtons.Left)
            {
                if (this.Bounds.Contains(objArg.Location))
                {
                    ListBoxAdv listBox = this.ContainerControl as ListBoxAdv;
                    if (listBox != null)
                    {
                        if (listBox.SelectionMode != eSelectionMode.None)
                        {
                            if (listBox.SelectionMode == eSelectionMode.MultiSimple)
                                this.SetIsSelected(!this.IsSelected, eEventSource.Mouse);
                            else if (listBox.SelectionMode == eSelectionMode.MultiExtended)
                            {
                                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                                {
                                    this.SetIsSelected(!this.IsSelected, eEventSource.Mouse);
                                }
                                else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                                {
                                    BaseItem itemContainer = listBox.GetBaseItemContainer();
                                    int index = itemContainer.SubItems.IndexOf(this);
                                    int hiSel = -1, lowSel = -1;
                                    for (int i = 0; i < listBox.SelectedItems.Count; i++)
                                    {
                                        int itemIndex = itemContainer.SubItems.IndexOf(listBox.SelectedItems[i]);
                                        if (itemIndex > hiSel || hiSel < 0) hiSel = itemIndex;
                                        if (itemIndex < lowSel || lowSel < 0) lowSel = itemIndex;
                                    }
                                    if (hiSel < 0 || lowSel < 0)
                                        this.SetIsSelected(true, eEventSource.Mouse);
                                    else
                                    {
                                        if (index < lowSel)
                                        {
                                            for (int i = lowSel; i >= index; i--)
                                            {
                                                ListBoxItem listItem = itemContainer.SubItems[i] as ListBoxItem;
                                                if (listItem != null) listItem.SetIsSelected(true, eEventSource.Mouse);
                                            }
                                        }
                                        else if (index > hiSel)
                                        {
                                            for (int i = hiSel; i <= index; i++)
                                            {
                                                ListBoxItem listItem = itemContainer.SubItems[i] as ListBoxItem;
                                                if (listItem != null) listItem.SetIsSelected(true, eEventSource.Mouse);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    listBox.ClearSelectedItems(eEventSource.Mouse);
                                    this.SetIsSelected(true, eEventSource.Mouse);
                                }
                            }
                            else if(!this.IsSelected)
                                this.SetIsSelected(true, eEventSource.Mouse);
                        }
                    }
                    else
                        this.SetIsSelected(true, eEventSource.Mouse);
                }
            }
            base.InternalMouseUp(objArg);
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

        private bool _MouseOver = false;
        /// <summary>
        /// Gets whether mouse is over the item.
        /// </summary>
        [Browsable(false)]
        public bool IsMouseOver
        {
            get { return _MouseOver; }
            internal set { _MouseOver = value; }
        }

        internal Size CheckBoxSize
        {
            get
            {
                return _CheckBoxSize;
            }
        }

        private eMouseState _CheckBoxMouseState = eMouseState.None;
        /// <summary>
        /// Gets the mouse state of the check box part of item if visible.
        /// </summary>
        [Browsable(false)]
        public eMouseState CheckBoxMouseState
        {
            get { return _CheckBoxMouseState; }
            internal set
            {
                if (_CheckBoxMouseState != value)
                {
                    _CheckBoxMouseState = value;
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// Indicates check-box state if visible.
        /// </summary>
        [DefaultValue(CheckState.Unchecked), Category("Appearance"), Description("Indicates check-box state if visible.")]
        public CheckState CheckState
        {
            get { return _CheckState; }
            set
            {
                if (value != _CheckState)
                {
                    CheckState oldValue = _CheckState;
                    _CheckState = value;
                    OnCheckStateChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when CheckState property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnCheckStateChanged(CheckState oldValue, CheckState newValue)
        {
            ListBoxAdv listBox = this.ContainerControl as ListBoxAdv;
            if (listBox != null)
            {
                ListBoxAdvItemCheckEventArgs e = new ListBoxAdvItemCheckEventArgs(this, newValue);
                listBox.ListItemCheckStateChanged(e);
                if (e.Cancel)
                    _CheckState = oldValue;
            }
            //OnPropertyChanged(new PropertyChangedEventArgs("CheckState"));
            this.Refresh();
        }

        // Fields...
        private Image _Image;
        private bool _IsSelected = false;
        /// <summary>
        /// Gets or sets whether item is selected.
        /// </summary>
        [Browsable(false), DefaultValue(false)]
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (value != _IsSelected)
                {
                    SetIsSelected(value, eEventSource.Code);
                }
            }
        }
        /// <summary>
        /// Called when IsSelected property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("IsSelected"));
            this.Refresh();
        }
        /// <summary>
        /// Sets selected state of the item including the source of the action that caused the change.
        /// </summary>
        /// <param name="isSelected"></param>
        /// <param name="source"></param>
        public void SetIsSelected(bool isSelected, eEventSource source)
        {
            bool oldValue = _IsSelected;
            ListBoxAdv listBox = this.ContainerControl as ListBoxAdv;
            _IsSelected = isSelected;
            if (listBox != null)
            {
                listBox.OnListBoxItemSelectedChanged(this, source);
            }
            OnIsSelectedChanged(oldValue, isSelected);
        }
        /// <summary>
        /// Indicates whether item changes its background colors when mouse is over the item.
        /// </summary>
        [DefaultValue(false), Category("Behavior"), Description("Indicates whether item changes its background colors when mouse is over the item")]
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

        /// <summary>
        /// Specifies image displayed on the item.
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Specifies image displayed on the item.")]
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
        [DefaultValue(13f), Category("Appearance"), Description("Indicates the size of the symbol in points.")]
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
