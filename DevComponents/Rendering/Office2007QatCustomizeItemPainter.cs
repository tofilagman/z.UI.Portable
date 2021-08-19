using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DevComponents.DotNetBar.Rendering
{
    /// <summary>
    /// Defines painter for the Office 2007 style QAT Customize Item.
    /// </summary>
    internal class Office2007QatCustomizeItemPainter : QatCustomizeItemPainter, IOffice2007Painter
    {
        #region IOffice2007Painter
        private Office2007ColorTable m_ColorTable = null; //new Office2007ColorTable();

        /// <summary>
        /// Gets or sets color table used by renderer.
        /// </summary>
        public Office2007ColorTable ColorTable
        {
            get { return m_ColorTable; }
            set { m_ColorTable = value; }
        }
        #endregion

        #region Internal Implementation
        public override void Paint(QatCustomizeItemRendererEventArgs e)
        {
            Graphics g = e.Graphics;
            QatCustomizeItem item = e.CustomizeItem;
            Rectangle r = item.DisplayRectangle;
            Region oldClip = null;
            if (g.Clip != null) oldClip = g.Clip as Region;
            g.SetClip(item.DisplayRectangle, CombineMode.Intersect);


            Office2007ButtonItemColorTable buttonColorTable = GetColorTable();
            Office2007ButtonItemStateColorTable state = buttonColorTable.Default;

            if (item.Expanded)
                state = buttonColorTable.Expanded;
            else if (item.IsMouseOver)
                state = buttonColorTable.MouseOver;

            if(StyleManager.IsMetro(item.EffectiveStyle))
                Office2007ButtonItemPainter.PaintBackground(g, state, r, RoundRectangleShapeDescriptor.RectangleShape);
            else
                Office2007ButtonItemPainter.PaintBackground(g, state, r, RoundRectangleShapeDescriptor.RoundCorner2);

            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.Default;
            
            Color color = state.ExpandBackground;
            Color colorLight = state.ExpandLight;

            Presentation.Shape shape = new Presentation.Shape();
            Presentation.ShapeBorder bl = new Presentation.ShapeBorder(colorLight, 1);
            Presentation.ShapeBorder b = new Presentation.ShapeBorder(color, 1);
            shape.Children.Add(new Presentation.Line(new Presentation.Location(0, 0), new Presentation.Location(Dpi.Width4, 0), b));
            shape.Children.Add(new Presentation.Line(new Presentation.Location(0, Dpi.Height1), new Presentation.Location(Dpi.Width4, Dpi.Height1), bl));

            shape.Children.Add(new Presentation.Line(new Presentation.Location(0, Dpi.Height3), new Presentation.Location(Dpi.Width4, Dpi.Height3), b));
            shape.Children.Add(new Presentation.Line(new Presentation.Location(Dpi.Width4, Dpi.Height3), new Presentation.Location(Dpi.Width2, Dpi.Height5), b));
            shape.Children.Add(new Presentation.Line(new Presentation.Location(Dpi.Width2, Dpi.Height5), new Presentation.Location(0, Dpi.Height3), b));
            shape.Children.Add(new Presentation.Line(new Presentation.Location(Dpi.Width1, Dpi.Height4), new Presentation.Location(Dpi.Width3, Dpi.Height4), b));
            shape.Children.Add(new Presentation.Line(new Presentation.Location(Dpi.Width4, Dpi.Height4), new Presentation.Location(Dpi.Width2, Dpi.Height6), bl));
            shape.Children.Add(new Presentation.Line(new Presentation.Location(Dpi.Width2, Dpi.Height6), new Presentation.Location(0, Dpi.Height4), bl));

            Rectangle sr = new Rectangle(r.X + (r.Width - Dpi.Width5) / 2, r.Y + (r.Height - Dpi.Height7) / 2, Dpi.Width5, Dpi.Height7);
            Presentation.ShapePaintInfo pi = new Presentation.ShapePaintInfo(g, sr);
            shape.Paint(pi);

            g.SmoothingMode = sm;

            if (oldClip != null)
            {
                g.Clip = oldClip;
                oldClip.Dispose();
            }
            else
                g.ResetClip();
        }

        protected virtual Office2007ButtonItemColorTable GetColorTable()
        {
            Office2007ColorTable colorTable = this.ColorTable;
            Office2007ButtonItemColorTable buttonColorTable = null;

            eButtonColor color = eButtonColor.Orange;
            buttonColorTable = colorTable.ButtonItemColors[Enum.GetName(typeof(eButtonColor), color)];

            if (buttonColorTable == null)
                return colorTable.ButtonItemColors[0];

            return buttonColorTable;
        }
        #endregion
    }
}
