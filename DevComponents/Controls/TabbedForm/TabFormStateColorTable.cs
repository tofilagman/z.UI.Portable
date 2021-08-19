using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DevComponents.DotNetBar.Metro;

namespace DevComponents.DotNetBar.Rendering
{
    /// <summary>
    /// Defines state color table for TabParentForm.
    /// </summary>
    public class TabFormStateColorTable
    {
        /// <summary>
        /// Gets or sets the colors for the top part of the background.
        /// </summary>
        public Color[] CaptionBackColors = new Color[0];
        /// <summary>
        /// Gets or sets the back colors gradient angle if there is more than one color in BackColors array.
        /// </summary>
        public int CaptionBackColorsGradientAngle = 90;
        /// <summary>
        /// Gets or sets the gradient colors positions if there is more than one color in BackColors array.
        /// </summary>
        public float[] CaptionBackColorsPositions = new float[0];

        /// <summary>
        /// Gets or sets the color of caption text.
        /// </summary>
        public Color CaptionText = Color.Empty;

        /// <summary>
        /// Gets or sets the border colors.
        /// </summary>
        public BorderColors BorderColors = new BorderColors(ColorScheme.GetColor("696969"));

        public TabFormStateColorTable()
        {
            
        }

        public TabFormStateColorTable(Color[] captionBackColors, Color captionText, BorderColors borderColors)
        {
            CaptionBackColors = captionBackColors;
            CaptionText = captionText;
            BorderColors = borderColors;
        }
    }
}
