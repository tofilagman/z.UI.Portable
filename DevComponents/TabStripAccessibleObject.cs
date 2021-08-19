using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar
{
    public class TabStripAccessibleObject : Control.ControlAccessibleObject
    {
        private TabStrip _Item;
        public TabStrip Item
        {
            get { return _Item; }
            set { _Item = value; }
        }

        public TabStripAccessibleObject(TabStrip tabStrip)
            : base(tabStrip)
        {
            if (tabStrip == null)
            {
                throw new ArgumentNullException("tabStrip");
            }

            this.Item = tabStrip;
        }

        #region public methods

        public override string Description
        {
            get
            {
                return Item.AccessibleDescription;
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
                bounds.Location = this.Item.PointToScreen(Point.Empty);
                return bounds;
            }
        }

        public override int GetChildCount()
        {
            return Item.Tabs.Count;
        }

        public override AccessibleObject GetChild(int index)
        {
            return Item.Tabs[index].AccessibleObject;
        }

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            return base.Navigate(navdir);
        }

        public override void Select(AccessibleSelection flags)
        {
            base.Select(flags);
        }
        public override AccessibleRole Role
        {
            get
            {
                return Item.AccessibleRole;
            }
        }

        #endregion

    }
}

