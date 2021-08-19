using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.Rendering;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Represents a tab in Tabbed Form user interface.
    /// </summary>
    public class TabFormItem : TabFormItemBase
    {
        #region Private Variables & Constructor
        private TabFormPanel _Panel = null;
        private string _CashedColorTableName = "Default";
        private bool _ReducedSize = false;
        private int _PaddingHorizontal = 0;
        /// <summary>
        /// Initializes a new instance of the TabFormItem class.
        /// </summary>
        public TabFormItem()
        {
            this.ButtonStyle = eButtonStyle.ImageAndText;
            //this.MouseDownCapture = true;
            //this.MouseUpNotification = true;
        }
        #endregion

        #region Internal Implementation
        public override void Paint(ItemPaintArgs p)
        {
            if (!PaintCustom(p))
            {
                Rendering.BaseRenderer renderer = p.Renderer;
                if (renderer != null)
                {
                    p.ButtonItemRendererEventArgs.Graphics = p.Graphics;
                    p.ButtonItemRendererEventArgs.ButtonItem = this;
                    p.ButtonItemRendererEventArgs.ItemPaintArgs = p;
                    renderer.DrawTabFormItem(p.ButtonItemRendererEventArgs);
                }
            }

            if (!string.IsNullOrEmpty(NotificationMarkText))
                DevComponents.DotNetBar.Rendering.NotificationMarkPainter.Paint(p.Graphics, this.Bounds, NotificationMarkPosition,
                    NotificationMarkText, new Size(NotificationMarkSize, NotificationMarkSize), NotificationMarkOffset, NotificationMarkColor);
            this.DrawInsertMarker(p.Graphics);
        }

        /// <summary>
        /// Returns true if custom painting is performed and internal painting should be bypassed.
        /// </summary>
        /// <param name="itemPaintArgs"></param>
        /// <returns></returns>
        private bool PaintCustom(ItemPaintArgs itemPaintArgs)
        {
            TabFormControl tc = GetTabFormControl();
            if (tc == null) return false;

            return !tc.InternalPaintTabFormItem(this,itemPaintArgs);
        }

        public override void RecalcSize()
        {
            _ImageRenderBounds = Rectangle.Empty;
            _TextRenderBounds = Rectangle.Empty;
            base.RecalcSize();

        }

        private bool _ItemDrag = false;
        private InsertPosition _DragInsertPosition = null;
        public override void InternalMouseMove(MouseEventArgs objArg)
        {
            //if (_ItemDrag)
            //{
            //    IDesignTimeProvider dtp = (IDesignTimeProvider)this.Parent;
            //    Point pScreen = this.PointToScreen(objArg.Location);
            //    InsertPosition pos = dtp.GetInsertPosition(pScreen, this);
            //    if (_DragInsertPosition != null)
            //    {
            //        dtp.DrawReversibleMarker(_DragInsertPosition.Position, _DragInsertPosition.Before);
            //        _DragInsertPosition = null;
            //    }
            //    if (pos != null)
            //    {
            //        if (pos.TargetProvider == null)
            //        {
            //            // Cursor is over drag item
            //            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.No;
            //        }
            //        else
            //        {
            //            pos.TargetProvider.DrawReversibleMarker(pos.Position, pos.Before);
            //            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
            //            _DragInsertPosition = pos;
            //        }
            //    }
            //    else
            //    {
            //        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.No;
            //    }
            //}
            //else 
            if (objArg.Button == MouseButtons.Left &&
                     (CloseButtonState == eButtonState.Normal || CloseButtonState == eButtonState.Disabled || CloseButtonState == eButtonState.Hidden) &&
                     (Math.Abs(objArg.X - this.MouseDownPt.X) > Dpi.Width6 || Math.Abs(objArg.Y - this.MouseDownPt.Y) > Dpi.Width6) && IsItemDragEnabled)
            {
                TabFormStripControl parent = this.ContainerControl as TabFormStripControl;
                if (parent != null)
                {
                    parent.StartItemDrag(this);
                }
                //IDesignTimeProvider dtp = this.Parent as IDesignTimeProvider;
                //if (dtp != null)
                //{
                //    TabFormStripControl c = this.ContainerControl as TabFormStripControl;
                //    if (c != null)
                //    {
                //        _ItemDrag = true;
                //        c.DragInProgress = true;
                //    }
                //}
            }
            else if (_CloseButtonVisible && !_CloseButtonBounds.IsEmpty)
            {
                if (_CloseButtonBounds.Contains(objArg.Location))
                    CloseButtonState = eButtonState.MouseOver;
                else
                    CloseButtonState = eButtonState.Normal;
            }
            base.InternalMouseMove(objArg);
        }

        private bool IsItemDragEnabled
        {
            get
            {
                TabFormStripControl c = this.ContainerControl as TabFormStripControl;
                if (c == null) return false;
                return c.IsTabDragEnabled;
            }
        }

        public override void InternalMouseDown(MouseEventArgs objArg)
        {
            if (_CloseButtonState == eButtonState.MouseOver && objArg.Button == MouseButtons.Left)
            {
                CloseButtonState = eButtonState.MouseDownLeft;
            }
            base.InternalMouseDown(objArg);
        }

        public override void InternalMouseUp(MouseEventArgs objArg)
        {
            //if (_ItemDrag)
            //{
            //    _ItemDrag = false;
            //    TabFormStripControl c = this.ContainerControl as TabFormStripControl;
            //    if (c != null)
            //        c.DragInProgress = false;
            //    if (_DragInsertPosition != null)
            //    {
            //        IDesignTimeProvider dtp = (IDesignTimeProvider) this.Parent;
            //        // Removes drag marker
            //        int insertPosition = _DragInsertPosition.Position;
            //        bool insertBefore = _DragInsertPosition.Before;
            //        dtp.DrawReversibleMarker(insertPosition, insertBefore);
            //        if (insertPosition > 0 && dtp == this.Parent)
            //        {
            //            if (this.Parent.SubItems.IndexOf(this) < insertPosition)
            //                insertPosition--;
            //        }
            //        BaseItem parent = this.Parent;
            //        parent.SubItems.Remove(this);
            //        this.Checked = false;
            //        dtp.InsertItemAt(this, insertPosition, insertBefore);
            //        this.Checked = true;
            //        _DragInsertPosition = null;
            //    }
            //}
            //else 
            if (_CloseButtonState == eButtonState.MouseDownLeft)
            {
                CloseButtonState = eButtonState.Normal;
                CloseTab(eEventSource.Mouse);
            }
            base.InternalMouseUp(objArg);
        }

        private void CloseTab(eEventSource source)
        {
            TabFormStripControl strip = this.ContainerControl as TabFormStripControl;
            if (strip != null)
            {
                strip.CloseTab(this, source);
            }
        }

        public override void InternalMouseLeave()
        {
            if (_CloseButtonVisible)
                CloseButtonState = eButtonState.Normal;
            base.InternalMouseLeave();
        }

        private eButtonState _CloseButtonState = eButtonState.Hidden;
        internal eButtonState CloseButtonState
        {
            get { return _CloseButtonState; }
            set
            {
                if (value != _CloseButtonState)
                {
                    _CloseButtonState = value;
                    if (!_CloseButtonBounds.IsEmpty)
                        this.Invalidate();
                }
            }
        }

        private Rectangle _CloseButtonBounds = Rectangle.Empty;
        /// <summary>
        /// Gets or sets the close button bounds.
        /// </summary>
        internal Rectangle CloseButtonBounds
        {
            get { return _CloseButtonBounds; }
            set { _CloseButtonBounds = value; }
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
        /// Gets or sets the additional padding added around the tab item in pixels. Default value is 0.
        /// </summary>
        [Browsable(true), DefaultValue(0), Category("Layout"), Description("Indicates additional padding added around the tab item in pixels.")]
        public int PaddingHorizontal
        {
            get { return _PaddingHorizontal; }
            set
            {
                _PaddingHorizontal = value;
                UpdateTabAppearance();
            }
        }

        private bool _CloseButtonVisible = true;

        /// <summary>
        /// Indicates whether close button is visible on the tabs which when clicked closes the tab. Default value is true.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether close button is visible on the tabs which when clicked closes the tab.")]
        public bool CloseButtonVisible
        {
            get { return _CloseButtonVisible; }
            set
            {
                _CloseButtonVisible = value;
                if (!value)
                    _CloseButtonState = eButtonState.Hidden;
                NeedRecalcSize = true;
                UpdateTabAppearance();
            }
        }

        /// <summary>
        /// Selects the tab.
        /// </summary>
        public void Select()
        {
            this.Checked = true;
        }
        /// <summary>
        /// Gets or sets whether size of the tab has been reduced below the default calculated size.
        /// </summary>
        internal bool ReducedSize
        {
            get { return _ReducedSize; }
            set { _ReducedSize = value; }
        }

        private eTabFormItemColor _ColorTable = eTabFormItemColor.Default;
        /// <summary>
        /// Gets or sets the predefined color of the tab. Default value is eTabFormItemColor.Default
        /// </summary>
        [Browsable(true), DefaultValue(eTabFormItemColor.Default), Category("Appearance"), Description("Indicates predefined color of the tab.")]
        public new eTabFormItemColor ColorTable
        {
            get { return _ColorTable; }
            set
            {
                if (_ColorTable != value)
                {
                    _ColorTable = value;
                    _CashedColorTableName = Enum.GetName(typeof(eTabFormItemColor), _ColorTable);
                    this.Refresh();
                }
            }
        }

        internal override string GetColorTableName()
        {
            return this.CustomColorName != "" ? this.CustomColorName : _CashedColorTableName;
        }

        /// <summary>
        /// Gets or sets the panel assigned to this tab item.
        /// </summary>
        [Browsable(false), DefaultValue(null)]
        public TabFormPanel Panel
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
                    TabFormItem b = item as TabFormItem;
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
                        // Following 3 lines reduce flashing of panel's child controls when Dock panel is shown
                        System.Windows.Forms.DockStyle oldDock = _Panel.Dock;
                        _Panel.Dock = System.Windows.Forms.DockStyle.None;
                        _Panel.Location = new Point(-32000, 32000);
                        _Panel.Enabled = true;
                        _Panel.Visible = true;
                        _Panel.BringToFront();
                        if (_Panel.Dock != oldDock)
                            _Panel.Dock = oldDock;
                    }

                }
            }
            else if (!this.Checked && _Panel != null) // Panels in popup mode will be taken care of by Ribbon
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

        /// <summary>
        /// Occurs just before Click event is fired.
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();
            if (!this.Checked)
            {
                if (this.DesignMode)
                    TypeDescriptor.GetProperties(this)["Checked"].SetValue(this, true);
                else
                {
                    TabFormControl shell = GetTabFormControl();
                    if (shell != null && !shell.ValidateChildren())
                        return;
                    this.Checked = true;
                }
            }
        }
        private TabFormControl GetTabFormControl()
        {
            TabFormStripControl strip = this.ContainerControl as TabFormStripControl;
            if (strip == null) return null;
            return strip.Parent as TabFormControl;
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
                        TabFormItem b = item as TabFormItem;
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
        /// Occurs after item visual style has changed.
        /// </summary>
        protected override void OnStyleChanged()
        {
            base.OnStyleChanged();
            UpdateTabAppearance();
        }

        private void UpdateTabAppearance()
        {
            this.VerticalPadding = 2;
            this.HorizontalPadding = 28 + _PaddingHorizontal + (_CloseButtonVisible ? CloseButtonSize : 0);

            this.NeedRecalcSize = true;
            this.OnAppearanceChanged();
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

        protected override bool ShouldDrawInsertMarker()
        {
            IOwner owner = GetOwner() as IOwner;
            
            return DesignInsertMarker != eDesignInsertPosition.None && this.Visible && this.Displayed &&
                   !this.DesignMode;
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
        /// Gets or set a value indicating whether tab is selected.
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

        ///// <summary>
        ///// Gets or sets the text color of the button.
        ///// </summary>
        //[System.ComponentModel.Browsable(false), DevCoBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), System.ComponentModel.Category("Appearance"), System.ComponentModel.Description("The foreground color used to display text.")]
        //public override Color ForeColor
        //{
        //    get
        //    {
        //        return base.ForeColor;
        //    }
        //    set
        //    {
        //        base.ForeColor = value;
        //    }
        //}

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

        public eTabFormStripControlDock TabAlignment
        {
            get
            {
                TabFormStripControl tabStrip = this.ContainerControl as TabFormStripControl;
                if (tabStrip == null) return eTabFormStripControlDock.Top;
                return tabStrip.TabAlignment;
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

        private TabFormItemColorTable _CustomColorTable = null;
        /// <summary>
        /// Gets or sets the custom color table for the tab. When set this color table overrides all color settings for a tab.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabFormItemColorTable CustomColorTable
        {
            get { return _CustomColorTable; }
            set { _CustomColorTable = value; }
        }

        internal static int Radius
        {
            get { return Dpi.Width8; }
        }

        internal static int Dia
        {
            get { return Radius * 2; }
        }

        internal static int Offset { get { return Dpi.Width2; } }

        internal static int TabOverlap
        {
            get { return Dpi.Width25; }
        }

        internal static int CloseButtonSize
        {
            get { return 13; }
        }

        /// <summary>
        /// Gets color table key for the default tab color table.
        /// </summary>
        public static readonly string DefaultColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Default);
        /// <summary>
        /// Gets color table key for the green tab color table.
        /// </summary>
        public static readonly string GreenColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Green);
        /// <summary>
        /// Gets color table key for the magenta tab color table.
        /// </summary>
        public static readonly string MagentaColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Magenta);
        /// <summary>
        /// Gets color table key for the orange tab color table.
        /// </summary>
        public static readonly string OrangeColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Orange);
        /// <summary>
        /// Gets color table key for the red tab color table.
        /// </summary>
        public static readonly string RedColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Red);
        /// <summary>
        /// Gets color table key for the blue tab color table.
        /// </summary>
        public static readonly string BlueColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Blue);
        /// <summary>
        /// Gets color table key for the yellow tab color table.
        /// </summary>
        public static readonly string YellowColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Yellow);
        /// <summary>
        /// Gets color table key for the purple tab color table.
        /// </summary>
        public static readonly string PurpleColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Purple);
        /// <summary>
        /// Gets color table key for the cyan tab color table.
        /// </summary>
        public static readonly string CyanColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Cyan);
        /// <summary>
        /// Gets color table key for the blue mist tab color table.
        /// </summary>
        public static readonly string BlueMistColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.BlueMist);
        /// <summary>
        /// Gets color table key for the purple mist tab color table.
        /// </summary>
        public static readonly string PurpleMistColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.PurpleMist);
        /// <summary>
        /// Gets color table key for the tan tab color table.
        /// </summary>
        public static readonly string TanColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Tan);
        /// <summary>
        /// Gets color table key for the lemon lime tab color table.
        /// </summary>
        public static readonly string LemonLimeColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.LemonLime);
        /// <summary>
        /// Gets color table key for the apple tab color table.
        /// </summary>
        public static readonly string AppleColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Apple);
        /// <summary>
        /// Gets color table key for the teal tab color table.
        /// </summary>
        public static readonly string TealColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Teal);
        /// <summary>
        /// Gets color table key for the red chalk tab color table.
        /// </summary>
        public static readonly string RedChalkColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.RedChalk);
        /// <summary>
        /// Gets color table key for the silver tab color table.
        /// </summary>
        public static readonly string SilverColorTableKey = Enum.GetName(typeof(eTabFormItemColor), eTabFormItemColor.Silver);
        #endregion
    }

    /// <summary>
    /// Specifies predefined color assigned to Tab Form Item.
    /// </summary>
    public enum eTabFormItemColor
    {
        Default,
        Blue,
        Yellow,
        Green,
        Red,
        Purple,
        Cyan,
        Orange,
        Magenta,
        BlueMist,
        PurpleMist,
        Tan,
        LemonLime,
        Apple,
        Teal,
        RedChalk,
        Silver
    }
}
