using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using DevComponents.DotNetBar.Primitives;
using System.Drawing.Design;

namespace DevComponents.DotNetBar
{
    [ToolboxItem(true), Designer("DevComponents.DotNetBar.Design.ListBoxAdvDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf"), System.Runtime.InteropServices.ComVisible(false)]
    [ToolboxBitmap(typeof(ToolboxIconResFinder), "ListBoxAdv.ico")]
    public class ListBoxAdv : ItemPanelBase
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ListBoxAdv class.
        /// </summary>
        public ListBoxAdv()
        {
            _SelectedItems = new CustomCollection<ListBoxItem>();
            m_ItemContainer.LayoutOrientation = eOrientation.Vertical;
            EnsureBindingGenerator();
            ItemVisualGenerator generator = ItemGenerator;
            generator.CustomCollectionBinding = true;
            generator.DataSource = _Items;
        }
        #endregion

        #region Implementation
        private string _CheckStateMember;
        private CustomCollection<object> _Items = new CustomCollection<object>();
        /// <summary>
        /// Gets the list of items displayed in list box.
        /// </summary>
        [Category("Appearance"), Description("List of items displayed in list box."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false), Localizable(true)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version= 2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public CustomCollection<object> Items
        {
            get { return _Items; }
        }
        /// <summary>
        /// Gets the collection of view items which are created to represent each data item in Items collection.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList Views
        {
            get { return  GetItemsCollection();}
        }

        protected override void OnDataSourceChanged(object newDataSource)
        {
            ClearDescriptorsCache();
            EnsureBindingGenerator();

            ItemVisualGenerator generator = ItemGenerator;
            if (newDataSource == null && generator.CustomCollectionBinding) return; // Nothing changed

            if (newDataSource == null)
            {
                generator.CustomCollectionBinding = true;
                generator.DataSource = _Items;
            }
            else
            {
                generator.CustomCollectionBinding = false;
                generator.DataSource = newDataSource;
            }
            if (generator.DataSource != newDataSource)
            {
                generator.DataSource = newDataSource;
                OnDataSourceChanged(EventArgs.Empty);
            }
        }

        // Fields...
        private bool _CheckBoxesVisible = false;
        /// <summary>
        /// Indicates whether check-boxes are visible inside list box items.
        /// </summary>
        [DefaultValue(false), Category("Appearance"), Description("Indicates whether check-boxes are visible inside list box items.")]
        public bool CheckBoxesVisible
        {
            get { return _CheckBoxesVisible; }
            set
            {
                if (value != _CheckBoxesVisible)
                {
                    bool oldValue = _CheckBoxesVisible;
                    _CheckBoxesVisible = value;
                    OnCheckBoxesVisibleChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when CheckBoxesVisible property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnCheckBoxesVisibleChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("CheckBoxesVisible"));
            m_ItemContainer.NeedRecalcSize = true;
            this.RecalcLayout();
        }

        private int _ItemHeight = 20;
        /// <summary>
        /// Indicates fixed item height. Maybe set to 0 to indicate that each item should be sized based on its content.
        /// </summary>
        [DefaultValue(20), Category("Appearance"), Description("Indicates fixed item height. Maybe set to 0 to indicate that each item should be sized based on its content.")]
        public int ItemHeight
        {
            get { return _ItemHeight; }
            set
            {
                if (value != _ItemHeight)
                {
                    int oldValue = _ItemHeight;
                    _ItemHeight = value;
                    OnItemHeightChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when ItemHeight property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnItemHeightChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("ItemHeight"));

        }

        protected override BaseItem GetDefaultItemTemplate()
        {
            ListBoxItem item = new ListBoxItem();
            return item;
        }

        private eSelectionMode _SelectionMode = eSelectionMode.One;
        /// <summary>
        /// Specifies the selection mode used by the control.
        /// </summary>
        [DefaultValue(eSelectionMode.One), Category("Behavior"), Description("Specifies the selection mode used by the control.")]
        public eSelectionMode SelectionMode
        {
            get { return _SelectionMode; }
            set
            {
                if (value != _SelectionMode)
                {
                    eSelectionMode oldValue = _SelectionMode;
                    _SelectionMode = value;
                    OnSelectionModeChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when SelectionMode property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnSelectionModeChanged(eSelectionMode oldValue, eSelectionMode newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("SelectionMode"));
        }

        protected override void OnItemAdded(BaseItem item, EventArgs e)
        {
            ListBoxItem listItem = item as ListBoxItem;
            bool selectedItemsChanged = false;
            if (listItem != null)
            {
                if (listItem.IsSelected)
                {
                    if (_SelectionMode == eSelectionMode.One)
                    {
                        if (_SelectedItems.Count > 0)
                        {
                            ListBoxItem[] selItems = new ListBoxItem[_SelectedItems.Count];
                            _SelectedItems.CopyTo(selItems);
                            foreach (ListBoxItem selItem in selItems)
                            {
                                selItem.IsSelected = false;
                            }
                        }
                        _SelectedItems.Add(listItem);
                        UpdateSelectedIndex();
                        selectedItemsChanged = true;
                    }
                    else if (_SelectionMode == eSelectionMode.MultiSimple)
                    {
                        _SelectedItems.Add(listItem);
                        UpdateSelectedIndex();
                        selectedItemsChanged = true;
                    }
                }
            }

            if (selectedItemsChanged)
                RaiseSelectionChangedEvents();

            base.OnItemAdded(item, e);
        }

        private void RaiseSelectionChangedEvents()
        {
            OnSelectedIndexChanged(EventArgs.Empty);
        }

        private void UpdateSelectedIndex()
        {
            if (_SelectedItems.Count == 0)
                _SelectedIndex = -1;
            else
            {
                ListBoxItem item = _SelectedItems[_SelectedItems.Count - 1];
                _SelectedIndex = m_ItemContainer.SubItems.IndexOf(item);
            }
        }
        protected override void OnItemRemoved(BaseItem item, ItemRemovedEventArgs e)
        {
            ListBoxItem listItem = item as ListBoxItem;
            bool selectedItemsChanged = false;
            if (listItem != null)
            {
                if (listItem.IsSelected)
                {
                    if (_SelectedItems.Contains(listItem))
                    {
                        _SelectedItems.Remove(listItem);
                        UpdateSelectedIndex();
                        selectedItemsChanged = true;
                    }
                }
            }

            if (selectedItemsChanged)
                RaiseSelectionChangedEvents();

            base.OnItemRemoved(item, e);
        }

        private bool _HandlingSelectedChanged = false;
        internal void OnListBoxItemSelectedChanged(ListBoxItem item, eEventSource source)
        {
            if (_HandlingSelectedChanged) return;
            _HandlingSelectedChanged = true;

            bool selectedItemsChanged = false;
            if (item.IsSelected)
            {
                if (_SelectionMode == eSelectionMode.One && _SelectedItems.Count > 0)
                {
                    ClearSelectedItems(source);
                }
                if (!_SelectedItems.Contains(item))
                {
                    _SelectedItems.Add(item);
                    selectedItemsChanged = true;
                    UpdateSelectedIndex();
                }
            }
            else
            {
                if (_SelectedItems.Contains(item))
                {
                    _SelectedItems.Remove(item);
                    selectedItemsChanged = true;
                    UpdateSelectedIndex();
                }
            }

            if (selectedItemsChanged)
            {
                this.Invalidate();
                RaiseSelectionChangedEvents();
            }

            _HandlingSelectedChanged = false;
        }

        internal void ClearSelectedItems(eEventSource source)
        {
            if (_SelectedItems.Count == 0) return;
            ListBoxItem[] selItems = new ListBoxItem[_SelectedItems.Count];
            _SelectedItems.CopyTo(selItems);
            foreach (ListBoxItem selItem in selItems)
            {
                selItem.SetIsSelected(false, source);
            }
            _SelectedItems.Clear();
        }

        /// <summary>
        /// Selects or clears the selection for the specified item in a ListBoxAdv. Use this property to set the selection of items in a multiple-selection ListBoxAdv. To select an item in a single-selection ListBox, use the SelectedIndex property.
        /// </summary>
        /// <param name="index">The zero-based index of the item in a ListBox to select or clear the selection for. You can pass -1 to clear the selection.</param>
        /// <param name="value">true to select the specified item; otherwise, false.</param>
        public void SetSelected(int index, bool value)
        {
            SetSelected(index, value, eEventSource.Code);
        }

        /// <summary>
        /// Selects or clears the selection for the specified item in a ListBoxAdv. Use this property to set the selection of items in a multiple-selection ListBoxAdv. To select an item in a single-selection ListBox, use the SelectedIndex property.
        /// </summary>
        /// <param name="index">The zero-based index of the item in a ListBox to select or clear the selection for. You can pass -1 to clear the selection.</param>
        /// <param name="value">true to select the specified item; otherwise, false.</param>
        /// <param name="source">Indicates the source of the change.</param>
        public void SetSelected(int index, bool value, eEventSource source)
        {
            if (index == -1)
            {
                ClearSelectedItems(source);
                return;
            }
            ListBoxItem item = m_ItemContainer.SubItems[index] as ListBoxItem;
            if (item != null)
                item.SetIsSelected(value, source);
        }

        /// <summary>
        /// Gets or sets the zero-based index of the currently selected item in a ListBoxAdv. Negative one (-1) is returned if no item is selected.
        /// </summary>
        [Bindable(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override int SelectedIndex
        {
            get
            {
                return _SelectedIndex;
            }
            set
            {
                if (_SelectedItems.Count > 0) ClearSelectedItems(eEventSource.Code);
                SetSelected(value, true);
            }
        }

        /// <summary>
        /// Selects next visible list box item.
        /// </summary>
        public void SelectNextItem()
        {
            int startIndex = this.SelectedIndex == -1 ? 0 : this.SelectedIndex + 1;
            if (startIndex >= m_ItemContainer.SubItems.Count)
                return;
            for (int i = startIndex; i < m_ItemContainer.SubItems.Count; i++)
            {
                ListBoxItem item = m_ItemContainer.SubItems[i] as ListBoxItem;
                if (item == null) continue;
                if (item.Visible)
                {
                    item.IsSelected = true;
                    EnsureVisible(item);
                    break;
                }
            }
        }

        /// <summary>
        /// Selects previous visible list box item.
        /// </summary>
        public void SelectPreviousItem()
        {
            int startIndex = this.SelectedIndex == -1 ? 0 : this.SelectedIndex - 1;
            if (startIndex < 0)
                return;
            for (int i = startIndex; i >= 0; i--)
            {
                ListBoxItem item = m_ItemContainer.SubItems[i] as ListBoxItem;
                if (item == null) continue;
                if (item.Visible)
                {
                    item.IsSelected = true;
                    EnsureVisible(item);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets a collection that contains the zero-based indexes of all currently selected items in the ListBoxAdv. Do not modify items in this collection. To select or deselect list items while in multi-selection mode use SetSelected method.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<int> SelectedIndices
        {
            get
            {
                return GetSelectedIndices();
            }
        }

        private List<int> GetSelectedIndices()
        {
            List<int> list = new List<int>();
            if (_SelectedItems.Count == 0) return list;
            foreach (ListBoxItem item in _SelectedItems)
            {
                list.Add(m_ItemContainer.SubItems.IndexOf(item));
            }

            return list;
        }

        /// <summary>
        /// Gets a collection containing the currently selected items in the ListBoxAdv. Do not modify items in this collection. To select or deselect list items while in multi-selection mode use SetSelected method.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CustomCollection<ListBoxItem> SelectedItems
        {
            get
            {
                return _SelectedItems;
            }
        }

        private CustomCollection<ListBoxItem> _SelectedItems;
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedItem
        {
            get
            {
                if (_SelectedItems.Count == 0) return null;
                ListBoxItem item = _SelectedItems[0];
                if (item.Tag is ItemBindingData)
                    return ((ItemBindingData)item.Tag).DataItem;
                return item;
            }
            set
            {
                if (value == null)
                {
                    foreach (ListBoxItem item in _SelectedItems)
                    {
                        item.SetIsSelected(false, eEventSource.Code);
                    }
                }
                else
                {
                    ListBoxItem item = FindListBoxItem(value);
                    if (item == null)
                        throw new InvalidOperationException("SelectedValue set cannot be found in Items collection");
                    item.SetIsSelected(true, eEventSource.Code);
                }
            }
        }

        /// <summary>
        /// Gets ListBoxItem which represents an object value from Items collection when Items collection contains non ListBoxItem objects.
        /// </summary>
        /// <param name="value">Value to get ListBoxItem for.</param>
        /// <returns></returns>
        public ListBoxItem FindListBoxItem(object value)
        {
            if (value is ListBoxItem)
                return (ListBoxItem)value;
            foreach (BaseItem visual in m_ItemContainer.SubItems)
            {
                ListBoxItem item = visual as ListBoxItem;
                if (item != null && item.Tag is ItemBindingData && ((ItemBindingData)item.Tag).DataItem == value)
                    return item;
            }
            return null;
        }

        private ListBoxItem _FocusItem = null;
        protected override void OnKeyboardItemSelected(BaseItem item)
        {
            base.OnKeyboardItemSelected(item);
            if (item is ListBoxItem)
            {
                ListBoxItem listItem = (ListBoxItem)item;
                if (_SelectionMode == eSelectionMode.One)
                {
                    listItem.SetIsSelected(true, eEventSource.Keyboard);
                }
                else
                {
                    if (_SelectionMode == eSelectionMode.MultiExtended)
                    {
                        if ((Control.ModifierKeys & Keys.Shift) == Keys.None)
                        {
                            ClearSelectedItems(eEventSource.Keyboard);
                            listItem.SetIsSelected(true, eEventSource.Keyboard);
                        }
                        else
                        {
                            if (listItem.IsSelected && _FocusItem != null && _FocusItem.IsSelected)
                            {
                                _FocusItem.SetIsSelected(false, eEventSource.Keyboard);
                            }
                            else
                            {
                                listItem.SetIsSelected(true, eEventSource.Keyboard);
                            }
                        }
                    }
                    if (_FocusItem != null) _FocusItem.OnLostFocus();
                    _FocusItem = listItem;
                    _FocusItem.OnGotFocus();
                }
            }
            else
            {
                if (_FocusItem != null) _FocusItem.OnLostFocus();
                _FocusItem = null;
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (_FocusItem != null) _FocusItem.OnLostFocus();
            _FocusItem = null;
            base.OnLostFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && _SelectionMode == eSelectionMode.MultiSimple && _FocusItem != null)
            {
                _FocusItem.SetIsSelected(!_FocusItem.IsSelected, eEventSource.Keyboard);
            }
            base.OnKeyDown(e);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & (Keys.Control | Keys.Alt)) == Keys.None)
            {
                Keys keys = keyData & Keys.KeyCode;
                if (keys == Keys.Left || keys == Keys.Right || keys == Keys.Up || keys == Keys.Down)
                {
                    ExKeyDown(new KeyEventArgs(keys));
                    return true;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        private string _DisplayMember = "";
        /// <summary>
        /// Indicates property name which will provide the value to display for this list box when data-binding is used and DataSource is set.
        /// </summary>
        [DefaultValue(""), Category("Data"), Description("Indicates property name which will provide the value to display for this list box when data-binding is used and DataSource is set.")]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version= 2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version= 2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string DisplayMember
        {
            get { return _DisplayMember; }
            set
            {
                if (value == null) value = "";
                if (value != _DisplayMember)
                {
                    string oldValue = _DisplayMember;
                    _DisplayMember = value;
                    OnDisplayMemberChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when DisplayMember property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnDisplayMemberChanged(string oldValue, string newValue)
        {
            EnsureBindingGenerator();
            ItemVisualGenerator generator = ItemGenerator;

            foreach (BindingDef item in generator.Bindings)
            {
                if (item.PropertyName == "Text")
                {
                    generator.Bindings.Remove(item);
                    break;
                }
            }

            if (!string.IsNullOrEmpty(newValue))
            {
                BindingDef bindDef = new BindingDef("Text", newValue);
                generator.Bindings.Add(bindDef);
            }
            generator.RefreshItems();
            //OnPropertyChanged(new PropertyChangedEventArgs("DisplayMember"));
        }

        /// <summary>
        /// Indicates property name which will provide the value for CheckState of the list box item when data-binding is used and DataSource is set.
        /// </summary>
        [DefaultValue(""), Category("Data"), Description("Indicates property name which will provide the value for CheckState of the list box item when data-binding is used and DataSource is set.")]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version= 2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version= 2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string CheckStateMember
        {
            get { return _CheckStateMember; }
            set
            {
                if (value == null) value = "";
                if (value != _CheckStateMember)
                {
                    string oldValue = _CheckStateMember;
                    _CheckStateMember = value;
                    OnCheckStateMemberChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when CheckStateMember property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnCheckStateMemberChanged(string oldValue, string newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("CheckStateMember"));
            EnsureBindingGenerator();
            ItemVisualGenerator generator = ItemGenerator;

            foreach (BindingDef item in generator.Bindings)
            {
                if (item.PropertyName == "CheckState")
                {
                    item.Format -= CheckStateBindingFormat;
                    generator.Bindings.Remove(item);
                    break;
                }
            }

            if (!string.IsNullOrEmpty(newValue))
            {
                BindingDef bindDef = new BindingDef("CheckState", newValue);
                bindDef.FormattingEnabled = true;
                bindDef.Format += CheckStateBindingFormat;
                generator.Bindings.Add(bindDef);
            }
            generator.RefreshItems();
        }

        /// <summary>
        /// Occurs when using data-binding with CheckStateMember specified and it allows you to convert a property value to CheckState.
        /// </summary>
        [Description("Occurs when using data-binding with CheckStateMember specified and it allows you to convert a property value to CheckState.")]
        public event CheckStateConvertEventHandler CheckStateConvert;
        /// <summary>
        /// Raises CheckStateConvert event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnCheckStateConvert(CheckStateConvertEventArgs e)
        {
            CheckStateConvertEventHandler handler = CheckStateConvert;
            if (handler != null)
                handler(this, e);
        }

        private void CheckStateBindingFormat(object sender, DataConvertEventArgs e)
        {
            if (CheckStateConvert != null)
            {
                CheckStateConvertEventArgs cea = new CheckStateConvertEventArgs(e.Value);
                OnCheckStateConvert(cea);
                if (cea.CheckState != null)
                {
                    e.Value = cea.CheckState.Value;
                    return;
                }
            }
            e.Value = GetCheckState(e.Value);
        }

        private CheckState GetCheckState(object value)
        {
            if (value is System.Data.SqlTypes.SqlBoolean)
            {
                System.Data.SqlTypes.SqlBoolean sqlBool = (System.Data.SqlTypes.SqlBoolean)value;
                if (sqlBool.IsTrue)
                    return CheckState.Checked;
                else if (sqlBool.IsNull)
                    return CheckState.Indeterminate;
                else
                    return CheckState.Unchecked;
            }
            else if (value is int?)
            {
                if (value == null) return CheckState.Indeterminate;
                if (((int?)value).Value == 0)
                    return CheckState.Unchecked;
                return CheckState.Checked;
            }
            else if (value is int)
            {
                if ((int)value == -1)
                    return CheckState.Indeterminate;
                return ((int)value) == 0 ? CheckState.Unchecked : CheckState.Checked;
            }
            else if (value is long)
            {
                if ((long)value == -1)
                    return CheckState.Indeterminate;
                return ((long)value) == 0 ? CheckState.Unchecked : CheckState.Checked;
            }
            else if (value is short)
            {
                if ((short)value == -1)
                    return CheckState.Indeterminate;
                return ((short)value) == 0 ? CheckState.Unchecked : CheckState.Checked;
            }
            else if (value is float)
            {
                if ((float)value == -1)
                    return CheckState.Indeterminate;
                return ((float)value) == 0 ? CheckState.Unchecked : CheckState.Checked;
            }
            else if (value is double)
            {
                if ((double)value == -1)
                    return CheckState.Indeterminate;
                return ((double)value) == 0 ? CheckState.Unchecked : CheckState.Checked;
            }
            else if (value is byte)
            {
                return ((byte)value) == 0 ? CheckState.Unchecked : CheckState.Checked;
            }
            else if (value is uint)
            {
                return ((uint)value) == 0 ? CheckState.Unchecked : CheckState.Checked;
            }
            else if (value is ulong)
            {
                return ((ulong)value) == 0 ? CheckState.Unchecked : CheckState.Checked;
            }
            else if (value is bool)
            {
                return ((bool)value) == false ? CheckState.Unchecked : CheckState.Checked;
            }

            return CheckState.Indeterminate;
        }

        private bool IsNull(object value)
        {
            return value == null ||
                value == DBNull.Value;
        }

        /// <summary>
        /// Returns the enumerator with selected values if any
        /// </summary>
        [Browsable(false)]
        public IEnumerable<object> SelectedValues
        {
            get
            {
                List<int> selectedIndexes = SelectedIndices;
                foreach (int index in selectedIndexes)
                {
                    yield return GetValueByIndex(index);
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the member property specified by the ValueMember property. If ValueMember specifies property that cannot be found on selected object this property returns null.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedValue
        {
            get
            {
                if (this.DataSource == null)
                    return null;

                int selIndex = this.SelectedIndex;
                return GetValueByIndex(selIndex);
            }
            set
            {
                if (string.IsNullOrEmpty(_ValueMember))
                    throw new InvalidOperationException("ValueMember property not set");
                int index = FindValueMemberIndexOf(value);
                if (index < 0)
                    throw new InvalidPropertyValueException("value not found.");

                this.SelectedItem = m_ItemContainer.SubItems[index];
            }
        }

        private object GetValueByIndex(int selIndex)
        {
            if (selIndex < 0) return null;
            ItemVisualGenerator generator = ItemGenerator;
            if (generator.DataManager == null || generator.DataManager.List == null)
                return null;

            object item = generator.DataManager.List[selIndex];

            return GetPropertyValue(item, _ValueMember);
        }
        private int FindValueMemberIndexOf(object value)
        {
            ItemVisualGenerator generator = ItemGenerator;
            if (generator.DataManager == null || generator.DataManager.List == null)
                return -1;
            IList list = generator.DataManager.List;
            for (int i = 0; i < list.Count; i++)
            {
                object itemValue = GetPropertyValue(list[i], _ValueMember);
                if (itemValue.Equals(value))
                    return i;
            }
            return -1;
        }
        private string _ValueMember = "";
        /// <summary>
        /// Indicates property name which will return the value for the selected item when SelectedValue property is accessed.
        /// </summary>
        [Description("Indicates property name which will return the value for the selected item when SelectedValue property is accessed.")]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version= 2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Category("Data")]
        public string ValueMember
        {
            get { return _ValueMember; }
            set
            {
                if (value == null) value = "";
                if (value != _ValueMember)
                {
                    string oldValue = _ValueMember;
                    _ValueMember = value;
                    OnValueMemberChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when ValueMember property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnValueMemberChanged(string oldValue, string newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("ValueMember"));   
        }

        Dictionary<string, PropertyDescriptor> _DescriptorsCache = new Dictionary<string, PropertyDescriptor>();
        /// <summary>
        /// Clears internal property descriptors cache when data-binding is used. In most cases it is not needed that you call this method. Do so only if instructed by DevComponents support.
        /// </summary>
        public void ClearDescriptorsCache()
        {
            _DescriptorsCache.Clear();
        }
        protected object GetPropertyValue(object item, string fieldName)
        {
            if ((item != null) && (fieldName.Length > 0))
            {
                try
                {
                    PropertyDescriptor descriptor = null;
                    string key = item.GetType().ToString() + ":" + fieldName;
                    if (!_DescriptorsCache.TryGetValue(key, out descriptor))
                    {
                        ItemVisualGenerator generator = ItemGenerator;
                        if (generator.DataManager != null)
                        {
                            descriptor = generator.DataManager.GetItemProperties().Find(fieldName, true);
                        }
                        else
                        {
                            descriptor = TypeDescriptor.GetProperties(item).Find(fieldName, true);
                        }
                        if (descriptor != null)
                            _DescriptorsCache.Add(key, descriptor);
                    }
                    if (descriptor != null)
                    {
                        item = descriptor.GetValue(item);
                    }
                }
                catch
                {
                }
            }
            return item;
        }

        /// <summary>
        /// Finds the first item in the ListBox that starts with the specified string. The search starts at a specific starting index.
        /// </summary>
        /// <param name="s">The text to search for.</param>
        /// <returns>The zero-based index of the first item found; returns ListBox.NoMatches if no match is found.</returns>
        public int FindString(string s)
        {
            return FindString(s, 0);
        }

        /// <summary>
        /// Finds the first item in the ListBox that starts with the specified string. The search starts at a specific starting index.
        /// </summary>
        /// <param name="s">The text to search for.</param>
        /// <param name="startIndex">The zero-based index of the item before the first item to be searched. Set to negative one (-1) to search from the beginning of the control. </param>
        /// <returns>The zero-based index of the first item found; returns ListBox.NoMatches if no match is found.</returns>
        public int FindString(string s, int startIndex)
        {
            if (startIndex < 0)
                startIndex = 0;

            for (int i = startIndex; i < m_ItemContainer.SubItems.Count; i++)
            {
                if (m_ItemContainer.SubItems[i].Text.StartsWith(s))
                    return i;
            }

            return ListBox.NoMatches;
        }

        /// <summary>
        /// Finds the first item in the ListBox that exactly matches the specified string. The search starts at a specific starting index.
        /// </summary>
        /// <param name="s">The text to search for.</param>
        /// <returns>The zero-based index of the first item found; returns ListBox.NoMatches if no match is found.</returns>
        public int FindStringExact(string s)
        {
            return FindStringExact(s, 0);
        }

        /// <summary>
        /// Finds the first item in the ListBox that exactly matches the specified string. The search starts at a specific starting index.
        /// </summary>
        /// <param name="s">The text to search for.</param>
        /// <param name="startIndex">The zero-based index of the item before the first item to be searched. Set to negative one (-1) to search from the beginning of the control. </param>
        /// <returns>The zero-based index of the first item found; returns ListBox.NoMatches if no match is found.</returns>
        public int FindStringExact(string s, int startIndex)
        {
            if (startIndex < 0)
                startIndex = 0;

            for (int i = startIndex; i < m_ItemContainer.SubItems.Count; i++)
            {
                if (m_ItemContainer.SubItems[i].Text == s)
                    return i;
            }

            return ListBox.NoMatches;
        }

        internal void ListItemCheckStateChanged(ListBoxAdvItemCheckEventArgs e)
        {
            OnItemCheck(e);
        }

        /// <summary>
        /// Occurs when CheckState of an ListBoxItem changes and it allows you to cancel the change.
        /// </summary>
        [Description("Occurs when CheckState of an ListBoxItem changes and it allows you to cancel the change.")]
        public event ListBoxAdvItemCheckEventHandler ItemCheck;
        /// <summary>
        /// Raises ItemCheck event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnItemCheck(ListBoxAdvItemCheckEventArgs e)
        {
            ListBoxAdvItemCheckEventHandler handler = ItemCheck;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Gets collection of checked items in the list. Modifying this collection does not have any effect on actual checked items. Use SetItemCheckState to set checked state of an item or access ListBoxItem directly and set its CheckState property.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ListBoxItem> CheckedItems
        {
            get
            {
                List<ListBoxItem> items = new List<ListBoxItem>();
                foreach (BaseItem item in m_ItemContainer.SubItems)
                {
                    ListBoxItem listItem = item as ListBoxItem;
                    if (listItem == null) continue;
                    if (listItem.CheckState != CheckState.Unchecked)
                        items.Add(listItem);
                }
                return items;
            }
        }

        /// <summary>
        /// Sets the check state of the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to set the state for. </param>
        /// <param name="value">One of the CheckState values. </param>
        public void SetItemCheckState(int index, CheckState value)
        {
            ListBoxItem item = m_ItemContainer.SubItems[index] as ListBoxItem;
            item.CheckState = value;
        }

        private bool _UseMnemonic = false;
        /// <summary>
        /// Gets or sets a value indicating whether the items interprets an ampersand character (&) in the control's Text property to be an access key prefix character.
        /// </summary>
        [Browsable(true), DefaultValue(false), Category("Appearance"), Description("Indicates whether the items interprets an ampersand character (&) in the control's Text property to be an access key prefix character.")]
        public bool UseMnemonic
        {
            get { return _UseMnemonic; }
            set
            {
                _UseMnemonic = value;
                m_ItemContainer.NeedRecalcSize = true;
            }
        }
        #endregion
    }



    /// <summary>
    /// Defines selection modes for list control.
    /// </summary>
    public enum eSelectionMode
    {
        /// <summary>
        /// Items cannot be selected.
        /// </summary>
        None,
        /// <summary>
        /// Only one item at the time can be selected.
        /// </summary>
        One,
        /// <summary>
        /// Multiple items can be selected.  A mouse click or pressing the SPACEBAR selects or deselects an item in the list.
        /// </summary>
        MultiSimple,
        /// <summary>
        /// Multiple items can be selected. Pressing SHIFT and clicking the mouse or pressing SHIFT and one of the arrow keys (UP ARROW, DOWN ARROW, LEFT ARROW, and RIGHT ARROW) extends the selection from the previously selected item to the current item. Pressing CTRL and clicking the mouse selects or deselects an item in the list. 
        /// </summary>
        MultiExtended
    }

    public delegate void CheckStateConvertEventHandler(object sender, CheckStateConvertEventArgs ea);
}
