using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace DevComponents.DotNetBar.Metro.Rendering
{
    internal class MetroRibbonTabGroupPainter : Office2007RibbonTabGroupPainter
    {
        protected override void PaintTabGroupBackground(System.Drawing.Graphics g, DevComponents.DotNetBar.Rendering.Office2007RibbonTabGroupColorTable colorTable, System.Drawing.Rectangle bounds, System.Drawing.Rectangle groupBounds, bool glassEnabled)
        {
            DisplayHelp.FillRectangle(g, groupBounds, colorTable.Background);

            if (StyleManager.Style != eStyle.Office2016)
            {
                Rectangle top = new Rectangle(groupBounds.X, groupBounds.Y, groupBounds.Width, 3);
                DisplayHelp.FillRectangle(g, top, colorTable.Border);
                if (StyleManager.Style == eStyle.OfficeMobile2014)
                {
                    DisplayHelp.DrawGradientLine(g, groupBounds.X, groupBounds.Y, groupBounds.X, 24, colorTable.Border, 1);
                    DisplayHelp.DrawGradientLine(g, groupBounds.Right - 1, groupBounds.Y, groupBounds.Right - 1, 24, colorTable.Border, 1);
                }
            }
        }
    }
}
