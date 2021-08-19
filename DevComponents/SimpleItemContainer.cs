using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Represents simple item container which orders items horizontally and support all ItemAlignment settings.
    /// </summary>
    [ToolboxItem(false)]
    public class SimpleItemContainer : ImageItem, IDesignTimeProvider
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the SimpleItemContainer class.
        /// </summary>
        public SimpleItemContainer()
        {
            // We contain other controls
            m_IsContainer = true;
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
        }
        #endregion

        #region Implementation
        public override void RecalcSize()
        {
            if (_LayoutOrientation == eOrientation.Vertical)
            {
                RecalcSizeVertical();
            }
            else
            {
                List<BaseItem> leftItems = new List<BaseItem>();
                List<BaseItem> centerItems = new List<BaseItem>();
                List<BaseItem> rightItems = new List<BaseItem>();

                int centerWidth = 0;
                Size minSize = SplitItems(leftItems, centerItems, rightItems, out centerWidth);
                Point loc = this.Bounds.Location;
                int bottom = this.Bounds.Bottom;
                loc.X += _PaddingLeft;
                if (this.WidthInternal == 0 || this.WidthInternal <= minSize.Width)
                {
                    BaseItem[] items = new BaseItem[leftItems.Count + centerItems.Count + rightItems.Count];
                    leftItems.CopyTo(items);
                    centerItems.CopyTo(items, leftItems.Count);
                    rightItems.CopyTo(items, leftItems.Count + centerItems.Count);
                    BaseItem previousItem = null;
                    // Just line items up
                    foreach (BaseItem item in items)
                    {
                        if (!item.Visible) continue;
                        item.Displayed = true;
                        loc.X -= GetOverlapSpacing(previousItem, item);
                        item.LeftInternal = loc.X;
                        if (IsVerticallyCentered(item))
                            item.TopInternal = loc.Y + (minSize.Height - item.HeightInternal)/2;
                        else
                            item.TopInternal = loc.Y + (minSize.Height - item.HeightInternal);
                        loc.X += item.WidthInternal + _ItemSpacing;
                        previousItem = item;
                    }
                    m_Rect.Size = minSize;
                }
                else
                {
                    int leftItemsRight = 0;
                    BaseItem previousItem = null;
                    foreach (BaseItem item in leftItems)
                    {
                        item.Displayed = true;
                        if (item.BeginGroup) loc.X += _BeginGroupSpacing;
                        loc.X -= GetOverlapSpacing(previousItem, item);
                        item.LeftInternal = loc.X;
                        if (IsVerticallyCentered(item))
                            item.TopInternal = loc.Y + (minSize.Height - item.HeightInternal)/2;
                        else
                            item.TopInternal = loc.Y + (minSize.Height - item.HeightInternal);
                        loc.X += item.WidthInternal + _ItemSpacing;
                        previousItem = item;
                    }
                    leftItemsRight = loc.X;

                    int rightItemsLeft = 0;
                    loc = new Point(this.Bounds.Right - _PaddingRight, this.Bounds.Y);
                    previousItem = null;
                    for (int i = rightItems.Count - 1; i >= 0; i--)
                    {
                        BaseItem item = rightItems[i];
                        loc.X -= item.WidthInternal;
                        item.Displayed = true;
                        loc.X += GetOverlapSpacing(previousItem, item);
                        item.LeftInternal = loc.X;
                        loc.X -= _ItemSpacing + (item.BeginGroup ? _BeginGroupSpacing : 0);
                        if (IsVerticallyCentered(item))
                            item.TopInternal = loc.Y + (minSize.Height - item.HeightInternal)/2;
                        else
                            item.TopInternal = loc.Y + (minSize.Height - item.HeightInternal);
                        previousItem = item;
                    }
                    rightItemsLeft = loc.X;

                    loc = new Point((this.WidthInternal - centerWidth)/2, this.Bounds.Y);
                    if (loc.X < leftItemsRight || loc.X + centerWidth > rightItemsLeft)
                    {
                        loc.X = leftItemsRight + ((rightItemsLeft - leftItemsRight) - centerWidth)/2;
                    }
                    previousItem = null;
                    foreach (BaseItem item in centerItems)
                    {
                        item.Displayed = true;
                        if (item.BeginGroup) loc.X += _BeginGroupSpacing;
                        loc.X -= GetOverlapSpacing(previousItem, item);
                        item.LeftInternal = loc.X;
                        if (IsVerticallyCentered(item))
                            item.TopInternal = loc.Y + (minSize.Height - item.HeightInternal)/2;
                        else
                            item.TopInternal = loc.Y + (minSize.Height - item.HeightInternal);
                        loc.X += item.WidthInternal + _ItemSpacing;
                        previousItem = item;
                    }
                    m_Rect.Height = minSize.Height;
                }

                if (m_Rect.Width < _MinimumSize.Width)
                    m_Rect.Width = _MinimumSize.Width;
                if (m_Rect.Height < _MinimumSize.Height)
                    m_Rect.Height = _MinimumSize.Height;
            }

            base.RecalcSize();
        }
        protected virtual bool IsVerticallyCentered(BaseItem item)
        {
            return !(item is DevComponents.DotNetBar.Metro.MetroTabItem);
        }
        private Size SplitItems(List<BaseItem> leftItems, List<BaseItem> centerItems, List<BaseItem> rightItems, out int centerWidth)
        {
            Size minSize = new Size();
            
            centerWidth = 0;
            BaseItem previousItem = null;
            foreach (BaseItem item in this.SubItems)
            {
                if (!item.Visible)
                {
                    item.Displayed = false;
                    continue;
                }
                item.RecalcSize();
                
                minSize.Width += item.WidthInternal + _ItemSpacing + (item.BeginGroup ? _BeginGroupSpacing : 0) - GetOverlapSpacing(previousItem, item);
                minSize.Height = Math.Max(item.HeightInternal, minSize.Height);
                if (item.ItemAlignment == eItemAlignment.Near)
                    leftItems.Add(item);
                else if (item.ItemAlignment == eItemAlignment.Center)
                {
                    centerItems.Add(item);
                    centerWidth += item.WidthInternal + _ItemSpacing;
                }
                else if (item.ItemAlignment == eItemAlignment.Far)
                    rightItems.Add(item);
                previousItem = item;
            }

            if (minSize.Width > 0) minSize.Width -= _ItemSpacing;
            if (centerWidth > 0) centerWidth -= _ItemSpacing;

            return minSize;
        }

        private int GetOverlapSpacing(BaseItem previousItem, BaseItem item)
        {
            if (previousItem == null || _OverlapSpacing <= 0 || _OverlapType == null ||
                !(_OverlapType.IsInstanceOfType(previousItem) && _OverlapType.IsInstanceOfType(item)))
                return 0;
            return _OverlapSpacing;
        }

        private bool IsHorizontallyCentered(BaseItem item)
        {
            return !(item is DevComponents.DotNetBar.Metro.MetroTabItem);
        }
        private Size SplitItemsVertical(List<BaseItem> topItems, List<BaseItem> centerItems, List<BaseItem> bottomItems, out int centerHeight)
        {
            Size minSize = new Size();
            centerHeight = 0;
            BaseItem previousItem = null;
            foreach (BaseItem item in this.SubItems)
            {
                if (!item.Visible)
                {
                    item.Displayed = false;
                    continue;
                }
                item.RecalcSize();
                minSize.Height += item.HeightInternal + _ItemSpacing + (item.BeginGroup ? _BeginGroupSpacing : 0) - GetOverlapSpacing(previousItem, item);
                minSize.Width = Math.Max(item.WidthInternal, minSize.Width);
                if (item.ItemAlignment == eItemAlignment.Near)
                    topItems.Add(item);
                else if (item.ItemAlignment == eItemAlignment.Center)
                {
                    centerItems.Add(item);
                    centerHeight += item.HeightInternal + _ItemSpacing;
                }
                else if (item.ItemAlignment == eItemAlignment.Far)
                    bottomItems.Add(item);
                previousItem = item;
            }

            if (minSize.Height > 0) minSize.Height -= _ItemSpacing;
            if (centerHeight > 0) centerHeight -= _ItemSpacing;

            return minSize;
        }
        private void RecalcSizeVertical()
        {
            List<BaseItem> leftItems = new List<BaseItem>();
            List<BaseItem> centerItems = new List<BaseItem>();
            List<BaseItem> rightItems = new List<BaseItem>();

            int centerHeight = 0;
            Size minSize = SplitItemsVertical(leftItems, centerItems, rightItems, out centerHeight);
            Point loc = this.Bounds.Location;
            int bottom = this.Bounds.Bottom;

            if (this.HeightInternal == 0 || this.HeightInternal <= minSize.Height)
            {
                BaseItem[] items = new BaseItem[leftItems.Count + centerItems.Count + rightItems.Count];
                leftItems.CopyTo(items);
                centerItems.CopyTo(items, leftItems.Count);
                rightItems.CopyTo(items, leftItems.Count + centerItems.Count);
                // Just line items up
                foreach (BaseItem item in items)
                {
                    if (!item.Visible) continue;
                    item.Displayed = true;
                    item.TopInternal = loc.Y;
                    item.LeftInternal = loc.X;
                    //if (IsHorizontallyCentered(item))
                    //    item.LeftInternal = loc.X + (minSize.Width - item.WidthInternal) / 2;
                    //else
                    //    item.LeftInternal = loc.X + (minSize.Width - item.WidthInternal);
                    item.WidthInternal = minSize.Width;
                    loc.Y += item.HeightInternal + _ItemSpacing;
                }
                m_Rect.Size = minSize;
            }
            else
            {
                int leftItemsRight = 0;
                foreach (BaseItem item in leftItems)
                {
                    item.Displayed = true;
                    if (item.BeginGroup) loc.Y += _BeginGroupSpacing;
                    item.TopInternal = loc.Y;
                    item.WidthInternal = minSize.Width;
                    if (IsHorizontallyCentered(item))
                        item.LeftInternal = loc.X + (minSize.Width - item.WidthInternal) / 2;
                    else
                        item.LeftInternal = loc.X + (minSize.Width - item.WidthInternal);
                    loc.Y += item.HeightInternal + _ItemSpacing;
                }
                leftItemsRight = loc.Y;

                int rightItemsLeft = 0;
                loc = new Point(this.Bounds.X, this.Bounds.Bottom);
                for (int i = rightItems.Count - 1; i >= 0; i--)
                {
                    BaseItem item = rightItems[i];
                    loc.Y -= item.HeightInternal;
                    item.Displayed = true;
                    item.TopInternal = loc.Y;
                    item.WidthInternal = minSize.Width;
                    loc.Y -= _ItemSpacing + (item.BeginGroup ? _BeginGroupSpacing : 0);
                    if (IsHorizontallyCentered(item))
                        item.LeftInternal = loc.X + (minSize.Width - item.WidthInternal) / 2;
                    else
                        item.LeftInternal = loc.X + (minSize.Width - item.WidthInternal);
                }
                rightItemsLeft = loc.Y;

                loc = new Point(this.Bounds.X, (this.HeightInternal - centerHeight) / 2);
                if (loc.Y < leftItemsRight || loc.Y + centerHeight > rightItemsLeft)
                {
                    loc.Y = leftItemsRight + ((rightItemsLeft - leftItemsRight) - centerHeight) / 2;
                }

                foreach (BaseItem item in centerItems)
                {
                    item.Displayed = true;
                    if (item.BeginGroup) loc.Y += _BeginGroupSpacing;
                    item.TopInternal = loc.Y;
                    item.WidthInternal = minSize.Width;
                    if (IsHorizontallyCentered(item))
                        item.LeftInternal = loc.X + (minSize.Width - item.WidthInternal) / 2;
                    else
                        item.LeftInternal = loc.X + (minSize.Width - item.WidthInternal);
                    loc.Y += item.HeightInternal + _ItemSpacing;
                }
                m_Rect.Width = minSize.Width;
            }

            if (m_Rect.Width < _MinimumSize.Width)
                m_Rect.Width = _MinimumSize.Width;
            if (m_Rect.Height < _MinimumSize.Height)
                m_Rect.Height = _MinimumSize.Height;
        }
        public override void Paint(ItemPaintArgs p)
        {
            foreach (BaseItem item in this.SubItems)
            {
                if (!item.Displayed) continue;
                if (item.BeginGroup)
                {
                    DisplayHelp.DrawLine(p.Graphics, item.LeftInternal -3, this.TopInternal + 4, item.LeftInternal - 3, this.Bounds.Bottom - 8, p.Colors.ItemSeparator, 1);
                }
                item.Paint(p);
            }
        }

        /// <summary>
        /// Returns copy of the item.
        /// </summary>
        public override BaseItem Copy()
        {
            SimpleItemContainer objCopy = new SimpleItemContainer();
            this.CopyToItem(objCopy);
            return objCopy;
        }

        /// <summary>
        /// Copies the item specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New instance.</param>
        internal void InternalCopyToItem(SimpleItemContainer copy)
        {
            CopyToItem(copy);
        }

        /// <summary>
        /// Copies the item specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New instance.</param>
        protected override void CopyToItem(BaseItem copy)
        {
            SimpleItemContainer c = copy as SimpleItemContainer;
            base.CopyToItem(c);
        }

        internal void SetSystemFocus()
        {
            if (m_HotSubItem != null || this.SubItems.Count == 0)
                return;

            BaseItem exp = this.ExpandedItem();
            if (exp != null)
            {
                m_HotSubItem = exp;
                m_HotSubItem.InternalMouseEnter();
                m_HotSubItem.InternalMouseMove(new MouseEventArgs(MouseButtons.None, 0, m_HotSubItem.LeftInternal + 2, m_HotSubItem.TopInternal + 2, 0));
                return;
            }

            foreach (BaseItem objItem in this.SubItems)
            {
                if (!objItem.SystemItem && objItem.Displayed && objItem.Visible)
                {
                    m_HotSubItem = objItem;
                    m_HotSubItem.InternalMouseEnter();
                    m_HotSubItem.InternalMouseMove(new MouseEventArgs(MouseButtons.None, 0, m_HotSubItem.LeftInternal + 2, m_HotSubItem.TopInternal + 2, 0));
                    break;
                }
            }
        }

        internal void ReleaseSystemFocus()
        {
            CollapseSubItems(this);
            if (m_HotSubItem != null)
            {
                Control c = this.ContainerControl as Control;
                if (c != null)
                {
                    Point p = c.PointToClient(Control.MousePosition);
                    if (m_HotSubItem.DisplayRectangle.Contains(p))
                        return;
                }
                m_HotSubItem.InternalMouseLeave();
                m_HotSubItem = null;
            }
        }

        private int _ItemSpacing;
        private Size _MinimumSize = Size.Empty;
        /// <summary>
        /// Gets or sets the minimum size of the container. Either Width or Height can be set or both. Default value is 0,0 which means
        /// that size is automatically calculated.
        /// </summary>
        [Browsable(false)]
        public Size MinimumSize
        {
            get { return _MinimumSize; }
            set
            {
                _MinimumSize = value;
                this.NeedRecalcSize = true;
                this.OnAppearanceChanged();
            }
        }

        //private bool ShouldSerializeMinimumSize()
        //{
        //    return !_MinimumSize.IsEmpty;
        //}

        private bool _SystemContainer = false;
        /// <summary>
        /// Returns whether instance of the item container is used as system container internally by DotNetBar.
        /// </summary>
        [Browsable(false)]
        public bool SystemContainer
        {
            get { return _SystemContainer; }
        }

        /// <summary>
        /// Sets whether container is used as system container internally by DotNetBar.
        /// </summary>
        /// <param name="b">true or false to indicate whether container is system container or not.</param>
        internal void SetSystemContainer(bool b)
        {
            _SystemContainer = b;
        }

        /// <summary>
        /// Indicates the spacing between items.
        /// </summary>
        public int ItemSpacing
        {
            get { return _ItemSpacing; }
            set
            {
                if (value != _ItemSpacing)
                {
                    int oldValue = _ItemSpacing;
                    _ItemSpacing = value;
                    OnItemSpacingChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when ItemSpacing property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnItemSpacingChanged(int oldValue, int newValue)
        {
            this.NeedRecalcSize = true;
            this.OnAppearanceChanged();
            //OnPropertyChanged(new PropertyChangedEventArgs("ItemSpacing"));
            
        }

        private int _BeginGroupSpacing = 6;
        /// <summary>
        /// Indicates additional spacing between item when its BeginGroup property is set.
        /// </summary>
        [DefaultValue(6), Category("Appearance"), Description("Indicates additional spacing between item when its BeginGroup property is set.")]
        public int BeginGroupSpacing
        {
            get { return _BeginGroupSpacing; }
            set { _BeginGroupSpacing = value; }
        }

        private eOrientation _LayoutOrientation = eOrientation.Horizontal;
        /// <summary>
        /// Specifies the layout orientation
        /// </summary>
        [DefaultValue(eOrientation.Horizontal), Category("Layout"), Description("Specifies the layout orientation")]
        public eOrientation LayoutOrientation {
            get { return _LayoutOrientation; }
            set
            {
                if (_LayoutOrientation != value)
                {
                    _LayoutOrientation = value;
                    NeedRecalcSize = true;
                }
            } 
        }

        private bool _EqualizeButtonSize = false;
        /// <summary>
        /// Indicates whether all items are resized to be of the size equal to largest item in the container
        /// </summary>
        [DefaultValue(false), Category("Layout"), Description("Indicates whether all items are resized to be of the size equal to largest item in the container")]
        public bool EqualizeButtonSize
        {
            get { return _EqualizeButtonSize; }
            set
            {
                if (_EqualizeButtonSize != value)
                {
                    _EqualizeButtonSize = value;
                    NeedRecalcSize = true;
                }
            }
        }

        private int _PaddingLeft = 0;
        /// <summary>
        /// Indicates left side padding within container.
        /// </summary>
        public int PaddingLeft
        {
            get { return _PaddingLeft; }
            set { _PaddingLeft = value; }
        }

        private int _PaddingRight;

        /// <summary>
        /// Indicates right side padding within container.
        /// </summary>
        public int PaddingRight
        {
            get { return _PaddingRight; }
            set { _PaddingRight = value; }
        }

        private int _OverlapSpacing = 0;
        /// <summary>
        /// Gets overlap spacing for specific item types specified by OverlapType in the container.
        /// </summary>
        public int OverlapSpacing
        {
            get { return _OverlapSpacing; }
            set { _OverlapSpacing = value; }
        }

        private Type _OverlapType = null;
        /// <summary>
        /// Gets the type of the item that will overlap.
        /// </summary>
        public Type OverlapType
        {
            get { return _OverlapType; }
            set { _OverlapType = value; }
        }
        #endregion

        #region IDesignTimeProvider
        public InsertPosition GetInsertPosition(Point pScreen, BaseItem DragItem)
        {
            return DesignTimeProviderContainer.GetInsertPosition(this, pScreen, DragItem);
        }
        public void DrawReversibleMarker(int iPos, bool Before)
        {
            DesignTimeProviderContainer.DrawReversibleMarker(this, iPos, Before);
            return;
        }

        public void InsertItemAt(BaseItem objItem, int iPos, bool Before)
        {
            DesignTimeProviderContainer.InsertItemAt(this, objItem, iPos, Before);
            return;
        }
        #endregion
    }
}
