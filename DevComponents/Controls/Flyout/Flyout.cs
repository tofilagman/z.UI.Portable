using DevComponents.DotNetBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Component to display flyout popup.
    /// </summary>
    [ToolboxBitmap(typeof(Flyout), "Controls.Flyout.ico")]
    [ToolboxItem(true), Designer("DevComponents.DotNetBar.Design.FlyoutDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf")]
    [DefaultEvent("PrepareContent")]
    public partial class Flyout : Component, IMessageHandlerClient
    {
        #region Constructor
        public Flyout()
        {
            InitializeComponent();
        }

        public Flyout(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        #endregion

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            Close();
            if (_MessageHandlerInstalled)
            {
                MessageHandler.UnregisterMessageClient(this);
                _MessageHandlerInstalled = false;
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion

        #region Implementation
        private bool _DropShadow;
        private bool _TopMost;
        private Control _Content = null;
        /// <summary>
        /// Indicates a control, usually panel with other controls inside of it, that is displayed on the flyout popup.
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates a control, usually panel with other controls inside of it, that is displayed on the flyout popup.")]
        public Control Content
        {
            get { return _Content; }
            set
            {
                if (value != _Content)
                {
                    Control oldValue = _Content;
                    _Content = value;
                    OnContentChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when Content property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnContentChanged(Control oldValue, Control newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Content"));

        }

        private ePointerSide _PointerSide = ePointerSide.Bottom;
        /// <summary>
        /// Indicates the side of the flyout triangle pointer is displayed on.
        /// </summary>
        [DefaultValue(ePointerSide.Bottom), Category("Appearance"), Description("Indicates the side of the flyout triangle pointer is displayed on.")]
        public ePointerSide PointerSide
        {
            get { return _PointerSide; }
            set
            {
                if (value != _PointerSide)
                {
                    ePointerSide oldValue = _PointerSide;
                    _PointerSide = value;
                    OnPointerSideChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when PointerSide property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnPointerSideChanged(ePointerSide oldValue, ePointerSide newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("PointerSide"));
        }

        private Control _TargetControl = null;
        /// <summary>
        /// Indicates the target control for the flyout display and positioning.
        /// </summary>
        [DefaultValue(null), Category("Behavior"), Description("Indicates the target control for the flyout display and positioning.")]
        public Control TargetControl
        {
            get { return _TargetControl; }
            set
            {
                if (value != _TargetControl)
                {
                    Control oldValue = _TargetControl;
                    _TargetControl = value;
                    OnTargetControlChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when TargetControl property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnTargetControlChanged(Control oldValue, Control newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("TargetControl"));
            if (!this.DesignMode)
            {
                DetachFromControl(oldValue, _DisplayMode, _DeepIntegration);
                AttachToControl(newValue, _DisplayMode, _DeepIntegration);
            }
        }

        private void AttachToControl(Control c, eFlyoutDisplayMode displayMode, bool deepIntegration)
        {
            if (c == null) return;
            if (displayMode == eFlyoutDisplayMode.MouseClick)
            {
                if (deepIntegration && c is SuperTabControl)
                {
                    SuperTabControl tab = (SuperTabControl)c;
                    tab.TabStrip.ItemClick += TabStripMouseEvent;
                }
                else if (deepIntegration && c is AdvTree.AdvTree)
                {
                    AdvTree.AdvTree tree = (AdvTree.AdvTree)c;
                    tree.NodeClick += TreeMouseEvent;
                }
                else if (deepIntegration && c is TokenEditor)
                {
                    TokenEditor token = (TokenEditor)c;
                    token.TokenMouseClick += TokenMouseEvent;
                }
                else if (deepIntegration && c is TabStrip)
                {
                    TabStrip strip = (TabStrip)c;
                    strip.TabMouseClick += TabItemMouseEvent;
                }
                else if (deepIntegration && c is TabControl)
                {
                    TabControl strip = (TabControl)c;
                    strip.TabStrip.TabMouseClick += TabItemMouseEvent;
                }
                else if (deepIntegration && c is Bar)
                {
                    Bar bar = (Bar)c;
                    bar.ItemClick += BaseItemMouseEvent;
                }
                else
                    c.MouseClick += TargetMouseClick;
            }
            else if (displayMode == eFlyoutDisplayMode.MouseHover)
            {
                if (deepIntegration && c is SuperTabControl)
                {
                    SuperTabControl tab = (SuperTabControl)c;
                    tab.TabStrip.MouseHover += TabStripMouseEvent;
                    tab.TabStrip.MouseLeave += TargetMouseLeave;
                }
                else if (deepIntegration && c is AdvTree.AdvTree)
                {
                    AdvTree.AdvTree tree = (AdvTree.AdvTree)c;
                    tree.NodeMouseHover += TreeMouseEvent;
                    tree.NodeMouseLeave += TargetMouseLeave;
                }
                else if (deepIntegration && c is TokenEditor)
                {
                    TokenEditor token = (TokenEditor)c;
                    token.TokenMouseHover += TokenMouseEvent;
                    token.TokenMouseLeave += TargetMouseLeave;
                }
                else if (deepIntegration && c is TabStrip)
                {
                    TabStrip tab = (TabStrip)c;
                    tab.TabMouseHover += TabItemMouseEvent;
                    tab.TabMouseLeave += TargetMouseLeave;
                }
                else if (deepIntegration && c is TabControl)
                {
                    TabControl tab = (TabControl)c;
                    tab.TabStrip.TabMouseHover += TabItemMouseEvent;
                    tab.TabStrip.TabMouseLeave += TargetMouseLeave;
                }
                else if (deepIntegration && c is Bar)
                {
                    Bar bar = (Bar)c;
                    bar.MouseHover += BaseItemMouseEvent;
                    bar.MouseLeave += TargetMouseLeave;
                }
                else
                    c.MouseHover += TargetMouseHover;
            }
            else if (displayMode == eFlyoutDisplayMode.MouseOver)
            {
                if (deepIntegration && c is SuperTabControl)
                {
                    SuperTabControl tab = (SuperTabControl)c;
                    tab.TabStrip.MouseEnter += TabStripMouseEvent;
                    tab.TabStrip.MouseLeave += TargetMouseLeave;
                }
                else if (deepIntegration && c is AdvTree.AdvTree)
                {
                    AdvTree.AdvTree tree = (AdvTree.AdvTree)c;
                    tree.NodeMouseEnter += TreeMouseEvent;
                    tree.NodeMouseLeave += TargetMouseLeave;
                }
                else if (deepIntegration && c is TokenEditor)
                {
                    TokenEditor token = (TokenEditor)c;
                    token.TokenMouseEnter += TokenMouseEvent;
                    token.TokenMouseLeave += TargetMouseLeave;
                }
                else if (deepIntegration && c is TabStrip)
                {
                    TabStrip tab = (TabStrip)c;
                    tab.TabMouseEnter += TabItemMouseEvent;
                    tab.TabMouseLeave += TargetMouseLeave;
                }
                else if (deepIntegration && c is TabControl)
                {
                    TabControl tab = (TabControl)c;
                    tab.TabStrip.TabMouseEnter += TabItemMouseEvent;
                    tab.TabStrip.TabMouseLeave += TargetMouseLeave;
                }
                else if (deepIntegration && c is Bar)
                {
                    Bar bar = (Bar)c;
                    bar.MouseMove += BaseItemMouseEvent;
                    bar.MouseLeave += TargetMouseLeave;
                }
                else
                    c.MouseEnter += TargetMouseEnter;
            }
            c.Leave += TargetLeaveFocus;
            c.VisibleChanged += TargetVisibleChanged;
        }

        private void DetachFromControl(Control c, eFlyoutDisplayMode displayMode, bool deepIntegration)
        {
            if (c == null) return;
            if (displayMode == eFlyoutDisplayMode.MouseClick)
            {
                if (deepIntegration && c is SuperTabControl)
                {
                    SuperTabControl tab = (SuperTabControl)c;
                    tab.TabStrip.ItemClick -= TabStripMouseEvent;
                }
                else if (deepIntegration && c is AdvTree.AdvTree)
                {
                    AdvTree.AdvTree tree = (AdvTree.AdvTree)c;
                    tree.NodeClick -= TreeMouseEvent;
                }
                else if (deepIntegration && c is TokenEditor)
                {
                    TokenEditor token = (TokenEditor)c;
                    token.TokenMouseClick -= TokenMouseEvent;
                }
                else if (deepIntegration && c is TabStrip)
                {
                    TabStrip strip = (TabStrip)c;
                    strip.TabMouseClick -= TabItemMouseEvent;
                }
                else if (deepIntegration && c is TabControl)
                {
                    TabControl strip = (TabControl)c;
                    strip.TabStrip.TabMouseClick -= TabItemMouseEvent;
                }
                else if (deepIntegration && c is Bar)
                {
                    Bar bar = (Bar)c;
                    bar.ItemClick -= BaseItemMouseEvent;
                }
                else
                    c.MouseClick -= TargetMouseClick;
            }
            else if (displayMode == eFlyoutDisplayMode.MouseHover)
            {
                if (deepIntegration && c is SuperTabControl)
                {
                    SuperTabControl tab = (SuperTabControl)c;
                    tab.TabStrip.MouseHover -= TabStripMouseEvent;
                    tab.TabStrip.MouseLeave -= TargetMouseLeave;
                }
                else if (deepIntegration && c is AdvTree.AdvTree)
                {
                    AdvTree.AdvTree tree = (AdvTree.AdvTree)c;
                    tree.NodeMouseHover -= TreeMouseEvent;
                    tree.NodeMouseLeave -= TargetMouseLeave;
                }
                else if (deepIntegration && c is TokenEditor)
                {
                    TokenEditor token = (TokenEditor)c;
                    token.TokenMouseHover -= TokenMouseEvent;
                    token.TokenMouseLeave -= TargetMouseLeave;
                }
                else if (deepIntegration && c is TabStrip)
                {
                    TabStrip tab = (TabStrip)c;
                    tab.TabMouseHover -= TabItemMouseEvent;
                    tab.TabMouseLeave -= TargetMouseLeave;
                }
                else if (deepIntegration && c is TabControl)
                {
                    TabControl tab = (TabControl)c;
                    tab.TabStrip.TabMouseHover -= TabItemMouseEvent;
                    tab.TabStrip.TabMouseLeave -= TargetMouseLeave;
                }
                else if (deepIntegration && c is Bar)
                {
                    Bar bar = (Bar)c;
                    bar.MouseHover -= BaseItemMouseEvent;
                    bar.MouseLeave -= TargetMouseLeave;
                }
                else
                    c.MouseHover -= TargetMouseHover;
            }
            else if (displayMode == eFlyoutDisplayMode.MouseOver)
            {
                if (deepIntegration && c is SuperTabControl)
                {
                    SuperTabControl tab = (SuperTabControl)c;
                    tab.TabStrip.MouseEnter -= TabStripMouseEvent;
                    tab.TabStrip.MouseLeave -= TargetMouseLeave;
                }
                else if (deepIntegration && c is AdvTree.AdvTree)
                {
                    AdvTree.AdvTree tree = (AdvTree.AdvTree)c;
                    tree.NodeMouseEnter -= TreeMouseEvent;
                    tree.NodeMouseLeave -= TargetMouseLeave;
                }
                else if (deepIntegration && c is TokenEditor)
                {
                    TokenEditor token = (TokenEditor)c;
                    token.TokenMouseEnter -= TokenMouseEvent;
                    token.TokenMouseLeave -= TargetMouseLeave;
                }
                else if (deepIntegration && c is TabStrip)
                {
                    TabStrip tab = (TabStrip)c;
                    tab.TabMouseEnter -= TabItemMouseEvent;
                    tab.TabMouseLeave -= TargetMouseLeave;
                }
                else if (deepIntegration && c is TabControl)
                {
                    TabControl tab = (TabControl)c;
                    tab.TabStrip.TabMouseEnter -= TabItemMouseEvent;
                    tab.TabStrip.TabMouseLeave -= TargetMouseLeave;
                }
                else if (deepIntegration && c is Bar)
                {
                    Bar bar = (Bar)c;
                    bar.MouseEnter -= BaseItemMouseEvent;
                    bar.MouseLeave -= TargetMouseLeave;
                }
                else
                    c.MouseEnter -= TargetMouseEnter;
            }
            c.Leave -= TargetLeaveFocus;
            c.VisibleChanged -= TargetVisibleChanged;
        }
        private bool _CloseDelayed = false;
        private void TargetMouseLeave(object sender, EventArgs e)
        {
            if (_IsFlyoutFormShown)
            {
                _CloseDelayed = true;
                BarUtilities.InvokeDelayed(new MethodInvoker(delegate { CloseFlyoutDelayed(); }), 1000);
                //Close();
            }
        }
        private void CloseFlyoutDelayed()
        {
            if (!_CloseDelayed) return;
            _CloseDelayed = false;
            if (_IsFlyoutFormShown && _Flyout!=null && !_Flyout.Bounds.Contains(Control.MousePosition) && 
                _TargetControl!=null && !(new Rectangle(_TargetControl.PointToScreen(_TargetControl.Location), _TargetControl.Size).Contains(Control.MousePosition)))
            {
                Close();
            }
        }
        private void TokenMouseEvent(object sender, EventArgs e)
        {
            if (_IsFlyoutFormShown)
                Close();
            Show(sender);
        }
        private void TreeMouseEvent(object sender, AdvTree.TreeNodeMouseEventArgs e)
        {
            if (_IsFlyoutFormShown)
                Close();
            Show(e.Node);
        }
        private void TabItemMouseEvent(object sender, EventArgs e)
        {
            if (sender is TabItem)
            {
                if (_IsFlyoutFormShown)
                    Close();
                if (!_IsFlyoutFormShown)
                {
                    Show(sender);
                }
            }
            else
                Close();
        }
        private void BaseItemMouseEvent(object sender, EventArgs e)
        {
            if (sender is BaseItem)
            {
                if (_IsFlyoutFormShown)
                    Close();
                if (!_IsFlyoutFormShown)
                {
                    Show(sender);
                }
            }
            else if (sender is Bar)
            {
                Bar bar = (Bar)sender;
                Point p = bar.PointToClient(Control.MousePosition);
                BaseItem item = bar.ItemsContainer.ItemAtLocation(p.X, p.Y);
                if (item != null)
                {
                    if (_TargetItem != null && _TargetItem.IsAlive && _TargetItem.Target == item)
                        return;
                    
                    if (_IsFlyoutFormShown)
                        Close();
                    
                    if (!_IsFlyoutFormShown)
                    {
                        Show(item);
                    }
                }
                else
                    Close();
            }
            else
                Close();
        }
        private void TabStripMouseEvent(object sender, EventArgs e)
        {
            if (sender is SuperTabItem)
            {
                if (_IsFlyoutFormShown)
                    Close();
                if (!_IsFlyoutFormShown)
                {
                    Show(sender);
                }
            }
            else
                Close();
        }

        private bool _DeepIntegration = true;
        /// <summary>
        /// Indicates whether Flyout integrates on item level with DotNetBar controls it recognizes like SuperTabControl, AdvTree etc.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether Flyout integrates on item level with DotNetBar controls it recognizes like SuperTabControl, AdvTree etc.")]
        public bool DeepIntegration
        {
            get { return _DeepIntegration; }
            set
            {
                if (value != _DeepIntegration)
                {
                    bool oldValue = _DeepIntegration;
                    _DeepIntegration = value;
                    OnDeepControlIntegrationChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when DeepControlIntegration property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnDeepControlIntegrationChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("DeepControlIntegration"));
            DetachFromControl(_TargetControl, _DisplayMode, oldValue);
            AttachToControl(_TargetControl, _DisplayMode, newValue);
        }

        private void TargetVisibleChanged(object sender, EventArgs e)
        {
            if ((_CloseMode & eFlyoutCloseMode.TargetControlHidden) == eFlyoutCloseMode.TargetControlHidden && _TargetControl != null && !_TargetControl.Visible)
                Close();
        }
        void TargetLeaveFocus(object sender, EventArgs e)
        {
            if ((_CloseMode & eFlyoutCloseMode.TargetControlLostFocus) == eFlyoutCloseMode.TargetControlLostFocus)
                Close();
        }

        void TargetMouseEnter(object sender, EventArgs e)
        {
            if (!_IsFlyoutFormShown)
                Show();
        }

        void TargetMouseHover(object sender, EventArgs e)
        {
            if (!_IsFlyoutFormShown)
                Show();
        }
        private void TargetMouseClick(object sender, MouseEventArgs e)
        {
            if (!_IsFlyoutFormShown)
                Show();
        }
        private eFlyoutDisplayMode _DisplayMode = eFlyoutDisplayMode.MouseOver;
        /// <summary>
        /// Specifies when the flyout is displayed.
        /// </summary>
        [DefaultValue(eFlyoutDisplayMode.MouseOver), Category("Behavior"), Description("Specifies when the flyout is displayed.")]
        public eFlyoutDisplayMode DisplayMode
        {
            get { return _DisplayMode; }
            set
            {
                if (value != _DisplayMode)
                {
                    eFlyoutDisplayMode oldValue = _DisplayMode;
                    _DisplayMode = value;
                    OnDisplayModeChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when DisplayMode property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnDisplayModeChanged(eFlyoutDisplayMode oldValue, eFlyoutDisplayMode newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("DisplayMode"));
            DetachFromControl(_TargetControl, oldValue, _DeepIntegration);
            AttachToControl(_TargetControl, newValue, _DeepIntegration);
        }

        private eFlyoutCloseMode _CloseMode = eFlyoutCloseMode.ClickOutside | eFlyoutCloseMode.ParentFormDeactivate;
        /// <summary>
        /// Indicates when Flyout is automatically closed.
        /// </summary>
        [DefaultValue(eFlyoutCloseMode.ClickOutside | eFlyoutCloseMode.ParentFormDeactivate), Category("Behavior"), Description("Indicates when Flyout is automatically closed.")]
        public eFlyoutCloseMode CloseMode
        {
            get { return _CloseMode; }
            set
            {
                if (value != _CloseMode)
                {
                    eFlyoutCloseMode oldValue = _CloseMode;
                    _CloseMode = value;
                    OnCloseModeChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when CloseMode property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnCloseModeChanged(eFlyoutCloseMode oldValue, eFlyoutCloseMode newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("CloseMode"));

        }

        private Rectangle GetTargetBounds(object targetItem)
        {
            if (targetItem is BaseItem)
            {
                BaseItem item = (BaseItem)targetItem;
                return new Rectangle(_TargetControl.PointToScreen(item.Bounds.Location), item.Bounds.Size);
            }
            else if (targetItem is DevComponents.AdvTree.Node)
            {
                DevComponents.AdvTree.Node node = (DevComponents.AdvTree.Node)targetItem;
                return new Rectangle(_TargetControl.PointToScreen(node.Bounds.Location), node.Bounds.Size);
            }
            else if(targetItem is EditToken)
            {
                EditToken token = (EditToken)targetItem;
                return new Rectangle(_TargetControl.PointToScreen(token.Bounds.Location), token.Bounds.Size);
            }
            else if (targetItem is TabItem)
            {
                TabItem tab = (TabItem)targetItem;
                return new Rectangle(_TargetControl.PointToScreen(tab.DisplayRectangle.Location), tab.DisplayRectangle.Size);
            }

            return new Rectangle(_TargetControl.PointToScreen(Point.Empty), _TargetControl.Size);
        }

        /// <summary>
        /// Occurs before the flyout is shown for specific target and allows you to prepare Content for it. Sender of event will be the targeted control or item.
        /// </summary>
        [Description("Occurs before the flyout is shown for specific target and allows you to prepare Content for it. Sender of event will be the targeted control or item.")]
        public event EventHandler PrepareContent;
        /// <summary>
        /// Raises PrepareContent event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnPrepareContent(object sender, EventArgs e)
        {
            EventHandler handler = PrepareContent;
            if (handler != null)
                handler(sender, e);
        }

        public virtual void Show(object targetItem)
        {
            OnPrepareContent(targetItem, EventArgs.Empty);

            Rectangle r = new Rectangle();
            ePointerSide pointerSide = _PointerSide;
            int pointerOffset = 10;

            if (_TargetControl != null)
            {
                Rectangle targetBounds = GetTargetBounds(targetItem ?? _TargetControl);
                ScreenInformation si = BarFunctions.ScreenFromControl(_TargetControl);
                if (pointerSide == ePointerSide.Top)
                {
                    // Displaying callout below the control
                    r.Size = GetFlyoutFormSize();
                    if (targetBounds.Bottom + r.Height > si.WorkingArea.Bottom)
                    {
                        // Move flyout above the control
                        pointerSide = ePointerSide.Bottom;
                        r.Location = new Point(targetBounds.X, targetBounds.Y - r.Height);
                    }
                    else
                        r.Location = new Point(targetBounds.X, targetBounds.Bottom);
                    if (targetBounds.Width > r.Width)
                        r.X += (targetBounds.Width - r.Width) / 2;
                    else if (targetBounds.Width < r.Width)
                        r.X -= (r.Width - targetBounds.Width) / 2;
                    if (r.X < si.WorkingArea.X)
                        r.X = si.WorkingArea.X;
                    else if (r.Right > si.WorkingArea.Right)
                        r.X = si.WorkingArea.Right - r.Width;
                    Rectangle intersect = Rectangle.Intersect(r, new Rectangle(targetBounds.X, r.Y, targetBounds.Width, r.Height));
                    if (intersect.IsEmpty)
                        pointerOffset = (Math.Min(targetBounds.Width, r.Width) - FlyoutForm.PointerSize.Width) / 2;
                    else
                        pointerOffset = Math.Abs(intersect.X - r.X) + (intersect.Width - FlyoutForm.PointerSize.Width) / 2;
                }
                else if (pointerSide == ePointerSide.Bottom)
                {
                    // Displaying callout above the control
                    r.Size = GetFlyoutFormSize();
                    if (targetBounds.Y - r.Height < si.WorkingArea.Y)
                    {
                        // Move flyout below the control
                        pointerSide = ePointerSide.Top;
                        r.Location = new Point(targetBounds.X, targetBounds.Bottom);
                    }
                    else
                        r.Location = new Point(targetBounds.X, targetBounds.Y - r.Height);
                    if (targetBounds.Width > r.Width)
                        r.X += (targetBounds.Width - r.Width) / 2;
                    else if (targetBounds.Width < r.Width)
                        r.X -= (r.Width - targetBounds.Width) / 2;
                    if (r.X < si.WorkingArea.X)
                        r.X = si.WorkingArea.X;
                    else if (r.Right > si.WorkingArea.Right)
                        r.X = si.WorkingArea.Right - r.Width;
                    Rectangle intersect = Rectangle.Intersect(r, new Rectangle(targetBounds.X, r.Y, targetBounds.Width, r.Height));
                    if (intersect.IsEmpty)
                        pointerOffset = (Math.Min(targetBounds.Width, r.Width) - FlyoutForm.PointerSize.Width) / 2;
                    else
                        pointerOffset = Math.Abs(intersect.X - r.X) + (intersect.Width - FlyoutForm.PointerSize.Width) / 2;
                }
                else if (pointerSide == ePointerSide.Left)
                {
                    // Displaying callout to the right of the target control
                    r.Size = GetFlyoutFormSize();
                    if (targetBounds.Right + r.Width > si.WorkingArea.Right)
                    {
                        // Move flyout to the left of the control
                        pointerSide = ePointerSide.Right;
                        r.Location = new Point(targetBounds.X - r.Width, targetBounds.Y);
                    }
                    else
                        r.Location = new Point(targetBounds.Right, targetBounds.Y);

                    if (targetBounds.Height > r.Height)
                        r.Y += (targetBounds.Height - r.Height) / 2;
                    pointerOffset = (Math.Min(targetBounds.Height, r.Height) - FlyoutForm.PointerSize.Width) / 2;
                }
                else if (pointerSide == ePointerSide.Right)
                {
                    // Displaying callout to the Left of the target control
                    r.Size = GetFlyoutFormSize();
                    if (targetBounds.X - r.Width < si.WorkingArea.X)
                    {
                        // Move flyout to the right of the control
                        pointerSide = ePointerSide.Left;
                        r.Location = new Point(targetBounds.Right, targetBounds.Y);
                    }
                    else
                        r.Location = new Point(targetBounds.X - r.Width, targetBounds.Y);

                    if (targetBounds.Height > r.Height)
                        r.Y += (targetBounds.Height - r.Height) / 2;
                    pointerOffset = (Math.Min(targetBounds.Height, r.Height) - FlyoutForm.PointerSize.Width) / 2;
                }
            }
            Show(r, pointerSide, pointerOffset, targetItem);
        }

        /// <summary>
        /// Shows flyout with the Content. 
        /// </summary>
        public virtual void Show()
        {
            Show(_TargetControl);
        }

        /// <summary>
        /// Returns the flyout size based on Content size.
        /// </summary>
        /// <returns>Proposed flyout size.</returns>
        public virtual Size GetFlyoutFormSize()
        {
            if (_Content != null)
            {
                Size size = _Content.Size;
                if (_PointerSide == ePointerSide.Bottom || _PointerSide == ePointerSide.Top)
                {
                    size.Height += FlyoutForm.PointerSize.Height + 4;
                    size.Width += 2;
                }
                else
                {
                    size.Width += FlyoutForm.PointerSize.Height + 4;
                    size.Height += 2;
                }
                return size;
            }
            return new Size(100, 100);
        }
        /// <summary>
        /// Shows flyout at specified location and with specified size. Size can be empty (0,0) and flyout will be automatically sized based on the content.
        /// </summary>
        /// <param name="screenFlyoutBounds"></param>
        public virtual void Show(Rectangle screenFlyoutBounds)
        {
            Show(screenFlyoutBounds, _PointerSide, 10, _TargetControl);
        }
        [Description("Occurs before the flyout form is shown and allows you to cancel the showing.")]
        public event FlyoutShowingEventHandler FlyoutShowing;
        /// <summary>
        /// Raises FlyoutShowing event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnFlyoutShowing(FlyoutShowingEventArgs e)
        {
            FlyoutShowingEventHandler handler = FlyoutShowing;
            if (handler != null)
                handler(this, e);
        }
        /// <summary>
        /// Occurs after flyout has been shown.
        /// </summary>
        [Description("Occurs after flyout has been shown.")]
        public event EventHandler FlyoutShown;
        /// <summary>
        /// Raises FlyoutShown event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnFlyoutShown(EventArgs e)
        {
            EventHandler handler = FlyoutShown;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Provides opportunity to cancel showing of the flyout before any objects are created and allocated. This is preferred event to cancel flyout showing.
        /// </summary>
        [Description("Provides opportunity to cancel showing of the flyout before any objects are created and allocated. This is preferred event to cancel flyout showing.")]
        public event CancelEventHandler QueryShowFlyout;
        /// <summary>
        /// Raises QueryShowFlyout event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnQueryShowFlyout(object sender, CancelEventArgs e)
        {
            CancelEventHandler handler = QueryShowFlyout;
            if (handler != null)
                handler(sender, e);
        }

        private bool _IsFlyoutFormShown = false;
        private Control _OldContentParent = null;
        private WeakReference _TargetItem = null;
        private FlyoutForm _Flyout = null;
        /// <summary>
        /// Shows flyout at specified location and with specified size. Size can be empty (0,0) and flyout will be automatically sized based on the content.
        /// </summary>
        /// <param name="screenFlyoutBounds">Screen bounds to display flyout at.</param>
        /// <param name="pointerSide">Side of the flyout which will have pointer triangle</param>
        /// <param name="pointerOffset">Pointer position either x or y depending on which side pointer is displayed on.</param>
        /// <param name="targetItem">Target item for the flyout.</param>
        public virtual void Show(Rectangle screenFlyoutBounds, ePointerSide pointerSide, int pointerOffset, object targetItem)
        {
            _CloseDelayed = false;
            if (_IsFlyoutFormShown)
            {
                if (_Flyout != null) _Flyout.Activate();
                return;
            }

            CancelEventArgs cancelShow = new CancelEventArgs();
            OnQueryShowFlyout(targetItem??_TargetControl,cancelShow);
            if (cancelShow.Cancel) return;

            FlyoutForm form = new FlyoutForm();
            if (!_BorderColor.IsEmpty)
                form.BorderColor = _BorderColor;
            if (!_BackColor.IsEmpty)
                form.BackColor = _BackColor;
            form.PointerSide = pointerSide;
            form.ActivateOnShow = _ActivateOnShow;
            form.TopMost = _TopMost;
            form.DropShadow = _DropShadow;
            form.PointerOffset = pointerOffset;
            if (_Content != null)
            {
                if (screenFlyoutBounds.Size.IsEmpty)
                {
                    screenFlyoutBounds.Size = GetFlyoutFormSize();
                }

                if (_Content.Parent != null)
                {
                    _OldContentParent = _Content.Parent;
                    _OldContentParent.Controls.Remove(_Content);
                }

                form.Controls.Add(_Content);
                _Content.Location = new Point(
                    (pointerSide == ePointerSide.Left) ? FlyoutForm.PointerSize.Height + 1 : 1,
                    (pointerSide == ePointerSide.Top) ? FlyoutForm.PointerSize.Height + 1 : 1);
                _Content.Visible = true;
            }

            form.Size = Size.Empty;
            form.Location = screenFlyoutBounds.Location;
            form.FormClosed += FlyoutFormClosed;
            form.FormClosing += FlyoutFormClosing;
            FlyoutShowingEventArgs eargs = new FlyoutShowingEventArgs(form, targetItem);
            OnFlyoutShowing(eargs);
            if (eargs.Cancel)
            {
                FlyoutCloseCleanup(form);
                form.Close();
                form.Dispose();
                return;
            }
            _IsFlyoutFormShown = true;
            form.Visible = true;
            form.Size = screenFlyoutBounds.Size;
            _Flyout = form;
            _TargetItem = new WeakReference(targetItem);
            OnFlyoutShown(EventArgs.Empty);

            if (!_MessageHandlerInstalled && (_CloseMode & eFlyoutCloseMode.ClickOutside) == eFlyoutCloseMode.ClickOutside)
            {
                MessageHandler.RegisterMessageClient(this);
                _MessageHandlerInstalled = true;
            }

            if (_Parent != null && (_CloseMode & eFlyoutCloseMode.ParentFormDeactivate) == eFlyoutCloseMode.ParentFormDeactivate)
            {
                Form parentForm = null;
                if (_Parent is Form)
                    parentForm = (Form)_Parent;
                else
                    parentForm = _Parent.FindForm();
                if (parentForm != null)
                {
                    parentForm.Deactivate += ParentFormDeactivate;
                    _ParentForm = parentForm;
                }
            }
        }

        /// <summary>
        /// Occurs before flyout is closed and allows you to cancel the closing.
        /// </summary>
        [Description("Occurs before flyout is closed and allows you to cancel the closing.")]
        public event FormClosingEventHandler FlyoutClosing;
        /// <summary>
        /// Raises FlyoutClosing event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnFlyoutClosing(FormClosingEventArgs e)
        {
            FormClosingEventHandler handler = FlyoutClosing;
            if (handler != null)
                handler(this, e);
        }
        private void FlyoutFormClosing(object sender, FormClosingEventArgs e)
        {
            OnFlyoutClosing(e);
        }

        /// <summary>
        /// Occurs after flyout is closed.
        /// </summary>
        [Description("Occurs after flyout is closed.")]
        public event FormClosedEventHandler FlyoutClosed;
        /// <summary>
        /// Raises FlyoutClosed event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnFlyoutClosed(FormClosedEventArgs e)
        {
            FormClosedEventHandler handler = FlyoutClosed;
            if (handler != null)
                handler(this, e);
        }

        private Form _ParentForm = null;
        void ParentFormDeactivate(object sender, EventArgs e)
        {
            if (_Flyout != null && _Flyout.Bounds.Contains(Control.MousePosition))
                return;
            Close();
        }

        /// <summary>
        /// Closes the flyout form if it was open.
        /// </summary>
        public virtual void Close()
        {
            FlyoutForm flyout = _Flyout;
            if (flyout != null)
            {
                flyout.Close();
                flyout.Dispose();
            }
        }

        private void FlyoutCloseCleanup(FlyoutForm form)
        {
            _TargetItem = null;
            _IsFlyoutFormShown = false;
            form.FormClosed -= FlyoutFormClosed;
            form.Controls.Remove(_Content);
            if (_OldContentParent != null)
            {
                _Content.Visible = false;
                _OldContentParent.Controls.Add(_Content);
                _OldContentParent = null;
            }
            _Flyout = null;
            if (_ParentForm != null)
            {
                _ParentForm.Deactivate -= ParentFormDeactivate;
                _ParentForm = null;
            }
        }
        private void FlyoutFormClosed(object sender, FormClosedEventArgs e)
        {
            FlyoutForm form = (FlyoutForm)sender;
            OnFlyoutClosed(e);
            FlyoutCloseCleanup(form);
        }

        private bool _ActivateOnShow = false;
        /// <summary>
        /// Indicates whether flyout is active/focused when its shown, default value is false.
        /// </summary>
        [DefaultValue(false), Category("Behavior"), Description("Indicates whether flyout is active/focused when its shown, default value is false.")]
        public bool ActivateOnShow
        {
            get { return _ActivateOnShow; }
            set
            {
                if (value != _ActivateOnShow)
                {
                    bool oldValue = _ActivateOnShow;
                    _ActivateOnShow = value;
                    OnActivateOnShowChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when ActivateOnShow property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnActivateOnShowChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("ActivateOnShow"));

        }

        /// <summary>
        /// Indicates whether flyout is made top-most window when shown
        /// </summary>
        [DefaultValue(false), Category("Behavior"), Description("Indicates whether flyout is made top-most window when shown.")]
        public bool TopMost
        {
            get { return _TopMost; }
            set
            {
                if (value != _TopMost)
                {
                    bool oldValue = _TopMost;
                    _TopMost = value;
                    OnTopMostChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when TopMost property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnTopMostChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("TopMost"));

        }

        private Color _BackColor = Color.Empty;
        /// <summary>
        /// Gets or sets the background flyout color. Default value is Color.Empty which indicates that current color scheme will be used.
        /// </summary>
        [Category("Columns"), Description("Indicates background flyout color. Default value is Color.Empty which indicates that current color scheme will be used.")]
        public Color BackColor
        {
            get { return _BackColor; }
            set { _BackColor = value; }
        }
        /// <summary>
        /// Gets whether property should be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeBackColor()
        {
            return !_BackColor.IsEmpty;
        }
        /// <summary>
        /// Resets property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetBackColor()
        {
            this.BackColor = Color.Empty;
        }

        private Color _BorderColor = Color.Empty;
        /// <summary>
        /// Gets or sets the flyout border color. Default value of Color.Empty indicates that color scheme will be used.
        /// </summary>
        [Category("Columns"), Description("Indicates flyout border color. Default value of Color.Empty indicates that color scheme will be used.")]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set { _BorderColor = value; }
        }
        /// <summary>
        /// Gets whether property should be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeBorderColor()
        {
            return !_BorderColor.IsEmpty;
        }
        /// <summary>
        /// Resets property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetBorderColor()
        {
            this.BorderColor = Color.Empty;
        }

        /// <summary>
        /// Indicates whether flyout displays drop shadow.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether flyout displays drop shadow")]
        public bool DropShadow
        {
            get { return _DropShadow; }
            set
            {
                if (value != _DropShadow)
                {
                    bool oldValue = _DropShadow;
                    _DropShadow = value;
                    OnDropShadowChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when DropShadow property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnDropShadowChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("DropShadow"));

        }

        private Control _Parent = null;
        /// <summary>
        /// Gets or sets the parent control for the Flyout. Parent is used to find the parent form so flyout can be closed when form is de-activated.
        /// </summary>
        [Browsable(false), DefaultValue(null)]
        public Control Parent
        {
            get { return _Parent; }
            set
            {
                if (value != _Parent)
                {
                    Control oldValue = _Parent;
                    _Parent = value;
                    OnParentChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when Parent property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnParentChanged(Control oldValue, Control newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Parent"));
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
            if ((_CloseMode & eFlyoutCloseMode.ClickOutside) == eFlyoutCloseMode.ClickOutside)
            {
                FlyoutForm form = _Flyout;
                if (form != null && form.Visible)
                {
                    if (!form.Bounds.Contains(Control.MousePosition))
                    {
                        // Ignore clicks on ComboBox popup
                        string s = NativeFunctions.GetClassName(hWnd);
                        s = s.ToLower();
                        if (s.IndexOf("combolbox") < 0)
                            Close();
                    }
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
    /// Defines delegate for the FlyoutShowing event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="ea"></param>
    public delegate void FlyoutShowingEventHandler(object sender, FlyoutShowingEventArgs e);
    public class FlyoutShowingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the reference to the flyout form.
        /// </summary>
        public FlyoutForm Flyout;
        /// <summary>
        /// Gets the reference to the flyout target usually TargetControl.
        /// </summary>
        public object Target;
        /// <summary>
        /// Allows you to cancel showing of the flyout by setting this value to true.
        /// </summary>
        public bool Cancel;
        /// <summary>
        /// Initializes a new instance of the FlyoutShowingEventArgs class.
        /// </summary>
        /// <param name="flyout"></param>
        /// <param name="target"></param>
        public FlyoutShowingEventArgs(FlyoutForm flyout, object target)
        {
            Flyout = flyout;
            Target = target;
        }
    }

    /// <summary>
    /// Defines the modes for Flyout display.
    /// </summary>
    public enum eFlyoutDisplayMode
    {
        /// <summary>
        /// Flyout is displayed manually using flyout.Show() method.
        /// </summary>
        Manual,
        /// <summary>
        /// Flyout is displayed when mouse is over TargetControl.
        /// </summary>
        MouseOver,
        /// <summary>
        /// Flyout is displayed when mouse is hovering over TargetControl.
        /// </summary>
        MouseHover,
        /// <summary>
        /// Flyout is displayed when left mouse button is clicked on TargetControl.
        /// </summary>
        MouseClick
    }

    /// <summary>
    /// Defines Flyout closing condition.
    /// </summary>
    [Flags()]
    public enum eFlyoutCloseMode
    {
        /// <summary>
        /// Flyout is closed manually using flyout.Close() method.
        /// </summary>
        Manual,
        /// <summary>
        /// Flyout is closed when user clicks outside of flyout bounds.
        /// </summary>
        ClickOutside,
        /// <summary>
        /// Flyout is closed when TargetControl is hidden.
        /// </summary>
        TargetControlHidden,
        /// <summary>
        /// Flyout is closed when TargetControl loses focus.
        /// </summary>
        TargetControlLostFocus,
        /// <summary>
        /// Flyout is closed when parent forms deactivates.
        /// </summary>
        ParentFormDeactivate
    }
}
