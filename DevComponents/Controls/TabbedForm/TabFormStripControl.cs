using DevComponents.DotNetBar.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using DevComponents.DotNetBar.Metro;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Represents tabbed UI strip control.
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    public class TabFormStripControl : ItemControl
    {
        #region Private Variables & Constructor
        private TabFormStripContainerItem _StripContainer = null;
        private int _CaptionHeight = 0;
        private bool _CaptionVisible = false;
        private Rectangle _CaptionBounds = Rectangle.Empty;
        private Rectangle _SystemCaptionItemBounds = Rectangle.Empty;
        private Font _CaptionFont = null;
        private bool _CanCustomize = true;
        private ElementStyle _DefaultBackgroundStyle = new ElementStyle();
        private string _TitleText = "";

        public TabFormStripControl()
        {
            this.SetStyle(ControlStyles.StandardDoubleClick, true);

            _StripContainer = new TabFormStripContainerItem(this);
            _StripContainer.GlobalItem = false;
            _StripContainer.ContainerControl = this;
            _StripContainer.Displayed = true;
            _StripContainer.SetOwner(this);
            _StripContainer.Style = eDotNetBarStyle.StyleManagerControlled;
            _StripContainer.CreateNewTab += StripContainerCreateNewTab;
            this.SetBaseItemContainer(_StripContainer);

            this.ColorScheme.Style = eDotNetBarStyle.Office2003;

            this.AutoSize = true;
            this.DragDropSupport = true;
            this.AllowDrop = true;
            this.AllowExternalDrop = true;

            // Setup system caption item
            _StripContainer.SystemCaptionItem.Click += new EventHandler(SystemCaptionClick);

            StyleManager.Register(this);
        }

        void StripContainerCreateNewTab(object sender, EventArgs e)
        {
            if (this.Parent is TabFormControl)
                ((TabFormControl)this.Parent).RaiseCreateNewTab(e);
        }
        protected override void Dispose(bool disposing)
        {
            StyleManager.Unregister(this);
            base.Dispose(disposing);
        }
        /// <summary>
        /// Called by StyleManager to notify control that style on manager has changed and that control should refresh its appearance if
        /// its style is controlled by StyleManager.
        /// </summary>
        /// <param name="newStyle">New active style.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void StyleManagerStyleChanged(eDotNetBarStyle newStyle)
        {
            this.Style = StyleManager.GetEffectiveStyle();
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when text markup link from TitleText markup is clicked. Markup links can be created using "a" tag, for example:
        /// <a name="MyLink">Markup link</a>
        /// </summary>
        [Description("Occurs when text markup link from TitleText markup is clicked.")]
        public event MarkupLinkClickEventHandler TitleTextMarkupLinkClick;
        #endregion

        #region Internal Implementation
        private TextMarkup.BodyElement _TitleTextMarkup = null;
        /// <summary>
        /// Gets or sets the rich text displayed on Ribbon Title instead of the Form.Text property. This property supports text-markup.
        /// You can use <font color="SysCaptionTextExtra"> markup to instruct the markup renderer to use Office 2007 system caption extra text color which
        /// changes depending on the currently selected color table. Note that when using this property you should manage also the Form.Text property since
        /// that is the text that will be displayed in Windows task-bar and elsewhere where system Form.Text property is used.
        /// You can also use the hyperlinks as part of the text markup and handle the TitleTextMarkupLinkClick event to be notified when they are clicked.
        /// </summary>
        [Browsable(true), DefaultValue(""), Editor("DevComponents.DotNetBar.Design.TextMarkupUIEditor, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf", typeof(System.Drawing.Design.UITypeEditor)), EditorBrowsable(EditorBrowsableState.Always), Category("Appearance"), Description("Indicates text displayed on Ribbon Title instead of the Form.Text property.")]
        public string TitleText
        {
            get { return _TitleText; }
            set
            {
                if (value == null) value = "";
                _TitleText = value;
                _TitleTextMarkup = null;

                if (!TextMarkup.MarkupParser.IsMarkup(ref _TitleText))
                    return;

                _TitleTextMarkup = TextMarkup.MarkupParser.Parse(_TitleText);

                if (_TitleTextMarkup != null)
                    _TitleTextMarkup.HyperLinkClick += new EventHandler(InternalTitleTextMarkupLinkClick);
                TitleTextMarkupLastArrangeBounds = Rectangle.Empty;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Occurs when text markup link is clicked.
        /// </summary>
        private void InternalTitleTextMarkupLinkClick(object sender, EventArgs e)
        {
            TextMarkup.HyperLink link = sender as TextMarkup.HyperLink;
            if (link != null)
            {
                if (TitleTextMarkupLinkClick != null)
                    TitleTextMarkupLinkClick(this, new MarkupLinkClickEventArgs(link.Name, link.HRef));
            }
        }

        /// <summary>
        /// Gets reference to parsed markup body element if text was markup otherwise returns null.
        /// </summary>
        internal TextMarkup.BodyElement TitleTextMarkupBody
        {
            get { return _TitleTextMarkup; }
        }

        internal Rectangle TitleTextMarkupLastArrangeBounds = Rectangle.Empty;

        /// <summary>
        /// Gets or sets whether control can be customized and items added by end-user using context menu to the quick access toolbar.
        /// Caption of the control must be visible for customization to be enabled. Default value is true.
        /// </summary>
        [DefaultValue(true), Browsable(true), Category("Customization"), Description("Indicates whether control can be customized. Caption must be visible for customization to be fully enabled.")]
        public bool CanCustomize
        {
            get { return _CanCustomize; }
            set { _CanCustomize = value; }
        }

        /// <summary>
        /// Gets or sets the explicit height of the caption provided by control. Caption height when set is composed of the TabGroupHeight and
        /// the value specified here. Default value is 0 which means that system default caption size is used.
        /// </summary>
        [Browsable(true), DefaultValue(0), Category("Appearance"), Description("Indicates explicit height of the caption provided by control")]
        public int CaptionHeight
        {
            get { return _CaptionHeight; }
            set
            {
                _CaptionHeight = value;
                _StripContainer.NeedRecalcSize = true;
            }
        }

        internal bool HasVisibleTabs
        {
            get
            {
                foreach (BaseItem item in this.Items)
                {
                    if (item.Visible) return true;
                }
                return false;
            }
        }

        internal Rectangle CaptionBounds
        {
            get { return _CaptionBounds; }
            set { _CaptionBounds = value; }
        }

        internal Rectangle SystemCaptionItemBounds
        {
            get { return _SystemCaptionItemBounds; }
            set { _SystemCaptionItemBounds = value; }
        }

        internal SystemCaptionItem SystemCaptionItem
        {
            get { return _StripContainer.SystemCaptionItem; }
        }

        /// <summary>
        /// Gets or sets whether custom caption line provided by the control is visible. Default value is false.
        /// This property should be set to true when control is used on Office2007RibbonForm.
        /// </summary>
        [Browsable(true), DefaultValue(false), Category("Appearance"), Description("Indicates whether custom caption line provided by the control is visible.")]
        public bool CaptionVisible
        {
            get { return _CaptionVisible; }
            set
            {
                _CaptionVisible = value;
                OnCaptionVisibleChanged();
            }
        }

        /// <summary>
        /// Gets or sets the font for the form caption text when CaptionVisible=true. Default value is NULL which means that system font is used.
        /// </summary>
        [Browsable(true), DefaultValue(null), Category("Appearance"), Description("")]
        public Font CaptionFont
        {
            get { return _CaptionFont; }
            set
            {
                _CaptionFont = value;
                _StripContainer.NeedRecalcSize = true;
                this.Invalidate();
            }
        }

        private void OnCaptionVisibleChanged()
        {
            _StripContainer.NeedRecalcSize = true;
            _StripContainer.OnCaptionVisibleChanged(_CaptionVisible);
            this.RecalcLayout();
        }

        internal eDotNetBarStyle EffectiveStyle
        {
            get
            {
                return _StripContainer.EffectiveStyle;
            }
        }
        /// <summary>
        /// Gets/Sets the visual style of the control.
        /// </summary>
        [Browsable(true), DevCoBrowsable(true), Category("Appearance"), Description("Specifies the visual style of the control."), DefaultValue(eDotNetBarStyle.Metro)]
        public eDotNetBarStyle Style
        {
            get
            {
                return _StripContainer.Style;
            }
            set
            {
                this.ColorScheme.Style = value;
                _StripContainer.Style = value;
                this.Invalidate();
                this.RecalcLayout();
            }
        }

        /// <summary>
        /// Returns collection of items on a bar.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
        public SubItemsCollection Items
        {
            get
            {
                return _StripContainer.TabsContainer.SubItems;
            }
        }

        /// <summary>
        /// Returns currently selected TabFormItem. TabFormItems are selected using the Checked property. Only a single
        /// TabFormItem can be Checked at any given time.
        /// </summary>
        [Browsable(false)]
        public TabFormItem SelectedTab
        {
            get
            {
                foreach (BaseItem item in this.Items)
                {
                    if (item is TabFormItem && ((TabFormItem)item).Checked)
                        return (TabFormItem)item;
                }
                return null;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WinApi.WindowsMessages.WM_NCHITTEST)
            {
                int borderSize = Dpi.Width6;
                // Get position being tested...
                int x = WinApi.LOWORD(m.LParam);
                int y = WinApi.HIWORD(m.LParam);
                Point p = PointToClient(new Point(x, y));
                if (IsGlassEnabled && this.CaptionVisible)
                {
                    Rectangle r = new Rectangle(this.Width - BarFunctions.CaptionButtonSize.Width * 3, 0, BarFunctions.CaptionButtonSize.Width * 3, BarFunctions.CaptionButtonSize.Height + 6);
                    if (r.Contains(p))
                    {
                        m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                        return;
                    }

                    r = new Rectangle(0, 0, this.Width, 4);
                    if (r.Contains(p))
                    {
                        m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                        return;
                    }

                    if (_CaptionBounds.Contains(p))
                    {
                        m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                        return;
                    }
                }
                else if (this.CaptionVisible && !this.IsMaximized)
                {
                    Rectangle r = new Rectangle(0, 0, this.Width, 4);
                    if (r.Contains(p))
                    {
                        m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                        return;
                    }
                }
                else if (!SystemCaptionItem.Bounds.Contains(p))
                {
                    BaseItem item = HitTest(p.X, p.Y);
                    if (item == null)
                    {
                        m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                        return;
                    }
                }
                else if (p.X <= borderSize || p.X >= this.Width - borderSize || p.Y <= borderSize || p.Y >= this.Height - borderSize)
                {
                    m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                    return;
                }
                if (BarFunctions.IsWindows7 && this.IsMaximized)
                {
                    if (this.CaptionBounds.Contains(p))
                    {
                        m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                        return;
                    }
                }
            }

            base.WndProc(ref m);
        }

        internal bool IsMaximized
        {
            get
            {
                Form f = this.FindForm();
                return (f != null && f.WindowState == FormWindowState.Maximized);
            }
        }

        /// <summary>
        /// Returns the color scheme used by control. Color scheme for Office2007 style will be retrived from the current renderer instead of
        /// local color scheme referenced by ColorScheme property.
        /// </summary>
        /// <returns>An instance of ColorScheme object.</returns>
        protected override ColorScheme GetColorScheme()
        {
            BaseRenderer r = GetRenderer();
            if (r is Office2007Renderer)
                return ((Office2007Renderer)r).ColorTable.LegacyColors;
            return base.GetColorScheme();
        }

        protected override void PaintControlBackground(ItemPaintArgs pa)
        {
            base.PaintControlBackground(pa);

            _CaptionBounds = Rectangle.Empty;
            _SystemCaptionItemBounds = Rectangle.Empty;

            Rendering.BaseRenderer render = GetRenderer();
            render.DrawTabFormStrip(new TabFormStripPainterArgs(this, pa.Graphics, pa));

#if TRIAL
            if (NativeFunctions.ColorExpAlt())
				{
					pa.Graphics.Clear(Color.White);
					TextDrawing.DrawString(pa.Graphics, "Your DotNetBar trial has expired :-(", this.Font, Color.FromArgb(128, Color.Black), this.ClientRectangle, eTextFormat.HorizontalCenter | eTextFormat.VerticalCenter);
				}
#endif
        }

        protected override ElementStyle GetBackgroundStyle()
        {
            if (this.BackgroundStyle.Custom)
                return base.GetBackgroundStyle();
            return _DefaultBackgroundStyle;
        }

        internal void InitDefaultStyles()
        {
            _DefaultBackgroundStyle = new ElementStyle();
        }

        internal ElementStyle InternalGetBackgroundStyle()
        {
            return this.GetBackgroundStyle();
        }

        internal Rectangle GetItemContainerBounds()
        {
            Rectangle r = base.GetItemContainerRectangle();
            if (_CaptionVisible)
            {
                r.Y += GetAbsoluteCaptionHeight();
            }
            else
                r.Y += TopNormalPadding;
            return r;
        }

        internal Rectangle GetCaptionContainerBounds()
        {
            Rectangle baseRect = base.GetItemContainerRectangle();
            if (this.IsGlassEnabled)
                baseRect.Y += 3;
            return new Rectangle(baseRect.X, baseRect.Y, baseRect.Width, GetAbsoluteCaptionHeight());
        }
        /// <summary>
        /// Returns effective caption height.
        /// </summary>
        /// <returns>Caption height.</returns>
        public int GetCaptionHeight()
        {
            if (_CaptionHeight == 0)
            {
                //if (StyleManager.Style == eStyle.OfficeMobile2014 || StyleManager.Style == eStyle.Office2016)
                    return Dpi.Height24;
                //else
                //    return Dpi.Height28;
            }
            else
            {
                return Dpi.Height(_CaptionHeight);
            }
        }

        internal int GetAbsoluteCaptionHeight()
        {
            return GetCaptionHeight();
        }

        internal int GetTotalCaptionHeight()
        {
            return GetCaptionHeight();
        }

        internal override bool IsGlassEnabled
        {
            get
            {
                return false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_StripContainer.NeedRecalcSize) this.RecalcSize();
            InitDefaultStyles();
            base.OnPaint(e);
        }

        protected override void RecalcSize()
        {
            _CaptionBounds = Rectangle.Empty;
            _SystemCaptionItemBounds = Rectangle.Empty;
            InitDefaultStyles();
            base.RecalcSize();
        }

        /// <summary>
        /// Returns automatically calculated height of the control given current content.
        /// </summary>
        /// <returns>Height in pixels.</returns>
        public override int GetAutoSizeHeight()
        {
            int i = base.GetAutoSizeHeight();
            if (!_CaptionVisible)
                i += TopNormalPadding;
            return i;
        }

        private int TopNormalPadding = 4;

        protected override bool OnMouseWheel(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
        {
            Rectangle r = this.DisplayRectangle;
            r.Location = this.PointToScreen(r.Location);

            TabFormControl rc = this.Parent as TabFormControl;

            if (rc != null && !rc.MouseWheelTabScrollEnabled) return false;

            Point mousePos = Control.MousePosition;

            bool parentActive = true;
            Form parentForm = this.FindForm();
            if (parentForm != null && !BarFunctions.IsFormActive(parentForm))
                parentActive = false;

            if (parentActive && r.Contains(mousePos) && !this.ShowKeyTips && this.Items.Count > 0)
            {
                IntPtr handle = NativeFunctions.WindowFromPoint(new NativeFunctions.POINT(mousePos));
                Control c = Control.FromChildHandle(handle);
                if (c == null)
                    c = Control.FromHandle(handle);
                if (c is TabFormStripControl || c is TabFormControl || c is TabFormPanel)
                {
                    TabFormItem selectedTab = this.SelectedTab;
                    int start = 0;
                    int end = this.Items.Count - 1;

                    int direction = 1;
                    if (wParam.ToInt64() > 0)
                    {
                        direction = -1;
                        end = 0;
                    }
                    if (selectedTab != null)
                    {
                        start = this.Items.IndexOf(selectedTab) + direction;
                        if (direction < 0 && start < 0) return false;

                        if (start == this.Items.Count)
                            start = 0;
                        else if (start < 0)
                            start = this.Items.Count - 1;
                    }

                    int index = start - direction;
                    do
                    {
                        index += direction;
                        if (index < 0 || index > this.Items.Count - 1) break;

                        if (this.Items[index] is TabFormItem && this.Items[index].Visible)
                        {
                            ((TabFormItem)this.Items[index]).Checked = true;
                            return true;
                        }
                    } while (index != end);
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the collection of items with the specified name.
        /// </summary>
        /// <param name="ItemName">Item name to look for.</param>
        /// <returns></returns>
        public override ArrayList GetItems(string ItemName)
        {
            ArrayList list = new ArrayList(15);
            BarFunctions.GetSubItemsByName(GetBaseItemContainer(), ItemName, list);

            TabFormControl rc = this.GetTabFormControl();

            if (rc != null && rc.GlobalContextMenuBar != null)
                BarFunctions.GetSubItemsByName(rc.GlobalContextMenuBar.ItemsContainer, ItemName, list);

            return list;
        }

        /// <summary>
        /// Returns the collection of items with the specified name and type.
        /// </summary>
        /// <param name="ItemName">Item name to look for.</param>
        /// <param name="itemType">Item type to look for.</param>
        /// <returns></returns>
        public override ArrayList GetItems(string ItemName, Type itemType)
        {
            ArrayList list = new ArrayList(15);
            BarFunctions.GetSubItemsByNameAndType(GetBaseItemContainer(), ItemName, list, itemType);

            TabFormControl rc = this.GetTabFormControl();
            if (rc != null && rc.GlobalContextMenuBar != null)
                BarFunctions.GetSubItemsByNameAndType(rc.GlobalContextMenuBar.ItemsContainer, ItemName, list, itemType);

            return list;
        }

        /// <summary>
        /// Returns the collection of items with the specified name and type.
        /// </summary>
        /// <param name="ItemName">Item name to look for.</param>
        /// <param name="itemType">Item type to look for.</param>
        /// <param name="useGlobalName">Indicates whether GlobalName property is used for searching.</param>
        /// <returns></returns>
        public override ArrayList GetItems(string ItemName, Type itemType, bool useGlobalName)
        {
            ArrayList list = new ArrayList(15);
            BarFunctions.GetSubItemsByNameAndType(GetBaseItemContainer(), ItemName, list, itemType, useGlobalName);

            TabFormControl rc = this.GetTabFormControl();

            if (rc != null && rc.GlobalContextMenuBar != null)
                BarFunctions.GetSubItemsByNameAndType(rc.GlobalContextMenuBar.ItemsContainer, ItemName, list, itemType, useGlobalName);

            return list;
        }

        /// <summary>
        /// Returns the first item that matches specified name.
        /// </summary>
        /// <param name="ItemName">Item name to look for.</param>
        /// <returns></returns>
        public override BaseItem GetItem(string ItemName)
        {
            BaseItem item = BarFunctions.GetSubItemByName(GetBaseItemContainer(), ItemName);
            if (item != null)
                return item;

            TabFormControl rc = this.GetTabFormControl();
            if (rc != null && rc.GlobalContextMenuBar != null)
                return BarFunctions.GetSubItemByName(rc.GlobalContextMenuBar.ItemsContainer, ItemName);

            return null;
        }
        #endregion

        #region Caption Container Support
        private bool _ShowIcon = true;
        /// <summary>
        /// Indicates whether Form.Icon is shown in top-left corner.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether Form.Icon is shown in top-left corner.")]
        public bool ShowIcon
        {
            get
            {
                return _ShowIcon;
            }
            set
            {
                if (_ShowIcon != value)
                {
                    _ShowIcon = value;
                    _StripContainer.NeedRecalcSize = true;
                    this.RecalcLayout();
                }
            }
        }

        /// <summary>
        /// Called when item on popup container is right-clicked.
        /// </summary>
        /// <param name="item">Instance of the item that is right-clicked.</param>
        protected override void OnPopupItemRightClick(BaseItem item)
        {
            //TabFormControl rc = this.GetTabFormControl();
            //if (rc != null)
            //    rc.ShowCustomizeContextMenu(item, false);
        }
        //private MetroTab GetTabFormControl()
        //{
        //    Control parent = this.Parent;
        //    while (parent != null && !(parent is MetroTab))
        //        parent = parent.Parent;
        //    if (parent is MetroTab)
        //        return parent as MetroTab;
        //    return null;
        //}
        protected override void OnMouseLeave(EventArgs e)
        {
            if (_TitleTextMarkup != null)
                _TitleTextMarkup.MouseLeave(this);
            base.OnMouseLeave(e);
        }

        internal bool MouseDownOnCaption
        {
            get
            {
                return _MouseDownOnCaption;
            }
        }

        private TabFormControl GetTabFormControl()
        {
            return this.Parent as TabFormControl;
        }

        internal void ShowSystemMenu(Point p)
        {
            Form form = this.FindForm();
            if (form is TabParentForm)
                ((TabParentForm)form).ShowSystemWindowMenu(p);
            else
            {
                const int TPM_RETURNCMD = 0x0100;
                byte[] bx = BitConverter.GetBytes(p.X);
                byte[] by = BitConverter.GetBytes(p.Y);
                byte[] blp = new byte[] { bx[0], bx[1], by[0], by[1] };
                int lParam = BitConverter.ToInt32(blp, 0);
                this.Capture = false;
                NativeFunctions.SendMessage(form.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.TrackPopupMenu(
                    NativeFunctions.GetSystemMenu(form.Handle, false), TPM_RETURNCMD, p.X, p.Y, 0, form.Handle, IntPtr.Zero), lParam);
            }
        }

        private bool _MouseDownOnCaption = false;
        private Point _MouseDownPoint = Point.Empty;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            _MouseDownOnCaption = false;
            _MouseDownPoint = e.Location;
            if (_CaptionVisible)
            {
                _MouseDownOnCaption = HitTestCaption(new Point(e.X, e.Y));
                if (e.Button == MouseButtons.Right && _MouseDownOnCaption)
                {
                    ShowSystemMenu(Control.MousePosition);
                    return;
                }

                if (_TitleTextMarkup != null)
                    _TitleTextMarkup.MouseDown(this, e);

                e = TranslateMouseEventArgs(e);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_CaptionVisible)
            {
                if (_TitleTextMarkup != null)
                    _TitleTextMarkup.MouseUp(this, e);

                e = TranslateMouseEventArgs(e);
            }
            _MouseDownOnCaption = false;
            base.OnMouseUp(e);
        }

        //internal BaseItem GetApplicationButton()
        //{
        //    if (!this.CaptionVisible)
        //        return null;
        //    BaseItem cont = this.CaptionContainerItem;

        //    if (this.EffectiveStyle == eDotNetBarStyle.Office2010 && this.Items.Count > 0 && this.Items[0] is MetroAppButton)
        //        return this.Items[0];

        //    if (cont.SubItems.Count > 0 && cont.SubItems[0] is MetroAppButton)
        //        return cont.SubItems[0];

        //    if (cont.SubItems.Count > 0 && cont.SubItems[0] is ButtonItem && ((ButtonItem)cont.SubItems[0]).HotTrackingStyle == eHotTrackingStyle.Image)
        //        return cont.SubItems[0];

        //    return null;
        //}
        /// <summary>
        /// Starts moving of the parent form action which happens when user attempts to drag the form caption.
        /// </summary>
        public void StartFormMove()
        {
            Form form = this.FindForm();
            if (form != null && form.WindowState == FormWindowState.Normal)
            {
                //PopupItem popup = GetApplicationButton() as PopupItem;
                //if (popup != null && popup.Expanded) popup.Expanded = false;

                const int HTCAPTION = 2;
                Point p = Control.MousePosition;
                byte[] bx = BitConverter.GetBytes(p.X);
                byte[] by = BitConverter.GetBytes(p.Y);
                byte[] blp = new byte[] { bx[0], bx[1], by[0], by[1] };
                int lParam = BitConverter.ToInt32(blp, 0);
                this.Capture = false;
                NativeFunctions.SendMessage(form.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_MOVE + HTCAPTION, lParam);
                _MouseDownOnCaption = false;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_CaptionVisible)
            {
                if (e.Button == MouseButtons.Left && (_MouseDownOnCaption && HitTestCaption(new Point(e.X, e.Y)) || HitTest(_MouseDownPoint.X, _MouseDownPoint.Y) == null))
                {
                    Form form = this.FindForm();
                    if (form != null && form.WindowState == FormWindowState.Normal)
                    {
                        StartFormMove();
                        return;
                    }
                }
                if (_TitleTextMarkup != null)
                    _TitleTextMarkup.MouseMove(this, e);
                e = TranslateMouseEventArgs(e);
            }
            base.OnMouseMove(e);
        }

        protected override void InternalOnClick(MouseButtons mb, Point mousePos)
        {
            if (_CaptionVisible)
            {
                MouseEventArgs e = new MouseEventArgs(mb, 0, mousePos.X, mousePos.Y, 0);
                e = TranslateMouseEventArgs(e);
                mousePos = new Point(e.X, e.Y);
            }

            base.InternalOnClick(mb, mousePos);
        }

        private MouseEventArgs TranslateMouseEventArgs(MouseEventArgs e)
        {
            if (e.Y <= 6)
            {
                Form form = this.FindForm();
                if (form != null && form.WindowState == FormWindowState.Maximized && form is RibbonForm)
                {
                    if (e.X <= 4)
                    {
                        //BaseItem sb = GetApplicationButton();
                        //if (sb != null)
                        //{
                        //    e = new MouseEventArgs(e.Button, e.Clicks, sb.LeftInternal + 1, sb.TopInternal + 1, e.Delta);
                        //}
                    }
                    else if (e.X >= this.Width - 6)
                        e = new MouseEventArgs(e.Button, e.Clicks, this.SystemCaptionItem.DisplayRectangle.Right - 4, this.SystemCaptionItem.DisplayRectangle.Top + 4, e.Delta);
                    else
                        e = new MouseEventArgs(e.Button, e.Clicks, e.X, this.CaptionContainerItem.TopInternal + 5, e.Delta);
                }
            }
            return e;
        }

        protected override void OnClick(EventArgs e)
        {
            if (_CaptionVisible)
            {
                if (_TitleTextMarkup != null)
                    _TitleTextMarkup.Click(this);
            }
            base.OnClick(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (_CaptionVisible)
            {
                // Check whether double click is on caption so window can be maximized/restored
                Point p = this.PointToClient(Control.MousePosition);
                if (HitTestCaption(p))
                {
                    Form form = this.FindForm();
                    if (form != null && form.MaximizeBox && (form.FormBorderStyle == FormBorderStyle.Sizable || form.FormBorderStyle == FormBorderStyle.SizableToolWindow))
                    {
                        if (form.WindowState == FormWindowState.Normal)
                        {
                            NativeFunctions.PostMessage(form.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_MAXIMIZE, 0);
                        }
                        else if (form.WindowState == FormWindowState.Maximized)
                        {
                            NativeFunctions.PostMessage(form.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_RESTORE, 0);
                        }
                    }
                }
            }
            base.OnDoubleClick(e);
        }

        /// <summary>
        /// Returns true if point is inside the caption area.
        /// </summary>
        /// <param name="p">Client point coordinates.</param>
        /// <returns>True if point is inside of caption area otherwise false.</returns>
        protected bool HitTestCaption(Point p)
        {
            return _CaptionBounds.Contains(p);
        }

        private void SystemCaptionClick(object sender, EventArgs e)
        {
            SystemCaptionItem sci = sender as SystemCaptionItem;
            Form frm = this.FindForm();

            if (frm == null)
                return;

            if (sci.LastButtonClick == sci.MouseDownButton)
            {
                if (sci.LastButtonClick == SystemButton.Minimize)
                {
                    NativeFunctions.PostMessage(frm.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_MINIMIZE, 0);
                }
                else if (sci.LastButtonClick == SystemButton.Maximize)
                {
                    NativeFunctions.PostMessage(frm.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_MAXIMIZE, 0);
                }
                else if (sci.LastButtonClick == SystemButton.Restore)
                {
                    NativeFunctions.PostMessage(frm.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_RESTORE, 0);
                }
                else if (sci.LastButtonClick == SystemButton.Close)
                {
                    NativeFunctions.PostMessage(frm.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_CLOSE, 0);
                }
                else if (sci.LastButtonClick == SystemButton.Help)
                {
                    NativeFunctions.PostMessage(frm.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_CONTEXTHELP, 0);
                }
            }
        }

        internal void CloseParentForm()
        {
            Form frm = this.FindForm();

            if (frm == null)
                return;

            NativeFunctions.PostMessage(frm.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_CLOSE, 0);
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Form frm = this.FindForm();
            if (frm == null)
                return;

            if (frm.WindowState == FormWindowState.Maximized || frm.WindowState == FormWindowState.Minimized)
                _StripContainer.SystemCaptionItem.RestoreEnabled = true;
            else
                _StripContainer.SystemCaptionItem.RestoreEnabled = false;
        }

        /// <summary>
        /// Gets the reference to the internal container item for the items displayed in control caption.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GenericItemContainer CaptionContainerItem
        {
            get { return _StripContainer.CaptionContainer; }
        }

        /// <summary>
        /// Gets the reference to the internal container for the ribbon tabs and other items.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SimpleItemContainer StripContainerItem
        {
            get { return _StripContainer.TabsContainer; }
        }

        internal TabFormStripContainerItem TabStripContainer
        {
            get
            {
                return _StripContainer;
            }
        }

        private Cursor _FloatDragCursor = null;
        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            if (e.Effect == DragDropEffects.None)
            {
                if (_FloatDragCursor == null) // Try to create drag cursor
                {

                }
                if (_FloatDragCursor != null)
                {
                    e.UseDefaultCursors = false;
                    Cursor.Current = _FloatDragCursor;
                }
            }
            base.OnGiveFeedback(e);
        }

        protected override void OnItemDragAndDropped(BaseItem dragItem)
        {
            base.OnItemDragAndDropped(dragItem);

            TabFormItem tab = dragItem as TabFormItem;
            if (tab != null && tab.Enabled && tab.Visible)
            {
                if (tab.Checked) tab.Checked = false;
                tab.Checked = true;
            }
        }

        //protected override void PaintKeyTips(Graphics g)
        //{
        //    if (!this.ShowKeyTips)
        //        return;

        //    KeyTipsRendererEventArgs e = new KeyTipsRendererEventArgs(g, Rectangle.Empty, "", GetKeyTipFont(), null);

        //    DevComponents.DotNetBar.Rendering.BaseRenderer renderer = GetRenderer();
        //    PaintContainerKeyTips(_StripContainer.TabsContainer, renderer, e);
        //    if (_CaptionVisible)
        //        PaintContainerKeyTips(_StripContainer.CaptionContainer, renderer, e);
        //}

        //protected override Rectangle GetKeyTipRectangle(Graphics g, BaseItem item, Font font, string keyTip)
        //{
        //    Rectangle r = base.GetKeyTipRectangle(g, item, font, keyTip);
        //    if (this.QuickToolbarItems.Contains(item))
        //        r.Y += 4;
        //    return r;
        //}
        #endregion

        #region Mdi Child System Item
        internal void ClearMDIChildSystemItems(bool bRecalcLayout)
        {
            if (_StripContainer.TabsContainer == null)
                return;
            bool recalc = false;
            try
            {
                if (this.Items.Contains("dotnetbarsysiconitem"))
                {
                    this.Items.Remove("dotnetbarsysiconitem");
                    recalc = true;
                }
                if (this.Items.Contains("dotnetbarsysmenuitem"))
                {
                    this.Items.Remove("dotnetbarsysmenuitem");
                    recalc = true;
                }
                if (bRecalcLayout && recalc)
                    this.RecalcLayout();
            }
            catch (Exception)
            {
            }
        }

        internal void ShowMDIChildSystemItems(System.Windows.Forms.Form objMdiChild, bool bRecalcLayout)
        {
            ClearMDIChildSystemItems(bRecalcLayout);

            if (objMdiChild == null)
                return;

            MDISystemItem mdi = new MDISystemItem("dotnetbarsysmenuitem");
            if (!objMdiChild.ControlBox)
                mdi.CloseEnabled = false;
            if (!objMdiChild.MinimizeBox)
                mdi.MinimizeEnabled = false;
            if (!objMdiChild.MaximizeBox)
            {
                mdi.RestoreEnabled = false;
            }
            mdi.ItemAlignment = eItemAlignment.Far;
            mdi.Click += new System.EventHandler(this.MDISysItemClick);

            this.Items.Add(mdi);

            if (bRecalcLayout)
                this.RecalcLayout();
        }

        private void MDISysItemClick(object sender, System.EventArgs e)
        {
            MDISystemItem mdi = sender as MDISystemItem;
            Form frm = this.FindForm();
            if (frm != null)
                frm = frm.ActiveMdiChild;
            if (frm == null)
            {
                ClearMDIChildSystemItems(true);
                return;
            }
            if (mdi.LastButtonClick == SystemButton.Minimize)
            {
                NativeFunctions.PostMessage(frm.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_MINIMIZE, 0);
            }
            else if (mdi.LastButtonClick == SystemButton.Restore)
            {
                NativeFunctions.PostMessage(frm.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_RESTORE, 0);
            }
            else if (mdi.LastButtonClick == SystemButton.Close)
            {
                NativeFunctions.PostMessage(frm.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_CLOSE, 0);
            }
            else if (mdi.LastButtonClick == SystemButton.NextWindow)
            {
                NativeFunctions.PostMessage(frm.Handle, NativeFunctions.WM_SYSCOMMAND, NativeFunctions.SC_NEXTWINDOW, 0);
            }
        }

        private eTabFormStripControlDock _TabAlignment = eTabFormStripControlDock.Top;
        /// <summary>
        /// Indiciates the appearance of the tab form items rendered on the strip
        /// </summary>
        [DefaultValue(eTabFormStripControlDock.Top), Category("Appearance"), Description("Indiciates the appearance of the tab form items rendered on the strip")]
        public eTabFormStripControlDock TabAlignment
        {
            get { return _TabAlignment; }
            set
            {
                _TabAlignment = value;
            }
        }

        private bool _IsTabDragEnabled = true;

        /// <summary>
        /// Indicates whether end-user tab reordering is enabled, default value is true.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether end-user tab reordering is enabled.")]
        public bool IsTabDragEnabled
        {
            get { return _IsTabDragEnabled; }
            set { _IsTabDragEnabled = value; }
        }

        private bool _TabDetachEnabled = true;
        /// <summary>
        /// Indicates whether user can detach the tabs into the new forms using drag and drop. Default value is true.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether user can detach the tabs into the new forms using drag and drop. Default value is true.")]
        public bool TabDetachEnabled
        {
            get { return _TabDetachEnabled; }
            set { _TabDetachEnabled = value; }
        }
        #endregion

        internal void InvokeMouseDown(MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        internal void InvokeMouseMove(MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        internal void InvokeMouseUp(MouseEventArgs e)
        {
            OnMouseUp(e);
        }

        internal void InvokeClick(EventArgs e)
        {
            OnClick(e);
        }
        /// <summary>
        /// Closes specified tab.
        /// </summary>
        /// <param name="tab">Tab to close</param>
        /// <param name="source">Source of the event</param>
        public void CloseTab(TabFormItem tab, eEventSource source)
        {
            TabFormTabCloseEventArgs e = new TabFormTabCloseEventArgs(tab, source);
            OnTabClosing(e);
            if (e.Cancel) return;

            // Close the tab
            this.Items.Remove(tab);
            Control c = tab.Panel;
            if (c != null)
            {
                if (c.Parent != null)
                    c.Parent.Controls.Remove(c);
                c.Dispose();
            }
            tab.Dispose();

            OnTabClosed(e);
        }

        protected override void OnItemRemoved(BaseItem item, ItemRemovedEventArgs e)
        {
            TabFormItem tab = item as TabFormItem;
            if (tab != null && tab.Checked && this.Items.Count > 0)
            {
                bool selected = false;
                int count = this.Items.Count;
                int index = Math.Min(Math.Max(0, e.ItemIndex), count - 1);
                for (int i = index; i < count; i++)
                {
                    TabFormItem t1 = this.Items[i] as TabFormItem;
                    if (t1 != null && t1.Enabled && t1.Visible)
                    {
                        t1.Checked = true;
                        selected = true;
                        break;
                    }
                }
                if (!selected)
                {
                    for (int i = index; i >= 0; i--)
                    {
                        TabFormItem t1 = this.Items[i] as TabFormItem;
                        if (t1 != null && t1.Enabled && t1.Visible)
                        {
                            t1.Checked = true;
                            selected = true;
                            break;
                        }
                    }
                }
            }
            base.OnItemRemoved(item, e);
        }

        protected override void OnItemAdded(BaseItem item, EventArgs e)
        {
            TabFormItem tab = item as TabFormItem;
            if (tab != null && this.SelectedTab == null && tab.Visible && tab.Enabled)
                tab.Checked = true;

            base.OnItemAdded(item, e);
        }

        /// <summary>
        /// Occurs before tab is closed and it allows canceling of the event.
        /// </summary>
        [Description("Occurs before tab is closed and it allows canceling of the event.")]
        public event TabFormTabCloseEventHandler TabClosing;

        /// <summary>
        /// Raises RemovingToken event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnTabClosing(TabFormTabCloseEventArgs e)
        {
            TabFormTabCloseEventHandler handler = TabClosing;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Occurs after tab is closed.
        /// </summary>
        [Description("Occurs after tab is closed.")]
        public event TabFormTabCloseEventHandler TabClosed;

        /// <summary>
        /// Raises RemovingToken event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnTabClosed(TabFormTabCloseEventArgs e)
        {
            TabFormTabCloseEventHandler handler = TabClosed;
            if (handler != null)
                handler(this, e);
        }

        private Form _OutlineDragForm = null;
        protected override void MouseDragOver(int x, int y, DragEventArgs dragArgs)
        {
            if (!DragInProgress && !ExternalDragInProgress)
                return;
            BaseItem dragItem = DragItem;
            if (!(dragItem is TabFormItem) || UseNativeDragDrop || !_TabDetachEnabled)
            {
                base.MouseDragOver(x, y, dragArgs);
                return;
            }

            if (ExternalDragInProgress && dragArgs != null)
                dragItem = dragArgs.Data.GetData(typeof(ButtonItem)) as BaseItem;

            if (DesignTimeProvider != null)
            {
                DesignTimeProvider.DrawReversibleMarker(InsertPosition, InsertBefore);
                DesignTimeProvider = null;
            }

            if (ExternalDragInProgress && dragItem == null)
                return;

            Point pScreen = this.PointToScreen(new Point(x, y));
            BaseItem baseItemContainer = GetBaseItemContainer();
            InsertPosition pos = ((IDesignTimeProvider)baseItemContainer).GetInsertPosition(pScreen, dragItem);

            if (pos == null || pos.TargetProvider == null)
            {
                Rectangle rScreen = new Rectangle(this.PointToScreen(Point.Empty), this.Size);
                if (!rScreen.Contains(pScreen))
                {
                    IntPtr windowAtHandle = NativeFunctions.WindowFromPoint(new NativeFunctions.POINT(pScreen));
                    if (windowAtHandle != IntPtr.Zero)
                    {
                        //Control cat = Control.FromChildHandle(windowAtHandle);
                        //if(cat!=null)
                        //    Console.WriteLine("{0} - {1}", cat.Name,cat);
                        TabFormStripControl stripAt = Control.FromChildHandle(windowAtHandle) as TabFormStripControl;
                        if (stripAt != null)
                        {
                            BaseItem cont = stripAt.GetBaseItemContainer();
                            pos = ((IDesignTimeProvider)cont).GetInsertPosition(pScreen, dragItem);
                        }
                    }
                }
            }

            if (pos != null && pos.TargetProvider != null)
            {
                DisposeOutlineDragForm();
                pos.TargetProvider.DrawReversibleMarker(pos.Position, pos.Before);
                InsertPosition = pos.Position;
                InsertBefore = pos.Before;
                DesignTimeProvider = pos.TargetProvider;
                Cursor.Current = Cursors.Hand;
            }
            else
            {
                Cursor.Current = Cursors.Hand;
                if (_OutlineDragForm == null)
                {
                    _OutlineDragForm = BarFunctions.CreateTransparentOutlineForm();
                    _OutlineDragForm.Name = "TabFormItemDragOutline";
                }
                Rectangle r = new Rectangle(pScreen.X - 200 / 2, pScreen.Y - 22, 200, 150);
                NativeFunctions.SetWindowPos(_OutlineDragForm.Handle,
                    new IntPtr(NativeFunctions.HWND_TOP), r.X, r.Y, r.Width, r.Height, NativeFunctions.SWP_SHOWWINDOW | NativeFunctions.SWP_NOACTIVATE);
            }
            OnMouseDragOverProcessed(x, y, dragArgs);
        }

        protected override void MouseDragDrop(int x, int y, DragEventArgs dragArgs)
        {
            if (!DragInProgress && !ExternalDragInProgress)
                return;
            DisposeOutlineDragForm();
            TabFormControl currentTabControl = GetTabFormControl();
            if (x == -1 && y == -1 || !(DragItem is TabFormItem) || currentTabControl == null || !_TabDetachEnabled || DesignTimeProvider == _StripContainer.TabsContainer)
            {
                base.MouseDragDrop(x, y, dragArgs);
                return;
            }

            TabFormItem dragItem = (TabFormItem)DragItem;
            if (ExternalDragInProgress)
                dragItem = dragArgs.Data.GetData(typeof(ButtonItem)) as TabFormItem;

            if (dragItem != null)
                dragItem.InternalMouseLeave();

            if (DesignTimeProvider != null)
                DesignTimeProvider.DrawReversibleMarker(InsertPosition, InsertBefore);

            if (DesignTimeProvider != null && DesignTimeProvider != _StripContainer.TabsContainer)
            {
                // Moving to different form or tab strip
                int index = currentTabControl.Items.IndexOf(dragItem);
                currentTabControl.Items.Remove(dragItem);
                if (dragItem.Checked)
                {
                    // Select some other tab
                    if (index < currentTabControl.Items.Count)
                    {
                        for (int i = index; i >=0; i--)
                        {
                            if (currentTabControl.Items[i] is TabFormItem && currentTabControl.Items[i].Enabled &&
                                currentTabControl.Items[i].Visible)
                                ((TabFormItem) currentTabControl.Items[i]).Checked = true;
                        }
                    }
                }
                if (dragItem.Panel != null && dragItem.Panel.Parent != null)
                    dragItem.Panel.Parent.Controls.Remove(dragItem.Panel);

                Form formToClose = null;
                if (currentTabControl.TabsCount == 0 && currentTabControl.IsAutoCreated)
                    formToClose = currentTabControl.FindForm();
                else
                    currentTabControl.RecalcLayout();

                DesignTimeProvider.InsertItemAt(dragItem, InsertPosition, InsertBefore);
                if (dragItem.Panel != null && DesignTimeProvider is BaseItem)
                {
                    TabFormStripControl targetStrip =
                        ((BaseItem)DesignTimeProvider).ContainerControl as TabFormStripControl;
                    if (targetStrip != null && targetStrip.Parent is TabFormControl)
                        targetStrip.Parent.Controls.Add(dragItem.Panel);
                }
                OnItemDragAndDropped(dragItem);
                ((IOwner)this).InvokeUserCustomize(dragItem, new EventArgs());

                if (formToClose != null)
                {
                    formToClose.Close();
                }
            }
            else
            {
                if (currentTabControl.TabsCount == 1) // There is nothing to detach here just moved the form...
                {
                    Form form = currentTabControl.FindForm();
                    if (form != null)
                        form.Location = new Point(Control.MousePosition.X - form.Width / 2,
                            Control.MousePosition.Y - Dpi.Height16);
                }
                else
                {
                    TabFormItemDetachEventArgs detachArgs = new TabFormItemDetachEventArgs(dragItem);
                    OnBeforeTabFormItemDetach(detachArgs);
                    if (!detachArgs.Cancel)
                    {
                        TabParentForm form = null;
                        TabFormControl tabControl = null;
                        if (detachArgs.TabForm != null && detachArgs.TabControl != null)
                        {
                            form = detachArgs.TabForm;
                            tabControl = detachArgs.TabControl;
                        }
                        else
                        {
                            form = new TabParentForm();
                            form.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
                            form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
                            form.Text = dragItem.Text;
                            tabControl = new TabFormControl();
                            tabControl.IsAutoCreated = true;
                            tabControl.Dock = DockStyle.Fill;
                            form.Controls.Add(tabControl);
                        }

                        int index = currentTabControl.Items.IndexOf(dragItem);
                        currentTabControl.Items.Remove(dragItem);
                        if (dragItem.Checked)
                        {
                            // Select some other tab
                            if (index < currentTabControl.Items.Count)
                            {
                                for (int i = index; i >= 0; i--)
                                {
                                    if (currentTabControl.Items[i] is TabFormItem && currentTabControl.Items[i].Enabled &&
                                        currentTabControl.Items[i].Visible)
                                        ((TabFormItem)currentTabControl.Items[i]).Checked = true;
                                }
                            }
                        }
                        if (dragItem.Panel != null && dragItem.Panel.Parent != null)
                            dragItem.Panel.Parent.Controls.Remove(dragItem.Panel);

                        // Its key to add panel to the new control first otherwise new form could not be moved
                        if (dragItem.Panel != null)
                        {
                            tabControl.Controls.Add(dragItem.Panel);
                            dragItem.Panel.SendToBack();
                        }
                        tabControl.Items.Add(dragItem);
                        tabControl.SelectedTab = dragItem;
                        tabControl.RecalcLayout();
                        form.StartPosition = FormStartPosition.Manual;
                        form.Location = new Point(Control.MousePosition.X - 800 / 2,
                            Control.MousePosition.Y - Dpi.Height16);
                        form.Size = new Size(800, 600);

                        detachArgs.TabForm = form;
                        detachArgs.TabControl = tabControl;
                        OnTabFormItemDetach(detachArgs);
                        if (MultiFormAppContext.Current!=null)
                            MultiFormAppContext.Current.RegisterOpenForm(form);
                        form.Show();

                        ((IOwner)this).InvokeUserCustomize(dragItem, EventArgs.Empty);
                    }
                }
            }

            DesignTimeProvider = null;
            DragInProgress = false;
            ExternalDragInProgress = false;
            Cursor.Current = Cursors.Default;
            this.Capture = false;
            if (dragItem != null)
            {
                dragItem._IgnoreClick = true;
                dragItem.DragStartPoint = Point.Empty;
            }
            GetBaseItemContainer().InternalMouseUp(new MouseEventArgs(MouseButtons.Left, 0, x, y, 0));
            if (dragItem != null)
                dragItem._IgnoreClick = false;

            DragItem = null;

            //OnUserCustomize(EventArgs.Empty);
        }

        /// <summary>
        /// Returns current number of tabs visile and hiden on the tab strip.
        /// </summary>
        [Browsable(false)]
        public int TabsCount
        {
            get
            {
                int count = 0;
                foreach (BaseItem item in Items)
                {
                    if (item is TabFormItem)
                        count++;
                }
                return count;
            }
        }

        /// <summary>
        /// Indicates whether new tab item which allows creation of new tab when clicked is visible. When visible you need to handle CreateNewTab event and create your new tab in event handler.
        /// </summary>
        [DefaultValue(false), Category("Appearance"), Description("Indicates whether new tab item which allows creation of new tab when clicked is visible. When visible you need to handle CreateNewTab event and create your new tab in event handler.")]
        public bool NewTabItemVisible
        {
            get { return _StripContainer.NewTabItemVisible; }
            set { _StripContainer.NewTabItemVisible = value; }
        }

        /// <summary>
        /// Occurs before TabFormItem is detached and gives you opportunity to cancel the action or provide your own new TabParentForm and TabFormControl.
        /// </summary>
        [Description("Occurs before TabFormItem is detached and gives you opportunity to cancel the action or provide your own new TabParentForm and TabFormControl.")]
        public event TabFormItemDetachEventHandler BeforeTabFormItemDetach;

        /// <summary>
        /// Raises BeforeTabFormItemDetach event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnBeforeTabFormItemDetach(TabFormItemDetachEventArgs e)
        {
            TabFormItemDetachEventHandler handler = BeforeTabFormItemDetach;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Occurs after TabFormItem has been detached and is added to the new form and tab control.
        /// </summary>
        [Description("Occurs after TabFormItem has been detached and is added to the new form and tab control.")]
        public event TabFormItemDetachEventHandler TabFormItemDetach;
        /// <summary>
        /// Raises TabFormItemDetach event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnTabFormItemDetach(TabFormItemDetachEventArgs e)
        {
            TabFormItemDetachEventHandler handler = TabFormItemDetach;
            if (handler != null)
                handler(this, e);
        }

        private void DisposeOutlineDragForm()
        {
            if (_OutlineDragForm != null)
            {
                _OutlineDragForm.Close();
                _OutlineDragForm.Dispose();
                _OutlineDragForm = null;
            }
        }
    }

    /// <summary>
    /// Defines delegate for the TabFormCloseTab event.
    /// </summary>
    public delegate void TabFormTabCloseEventHandler(object sender, TabFormTabCloseEventArgs e);

    /// <summary>
    /// Defines delegate for the TabFormCloseTab event.
    /// </summary>
    public class TabFormTabCloseEventArgs : EventArgs
    {
        /// <summary>
        /// Allows to cancel the closing of the tab.
        /// </summary>
        public bool Cancel = false;
        /// <summary>
        /// Reference to tab being closed.
        /// </summary>
        public readonly TabFormItem Tab;
        /// <summary>
        /// Source of the event.
        /// </summary>
        public readonly eEventSource Source;

        public TabFormTabCloseEventArgs(TabFormItem tab, eEventSource source)
        {
            Tab = tab;
            Source = source;
        }
    }

    /// <summary>
    /// Defines delegate for the TabFormItemDetach event.
    /// </summary>
    public delegate void TabFormItemDetachEventHandler(object sender, TabFormItemDetachEventArgs e);

    /// <summary>
    /// Defines delegate for the TabFormItemDetach event.
    /// </summary>
    public class TabFormItemDetachEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the reference to the form which will host newly detached TabFormItem and its panel, an instance of TabParentForm. You can provide your own instance of TabParentForm and TabControl to use instead of controls creating them. You must provide both TabParentForm and TabControl. Provide these in BeforeTabFormItemDetach event.
        /// </summary>
        public TabParentForm TabForm = null;
        /// <summary>
        /// Gets or sets the reference to TabFormControl which will receive TabFormItem being dragged. If you provide your own TabParentForm and TabControl you must provide both. Provide these in BeforeTabFormItemDetach event.
        /// </summary>
        public TabFormControl TabControl = null;
        /// <summary>
        /// Gets reference to the TabFormItem being detached.
        /// </summary>
        public readonly TabFormItem TabFormItem;
        /// <summary>
        /// Enables canceling of TabFormItem detachment from its parent.
        /// </summary>
        public bool Cancel = false;
        public TabFormItemDetachEventArgs(TabFormItem tab)
        {
            this.TabFormItem = tab;
        }
    }

}
