using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevComponents.DotNetBar;

namespace DevComponents.AdvTree.Display
{
    internal class NodeExpandTriangleDisplay : NodeExpandDisplay
    {
        /// <summary>
        /// Draw triangular type expand button.
        /// </summary>
        /// <param name="e">Expand button context information.</param>
        public override void DrawExpandButton(NodeExpandPartRendererEventArgs e)
        {
            if (e.ExpandPartBounds.IsEmpty)
                return;

            SmoothingMode sm = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int pw = Dpi.Width5;
            Rectangle r = new Rectangle(e.ExpandPartBounds.X , e.ExpandPartBounds.Y + (e.ExpandPartBounds.Height - 8) / 2, pw, Dpi.Height8);

            GraphicsPath path = null;
            if (e.Node.Expanded)
            {
                path = new GraphicsPath();
                path.AddLine(r.X, r.Y + pw, r.X + pw, r.Y);
                path.AddLine(r.X + pw, r.Y, r.X + pw, r.Y + pw);
                path.CloseAllFigures();
            }
            else
            {
                path = new GraphicsPath();
                path.AddLine(r.X, r.Y, r.X, r.Bottom);
                path.AddLine(r.X, r.Bottom, r.X + Dpi.Width4, r.Y + r.Height / 2);
                path.CloseAllFigures();
            }

            Brush brush = GetBackgroundBrush(e);
            if (brush != null)
            {
                e.Graphics.FillPath(brush, path);
                brush.Dispose();
            }

            Pen pen = GetBorderPen(e);
            if(pen!=null)
            {
                e.Graphics.DrawPath(pen, path);
                pen.Dispose();
            }
            e.Graphics.SmoothingMode = sm;
            if(path!=null) path.Dispose();
        }
    }
}
