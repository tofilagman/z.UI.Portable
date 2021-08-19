using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DevComponents.DotNetBar.Rendering
{
    public class TabFormItemColorTable
    {
        /// <summary>
        /// Gets or sets the name of the color table.
        /// </summary>
        public string Name = "";
        /// <summary>
        /// Gets or sets the default tab colors.
        /// </summary>
        public TabFormItemStateColorTable Default = new TabFormItemStateColorTable();
        /// <summary>
        /// Gets or sets the disabled tab colors.
        /// </summary>
        public TabFormItemStateColorTable Disabled = new TabFormItemStateColorTable();
        /// <summary>
        /// Gets or sets the selected tab colors.
        /// </summary>
        public TabFormItemStateColorTable Selected = new TabFormItemStateColorTable();
        /// <summary>
        /// Gets or sets the colors when mouse is over the tab but tab is not selected.
        /// </summary>
        public TabFormItemStateColorTable MouseOver = new TabFormItemStateColorTable();
        /// <summary>
        /// Gets or sets colors for the tab close button.
        /// </summary>
        public TabCloseButtonColorTable CloseButton = new TabCloseButtonColorTable();
    }

    /// <summary>
    /// Defines the color table for RibbonTabItem states.
    /// </summary>
    public class TabFormItemStateColorTable
    {
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
        /// Gets or sets the item border colors.
        /// </summary>
        public Color[] BorderColors = new Color[0];

        /// <summary>
        /// Creates a copy of the state color table.
        /// </summary>
        public TabFormItemStateColorTable Clone()
        {
            TabFormItemStateColorTable t=new TabFormItemStateColorTable();
            t.BorderColors = BorderColors;
            t.BackColors = BackColors;
            t.TextColor = TextColor;
            t.BackColorsGradientAngle = BackColorsGradientAngle;
            t.BackColorsPositions = BackColorsPositions;
            return t;
        }

    }

    /// <summary>
    /// Defines color table for TabFormItem close button displayed on tabs.
    /// </summary>
    public class TabCloseButtonColorTable
    {
        /// <summary>
        /// Colors for the button in default state.
        /// </summary>
        public TabCloseButtonStateColorTable Normal = new TabCloseButtonStateColorTable();
        /// <summary>
        /// Colors for button in mouse over state.
        /// </summary>
        public TabCloseButtonStateColorTable MouseOver = new TabCloseButtonStateColorTable(new Color[] { ColorScheme.GetColor(0xDB4336) }, ColorScheme.GetColor(0xFDFBFA), ColorScheme.GetColor(0xDB4336));
        /// <summary>
        /// Colors for button when pressed with mouse state.
        /// </summary>
        public TabCloseButtonStateColorTable Pressed = new TabCloseButtonStateColorTable(new Color[] { ColorScheme.GetColor(0xA8352A) }, ColorScheme.GetColor(0xFDFBFA), ColorScheme.GetColor(0xA8352A));
    }
    /// <summary>
    /// Defines state color table for TabFormItem close button displayed on tabs.
    /// </summary>
    public class TabCloseButtonStateColorTable
    {
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
        /// Indicates item sign color.
        /// </summary>
        public Color ForeColor = Color.Empty;
        /// <summary>
        /// Indicates item border color.
        /// </summary>
        public Color BorderColor = Color.Empty;

        public TabCloseButtonStateColorTable()
        {

        }

        public TabCloseButtonStateColorTable(Color[] backColors, Color foreColor, Color borderColor)
        {
            BackColors = backColors;
            ForeColor = foreColor;
            BorderColor = borderColor;
        }

    }
}
