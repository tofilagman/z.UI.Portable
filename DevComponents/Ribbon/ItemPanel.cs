using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Represents generic item panel container control.
    /// </summary>
    [ToolboxBitmap(typeof(ItemPanel), "Ribbon.ItemPanel.ico"), ToolboxItem(true), Designer("DevComponents.DotNetBar.Design.ItemPanelDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf"), System.Runtime.InteropServices.ComVisible(false)]
    public class ItemPanel : ItemPanelBase, IScrollableItemControl, IBindingSupport
    {
        #region Private Variables
        #endregion

        #region Constructor
        public ItemPanel()
        {
        }
        #endregion

        #region Internal Implementation
        /// <summary>
        /// Returns first checked top-level button item.
        /// </summary>
        /// <returns>An ButtonItem object or null if no button could be found.</returns>
        public ButtonItem GetChecked()
        {
            foreach (BaseItem item in this.Items)
            {
                if (item.Visible && item is ButtonItem && ((ButtonItem)item).Checked)
                    return item as ButtonItem;
            }

            return null;
        }

        /// <summary>
        /// Gets or sets default layout orientation inside the control. You can have multiple layouts inside of the control by adding
        /// one or more instances of the ItemContainer object and chaning it's LayoutOrientation property.
        /// </summary>
        [Browsable(true), DevCoBrowsable(true), Category("Layout"), DefaultValue(eOrientation.Horizontal), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual eOrientation LayoutOrientation
        {
            get { return ItemContainer.LayoutOrientation; }
            set
            {
                ItemContainer.LayoutOrientation = value;
                if (this.DesignMode)
                    this.RecalcLayout();
            }
        }

        /// <summary>
        /// Gets or sets whether items contained by container are resized to fit the container bounds. When container is in horizontal
        /// layout mode then all items will have the same height. When container is in vertical layout mode then all items
        /// will have the same width. Default value is true.
        /// </summary>
        [Browsable(true), DevCoBrowsable(true), DefaultValue(true), Category("Layout")]
        public virtual bool ResizeItemsToFit
        {
            get { return ItemContainer.ResizeItemsToFit; }
            set
            {
                ItemContainer.ResizeItemsToFit = value;
            }
        }

        /// <summary>
        /// Gets or sets whether ButtonItem buttons when in vertical layout are fit into the available width so any text inside of them
        /// is wrapped if needed. Default value is false.
        /// </summary>
        [DefaultValue(false), Category("Layout"), Description("Indicates whether ButtonItem buttons when in vertical layout are fit into the available width so any text inside of them is wrapped if needed.")]
        public bool FitButtonsToContainerWidth
        {
            get { return ItemContainer.FitOversizeItemIntoAvailableWidth; }
            set
            {
                ItemContainer.FitOversizeItemIntoAvailableWidth = value;
                if (this.DesignMode)
                    RecalcLayout();
            }
        }


        /// <summary>
        /// Gets or sets the item alignment when container is in horizontal layout. Default value is Left.
        /// </summary>
        [Browsable(true), DefaultValue(eHorizontalItemsAlignment.Left), Category("Layout"), Description("Indicates item alignment when container is in horizontal layout."), DevCoBrowsable(true)]
        public eHorizontalItemsAlignment HorizontalItemAlignment
        {
            get { return ItemContainer.HorizontalItemAlignment; }
            set
            {
                ItemContainer.HorizontalItemAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets whether items in horizontal layout are wrapped into the new line when they cannot fit allotted container size. Default value is false.
        /// </summary>
        [Browsable(true), DefaultValue(false), Category("Layout"), Description("Indicates whether items in horizontal layout are wrapped into the new line when they cannot fit allotted container size.")]
        public virtual bool MultiLine
        {
            get { return ItemContainer.MultiLine; }
            set
            {
                ItemContainer.MultiLine = value;
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
                return m_ItemContainer.SubItems;
            }
        }

        /// <summary>
        /// Indicates whether block elements inside of container when aligned center or right will reserve the space to the left. Default value is true.
        /// </summary>
        [DefaultValue(true), Category("Indicates whether block elements inside of container (affects span or div for example) when aligned center or right will reserve the space to the left.")]
        public bool ReserveLeftSpace
        {
            get { return ItemContainer.ReserveLeftSpace; }
            set { ItemContainer.ReserveLeftSpace = value; }
        }
        #endregion

        #region Binding and Templating Support
        /// <summary>
        /// Gets or sets the index specifying the currently selected item.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("Gets or sets the index specifying the currently selected item.")]
        public override int SelectedIndex
        {
            get
            {
                return _SelectedIndex;
            }
            set
            {
                if (value != _SelectedIndex)
                {
                    if (value == -1)
                    {
                        _SelectedIndex = -1;
                        SetItemSelection(this.SelectedItem, false);
                    }
                    else
                    {
                        BaseItem item = null;
                        if (_SelectedIndex >= this.Items.Count)
                        {
                            _SelectedIndex = -1;
                            return;
                        }
                        if (_SelectedIndex > -1)
                            item = this.Items[_SelectedIndex];
                        SetItemSelection(this.SelectedItem, false);
                        SetItemSelection(item, true);
                        _SelectedIndex = value;
                    }
                    OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }

        protected override void OnButtonCheckedChanged(ButtonItem item, EventArgs e)
        {
            if (item.Checked)
            {
                _SelectedIndex = this.Items.IndexOf(item);
                OnSelectedIndexChanged(e);
            }
            base.OnButtonCheckedChanged(item, e);
        }

        /// <summary>
        /// Adds new item to the ItemPanel based on specified ItemTemplate and sets its Text property.
        /// </summary>
        /// <param name="text">Text to assign to the item.</param>
        /// <returns>reference to newly created item</returns>
        public BaseItem AddItem(string text)
        {
            BaseItem template = GetItemTemplate();
            if (template == null)
                throw new NullReferenceException("ItemTemplate property not set.");
            BaseItem item = template.Copy();
            item.Text = text;
            this.Items.Add(item);

            return item;
        }

        /// <summary>
        /// Gets the list of ButtonItem or CheckBoxItem controls that have their Checked property set to true.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Collections.Generic.List<BaseItem> SelectedItems
        {
            get
            {
                System.Collections.Generic.List<BaseItem> items = new System.Collections.Generic.List<BaseItem>();
                SubItemsCollection itemsCollection = this.Items;
                GetSelectedItems(items, itemsCollection);
                return items;
            }
        }

        private static void GetSelectedItems(System.Collections.Generic.List<BaseItem> items, SubItemsCollection itemsCollection)
        {
            foreach (BaseItem item in itemsCollection)
            {
                if (item is ItemContainer)
                {
                    GetSelectedItems(items, item.SubItems);
                    continue;
                }
                ButtonItem button = item as ButtonItem;
                if (button != null && button.Checked)
                    items.Add(button);
                else
                {
                    CheckBoxItem cb = item as CheckBoxItem;
                    if (cb != null && cb.Checked)
                        items.Add(cb);
                    else if (item is DevComponents.DotNetBar.Metro.MetroTileItem && ((DevComponents.DotNetBar.Metro.MetroTileItem)item).Checked)
                        items.Add(item);

                }
            }
        }
        /// <summary>
        /// Gets or sets ButtonItem or CheckBoxItem item that have their Checked property set to true.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BaseItem SelectedItem
        {
            get
            {
                int i = this.SelectedIndex;
                if (i == -1) return null;
                return this.Items[i];
            }
            set
            {
                SetItemSelection(this.SelectedItem, false);
                SetItemSelection(value, true);
                if (value != null)
                    _SelectedIndex = this.Items.IndexOf(value);
                else
                    _SelectedIndex = -1;
            }
        }
        private bool SetItemSelection(BaseItem item, bool isSelected)
        {
            if (item == null) return false;
            bool isSet = true;
            if (item is CheckBoxItem)
                ((CheckBoxItem)item).Checked = isSelected;
            else if (item is ButtonItem)
                ((ButtonItem)item).Checked = isSelected;
            else if (item is DevComponents.DotNetBar.Metro.MetroTileItem)
                ((DevComponents.DotNetBar.Metro.MetroTileItem)item).Checked = isSelected;
            else
                isSet = false;

            return isSet;
        }
        #endregion
    }
}
