using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Provides data for the Slider item rendering events.
    /// </summary>
    public class ListBoxItemRendererEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the reference to the item being rendered.
        /// </summary>
        public ListBoxItem Item = null;

        /// <summary>
        /// Gets or sets the reference to graphics object.
        /// </summary>
        public Graphics Graphics = null;

        internal ItemPaintArgs ItemPaintArgs = null;

        /// <summary>
        /// Creates new instance of the object and initializes it with default values.
        /// </summary>
        /// <param name="item">Reference to the ListBoxItem being rendered.</param>
        /// <param name="g">Reference to the graphics object.</param>
        public ListBoxItemRendererEventArgs(ListBoxItem item, Graphics g)
        {
            this.Item = item;
            this.Graphics = g;
        }
    }
}