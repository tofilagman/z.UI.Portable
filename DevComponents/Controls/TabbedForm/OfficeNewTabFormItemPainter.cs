using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using DevComponents.DotNetBar.Controls;

namespace DevComponents.DotNetBar.Rendering
{
    internal class OfficeNewTabFormItemPainter : Office2007ButtonItemPainter
    {
        protected override void PaintState(ButtonItem button, ItemPaintArgs pa, CompositeImage image, Rectangle r, bool isMouseDown)
        {
            if (r.IsEmpty || !IsItemEnabled(button, pa) || r.Width == 0 || r.Height == 0)
                return;

            NewTabFormItem tab = button as NewTabFormItem;
            if (tab == null || IsOnMenu(button, pa))
            {
                base.PaintState(button, pa, image, r, isMouseDown);
                return;
            }

            bool isOnMenu = pa.IsOnMenu;
            TabFormItemColorTable tabColorTable = GetColorTable(tab);
            if (tabColorTable == null)
                return;

            TabFormItemStateColorTable stateColors = GetStateColorTable(tabColorTable, tab);

            if (stateColors == null)
                return;

            r.X += Overlap;
            r.Width -= Overlap;
            
            Graphics g = pa.Graphics;
            Region oldClip = g.Clip;
            try
            {
                Rectangle rClip = GetTabBounds(tab);
                rClip.Inflate(2, 1);
                g.SetClip(rClip, CombineMode.Intersect);

                if (stateColors.BackColors != null && stateColors.BackColors.Length > 0 || tab.BackColors != null && tab.BackColors.Length > 0)
                {
                    using (Brush brush = DisplayHelp.CreateBrush(r, (tab.BackColors != null && tab.BackColors.Length > 0) ? tab.BackColors : stateColors.BackColors, stateColors.BackColorsGradientAngle, stateColors.BackColorsPositions))
                    {
                        GraphicsPath path = GetTabPath(r);
                        g.FillPath(brush, path);
                        tab.TabPath = path;
                    }
                }

                if (stateColors.BorderColors.Length > 0)
                {
                    Rectangle borderRect = r;
                    foreach (Color color in stateColors.BorderColors)
                    {
                        using (GraphicsPath path = GetTabPath(borderRect))
                        {
                            using (Pen pen = new Pen(color, Dpi.Width1))
                            {
                                pen.LineJoin = LineJoin.Round;
                                g.DrawPath(pen, path);
                            }
                        }
                        borderRect.Inflate(-Dpi.Width1, -Dpi.Width1);
                    }
                }

                g.Clip = oldClip;
            }
            finally
            {
                if (oldClip != null) oldClip.Dispose();
            }
        }

        private int Overlap
        {
            get { return TabFormItem.TabOverlap / 2; }
        }

        private Rectangle GetTabBounds(NewTabFormItem tab)
        {
            Rectangle r = tab.DisplayRectangle;
            r.X += Overlap;
            r.Width -= Overlap;
            using (GraphicsPath tabPath = GetTabPath(r))
                r = Rectangle.Round(tabPath.GetBounds());
            return r;
        }

        private GraphicsPath GetTabPath(Rectangle r)
        {
            GraphicsPath path = new GraphicsPath();
            //float factor = r.Height / 13.5f;

            path.AddLine(2, 0, 5, 14);
            path.AddLine(31, 14, 28, 0);

            //path.AddLine(29.5f, 14.5f, 6.5f, 14.5f);
            //path.AddCurve(new PointF[]
            //{
            //    new PointF(5.83333f,14.1667f ),
            //    new PointF(5.16667f,13.8333f),
            //    new PointF(4.66667f,13.1667f)
            //});
            //path.AddCurve(new PointF[]
            //{
            //    new PointF(4.16667f,12.5f),
            //    new PointF(3.83333f,11.5f),
            //    new PointF(3.16667f,9.83333f)
            //});
            //path.AddCurve(new PointF[]
            //{
            //    new PointF(2.5f,8.16667f ),
            //    new PointF(1.5f,5.83333f ),
            //    new PointF(1f,4.16667f)
            //});
            //path.AddCurve(new PointF[]
            //{
            //    new PointF(0.5f,2.5f),
            //    new PointF(0.5f,1.5f ),
            //    new PointF(0.5f,0.5f)
            //});
            //path.AddLine(0.5f, 0.5f, 25.5f, 0.5f);
            //path.AddCurve(new PointF[]
            //{
            //    new PointF(26.8333f,2.5f ),
            //    new PointF(28.1667f,4.5f ),
            //    new PointF(29.1667f,6.83333f)
            //});
            //path.AddCurve(new PointF[]
            //{
            //    new PointF(30.1667f,9.16667f ),
            //    new PointF(30.8333f,11.8333f ),
            //    new PointF(30.8333f,13.1667f)
            //});
            //path.AddCurve(new PointF[]
            //{
            //    new PointF(30.8333f,14.5f ),
            //    new PointF(30.1667f,14.5f ),
            //    new PointF(30.1667f,14.5f )
            //});

            path.CloseAllFigures();

            Matrix translateMatrix = new Matrix();
            translateMatrix.Translate(r.X, r.Y);
            if (Dpi.Factor.Width > 1)
                translateMatrix.Scale(Dpi.Factor.Width, Dpi.Factor.Width);
            path.Transform(translateMatrix);

            return path;
        }


        private TabFormItemColorTable GetColorTable(NewTabFormItem tab)
        {
            return this.ColorTable.TabFormItemColorTables[tab.GetColorTableName()];
        }

        internal static TabFormItemStateColorTable GetStateColorTable(TabFormItemColorTable tabColorTable, NewTabFormItem tab)
        {
            if (tabColorTable == null)
                return null;

            TabFormItemStateColorTable stateColors = null;

            if (!tab.RenderTabState)
                stateColors = tabColorTable.Default;
            else if (!tab.GetEnabled())
                stateColors = tabColorTable.Disabled;
            else if (tab.Checked)
                stateColors = tabColorTable.Selected;
            else if (tab.IsMouseOver)
                stateColors = tabColorTable.MouseOver;
            else
                stateColors = tabColorTable.Default;

            return stateColors;
        }
    }
}
