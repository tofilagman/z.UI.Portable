using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using DevComponents.DotNetBar.Metro;
using DevComponents.DotNetBar.Rendering;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Represents SideNav control to create "hamburger" menus.
    /// </summary>
    [ToolboxBitmap(typeof(SideNav), "Controls.SideNav.ico"), ToolboxItem(true),Designer("DevComponents.DotNetBar.Design.SideNavDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf"), System.Runtime.InteropServices.ComVisible(false)]
    public class SideNav : System.Windows.Forms.ContainerControl
    {
        #region Constructor
        private SideNavStrip _Strip = null;
        private Bar _TitleBar = null;
        private LabelItem _TitleLabel = null;
        private ButtonItem _CloseButton = null;
        private ButtonItem _MaximizeButton = null;
        private ButtonItem _RestoreButton = null;
        private Control _Splitter = null;
        public SideNav()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                  | ControlStyles.ResizeRedraw
                  | DisplayHelp.DoubleBufferFlag
                  | ControlStyles.UserPaint
                  | ControlStyles.Opaque
                  , true);

            this.Padding = new System.Windows.Forms.Padding(1);

            _Splitter = new Control();
            _Splitter.Width = 4;
            _Splitter.Name = "splitter";
            _Splitter.Cursor = Cursors.VSplit;
            _Splitter.Dock = DockStyle.Right;
            _Splitter.MouseMove += SplitterMouseMove;
            _Splitter.MouseDown += SplitterMouseDown;
            this.Controls.Add(_Splitter);


            // Title bar and fold/extend buttons
            Bar titleBar = new Bar();
            titleBar.Name = "titleBar";
            titleBar.AntiAlias = true;
            titleBar.Name = "titleBar";
            titleBar.PaddingBottom = 7;
            titleBar.PaddingTop = 5;
            titleBar.PaddingLeft = 6;
            titleBar.RoundCorners = false;
            titleBar.Dock = DockStyle.Top;
            titleBar.Stretch = true;
            titleBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            titleBar.TabIndex = 0;
            titleBar.TabStop = false;
            titleBar.BarType = eBarType.StatusBar;
            titleBar.ItemsContainer.OverflowEnabled = false;
            LabelItem titleLabel=new LabelItem();
            titleLabel.Name = "titleLabel";
            titleLabel.Text = "Title";
            titleLabel.FontBold = true;
            _TitleLabel = titleLabel;
            titleBar.Items.Add(titleLabel);
            _TitleBar = titleBar;
            _CloseButton = new ButtonItem("openCloseButton");
            _CloseButton.Symbol = "\uf0d9";
            _CloseButton.ImagePaddingHorizontal = 18;
            _CloseButton.ItemAlignment= eItemAlignment.Far;
            _CloseButton.SymbolSize = 12f;
            _CloseButton.Click+=CloseButtonClick;
            titleBar.Items.Add(_CloseButton);
            _MaximizeButton = new ButtonItem("maximizeButton");
            _MaximizeButton.Symbol = "\uf090";
            _MaximizeButton.ItemAlignment = eItemAlignment.Far;
            _MaximizeButton.SymbolSize = 12f;
            _MaximizeButton.Click += MaximizeButtonClick;
            titleBar.Items.Add(_MaximizeButton);
            _RestoreButton = new ButtonItem("restoreButton");
            _RestoreButton.Symbol = "\uf100";
            _RestoreButton.ItemAlignment = eItemAlignment.Far;
            _RestoreButton.SymbolSize = 12f;
            _RestoreButton.Click += RestoreButtonClick;
            _RestoreButton.Visible = false;
            titleBar.Items.Add(_RestoreButton);

            this.Controls.Add(titleBar);

            _Strip = new SideNavStrip();
            _Strip.Dock = DockStyle.Left;
            _Strip.AutoSize = true;
            _Strip.AutoSyncSizeOrientation = eOrientation.Horizontal;
            _Strip.ButtonCheckedChanged += StripButtonCheckedChanged;

            //SideNavItem menuItem=new SideNavItem();
            //menuItem.Name = "menuItem";
            //menuItem.Text = "Menu";
            //menuItem.Symbol = "\uf0c9";
            //menuItem.IsSystemMenu = true;
            //_Strip.Items.Add(menuItem);

            //Separator sep = new Separator();
            //sep.SeparatorOrientation = eDesignMarkerOrientation.Vertical;
            //sep.FixedSize = new Size(3,1);
            //sep.Padding.Left = 6;
            //sep.Padding.Right = 6;
            //_Strip.Items.Add(sep);

            //SideNavItem item = new SideNavItem();
            //item.Text = "Home";
            //item.Symbol = "\uf015";
            //_Strip.Items.Add(item);
            //SideNavPanel panel=new SideNavPanel();
            //panel.Dock = DockStyle.Fill;
            //this.Controls.Add(panel);
            //item.Panel = panel;

            //item = new SideNavItem();
            //item.Text = "Explore";
            //item.Symbol = "\uf002";
            //_Strip.Items.Add(item);

            //ButtonItem button=new ButtonItem();
            //button.Text = "Button";
            //button.Symbol = "\uf003";
            //button.ButtonStyle = eButtonStyle.ImageAndText;
            //_Strip.Items.Add(button);
            
            this.Controls.Add(_Strip);
            _Strip.Width = 48;

            StyleManager.Register(this);
            UpdateColors();
        }

        protected override void Dispose(bool disposing)
        {
            if (_IsMaximized && disposing && this.Parent!=null)
            {
                this.Parent.Resize -= ParentResize;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Called by StyleManager to notify control that style on manager has changed and that control should refresh its appearance if
        /// its style is controlled by StyleManager.
        /// </summary>
        /// <param name="newStyle">New active style.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void StyleManagerStyleChanged(eDotNetBarStyle newStyle)
        {
            UpdateColors();
        }
        /// <summary>
        /// Updates the control colors from the global color table.
        /// </summary>
        public void UpdateColors()
        {
            SideNavColorTable ct = GetColorTable();
            _TitleBar.BackColor = ct.TitleBackColor;
            _TitleBar.BorderColors = ct.TitleBorderColors;
            this.Invalidate(true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {   
            Graphics g = e.Graphics;
            DisplayHelp.FillRectangle(g,this.ClientRectangle, this.BackColor);
            SideNavColorTable table = GetColorTable();
            DisplayHelp.DrawRoundedRectangle(g, this.ClientRectangle, table.BorderColors, 0);
        }

        private SideNavColorTable GetColorTable()
        {
            return ((Office2007Renderer)GlobalManager.Renderer).ColorTable.SideNav;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetDesignMode()
        {
            _Strip.SetDesignMode(true);
        }

        private bool _EnableSplitter = true;
        /// <summary>
        /// Indicates whether splitter that is located on right hand side of open control is visible and enabled.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether splitter that is located on right hand side of open control is visible and enabled.")]
        public bool EnableSplitter
        {
            get { return _EnableSplitter; }
            set
            {
                if (_EnableSplitter != value)
                {
                    bool oldValue = _EnableSplitter;
                    _EnableSplitter = value;
                    OnEnableSplitterChanged(value, oldValue);
                }
            }
        }
        protected virtual void OnEnableSplitterChanged(bool newValue, bool oldValue)
        {
            _Splitter.Visible = newValue;
        }

        private bool _EnableClose = true;
        /// <summary>
        /// Indicates whether button which folds/closes the control is visible.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether button which folds/closes the control is visible.")]
        public bool EnableClose
        {
            get { return _EnableClose; }
            set
            {
                if (_EnableClose != value)
                {
                    bool oldValue = _EnableClose;
                    _EnableClose = value;
                    OnEnableCloseChanged(value, oldValue);
                }
            }
        }
        protected virtual void OnEnableCloseChanged(bool newValue, bool oldValue)
        {
            _CloseButton.Visible = newValue;
            _TitleBar.RecalcLayout();
        }

        private bool _EnableMaximize = true;
        // <summary>
        /// Indicates whether buttons which maximize and restore the control are visible.
        /// </summary>
        [DefaultValue(true), Category("Apeparance"), Description("Indicates whether buttons which maximize and restore the control are visible.")]
        public bool EnableMaximize
        {
            get { return _EnableMaximize; }
            set
            {
                if (_EnableMaximize != value)
                {
                    bool oldValue = _EnableMaximize;
                    _EnableMaximize = value;
                    OnEnableMaximizeChanged(value, oldValue);
                }
            }
        }
        protected virtual void OnEnableMaximizeChanged(bool newValue, bool oldValue)
        {
            if (!newValue)
            {
                _MaximizeButton.Visible = false;
                _RestoreButton.Visible = false;
            }
            else
            {
                if (_IsMaximized)
                    _RestoreButton.Visible = true;
                else
                    _MaximizeButton.Visible = true;
            }
            _TitleBar.RecalcLayout();
        }

        /// <summary>
        /// Returns collection of items on a bar.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
        public SubItemsCollection Items
        {
            get
            {
                return _Strip.Items;
            }
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close(eEventSource.Mouse);
        }

        private void MaximizeButtonClick(object sender, EventArgs e)
        {
            Maximize(eEventSource.Mouse);
        }

        private bool _IsMaximized = false;
        private int _RestoredWidth = 0;
        /// <summary>
        /// Maximizes control width so it fills up space to the right of the control.
        /// </summary>
        /// <param name="source">Source of the event.</param>
        public void Maximize(eEventSource source)
        {
            if(_IsMaximized) return;
            
            if (this.Parent == null) 
                return;
            if (this.Bounds.Right >= this.Parent.ClientRectangle.Right)
                return;
            
            CancelSourceEventArgs args=new CancelSourceEventArgs(source);
            OnBeforeMaximize(args);
            if(args.Cancel) return;

            _IsMaximized = true;
            int maxWidth = GetMaxWidth();
            _RestoredWidth = this.Width;
            BarFunctions.AnimateControl(this, true, _AnimationTime, this.Bounds, new Rectangle(this.Location, new Size(maxWidth, this.Height)));
            _RestoreButton.Visible = true;
            _MaximizeButton.Visible = false;
            _TitleBar.RecalcLayout();
            _Splitter.Visible = false;
            this.Invalidate();
            this.Parent.Resize += ParentResize;
        }

        void ParentResize(object sender, EventArgs e)
        {
            if (this.Parent.Width > 0)
                this.Width = GetMaxWidth();
        }

        private int GetMaxWidth()
        {
            int width = this.Width + this.Parent.ClientRectangle.Right - this.Bounds.Right;
            if (this.Parent is MetroAppForm)
            {
                MetroAppForm form = (MetroAppForm) this.Parent;
                if (form.BorderThickness.Right > 0)
                    width -= (int)form.BorderThickness.Right;
                else
                    width -= 3;
            }
            
            return width;
        }

        private void RestoreButtonClick(object sender, EventArgs e)
        {
            Restore(eEventSource.Mouse);
        }

        /// <summary>
        /// Restores the control to previous size if it was maximized before.
        /// </summary>
        /// <param name="source">Source of event.</param>
        public void Restore(eEventSource source)
        {
            if (!_IsMaximized || _RestoredWidth == 0) return;
            _IsMaximized = false;
            
            CancelSourceEventArgs args = new CancelSourceEventArgs(source);
            OnBeforeRestore(args);
            if (args.Cancel) return;

            BarFunctions.AnimateControl(this, true, _AnimationTime, this.Bounds, new Rectangle(this.Location, new Size(_RestoredWidth, this.Height)));
            _RestoredWidth = 0;
            _RestoreButton.Visible = false;
            _MaximizeButton.Visible = true;
            _TitleBar.RecalcLayout();
            _Splitter.Visible = _EnableSplitter;
            this.Invalidate();
            if(this.Parent!=null)
                this.Parent.Resize -= ParentResize;
        }

        private bool _IsOpen = true;
        private int _OpenWidth = 0;
        /// <summary>
        /// Gets or sets whether control is closed, i.e. whether selected item panel is shown or not. When closed
        /// any selected item is unselected and selected panel hidden.
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        public bool IsClosed
        {
            get { return !_IsOpen; }
            set
            {
                if (value)
                {
                    if(_IsOpen)
                        Close(eEventSource.Code);
                }
                else
                {
                    if (!_IsOpen)
                    {
                        SideNavItem item = FirstVisibleSideNavItem??_LastOpenItem;
                        if (item != null)
                            Open(item, eEventSource.Code);
                    }
                }
            }
        }

        private SideNavItem FirstVisibleSideNavItem
        {
            get
            {
                for (int i = 0; i < _Strip.Items.Count; i++)
                {
                    if (_Strip.Items[i] is SideNavItem && _Strip.Items[i].Visible)
                        return (SideNavItem)_Strip.Items[i];
                }
                return null;
            }
        }

        private void StripButtonCheckedChanged(object sender, EventArgs e)
        {
            if (sender is SideNavItem && ((SideNavItem)sender).Checked)
            {
                OnSelectedItemChanged(EventArgs.Empty);
                if (!_IsOpen)
                    Open((SideNavItem)sender, eEventSource.Code);
            }
        }

        /// <summary>
        /// Opens the control, i.e. expands it, selects specified item and shows its associated panel.
        /// </summary>
        /// <param name="item">Item to select.</param>
        /// <param name="source">Source of the event.</param>
        public void Open(SideNavItem item, eEventSource source)
        {
            if (item == null)
                throw new ArgumentException("item must be set to valid item to select");

            if (_IsOpen) return;

            CancelSourceEventArgs args=new CancelSourceEventArgs(source, item);
            OnBeforeOpen(args);
            if(args.Cancel) return;

            if (!item.Checked)
                item.Checked = true;
            BarFunctions.AnimateControl(this, true, _AnimationTime, this.Bounds, new Rectangle(this.Location, new Size(_OpenWidth, this.Height)));
            _IsOpen = true;
            _Splitter.Visible = _EnableSplitter;
            this.Invalidate();
        }

        private SideNavItem _LastOpenItem = null;
        /// <summary>
        /// Closes the control, i.e. unselects any selected item, hide its associated panel and folds the control.
        /// </summary>
        /// <param name="source">Source of the event.</param>
        public void Close(eEventSource source)
        {
            if(!_IsOpen) return;

            CancelSourceEventArgs args = new CancelSourceEventArgs(source);
            OnBeforeClose(args);
            if (args.Cancel) return;

            _OpenWidth = this.Width;
            int closedWidth = _Strip.Width + this.Padding.Horizontal;
            _Splitter.Visible = false;
            BarFunctions.AnimateControl(this, true, _AnimationTime, this.Bounds, new Rectangle(this.Location, new Size(closedWidth, this.Height)));
            _IsOpen = false;
            if (_Strip.SelectedItem != null)
            {
                _LastOpenItem = _Strip.SelectedItem;
                _Strip.SelectedItem.Checked = false;
            }
            else
                _LastOpenItem = null;

            this.Invalidate();
        }

        private bool _IsMenuExpanded = true;
        /// <summary>
        /// Indicates whether side menu is expanded, i.e. shows both image and text. When menu is collapsed only image is shown.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether side menu is expanded, i.e. shows both image and text. When menu is collapsed only image is shown.")]
        public bool IsMenuExpanded
        {
            get { return _IsMenuExpanded; }
            set
            {
                if (_IsMenuExpanded != value)
                {
                    bool oldValue = _IsMenuExpanded;
                    _IsMenuExpanded = value;
                    OnIsMenuExpandedChanged(value, oldValue);
                }
            }
        }

        protected virtual void OnIsMenuExpandedChanged(bool newValue, bool oldValue)
        {
            ExpandMenu(newValue);
            OnIsMenuExpandedChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when IsMenuExpanded property has changed its value.
        /// </summary>
        [Description("Occurs when IsMenuExpanded property has changed.")]
        public event EventHandler IsMenuExpandedChanged;

        /// <summary>
        /// Raises IsMenuExpandedChanged event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnIsMenuExpandedChanged(EventArgs e)
        {
            EventHandler h = IsMenuExpandedChanged;
            if (h != null)
                h(this, e);
        }

        private bool _IsMenuExpandedDelayedSet = false;
        /// <summary>
        /// Expands or collapses the control items menu.
        /// </summary>
        /// <param name="expand"></param>
        private void ExpandMenu(bool expand)
        {
            if (!this.IsHandleCreated)
            {
                _IsMenuExpandedDelayedSet = true;
                return;
            }
            for (int i = 0; i < _Strip.Items.Count; i++)
            {
                ButtonItem button = _Strip.Items[i] as ButtonItem;
                if (button != null)
                {
                    button.ButtonStyle = (expand ? eButtonStyle.ImageAndText : eButtonStyle.Default);
                }
            }
            _Strip.RecalcLayout();
            if (!_IsOpen)
                this.Width = _Strip.Width + this.Padding.Horizontal;
        }

        private int _AnimationTime = 250;
        /// <summary>
        /// Indicates the animation time in milliseconds for operations that perform visual animation of transition. Set to zero to disable animation.
        /// </summary>
        [DefaultValue(250), Category("Behavior"), Description("Indicates the animation time in milliseconds for operations that perform visual animation of transition. Set to zero to disable animation.")]
        public int AnimationTime
        {
            get { return _AnimationTime; }
            set
            {
                if (value != _AnimationTime)
                {
                    int oldValue = _AnimationTime;
                    _AnimationTime = value;
                    OnAnimationTimeChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when AnimationTime property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnAnimationTimeChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("AnimationTime"));

        }

        /// <summary>
        /// Occurs before the control is maximized and allows you to cancel that.
        /// </summary>
        [Description("Occurs before the control is maximized and allows you to cancel that.")]
        public event CancelSourceEventHandler BeforeMaximize;
        /// <summary>
        /// Raises BeforeMaximize event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnBeforeMaximize(CancelSourceEventArgs e)
        {
            CancelSourceEventHandler h = BeforeMaximize;
            if (h != null)
                h(this, e);
        }
        /// <summary>
        /// Occurs before the control is restored and allows you to cancel that.
        /// </summary>
        [Description("Occurs before the control is restored and allows you to cancel that.")]
        public event CancelSourceEventHandler BeforeRestore;
        /// <summary>
        /// Raises BeforeRestore event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnBeforeRestore(CancelSourceEventArgs e)
        {
            CancelSourceEventHandler h = BeforeRestore;
            if (h != null)
                h(this, e);
        }

        /// <summary>
        /// Occurs before the control is opened and allows you to cancel that.
        /// </summary>
        [Description("Occurs before the control is opened and allows you to cancel that.")]
        public event CancelSourceEventHandler BeforeOpen;
        /// <summary>
        /// Raises BeforeOpen event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnBeforeOpen(CancelSourceEventArgs e)
        {
            CancelSourceEventHandler h = BeforeOpen;
            if (h != null)
                h(this, e);
        }

        /// <summary>
        /// Occurs before the control is closed and allows you to cancel that.
        /// </summary>
        [Description("Occurs before the control is closed and allows you to cancel that.")]
        public event CancelSourceEventHandler BeforeClose;
        /// <summary>
        /// Raises BeforeClose event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnBeforeClose(CancelSourceEventArgs e)
        {
            CancelSourceEventHandler h = BeforeClose;
            if (h != null)
                h(this, e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            UpdateSelectedItemTitle();
            UpdateColors();
            if (_IsMenuExpandedDelayedSet)
            {
                ExpandMenu(_IsMenuExpanded);
                _IsMenuExpandedDelayedSet = false;
            }
            base.OnHandleCreated(e);
        }

        internal void UpdateSelectedItemTitle()
        {
            SideNavItem item = _Strip.SelectedItem;
            if (item == null) return;

            if (!string.IsNullOrEmpty(item.Title))
                _TitleLabel.Text = item.Title;
            else
                _TitleLabel.Text = item.Text;
        }

        private Point _SplitterMouseDownPoint = Point.Empty;
        private void SplitterMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _SplitterMouseDownPoint = e.Location;
        }
        private void SplitterMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int maxWidth = GetMaxWidth();
                int minWidth = _Strip.Width + 64;
                int newWidth = this.Width + (e.X - _SplitterMouseDownPoint.X);
                if (newWidth > maxWidth)
                    newWidth = maxWidth;
                else if (newWidth < minWidth)
                    newWidth = minWidth;
                this.Width = newWidth;
            }
        }

        /// <summary>
        /// Gets currently selected item. Only items with Panel assigned can be selected.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SideNavItem SelectedItem
        {
            get
            {
                foreach (BaseItem item in this.Items)
                {
                    if (item is SideNavItem && ((SideNavItem)item).Checked)
                    {
                        return (SideNavItem)item;
                    }
                }
                return null;
            }
            set
            {
                if (value != null && value.Panel != null)
                    value.Checked = true;
            }
        }

        /// <summary>
        /// Occurs when SelectedItem changes.
        /// </summary>
        [Description("Occurs when SelectedItem changes.")]
        public event EventHandler SelectedItemChanged;

        /// <summary>
        /// Raises SelectedItemChanged event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnSelectedItemChanged(EventArgs e)
        {
            EventHandler h = SelectedItemChanged;
            if (h != null)
                h(this, e);
        }
        /// <summary>
        /// Gets reference to internal SideNavStrip control.
        /// </summary>
        [Browsable(false)]
        public SideNavStrip SideNavStrip
        {
            get { return _Strip; }
        }
        #endregion
    }

    /// <summary>
    /// Defines delegate for the CancelSource events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="ea"></param>
    public delegate void CancelSourceEventHandler(object sender, CancelSourceEventArgs e);
    /// <summary>
    /// Event arguments for CancelSourceEventHandler
    /// </summary>
    public class CancelSourceEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Gets the source of the event.
        /// </summary>
        public eEventSource Source = eEventSource.Code;
        /// <summary>
        /// Gets any optional data that is associated with the event.
        /// </summary>
        public object Data = null;
        /// <summary>
        /// Creates new instance of the object.
        /// </summary>
        /// <param name="source">Source of event</param>
        public CancelSourceEventArgs(eEventSource source)
        {
            this.Source = source;
        }
        /// <summary>
        /// Creates new instance of the object.
        /// </summary>
        /// <param name="source">Source of event</param>
        /// <param name="data">Optional data associated with the event.</param>
        public CancelSourceEventArgs(eEventSource source, object data)
        {
            this.Source = source;
            this.Data = data;
        }
    }
}
