using DevComponents.DotNetBar.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DevComponents.DotNetBar.Rendering
{
    /// <summary>
    /// Provides data for the SideNavItem rendering events.
    /// </summary>
    public class SideNavItemRendererEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the reference to the item being rendered.
        /// </summary>
        public SideNavItem Item = null;

        /// <summary>
        /// Gets or sets the reference to graphics object.
        /// </summary>
        public Graphics Graphics = null;

        /// <summary>
        /// Indicates whether to cancel system rendering of the item.
        /// </summary>
        public bool Cancel = false;

        internal ItemPaintArgs ItemPaintArgs = null;

        /// <summary>
        /// Creates new instance of the object and initializes it with default values.
        /// </summary>
        /// <param name="item">Reference to the ListBoxItem being rendered.</param>
        /// <param name="g">Reference to the graphics object.</param>
        public SideNavItemRendererEventArgs(SideNavItem item, Graphics g)
        {
            this.Item = item;
            this.Graphics = g;
        }
    }
}
