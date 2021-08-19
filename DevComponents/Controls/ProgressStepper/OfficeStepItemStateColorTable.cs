using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DevComponents.DotNetBar.Rendering
{
    /// <summary>
    /// Represents the color table for StepItem single state.
    /// </summary>
    public class OfficeStepItemStateColorTable
    {
        /// <summary>
        /// Initializes a new instance of the OfficeStepItemStateColorTable class.
        /// </summary>
        public OfficeStepItemStateColorTable()
        {
            
        }
        /// <summary>
        /// Initializes a new instance of the OfficeStepItemStateColorTable class.
        /// </summary>
        /// <param name="backColors"></param>
        /// <param name="textColor"></param>
        /// <param name="borderColors"></param>
        public OfficeStepItemStateColorTable(Color[] backColors, Color textColor, Color[] borderColors)
        {
            BackColors = backColors;
            TextColor = textColor;
            BorderColors = borderColors;
        }
        /// <summary>
        /// Initializes a new instance of the OfficeStepItemStateColorTable class.
        /// </summary>
        /// <param name="backColors"></param>
        /// <param name="backColorsGradientAngle"></param>
        /// <param name="backColorsPositions"></param>
        /// <param name="textColor"></param>
        /// <param name="borderColors"></param>
        public OfficeStepItemStateColorTable(Color[] backColors, int backColorsGradientAngle, float[] backColorsPositions, Color textColor, Color[] borderColors)
        {
            BackColors = backColors;
            BackColorsGradientAngle = backColorsGradientAngle;
            BackColorsPositions = backColorsPositions;
            TextColor = textColor;
            BorderColors = borderColors;
        }
        /// <summary>
        /// Gets or sets the background colors for the step item.
        /// </summary>
        public Color[] BackColors= new Color[0];
        /// <summary>
        /// Gets or sets the back colors gradient angle if there is more than one color in BackColors array.
        /// </summary>
        public int BackColorsGradientAngle = 90;
        /// <summary>
        /// Gets or sets the gradient colors positions if there is more than one color in BackColors array.
        /// </summary>
        public float[] BackColorsPositions = new float[0];
        /// <summary>
        /// Gets or sets the text color for the step item.
        /// </summary>
        public Color TextColor = Color.Black;
       /// <summary>
       /// Gets or sets the border colors of the step item.
       /// </summary>
        public Color[] BorderColors = new Color[0];
    }
}
