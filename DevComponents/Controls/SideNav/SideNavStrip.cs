using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using DevComponents.DotNetBar.Rendering;
using System.Drawing;
using DevComponents.AdvTree.Layout;

namespace DevComponents.DotNetBar.Controls
{
    [ToolboxItem(false)]
    public class SideNavStrip : ItemControl
    {
        #region Constructor
        private SimpleItemContainer _ItemContainer = null;

        public SideNavStrip()
        {
            _ItemContainer = new SimpleItemContainer();
            _ItemContainer.GlobalItem = false;
            _ItemContainer.ContainerControl = this;
            _ItemContainer.Stretch = true;
            _ItemContainer.Displayed = true;
            _ItemContainer.Style = eDotNetBarStyle.StyleManagerControlled;
            _ItemContainer.LayoutOrientation = eOrientation.Vertical;
            //base.AutoSize = true;
            this.ColorScheme.Style = eDotNetBarStyle.StyleManagerControlled;
            _ItemContainer.SetOwner(this);
            this.SetBaseItemContainer(_ItemContainer);
            this.BackgroundStyle.Class = ElementStyleClassKeys.SideNavStripKey;
            StyleManager.Register(this);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called by StyleManager to notify control that style on manager has changed and that control should refresh its appearance if
        /// its style is controlled by StyleManager.
        /// </summary>
        /// <param name="newStyle">New active style.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void StyleManagerStyleChanged(eDotNetBarStyle newStyle)
        {
            _ItemContainer.NeedRecalcSize = true;
        }

        /// <summary>
        /// Returns collection of items on a bar.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
        public SubItemsCollection Items
        {
            get
            {
                return _ItemContainer.SubItems;
            }
        }

        /// <summary>
        /// Gets currently selected item.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SideNavItem SelectedItem
        {
            get
            {
                foreach (BaseItem item in this.Items)
                {
                    if (item is SideNavItem && ((SideNavItem)item).Checked)
                    {
                        return (SideNavItem)item;
                    }
                }
                return null;
            }
            set
            {
                if (value != null && value.Panel != null)
                    value.Checked = true;
            }
        }

        protected override void PaintControl(ItemPaintArgs pa)
        {
            base.PaintControl(pa);
            SideNavItem selectedItem = this.SelectedItem;
            if (selectedItem != null)
            {
                Color color = GetBorderColor();
                if (!color.IsEmpty)
                {
                    Graphics g = pa.Graphics;
                    using (Pen pen = new Pen(color))
                    {
                        if (selectedItem.TopInternal > 0)
                            g.DrawLine(pen, this.Width - 1, 0, this.Width - 1, selectedItem.TopInternal - 1);
                        if (selectedItem.Bounds.Bottom < this.Height)
                            g.DrawLine(pen, this.Width - 1, selectedItem.Bounds.Bottom, this.Width - 1, this.Height);
                    }


                }
            }
        }

        private Color GetBorderColor()
        {
            SideNavColorTable ct = GetColorTable();
            if (ct.BorderColors != null && ct.BorderColors.Length > 0)
                return ct.BorderColors[0];
            return Color.Empty;
        }

        private SideNavColorTable GetColorTable()
        {
            return ((Office2007Renderer)GlobalManager.Renderer).ColorTable.SideNav;
        }
        internal void SelectFirstItem()
        {
            foreach (BaseItem item in this.Items)
            {
                if (item.Visible && item.Enabled && item is SideNavItem)
                {
                    ((SideNavItem)item).Checked = true;
                    break;
                }
            }
        }

        protected override void OnButtonCheckedChanged(ButtonItem item, EventArgs e)
        {
            if (this.Parent is SideNav)
                ((SideNav)this.Parent).UpdateSelectedItemTitle();
            base.OnButtonCheckedChanged(item, e);
        }

        #endregion
    }
}
