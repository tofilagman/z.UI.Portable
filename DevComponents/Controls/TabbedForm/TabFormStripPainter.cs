using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DevComponents.DotNetBar.Controls;

namespace DevComponents.DotNetBar.Rendering
{
    internal abstract class TabFormStripPainter
    {
        public abstract void Paint(TabFormStripPainterArgs renderingInfo);
    }

    public class TabFormStripPainterArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets Graphics object group is rendered on.
        /// </summary>
        public Graphics Graphics = null;

        /// <summary>
        /// Gets or sets the reference to SwitchButtonItem being rendered.
        /// </summary>
        public TabFormStripControl TabFormStrip = null;

        /// <summary>
        /// Gets or sets the ItemPaintArgs reference.
        /// </summary>
        internal ItemPaintArgs ItemPaintArgs;

        /// <summary>
        /// Indicates whether to cancel system rendering of the item.
        /// </summary>
        public bool Cancel = false;

        public TabFormStripPainterArgs(TabFormStripControl tabFormStrip, Graphics graphics, ItemPaintArgs itemPaintArgs)
        {
            Graphics = graphics;
            TabFormStrip = tabFormStrip;
            ItemPaintArgs = itemPaintArgs;
        }
    }
}
