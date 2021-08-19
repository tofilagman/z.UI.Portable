using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.UI.ContentManager;
using DevComponents.DotNetBar.Primitives;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Defines the internal container item for the ribbon strip control.
    /// </summary>
    [System.ComponentModel.ToolboxItem(false), System.ComponentModel.DesignTimeVisible(false)]
    internal class TabFormStripContainerItem : ImageItem, IDesignTimeProvider
    {
        #region Private Variables
        private TabFormItemsSimpleContainer _ItemContainer = null;
        private CaptionItemContainer _CaptionContainer = null;
        private SystemCaptionItem _SystemCaptionItem = null;
        private TabFormStripControl _TabStrip = null;
        private SystemCaptionItem _WindowIcon = null;
        private Separator _IconSeparator = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates new instance of the class and initializes it with the parent RibbonStrip control.
        /// </summary>
        /// <param name="parent">Reference to parent RibbonStrip control</param>
        public TabFormStripContainerItem(TabFormStripControl parent)
        {
            _TabStrip = parent;

            // We contain other controls
            m_IsContainer = true;
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            _ItemContainer = new TabFormItemsSimpleContainer();
            _ItemContainer.ContainerControl = parent;
            _ItemContainer.GlobalItem = false;
            _ItemContainer.OverlapSpacing = TabFormItem.TabOverlap;
            _ItemContainer.OverlapType = typeof(TabFormItemBase);
            //_ItemContainer.WrapItems = false;
            //_ItemContainer.EventHeight = false;
            //_ItemContainer.UseMoreItemsButton = false;
            _ItemContainer.Stretch = true;
            _ItemContainer.Displayed = true;
            //_ItemContainer.SystemContainer = true;
            //_ItemContainer.PaddingTop = 0;
            //_ItemContainer.PaddingBottom = 0;
            //_ItemContainer.PaddingLeft = 0;
            //_ItemContainer.ItemSpacing = 1;

            _CaptionContainer = new CaptionItemContainer();
            _CaptionContainer.ContainerControl = parent;
            _CaptionContainer.GlobalItem = false;
            _CaptionContainer.WrapItems = false;
            _CaptionContainer.EventHeight = false;
            _CaptionContainer.EqualButtonSize = false;
            _CaptionContainer.ToolbarItemsAlign = eContainerVerticalAlignment.Top;
            _CaptionContainer.UseMoreItemsButton = false;
            _CaptionContainer.Stretch = true;
            _CaptionContainer.Displayed = true;
            _CaptionContainer.SystemContainer = true;
            _CaptionContainer.PaddingBottom = 0;
            _CaptionContainer.PaddingTop = 0;
            _CaptionContainer.PaddingLeft = 6;
            _CaptionContainer.ItemSpacing = 1;
            _CaptionContainer.TrackSubItemsImageSize = false;
            _CaptionContainer.ItemAdded += new EventHandler(this.CaptionContainerNewItemAdded);

            _CaptionItemsContainer = new CaptionItemContainer();
            _CaptionItemsContainer.ContainerControl = parent;
            _CaptionItemsContainer.GlobalItem = false;
            _CaptionItemsContainer.WrapItems = false;
            _CaptionItemsContainer.EventHeight = false;
            _CaptionItemsContainer.EqualButtonSize = false;
            _CaptionItemsContainer.ToolbarItemsAlign = eContainerVerticalAlignment.Top;
            _CaptionItemsContainer.UseMoreItemsButton = false;
            _CaptionItemsContainer.Stretch = true;
            _CaptionItemsContainer.Displayed = true;
            _CaptionItemsContainer.SystemContainer = true;
            _CaptionItemsContainer.PaddingBottom = 0;
            _CaptionItemsContainer.PaddingTop = 0;
            _CaptionItemsContainer.PaddingLeft = 6;
            _CaptionItemsContainer.ItemSpacing = 1;
            _CaptionItemsContainer.TrackSubItemsImageSize = false;

            this.SubItems.Add(_CaptionContainer);
            this.SubItems.Add(_CaptionItemsContainer);
            this.SubItems.Add(_ItemContainer);

            SystemCaptionItem sc = new SystemCaptionItem();
            sc.RestoreEnabled = false;
            sc.IsSystemIcon = false;
            sc.ItemAlignment = eItemAlignment.Far;
            sc.SetSystemItem(true);
            _SystemCaptionItem = sc;
            this.SubItems.Add(_SystemCaptionItem);
        }


        #endregion

        #region Internal Implementation
        internal void OnCaptionVisibleChanged(bool newCaptionVisible)
        {
            if (_SystemCaptionItem != null)
            {
                if (newCaptionVisible)
                {
                    if (_SystemCaptionItem.Parent != _CaptionContainer)
                    {
                        if (_SystemCaptionItem.Parent != null)
                            _SystemCaptionItem.Parent.SubItems.Remove(_SystemCaptionItem);
                        _CaptionContainer.SubItems.Add(_SystemCaptionItem);
                    }
                }
                else
                {
                    if (_SystemCaptionItem.Parent != this)
                    {
                        if (_SystemCaptionItem.Parent != null)
                            _SystemCaptionItem.Parent.SubItems.Remove(_SystemCaptionItem);
                        this.SubItems.Add(_SystemCaptionItem);
                    }
                }
            }
        }

        private CaptionItemContainer _CaptionItemsContainer = null;
        /// <summary>
        /// Gets the list of the items displayed in form caption if its visible.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false)]
        public SubItemsCollection CaptionItems
        {
            get { return _CaptionItemsContainer.SubItems; }
        }

        /// <summary>
        /// Occurs after an item has been added to the container. This procedure is called on both item being added and the parent of the item. To distinguish between those two states check the item parameter.
        /// </summary>
        /// <param name="item">When occurring on the parent this will hold the reference to the item that has been added. When occurring on the item being added this will be null (Nothing).</param>
        protected internal override void OnItemAdded(BaseItem item)
        {
            if (_SystemCaptionItem != null && _SystemCaptionItem.Parent == this && item != _SystemCaptionItem)
            {
                this.SubItems._Remove(_SystemCaptionItem);
                this.SubItems._Add(_SystemCaptionItem);
            }
            base.OnItemAdded(item);
        }

        private void CaptionContainerNewItemAdded(object sender, EventArgs e)
        {
            if (sender is BaseItem)
            {
                BaseItem item = sender as BaseItem;
                if (!(item is SystemCaptionItem))
                {
                    if (_CaptionContainer.SubItems.Contains(_SystemCaptionItem))
                    {
                        _CaptionContainer.SubItems._Remove(_SystemCaptionItem);
                        _CaptionContainer.SubItems._Add(_SystemCaptionItem);
                    }
                }
            }
        }

        internal void ReleaseSystemFocus()
        {
            _ItemContainer.ReleaseSystemFocus();
            if (_TabStrip.CaptionVisible)
            {
                _CaptionContainer.ReleaseSystemFocus();
                _CaptionItemsContainer.ReleaseSystemFocus();
            }
        }

        public override void ContainerLostFocus(bool appLostFocus)
        {
            base.ContainerLostFocus(appLostFocus);
            _ItemContainer.ContainerLostFocus(appLostFocus);
            if (_TabStrip.CaptionVisible)
            {
                _CaptionContainer.ContainerLostFocus(appLostFocus);
                _CaptionItemsContainer.ContainerLostFocus(appLostFocus);
            }
        }

        internal void SetSystemFocus()
        {
            if (_TabStrip.CaptionVisible && _ItemContainer.ExpandedItem() == null)
                _CaptionContainer.SetSystemFocus();
            else
                _ItemContainer.SetSystemFocus();
        }

        /// <summary>
        /// Paints this base container
        /// </summary>
        public override void Paint(ItemPaintArgs pa)
        {
            if (this.SuspendLayout)
                return;
            if (_ScrollEnabled)
            {
                pa.Graphics.SetClip(_ScrollClipBounds);
                _ItemContainer.ClippingRectangle = _ScrollClipBounds;
            }
            else
                _ItemContainer.ClippingRectangle = Rectangle.Empty;
            _ItemContainer.Paint(pa);
            if (_TabStrip.CaptionVisible)
            {
                _CaptionContainer.Paint(pa);
                _CaptionItemsContainer.Paint(pa);
            }
            if (_SystemCaptionItem.Parent == this)
                _SystemCaptionItem.Paint(pa);

        }

        private int _ScrollButtonWidth = 22;
        /// <summary>
        /// Gets or sets the scroll button width.
        /// </summary>
        public int ScrollButtonWidth
        {
            get { return _ScrollButtonWidth; }
            set { _ScrollButtonWidth = value; }
        }
        private bool _ScrollEnabled = false;
        private ButtonX _LeftScrollButton = null, _RightScrollButton = null;
        private Rectangle _ScrollClipBounds = Rectangle.Empty;
        private int _MaxScrolPos = 0;
        private void RepositionScrollButtons()
        {
            if (_LeftScrollButton == null || _RightScrollButton == null) return;
            Rectangle tabsBounds = GetItemContainerBounds();
            Rectangle itemsBounds = _ItemContainer.Bounds;
            int scrollButtonWidth = Dpi.Width(_ScrollButtonWidth);
            _LeftScrollButton.Bounds = new Rectangle(tabsBounds.X, itemsBounds.Y + Dpi.Height2, scrollButtonWidth, itemsBounds.Height - Dpi.Height3);
            _RightScrollButton.Bounds = new Rectangle(tabsBounds.Right - scrollButtonWidth, itemsBounds.Y + Dpi.Height2, scrollButtonWidth, itemsBounds.Height - Dpi.Height3);
        }

        public override void RecalcSize()
        {
            if (this.SuspendLayout)
                return;
            TabFormStripControl strip = this.ContainerControl as TabFormStripControl;
            Form form = null;
            if (strip != null)
                form = strip.FindForm();
            if (_SystemCaptionItem.Parent == this)
            {
                _SystemCaptionItem.Displayed = true;
                _SystemCaptionItem.RecalcSize();
                _SystemCaptionItem.LeftInternal = this.Bounds.Right - _SystemCaptionItem.WidthInternal;
                _SystemCaptionItem.TopInternal = this.Bounds.Y + (IsParentFormMaximized ? Dpi.Height4 : Dpi.Height1);
            }
            Rectangle tabsBounds = GetItemContainerBounds();
            _ItemContainer.Bounds = tabsBounds;
            _ItemContainer.RecalcSize();
            if (_ItemContainer.WidthInternal > tabsBounds.Width) // Using scroll buttons
            {
                _MaxScrolPos = (tabsBounds.Width - Dpi.Width(_ScrollButtonWidth * 2)) - _ItemContainer.WidthInternal;
                if (_ScrollEnabled)
                {
                    // Just check scroll position
                    //throw new NotImplementedException();
                    RepositionScrollButtons();
                }
                else
                {
                    if (strip != null)
                    {
                        _LeftScrollButton = new ButtonX();
                        _LeftScrollButton.Name = "sysLeftScrollButton";
                        _LeftScrollButton.Symbol = "\ue314";
                        _LeftScrollButton.SymbolSet = eSymbolSet.Material;
                        _LeftScrollButton.Shape = new RoundRectangleShapeDescriptor(0);
                        _LeftScrollButton.Click += LeftScrollButtonClick;
                        _RightScrollButton = new ButtonX();
                        _RightScrollButton.Name = "sysRightScrollButton";
                        _RightScrollButton.Symbol = "\ue315";
                        _RightScrollButton.SymbolSet = eSymbolSet.Material;
                        _RightScrollButton.Shape = new RoundRectangleShapeDescriptor(0);
                        _RightScrollButton.Click += RightScrollButtonClick;
                        RepositionScrollButtons();
                        strip.Controls.Add(_LeftScrollButton);
                        strip.Controls.Add(_RightScrollButton);
                        _ScrollEnabled = true;
                    }
                }
                tabsBounds.X += Dpi.Width(_ScrollButtonWidth);
                tabsBounds.Width -= Dpi.Width(_ScrollButtonWidth) * 2;
                _ItemContainer.Bounds = tabsBounds;
                _ItemContainer.RecalcSize();
                if (_ScrollPos < _MaxScrolPos) _ScrollPos = _MaxScrolPos;
                _ItemContainer.ScrollPosition = _ScrollPos;
                _ScrollClipBounds = tabsBounds;
                //_ScrollClipBounds.X += Dpi.Width(_ScrollButtonWidth);
                //_ScrollClipBounds.Width -= Dpi.Width(_ScrollButtonWidth) * 2;
            }
            else if (_ScrollEnabled)
            {
                // Remove scrolling
                if (_LeftScrollButton != null)
                {
                    if (_LeftScrollButton.Parent != null)
                        _LeftScrollButton.Parent.Controls.Remove(_LeftScrollButton);
                    _LeftScrollButton.Click -= LeftScrollButtonClick;
                    _LeftScrollButton.Dispose();
                    _LeftScrollButton = null;
                }
                if (_RightScrollButton != null)
                {
                    if (_RightScrollButton.Parent != null)
                        _RightScrollButton.Parent.Controls.Remove(_RightScrollButton);
                    _RightScrollButton.Click -= RightScrollButtonClick;
                    _RightScrollButton.Dispose();
                    _RightScrollButton = null;
                }
                _ScrollEnabled = false;
            }

            if (_ItemContainer.HeightInternal < 0) _ItemContainer.HeightInternal = 0;
            bool isMaximized = false;
            if (_TabStrip.CaptionVisible)
            {
                Rectangle captionContainerBounds = GetCaptionContainerBounds();
                _CaptionContainer.Bounds = captionContainerBounds;
                _CaptionContainer.RecalcSize();
                Size frameBorderSize = SystemInformation.FrameBorderSize;
                if (strip != null)
                {
                    if (form != null)
                    {
                        if (_WindowIcon != null)
                            _WindowIcon.SetVisibleDirect(form.ShowIcon && _TabStrip.ShowIcon);
                        if (_IconSeparator != null)
                            _IconSeparator.SetVisibleDirect(form.ShowIcon && _TabStrip.ShowIcon);
                    }
                    TabParentForm appForm = form as TabParentForm;
                    if (appForm != null)
                    {
                        NonClientInfo nci = appForm.GetNonClientInfo();
                        frameBorderSize.Width = nci.LeftBorder;
                        frameBorderSize.Height = nci.BottomBorder;
                    }
                }
                if (_TabStrip.CaptionHeight == 0 && _SystemCaptionItem.TopInternal < (frameBorderSize.Height - 1))
                {
                    if (form != null && form.WindowState == FormWindowState.Maximized)
                        isMaximized = true;

                    if (isMaximized)
                    {
                        _SystemCaptionItem.TopInternal = 1;
                        if (_WindowIcon != null) _WindowIcon.TopInternal = 1;
                    }
                    else
                    {
                        _SystemCaptionItem.TopInternal = Math.Max(1, frameBorderSize.Height - 6);
                        if (_WindowIcon != null) _WindowIcon.TopInternal = frameBorderSize.Height - 5;
                    }
                }

                // Adjust the Y position of the items inside of the caption container since they are top aligned and
                // quick access toolbar items should be aligned with the bottom of the system caption item.
                if (System.Environment.OSVersion.Version.Major >= 6)
                {
                    int topOffset = 3;
                    if (isMaximized)
                        topOffset += 1;
                    int maxBottom = 0;
                    foreach (BaseItem item in _CaptionContainer.SubItems)
                    {
                        if (!(item is ApplicationButton || item is DevComponents.DotNetBar.Metro.MetroAppButton) && item != _SystemCaptionItem && item != _IconSeparator)
                            item.TopInternal += topOffset;
                        else if (item == _IconSeparator)
                            item.TopInternal += (isMaximized ? 2 : Dpi.Height4);
                        maxBottom = Math.Max(item.Bounds.Bottom, maxBottom);
                    }
                    if (_CaptionContainer.MoreItems != null)
                        _CaptionContainer.MoreItems.TopInternal += topOffset;
                    if (maxBottom > _CaptionContainer.HeightInternal) _CaptionContainer.SetDisplayRectangle(new Rectangle(_CaptionContainer.Bounds.X, _CaptionContainer.Bounds.Y, _CaptionContainer.Bounds.Width, maxBottom));
                }
                else
                {
                    int maxBottom = 0;
                    foreach (BaseItem item in _CaptionContainer.SubItems)
                    {
                        if (item.HeightInternal < _SystemCaptionItem.HeightInternal && (item != _IconSeparator && item != _WindowIcon))
                        {
                            //item.TopInternal += (m_SystemCaptionItem.HeightInternal - item.HeightInternal);
                            item.TopInternal = (_SystemCaptionItem.Bounds.Bottom - (item.HeightInternal + ((item is LabelItem) ? 2 : 0)));
                            maxBottom = Math.Max(item.Bounds.Bottom, maxBottom);
                        }
                    }
                    if (_CaptionContainer.MoreItems != null)
                        _CaptionContainer.MoreItems.TopInternal += (_SystemCaptionItem.HeightInternal - _CaptionContainer.MoreItems.HeightInternal);
                    if (maxBottom > _CaptionContainer.HeightInternal) _CaptionContainer.SetDisplayRectangle(new Rectangle(_CaptionContainer.Bounds.X, _CaptionContainer.Bounds.Y, _CaptionContainer.Bounds.Width, maxBottom));
                }

                // Items contained in caption are inside their own container
                Rectangle captionItemsContainerBounds = captionContainerBounds;
                if (_IconSeparator != null)
                {
                    captionItemsContainerBounds.X = _IconSeparator.Bounds.Right + Dpi.Width6;
                    captionItemsContainerBounds.Width -= _IconSeparator.Bounds.Right + Dpi.Width6;
                }
                if (_SystemCaptionItem != null)
                {
                    captionItemsContainerBounds.Width -= (captionItemsContainerBounds.Right -
                                                          _SystemCaptionItem.Bounds.Left) + Dpi.Width6;
                }

                if (strip != null)
                {
                    Size titleSize = Size.Empty;
                    using (Graphics g = strip.CreateGraphics())
                    {
                        string text = strip.TitleText;
                        if (string.IsNullOrEmpty(text) && form != null) text = form.Text;
                        bool isTitleTextMarkup = strip.TitleTextMarkupBody != null;
                        if (isTitleTextMarkup)
                        {
                            TextMarkup.MarkupDrawContext d = new TextMarkup.MarkupDrawContext(g, strip.Font, Color.Black,
                                strip.RightToLeft == System.Windows.Forms.RightToLeft.Yes, captionItemsContainerBounds, false);
                            d.AllowMultiLine = false;
                            TextMarkup.BodyElement body = strip.TitleTextMarkupBody;
                            body.Measure(captionItemsContainerBounds.Size, d);
                            titleSize = body.Bounds.Size;
                            strip.TitleTextMarkupLastArrangeBounds=Rectangle.Empty;
                        }
                        else
                        {
                            titleSize = TextDrawing.MeasureString(g, text, strip.Font);
                        }
                    }
                    captionItemsContainerBounds.X += titleSize.Width + Dpi.Width4;
                    captionItemsContainerBounds.Width -= titleSize.Width + Dpi.Width4;
                }

                if (captionItemsContainerBounds.Width > 0)
                {
                    _CaptionItemsContainer.Bounds = captionItemsContainerBounds;
                    _CaptionItemsContainer.RecalcSize();
                    // Adjust the Y position of the items inside of the caption container since they are top aligned and
                    // quick access toolbar items should be aligned with the bottom of the system caption item.
                    if (_CaptionItemsContainer.SubItems.Count > 0)
                    {
                        int topOffset = 3;
                        if (isMaximized)
                            topOffset += 1;
                        int maxBottom = 0;
                        foreach (BaseItem item in _CaptionItemsContainer.SubItems)
                        {
                            item.TopInternal += topOffset;
                            maxBottom = Math.Max(item.Bounds.Bottom, maxBottom);
                        }
                        if (_CaptionItemsContainer.MoreItems != null)
                            _CaptionItemsContainer.MoreItems.TopInternal += topOffset;
                        if (maxBottom > _CaptionItemsContainer.HeightInternal)
                            _CaptionItemsContainer.SetDisplayRectangle(new Rectangle(_CaptionItemsContainer.Bounds.X,
                                _CaptionItemsContainer.Bounds.Y, _CaptionItemsContainer.Bounds.Width, maxBottom));
                    }
                }

                if (_ItemContainer.HeightInternal == 0)
                    this.HeightInternal = _CaptionContainer.HeightInternal;
                else
                    this.HeightInternal = _ItemContainer.Bounds.Bottom;// -m_CaptionContainer.Bounds.Top;
            }
            else
            {
                int h = _ItemContainer.HeightInternal;
                this.HeightInternal = h;
            }

            base.RecalcSize();
        }

        private bool IsParentFormMaximized
        {
            get
            {
                Control c = (Control)this.ContainerControl;
                if (c != null)
                {
                    Form form = c.FindForm();
                    if (form != null)
                        return (form.WindowState == FormWindowState.Maximized);
                }
                return false;
            }
        }
        private int _ScrollStep = 96;
        /// <summary>
        /// Indicates the scroll in pixels each time scroll button is pressed.
        /// </summary>
        public int ScrollStep
        {
            get { return _ScrollStep; }
            set { _ScrollStep = value; }
        }

        private Animation.AnimationInt _Animation = null;
        private int _ScrollPos = 0;
        void RightScrollButtonClick(object sender, EventArgs e)
        {
            int newScrollPos = Math.Max(_MaxScrolPos, _ScrollPos - Dpi.Width(_ScrollStep));
            if (newScrollPos != _ScrollPos)
            {
                WaitForCurrentAnimationToComplete();
                EnsureAnimation();
                _ScrollPos = newScrollPos;
                _Animation.Animations.Add(new Animation.AnimationRequest(_ItemContainer, "ScrollPosition", _ItemContainer.ScrollPosition, newScrollPos));
                _Animation.Start();
            }
        }

        void LeftScrollButtonClick(object sender, EventArgs e)
        {
            int newScrollPos = Math.Min(0, _ScrollPos + Dpi.Width(_ScrollStep));
            if (newScrollPos != _ScrollPos)
            {
                WaitForCurrentAnimationToComplete();
                EnsureAnimation();
                _ScrollPos = newScrollPos;
                _Animation.Animations.Add(new Animation.AnimationRequest(_ItemContainer, "ScrollPosition", _ItemContainer.ScrollPosition, newScrollPos));
                _Animation.Start();
            }
        }

        private void EnsureAnimation()
        {
            if (_Animation == null)
            {
                _Animation = new Animation.AnimationInt(Animation.AnimationEasing.EaseOutQuint, 300);
                _Animation.AnimationCompleted += AnimationCompleted;
            }
        }
        private void AnimationCompleted(object sender, EventArgs e)
        {
            Animation.Animation anim = (Animation.Animation)sender;
            anim.AnimationCompleted -= AnimationCompleted;
            anim.Dispose();
        }
        private void WaitForCurrentAnimationToComplete()
        {
            if (_Animation != null)
            {
                DateTime start = DateTime.Now;
                while (!_Animation.IsDisposed)
                {
                    Application.DoEvents();
                    if (DateTime.Now.Subtract(start).TotalMilliseconds > 1000)
                    {
                        AbortCurrentAnimation();
                        break;
                    }
                }
                _Animation = null;
            }
        }
        private void AbortCurrentAnimation()
        {
            Animation.Animation anim = _Animation;
            _Animation = null;
            if (anim != null)
            {
                anim.Stop();
                anim.Dispose();
            }
        }

        private Rectangle GetItemContainerBounds()
        {
            Rectangle r = _TabStrip.GetItemContainerBounds();
            if (_SystemCaptionItem.Parent == this)
                r.Width -= _SystemCaptionItem.WidthInternal + Dpi.Width2;
            return r;
        }

        private Rectangle GetCaptionContainerBounds()
        {
            return _TabStrip.GetCaptionContainerBounds();
        }

        /// <summary>
        /// Gets reference to internal ribbon strip container that contains tabs and/or other items.
        /// </summary>
        public TabFormItemsSimpleContainer TabsContainer
        {
            get { return _ItemContainer; }
        }

        /// <summary>
        /// Gets reference to internal caption container item that contains the quick toolbar, start button and system caption item.
        /// </summary>
        public GenericItemContainer CaptionContainer
        {
            get { return _CaptionContainer; }
        }

        public SystemCaptionItem SystemCaptionItem
        {
            get { return _SystemCaptionItem; }
        }

        /// <summary>
        /// Returns copy of GenericItemContainer item
        /// </summary>
        public override BaseItem Copy()
        {
            TabFormStripContainerItem objCopy = new TabFormStripContainerItem(_TabStrip);
            this.CopyToItem(objCopy);

            return objCopy;
        }
        protected override void CopyToItem(BaseItem copy)
        {
            TabFormStripContainerItem objCopy = copy as TabFormStripContainerItem;
            base.CopyToItem(objCopy);
        }


        public override void InternalClick(MouseButtons mb, Point mpos)
        {
            _ItemContainer.InternalClick(mb, mpos);
            if (_SystemCaptionItem.Parent == this)
                _SystemCaptionItem.InternalClick(mb, mpos);
            if (_TabStrip.CaptionVisible)
            {
                _CaptionContainer.InternalClick(mb, mpos);
                _CaptionItemsContainer.InternalClick(mb, mpos);
            }
        }

        public override void InternalDoubleClick(MouseButtons mb, Point mpos)
        {
            _ItemContainer.InternalDoubleClick(mb, mpos);
            if (_TabStrip.CaptionVisible)
            {
                _CaptionContainer.InternalDoubleClick(mb, mpos);
                _CaptionItemsContainer.InternalDoubleClick(mb, mpos);
            }
        }

        public override void InternalMouseDown(MouseEventArgs objArg)
        {
            _ItemContainer.InternalMouseDown(objArg);
            if (_SystemCaptionItem.Parent == this)
                _SystemCaptionItem.InternalMouseDown(objArg);
            if (_TabStrip.CaptionVisible)
            {
                if (this.DesignMode && _CaptionContainer.ItemAtLocation(objArg.X, objArg.Y) != null || !this.DesignMode)
                    _CaptionContainer.InternalMouseDown(objArg);
                if (this.DesignMode && _CaptionItemsContainer.ItemAtLocation(objArg.X, objArg.Y) != null || !this.DesignMode)
                    _CaptionItemsContainer.InternalMouseDown(objArg);
            }
        }

        public override void InternalMouseHover()
        {
            _ItemContainer.InternalMouseHover();
            if (_SystemCaptionItem.Parent == this)
                _SystemCaptionItem.InternalMouseHover();
            if (_TabStrip.CaptionVisible)
            {
                _CaptionContainer.InternalMouseHover();
                _CaptionItemsContainer.InternalMouseHover();
            }
        }

        public override void InternalMouseLeave()
        {
            _ItemContainer.InternalMouseLeave();
            if (_SystemCaptionItem.Parent == this)
                _SystemCaptionItem.InternalMouseLeave();
            if (_TabStrip.CaptionVisible)
            {
                _CaptionContainer.InternalMouseLeave();
                _CaptionItemsContainer.InternalMouseLeave();
            }
        }

        public override void InternalMouseMove(MouseEventArgs objArg)
        {
            _ItemContainer.InternalMouseMove(objArg);
            if (_SystemCaptionItem.Parent == this)
                _SystemCaptionItem.InternalMouseMove(objArg);
            if (_TabStrip.CaptionVisible)
            {
                _CaptionContainer.InternalMouseMove(objArg);
                _CaptionItemsContainer.InternalMouseMove(objArg);
            }
        }

        public override void InternalMouseUp(MouseEventArgs objArg)
        {
            _ItemContainer.InternalMouseUp(objArg);
            if (_SystemCaptionItem.Parent == this)
                _SystemCaptionItem.InternalMouseUp(objArg);
            if (_TabStrip.CaptionVisible)
            {
                _CaptionContainer.InternalMouseUp(objArg);
                _CaptionItemsContainer.InternalMouseUp(objArg);
            }
        }

        public override void InternalKeyDown(KeyEventArgs objArg)
        {
            BaseItem expanded = this.ExpandedItem();
            if (expanded == null)
                expanded = _CaptionContainer.ExpandedItem();
            if (expanded == null)
                expanded = _ItemContainer.ExpandedItem();

            if (expanded == null || !_TabStrip.CaptionVisible)
            {
                _ItemContainer.InternalKeyDown(objArg);
                if (!objArg.Handled && _TabStrip.CaptionVisible)
                {
                    _CaptionContainer.InternalKeyDown(objArg);
                    _CaptionItemsContainer.InternalKeyDown(objArg);
                }
            }
            else
            {
                if (expanded.Parent == _ItemContainer)
                {
                    _ItemContainer.InternalKeyDown(objArg);
                }
                else
                {
                    _CaptionContainer.InternalKeyDown(objArg);
                }
            }
        }

        private bool _NewTabItemVisible = false;
        /// <summary>
        /// Indicates whether new tab item which allows creation of new tab when clicked is visible. When visible you need to handle CreateNewTab event and create your new tab in event handler.
        /// </summary>
        [DefaultValue(false), Category("Appearance"), Description("Indicates whether new tab item which allows creation of new tab when clicked is visible. When visible you need to handle CreateNewTab event and create your new tab in event handler.")]
        public bool NewTabItemVisible
        {
            get { return _NewTabItemVisible; }
            set
            {
                _NewTabItemVisible = value;
                UpdateNewTabItemVisibility();
            }
        }

        private NewTabFormItem _NewTabFormItem = null;
        private void UpdateNewTabItemVisibility()
        {
            if (_NewTabItemVisible)
            {
                if (_NewTabFormItem == null)
                {
                    _NewTabFormItem = new NewTabFormItem();
                    _ItemContainer.SubItems.Add(_NewTabFormItem);
                    _NewTabFormItem.Click += NewTabFormItemClick;
                }
            }
            else
            {
                if (_NewTabFormItem != null)
                {
                    _NewTabFormItem.Parent.SubItems.Remove(_NewTabFormItem);
                    _NewTabFormItem.Dispose();
                    _NewTabFormItem = null;
                }
            }

            NeedRecalcSize = true;
            this.RecalcSize();
        }

        private void NewTabFormItemClick(object sender, EventArgs e)
        {
            OnCreateNewTab(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when new tab item is clicked by end user and allows you to create and add new tab to the control.
        /// </summary>
        [Description("Occurs when new tab item is clicked by end user and allows you to create and add new tab to the control.")]
        public event EventHandler CreateNewTab;

        /// <summary>
        /// Raises CreateNewTab event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnCreateNewTab(EventArgs e)
        {
            EventHandler handler = CreateNewTab;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Return Sub Item at specified location
        /// </summary>
        public override BaseItem ItemAtLocation(int x, int y)
        {
            if (_ItemContainer.DisplayRectangle.Contains(x, y))
                return _ItemContainer.ItemAtLocation(x, y);

            if (_CaptionContainer.DisplayRectangle.Contains(x, y))
                return _CaptionContainer.ItemAtLocation(x, y);

            return null;
        }

        protected override void OnStyleChanged()
        {
            eDotNetBarStyle effectiveStyle = this.EffectiveStyle;
            //if (effectiveStyle == eDotNetBarStyle.Office2010)
            //{
            if (_WindowIcon == null)
            {
                _IconSeparator = new Separator("sys_caption_separator");
                _IconSeparator.SetSystemItem(true);
                _IconSeparator.DesignTimeVisible = false;
                _IconSeparator.CanCustomize = false;
                _CaptionContainer.SubItems._Add(_IconSeparator, 0);
                _WindowIcon = new SystemCaptionItem();
                _WindowIcon.Name = "sys_caption_icon";
                _WindowIcon.Enabled = false;
                _WindowIcon.Style = this.Style;
                _WindowIcon.IsSystemIcon = true;
                _WindowIcon.DesignTimeVisible = false;
                _WindowIcon.CanCustomize = false;
                _WindowIcon.QueryIconOnPaint = true;
                _WindowIcon.MouseDown += WindowIconMouseDown;
                _WindowIcon.DoubleClick += WindowIconDoubleClick;
                _WindowIcon.SetSystemItem(true);
                _CaptionContainer.SubItems._Add(_WindowIcon, 0);
            }
            //}
            //else if (effectiveStyle == eDotNetBarStyle.Windows7)
            //{
            //    if (_WindowIcon == null)
            //    {
            //        _IconSeparator = new Separator("sys_caption_separator");
            //        _IconSeparator.FixedSize = new Size(3, 12);
            //        _IconSeparator.SetSystemItem(true);
            //        _IconSeparator.DesignTimeVisible = false;
            //        _IconSeparator.CanCustomize = false;
            //        _CaptionContainer.SubItems._Add(_IconSeparator, 0);
            //        _WindowIcon = new SystemCaptionItem();
            //        _WindowIcon.Name = "sys_caption_icon";
            //        _WindowIcon.Enabled = false;
            //        _WindowIcon.Style = this.Style;
            //        _WindowIcon.IsSystemIcon = true;
            //        _WindowIcon.QueryIconOnPaint = true;
            //        _WindowIcon.DesignTimeVisible = false;
            //        _WindowIcon.CanCustomize = false;
            //        _WindowIcon.SetSystemItem(true);
            //        _WindowIcon.MouseDown += WindowIconMouseDown;
            //        _CaptionContainer.SubItems._Add(_WindowIcon, 0);
            //    }
            //}
            //else if (StyleManager.IsMetro(effectiveStyle))
            //{
            //    if (_WindowIcon == null)
            //    {
            //        _IconSeparator = new Separator("sys_caption_separator");
            //        _IconSeparator.FixedSize = new Size(3, 14);
            //        _IconSeparator.SetSystemItem(true);
            //        _IconSeparator.DesignTimeVisible = false;
            //        _IconSeparator.CanCustomize = false;
            //        _CaptionContainer.SubItems._Add(_IconSeparator, 0);
            //        _WindowIcon = new SystemCaptionItem();
            //        _WindowIcon.Name = "sys_caption_icon";
            //        _WindowIcon.Enabled = false;
            //        _WindowIcon.Style = this.Style;
            //        _WindowIcon.IsSystemIcon = true;
            //        _WindowIcon.DesignTimeVisible = false;
            //        _WindowIcon.CanCustomize = false;
            //        _WindowIcon.QueryIconOnPaint = true;
            //        _WindowIcon.MouseDown += WindowIconMouseDown;
            //        _WindowIcon.DoubleClick += WindowIconDoubleClick;
            //        _WindowIcon.SetSystemItem(true);
            //        _CaptionContainer.SubItems._Add(_WindowIcon, 0);
            //    }
            //}
            //else if (_WindowIcon != null)
            //{
            //    if (_CaptionContainer.SubItems.Contains(_WindowIcon))
            //        _CaptionContainer.SubItems._Remove(_WindowIcon);
            //    _WindowIcon.MouseDown -= WindowIconMouseDown;
            //    _WindowIcon.DoubleClick -= WindowIconDoubleClick;
            //    _WindowIcon.Dispose();
            //    _WindowIcon = null;
            //    if (_CaptionContainer.SubItems.Contains(_IconSeparator))
            //        _CaptionContainer.SubItems._Remove(_IconSeparator);
            //    _IconSeparator.Dispose();
            //    _IconSeparator = null;
            //}
            base.OnStyleChanged();
        }

        void WindowIconDoubleClick(object sender, EventArgs e)
        {
            if (_TabStrip != null)
            {
                _TabStrip.CloseParentForm();
            }
        }

        void WindowIconMouseDown(object sender, MouseEventArgs e)
        {
            TabFormStripControl mts = this.ContainerControl as TabFormStripControl;
            if (mts != null)
            {
                Point p = new Point(_WindowIcon.LeftInternal, _WindowIcon.Bounds.Bottom + 1);
                p = mts.PointToScreen(p);
                mts.ShowSystemMenu(p);
            }
        }
        #endregion

        #region IDesignTimeProvider Members

        public InsertPosition GetInsertPosition(Point pScreen, BaseItem DragItem)
        {
            InsertPosition pos = _ItemContainer.GetInsertPosition(pScreen, DragItem);
            //if (pos == null && _TabStrip.CaptionVisible)
            //    pos = _CaptionItemsContainer.GetInsertPosition(pScreen, DragItem);
            return pos;
        }

        public void DrawReversibleMarker(int iPos, bool Before)
        {
            //DesignTimeProviderContainer.DrawReversibleMarker(this, iPos, Before);
        }

        public void InsertItemAt(BaseItem objItem, int iPos, bool Before)
        {
            //DesignTimeProviderContainer.InsertItemAt(this, objItem, iPos, Before);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is expanded or not. For Popup items this would indicate whether the item is popped up or not.
        /// </summary>
        [System.ComponentModel.Browsable(false), System.ComponentModel.DefaultValue(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public override bool Expanded
        {
            get
            {
                return base.Expanded;
            }
            set
            {
                base.Expanded = value;
                if (!value)
                {
                    foreach (BaseItem item in this.SubItems)
                        item.Expanded = false;
                }
            }
        }

        /// <summary>
        /// When parent items does recalc size for its sub-items it should query
        /// image size and store biggest image size into this property.
        /// </summary>
        [System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override System.Drawing.Size SubItemsImageSize
        {
            get
            {
                return base.SubItemsImageSize;
            }
            set
            {
                //m_SubItemsImageSize = value;
            }
        }
        #endregion
    }

    // <summary>
    /// Represents simple item container which orders items horizontally and support all ItemAlignment settings.
    /// </summary>
    [ToolboxItem(false), EditorBrowsable(EditorBrowsableState.Never)]
    public class TabFormItemsSimpleContainer : SimpleItemContainer
    {
        public override void Paint(ItemPaintArgs p)
        {
            TabFormItem selectedTab = null;
            //foreach (BaseItem item in this.SubItems)
            for (int i = this.SubItems.Count - 1; i >= 0; i--)
            {
                BaseItem item = this.SubItems[i];
                if (!item.Displayed) continue;
                if (item.BeginGroup)
                {
                    DisplayHelp.DrawLine(p.Graphics, item.LeftInternal - 3, this.TopInternal + 4, item.LeftInternal - 3, this.Bounds.Bottom - 8, p.Colors.ItemSeparator, 1);
                }
                TabFormItem tab = item as TabFormItem;
                if (tab != null && tab.Checked && selectedTab == null)
                    selectedTab = tab;
                else
                {
                    if (_ClippingRectangle.IsEmpty || _ClippingRectangle.IntersectsWith(item.Bounds))
                        item.Paint(p);
                }
            }
            if (selectedTab != null)
            {
                selectedTab.Paint(p);
            }
        }

        public override void RecalcSize()
        {
            _ScrollPosition = 0;
            base.RecalcSize();
        }

        /// <summary>
        /// Return Sub Item at specified location
        /// </summary>
        public override BaseItem ItemAtLocation(int x, int y)
        {
            foreach (BaseItem objSub in SubItems)
            {
                TabFormItemBase tab = objSub as TabFormItemBase;
                if (tab != null && tab.Visible && tab.Displayed && tab.TabPath != null)
                {
                    if (tab.TabPath.IsVisible(x, y))
                        return tab;
                }
            }

            return base.ItemAtLocation(x, y);
        }

        private NewTabFormItem _NewTabFormItem = null;
        /// <summary>
        /// Occurs after an item has been added to the container. This procedure is called on both item being added and the parent of the item. To distinguish between those two states check the item parameter.
        /// </summary>
        /// <param name="item">When occurring on the parent this will hold the reference to the item that has been added. When occurring on the item being added this will be null (Nothing).</param>
        protected internal override void OnItemAdded(BaseItem item)
        {
            if (item is NewTabFormItem)
                _NewTabFormItem = (NewTabFormItem)item;
            else if (_NewTabFormItem != null)
            {
                this.SubItems._Remove(_NewTabFormItem);
                this.SubItems._Add(_NewTabFormItem);
            }
            base.OnItemAdded(item);
        }

        /// <summary>
        /// Occurs after an item has been removed.
        /// </summary>
        /// <param name="item">Item being removed.</param>
        protected internal override void OnAfterItemRemoved(BaseItem item, int itemIndex)
        {
            if (item == _NewTabFormItem)
                _NewTabFormItem = null;
            base.OnAfterItemRemoved(item, itemIndex);
        }

        protected override bool IsVerticallyCentered(BaseItem item)
        {
            return !(item is TabFormItem);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is expanded or not. For Popup items this would indicate whether the item is popped up or not.
        /// </summary>
        [System.ComponentModel.Browsable(false), System.ComponentModel.DefaultValue(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public override bool Expanded
        {
            get
            {
                return base.Expanded;
            }
            set
            {
                base.Expanded = value;
                if (!value)
                {
                    foreach (BaseItem item in this.SubItems)
                        item.Expanded = false;
                }
            }
        }

        private Rectangle _ClippingRectangle;
        public Rectangle ClippingRectangle
        {
            get { return _ClippingRectangle; }
            set { _ClippingRectangle = value; }
        }

        private int _ScrollPosition = 0;
        [System.Reflection.Obfuscation(Exclude = true)]
        public int ScrollPosition
        {
            get
            {
                return _ScrollPosition;
            }
            set
            {
                value = Math.Min(0, value); // Must be negative number
                if (_ScrollPosition != value)
                {
                    Point offset = new Point(value - _ScrollPosition, 0);
                    _ScrollPosition = value;
                    OffsetSubItems(offset);
                    this.Refresh();
                }
            }
        }

        private void OffsetSubItems(Point offset)
        {
            if (offset.IsEmpty)
                return;

            BaseItem[] items = new BaseItem[this.SubItems.Count];
            this.SubItems.CopyTo(items, 0);
            foreach (IBlock b in items)
            {
                Rectangle r = b.Bounds;
                r.Offset(offset);
                b.Bounds = r;
            }
        }
    }
}
