using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;

namespace DevComponents.DotNetBar
{
	/// <summary>
	/// Statis functions for design-time support.
	/// </summary>
	internal class DesignTime
	{
		public static void DrawDesignTimeSelection(Graphics g, Rectangle r, Color c)
		{
			bool antiAlias=false;
			if(g.SmoothingMode==System.Drawing.Drawing2D.SmoothingMode.AntiAlias)
			{
				antiAlias=true;
				g.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.Default;
			}
            g.DrawRectangle(Pens.White, r);
			using(Pen pen=new Pen(c,1))
			{
				pen.DashStyle=DashStyle.Dot;
				g.DrawRectangle(pen,r);
                //r.Inflate(-1,-1);
                //g.DrawRectangle(pen,r);
			}

			if(antiAlias)
				g.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
		}

        public static void DrawDesignTimeSelection(Graphics g, GraphicsPath path, Color c)
        {
            bool antiAlias = false;
            if (g.SmoothingMode == System.Drawing.Drawing2D.SmoothingMode.AntiAlias)
            {
                antiAlias = true;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
            g.DrawPath(Pens.White, path);
            using (Pen pen = new Pen(c, 1))
            {
                pen.DashStyle = DashStyle.Dot;
                g.DrawPath(pen, path);
            }

            if (antiAlias)
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }
	}
}
