using System;
using System.Text;

namespace DevComponents.DotNetBar.Rendering
{
    /// <summary>
    /// Renders the Slider items.
    /// </summary>
    internal class SliderPainter
    {
        public virtual void Paint(SliderItemRendererEventArgs e) { }
    }

    /// <summary>
    /// Renders the Range Slider items.
    /// </summary>
    internal class RangeSliderPainter
    {
        public virtual void Paint(RangeSliderItemRendererEventArgs e) { }
    }
}
