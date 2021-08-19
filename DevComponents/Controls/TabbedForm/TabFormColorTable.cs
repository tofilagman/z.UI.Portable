using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DevComponents.DotNetBar.Metro;

namespace DevComponents.DotNetBar.Rendering
{
    public class TabFormColorTable
    {
        /// <summary>
        /// Gets or sets the color table for form in active state.
        /// </summary>
        public TabFormStateColorTable Active = new TabFormStateColorTable();

        /// <summary>
        /// Gets or sets the color table for from in inactive state.
        /// </summary>
        public TabFormStateColorTable Inactive = new TabFormStateColorTable();

        /// <summary>
        /// Gets or sets the border thickness.
        /// </summary>
        public Thickness BorderThickness = new Thickness(1, 1, 1, 1);

        /// <summary>
        /// Gets or sets the background color of the form.
        /// </summary>
        public Color BackColor = Color.Empty;

        /// <summary>
        /// Gets or sets the text color of the form.
        /// </summary>
        public Color TextColor = Color.Empty;

        /// <summary>
        /// Gets or sets the text formatting for caption text.
        /// </summary>
        public eTextFormat CaptionTextFormat = eTextFormat.VerticalCenter | eTextFormat.Left | eTextFormat.EndEllipsis | eTextFormat.NoPrefix;
    }
}
