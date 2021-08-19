using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using DevComponents.DotNetBar.Controls;

namespace DevComponents.DotNetBar.Rendering
{
    internal class OfficeTabFormItemPainter : Office2007ButtonItemPainter
    {
        protected override Rectangle GetTextRectangle(ButtonItem button, ItemPaintArgs pa, eTextFormat stringFormat, CompositeImage image)
        {
            Rectangle r = base.GetTextRectangle(button, pa, stringFormat, image);
            if (image == null && string.IsNullOrEmpty(button.SymbolRealized))
            {
                r.Inflate(-(TabFormItem.TabOverlap / 2), 0);
                //r.Inflate(-3, 0);
            }
            else
                r.X += TabFormItem.TabOverlap / 2 - TabFormItem.Offset;

            return r;
        }

        public override Rectangle GetImageRectangle(ButtonItem button, ItemPaintArgs pa, CompositeImage image)
        {
            Rectangle r = base.GetImageRectangle(button, pa, image);
            //if (button.ImagePosition == eImagePosition.Left)
            r.X += TabFormItem.TabOverlap / 2 - TabFormItem.Offset;
            //else if (button.ImagePosition == eImagePosition.Right)
            //   r.X -= TabOverlap/2 - Offset;
            return r;
        }

        public override eTextFormat GetStringFormat(ButtonItem button, ItemPaintArgs pa, CompositeImage image)
        {
            eTextFormat sf = base.GetStringFormat(button, pa, image);
            sf &= ~(sf & eTextFormat.EndEllipsis);
            return sf;
        }

        protected override void PaintState(ButtonItem button, ItemPaintArgs pa, CompositeImage image, Rectangle r, bool isMouseDown)
        {
            if (r.IsEmpty || !IsItemEnabled(button, pa) || r.Width == 0 || r.Height == 0)
                return;

            TabFormItem tab = button as TabFormItem;
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

            Graphics g = pa.Graphics;
            Region oldClip = g.Clip;
            try
            {
                Rectangle rClip = GetTabBounds(tab);
                rClip.Inflate(1, 0);
                g.SetClip(rClip, CombineMode.Intersect);

                eTabFormStripControlDock tabAlign = tab.TabAlignment;
                tab.TabPath = GetTabPath(r, tabAlign, true);
                if (stateColors.BackColors != null && stateColors.BackColors.Length > 0 || tab.BackColors != null && tab.BackColors.Length > 0)
                {
                    using (Brush brush = DisplayHelp.CreateBrush(r, (tab.BackColors != null && tab.BackColors.Length > 0) ? tab.BackColors : stateColors.BackColors, stateColors.BackColorsGradientAngle, stateColors.BackColorsPositions))
                    {
                        using (GraphicsPath path = GetTabPath(r, tabAlign, true))
                        {
                            g.FillPath(Brushes.White, path);
                            g.FillPath(brush, path);
                        }
                    }
                }

                if (stateColors.BorderColors.Length > 0)
                {
                    Rectangle borderRect = r;
                    foreach (Color color in stateColors.BorderColors)
                    {
                        using (GraphicsPath path = GetTabPath(borderRect, tabAlign, false))
                        {
                            using (Pen pen = new Pen(color, Dpi.Width1))
                                g.DrawPath(pen, path);
                        }
                        borderRect.Inflate(-Dpi.Width1, -Dpi.Width1);
                    }

                }

                if (tab.CloseButtonVisible)
                {
                    int closeButtonSize = Dpi.Width(TabFormItem.CloseButtonSize);
                    Rectangle closeBounds = new Rectangle(r.Right - (TabFormItem.TabOverlap + TabFormItem.Radius - TabFormItem.Offset), r.Y + (r.Height - closeButtonSize) / 2 + Dpi.Height1, closeButtonSize, closeButtonSize);
                    TabCloseButtonStateColorTable closeButtonColorTable = GetCloseButtonColorTable(tabColorTable, tab);
                    if (closeButtonColorTable.BackColors.Length > 0)
                    {
                        using (
                            Brush brush = DisplayHelp.CreateBrush(closeBounds, closeButtonColorTable.BackColors,
                                closeButtonColorTable.BackColorsGradientAngle, closeButtonColorTable.BackColorsPositions)
                            )
                        {
                            g.FillEllipse(brush, closeBounds);
                        }
                    }

                    if (!closeButtonColorTable.BorderColor.IsEmpty)
                    {
                        using (Pen pen = new Pen(closeButtonColorTable.BorderColor, Dpi.Width1))
                        {
                            g.DrawEllipse(pen, closeBounds);
                        }
                    }
                    Color fc = stateColors.TextColor;
                    if (!closeButtonColorTable.ForeColor.IsEmpty)
                        fc = closeButtonColorTable.ForeColor;


                    using (Pen pen = new Pen(fc, Dpi.Width2))
                    {
                        Rectangle rc = closeBounds;
                        rc.Inflate(-Dpi.Width3, -Dpi.Width3);

                        g.DrawLine(pen, rc.X, rc.Y, rc.Right, rc.Bottom);
                        g.DrawLine(pen, rc.Right, rc.Y, rc.X, rc.Bottom);

                    }

                    tab.CloseButtonBounds = closeBounds;
                }
                else
                    tab.CloseButtonBounds = Rectangle.Empty;

                if (tab.Checked)
                {
                    g.ResetClip();
                    using (Pen pen = new Pen(stateColors.BorderColors[0], Dpi.Width1))
                    {
                        Rectangle tb = GetTabBounds(tab);
                        tb.Inflate(-Dpi.Width2, 0);
                        TabFormItemsSimpleContainer cont = tab.Parent as TabFormItemsSimpleContainer;
                        if (cont == null || cont.ClippingRectangle.IsEmpty || cont.ClippingRectangle.IntersectsWith(tb))
                        {
                            if (cont != null && !cont.ClippingRectangle.IsEmpty && cont.ClippingRectangle.IntersectsWith(tb) && tb.Right > cont.ClippingRectangle.Right)
                                tb.Width = Math.Max(1, cont.ClippingRectangle.Right - tb.X);

                            if (tabAlign == eTabFormStripControlDock.Bottom)
                            {
                                g.DrawLine(pen, 0, tb.Y, tb.X, tb.Y);
                                g.DrawLine(pen, tb.Right, tb.Y, pa.ContainerControl.Width, tb.Y);
                            }
                            else
                            {
                                g.DrawLine(pen, 0, tb.Bottom - 1, tb.X, tb.Bottom - 1);
                                g.DrawLine(pen, tb.Right, tb.Bottom - 1, pa.ContainerControl.Width, tb.Bottom - 1);
                            }
                        }
                        else
                        {
                            if (tabAlign == eTabFormStripControlDock.Bottom)
                            {
                                g.DrawLine(pen, 0, tb.Y, pa.ContainerControl.Width, tb.Y);
                            }
                            else
                            {
                                g.DrawLine(pen, 0, tb.Bottom - 1, pa.ContainerControl.Width, tb.Bottom - 1);
                            }
                        }
                    }
                }

                g.Clip = oldClip;
            }
            finally
            {
                if (oldClip != null) oldClip.Dispose();
            }
        }

        private TabCloseButtonStateColorTable GetCloseButtonColorTable(TabFormItemColorTable tabColorTable, TabFormItem tab)
        {
            if (tab.CloseButtonState == eButtonState.MouseDownLeft)
                return tabColorTable.CloseButton.Pressed;
            else if (tab.CloseButtonState == eButtonState.MouseOver)
                return tabColorTable.CloseButton.MouseOver;

            return tabColorTable.CloseButton.Normal;
        }

        private Rectangle GetTabBounds(TabFormItem tab)
        {
            Rectangle r = tab.DisplayRectangle;
            using (GraphicsPath tabPath = GetTabPath(r, tab.TabAlignment, false))
                r = Rectangle.Round(tabPath.GetBounds());
            return r;
        }
        private GraphicsPath GetTabPath(Rectangle r, eTabFormStripControlDock align, bool bCloseFigure)
        {
            return GetTopTabPath(r);
            Rectangle rbox = r;

            //if (align == eTabFormStripControlDock.Left)
            //{
            //    // Left
            //    rbox = new Rectangle(r.X, r.Y, r.Height, r.Width);
            //}
            //else if (align == eTabFormStripControlDock.Right)
            //{
            //    // Right
            //    rbox = new Rectangle(r.Right - r.Height, r.Y, r.Height, r.Width);
            //}

            GraphicsPath path = new GraphicsPath();

            if (align != eTabFormStripControlDock.Bottom)
            {
                Point[] p = new Point[4];
                int slope = Math.Min(rbox.Width > rbox.Height ? 16 : 10, rbox.Height / 2);
                p[0].X = rbox.X - (slope + Dpi.Width6);
                p[0].Y = rbox.Bottom - 1;
                p[1].X = p[0].X + Dpi.Width6;
                p[1].Y = p[0].Y - Dpi.Height4;
                p[2].X = p[1].X + slope - Dpi.Width4;
                p[2].Y = rbox.Y + Dpi.Height3;
                p[3].X = rbox.X + Dpi.Width3;
                p[3].Y = rbox.Y;
                path.AddCurve(p, 0, 3, Math.Max(.3f, 1f * Math.Min(1, slope / (float)rbox.Height)));

                //path.AddLine(p[3].X + 1, rbox.Y, rbox.Right-Dpi.Width4, rbox.Y);

                p = new Point[4];
                p[0].X = rbox.Right - Dpi.Width3;
                p[0].Y = rbox.Y;
                p[1].X = p[0].X + Dpi.Width7;
                p[1].Y = p[0].Y + Dpi.Height3;
                p[2].X = p[1].X + slope - Dpi.Width4;
                p[2].Y = rbox.Bottom - Dpi.Height4 - 1;
                p[3].X = p[2].X + Dpi.Width6;
                p[3].Y = rbox.Bottom - 1;
                path.AddCurve(p, 0, 3, Math.Max(.1f, 1f * Math.Min(1, slope / (float)rbox.Height)));

                if (bCloseFigure)
                {
                    path.AddLine(p[0].X, rbox.Bottom, rbox.Right, rbox.Bottom);
                    path.CloseAllFigures();
                }
            }
            else
            {
                Point[] p = new Point[4];
                int slope = Math.Min(18, rbox.Height / 2);

                p[0].X = rbox.X - (slope + Dpi.Width6);
                p[0].Y = rbox.Y;
                p[1].X = p[0].X + Dpi.Width6;
                p[1].Y = p[0].Y + Dpi.Height4;
                p[2].X = p[1].X + slope - Dpi.Width4;
                p[2].Y = rbox.Bottom - Dpi.Height3 - 1;
                p[3].X = rbox.X + Dpi.Width3;
                p[3].Y = rbox.Bottom - 1;
                path.AddCurve(p, 0, 3, Math.Max(.1f, 1f * Math.Min(1, slope / (float)rbox.Height)));

                //path.AddLine(p[3].X + 1, rbox.Y, rbox.Right-Dpi.Width4, rbox.Y);

                p = new Point[4];
                p[0].X = rbox.Right - Dpi.Width3;
                p[0].Y = rbox.Bottom;
                p[1].X = p[0].X + Dpi.Width6;
                p[1].Y = p[0].Y - Dpi.Height3;
                p[2].X = p[1].X + slope - Dpi.Width4;
                p[2].Y = rbox.Y + Dpi.Height4;
                p[3].X = p[2].X + Dpi.Width6;
                p[3].Y = rbox.Y;
                path.AddCurve(p, 0, 3, Math.Max(.1f, 1f * Math.Min(1, slope / (float)rbox.Height)));

                if (bCloseFigure)
                {
                    path.AddLine(p[0].X, rbox.Bottom, rbox.Right, rbox.Bottom);
                    path.CloseAllFigures();
                }
            }

            //if (align == eTabStripAlignment.Left)
            //{
            //    // Left
            //    Matrix m = new Matrix();
            //    //RectangleF rf=path.GetBounds();
            //    m.RotateAt(-90, new PointF(rbox.X, rbox.Bottom));
            //    m.Translate(rbox.Height, rbox.Width - rbox.Height, MatrixOrder.Append);
            //    path.Transform(m);
            //}
            //else if (align == eTabStripAlignment.Right)
            //{
            //    // Right
            //    Matrix m = new Matrix();
            //    //RectangleF rf=path.GetBounds();
            //    m.RotateAt(90, new PointF(rbox.Right, rbox.Bottom));
            //    m.Translate(-rbox.Height, rbox.Width - (rbox.Height - 1), MatrixOrder.Append);
            //    path.Transform(m);
            //}

            return path;
        }

        private GraphicsPath GetTopTabPath(Rectangle tabRect)
        {
            Rectangle r = tabRect;
            r.Width -= Dpi.Width1;
            //r.Height -= Dpi.Height(StripeBot);

            int k = Math.Min(20, r.Height);
            float scale = (float)k / 20;

            int delta = (int)(90 * scale) - 25;
            int n = (int)((TabFormItem.TabOverlap / 2f) * scale);

            // Create the path

            GraphicsPath path = new GraphicsPath();

            int dia = TabFormItem.Dia;
            Rectangle ar = new Rectangle(r.X - TabFormItem.Radius, r.Bottom - dia, dia, dia);
            path.AddArc(ar, 90, -delta);

            int offset = TabFormItem.Offset;
            ar = new Rectangle(r.X + TabFormItem.Radius + offset, r.Y, dia, dia);
            path.AddArc(ar, 270 - delta, delta);

            ar = new Rectangle(r.Right - (dia + offset + n), r.Y, dia, dia);
            path.AddArc(ar, 270, delta);

            ar = new Rectangle(r.Right - (dia - offset * 2), r.Bottom - dia, dia, dia);
            path.AddArc(ar, 90 + delta, -delta);

            return (path);
        }



        protected override Color GetTextColor(ButtonItem button, ItemPaintArgs pa)
        {
            if (!IsItemEnabled(button, pa) || !(button is TabFormItem))
                return base.GetTextColor(button, pa);

            TabFormItem tab = button as TabFormItem;
            if (!tab.ForeColor.IsEmpty)
                return tab.ForeColor;

            Color textColor = Color.Empty;

            TabFormItemStateColorTable ct = GetStateColorTable(GetColorTable(tab), tab);

            if (ct != null)
            {
                //if (pa.GlassEnabled && !ct.GlassText.IsEmpty)
                //    return ct.GlassText;
                textColor = ct.TextColor;
            }

            if (textColor.IsEmpty)
                return base.GetTextColor(button, pa);

            return textColor;
        }

        private TabFormItemColorTable GetColorTable(TabFormItem tab)
        {
            return this.ColorTable.TabFormItemColorTables[tab.GetColorTableName()];
        }

        internal static TabFormItemStateColorTable GetStateColorTable(TabFormItemColorTable tabColorTable, TabFormItem tab)
        {
            if (tab.CustomColorTable != null)
                tabColorTable = tab.CustomColorTable;

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
        //#if FRAMEWORK20
        //        public override void PaintButtonText(ButtonItem button, ItemPaintArgs pa, Color textColor, CompositeImage image)
        //        {
        //            eDotNetBarStyle effectiveStyle = button.EffectiveStyle;
        //            if (!((effectiveStyle == eDotNetBarStyle.Office2010 || StyleManager.IsMetro(effectiveStyle)) && pa.GlassEnabled))
        //            {
        //                base.PaintButtonText(button, pa, textColor, image);
        //                return;
        //            }

        //            Rectangle r = GetTextRectangle(button, pa, eTextFormat.HorizontalCenter | eTextFormat.VerticalCenter, image);
        //            //r.Offset(0, 3);
        //            //r.Height -= 2;
        //            ThemeTextFormat textFormat = ThemeTextFormat.Center | ThemeTextFormat.VCenter | ThemeTextFormat.HidePrefix | ThemeTextFormat.SingleLine;
        //            bool renderGlow = true;
        //            //if (effectiveStyle == eDotNetBarStyle.Office2010 && StyleManager.Style == eStyle.Office2010Black)
        //            //    renderGlow = false;
        //            Office2007RibbonControlPainter.PaintTextOnGlass(pa.Graphics, button.Text, pa.Font, r, textFormat, textColor, true, renderGlow, 10);
        //        }
        //#endif
    }
}
