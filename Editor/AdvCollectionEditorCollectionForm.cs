using DevComponents.DotNetBar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace z.UI.Editor
{
    /// <summary>
    ///  This is the collection editor's default implementation of a collection form.
    /// </summary>
    public class AdvCollectionEditorCollectionForm : AdvCollectionForm
    {
        private const int TextIndent = 1;
        private const int PaintWidth = 20;
        private const int PaintIndent = 26;
        private static readonly double s_log10 = Math.Log(10);

        private ArrayList _createdItems;
        private ArrayList _removedItems;
        private ArrayList _originalItems;

        private readonly AdvCollectionEditor _editor;

        private FilterListBox _listbox;
        private SplitButton _addButton;
        private Button _removeButton;
        private Button _cancelButton;
        private Button _okButton;
        private Button _downButton;
        private Button _upButton;
        private AdvPropertyGrid _propertyBrowser;
        private Label _membersLabel;
        private Label _propertiesLabel;
        private readonly ContextMenuStrip _addDownMenu;

        private int _suspendEnabledCount;
        private SplitContainer splitContainer1;
        private Panel panel3;
        private Panel panel2;
        private Panel panel1;
        private bool _dirty;

        public AdvCollectionEditorCollectionForm(AdvCollectionEditor editor) : base(editor)
        {
            _editor = editor;
            InitializeComponent();
            //if (DpiHelper.IsScalingRequired)
            //{
            //    ScaleButtonImageLogicalToDevice(_downButton);
            //    ScaleButtonImageLogicalToDevice(_upButton);
            //}
            Text = string.Format("{0} Collection Editor", CollectionItemType.Name);

            HookEvents();

            Type[] newItemTypes = NewItemTypes;
            if (newItemTypes.Length > 1)
            {
                EventHandler addDownMenuClick = new EventHandler(AddDownMenu_click);
                _addButton.ShowSplit = true;
                _addDownMenu = new ContextMenuStrip();
                _addButton.ContextMenuStrip = _addDownMenu;
                for (int i = 0; i < newItemTypes.Length; i++)
                {
                    _addDownMenu.Items.Add(new TypeMenuItem(newItemTypes[i], addDownMenuClick));
                }
            }
            AdjustListBoxItemHeight();
        }

        private bool IsImmutable
        {
            get
            {
                foreach (ListItem item in _listbox.SelectedItems)
                {
                    Type type = item.Value.GetType();

                    // The type is considered immutable if the converter is defined as requiring a
                    // create instance or all the properties are read-only.
                    if (!TypeDescriptor.GetConverter(type).GetCreateInstanceSupported())
                    {
                        foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(type))
                        {
                            if (!p.IsReadOnly)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }

        /// <summary>
        ///  Adds a new element to the collection.
        /// </summary>
        private void AddButton_click(object sender, EventArgs e)
        {
            PerformAdd();
        }

        /// <summary>
        ///  Processes a click of the drop down type menu. This creates a new instance.
        /// </summary>
        private void AddDownMenu_click(object sender, EventArgs e)
        {
            if (sender is TypeMenuItem typeMenuItem)
            {
                CreateAndAddInstance(typeMenuItem.ItemType);
            }
        }

        /// <summary>
        ///  This Function adds the individual objects to the ListBox.
        /// </summary>
        private void AddItems(IList instances)
        {
            if (_createdItems is null)
            {
                _createdItems = new ArrayList();
            }

            _listbox.BeginUpdate();
            try
            {
                foreach (object instance in instances)
                {
                    if (instance != null)
                    {
                        _dirty = true;
                        _createdItems.Add(instance);
                        ListItem created = new ListItem(_editor, instance);
                        _listbox.Items.Add(created);
                    }
                }
            }
            finally
            {
                _listbox.EndUpdate();
            }

            if (instances.Count == 1)
            {
                // optimize for the case where we just added one thing...
                UpdateItemWidths(_listbox.Items[_listbox.Items.Count - 1] as ListItem);
            }
            else
            {
                UpdateItemWidths(null);
            }

            SuspendEnabledUpdates();
            try
            {
                _listbox.ClearSelected();
                _listbox.SelectedIndex = _listbox.Items.Count - 1;

                object[] items = new object[_listbox.Items.Count];
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = ((ListItem)_listbox.Items[i]).Value;
                }
                Items = items;

                // If omeone changes the edit value which resets the selindex, we
                // should keep the new index.
                if (_listbox.Items.Count > 0 && _listbox.SelectedIndex != _listbox.Items.Count - 1)
                {
                    _listbox.ClearSelected();
                    _listbox.SelectedIndex = _listbox.Items.Count - 1;
                }
            }
            finally
            {
                ResumeEnabledUpdates(true);
            }
        }

        private void AdjustListBoxItemHeight()
        {
            _listbox.ItemHeight = Font.Height + SystemInformation.BorderSize.Width * 2;
        }

        /// <summary>
        ///  Determines whether removal of a specific list item should be permitted.
        ///  Used to determine enabled/disabled state of the Remove (X) button.
        ///  Items added after editor was opened may always be removed.
        ///  Items that existed before editor was opened require a call to CanRemoveInstance.
        /// </summary>
        private bool AllowRemoveInstance(object value)
        {
            if (_createdItems != null && _createdItems.Contains(value))
            {
                return true;
            }

            return CanRemoveInstance(value);
        }

        private int CalcItemWidth(Graphics g, ListItem item)
        {
            int c = Math.Max(2, _listbox.Items.Count);
            SizeF sizeW = g.MeasureString(c.ToString(CultureInfo.CurrentCulture), _listbox.Font);

            int charactersInNumber = ((int)(Math.Log((double)(c - 1)) / s_log10) + 1);
            int w = 4 + charactersInNumber * (Font.Height / 2);

            w = Math.Max(w, (int)Math.Ceiling(sizeW.Width));
            w += SystemInformation.BorderSize.Width * 4;

            SizeF size = g.MeasureString(GetDisplayText(item), _listbox.Font);
            int pic = 0;
            if (item.Editor != null && item.Editor.GetPaintValueSupported())
            {
                pic = PaintWidth + TextIndent;
            }
            return (int)Math.Ceiling(size.Width) + w + pic + SystemInformation.BorderSize.Width * 4;
        }

        /// <summary>
        ///  Aborts changes made in the editor.
        /// </summary>
        private void CancelButton_click(object sender, EventArgs e)
        {
            try
            {
                _editor.CancelChanges();

                if (!CollectionEditable || !_dirty)
                {
                    return;
                }

                _dirty = false;
                _listbox.Items.Clear();

                if (_createdItems != null)
                {
                    object[] items = _createdItems.ToArray();
                    if (items.Length > 0 && items[0] is IComponent && ((IComponent)items[0]).Site != null)
                    {
                        // here we bail now because we don't want to do the "undo" manually,
                        // we're part of a trasaction, we've added item, the rollback will be
                        // handled by the undo engine because the component in the collection are sited
                        // doing it here kills perfs because the undo of the transaction has to rollback the remove and then
                        // rollback the add. This is useless and is only needed for non sited component or other classes
                        return;
                    }
                    for (int i = 0; i < items.Length; i++)
                    {
                        DestroyInstance(items[i]);
                    }
                    _createdItems.Clear();
                }
                if (_removedItems != null)
                {
                    _removedItems.Clear();
                }

                // Restore the original contents. Because objects get parented during CreateAndAddInstance, the underlying collection
                // gets changed during add, but not other operations. Not all consumers of this dialog can roll back every single change,
                // but this will at least roll back the additions, removals and reordering. See ASURT #85470.
                if (_originalItems != null && (_originalItems.Count > 0))
                {
                    object[] items = new object[_originalItems.Count];
                    for (int i = 0; i < _originalItems.Count; i++)
                    {
                        items[i] = _originalItems[i];
                    }
                    Items = items;
                    _originalItems.Clear();
                }
                else
                {
                    Items = Array.Empty<object>();
                }
            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.None;
                DisplayError(ex);
            }
        }

        /// <summary>
        ///  Performs a create instance and then adds the instance to the list box.
        /// </summary>
        private void CreateAndAddInstance(Type type)
        {
            try
            {
                object instance = CreateInstance(type);
                IList multipleInstance = _editor.GetObjectsFromInstance(instance);

                if (multipleInstance != null)
                {
                    AddItems(multipleInstance);
                }
            }
            catch (Exception e)
            {
                DisplayError(e);
            }
        }

        /// <summary>
        ///  Moves the selected item down one.
        /// </summary>
        private void DownButton_click(object sender, EventArgs e)
        {
            try
            {
                SuspendEnabledUpdates();
                _dirty = true;
                int index = _listbox.SelectedIndex;
                if (index == _listbox.Items.Count - 1)
                {
                    return;
                }

                int ti = _listbox.TopIndex;
                object itemMove = _listbox.Items[index];
                _listbox.Items[index] = _listbox.Items[index + 1];
                _listbox.Items[index + 1] = itemMove;

                if (ti < _listbox.Items.Count - 1)
                {
                    _listbox.TopIndex = ti + 1;
                }

                _listbox.ClearSelected();
                _listbox.SelectedIndex = index + 1;

                // enabling/disabling the buttons has moved the focus to the OK button, move it back to the sender
                Control ctrlSender = (Control)sender;

                if (ctrlSender.Enabled)
                {
                    ctrlSender.Focus();
                }
            }
            finally
            {
                ResumeEnabledUpdates(true);
            }
        }

        private void CollectionEditor_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            _editor.ShowHelp();
        }

        private void Form_HelpRequested(object sender, HelpEventArgs e)
        {
            _editor.ShowHelp();
        }

        /// <summary>
        ///  Retrieves the display text for the given list item (if any). The item determines its own display text
        ///  through its ToString() method, which delegates to the GetDisplayText() override on the parent AdvCollectionEditor.
        ///  This means in theory that the text can change at any time (ie. its not fixed when the item is added to the list).
        ///  The item returns its display text through ToString() so that the same text will be reported to Accessibility clients.
        /// </summary>
        private string GetDisplayText(ListItem item)
        {
            return (item is null) ? string.Empty : item.ToString();
        }

        private void HookEvents()
        {
            _listbox.KeyDown += new KeyEventHandler(Listbox_keyDown);
            _listbox.DrawItem += new DrawItemEventHandler(Listbox_drawItem);
            _listbox.SelectedIndexChanged += new EventHandler(Listbox_SelectedIndexChanged);
            _listbox.HandleCreated += new EventHandler(Listbox_HandleCreated);
            _upButton.Click += new EventHandler(UpButton_Click);
            _downButton.Click += new EventHandler(DownButton_click);
            _propertyBrowser.PropertyValueChanged += new PropertyChangedEventHandler(PropertyGrid_propertyValueChanged);
            _addButton.Click += new EventHandler(AddButton_click);
            _removeButton.Click += new EventHandler(RemoveButton_Click);
            _okButton.Click += new EventHandler(OKButton_Click);
            _cancelButton.Click += new EventHandler(CancelButton_click);
            HelpButtonClicked += new CancelEventHandler(CollectionEditor_HelpButtonClicked);
            HelpRequested += new HelpEventHandler(Form_HelpRequested);
            Shown += new EventHandler(Form_Shown);
        }

        private void InitializeComponent()
        {
            this._membersLabel = new System.Windows.Forms.Label();
            this._listbox = new z.UI.Editor.FilterListBox();
            this._upButton = new System.Windows.Forms.Button();
            this._downButton = new System.Windows.Forms.Button();
            this._propertiesLabel = new System.Windows.Forms.Label();
            this._propertyBrowser = new DevComponents.DotNetBar.AdvPropertyGrid();
            this._addButton = new z.UI.Editor.SplitButton();
            this._removeButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this._propertyBrowser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // _membersLabel
            // 
            this._membersLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._membersLabel.Location = new System.Drawing.Point(0, 0);
            this._membersLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this._membersLabel.Name = "_membersLabel";
            this._membersLabel.Size = new System.Drawing.Size(271, 23);
            this._membersLabel.TabIndex = 3;
            // 
            // _listbox
            // 
            this._listbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._listbox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._listbox.FormattingEnabled = true;
            this._listbox.Location = new System.Drawing.Point(0, 23);
            this._listbox.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this._listbox.Name = "_listbox";
            this._listbox.Size = new System.Drawing.Size(215, 393);
            this._listbox.TabIndex = 4;
            // 
            // _upButton
            // 
            this._upButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._upButton.BackgroundImage = BarFunctions.LoadBitmap("SystemImages.NavBarShowMore.png"); 
            this._upButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._upButton.Location = new System.Drawing.Point(12, 20);
            this._upButton.Name = "_upButton";
            this._upButton.Size = new System.Drawing.Size(31, 53);
            this._upButton.TabIndex = 7;
            // 
            // _downButton
            // 
            this._downButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._downButton.BackgroundImage = BarFunctions.LoadBitmap("SystemImages.NavBarShowLess.png");
            this._downButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._downButton.Location = new System.Drawing.Point(12, 94);
            this._downButton.Name = "_downButton";
            this._downButton.Size = new System.Drawing.Size(31, 56);
            this._downButton.TabIndex = 0;
            // 
            // _propertiesLabel
            // 
            this._propertiesLabel.AutoEllipsis = true;
            this._propertiesLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._propertiesLabel.Location = new System.Drawing.Point(0, 0);
            this._propertiesLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this._propertiesLabel.Name = "_propertiesLabel";
            this._propertiesLabel.Size = new System.Drawing.Size(540, 23);
            this._propertiesLabel.TabIndex = 2;
            // 
            // _propertyBrowser
            // 
            this._propertyBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._propertyBrowser.GridLinesColor = System.Drawing.Color.WhiteSmoke;
            this._propertyBrowser.Location = new System.Drawing.Point(0, 23);
            this._propertyBrowser.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this._propertyBrowser.Name = "_propertyBrowser";
            this._propertyBrowser.Size = new System.Drawing.Size(540, 452);
            this._propertyBrowser.TabIndex = 5;
            // 
            // _addButton
            // 
            this._addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._addButton.Location = new System.Drawing.Point(24, 14);
            this._addButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this._addButton.Name = "_addButton";
            this._addButton.Size = new System.Drawing.Size(75, 23);
            this._addButton.TabIndex = 0;
            this._addButton.Text = "Add";
            // 
            // _removeButton
            // 
            this._removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._removeButton.Location = new System.Drawing.Point(118, 14);
            this._removeButton.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this._removeButton.Name = "_removeButton";
            this._removeButton.Size = new System.Drawing.Size(75, 23);
            this._removeButton.TabIndex = 1;
            this._removeButton.Text = "Remove";
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okButton.Location = new System.Drawing.Point(613, 23);
            this._okButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 0;
            this._okButton.Text = "&Ok";
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(694, 23);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "&Cancel";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.FixedPanel = FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._listbox);
            this.splitContainer1.Panel1.Controls.Add(this.panel3);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            this.splitContainer1.Panel1.Controls.Add(this._membersLabel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._propertyBrowser);
            this.splitContainer1.Panel2.Controls.Add(this._propertiesLabel);
            this.splitContainer1.Size = new System.Drawing.Size(815, 475);
            this.splitContainer1.SplitterDistance = 271;
            this.splitContainer1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._cancelButton);
            this.panel1.Controls.Add(this._okButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 475);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(815, 72);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this._removeButton);
            this.panel2.Controls.Add(this._addButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 416);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(271, 59);
            this.panel2.TabIndex = 8;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this._upButton);
            this.panel3.Controls.Add(this._downButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(215, 23);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(56, 393);
            this.panel3.TabIndex = 9;
            // 
            // AdvCollectionEditorCollectionForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(815, 547);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvCollectionEditorCollectionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this._propertyBrowser)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void UpdateItemWidths(ListItem item)
        {
            if (!_listbox.IsHandleCreated)
            {
                return;
            }

            using (Graphics g = _listbox.CreateGraphics())
            {
                int old = _listbox.HorizontalExtent;

                if (item != null)
                {
                    int w = CalcItemWidth(g, item);
                    if (w > old)
                    {
                        _listbox.HorizontalExtent = w;
                    }
                }
                else
                {
                    int max = 0;
                    foreach (ListItem i in _listbox.Items)
                    {
                        int w = CalcItemWidth(g, i);
                        if (w > max)
                        {
                            max = w;
                        }
                    }
                    _listbox.HorizontalExtent = max;
                }
            }
        }

        /// <summary>
        ///  This draws a row of the listbox.
        /// </summary>
        private void Listbox_drawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                ListItem item = (ListItem)_listbox.Items[e.Index];

                Graphics g = e.Graphics;

                int c = _listbox.Items.Count;
                int maxC = (c > 1) ? c - 1 : c;
                // We add the +4 is a fudge factor...
                SizeF sizeW = g.MeasureString(maxC.ToString(CultureInfo.CurrentCulture), _listbox.Font);

                int charactersInNumber = ((int)(Math.Log((double)maxC) / s_log10) + 1);// Luckily, this is never called if count = 0
                int w = 4 + charactersInNumber * (Font.Height / 2);

                w = Math.Max(w, (int)Math.Ceiling(sizeW.Width));
                w += SystemInformation.BorderSize.Width * 4;

                Rectangle button = new Rectangle(e.Bounds.X, e.Bounds.Y, w, e.Bounds.Height);

                ControlPaint.DrawButton(g, button, ButtonState.Normal);
                button.Inflate(-SystemInformation.BorderSize.Width * 2, -SystemInformation.BorderSize.Height * 2);

                int offset = w;

                Color backColor = SystemColors.Window;
                Color textColor = SystemColors.WindowText;
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    backColor = SystemColors.Highlight;
                    textColor = SystemColors.HighlightText;
                }
                Rectangle res = new Rectangle(e.Bounds.X + offset, e.Bounds.Y,
                                              e.Bounds.Width - offset,
                                              e.Bounds.Height);
                g.FillRectangle(new SolidBrush(backColor), res);
                if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                {
                    ControlPaint.DrawFocusRectangle(g, res);
                }
                offset += 2;

                if (item.Editor != null && item.Editor.GetPaintValueSupported())
                {
                    Rectangle baseVar = new Rectangle(e.Bounds.X + offset, e.Bounds.Y + 1, PaintWidth, e.Bounds.Height - 3);
                    g.DrawRectangle(SystemPens.ControlText, baseVar.X, baseVar.Y, baseVar.Width - 1, baseVar.Height - 1);
                    baseVar.Inflate(-1, -1);
                    item.Editor.PaintValue(item.Value, g, baseVar);
                    offset += PaintIndent + TextIndent;
                }

                StringFormat format = new StringFormat();
                try
                {
                    format.Alignment = StringAlignment.Center;
                    g.DrawString(e.Index.ToString(CultureInfo.CurrentCulture), Font, SystemBrushes.ControlText,
                                 new Rectangle(e.Bounds.X, e.Bounds.Y, w, e.Bounds.Height), format);
                }

                finally
                {
                    format?.Dispose();
                }

                Brush textBrush = new SolidBrush(textColor);

                string itemText = GetDisplayText(item);

                try
                {
                    g.DrawString(itemText, Font, textBrush, new Rectangle(e.Bounds.X + offset, e.Bounds.Y, e.Bounds.Width - offset, e.Bounds.Height));
                }

                finally
                {
                    textBrush?.Dispose();
                }

                // Check to see if we need to change the horizontal extent of the listbox
                int width = offset + (int)g.MeasureString(itemText, Font).Width;
                if (width > e.Bounds.Width && _listbox.HorizontalExtent < width)
                {
                    _listbox.HorizontalExtent = width;
                }
            }
        }

        /// <summary>
        ///  Handles keypress events for the list box.
        /// </summary>
        private void Listbox_keyDown(object sender, KeyEventArgs kevent)
        {
            switch (kevent.KeyData)
            {
                case Keys.Delete:
                    PerformRemove();
                    break;
                case Keys.Insert:
                    PerformAdd();
                    break;
            }
        }

        /// <summary>
        ///  Event that fires when the selected list box index changes.
        /// </summary>
        private void Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        /// <summary>
        ///  Event that fires when the list box's window handle is created.
        /// </summary>
        private void Listbox_HandleCreated(object sender, EventArgs e)
        {
            UpdateItemWidths(null);
        }

        /// <summary>
        ///  Commits the changes to the editor.
        /// </summary>
        private void OKButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_dirty || !CollectionEditable)
                {
                    _dirty = false;
                    DialogResult = DialogResult.Cancel;
                    return;
                }

                if (_dirty)
                {
                    object[] items = new object[_listbox.Items.Count];
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i] = ((ListItem)_listbox.Items[i]).Value;
                    }

                    Items = items;
                }

                if (_removedItems != null && _dirty)
                {
                    object[] deadItems = _removedItems.ToArray();

                    for (int i = 0; i < deadItems.Length; i++)
                    {
                        DestroyInstance(deadItems[i]);
                    }
                    _removedItems.Clear();
                }

                _createdItems?.Clear();
                _originalItems?.Clear();

                _listbox.Items.Clear();
                _dirty = false;
            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.None;
                DisplayError(ex);
            }
        }

        /// <summary>
        ///  Reflect any change events to the instance object
        /// </summary>
        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            // see if this is any of the items in our list...this can happen if we launched a child editor
            if (!_dirty && _originalItems != null)
            {
                foreach (object item in _originalItems)
                {
                    if (item == e.Component)
                    {
                        _dirty = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///  This is called when the value property in the CollectionForm has changed.
        ///  In it you should update your user interface to reflect the current value.
        /// </summary>
        public override void OnEditValueChanged()
        {
            if (!Visible)
            {
                return;
            }

            // Remember these contents for cancellation
            if (_originalItems is null)
            {
                _originalItems = new ArrayList();
            }
            _originalItems.Clear();

            // Now update the list box.
            _listbox.Items.Clear();
            _propertyBrowser.Site = new PropertyGridSite(Context, _propertyBrowser);
            if (EditValue != null)
            {
                SuspendEnabledUpdates();
                try
                {
                    object[] items = Items;
                    for (int i = 0; i < items.Length; i++)
                    {
                        _listbox.Items.Add(new ListItem(_editor, items[i]));
                        _originalItems.Add(items[i]);
                    }
                    if (_listbox.Items.Count > 0)
                    {
                        _listbox.SelectedIndex = 0;
                    }
                }
                finally
                {
                    ResumeEnabledUpdates(true);
                }
            }
            else
            {
                UpdateEnabled();
            }

            AdjustListBoxItemHeight();
            UpdateItemWidths(null);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            AdjustListBoxItemHeight();
        }

        /// <summary>
        ///  Performs the actual add of new items. This is invoked by the add button
        ///  as well as the insert key on the list box.
        /// </summary>
        private void PerformAdd()
        {
            CreateAndAddInstance(NewItemTypes[0]);
        }

        /// <summary>
        ///  Performs a remove by deleting all items currently selected in the list box.
        ///  This is called by the delete button as well as the delete key on the list box.
        /// </summary>
        private void PerformRemove()
        {
            int index = _listbox.SelectedIndex;

            if (index != -1)
            {
                SuspendEnabledUpdates();
                try
                {
                    if (_listbox.SelectedItems.Count > 1)
                    {
                        ArrayList toBeDeleted = new ArrayList(_listbox.SelectedItems);
                        foreach (ListItem item in toBeDeleted)
                        {
                            RemoveInternal(item);
                        }
                    }
                    else
                    {
                        RemoveInternal((ListItem)_listbox.SelectedItem);
                    }
                    if (index < _listbox.Items.Count)
                    {
                        _listbox.SelectedIndex = index;
                    }
                    else if (_listbox.Items.Count > 0)
                    {
                        _listbox.SelectedIndex = _listbox.Items.Count - 1;
                    }
                }
                finally
                {
                    ResumeEnabledUpdates(true);
                }
            }
        }

        /// <summary>
        ///  When something in the properties window changes, we update pertinent text here.
        /// </summary>
        private void PropertyGrid_propertyValueChanged(object sender, PropertyChangedEventArgs e)
        {
            _dirty = true;

            // Refresh selected listbox item so that it picks up any name change
            SuspendEnabledUpdates();
            try
            {
                int selectedItem = _listbox.SelectedIndex;
                if (selectedItem >= 0)
                {
                    _listbox.RefreshItem(_listbox.SelectedIndex);
                }
            }
            finally
            {
                ResumeEnabledUpdates(false);
            }

            // if a property changes, invalidate the grid in case it affects the item's name.
            UpdateItemWidths(null);
            _listbox.Invalidate();

            // also update the string above the grid.
            _propertiesLabel.Text = string.Format("{0} properties:", GetDisplayText((ListItem)_listbox.SelectedItem));
        }

        /// <summary>
        ///  Used to actually remove the items, one by one.
        /// </summary>
        private void RemoveInternal(ListItem item)
        {
            if (item != null)
            {
                _editor.OnItemRemoving(item.Value);

                _dirty = true;

                if (_createdItems != null && _createdItems.Contains(item.Value))
                {
                    DestroyInstance(item.Value);
                    _createdItems.Remove(item.Value);
                    _listbox.Items.Remove(item);
                }
                else
                {
                    try
                    {
                        if (CanRemoveInstance(item.Value))
                        {
                            if (_removedItems is null)
                            {
                                _removedItems = new ArrayList();
                            }

                            _removedItems.Add(item.Value);
                            _listbox.Items.Remove(item);
                        }
                        else
                        {
                            throw new Exception(string.Format("The item '{0}' cannot be removed.", GetDisplayText(item)));
                        }
                    }
                    catch (Exception ex)
                    {
                        DisplayError(ex);
                    }
                }

                UpdateItemWidths(null);
            }
        }

        /// <summary>
        ///  Removes the selected item.
        /// </summary>
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            PerformRemove();

            // enabling/disabling the buttons has moved the focus to the OK button, move it back to the sender
            Control ctrlSender = (Control)sender;
            if (ctrlSender.Enabled)
            {
                ctrlSender.Focus();
            }
        }

        /// <summary>
        ///  used to prevent flicker when playing with the list box selection call resume when done.
        ///  Calls to UpdateEnabled will return silently until Resume is called
        /// </summary>
        private void ResumeEnabledUpdates(bool updateNow)
        {
            _suspendEnabledCount--;

            Debug.Assert(_suspendEnabledCount >= 0, "Mismatch suspend/resume enabled");

            if (updateNow)
            {
                UpdateEnabled();
            }
            else
            {
                BeginInvoke(new MethodInvoker(UpdateEnabled));
            }
        }
        /// <summary>
        ///  Used to prevent flicker when playing with the list box selection call resume when done.
        ///  Calls to UpdateEnabled will return silently until Resume is called
        /// </summary>
        private void SuspendEnabledUpdates() => _suspendEnabledCount++;

        /// <summary>
        ///  Called to show the dialog via the IWindowsFormsEditorService
        /// </summary>
        protected internal override DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc)
        {
            IComponentChangeService cs = _editor.Context.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            DialogResult result = DialogResult.OK;
            try
            {
                if (cs != null)
                {
                    cs.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
                }

                // This is cached across requests, so reset the initial focus.
                ActiveControl = _listbox;
                result = base.ShowEditorDialog(edSvc);
            }
            finally
            {
                if (cs != null)
                {
                    cs.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                }
            }

            return result;
        }

        /// <summary>
        ///  Moves an item up one in the list box.
        /// </summary>
        private void UpButton_Click(object sender, EventArgs e)
        {
            int index = _listbox.SelectedIndex;
            if (index == 0)
            {
                return;
            }

            _dirty = true;
            try
            {
                SuspendEnabledUpdates();
                int ti = _listbox.TopIndex;
                object itemMove = _listbox.Items[index];
                _listbox.Items[index] = _listbox.Items[index - 1];
                _listbox.Items[index - 1] = itemMove;

                if (ti > 0)
                {
                    _listbox.TopIndex = ti - 1;
                }

                _listbox.ClearSelected();
                _listbox.SelectedIndex = index - 1;

                // enabling/disabling the buttons has moved the focus to the OK button, move it back to the sender
                Control ctrlSender = (Control)sender;

                if (ctrlSender.Enabled)
                {
                    ctrlSender.Focus();
                }
            }
            finally
            {
                ResumeEnabledUpdates(true);
            }
        }

        /// <summary>
        ///  Updates the set of enabled buttons.
        /// </summary>
        private void UpdateEnabled()
        {
            if (_suspendEnabledCount > 0)
            {
                // We're in the midst of a suspend/resume block  Resume should call us back.
                return;
            }

            bool editEnabled = (_listbox.SelectedItem != null) && CollectionEditable;
            _removeButton.Enabled = editEnabled && AllowRemoveInstance(((ListItem)_listbox.SelectedItem).Value);
            _upButton.Enabled = editEnabled && _listbox.Items.Count > 1;
            _downButton.Enabled = editEnabled && _listbox.Items.Count > 1;
            _propertyBrowser.Enabled = editEnabled;
            _addButton.Enabled = CollectionEditable;

            if (_listbox.SelectedItem != null)
            {
                object[] items;

                // If we are to create new instances from the items, then we must wrap them in an outer object.
                // otherwise, the user will be presented with a batch of read only properties, which isn't terribly useful.
                if (IsImmutable)
                {
                    items = new object[] { new SelectionWrapper(CollectionType, CollectionItemType, _listbox, _listbox.SelectedItems) };
                }
                else
                {
                    items = new object[_listbox.SelectedItems.Count];
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i] = ((ListItem)_listbox.SelectedItems[i]).Value;
                    }
                }

                int selectedItemCount = _listbox.SelectedItems.Count;
                if ((selectedItemCount == 1) || (selectedItemCount == -1))
                {
                    // handle both single select listboxes and a single item selected in a multi-select listbox
                    _propertiesLabel.Text = string.Format("{0} properties:", GetDisplayText((ListItem)_listbox.SelectedItem));
                }
                else
                {
                    _propertiesLabel.Text = "Multi-Select Properties:"; //SR.CollectionEditorPropertiesMultiSelect;
                }

                if (_editor.IsAnyObjectInheritedReadOnly(items))
                {
                    _propertyBrowser.SelectedObjects = null;
                    _propertyBrowser.Enabled = false;
                    _removeButton.Enabled = false;
                    _upButton.Enabled = false;
                    _downButton.Enabled = false;
                    _propertiesLabel.Text = "Properties"; //SR.CollectionEditorInheritedReadOnlySelection;
                }
                else
                {
                    _propertyBrowser.Enabled = true;
                    _propertyBrowser.SelectedObjects = items;
                }
            }
            else
            {
                _propertiesLabel.Text = "Properties";   //SR.CollectionEditorPropertiesNone;
                _propertyBrowser.SelectedObject = null;
            }
        }

        /// <summary>
        ///  When the form is first shown, update controls due to the edit value changes which happened when the form is invisible.
        /// </summary>
        private void Form_Shown(object sender, EventArgs e)
        {
            OnEditValueChanged();
        }

        /// <summary>
        ///  Create a new button bitmap scaled for the device units.
        ///  Note: original image might be disposed.
        /// </summary>
        /// <param name="button">button with an image, image size is defined in logical units</param>
        private static void ScaleButtonImageLogicalToDevice(Button button)
        {
            if (button is null || !(button.Image is Bitmap buttonBitmap))
            {
                return;
            }

            //Bitmap deviceBitmap = DpiHelper.CreateScaledBitmap(buttonBitmap);
            button.Image.Dispose();
            //button.Image = //new Image(); //deviceBitmap;
        }

        /// <summary>
        ///  This class implements a custom type descriptor that is used to provide
        ///  properties for the set of selected items in the collection editor.
        ///  It provides a single property that is equivalent to the editor's collection item type.
        /// </summary>
        private class SelectionWrapper : PropertyDescriptor, ICustomTypeDescriptor
        {
            private readonly Control _control;
            private readonly ICollection _collection;
            private readonly PropertyDescriptorCollection _properties;
            private object _value;

            public SelectionWrapper(Type collectionType, Type collectionItemType, Control control, ICollection collection)
                : base("Value", new Attribute[] { new CategoryAttribute(collectionItemType.Name) })
            {
                ComponentType = collectionType;
                PropertyType = collectionItemType;
                _control = control;
                _collection = collection;
                _properties = new PropertyDescriptorCollection(new PropertyDescriptor[] { this });

                Debug.Assert(collection.Count > 0, "We should only be wrapped if there is a selection");
                _value = this;

                // In a multiselect case, see if the values are different. If so, NULL our value to represent indeterminate.
                foreach (ListItem li in collection)
                {
                    if (_value == this)
                    {
                        _value = li.Value;
                    }
                    else
                    {
                        object nextValue = li.Value;
                        if (_value != null)
                        {
                            if (nextValue is null)
                            {
                                _value = null;
                                break;
                            }
                            else
                            {
                                if (!_value.Equals(nextValue))
                                {
                                    _value = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (nextValue != null)
                            {
                                _value = null;
                                break;
                            }
                        }
                    }
                }
            }

            /// <summary>
            ///  When overridden in a derived class, gets the type of the component this property is bound to.
            /// </summary>
            public override Type ComponentType { get; }

            /// <summary>
            ///  When overridden in a derived class, gets a value indicating whether this property is read-only.
            /// </summary>
            public override bool IsReadOnly => false;

            /// <summary>
            ///  When overridden in a derived class, gets the type of the property.
            /// </summary>
            public override Type PropertyType { get; }

            /// <summary>
            ///  When overridden in a derived class, indicates whether resetting the <paramref name="component"/>
            ///  will change the value of the <paramref name="component"/>.
            /// </summary>
            public override bool CanResetValue(object component) => false;

            /// <summary>
            ///  When overridden in a derived class, gets the current value of the property on a component.
            /// </summary>
            public override object GetValue(object component) => _value;

            /// <summary>
            ///  When overridden in a derived class, resets the value for this property of the component.
            /// </summary>
            public override void ResetValue(object component)
            {
            }

            /// <summary>
            ///  When overridden in a derived class, sets the value of the component to a different value.
            /// </summary>
            public override void SetValue(object component, object value)
            {
                _value = value;

                foreach (ListItem li in _collection)
                {
                    li.Value = value;
                }
                _control.Invalidate();
                OnValueChanged(component, EventArgs.Empty);
            }

            /// <summary>
            ///  When overridden in a derived class, indicates whether the value of this property needs to be persisted.
            /// </summary>
            public override bool ShouldSerializeValue(object component) => false;

            /// <summary>
            ///  Retrieves an array of member attributes for the given object.
            /// </summary>
            AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
                return TypeDescriptor.GetAttributes(PropertyType);
            }

            /// <summary>
            ///  Retrieves the class name for this object. If null is returned, the type name is used.
            /// </summary>
            string ICustomTypeDescriptor.GetClassName() => PropertyType.Name;

            /// <summary>
            ///  Retrieves the name for this object. If null is returned, the default is used.
            /// </summary>
            string ICustomTypeDescriptor.GetComponentName() => null;

            /// <summary>
            ///  Retrieves the type converter for this object.
            /// </summary>
            TypeConverter ICustomTypeDescriptor.GetConverter() => null;

            /// <summary>
            ///  Retrieves the default event.
            /// </summary>
            EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => null;

            /// <summary>
            ///  Retrieves the default property.
            /// </summary>
            PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => this;

            /// <summary>
            ///  Retrieves the an editor for this object.
            /// </summary>
            object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => null;

            /// <summary>
            ///  Retrieves an array of events that the given component instance provides.
            ///  This may differ from the set of events the class provides.
            ///  If the component is sited, the site may add or remove additional events.
            /// </summary>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
                return EventDescriptorCollection.Empty;
            }

            /// <summary>
            ///  Retrieves an array of events that the given component instance provides.
            ///  This may differ from the set of events the class provides.
            ///  If the component is sited, the site may add or remove additional events.
            ///  The returned array of events will be filtered by the given set of attributes.
            /// </summary>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
                return EventDescriptorCollection.Empty;
            }

            /// <summary>
            ///  Retrieves an array of properties that the given component instance provides.
            ///  This may differ from the set of properties the class provides.
            ///  If the component is sited, the site may add or remove additional properties.
            /// </summary>
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
                return _properties;
            }

            /// <summary>
            ///  Retrieves an array of properties that the given component instance provides.
            ///  This may differ from the set of properties the class provides.
            ///  If the component is sited, the site may add or remove additional properties.
            ///  The returned array of properties will be filtered by the given set of attributes.
            /// </summary>
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
                return _properties;
            }

            /// <summary>
            ///  Retrieves the object that directly depends on this value being edited.
            ///  This is generally the object that is required for the PropertyDescriptor's GetValue and SetValue  methods.
            ///  If 'null' is passed for the PropertyDescriptor, the ICustomComponent descripotor implemementation should return the default object,
            ///  that is the main object that exposes the properties and attributes
            /// </summary>
            object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
            {
                return this;
            }
        }

        /// <summary>
        ///  This is a single entry in our list box. It contains the value we're editing as well
        ///  as accessors for the type converter and UI editor.
        /// </summary>
        private class ListItem
        {
            private object _value;
            private object _uiTypeEditor;
            private readonly AdvCollectionEditor _parentCollectionEditor;

            public ListItem(AdvCollectionEditor parentCollectionEditor, object value)
            {
                _value = value;
                _parentCollectionEditor = parentCollectionEditor;
            }

            public override string ToString()
            {
                return _parentCollectionEditor.GetDisplayText(_value);
            }

            public UITypeEditor Editor
            {
                get
                {
                    if (_uiTypeEditor is null)
                    {
                        _uiTypeEditor = TypeDescriptor.GetEditor(_value, typeof(UITypeEditor));
                        if (_uiTypeEditor is null)
                        {
                            _uiTypeEditor = this;
                        }
                    }

                    if (_uiTypeEditor != this)
                    {
                        return (UITypeEditor)_uiTypeEditor;
                    }

                    return null;
                }
            }

            public object Value
            {
                get => _value;
                set
                {
                    _uiTypeEditor = null;
                    _value = value;
                }
            }
        }

        /// <summary>
        ///  Menu items we attach to the drop down menu if there are multiple types the collection editor can create.
        /// </summary>
        private class TypeMenuItem : ToolStripMenuItem
        {
            public TypeMenuItem(Type itemType, EventHandler handler) : base(itemType.Name, null, handler)
            {
                ItemType = itemType;
            }

            public Type ItemType { get; }
        }
    }

}
