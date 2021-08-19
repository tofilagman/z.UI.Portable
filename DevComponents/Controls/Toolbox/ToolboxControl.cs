using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevComponents.AdvTree;
using DevComponents.DotNetBar.Primitives;
using DevComponents.DotNetBar.Rendering;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Represents Toolbox control to create.
    /// </summary>
    [ToolboxBitmap(typeof(ToolboxControl), "Controls.ToolboxControl.bmp"), ToolboxItem(true), Designer("DevComponents.DotNetBar.Design.ToolboxControlDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf"), System.Runtime.InteropServices.ComVisible(false)]
    public class ToolboxControl : ContainerControl
    {
        #region Implementation

        private ItemPanel _ItemPanel = null;
        private Bar _TitleBar = null;
        private TextBoxX _SearchBox = null;
        private Bar _MenuBar = null;

        public ToolboxControl()
        {
            _BackgroundStyle.StyleChanged += new EventHandler(this.VisualPropertyChanged);
            _BackgroundStyle.Class = ElementStyleClassKeys.ToolboxControlKey;
            this.Padding = new System.Windows.Forms.Padding(2);

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque |
            ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer | ControlStyles.Selectable, true);

            _ItemPanel = CreateItemsPanel();
            this.Controls.Add(_ItemPanel);

            _MenuBar = CreateMenuBar();
            this.Controls.Add(_MenuBar);

            _SearchBox = CreateSearchBox();
            this.Controls.Add(_SearchBox);

            _TitleBar = CreateTitleBar();
            this.Controls.Add(_TitleBar);

            _SelectedItems = new CustomCollection<ToolboxItem>();

            _Groups.CollectionChanged += GroupsCollectionChanged;
        }

        private void GroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int index = e.NewStartingIndex;
                foreach (ToolboxGroup group in e.NewItems)
                {
                    _ItemPanel.Items.Insert(index, group);
                    index++;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ToolboxGroup group in e.OldItems)
                    _ItemPanel.Items.Remove(group);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _ItemPanel.Items.Clear();
                if (e.NewItems != null)
                {
                    foreach (ToolboxGroup group in e.NewItems)
                        _ItemPanel.Items.Add(group);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                _ItemPanel.Items.Remove(e.NewStartingIndex);
                _ItemPanel.Items.Insert(e.NewStartingIndex, (BaseItem)e.NewItems[0]);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private ButtonItem _MenuButton = null;
        private Bar CreateMenuBar()
        {
            Bar menuBar = new Bar();
            menuBar.Name = "menuBar";
            menuBar.AntiAlias = true;
            menuBar.BackColor = System.Drawing.Color.Transparent;
            menuBar.Dock = System.Windows.Forms.DockStyle.Top;
            menuBar.Font = new System.Drawing.Font("Segoe UI", 9F);
            menuBar.IsMaximized = false;
            menuBar.Location = new System.Drawing.Point(6, 93);
            menuBar.PaddingBottom = 4;
            menuBar.PaddingTop = 4;
            menuBar.Size = new System.Drawing.Size(365, 61);
            menuBar.Stretch = true;
            menuBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            menuBar.TabIndex = 2;
            menuBar.TabStop = false;

            ButtonItem menuButton = new ButtonItem("menuButton");
            menuButton.AutoExpandOnClick = true;
            menuButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            menuButton.ImagePaddingHorizontal = 30;
            menuButton.ImagePaddingVertical = 12;
            menuButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Right;
            menuButton.PopupSide = DevComponents.DotNetBar.ePopupSide.Right;
            menuButton.Text = "Menu";
            menuButton.SubItems.Add(new ButtonItem()); // Just placeholder
            menuButton.PopupOpen += MenuButtonPopupOpen;
            menuButton.PopupClose += MenuButtonPopupClose;
            _MenuButton = menuButton;

            menuBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            menuButton});

            return menuBar;
        }

        /// <summary>
        /// Gets the Bar control used as menu bar.
        /// </summary>
        [Browsable(false)]
        public Bar MenuBar
        {
            get { return _MenuBar; }
        }

        private void MenuButtonPopupClose(object sender, EventArgs e)
        {
            ClearAndDisposeMenuItems();
            _MenuButton.SubItems.Add(new ButtonItem());
        }

        private void ClearAndDisposeMenuItems()
        {
            foreach (BaseItem item in _MenuButton.SubItems)
            {
                item.Dispose();
            }
            _MenuButton.SubItems.Clear();
        }

        private void MenuButtonPopupOpen(object sender, PopupOpenEventArgs e)
        {
            ClearAndDisposeMenuItems();
            foreach (BaseItem item in this.Groups)
            {
                ToolboxGroup group = item as ToolboxGroup;
                if (group != null)
                {
                    ButtonItem menuItem = new ButtonItem();
                    menuItem.Text = group.TitleText;
                    menuItem.Click += MenuItemClick;
                    menuItem.Tag = group;
                    _MenuButton.SubItems.Add(menuItem);
                }
            }
        }

        private void MenuItemClick(object sender, EventArgs e)
        {
            ToolboxGroup group = ((BaseItem)sender).Tag as ToolboxGroup;
            if (group != null)
                group.Expanded = true;
        }

        private string _SearchBoxWatermark = "Enter text to search...";

        /// <summary>
        /// Indicates the search-box watermark text.
        /// </summary>
        [DefaultValue("Enter text to search..."), Category("Appearance"), Description("Indicates the search-box watermark text."), Localizable(true)]
        public string SearchBoxWatermark
        {
            get { return _SearchBoxWatermark; }
            set
            {
                if (_SearchBoxWatermark != value)
                {
                    string oldValue = value;
                    _SearchBoxWatermark = value;
                    OnSearchBoxWatermarkChanged(oldValue, value);
                }
            }
        }

        protected virtual void OnSearchBoxWatermarkChanged(string oldValue, string newValue)
        {
            _SearchBox.WatermarkText = newValue;
        }

        private TextBoxX CreateSearchBox()
        {
            TextBoxX searchBox = new TextBoxX();
            searchBox.Name = "searchBox";
            searchBox.Border.Class = "TextBoxBorder";
            searchBox.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            searchBox.ButtonCustom.Symbol = "";
            searchBox.ButtonCustom.Visible = true;
            searchBox.ButtonCustomClick += SearchBoxButtonCustomClick;
            searchBox.Dock = System.Windows.Forms.DockStyle.Top;
            searchBox.Location = new System.Drawing.Point(6, 55);
            searchBox.PreventEnterBeep = true;
            searchBox.Size = new System.Drawing.Size(365, 38);
            searchBox.TabIndex = 0;
            searchBox.WatermarkText = "Enter text to search...";
            searchBox.TextChanged += SearchBoxTextChanged;
            searchBox.WordWrap = false;

            return searchBox;
        }
        /// <summary>
        /// Returns reference to internal search text-box.
        /// </summary>
        [Browsable(false)]
        public TextBoxX SearchBoxTextBox
        {
            get { return _SearchBox; }
        }

        void SearchBoxButtonCustomClick(object sender, EventArgs e)
        {
            _SearchBox.Text = string.Empty;
        }

        private void SearchBoxTextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_SearchBox.Text))
                _SearchBox.ButtonCustom.Symbol = "\uf002";
            else
                _SearchBox.ButtonCustom.Symbol = "\uf00d";
            Search(_SearchBox.Text);
        }

        private ButtonItem _ExpandButton = null;
        private LabelItem _TitleLabel = null;
        private Bar CreateTitleBar()
        {
            Bar titleBar = new Bar();
            titleBar.Name = "titleBar";
            titleBar.AntiAlias = true;
            titleBar.BackColor = System.Drawing.Color.Transparent;
            titleBar.Dock = System.Windows.Forms.DockStyle.Top;
            titleBar.IsMaximized = false;
            titleBar.Location = new System.Drawing.Point(6, 6);
            titleBar.PaddingBottom = 4;
            titleBar.PaddingTop = 4;
            titleBar.Size = new System.Drawing.Size(365, 49);
            titleBar.Stretch = true;
            titleBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            titleBar.TabIndex = 0;
            titleBar.TabStop = false;

            LabelItem toolboxLabel = new LabelItem("toolboxLabel");
            toolboxLabel.Text = "Toolbox";
            _TitleLabel = toolboxLabel;

            ButtonItem expandButton = new ButtonItem("expandButton");
            expandButton.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            expandButton.Symbol = "\uf053";
            expandButton.SymbolSize = 10F;
            expandButton.Click += ExpandButtonClick;
            _ExpandButton = expandButton;

            titleBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            toolboxLabel,
            expandButton});

            return titleBar;
        }

        private string _TitleText = "Toolbox";

        /// <summary>
        /// Indicates the title label text.
        /// </summary>
        [DefaultValue("Toolbox"), Category("Appearance"), Description("Indicates the title label text."), Localizable(true)]
        public string TitleText
        {
            get { return _TitleText; }
            set
            {
                if (_TitleText != value)
                {
                    string oldValue = value;
                    _TitleText = value;
                    OnTitleTextChanged(oldValue, value);
                }
            }
        }

        protected virtual void OnTitleTextChanged(string oldValue, string newValue)
        {
            _TitleLabel.Text = newValue;
        }

        /// <summary>
        /// Gets the Bar control used as title bar.
        /// </summary>
        [Browsable(false)]
        public Bar TitleBar
        {
            get { return _TitleBar; }
        }

        private void ExpandButtonClick(object sender, EventArgs e)
        {
            this.Expanded = !this.Expanded;
        }

        private ItemPanel CreateItemsPanel()
        {
            ItemPanel itemPanel = new ItemPanel();
            itemPanel.Dock = DockStyle.Fill;
            itemPanel.LayoutOrientation = eOrientation.Vertical;
            itemPanel.AutoScroll = true;
#if !TRIAL
            itemPanel.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
#endif
            itemPanel.ItemClick += ItemPanelItemClick;
            itemPanel.DragDropSupport = true;
            itemPanel.EnableDragDrop = true;
            itemPanel.UseNativeDragDrop = true;
            itemPanel.AllowDrop = true;
            itemPanel.BeforeItemDrag += ItemsPanelBeforeItemDrag;
            itemPanel.GetBaseItemContainer().SubItemsChanged += ItemsChanged;
            itemPanel.DragDropAllowedContainerTypes.Add(typeof(ToolboxGroup));
            return itemPanel;
        }

        private void ItemsChanged(object sender, CollectionChangeEventArgs e)
        {
            if (!IsUpdateSuspended)
                this.RecalcLayout();
        }

        private int _UpdateSuspended = 0;
        /// <summary>
        /// Disables any redrawing of the tree control. To maintain performance while items
        /// are added one at a time to the control, call the BeginUpdate method. The BeginUpdate
        /// method prevents the control from painting until the
        /// <see cref="EndUpdate">EndUpdate</see> method is called.
        /// </summary>
        public void BeginUpdate()
        {
            _UpdateSuspended++;
        }

        /// <summary>
        /// Enables the redrawing of the tree view. To maintain performance while items are
        /// added one at a time to the control, call the <see cref="BeginUpdate">BeginUpdate</see>
        /// method. The BeginUpdate method prevents the control from painting until the EndUpdate
        /// method is called.
        /// </summary>
        /// <remarks>
        /// Call to EndUpdate will enable the layout and painting in tree control. If there
        /// are any pending layouts the EndUpdate will call
        /// <see cref="RecalcLayout">RecalcLayout</see> method to perform the layout and it will
        /// repaint the control.
        /// </remarks>
        public void EndUpdate()
        {
            EndUpdate(true);
        }

        /// <summary>
        /// Enables the redrawing of the tree view. To maintain performance while items are
        /// added one at a time to the control, call the <see cref="BeginUpdate">BeginUpdate</see>
        /// method. The BeginUpdate method prevents the control from painting until the EndUpdate
        /// method is called.
        /// </summary>
        /// <param name="performLayoutAndRefresh">Gets or sets whether layout and refresh of control is performed if there are no other update blocks pending.</param>
        public void EndUpdate(bool performLayoutAndRefresh)
        {
            if (_UpdateSuspended > 0) _UpdateSuspended--;
            if (_UpdateSuspended == 0 && performLayoutAndRefresh)
            {
                this.RecalcLayout();
                this.Invalidate(true);
            }
        }

        /// <summary>
        /// Gets whether layout is suspended for tree control. Layout is suspended after
        /// call to <see cref="BeginUpdate">BeginUpdate</see> method and it is resumed after the
        /// call to <see cref="EndUpdate">EndUpdate</see> method.
        /// </summary>
        [Browsable(false)]
        public bool IsUpdateSuspended
        {
            get { return (_UpdateSuspended > 0); }
        }

        public void RecalcLayout()
        {
            _ItemPanel.RecalcLayout();
        }

        /// <summary>
        /// Returns reference to internal item panel used to display the toolbox items and groups.
        /// </summary>
        [Browsable(false)]
        public ItemPanel ItemsPanel
        {
            get { return _ItemPanel; }
        }

        private void ItemsPanelBeforeItemDrag(object sender, CancelEventArgs e)
        {
            OnBeforeItemDrag(sender, e);
        }

        /// <summary>
        /// Returns collection of toolbox control groups, collection of ToolboxGroup items.
        /// </summary>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
        //[System.ComponentModel.Editor("DevComponents.DotNetBar.Design.ToolboxControlGroupsEditor, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf", typeof(System.Drawing.Design.UITypeEditor))]
        //public SubItemsCollection Groups
        //{
        //    get { return _ItemPanel.GetBaseItemContainer().SubItems; }
        //}

        private CustomCollection<ToolboxGroup> _Groups = new CustomCollection<ToolboxGroup>();
        /// <summary>
        /// Gets the list of items displayed in list box.
        /// </summary>
        [Category("Appearance"), Description("List of items displayed in list box."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false), Localizable(false)]
        public CustomCollection<ToolboxGroup> Groups
        {
            get { return _Groups; }
        }

        private bool _ExpandSingleGroupOnly = true;

        /// <summary>
        /// Indicates whether single group only is expanded at a time. When new group is expanded currently expanded group is collapsed.
        /// </summary>
        [DefaultValue(true), Category("Behavior"),
         Description("Indicates whether single toolbox group only is expanded at a time")]
        public bool ExpandSingleGroupOnly
        {
            get { return _ExpandSingleGroupOnly; }
            set { _ExpandSingleGroupOnly = value; }
        }

        /// <summary>
        /// Occurs after Expanded property value has changed
        /// </summary>
        [Description("Occurs after Expanded property value has changed")]
        public event EventHandler ExpandedChanged;

        /// <summary>
        /// Raises ExpandedChanged event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnExpandedChanged(EventArgs e)
        {
            EventHandler handler = ExpandedChanged;
            if (handler != null)
                handler(this, e);
        }

        private bool _Expanded = true;
        /// <summary>
        /// Gets or sets whether control is expanded and shows items in full size with image and text. When collapsed
        /// control will show only images for toolbox items and will hide other UI elements to minimize its size.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether control is expanded and shows items in full size with images and text")]
        public bool Expanded
        {
            get { return _Expanded; }
            set
            {
                if (_Expanded != value)
                {
                    _Expanded = value;
                    OnExpandedChanged();
                }
            }
        }

        private Color _CollapsedSeparatorColor = Color.Gray;

        /// <summary>
        /// Indicates the collapsed toolbox separator color, the line drawn between the toolbox and menu/expand buttons above.
        /// </summary>
        [Category("Appearance"), Description("Indicates the collapsed toolbox separator color, the line drawn between the toolbox and menu/expand buttons above.")]
        public Color CollapsedSeparatorColor
        {
            get { return _CollapsedSeparatorColor; }
            set
            {
                _CollapsedSeparatorColor = value;
                if (!_Expanded)
                {
                    _MenuBar.Invalidate();
                    _MenuBar.BorderColors = new Color[] { Color.Empty, Color.Empty, Color.Empty, _CollapsedSeparatorColor };
                }
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeCollapsedSeparatorColor()
        {
            return _CollapsedSeparatorColor != Color.Gray;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetCollapsedSeparatorColor()
        {
            CollapsedSeparatorColor = Color.Gray;
        }

        /// <summary>
        /// Gets or sets the expanded width of the control. This property value is valid after control has been collapsed. 
        /// </summary>
        [Browsable(false)]
        public int ExpandedWidth
        {
            get { return _ExpandedWidth; }
            set { _ExpandedWidth = value; }
        }

        private List<BaseItemAutoSizeBag> _CollapsedList = new List<BaseItemAutoSizeBag>();
        private int _ExpandedWidth = 0;
        protected virtual void OnExpandedChanged()
        {
            OnExpandedChanged(EventArgs.Empty);
            if (!_Expanded)
            {
                _ExpandedWidth = this.Width;
                _ExpandButton.Symbol = "\uf054";
                _MenuButton.ButtonStyle = eButtonStyle.Default;
                _TitleLabel.Visible = false;

                _MenuButton.Symbol = "\ue037";
                _MenuButton.SymbolSet = eSymbolSet.Material;
                _MenuButton.SymbolSize = 10f;
                _MenuButton.ButtonStyle = eButtonStyle.Default;
                _MenuButton.ImagePaddingHorizontal = 8;
                _MenuButton.ImagePaddingVertical = 6;
                _MenuButton.ImagePosition = eImagePosition.Left;
                _MenuButton.ShowSubItems = false;
                _MenuBar.BorderColors = new Color[] { Color.Empty, Color.Empty, Color.Empty, _CollapsedSeparatorColor };

                _SearchBox.Visible = false;

                // Collapse everything
                foreach (BaseItem item in _ItemPanel.Items)
                {
                    if (!(item is ToolboxGroup)) continue;

                    ToolboxGroup group = (ToolboxGroup)item;
                    BaseItemAutoSizeBag bagGroup = AutoSizeBagFactory.CreateAutoSizeBag(group);
                    bagGroup.RecordSetting(item);
                    _CollapsedList.Add(bagGroup);
                    group.TitleVisible = false;
                    group.LayoutOrientation = eOrientation.Vertical;
                    group.MultiLine = false;
                    foreach (BaseItem ti in group.SubItems)
                    {
                        ButtonItem tbItem = ti as ButtonItem;
                        if (tbItem != null)
                        {
                            BaseItemAutoSizeBag bagItem = AutoSizeBagFactory.CreateAutoSizeBag(tbItem);
                            bagItem.RecordSetting(tbItem);
                            _CollapsedList.Add(bagItem);
                            tbItem.ButtonStyle = eButtonStyle.Default;
                        }
                    }
                }
            }
            else
            {
                // Expand everything
                _ExpandButton.Symbol = "\uf053";
                _MenuBar.BorderColors = new Color[0];
                _MenuButton.ButtonStyle = eButtonStyle.ImageAndText;
                _TitleLabel.Visible = true;

                _MenuButton.Symbol = string.Empty;
                _MenuButton.ButtonStyle = eButtonStyle.Default;
                _MenuButton.ImagePaddingHorizontal = 30;
                _MenuButton.ImagePaddingVertical = 12;
                _MenuButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Right;
                _MenuButton.ShowSubItems = true;
                _MenuBar.BorderColors = new Color[0];

                _SearchBox.Visible = true;

                BaseItemAutoSizeBag[] bagItems = new BaseItemAutoSizeBag[_CollapsedList.Count];
                _CollapsedList.CopyTo(bagItems);
                foreach (BaseItemAutoSizeBag ab in bagItems)
                    ab.RestoreSettings();

                _CollapsedList.Clear();

                this.Width = _ExpandedWidth;
            }

            _ItemPanel.RecalcLayout();

            if (_Expanded)
            {
                _MenuButton.FixedSize = Size.Empty;
                _ExpandButton.FixedSize = Size.Empty;
            }
            else
            {
                int itemsWidth = UpdateCollapsedWidth();
                _MenuButton.FixedSize = new Size(itemsWidth - Dpi.Width2, _MenuButton.HeightInternal);
                _ExpandButton.FixedSize = new Size(itemsWidth - Dpi.Width2, _ExpandButton.HeightInternal);

            }

            _MenuBar.RecalcLayout();
            _TitleBar.RecalcLayout();
        }

        private int UpdateCollapsedWidth()
        {
            int itemsWidth = GetItemsWidth();
            int width = itemsWidth +
                    ElementStyleLayout.LeftWhiteSpace(_ItemPanel.BackgroundStyle, eSpacePart.Margin | eSpacePart.Padding) +
                    ElementStyleLayout.RightWhiteSpace(_ItemPanel.BackgroundStyle, eSpacePart.Padding | eSpacePart.Margin) + this.Padding.Horizontal;
            _MenuButton.FixedSize = new Size(itemsWidth - Dpi.Width2, _MenuButton.HeightInternal);
            _ExpandButton.FixedSize = new Size(itemsWidth - Dpi.Width2, _ExpandButton.HeightInternal);
            this.Width = width;
            return itemsWidth;
        }

        private int GetItemsWidth()
        {
            int width = 0;
            bool scrollBar = false;
            foreach (BaseItem item in _ItemPanel.Items)
            {
                if (item is ToolboxGroup)
                {
                    foreach (BaseItem ti in item.SubItems)
                    {
                        if (ti is ButtonItem && ti.WidthInternal > width)
                            width = ti.WidthInternal;
                    }
                }
                if (item.Bounds.Bottom > this.ClientRectangle.Height) scrollBar = true;
            }

            if (scrollBar)
                width += SystemInformation.VerticalScrollBarWidth;

            if (width == 0)
                width = 32;

            return width;
        }

        /// <summary>
        /// Occurs after toolbox group is expanded
        /// </summary>
        [Description("Occurs after toolbox group is expanded")]
        public event EventHandler ToolboxGroupExpanded;

        /// <summary>
        /// Raises RemovingToken event.
        /// </summary>
        /// <param name="sender">Provides source of the event.</param>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnToolboxGroupExpanded(object sender, EventArgs e)
        {
            EventHandler handler = ToolboxGroupExpanded;
            if (handler != null)
                handler(sender, e);
        }
        internal void InvokeToolboxGroupExpanded(ToolboxGroup toolboxGroup)
        {
            if (!_Expanded)
            {
                // Adjust width of collapsed control if needed
                UpdateCollapsedWidth();
            }
            OnToolboxGroupExpanded(toolboxGroup, EventArgs.Empty);
        }

        private eToolboxItemSelectionMode _SelectionMode = eToolboxItemSelectionMode.Single;

        /// <summary>
        /// Indicates toolbox item selection mode.
        /// </summary>
        [DefaultValue(eToolboxItemSelectionMode.Single), Category("Behavior"), Description("Indicates toolbox item selection mode.")]
        public eToolboxItemSelectionMode SelectionMode
        {
            get { return _SelectionMode; }
            set
            {
                if (_SelectionMode != value)
                {
                    _SelectionMode = value;
                    OnSelectionModeChanged();
                }
            }
        }

        protected virtual void OnSelectionModeChanged()
        {

        }

        void ItemPanelItemClick(object sender, EventArgs e)
        {
            if (_SelectionMode == eToolboxItemSelectionMode.NoSelection) return;

            ToolboxItem item = sender as ToolboxItem;
            if (item == null || item.AutoCheckOnClick || item.Checked && (Control.ModifierKeys & Keys.Control) == Keys.None) return;

            if (item.Checked)
                SetSelectedItem(item, false);
            else
                SetSelectedItem(item, true);

        }

        /// <summary>
        /// Selects or deselects an item.
        /// </summary>
        /// <param name="item">Item to select or deselect.</param>
        /// <param name="isSelected">Selection state.</param>
        public void SetSelectedItem(ToolboxItem item, bool isSelected)
        {
            if (_SelectionMode == eToolboxItemSelectionMode.Single)
            {
                if (_SelectedItem == item && !isSelected)
                    this.SelectedItem = null;
                else if (isSelected)
                    this.SelectedItem = item;
            }
            else if (item.Checked != isSelected)
            {
                item.Checked = isSelected;
            }
        }

        /// <summary>
        /// Occurs after selected item has changed.
        /// </summary>
        [Description("Occurs after selected item has changed. ")]
        public event EventHandler SelectedItemChanged;

        /// <summary>
        /// Raises RemovingToken event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnSelectedItemChanged(EventArgs e)
        {
            EventHandler handler = SelectedItemChanged;
            if (handler != null)
                handler(this, e);
        }

        private CustomCollection<ToolboxItem> _SelectedItems;
        /// <summary>
        /// Gets a collection containing the currently selected items in the ToolboxControl. Do not modify items in this collection. To select or deselect list items while in multi-selection mode use SetSelected method.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CustomCollection<ToolboxItem> SelectedItems
        {
            get
            {
                return _SelectedItems;
            }
        }

        private ToolboxItem _SelectedItem;
        /// <summary>
        /// Gets or sets selected item in toolbox control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolboxItem SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (_SelectedItem != value)
                {
                    ToolboxItem oldValue = _SelectedItem;
                    _SelectedItem = value;
                    OnSelectedItemChanged(oldValue, value);
                }
            }
        }

        private bool _IgnoreCheckedChange = false;
        protected virtual void OnSelectedItemChanged(ToolboxItem oldValue, ToolboxItem newValue)
        {
            if (oldValue != null)
            {
                if (newValue != null) _IgnoreCheckedChange = true;
                oldValue.Checked = false;
                _IgnoreCheckedChange = false;
            }
            if (newValue != null) newValue.Checked = true;
        }

        internal void ToolboxItemCheckedChanged(ToolboxItem toolboxItem)
        {
            if (_SelectionMode == eToolboxItemSelectionMode.Single)
            {
                _SelectedItems.Clear();
                if (toolboxItem.Checked)
                    _SelectedItem = toolboxItem;
            }
            else
            {
                if (!toolboxItem.Checked && _SelectedItems.Contains(toolboxItem))
                    _SelectedItems.Remove(toolboxItem);
                else if (toolboxItem.Checked)
                    _SelectedItems.Add(toolboxItem);
            }
            if (!_IgnoreCheckedChange)
                OnSelectedItemChanged(EventArgs.Empty);
        }

        private bool _TitleVisible = true;

        /// <summary>
        /// Indicates whether title bar of the control is visible, default value is true.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether title bar of the control is visible.")]
        public bool TitleVisible
        {
            get { return _TitleVisible; }
            set
            {
                if (_TitleVisible != value)
                {
                    bool oldValue = value;
                    _TitleVisible = value;
                    OnTitleVisibleChanged(oldValue, value);
                }
            }
        }

        protected virtual void OnTitleVisibleChanged(bool oldValue, bool newValue)
        {
            _TitleBar.Visible = newValue;
        }

        private bool _MenuVisible = true;

        /// <summary>
        /// Indicates whether menu bar of the control is visible, default value is true.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether menu bar of the control is visible")]
        public bool MenuVisible
        {
            get { return _MenuVisible; }
            set
            {
                if (_MenuVisible != value)
                {
                    bool oldValue = value;
                    _MenuVisible = value;
                    OnMenuVisibleChanged(oldValue, value);
                }
            }
        }

        protected virtual void OnMenuVisibleChanged(bool oldValue, bool newValue)
        {
            _MenuBar.Visible = newValue;
        }

        private bool _SearchBoxVisible = true;
        /// <summary>
        /// Gets or set whether search text-box which allows searching for the toolbox items is visible. Default value is true.
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether search text-box which allows searching for the toolbox items is visible.")]
        public bool SearchBoxVisible
        {
            get { return _SearchBoxVisible; }
            set
            {
                if (_SearchBoxVisible != value)
                {
                    _SearchBoxVisible = value;
                    OnSearchBoxVisibleChanged();
                }
            }
        }

        private bool _SearchForEachWord = true;

        /// <summary>
        /// Indicates whether search text when entered is split into separate words and items returned that match any of the words entered.
        /// </summary>
        [Category("Behavior"), DefaultValue(true), Description("Indicates whether search text when entered is split into separate words and items returned that match any of the words entered.")]
        public bool SearchForEachWord
        {
            get { return _SearchForEachWord; }
            set
            {
                if (_SearchForEachWord != value)
                {
                    bool oldValue = value;
                    _SearchForEachWord = value;
                    OnSearchForEachWordChanged(oldValue, value);
                }
            }
        }

        protected virtual void OnSearchForEachWordChanged(bool oldValue, bool newValue)
        {

        }

        private void OnSearchBoxVisibleChanged()
        {
            _SearchBox.Visible = _SearchBoxVisible;
        }

        private bool _IsSearching = false;
        private List<BaseItem> _HiddenItems = new List<BaseItem>();
        private List<ToolboxGroup> _ExpandedGroups = new List<ToolboxGroup>();
        private string _LastSearchText = string.Empty;
        /// <summary>
        /// Filters control toolbox items based on specified text. To clear last search and show all items pass string.empty or null/nothing as search text.
        /// </summary>
        /// <param name="text">Text to search for</param>
        public void Search(string text)
        {
            _IsSearching = true;
            RestoreHiddenItems();

            if (!string.IsNullOrEmpty(text))
            {
                text = text.ToLower();
                string[] wordList = text.Split(' ');
                foreach (BaseItem item in _ItemPanel.Items)
                {
                    ToolboxGroup group = item as ToolboxGroup;
                    if (group == null) continue;
                    if (group.Expanded) _ExpandedGroups.Add(group);
                    group.Expanded = true;
                    foreach (BaseItem groupItem in group.SubItems)
                    {
                        ToolboxItem ti = groupItem as ToolboxItem;
                        if (ti == null)
                        {
                            ti.Visible = false;
                            _HiddenItems.Add(ti);
                            continue;
                        }
                        if (!((!_SearchForEachWord || wordList.Length < 2) && ti.Text.ToLower().Contains(text) ||
                            _SearchBoxVisible && ContainsAny(ti.Text.ToLower(), wordList)))
                        {
                            ti.Visible = false;
                            _HiddenItems.Add(ti);
                        }
                    }
                    if (group.VisibleSubItems == 0)
                    {
                        group.Visible = false;
                        _HiddenItems.Add(group);
                    }
                }
            }
            _ItemPanel.RecalcLayout();
            _IsSearching = false;
            _LastSearchText = text;
        }

        /// <summary>
        /// Gets whether control is performing search operation.
        /// </summary>
        [Browsable(false)]
        public bool IsSearching
        {
            get { return _IsSearching; }
        }

        private void RestoreHiddenItems()
        {
            bool restoreExpanded = _HiddenItems.Count > 0 || !string.IsNullOrEmpty(_LastSearchText);
            if (_HiddenItems.Count > 0)
            {
                foreach (BaseItem item in _HiddenItems)
                {
                    item.Visible = true;
                }
                _HiddenItems.Clear();
            }

            if (restoreExpanded)
            {
                foreach (BaseItem item in _ItemPanel.Items)
                {
                    ToolboxGroup group = item as ToolboxGroup;
                    if (group == null) continue;
                    if (_ExpandedGroups.Contains(group))
                        group.Expanded = true;
                    else
                        group.Expanded = false;
                }
                _ExpandedGroups.Clear();
            }
        }

        private ElementStyle _BackgroundStyle = new ElementStyle();
        /// <summary>
        /// Specifies the background style of the control.
        /// </summary>
        [Browsable(true), DevCoBrowsable(true), Category("Style"), Description("Gets or sets bar background style."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ElementStyle BackgroundStyle
        {
            get { return _BackgroundStyle; }
        }

        /// <summary>
        /// Resets style to default value. Used by windows forms designer.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetBackgroundStyle()
        {
            _BackgroundStyle.StyleChanged -= new EventHandler(this.VisualPropertyChanged);
            _BackgroundStyle = new ElementStyle();
            _BackgroundStyle.StyleChanged += new EventHandler(this.VisualPropertyChanged);
            this.Invalidate();
        }

        private void VisualPropertyChanged(object sender, EventArgs e)
        {
            OnVisualPropertyChanged();
        }

        protected virtual void OnVisualPropertyChanged()
        {
            this.Invalidate();
        }

        private void PaintStyleBackground(Graphics g)
        {
            if (!this.BackColor.IsEmpty)
            {
                using (SolidBrush brush = new SolidBrush(this.BackColor))
                    g.FillRectangle(brush, this.ClientRectangle);
            }

            ElementStyleDisplayInfo info = new ElementStyleDisplayInfo();
            info.Bounds = this.ClientRectangle;
            info.Graphics = g;
            bool disposeStyle = false;
            ElementStyle style = ElementStyleDisplay.GetElementStyle(_BackgroundStyle, out disposeStyle);
            info.Style = style;
            ElementStyleDisplay.Paint(info);
            if (disposeStyle)
                style.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            PaintStyleBackground(g);

            base.OnPaint(e);
        }

        // <summary>
        /// Occurs before an item drag &amp; drop operation is started and allows cancellation.
        /// </summary>
        [Description("Occurs before an item drag & drop operation is started and allows cancellation")]
        public event CancelEventHandler BeforeItemDrag;
        /// <summary>
        /// Raises BeforeItemDrag event.
        /// </summary>
        /// <param name="itemSource">Item being dragged.</param>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnBeforeItemDrag(object itemSource, CancelEventArgs e)
        {
            CancelEventHandler handler = BeforeItemDrag;
            if (handler != null)
                handler(itemSource, e);
        }

        private static bool ContainsAny(string textToSearch, params string[] wordsList)
        {
            foreach (string word in wordsList)
            {
                if (textToSearch.Contains(word))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether item drag and drop is enabled. Default value is true.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether item drag and drop is enabled. Default value is true.")]
        public bool ItemDragDropEnabled
        {
            get { return _ItemPanel.EnableDragDrop; }
            set { _ItemPanel.EnableDragDrop = value; }
        }

        /// <summary>
        /// Indicates whether toolbox items can be rearranged using drag & drop.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether toolbox items can be rearranged using drag & drop.")]
        public bool ItemRearrangeEnabled
        {
            get { return _ItemPanel.AllowDrop; }
            set { _ItemPanel.AllowDrop = value; }
        }
        #endregion

        #region Licensing
#if !TRIAL
        private string m_LicenseKey = "";
        private bool m_DialogDisplayed = false;
        [Browsable(false), DefaultValue("")]
        public string LicenseKey
        {
            get { return m_LicenseKey; }
            set
            {
                if (NodeOperations.ValidateLicenseKey(value))
                    return;
                m_LicenseKey = (!NodeOperations.CheckLicenseKey(value) ? "9dsjkhds7" : value);
            }
        }
#endif
        #endregion

    }

    /// <summary>
    /// Defines selection modes for toolbox control items.
    /// </summary>
    public enum eToolboxItemSelectionMode
    {
        /// <summary>
        /// No item selection is allowed.
        /// </summary>
        NoSelection,
        /// <summary>
        /// Only single item can be selected.
        /// </summary>
        Single,
        /// <summary>
        /// Multiple items can be selected.
        /// </summary>
        Multiple
    }
}
