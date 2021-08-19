using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.Rendering;
using DevComponents.UI.ContentManager;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Represents a group in ToolboxControl
    /// </summary>
    [ToolboxItem(false), DesignTimeVisible(false), DefaultEvent("Click"), Designer("DevComponents.DotNetBar.Design.ToolboxGroupDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf")]
    public class ToolboxGroup : ItemContainer
    {
        #region Constructor

        /// <summary>
        /// Creates new instance of the ItemContainer object.
        /// </summary>
        public ToolboxGroup()
        {
            this.MultiLine = true;
            this.ResizeItemsToFit = false;
            this.TitleStyle.Class = ElementStyleClassKeys.ToolboxGroupTitleKey;
            this.TitleMouseOverStyle.Class = ElementStyleClassKeys.ToolboxGroupTitleMouseOverKey;
            this.GlobalItem = false;
            this.EqualItemSize = true;
            _TitleGroupExpandedStyle = new ElementStyle();
            _TitleGroupExpandedStyle.StyleChanged += TitleGroupExpandedStyleChanged;
            _TitleGroupExpandedStyle.Class = ElementStyleClassKeys.ToolboxGroupExpandedTitleKey;
            //this.CanCustomize = false;
        }
        /// <summary>
        /// Creates new instance of the ItemContainer object.
        /// </summary>
        public ToolboxGroup(string groupText)
            : this()
        {
            this.TitleText = groupText;
        }

        /// <summary>
        /// Returns copy of the item.
        /// </summary>
        public override BaseItem Copy()
        {
            ToolboxGroup objCopy = new ToolboxGroup(this.Name);
            this.CopyToItem(objCopy);
            return objCopy;
        }

        /// <summary>
        /// Copies the ButtonItem specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New ButtonItem instance.</param>
        internal void InternalCopyToItem(ButtonItem copy)
        {
            CopyToItem((BaseItem)copy);
        }

        /// <summary>
        /// Copies the ButtonItem specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New ButtonItem instance.</param>
        protected override void CopyToItem(BaseItem copy)
        {
            ToolboxGroup copyGroup = copy as ToolboxGroup;
            base.CopyToItem(copyGroup);
            if (copyGroup != null)
            {
                copyGroup.ResizeItemsToFit = this.ResizeItemsToFit;
            }
        }

        #endregion

            #region Implementation
        private List<BaseItem> _HiddenItems = new List<BaseItem>();
        /// <summary>
        /// Occurs when Expanded state changes. If overridden base implementation must be called so default processing can occur.
        /// </summary>
        protected internal override void OnExpandChange()
        {
            ApplyExpandedProperty();
            base.OnExpandChange();
        }

        private void ApplyExpandedProperty()
        {
            ToolboxControl tc = GetToolboxControl();
            bool supressRecalcLayout = false;
            if (tc != null && tc.IsSearching) supressRecalcLayout = true;

            if (this.Expanded)
            {
                foreach (BaseItem item in _HiddenItems)
                {
                    item.Visible = true;
                }
                _HiddenItems.Clear();
                if (tc != null)
                {
                    if (tc.ExpandSingleGroupOnly && !tc.IsSearching)
                    {
                        foreach (BaseItem item in tc.Groups)
                        {
                            ToolboxGroup group = item as ToolboxGroup;
                            if (group != null && group != this && group.Expanded)
                                group.Expanded = false;
                        }
                    }
                    if (!supressRecalcLayout)
                        this.RecalcLayout();
                    tc.InvokeToolboxGroupExpanded(this);
                }
            }
            else
            {
                foreach (BaseItem item in this.SubItems)
                {
                    if (item.Visible)
                    {
                        item.Visible = false;
                        _HiddenItems.Add(item);
                    }
                }
            }
            this.NeedRecalcSize = true;
            if (!supressRecalcLayout)
                this.RecalcLayout();
        }

        /// <summary>
        /// Returns reference to the parent ToolboxControl.
        /// </summary>
        /// <returns></returns>
        public ToolboxControl GetToolboxControl()
        {
            ItemPanel c = this.ContainerControl as ItemPanel;
            if (c != null)
                return c.Parent as ToolboxControl;
            return null;
        }

        private void RecalcLayout()
        {
            System.Windows.Forms.Control c = this.ContainerControl as System.Windows.Forms.Control;
            if (c != null)
            {
                BarFunctions.InvokeRecalcLayout(c, true);
            }
        }

        protected override void OnSubItemsChanged(CollectionChangeAction action, BaseItem item)
        {
            if (action == CollectionChangeAction.Add)
            {
                if (!this.Expanded && item != null && item.Visible)
                {
                    item.Visible = false;
                    _HiddenItems.Add(item);
                }
                else if (this.Expanded && item != null && !item.Visible)
                    item.Visible = true;
            }
            else if (action == CollectionChangeAction.Remove)
            {
                if (!this.Expanded && item != null && _HiddenItems.Contains(item))
                {
                    _HiddenItems.Remove(item);
                }
            }
            else if (action == CollectionChangeAction.Refresh)
            {
                _HiddenItems.Clear();
                if (!this.Expanded)
                    ApplyExpandedProperty();
            }
            base.OnSubItemsChanged(action, item);
        }

        public override void InternalMouseDown(MouseEventArgs objArg)
        {
            if (TitleRectangle.Contains(objArg.Location))
            {
                this.Expanded = !this.Expanded;
            }
            base.InternalMouseDown(objArg);
        }

        protected override ElementStyle GetActiveTitleStyle()
        {
            if (this.Expanded && _TitleGroupExpandedStyle.Custom)
                return _TitleGroupExpandedStyle;
            return base.GetActiveTitleStyle();
        }

        private ElementStyle _TitleGroupExpandedStyle = null;
        /// <summary>
        /// Specifies the title background style when toolbox group is expanded.
        /// </summary>
        [Browsable(true), DevCoBrowsable(true), Category("Style"), Description("Specifies the title background style when toolbox group is expanded."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ElementStyle TitleGroupExpandedStyle
        {
            get { return _TitleGroupExpandedStyle; }
        }
        private void TitleGroupExpandedStyleChanged(object sender, EventArgs e)
        {
            this.OnAppearanceChanged();
        }

        protected override IContentLayout GetContentLayout()
        {
            SerialContentLayoutManager manager = (SerialContentLayoutManager)base.GetContentLayout();
            ToolboxControl tc = GetToolboxControl();
            if (_StretchItems && this.LayoutOrientation == eOrientation.Vertical && !this.MultiLine && tc != null && tc.Expanded)
            {
                manager.VerticalFitContainerWidth = true;
            }
            return manager;
        }

        private bool _StretchItems = true;
        /// <summary>
        /// Indicates whether items in vertical layout orientation with MultiLine=false are stretched to fill group width. Default value is true.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether items in vertical layout orientation with MultiLine=false are stretched to fill group width. Default value is true.")]
        public bool StretchItems
        {
            get { return _StretchItems; }
            set { _StretchItems = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is expanded or not. For Popup items this would indicate whether the item is popped up or not.
        /// </summary>
        [DefaultValue(false), DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public override bool Expanded
        {
            get { return base.Expanded; }

            set { base.Expanded = value; }
        }

        public override bool CanExpand
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets whether items in horizontal layout are wrapped into the new line when they cannot fit allotted container size. Default value is false.
        /// </summary>
        [Browsable(true), DefaultValue(true), Category("Layout"), Description("Indicates whether items in horizontal layout are wrapped into the new line when they cannot fit allotted container size.")]
        public override bool MultiLine
        {
            get { return base.MultiLine; }
            set
            {
                base.MultiLine = value;
            }
        }

        /// <summary>
        /// Gets or sets whether items contained by container are resized to fit the container bounds. When container is in horizontal
        /// layout mode then all items will have the same height. When container is in vertical layout mode then all items
        /// will have the same width. Default value is true.
        /// </summary>
        [Browsable(true), DevCoBrowsable(true), DefaultValue(false), Category("Layout"), Description("Indicates whether items contained by container are resized to fit the container bounds. When container is in horizontal layout mode then all items will have the same height. When container is in vertical layout mode then all items will have the same width.")]
        public override bool ResizeItemsToFit
        {
            get { return base.ResizeItemsToFit; }
            set { base.ResizeItemsToFit = value; }
        }

        /// <summary>
        /// Gets or sets whether all items are equally sized based on the size of the largest item in the list.
        /// </summary>
        [DefaultValue(false), Category("Layout"), Description("Indicates whether all items are equally sized based on the size of the largest item in the list.")]
        public override bool EqualItemSize
        {
            get { return base.EqualItemSize; }
            set { base.EqualItemSize = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

        #endregion

    }
}
