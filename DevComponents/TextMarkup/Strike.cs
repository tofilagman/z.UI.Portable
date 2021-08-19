using System.Drawing;

 
namespace DevComponents.DotNetBar.TextMarkup
 
{
    internal class Strike : MarkupElement
    {
        public override void Measure(Size availableSize, MarkupDrawContext d)
        {
            Bounds = Rectangle.Empty;
        }

        public override void Render(MarkupDrawContext d)
        {
            d.StrikeOut = true;
        }

        public override void RenderEnd(MarkupDrawContext d)
        {
            d.StrikeOut = false;

            base.RenderEnd(d);
        }

        protected override void ArrangeCore(Rectangle finalRect, MarkupDrawContext d)
        {
        }
    }
}
