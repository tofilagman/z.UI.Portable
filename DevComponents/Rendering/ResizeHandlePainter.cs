using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DevComponents.DotNetBar.Rendering
{
    internal static class ResizeHandlePainter
    {
        internal static void DrawResizeHandle(Graphics g, Rectangle statusBarBounds, Color lightColor, Color color, bool rightToLeft)
        {
            int direction = 1;
            Point startLoc = new Point(statusBarBounds.Right, statusBarBounds.Bottom);
            if (rightToLeft)
            {
                direction = -1;
                startLoc = new Point(0, statusBarBounds.Bottom - Dpi.Width2);
            }

            using (Pen pen = new Pen(lightColor, Dpi.Width1))
            {
                g.DrawLine(pen, startLoc.X - Dpi.Width2 * direction, startLoc.Y - Dpi.Height2,
                    startLoc.X - Dpi.Width3 * direction, startLoc.Y - Dpi.Height2);
                g.DrawLine(pen, startLoc.X - Dpi.Width6 * direction, startLoc.Y - Dpi.Height2,
                    startLoc.X - Dpi.Width7 * direction, startLoc.Y - Dpi.Height2);
                g.DrawLine(pen, startLoc.X - Dpi.Width10 * direction, startLoc.Y - Dpi.Height2,
                    startLoc.X - Dpi.Width11 * direction, startLoc.Y - Dpi.Height2);
                g.DrawLine(pen, startLoc.X - Dpi.Width2 * direction, startLoc.Y - Dpi.Height2,
                    startLoc.X - Dpi.Width2 * direction, startLoc.Y - Dpi.Height3);
                g.DrawLine(pen, startLoc.X - Dpi.Width6 * direction, startLoc.Y - Dpi.Height2,
                    startLoc.X - Dpi.Width6 * direction, startLoc.Y - Dpi.Height3);
                g.DrawLine(pen, startLoc.X - Dpi.Width10 * direction, startLoc.Y - Dpi.Height2,
                    startLoc.X - Dpi.Width10 * direction, startLoc.Y - Dpi.Height3);
                g.DrawLine(pen, startLoc.X - Dpi.Width2 * direction, startLoc.Y - Dpi.Height6,
                    startLoc.X - Dpi.Width3 * direction, startLoc.Y - Dpi.Height6);
                g.DrawLine(pen, startLoc.X - Dpi.Width6 * direction, startLoc.Y - Dpi.Height6,
                    startLoc.X - Dpi.Width7 * direction, startLoc.Y - Dpi.Height6);
                g.DrawLine(pen, startLoc.X - Dpi.Width2 * direction, startLoc.Y - Dpi.Height6,
                    startLoc.X - Dpi.Width2 * direction, startLoc.Y - Dpi.Height7);
                g.DrawLine(pen, startLoc.X - Dpi.Width6 * direction, startLoc.Y - Dpi.Height6,
                    startLoc.X - Dpi.Width6 * direction, startLoc.Y - Dpi.Height7);
                g.DrawLine(pen, startLoc.X - Dpi.Width2 * direction, startLoc.Y - Dpi.Height10,
                    startLoc.X - Dpi.Width3 * direction, startLoc.Y - Dpi.Height10);
                g.DrawLine(pen, startLoc.X - Dpi.Width2 * direction, startLoc.Y - Dpi.Height10,
                    startLoc.X - Dpi.Width2 * direction, startLoc.Y - Dpi.Height11);
            }

            // draw dark squares
            using (Pen pen = new Pen(color, Dpi.Width1))
            {
                g.DrawRectangle(pen, startLoc.X - Dpi.Width4 * direction, startLoc.Y - Dpi.Height4, Dpi.Width1, Dpi.Width1);
                g.DrawRectangle(pen, startLoc.X - Dpi.Width8 * direction, startLoc.Y - Dpi.Height4, Dpi.Width1, Dpi.Width1);
                g.DrawRectangle(pen, startLoc.X - Dpi.Width12 * direction, startLoc.Y - Dpi.Height4, Dpi.Width1, Dpi.Width1);
                g.DrawRectangle(pen, startLoc.X - Dpi.Width4 * direction, startLoc.Y - Dpi.Height8, Dpi.Width1, Dpi.Width1);
                g.DrawRectangle(pen, startLoc.X - Dpi.Width8 * direction, startLoc.Y - Dpi.Height8, Dpi.Width1, Dpi.Width1);
                g.DrawRectangle(pen, startLoc.X - Dpi.Width4 * direction, startLoc.Y - Dpi.Height12, Dpi.Width1, Dpi.Width1);
            }
        }
    }
}
