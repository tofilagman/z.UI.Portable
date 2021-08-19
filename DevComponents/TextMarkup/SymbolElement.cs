using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;

 
namespace DevComponents.DotNetBar.TextMarkup
 
{
    internal class SymbolElement : MarkupElement
    {
        #region Internal Implementation
        private Size _DefaultSize = new Size(16, 16);
        private eSymbol _Symbol = eSymbol.XInCircle;

        public override void Measure(System.Drawing.Size availableSize, MarkupDrawContext d)
        {
            this.Bounds = new Rectangle(Point.Empty, Dpi.Size(_DefaultSize));
        }

        protected override void ArrangeCore(System.Drawing.Rectangle finalRect, MarkupDrawContext d) { }

        public override void Render(MarkupDrawContext d)
        {
            Rectangle r = this.Bounds;
            r.Offset(d.Offset);

            if (!d.ClipRectangle.IsEmpty && !r.IntersectsWith(d.ClipRectangle))
                return;

            Graphics g = d.Graphics;
            Color color = d.CurrentForeColor;

            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_Symbol == eSymbol.XInCircle)
            {
                Rectangle sr = new Rectangle(r.X + (r.Width - Dpi.Width14) / 2, r.Y + (r.Height - Dpi.Height14) / 2, Dpi.Width14, Dpi.Height14);
                using (Pen pen = new Pen(color, 1.51f))
                {
                    //sr.Width-=2;
                    //sr.Height-=2;
                    g.DrawEllipse(pen, sr); 
                    g.DrawLine(pen, sr.X + Dpi.Width4, sr.Y + Dpi.Height4, sr.X + Dpi.Width10, sr.Y + Dpi.Height10);
                    g.DrawLine(pen, sr.X + Dpi.Width10, sr.Y + Dpi.Height4, sr.X + Dpi.Width4, sr.Y + Dpi.Height10);
                }
            }

            g.SmoothingMode = sm;

            this.RenderBounds = r;
        }

        public override void ReadAttributes(XmlTextReader reader)
        {
            _Symbol = eSymbol.XInCircle;

            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                if (reader.Name.ToLower() == "type")
                {
                    string s = reader.Value.ToLower();
                    if (s == "xincircle")
                        _Symbol = eSymbol.XInCircle;
                    break;
                }
            }
        }

        private enum eSymbol
        {
            XInCircle
        }

        /// <summary>
        /// Returns whether layout manager can start new line with this element.
        /// </summary>
        public override bool CanStartNewLine
        {
            get { return false; }
        }
        #endregion

    }
}
