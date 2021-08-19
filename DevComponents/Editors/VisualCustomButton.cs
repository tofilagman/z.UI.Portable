 
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using DevComponents.DotNetBar;

namespace DevComponents.Editors
{
    public class VisualCustomButton : VisualButton
    {
        #region Private Variables
        #endregion

        #region Events
        #endregion

        #region Constructor
        #endregion

        #region Internal Implementation
        protected override void PaintButtonBackground(PaintInfo p, DevComponents.DotNetBar.Rendering.Office2007ButtonItemStateColorTable ct)
        {
            base.PaintButtonBackground(p, ct);

            if (this.Text.Length == 0 && this.Image == null && string.IsNullOrEmpty(this.Symbol))
            {
                Point pt = new Point(RenderBounds.X + (RenderBounds.Width - Dpi.Width7) / 2, RenderBounds.Bottom - Dpi.Height6);
                using (SolidBrush brush = new SolidBrush(ct.Text))
                {
                    Size rs = new Size(Dpi.Width2, Dpi.Height2);
                    for (int i = 0; i < 3; i++)
                    {
                        //p.Graphics.FillRectangle(brush, new Rectangle(pt, rs));
                        p.Graphics.FillEllipse(brush, new Rectangle(pt, rs));
                        pt.X += rs.Width + Dpi.Width1;
                    }
                }
            }
        }
        #endregion

    }
}
 
