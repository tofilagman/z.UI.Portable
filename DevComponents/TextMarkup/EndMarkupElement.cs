using System;
using System.Text;
using System.Drawing;

 
namespace DevComponents.DotNetBar.TextMarkup
{
    internal class EndMarkupElement : MarkupElement
    {
        #region Internap Implementation
        private MarkupElement m_StartElement = null;

        public EndMarkupElement(MarkupElement startElement)
        {
            m_StartElement = startElement;
        }

        public override void Measure(System.Drawing.Size availableSize, MarkupDrawContext d)
        {
            m_StartElement.MeasureEnd(availableSize, d);
            this.Bounds = Rectangle.Empty;
        }

        public override void Render(MarkupDrawContext d)
        {
            m_StartElement.RenderEnd(d);
        }

        protected override void ArrangeCore(System.Drawing.Rectangle finalRect, MarkupDrawContext d)
        {
            this.Bounds = Rectangle.Empty;
        }

        /// <summary>
        /// Gets reference to markup start element.
        /// </summary>
        public MarkupElement StartElement
        {
            get { return m_StartElement; }
        }
        #endregion
    }
}
