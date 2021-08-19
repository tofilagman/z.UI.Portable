using System;
using System.Text;
using System.Drawing;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Provides data for the Slider item rendering events.
    /// </summary>
    public class StepItemRendererEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the reference to the item being rendered.
        /// </summary>
        public StepItem Item = null;

        /// <summary>
        /// Gets or sets the reference to graphics object.
        /// </summary>
        public Graphics Graphics = null;

        internal ItemPaintArgs ItemPaintArgs = null;

        /// <summary>
        /// Creates new instance of the object and initializes it with default values.
        /// </summary>
        /// <param name="overflowItem">Reference to the Slider item being rendered.</param>
        /// <param name="g">Reference to the graphics object.</param>
        public StepItemRendererEventArgs(StepItem item, Graphics g)
        {
            this.Item = item;
            this.Graphics = g;
        }
    }
}
