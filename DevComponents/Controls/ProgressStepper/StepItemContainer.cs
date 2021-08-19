using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Container for the StepItem objects.
    /// </summary>
    public class StepItemContainer : ImageItem, IDesignTimeProvider
    {
        #region Implementation
        /// <summary>
        /// Initializes a new instance of the StepContainer class.
        /// </summary>
        public StepItemContainer()
        {
            m_IsContainer = true;
            this.AutoCollapseOnClick = true;
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
        }
        /// <summary>
        /// Returns copy of the item.
        /// </summary>
        public override BaseItem Copy()
        {
            StepItemContainer objCopy = new StepItemContainer();
            objCopy.Name = this.Name;
            this.CopyToItem(objCopy);
            return objCopy;
        }

        /// <summary>
        /// Copies the StepContainer specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New StepContainer instance.</param>
        internal void InternalCopyToItem(StepItemContainer copy)
        {
            CopyToItem(copy);
        }

        /// <summary>
        /// Copies the StepContainer specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New StepContainer instance.</param>
        protected override void CopyToItem(BaseItem copy)
        {
            StepItemContainer c = copy as StepItemContainer;
            base.CopyToItem(c);


        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void RecalcSize()
        {
            bool first = true;
            Point loc = this.Bounds.Location;
            Size totalSize = Size.Empty;
            bool uniformHeight = true;
            SubItemsCollection subItems = this.SubItems;
            bool last = true;
            int startIndex = subItems.Count - 1;
            for (int i = startIndex; i >= 0; i--)
            {
                StepItem item = subItems[i] as StepItem;
                if (item != null && item.Visible)
                {
                    item.IsLast = last;
                    last = false;
                }
            }
            foreach (BaseItem item in subItems)
            {
                item.DesignMarkerOrientation = eDesignMarkerOrientation.Horizontal;
                StepItem si = item as StepItem;
                if (!item.Visible)
                {
                    item.Displayed = false;
                    if (si != null)
                    {
                        si.IsLast = false;
                    }
                    continue;
                }
                item.Displayed = true;
                if (si != null)
                {
                    si.IsFirst = first;
                    first = false;
                    si.RecalcSize();
                    si.SetDisplayRectangle(new Rectangle(loc, si.Size));
                    int stepWidth = (si.Size.Width - _PointerSize - 1); // 1 is so right hand line overlaps
                    loc.X += stepWidth;
                    totalSize.Width += stepWidth;
                }
                else
                {
                    first = true;
                    item.RecalcSize();
                    item.SetDisplayRectangle(new Rectangle(loc, item.Size));
                    loc.X += item.WidthInternal;
                    totalSize.Width += item.WidthInternal;
                }
                if (uniformHeight && totalSize.Height > 0 && totalSize.Height != item.HeightInternal)
                    uniformHeight = false;
                totalSize.Height = Math.Max(totalSize.Height, item.HeightInternal);
            }
            totalSize.Width += _PointerSize;

            // Vertically center items if they are all not of same height
            if (!uniformHeight)
            {
                foreach (BaseItem item in subItems)
                {
                    if (!item.Visible)
                        continue;
                    if (item.HeightInternal < totalSize.Height)
                        item.TopInternal = (totalSize.Height - item.HeightInternal) / 2;
                }
            }
            m_Rect.Size = totalSize;
            _CalculatedHeight = totalSize.Height;
            _CalculatedWidth = totalSize.Width;
            base.RecalcSize();
        }

        public override void Paint(ItemPaintArgs p)
        {
            ItemDisplay display = GetItemDisplay();
            display.Paint(this, p);
        }

        private ItemDisplay m_ItemDisplay = null;
        internal ItemDisplay GetItemDisplay()
        {
            if (m_ItemDisplay == null)
            {
                m_ItemDisplay = new ItemDisplay();
            }
            return m_ItemDisplay;
        }

        private int _PointerSize = 10;
        /// <summary>
        /// Gets or sets the arrow pointer width for the StepItem objects hosted within this container.
        /// </summary>
        [DefaultValue(10), Category("Appearance"), Description("Gets or sets the arrow pointer width for the StepItem objects hosted within this container.")]
        public int PointerSize
        {
            get { return _PointerSize; }
            set
            {
                if (value != _PointerSize)
                {
                    int oldValue = _PointerSize;
                    _PointerSize = value;
                    OnPointerSizeChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when PointerSize property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnPointerSizeChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("PointerSize"));

        }

        private int _CalculatedWidth;
        internal int CalculatedWidth
        {
            get { return _CalculatedWidth; }
            set
            {
                _CalculatedWidth = value;
            }
        }

        private int _CalculatedHeight;
        internal int CalculatedHeight
        {
            get { return _CalculatedHeight; }
            set
            {
                _CalculatedHeight = value;
            }
        }

        #endregion

        #region IDesignTimeProvider Implementation
        protected virtual InsertPosition GetContainerInsertPosition(Point pScreen, BaseItem dragItem)
        {
            return DesignTimeProviderContainer.GetInsertPosition(this, pScreen, dragItem);
        }
        InsertPosition IDesignTimeProvider.GetInsertPosition(Point pScreen, BaseItem dragItem)
        {
            return GetContainerInsertPosition(pScreen, dragItem);
        }
        void IDesignTimeProvider.DrawReversibleMarker(int iPos, bool Before)
        {
            DesignTimeProviderContainer.DrawReversibleMarker(this, iPos, Before);
        }
        void IDesignTimeProvider.InsertItemAt(BaseItem objItem, int iPos, bool Before)
        {
            DesignTimeProviderContainer.InsertItemAt(this, objItem, iPos, Before);
        }

        #endregion
    }
}
