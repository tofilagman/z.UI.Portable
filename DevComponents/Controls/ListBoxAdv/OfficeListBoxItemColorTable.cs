using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DevComponents.DotNetBar.Rendering
{
    /// <summary>
    /// Defines color table for ListBoxItem.
    /// </summary>
    public class OfficeListBoxItemColorTable
    {
        /// <summary>
        /// Specifies default state color table.
        /// </summary>
        public OfficeListBoxItemStateColorTable Default = new OfficeListBoxItemStateColorTable();
        /// <summary>
        /// Specifies mouse over state color table.
        /// </summary>
        public OfficeListBoxItemStateColorTable MouseOver = new OfficeListBoxItemStateColorTable();
        /// <summary>
        /// Specifies selected state color table.
        /// </summary>
        public OfficeListBoxItemStateColorTable Selected = new OfficeListBoxItemStateColorTable();
    }
    /// <summary>
    /// Defines single state color table for ListBoxItem.
    /// </summary>
    public class OfficeListBoxItemStateColorTable
    {
        /// <summary>
        /// Initializes a new instance of the OfficeListBoxItemStateColorTable class.
        /// </summary>
        public OfficeListBoxItemStateColorTable()
        {
        }
        /// <summary>
        /// Initializes a new instance of the OfficeListBoxItemStateColorTable class.
        /// </summary>
        /// <param name="textColor"></param>
        /// <param name="backColors"></param>
        /// <param name="backColorsGradientAngle"></param>
        /// <param name="backColorsPositions"></param>
        public OfficeListBoxItemStateColorTable(Color textColor, Color[] backColors, int backColorsGradientAngle, float[] backColorsPositions)
        {
            TextColor = textColor;
            BackColors = backColors;
            BackColorsGradientAngle = backColorsGradientAngle;
            BackColorsPositions = backColorsPositions;
        }
        /// <summary>
        /// Initializes a new instance of the OfficeListBoxItemStateColorTable class.
        /// </summary>
        /// <param name="textColor"></param>
        public OfficeListBoxItemStateColorTable(Color textColor)
        {
            TextColor = textColor;
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
    }
}
