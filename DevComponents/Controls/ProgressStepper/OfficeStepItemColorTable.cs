using System;
using System.Collections.Generic;
using System.Text;

namespace DevComponents.DotNetBar.Rendering
{
    /// <summary>
    /// Defines color table for the StepItem.
    /// </summary>
    public class OfficeStepItemColorTable
    {
        /// <summary>
        /// Gets or sets the default state StepItem colors.
        /// </summary>
        public OfficeStepItemStateColorTable Default = new OfficeStepItemStateColorTable();
        /// <summary>
        /// Gets or sets the mouse over state StepItem colors.
        /// </summary>
        public OfficeStepItemStateColorTable MouseOver = new OfficeStepItemStateColorTable();
        /// <summary>
        /// Gets or sets the StepItem colors when mouse is pressed over the item.
        /// </summary>
        public OfficeStepItemStateColorTable Pressed = new OfficeStepItemStateColorTable();
        /// <summary>
        /// Gets or sets the StepItem colors when Value property is greater than Minimum property value, i.e. item is reporting progress.
        /// Note that only Background color is used when progress indicator is drawn.
        /// </summary>
        public OfficeStepItemStateColorTable Progress = new OfficeStepItemStateColorTable();
    }
}
