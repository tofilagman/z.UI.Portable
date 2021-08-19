using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DevComponents.DotNetBar.Rendering
{
    /// <summary>
    /// Defines color table for SideNav control.
    /// </summary>
    public class SideNavColorTable
    {
        /// <summary>
        /// Gets or sets the color table for SideNavItem
        /// </summary>
        public SideNavItemColorTable SideNavItem = new SideNavItemColorTable();
        /// <summary>
        /// Gets or sets the background color of SideNav title bar.
        /// </summary>
        public Color TitleBackColor = Color.Empty;
        /// <summary>
        /// Gets or sets the border color of the title of SideNav control.
        /// </summary>
        public Color[] TitleBorderColors = new Color[0];
        /// <summary>
        /// Gets or sets the color of the strip which hosts the items.
        /// </summary>
        public Color ItemsBackColor = Color.Empty;
        /// <summary>
        /// Gets or sets the border color of the SideNav control.
        /// </summary>
        public Color[] BorderColors = new Color[0];
        /// <summary>
        /// Gets or sets the back color of panels that are attached to SideNavItem and displayed when SideNavItem is selected.
        /// </summary>
        public Color PanelBackColor = Color.Empty;
    }

    /// <summary>
    /// Defines color table for SideNavItem
    /// </summary>
    public class SideNavItemColorTable
    {
        /// <summary>
        /// Gets or sets the color table for Default state.
        /// </summary>
        public SideNavItemStateColorTable Default = new SideNavItemStateColorTable();
        /// <summary>
        /// Gets or sets the color table for MouseOver state.
        /// </summary>
        public SideNavItemStateColorTable MouseOver = new SideNavItemStateColorTable();
        /// <summary>
        /// Gets or sets the color table for Pressed state.
        /// </summary>
        public SideNavItemStateColorTable Pressed = new SideNavItemStateColorTable();
        /// <summary>
        /// Gets or sets the color table for Selected state.
        /// </summary>
        public SideNavItemStateColorTable Selected = new SideNavItemStateColorTable();
    }

    /// <summary>
    /// Defines state color table for SideNavItem
    /// </summary>
    public class SideNavItemStateColorTable
    {
        public SideNavItemStateColorTable()
        {
            
        }
        public SideNavItemStateColorTable(Color textColor)
        {
            this.TextColor = textColor;
        }
        public SideNavItemStateColorTable(Color textColor, Color[] backColors, Color[] borderColors)
        {
            this.TextColor = textColor;
            this.BackColors = backColors;
            this.BorderColors = borderColors;
        }

        public SideNavItemStateColorTable(Color textColor, Color[] backColors, Color[] borderColors, int cornerRadius)
        {
            this.TextColor = textColor;
            this.BackColors = backColors;
            this.BorderColors = borderColors;
            this.CornerRadius = cornerRadius;
        }

        /// <summary>
        /// Indicates item text color.
        /// </summary>
        public Color TextColor = Color.Black;
        /// <summary>
        /// Gets or sets the background colors for the item.
        /// </summary>
        public Color[] BackColors = new Color[0];
        /// <summary>
        /// Gets or sets the back colors gradient angle if there is more than one color in BackColors array.
        /// </summary>
        public int BackColorsGradientAngle = 90;
        /// <summary>
        /// Gets or sets the gradient colors positions if there is more than one color in BackColors array.
        /// </summary>
        public float[] BackColorsPositions = new float[0];
        /// <summary>
        /// Gets or sets the border colors for the item.
        /// </summary>
        public Color[] BorderColors = new Color[0];
        /// <summary>
        /// Indicates the corner radius.
        /// </summary>
        public int CornerRadius = 0;
    }
}
