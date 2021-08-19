using System;
using System.Text;
using System.Drawing;

 
namespace DevComponents.DotNetBar.TextMarkup
 
{
    internal class Underline : FontChangeElement
    {
        #region Internal Implementation
        protected override void SetFont(MarkupDrawContext d)
        {
            d.Underline = true;
            //Font font = d.CurrentFont;
            //FontStyle style = d.CurrentFont.Style | FontStyle.Underline;

            //if (!font.Underline && font.FontFamily.IsStyleAvailable(style))
            //    d.CurrentFont = new Font(font, style);
            //else
            //    font = null;

            //if (font != null)
            //    m_OldFont = font;

            base.SetFont(d);
        }

        public override void MeasureEnd(Size availableSize, MarkupDrawContext d)
        {
            d.Underline = false;
            base.MeasureEnd(availableSize, d);
        }

        public override void RenderEnd(MarkupDrawContext d)
        {
            d.Underline = false;
            base.RenderEnd(d);
        }
        #endregion
    }
}
