using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DevComponents.DotNetBar
{
    public class TabItemAccessibleObject : AccessibleObject
    {
        private TabItem _Item;
        public TabItem Item
        {
            get { return _Item; }
            set { _Item = value; }
        }

        public TabItemAccessibleObject(TabItem tabItem)
        {
            if (tabItem == null)
            {
                throw new ArgumentNullException("tabItem");
            }

            this.Item = tabItem;
        }

        #region public methods

        public override string Description
        {
            get
            {
                return Item.Name;
            }
        }

        public override string Value
        {
            get
            {
                return Item.Text;
            }
            set
            {
                Item.Text = value;
            }
        }

        public override AccessibleObject Parent
        {
            get
            {
                return Item.Parent.AccessibilityObject;
            }
        }

        public override System.Drawing.Rectangle Bounds
        {
            get
            {
                Rectangle bounds = this.Item.DisplayRectangle;
                Point location = this.Item.Parent.PointToScreen(bounds.Location);
                return new Rectangle(location, bounds.Size);
            }
        }

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            return base.Navigate(navdir);
        }

        public override void Select(AccessibleSelection flags)
        {
            Item.Parent.SelectedTab = Item;
            base.Select(flags);
        }

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.OutlineItem;
            }
        }

        #endregion

    }
}

