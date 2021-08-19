using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DevComponents.DotNetBar.Rendering
{
    /// <summary>
    /// Defines the color table for the Range Slider Item in single state.
    /// </summary>
    public class Office2010RangeSliderColorTable
    {
        /// <summary>
        /// Gets or sets the colors for the slider which changes the minimum value of the range.
        /// </summary>
        public Office2010RangeSliderPartStateColorTable MinRangeSlider = new Office2010RangeSliderPartStateColorTable();
        /// <summary>
        /// Gets or sets the colors for the slider which changes the maximum value of the range.
        /// </summary>
        public Office2010RangeSliderPartStateColorTable MaxRangeSlider = new Office2010RangeSliderPartStateColorTable();

        /// <summary>
        /// Gets or sets the tick line color.
        /// </summary>
        public Color TickLineColor = Color.Empty;
        /// <summary>
        
        /// <summary>
        /// Gets or sets the color for the line which indicates current range value.
        /// </summary>
        public Color RangeValueLineColor = Color.Empty;
        /// <summary>
        /// Gets or sets the background colors of the current range value.
        /// </summary>
        public GradientColorTable RangeValueBackground = null;

        /// <summary>
        /// Gets or sets the color for the line showing control range.
        /// </summary>
        public Color RangeLineColor = Color.Empty;
        /// <summary>
        /// Gets or sets the background colors for the line showing control range.
        /// </summary>
        public GradientColorTable RangeBackground = null;
        /// <summary>
        /// Gets or sets the corner radius for the range rectangle.
        /// </summary>
        public int RangeCornerRadius = 1;
    }

    /// <summary>
    /// Defines the color table for the Range Slider Item in single state.
    /// </summary>
    public class Office2010RangeSliderPartStateColorTable
    {
        /// <summary>
        /// Gets or sets the default state colors.
        /// </summary>
        public Office2010RangeChangePartColorTable Default = new Office2010RangeChangePartColorTable();

        /// <summary>
        /// Gets or sets the mouse over state colors.
        /// </summary>
        public Office2010RangeChangePartColorTable MouseOver = new Office2010RangeChangePartColorTable();

        /// <summary>
        /// Gets or sets the mouse pressed colors.
        /// </summary>
        public Office2010RangeChangePartColorTable Pressed = new Office2010RangeChangePartColorTable();

        /// <summary>
        /// Gets or sets the disabled colors.
        /// </summary>
        public Office2010RangeChangePartColorTable Disabled = new Office2010RangeChangePartColorTable();
    }

    public class Office2010RangeChangePartColorTable
    {
        /// <summary>
        /// Initializes a new instance of the Office2010RangeChangePartColorTable class.
        /// </summary>
        public Office2010RangeChangePartColorTable()
        {
        }
        /// <summary>
        /// Initializes a new instance of the Office2010RangeChangePartColorTable class.
        /// </summary>
        /// <param name="background"></param>
        /// <param name="borderColor"></param>
        /// <param name="borderLightColor"></param>
        public Office2010RangeChangePartColorTable(GradientColorTable background, Color borderColor, Color borderLightColor)
        {
            Background = background;
            BorderColor = borderColor;
            BorderLightColor = borderLightColor;
        }
        /// <summary>
        /// Gets or sets the part background colors.
        /// </summary>
        public GradientColorTable Background = null;

        /// <summary>
        /// Gets or sets the part border color
        /// </summary>
        public Color BorderColor = Color.Empty;

        /// <summary>
        /// Gets or sets the part border light color
        /// </summary>
        public Color BorderLightColor = Color.Empty;
    }
}
