using DevComponents.DotNetBar.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.Metro;
using DevComponents.DotNetBar.Primitives;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Represents Tabbed Forms control for creating tabbed user interface as replacement for MDI child forms.
    /// </summary>
    [ToolboxBitmap(typeof(TabFormControl), "TabFormControl.ico"), ToolboxItem(true), Designer("DevComponents.DotNetBar.Design.TabFormControlDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf"), System.Runtime.InteropServices.ComVisible(false)]
    public class TabFormControl : System.Windows.Forms.ContainerControl
    {
        #region Events

        /// <summary>
        /// Occurs 
        /// </summary>
        [Description("Occurs ")]
        public event PaintTabFormItemEventHandler PaintTabFormItem;

        /// <summary>
        /// Raises RemovingToken event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnPaintTabFormItem(PaintTabFormItemEventArgs e)
        {
            PaintTabFormItemEventHandler handler = PaintTabFormItem;
            if (handler != null)
                handler(this, e);
        }
        /// <summary>
        /// Occurs when DotNetBar is looking for translated text for one of the internal text that are
        /// displayed on menus, toolbars and customize forms. You need to set Handled=true if you want
        /// your custom text to be used instead of the built-in system value.
        /// </summary>
        public event DotNetBarManager.LocalizeStringEventHandler LocalizeString;

        /// <summary>
        /// Occurs when Item on control is clicked.
        /// </summary>
        [Description("Occurs when Item on control is clicked.")]
        public event EventHandler ItemClick;

        /// <summary>
        ///     Occurs after selected tab has changed. You can use
        ///     <see cref="SelectedTab">TabFormControl.SelectedTab</see>
        ///     property to get reference to newly selected tab.
        /// </summary>
        public event EventHandler SelectedTabChanged;

        /// <summary>
        /// Occurs when text markup link from TitleText markup is clicked. Markup links can be created using "a" tag, for example:
        /// <a name="MyLink">Markup link</a>
        /// </summary>
        [Description("Occurs when text markup link from TitleText markup is clicked.")]
        public event MarkupLinkClickEventHandler TitleTextMarkupLinkClick;

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
        #endregion

        #region Private Variables and Constructor
        private TabFormStripControl _TabStrip = null;
        private bool m_AutoSize = false;
        private ShadowPaintInfo m_ShadowPaintInfo = null;
        private bool _UseCustomizeDialog = true;
        //private bool m_EnableQatPlacement = true;

        private int DefaultBottomDockPadding = 0;
        private DevComponents.DotNetBar.Ribbon.SubItemsQatCollection m_QatSubItemsCollection = null;
        private RibbonLocalization m_SystemText = new RibbonLocalization();
        private bool m_MenuTabsEnabled = true;
        private ContextMenuBar m_GlobalContextMenuBar = null;

        public TabFormControl()
        {
            // This forces the initialization out of paint loop which speeds up how fast components show up
            BaseRenderer renderer = DevComponents.DotNetBar.Rendering.GlobalManager.Renderer;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                  | ControlStyles.ResizeRedraw
                  | DisplayHelp.DoubleBufferFlag
                  | ControlStyles.UserPaint
                  | ControlStyles.Opaque
                  , true);
            _TabStrip = new TabFormStripControl();
            _TabStrip.Dock = DockStyle.Top;
            _TabStrip.Height = 40;
            _TabStrip.ItemAdded += new System.EventHandler(TabStripItemAdded);
            _TabStrip.LocalizeString += new DotNetBarManager.LocalizeStringEventHandler(TabStripLocalizeString);
            _TabStrip.ItemClick += new System.EventHandler(TabStripItemClick);
            _TabStrip.ButtonCheckedChanged += new EventHandler(TabStripButtonCheckedChanged);
            _TabStrip.TitleTextMarkupLinkClick += new MarkupLinkClickEventHandler(TabStripTitleTextMarkupLinkClick);
            _TabStrip.BeforeTabFormItemDetach += TabStripBeforeTabFormItemDetach;
            _TabStrip.TabFormItemDetach += TabStripTabFormItemDetach;
            this.Controls.Add(_TabStrip);
            this.TabStop = false;
            this.DockPadding.Bottom = DefaultBottomDockPadding;
            StyleManager.Register(this);
        }

        protected override void Dispose(bool disposing)
        {
            StyleManager.Unregister(this);
            base.Dispose(disposing);
        }
        #endregion

        #region Internal Implementation
        /// <summary>
        /// Called by StyleManager to notify control that style on manager has changed and that control should refresh its appearance if
        /// its style is controlled by StyleManager.
        /// </summary>
        /// <param name="newStyle">New active style.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void StyleManagerStyleChanged(eDotNetBarStyle newStyle)
        {
            this.DockPadding.Left = 0;
        }

        internal bool IsDesignMode
        {
            get { return this.DesignMode; }
        }

        internal bool InternalPaintTabFormItem(TabFormItem tab, ItemPaintArgs itemPaintArgs)
        {
            PaintTabFormItemEventArgs p = new PaintTabFormItemEventArgs(tab, itemPaintArgs.Graphics);
            OnPaintTabFormItem(p);
            return p.PaintDefault;
        }
        protected virtual void OnSelectedTabChanged(EventArgs e)
        {
            if (SelectedTabChanged != null)
                SelectedTabChanged(this, e);
        }

        void TabStripButtonCheckedChanged(object sender, EventArgs e)
        {
            if (sender is TabFormItem && ((TabFormItem)sender).Checked)
            {
                OnSelectedTabChanged(new EventArgs());
            }
        }

        private void TabStripItemClick(object sender, System.EventArgs e)
        {
            if (ItemClick != null)
                ItemClick(sender, e);
        }

        private void TabStripLocalizeString(object sender, LocalizeEventArgs e)
        {
            if (LocalizeString != null)
                LocalizeString(this, e);
        }

        /// <summary>
        /// Gets or sets whether anti-alias smoothing is used while painting. Default value is false.
        /// </summary>
        [DefaultValue(true), Browsable(true), Category("Appearance"), Description("Gets or sets whether anti-aliasing is used while painting.")]
        public bool AntiAlias
        {
            get { return _TabStrip.AntiAlias; }
            set
            {
                _TabStrip.AntiAlias = value;
                this.Invalidate();
            }
        }

        internal DevComponents.DotNetBar.Rendering.Office2007ColorTable GetOffice2007ColorTable()
        {
            DevComponents.DotNetBar.Rendering.Office2007Renderer r = DevComponents.DotNetBar.Rendering.GlobalManager.Renderer as DevComponents.DotNetBar.Rendering.Office2007Renderer;
            if (r != null)
                return r.ColorTable;
            return new DevComponents.DotNetBar.Rendering.Office2007ColorTable();
        }

        /// <summary>
        /// Performs the setup of the TabFormPanel with the current style of the TabFormControl Control.
        /// </summary>
        /// <param name="panel">Panel to apply style changes to.</param>
        public void SetTabPanelStyle(TabFormPanel panel)
        {
            if (this.DesignMode)
            {
                TypeDescriptor.GetProperties(panel.DockPadding)["Left"].SetValue(panel.DockPadding, 3);
                TypeDescriptor.GetProperties(panel.DockPadding)["Right"].SetValue(panel.DockPadding, 3);
                TypeDescriptor.GetProperties(panel.DockPadding)["Bottom"].SetValue(panel.DockPadding, 3);
            }
            else
            {
                panel.DockPadding.Left = 3;
                panel.DockPadding.Right = 3;
                panel.DockPadding.Bottom = 3;
            }
            panel.Style.Class = ElementStyleClassKeys.TabFormPanelKey;
        }

        /// <summary>
        /// Creates new Tab at specified position, creates new associated panel and adds them to the control.
        /// </summary>
        /// <param name="text">Specifies the text displayed on the tab.</param>
        /// <param name="name">Specifies the name of the tab</param>
        /// <param name="insertPosition">Specifies the position of the new tab inside of Items collection.</param>
        /// <returns>New instance of the TabFormItem that was created.</returns>
        public TabFormItem CreateTab(string text, string name, int insertPosition)
        {
            TabFormItem item = new TabFormItem();
            item.Text = text;
            item.Name = name;

            TabFormPanel panel = new TabFormPanel();
            panel.Dock = DockStyle.Fill;
            SetTabPanelStyle(panel);
            this.Controls.Add(panel);
            panel.SendToBack();

            item.Panel = panel;
            if (insertPosition < 0)
            {
                insertPosition = this.Items.Count;
                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.Items[i].ItemAlignment == eItemAlignment.Far)
                    {
                        insertPosition = i;
                        break;
                    }
                }
                if (insertPosition >= this.Items.Count)
                    this.Items.Add(item);
                else
                    this.Items.Insert(insertPosition, item);
            }
            else if (insertPosition > this.Items.Count - 1)
                this.Items.Add(item);
            else
                this.Items.Insert(insertPosition, item);

            return item;
        }

        /// <summary>
        /// Creates new Tab and associated panel and adds them to the control.
        /// </summary>
        /// <param name="text">Specifies the text displayed on the tab.</param>
        /// <param name="name">Specifies the name of the tab</param>
        /// <returns>New instance of the TabFormItem that was created.</returns>
        public TabFormItem CreateTab(string text, string name)
        {
            return CreateTab(text, name, -1);
        }

        /// <summary>
        /// Recalculates layout of the control and applies any changes made to the size or position of the items contained.
        /// </summary>
        public void RecalcLayout()
        {
            _TabStrip.RecalcLayout();
        }

        protected override void OnHandleCreated(System.EventArgs e)
        {
            base.OnHandleCreated(e);
            foreach (Control c in this.Controls)
            {
                IntPtr h = c.Handle;
                if (c is TabFormPanel)
                {
                    foreach (Control r in c.Controls)
                        h = r.Handle;
                }
            }
            this.RecalcLayout();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (this.DesignMode)
                return;
            TabFormPanel panel = e.Control as TabFormPanel;
            if (panel == null)
                return;
            if (panel.TabFormItem != null)
            {
                if (this.Items.Contains(panel.TabFormItem) && this.SelectedTab == panel.TabFormItem)
                {
                    panel.Visible = true;
                    panel.BringToFront();
                }
                else
                    panel.Visible = false;
            }
            else
                panel.Visible = false;
        }

        private void TabStripItemAdded(object sender, System.EventArgs e)
        {
            if (this.DesignMode)
                return;

            if (sender is TabFormItem)
            {
                TabFormItem tab = sender as TabFormItem;
                if (tab.Panel != null)
                {
                    if (this.Controls.Contains(tab.Panel) && tab.Checked)
                    {
                        tab.Panel.Visible = true;
                        tab.Panel.BringToFront();
                    }
                    else
                        tab.Panel.Visible = false;
                }
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            TabFormPanel panel = e.Control as TabFormPanel;
            if (panel == null)
                return;
            if (panel.TabFormItem != null)
            {
                if (this.Items.Contains(panel.TabFormItem))
                    this.Items.Remove(panel.TabFormItem);
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetDesignMode()
        {
            _TabStrip.SetDesignMode(true);
        }

        private ElementStyle GetBackgroundStyle()
        {
            return _TabStrip.InternalGetBackgroundStyle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ElementStyle style = GetBackgroundStyle();
            if (style.BackColor.A < 255 && !style.BackColor.IsEmpty ||
                this.BackColor == Color.Transparent || this.BackgroundImage != null)
            {
                base.OnPaintBackground(e);
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(this.BackColor))
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            ElementStyleDisplayInfo info = new ElementStyleDisplayInfo(style, e.Graphics, this.ClientRectangle);
            ElementStyleDisplay.PaintBackground(info);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WinApi.WindowsMessages.WM_NCHITTEST && !this.DesignMode)
            {
                // Get position being tested...
                int x = WinApi.LOWORD(m.LParam);
                int y = WinApi.HIWORD(m.LParam);
                Point p = PointToClient(new Point(x, y));

                if (this.CaptionVisible && _TabStrip != null && !_TabStrip.IsMaximized)
                {
                    TabParentForm form = this.FindForm() as TabParentForm;
                    if (form == null || form.Sizable)
                    {
                        int formBorderSize = 4;
                        Rectangle r = new Rectangle(0, 0, this.Width, formBorderSize);
                        if (r.Contains(p)) // Top side form resizing
                        {
                            m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                            return;
                        }
                        r = new Rectangle(0, 0, formBorderSize, this.Height); // Left side form resizing
                        if (r.Contains(p))
                        {
                            m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                            return;
                        }
                        r = new Rectangle(this.Width - formBorderSize, 0, formBorderSize, this.Height); // Right side form resizing
                        if (r.Contains(p))
                        {
                            m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                            return;
                        }
                    }
                }
                else if (_TabStrip != null)
                {
                    Point pts = _TabStrip.PointToClient(new Point(x, y));
                    if (_TabStrip.HitTest(pts.X, pts.Y) == null)
                    {
                        m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                        return;
                    }
                }
                //if (BarFunctions.IsWindows7 && _TabStrip != null && _TabStrip.IsMaximized)
                //{
                //    Rectangle r = _TabStrip.CaptionBounds;
                //    if (r.Contains(p))
                //    {
                //        m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                //        return;
                //    }
                //}
                //// System Icon
                //if ((p.X < 28 && this.RightToLeft == RightToLeft.No || p.X > this.Width - 28 && this.RightToLeft == RightToLeft.Yes) && p.Y < 28)
                //{
                //    m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                //    return;
                //}
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Returns current number of tabs visible and hidden on the tab strip.
        /// </summary>
        [Browsable(false)]
        public int TabsCount
        {
            get { return _TabStrip.TabsCount; }
        }

        private void TabStripTabFormItemDetach(object sender, TabFormItemDetachEventArgs e)
        {
            OnTabFormItemDetach(e);
        }

        private void TabStripBeforeTabFormItemDetach(object sender, TabFormItemDetachEventArgs e)
        {
            OnBeforeTabFormItemDetach(e);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Indicates whether Form.Icon is shown in top-left corner.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether Form.Icon is shown in top-left corner.")]
        public bool ShowIcon
        {
            get
            {
                return _TabStrip.ShowIcon;
            }
            set
            {
                _TabStrip.ShowIcon = value;
            }
        }

        /// <summary>
        /// Gets or sets the rich text displayed on title bar instead of the Form.Text property. This property supports text-markup.
        /// You can use <font color="SysCaptionTextExtra"> markup to instruct the markup renderer to use Office 2007 system caption extra text color which
        /// changes depending on the currently selected color table. Note that when using this property you should manage also the Form.Text property since
        /// that is the text that will be displayed in Windows task-bar and elsewhere where system Form.Text property is used.
        /// You can also use the hyperlinks as part of the text markup and handle the TitleTextMarkupLinkClick event to be notified when they are clicked.
        /// </summary>
        [Browsable(true), DefaultValue(""), Editor("DevComponents.DotNetBar.Design.TextMarkupUIEditor, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf", typeof(System.Drawing.Design.UITypeEditor)), EditorBrowsable(EditorBrowsableState.Always), Category("Appearance"), Description("Indicates text displayed on Title bar instead of the Form.Text property.")]
        public string TitleText
        {
            get { return _TabStrip.TitleText; }
            set { _TabStrip.TitleText = value; }
        }

        /// <summary>
        /// Occurs when text markup link is clicked.
        /// </summary>
        private void TabStripTitleTextMarkupLinkClick(object sender, MarkupLinkClickEventArgs e)
        {
            if (TitleTextMarkupLinkClick != null)
                TitleTextMarkupLinkClick(this, new MarkupLinkClickEventArgs(e.Name, e.HRef));
        }

        /// <summary>
        /// Gets or sets the Context menu bar associated with the this control which is used as part of Global Items feature. The context menu 
        /// bar assigned here will be used to search for the items with the same Name or GlobalName property so global properties can be propagated when changed.
        /// You should assign this property to enable the Global Items feature to reach your ContextMenuBar.
        /// </summary>
        [DefaultValue(null), Description("Indicates Context menu bar associated with the TabFormControl control which is used as part of Global Items feature."), Category("Data")]
        public ContextMenuBar GlobalContextMenuBar
        {
            get { return m_GlobalContextMenuBar; }
            set
            {
                if (m_GlobalContextMenuBar != null)
                    m_GlobalContextMenuBar.GlobalParentComponent = null;
                m_GlobalContextMenuBar = value;
                if (m_GlobalContextMenuBar != null)
                    m_GlobalContextMenuBar.GlobalParentComponent = this;
            }
        }

        /// <summary>
        /// Gets or sets whether custom caption and quick access toolbar provided by the control is visible. Default value is false.
        /// This property should be set to true when control is used on MetroAppForm.
        /// </summary>
        [Browsable(true), DefaultValue(false), Category("Caption"), Description("Indicates whether custom caption and quick access toolbar provided by the control is visible.")]
        public bool CaptionVisible
        {
            get { return _TabStrip.CaptionVisible; }
            set
            {
                _TabStrip.CaptionVisible = value;
            }
        }

        /// <summary>
        /// Gets or sets the font for the form caption text when CaptionVisible=true. Default value is NULL which means that system font is used.
        /// </summary>
        [Browsable(true), DefaultValue(null), Category("Caption"), Description("Indicates font for the form caption text when CaptionVisible=true.")]
        public Font CaptionFont
        {
            get { return _TabStrip.CaptionFont; }
            set
            {
                _TabStrip.CaptionFont = value;
            }
        }

        /// <summary>
        /// Gets or sets the explicit height of the caption provided by control. Caption height when set is composed of the TabGroupHeight and
        /// the value specified here. Default value is 0 which means that system default caption size is used.
        /// </summary>
        [Browsable(true), DefaultValue(0), Category("Caption"), Description("Indicates explicit height of the caption provided by control")]
        public int CaptionHeight
        {
            get { return _TabStrip.CaptionHeight; }
            set
            {
                _TabStrip.CaptionHeight = value;
                this.RecalcLayout();
            }
        }

        /// <summary>
        /// Specifies the background style of the control.
        /// </summary>
        [Browsable(true), DevCoBrowsable(true), Category("Style"), Description("Gets or sets bar background style."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ElementStyle BackgroundStyle
        {
            get { return _TabStrip.BackgroundStyle; }
        }

        /// <summary>
        /// Gets or sets the currently selected TabFormItem. TabFormItems are selected using the Checked property. Only a single
        /// TabFormItem can be selected (Checked) at any given time.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabFormItem SelectedTab
        {
            get { return _TabStrip.SelectedTab; }
            set
            {
                if (value != null)
                {
                    if(!value.Checked)
                        value.Checked = true;
                    else if (value.Panel != null && !value.Panel.Visible)
                        value.Panel.Visible = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether new tab item which allows creation of new tab when clicked is visible. When visible you need to handle CreateNewTab event and create your new tab in event handler.
        /// </summary>
        [DefaultValue(false), Category("Appearance"), Description("Indicates whether new tab item which allows creation of new tab when clicked is visible. When visible you need to handle CreateNewTab event and create your new tab in event handler.")]
        public bool NewTabItemVisible
        {
            get { return _TabStrip.NewTabItemVisible; }
            set { _TabStrip.NewTabItemVisible = value; }
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
        protected internal virtual void OnCreateNewTab(EventArgs e)
        {
            EventHandler handler = CreateNewTab;
            if (handler != null)
                handler(this, e);
        }

        internal void RaiseCreateNewTab(EventArgs e)
        {
            OnCreateNewTab(e);
        }

        /// <summary>
        /// Returns reference to internal tab-strip control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabFormStripControl TabStrip
        {
            get { return _TabStrip; }
        }

        /// <summary>
        /// Returns collection of items on a bar.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
        public SubItemsCollection Items
        {
            get { return _TabStrip.Items; }
        }

        ///// <summary>
        ///// Returns collection of quick toolbar access and caption items.
        ///// </summary>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
        //public SubItemsCollection QuickToolbarItems
        //{
        //    get { return GetQatSubItemsCollection(); }
        //}

        //private SubItemsCollection GetQatSubItemsCollection()
        //{
        //    return _TabStrip.QuickToolbarItems;
        //}
        /// <summary>
        /// Gets collection of items displayed in control captions, if it is visible (CaptionVisible=true).
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
        public SubItemsCollection CaptionItems
        {
            get { return _TabStrip.TabStripContainer.CaptionItems; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public eDotNetBarStyle EffectiveStyle
        {
            get
            {
                return _TabStrip.EffectiveStyle;
            }
        }

        private TabFormColorTable _ColorTable;
        /// <summary>
        /// Gets or sets the custom color table for the control. When set this color table overrides all system color settings for control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabFormColorTable ColorTable
        {
            get { return _ColorTable; }
            set { _ColorTable = value; }
        }
        //private static BaseItem GetAppButton(TabFormControl tab)
        //{
        //    BaseItem appButton = null;
        //    for (int i = 0; i < tab.QuickToolbarItems.Count; i++)
        //    {
        //        if (tab.QuickToolbarItems[i] is MetroAppButton)
        //        {
        //            appButton = tab.QuickToolbarItems[i];
        //            break;
        //        }

        //    }
        //    return appButton;
        //}
        //protected override void OnBackColorChanged(EventArgs e)
        //{
        //    base.OnBackColorChanged(e);
        //}

#if TRIAL
        private bool _ShownOnce = false;
#endif
        protected override void OnVisibleChanged(EventArgs e)
        {
#if TRIAL
                if(!this.DesignMode && !_ShownOnce)
                {
				    RemindForm frm=new RemindForm();
				    frm.ShowDialog();
				    frm.Dispose();
                    _ShownOnce = true;
                }
#endif

            base.OnVisibleChanged(e);
        }

        private bool _MouseWheelTabScrollEnabled = true;
        /// <summary>
        /// Gets or sets whether mouse wheel scrolls through the tabs. Default value is true.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether mouse wheel scrolls through the tabs.")]
        public bool MouseWheelTabScrollEnabled
        {
            get { return _MouseWheelTabScrollEnabled; }
            set
            {
                _MouseWheelTabScrollEnabled = value;
            }
        }

        /// <summary>
        /// ImageList for images used on Items. Images specified here will always be used on menu-items and are by default used on all Bars.
        /// </summary>
        [Browsable(true), Category("Data"), DefaultValue(null), Description("ImageList for images used on Items. Images specified here will always be used on menu-items and are by default used on all Bars.")]
        public ImageList Images
        {
            get { return _TabStrip.Images; }
            set { _TabStrip.Images = value; }
        }

        /// <summary>
        /// ImageList for medium-sized images used on Items.
        /// </summary>
        [Browsable(true), Category("Data"), DefaultValue(null), Description("ImageList for medium-sized images used on Items.")]
        public ImageList ImagesMedium
        {
            get { return _TabStrip.ImagesMedium; }
            set { _TabStrip.ImagesMedium = value; }
        }

        /// <summary>
        /// ImageList for large-sized images used on Items.
        /// </summary>
        [Browsable(true), Category("Data"), DefaultValue(null), Description("ImageList for large-sized images used on Items.")]
        public ImageList ImagesLarge
        {
            get { return _TabStrip.ImagesLarge; }
            set { _TabStrip.ImagesLarge = value; }
        }

        protected override void OnTabStopChanged(System.EventArgs e)
        {
            base.OnTabStopChanged(e);
            _TabStrip.TabStop = this.TabStop;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can give the focus to this control using the TAB key. Default value is false.
        /// </summary>
        [Browsable(true), DefaultValue(false), Category("Behavior"), Description("Indicates whether the user can give the focus to this control using the TAB key.")]
        public new bool TabStop
        {
            get { return base.TabStop; }
            set { base.TabStop = value; }
        }
        #endregion

        #region Metro Customization

        ///// <summary>
        ///// Gets the reference to the localization object which holds all system text used by the component.
        ///// </summary>
        //[Browsable(true), DevCoBrowsable(true), NotifyParentPropertyAttribute(true), Category("Localization"), Description("Gets system text used by the component.."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //public RibbonLocalization SystemText
        //{
        //    get { return m_SystemText; }
        //}

        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, 100);
            }
        }

        private eTabFormStripControlDock _TabStripDock = eTabFormStripControlDock.Top;
        /// <summary>
        /// Gets or sets the side tab-strip is docked to.
        /// </summary>
        [DefaultValue(eTabFormStripControlDock.Top), Category("Appearance"), Description("Indicates side tab-strip is docked to.")]
        public eTabFormStripControlDock TabStripDock
        {
            get { return _TabStripDock; }
            set
            {
                _TabStripDock = value;
                if (_TabStripDock == eTabFormStripControlDock.Top)
                    _TabStrip.Dock = DockStyle.Top;
                else if (_TabStripDock == eTabFormStripControlDock.Bottom)
                    _TabStrip.Dock = DockStyle.Bottom;
            }
        }
        
        /// <summary>
        /// Gets or sets the font tab items are displayed with.
        /// </summary>
        [AmbientValue((string) null), Localizable(true) , Category("Appearance"), Description("Indicates the font tab items are displayed with.")]
        public Font TabStripFont
        {
            get { return _TabStrip.Font; }
            set
            {
                _TabStrip.Font = value;
            }
        }

        /// <summary>
        /// Gets or sets whether this TabFormControl was auto-created as result of end-user tearing off the tab.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAutoCreated { get; set; }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            if (Dpi.RecordScalePerControl)
                Dpi.SetScaling(factor);
            base.ScaleControl(factor, specified);
        }
        #endregion

    }
    /// <summary>
    /// Specifies dock side for tab form strip control.
    /// </summary>
    public enum eTabFormStripControlDock
    {
        Top,
        Bottom
    }

    /// <summary>
    /// Defines delegate for the PaintTabFormItem event.
    /// </summary>
    public delegate void PaintTabFormItemEventHandler(object sender, PaintTabFormItemEventArgs e);

    /// <summary>
    /// Defines delegate for the PaintTabFormItem event.
    /// </summary>
    public class PaintTabFormItemEventArgs : EventArgs
    {
        /// <summary>
        /// Gets reference to the tab being painted.
        /// </summary>
        public readonly TabFormItem Tab;
        /// <summary>
        /// Gets reference to the graphic canvas for painting.
        /// </summary>
        public readonly Graphics Graphics;
        /// <summary>
        /// Gets or sets whether default painting for the item is performed, default value is true. Set to false to disable internal painting.
        /// </summary>
        public bool PaintDefault = true;
        public PaintTabFormItemEventArgs(TabFormItem tab, Graphics g)
        {
            this.Tab = tab;
            this.Graphics = g;
        }
    }
}
