using System;
using System.Collections.Generic;
using System.Text;
using DevComponents.DotNetBar.Rendering;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DevComponents.DotNetBar.Metro.Rendering
{
    internal class MetroAppButtonPainter : Office2007ButtonItemPainter
    {
        public override Office2007ButtonItemColorTable GetColorTable(ButtonItem button, eButtonContainer buttonCont)
        {
            Office2007ColorTable colorTable = this.ColorTable;
            Office2007ButtonItemColorTable buttonColorTable = null;

            if (button.CustomColorName != "")
                buttonColorTable = colorTable.ApplicationButtonColors[button.CustomColorName];

            if (buttonColorTable == null)
                buttonColorTable = colorTable.ApplicationButtonColors[button.GetColorTableName()];

            if (buttonColorTable == null && colorTable.ApplicationButtonColors.Count > 0)
                buttonColorTable = colorTable.ApplicationButtonColors[0];

            if (buttonColorTable == null) // Return fall back static table
                buttonColorTable = DevComponents.DotNetBar.Metro.ColorTables.MetroOfficeColorTableInitializer.GetAppFallBackColorTable();

            return buttonColorTable;
        }

        protected override Rectangle GetDisplayRectangle(ButtonItem button)
        {
            Rectangle r = button.DisplayRectangle;
            if (button is ApplicationButton && ((ApplicationButton)button).IsMenuOverlayPaint)
            {
                r.Width += 4;
                r.Height += 1;
            }
            return r;
        }

        private static readonly int RibbonFormWidthOffset = 2;
        public override void PaintButtonText(ButtonItem button, ItemPaintArgs pa, Color textColor, CompositeImage image)
        {
            Rectangle r = GetDisplayRectangle(button);
            r.Inflate(-1, -1);
            r.Width--;

            if (pa.ContainerControl.FindForm() is RibbonForm)
                r.Width -= RibbonFormWidthOffset+2;

            if (button is ApplicationButton && ((ApplicationButton)button).IsMenuOverlayPaint)
            {
                r.Height -= 1;
                r.Width +=3;
            }

            eTextFormat format = eTextFormat.HorizontalCenter | eTextFormat.VerticalCenter | eTextFormat.HidePrefix;
            TextDrawing.DrawString(pa.Graphics, button.Text, pa.Font, textColor, r, format);
        }

        protected override Color GetTextColor(ButtonItem button, ItemPaintArgs pa)
        {
            return base.GetTextColor(button, pa, GetColorTable(button, eButtonContainer.RibbonStrip));
        }

        public override void PaintButtonImage(ButtonItem button, ItemPaintArgs pa, CompositeImage image, Rectangle imagebounds)
        {
            if (!string.IsNullOrEmpty(button.Text)) return;
            Rectangle r = GetDisplayRectangle(button);
            r.X = r.X + (r.Width - imagebounds.Width) / 2;
            r.Y = r.Y + (r.Height - imagebounds.Height) / 2;
            r.Width = imagebounds.Width;
            r.Height = imagebounds.Height;
            base.PaintButtonImage(button, pa, image, r);
        }

        public override void PaintButtonBackground(ButtonItem button, ItemPaintArgs pa, CompositeImage image)
        {
            Office2007ButtonItemColorTable colors = GetColorTable(button, eButtonContainer.RibbonStrip);
            Office2007ButtonItemStateColorTable stateColors = colors.Default;
            
            if (button.IsMouseDown)
                stateColors = colors.Pressed;
            else if (button.IsMouseOver)
                stateColors = colors.MouseOver;
            Rectangle bounds = GetDisplayRectangle(button);
            bounds.Width--;
            //bounds.Height--;
            Graphics g = pa.Graphics;

            if (pa.ContainerControl.FindForm() is RibbonForm)
                bounds.Width -= RibbonFormWidthOffset;

            //using (GraphicsPath borderPath = DisplayHelp.GetRoundedRectanglePath(bounds, 0, 2, 2, 0))
            {
                //DisplayHelp.FillPath(g, borderPath, stateColors.Background);
                DisplayHelp.FillRectangle(g, bounds, stateColors.Background);

                if (stateColors.BottomBackgroundHighlight != null && !stateColors.BottomBackgroundHighlight.IsEmpty)
                {
                    Rectangle ellipse = new Rectangle(bounds.X - 12, bounds.Y + bounds.Height / 2 - 4, bounds.Width + 24, bounds.Height + 4);
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddEllipse(ellipse);
                        using (PathGradientBrush brush = new PathGradientBrush(path))
                        {
                            brush.CenterColor = stateColors.BottomBackgroundHighlight.Start;
                            brush.SurroundColors = new Color[] { stateColors.BottomBackgroundHighlight.End };
                            brush.CenterPoint = new PointF(ellipse.X + ellipse.Width / 2, bounds.Bottom - 1);

                            g.FillRectangle(brush, bounds);
                        }
                    }
                }

                if (stateColors.InnerBorder != null && !stateColors.InnerBorder.IsEmpty)
                {
                    Rectangle innerBorder = bounds;
                    innerBorder.Inflate(-1, -1);
                    using (GraphicsPath innerBorderPath = new GraphicsPath())
                    {
                        innerBorderPath.AddRectangle(innerBorder);
                        DisplayHelp.DrawGradientPath(g, innerBorderPath, innerBorder, stateColors.InnerBorder, 1);
                    }
                }

                if (stateColors.OuterBorder != null && !stateColors.OuterBorder.IsEmpty)
                    DisplayHelp.DrawRectangle(g, stateColors.OuterBorder.Start, bounds); //DisplayHelp.DrawGradientPathBorder(g, borderPath, stateColors.OuterBorder, 1);
            }
        }
    }
}
