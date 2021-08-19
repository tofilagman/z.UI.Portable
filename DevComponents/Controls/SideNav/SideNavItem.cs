using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.Rendering;

namespace DevComponents.DotNetBar.Controls
{
    [ToolboxItem(false), DesignTimeVisible(false)]
    public class SideNavItem : ButtonItem
    {
        #region Private Variables & Constructor
        private SideNavPanel _Panel = null;

        /// <summary>
        /// Initializes a new instance of the MetroTabItem class.
        /// </summary>
        public SideNavItem()
        {
            this.ButtonStyle = eButtonStyle.ImageAndText;
        }
        #endregion

        #region Internal Implementation
        public override void Paint(ItemPaintArgs p)
        {
            Rendering.BaseRenderer renderer = p.Renderer;
            if (renderer != null)
            {
                SideNavItemRendererEventArgs args = new SideNavItemRendererEventArgs(this, p.Graphics);
                args.ItemPaintArgs = p;
                renderer.DrawSideNavItem(args);
            }

            if (!string.IsNullOrEmpty(NotificationMarkText))
                DevComponents.DotNetBar.Rendering.NotificationMarkPainter.Paint(p.Graphics, this.Bounds, NotificationMarkPosition,
                    NotificationMarkText, new Size(NotificationMarkSize, NotificationMarkSize), NotificationMarkOffset, NotificationMarkColor);
            
            if (this.DesignMode && this.Focused)
            {
                Rectangle r = this.Bounds;
                r.Inflate(-1, -1);
                DesignTime.DrawDesignTimeSelection(p.Graphics, r, p.Colors.ItemDesignTimeBorder);
            }

            this.DrawInsertMarker(p.Graphics);
        }

        public override void RecalcSize()
        {
            _ImageRenderBounds = Rectangle.Empty;
            _TextRenderBounds = Rectangle.Empty;
            base.RecalcSize();
        }

        private Rectangle _ImageRenderBounds = Rectangle.Empty;
        /// <summary>
        /// Gets or sets cached image rendering bounds.
        /// </summary>
        internal Rectangle ImageRenderBounds
        {
            get { return _ImageRenderBounds; }
            set { _ImageRenderBounds = value; }
        }

        private Rectangle _TextRenderBounds = Rectangle.Empty;
        /// <summary>
        /// Gets or sets cached text rendering bounds.
        /// </summary>
        internal Rectangle TextRenderBounds
        {
            get { return _TextRenderBounds; }
            set { _TextRenderBounds = value; }
        }

        protected override bool IsFadeEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Selects the tab.
        /// </summary>
        public void Select()
        {
            if (AllowSelection)
                this.Checked = true;
        }

        /// <summary>
        /// Gets or sets the panel assigned to this tab item.
        /// </summary>
        [Browsable(false), DefaultValue(null)]
        public SideNavPanel Panel
        {
            get { return _Panel; }
            set
            {
                _Panel = value;
                OnPanelChanged();
            }
        }

        private void OnPanelChanged()
        {
            ChangePanelVisibility();
        }

        /// <summary>
        /// Called after Checked property has changed.
        /// </summary>
        protected override void OnCheckedChanged()
        {
            if (this.Checked && this.Parent != null)
            {
                ChangePanelVisibility();
                foreach (BaseItem item in this.Parent.SubItems)
                {
                    if (item == this)
                        continue;
                    SideNavItem b = item as SideNavItem;
                    if (b != null && b.Checked)
                    {
                        if (this.DesignMode)
                            TypeDescriptor.GetProperties(b)["Checked"].SetValue(b, false);
                        else
                            b.Checked = false;
                    }
                }
            }

            if (BarFunctions.IsOffice2007Style(this.EffectiveStyle) && this.ContainerControl is System.Windows.Forms.Control)
                ((System.Windows.Forms.Control)this.ContainerControl).Invalidate();
            if (!this.Checked)
                ChangePanelVisibility();

            InvokeCheckedChanged();
        }

        private void ChangePanelVisibility()
        {
            if (this.Checked && _Panel != null)
            {
                if (this.DesignMode)
                {
                    if (!_Panel.Visible) _Panel.Visible = true;
                    TypeDescriptor.GetProperties(_Panel)["Visible"].SetValue(_Panel, true);
                    _Panel.BringToFront();
                }
                else
                {
                    if (!_Panel.IsDisposed)
                    {
                        // Had to remove this optimization since in certain use-cases it caused
                        // .NET WinForms layout framework to move child controls inside of the panel
                        // Following 3 lines reduce flashing of panel's child controls when Dock panel is shown
                        //System.Windows.Forms.DockStyle oldDock = _Panel.Dock;
                        //_Panel.Dock = System.Windows.Forms.DockStyle.None;
                        //_Panel.Location = new Point(-32000, 32000);
                        _Panel.Enabled = true;
                        _Panel.Visible = true;
                        _Panel.BringToFront();
                        //if (_Panel.Dock != oldDock)
                        //    _Panel.Dock = oldDock;
                    }

                }
            }
            else if (!this.Checked && _Panel != null)
            {
                if (this.DesignMode)
                    TypeDescriptor.GetProperties(_Panel)["Visible"].SetValue(_Panel, false);
                else
                {
                    _Panel.Visible = false;
                    _Panel.Enabled = false;
                }
            }
        }

        private bool AllowSelection
        {
            get { return _Panel != null; }
        }

        /// <summary>
        /// Occurs just before Click event is fired.
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();
            if (_IsSystemMenu)
            {
                SideNavStrip strip = this.ContainerControl as SideNavStrip;
                if(strip!=null && strip.Parent is SideNav)
                    ((SideNav)strip.Parent).IsMenuExpanded = !((SideNav)strip.Parent).IsMenuExpanded;
            }
            else if (!this.Checked && AllowSelection)
            {
                if (this.DesignMode)
                    TypeDescriptor.GetProperties(this)["Checked"].SetValue(this, true);
                else
                {
                    SideNav nav = GetSideNav();
                    if (nav != null && !nav.ValidateChildren())
                        return;
                    this.Checked = true;
                }
            }
        }
        private SideNav GetSideNav()
        {
            SideNavStrip strip = this.ContainerControl as SideNavStrip;
            if (strip == null) return null;
            return strip.Parent as SideNav;
        }
        /// <summary>
        /// Called when Visibility of the items has changed.
        /// </summary>
        /// <param name="bVisible">New Visible state.</param>
        protected internal override void OnVisibleChanged(bool bVisible)
        {
            base.OnVisibleChanged(bVisible);
            if (!bVisible && this.Checked)
            {
                TypeDescriptor.GetProperties(this)["Checked"].SetValue(this, false);
                // Try to check first item in the group
                if (this.Parent != null)
                {
                    foreach (BaseItem item in this.Parent.SubItems)
                    {
                        if (item == this || !item.GetEnabled() || !item.Visible)
                            continue;
                        SideNavItem b = item as SideNavItem;
                        if (b != null)
                        {
                            TypeDescriptor.GetProperties(b)["Checked"].SetValue(this, true);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or set the Group item belongs to. The groups allows a user to choose from mutually exclusive options within the group. The choice is reflected by Checked property.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), DefaultValue(""), EditorBrowsable(EditorBrowsableState.Never)]
        public override string OptionGroup
        {
            get { return base.OptionGroup; }
            set { base.OptionGroup = value; }
        }

        /// <summary>
        /// Returns the collection of sub items.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)]
        public override SubItemsCollection SubItems
        {
            get { return base.SubItems; }
        }

        internal override void DoAccesibleDefaultAction()
        {
            if (AllowSelection)
                this.Checked = true;
        }

        protected override void Invalidate(System.Windows.Forms.Control containerControl)
        {
            Rectangle r = m_Rect;
            r.Width++;
            r.Height++;
            if (containerControl.InvokeRequired)
                containerControl.BeginInvoke(new MethodInvoker(delegate { containerControl.Invalidate(r, true); }));
            else
                containerControl.Invalidate(r, true);
        }

        public override bool UseParentSubItemsImageSize
        {
            get
            {
                return false;
            }
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

        private Color[] _BorderColors = null;
        /// <summary>
        /// Indicates the array of colors that when set are used to draw the border of the item.
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates the array of colors that when set are used to draw the border of the item."), TypeConverter(typeof(ArrayConverter))]
        public Color[] BorderColors
        {
            get
            {
                return _BorderColors;
            }
            set
            {
                if (_BorderColors != value)
                {
                    _BorderColors = value;
                    //OnPropertyChanged(new PropertyChangedEventArgs("Colors"));
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// Occurs after item visual style has changed.
        /// </summary>
        protected override void OnStyleChanged()
        {
            base.OnStyleChanged();
            UpdateItemAppearance();
        }

        private int _PaddingHorizontal = 0;
        /// <summary>
        /// Gets or sets the additional padding added around the tab item in pixels. Default value is 0.
        /// </summary>
        [Browsable(true), DefaultValue(0), Category("Layout"), Description("Indicates additional padding added around the tab item in pixels.")]
        public int PaddingHorizontal
        {
            get { return _PaddingHorizontal; }
            set
            {
                _PaddingHorizontal = value;
                UpdateItemAppearance();
            }
        }
        private void UpdateItemAppearance()
        {
            this.VerticalPadding = 6;
            this.HorizontalPadding = 14 + _PaddingHorizontal;

            this.NeedRecalcSize = true;
            this.OnAppearanceChanged();
        }

        private string _Title = "";
        /// <summary>
        /// Indicates an optional title for the associated panel. If not set item text is used.
        /// </summary>
        [DefaultValue(""), Category("Appearance"), Description("Indicates an optional title for the associated panel. If not set item text is used.")]
        public string Title
        {
            get { return _Title; }
            set
            {
                if (value == null) value = "";
                if (_Title != value)
                {
                    string oldValue = _Title;
                    _Title = value;
                    OnTitleChanged(value, oldValue);
                }
            }
        }

        protected virtual void OnTitleChanged(string newValue, string oldValue)
        {
            if (this.Checked)
            {
                SideNavStrip strip = this.ContainerControl as SideNavStrip;
                if (strip != null && strip.Parent is SideNav)
                    ((SideNav)strip.Parent).UpdateSelectedItemTitle();
            }
        }

        private bool _IsSystemMenu = false;
        /// <summary>
        /// Gets or sets whether this item acts as the SideNav control system menu which collapses and expands the SideNav items.
        /// </summary>
        [DefaultValue(false), Category("Behavior"), Description("Indicates whether this item acts as the SideNav control system menu which collapses and expands the SideNav items.")]
        public bool IsSystemMenu
        {
            get { return _IsSystemMenu; }
            set
            {
                if (_IsSystemMenu != value)
                {
                    bool oldValue = _IsSystemMenu;
                    _IsSystemMenu = value;
                    OnIsSystemMenuChanged(value, oldValue);
                }
            }
        }

        protected virtual void OnIsSystemMenuChanged(bool newValue, bool oldValue)
        {
            if (newValue && this.DesignMode)
            {
                if (string.IsNullOrEmpty(this.Symbol))
                    this.Symbol = "\uf0c9";
                if (string.IsNullOrEmpty(this.Text))
                    this.Text = "Menu";
            }

            //throw new NotImplementedException();
        }
        #endregion

        #region Hidden Properties
        /// <summary>
        /// Indicates whether the item will auto-collapse (fold) when clicked. 
        /// When item is on popup menu and this property is set to false, menu will not
        /// close when item is clicked.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), Category("Behavior"), DefaultValue(true), Description("Indicates whether the item will auto-collapse (fold) when clicked.")]
        public override bool AutoCollapseOnClick
        {
            get
            {
                return base.AutoCollapseOnClick;
            }
            set
            {
                base.AutoCollapseOnClick = value;
            }
        }

        /// <summary>
        /// Indicates whether the item will auto-expand when clicked. 
        /// When item is on top level bar and not on menu and contains sub-items, sub-items will be shown only if user
        /// click the expand part of the button. Setting this propert to true will expand the button and show sub-items when user
        /// clicks anywhere inside of the button. Default value is false which indicates that button is expanded only
        /// if its expand part is clicked.
        /// </summary>
        [DefaultValue(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DevCoBrowsable(false), Category("Behavior"), Description("Indicates whether the item will auto-collapse (fold) when clicked.")]
        public override bool AutoExpandOnClick
        {
            get
            {
                return base.AutoExpandOnClick;
            }
            set
            {
                base.AutoExpandOnClick = value;
            }
        }

        /// <summary>
        /// Gets or sets whether item can be customized by end user.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true), System.ComponentModel.Category("Behavior"), System.ComponentModel.Description("Indicates whether item can be customized by user.")]
        public override bool CanCustomize
        {
            get
            {
                return base.CanCustomize;
            }
            set
            {
                base.CanCustomize = value;
            }
        }

        /// <summary>
        /// Gets or set a value indicating whether the button is in the checked state.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), Category("Appearance"), Description("Indicates whether item is checked or not."), DefaultValue(false)]
        public override bool Checked
        {
            get
            {
                return base.Checked;
            }
            set
            {
                base.Checked = value;
            }
        }

        /// <summary>
        /// Gets or sets whether Click event will be auto repeated when mouse button is kept pressed over the item.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false), Category("Behavior"), Description("Gets or sets whether Click event will be auto repeated when mouse button is kept pressed over the item.")]
        public override bool ClickAutoRepeat
        {
            get
            {
                return base.ClickAutoRepeat;
            }
            set
            {
                base.ClickAutoRepeat = value;
            }
        }

        /// <summary>
        /// Gets or sets the auto-repeat interval for the click event when mouse button is kept pressed over the item.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(600), Category("Behavior"), Description("Gets or sets the auto-repeat interval for the click event when mouse button is kept pressed over the item.")]
        public override int ClickRepeatInterval
        {
            get
            {
                return base.ClickRepeatInterval;
            }
            set
            {
                base.ClickRepeatInterval = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is enabled.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), DefaultValue(true), Category("Behavior"), Description("Indicates whether is item enabled.")]
        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
            }
        }

        /// <summary>
        /// Indicates item's visiblity when on pop-up menu.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), Category("Appearance"), Description("Indicates item's visiblity when on pop-up menu."), DefaultValue(eMenuVisibility.VisibleAlways)]
        public override eMenuVisibility MenuVisibility
        {
            get
            {
                return base.MenuVisibility;
            }
            set
            {
                base.MenuVisibility = value;
            }
        }

        /// <summary>
        /// Indicates when menu items are displayed when MenuVisiblity is set to VisibleIfRecentlyUsed and RecentlyUsed is true.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DevCoBrowsable(false), Category("Appearance"), Description("Indicates when menu items are displayed when MenuVisiblity is set to VisibleIfRecentlyUsed and RecentlyUsed is true."), DefaultValue(ePersonalizedMenus.Disabled)]
        public override ePersonalizedMenus PersonalizedMenus
        {
            get
            {
                return base.PersonalizedMenus;
            }
            set
            {
                base.PersonalizedMenus = value;
            }
        }

        /// <summary>
        /// Indicates Animation type for Popups.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), Category("Behavior"), Description("Indicates Animation type for Popups."), DefaultValue(ePopupAnimation.ManagerControlled)]
        public override ePopupAnimation PopupAnimation
        {
            get
            {
                return base.PopupAnimation;
            }
            set
            {
                base.PopupAnimation = value;
            }
        }

        /// <summary>
        /// Indicates the font that will be used on the popup window.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), Category("Appearance"), Description("Indicates the font that will be used on the popup window."), DefaultValue(null)]
        public override System.Drawing.Font PopupFont
        {
            get
            {
                return base.PopupFont;
            }
            set
            {
                base.PopupFont = value;
            }
        }

        /// <summary>
        /// Indicates whether sub-items are shown on popup Bar or popup menu.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), Category("Appearance"), Description("Indicates whether sub-items are shown on popup Bar or popup menu."), DefaultValue(ePopupType.Menu)]
        public override ePopupType PopupType
        {
            get
            {
                return base.PopupType;
            }
            set
            {
                base.PopupType = value;
            }
        }

        /// <summary>
        /// Specifies the inital width for the Bar that hosts pop-up items. Applies to PopupType.Toolbar only.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), Category("Layout"), Description("Specifies the inital width for the Bar that hosts pop-up items. Applies to PopupType.Toolbar only."), DefaultValue(200)]
        public override int PopupWidth
        {
            get
            {
                return base.PopupWidth;
            }
            set
            {
                base.PopupWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets whether item will display sub items.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true), Category("Behavior"), Description("Determines whether sub-items are displayed.")]
        public override bool ShowSubItems
        {
            get
            {
                return base.ShowSubItems;
            }
            set
            {
                base.ShowSubItems = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the item expands automatically to fill out the remaining space inside the container. Applies to Items on stretchable, no-wrap Bars only.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false), Category("Appearance"), Description("Indicates whether item will stretch to consume empty space. Items on stretchable, no-wrap Bars only.")]
        public override bool Stretch
        {
            get
            {
                return base.Stretch;
            }
            set
            {
                base.Stretch = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the expand part of the button item.
        /// </summary>
        [Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), Category("Behavior"), Description("Indicates the width of the expand part of the button item."), DefaultValue(12)]
        public override int SubItemsExpandWidth
        {
            get { return base.SubItemsExpandWidth; }
            set
            {
                base.SubItemsExpandWidth = value;
            }
        }

        /// <summary>
        /// Gets or set the alternative shortcut text.
        /// </summary>
        [System.ComponentModel.Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DevCoBrowsable(false), System.ComponentModel.Category("Design"), System.ComponentModel.Description("Gets or set the alternative Shortcut Text.  This text appears next to the Text instead of any shortcuts"), System.ComponentModel.DefaultValue("")]
        public override string AlternateShortCutText
        {
            get
            {
                return base.AlternateShortCutText;
            }
            set
            {
                base.AlternateShortCutText = value;
            }
        }

        /// <summary>
        /// Gets or sets whether item separator is shown before this item.
        /// </summary>
        [System.ComponentModel.Browsable(false), DevCoBrowsable(false), System.ComponentModel.DefaultValue(false), System.ComponentModel.Category("Appearance"), System.ComponentModel.Description("Indicates whether this item is beginning of the group.")]
        public override bool BeginGroup
        {
            get
            {
                return base.BeginGroup;
            }
            set
            {
                base.BeginGroup = value;
            }
        }

        /// <summary>
        /// Returns category for this item. If item cannot be customzied using the
        /// customize dialog category is empty string.
        /// </summary>
        [System.ComponentModel.Browsable(false), DevCoBrowsable(false), System.ComponentModel.DefaultValue(""), System.ComponentModel.Category("Design"), System.ComponentModel.Description("Indicates item category used to group similar items at design-time."), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Category
        {
            get
            {

                return base.Category;
            }
            set
            {
                base.Category = value;
            }
        }

        /// <summary>
        /// Gets or sets the text color of the button when mouse is over the item.
        /// </summary>
        [System.ComponentModel.Browsable(false), DevCoBrowsable(false), System.ComponentModel.Category("Appearance"), System.ComponentModel.Description("The foreground color used to display text when mouse is over the item."), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color HotForeColor
        {
            get
            {
                return base.HotForeColor;
            }
            set
            {
                base.HotForeColor = value;
            }
        }

        /// <summary>
        /// Indicates the way item is painting the picture when mouse is over it. Setting the value to Color will render the image in gray-scale when mouse is not over the item.
        /// </summary>
        [System.ComponentModel.Browsable(false), DevCoBrowsable(false), System.ComponentModel.Category("Appearance"), System.ComponentModel.Description("Indicates the way item is painting the picture when mouse is over it. Setting the value to Color will render the image in gray-scale when mouse is not over the item."), System.ComponentModel.DefaultValue(eHotTrackingStyle.Default), EditorBrowsable(EditorBrowsableState.Never)]
        public override eHotTrackingStyle HotTrackingStyle
        {
            get { return base.HotTrackingStyle; }
            set
            {
                base.HotTrackingStyle = value;
            }
        }

        /// <summary>
        /// Gets/Sets the button style which controls the appearance of the button elements. Changing the property can display image only, text only or image and text on the button at all times.
        /// </summary>
        [Browsable(false), Category("Appearance"), Description("Determines the style of the button."), DefaultValue(eButtonStyle.ImageAndText), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override eButtonStyle ButtonStyle
        {
            get
            {
                return base.ButtonStyle;
            }
            set
            {
                base.ButtonStyle = value;
            }
        }

        
        #endregion
    }
}
