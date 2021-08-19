using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace DevComponents.DotNetBar.Rendering
{
    internal class OfficeStepItemPainter : StepItemPainter
    {
        #region Implementation
        /// <summary>
        /// Paints StepItem.
        /// </summary>
        /// <param name="e">Provides arguments for the operation.</param>
        public override void Paint(StepItemRendererEventArgs e)
        {
            StepItem item = e.Item;
            OfficeStepItemColorTable table = ColorTable.StepItem;
            OfficeStepItemStateColorTable ct = table.Default;
            if (item.HotTracking && item.IsMouseDown)
                ct = table.Pressed;
            else if (item.HotTracking && item.IsMouseOver)
                ct = table.MouseOver;

            OfficeStepItemStateColorTable ctProgress = table.Progress;
            Rectangle clip = Rectangle.Empty;

            Rectangle r = item.Bounds;
            int pointerSize = item.GetPointerSize();
            Graphics g = e.ItemPaintArgs.Graphics;
            GraphicsPath path = GetPath(item, pointerSize);
            item.ItemPath = path;
            using (Brush brush = DisplayHelp.CreateBrush(path.GetBounds(), (item.BackColors != null && item.BackColors.Length > 0) ? item.BackColors : ct.BackColors, ct.BackColorsGradientAngle, ct.BackColorsPositions))
            {
                g.FillPath(brush, path);
            }
            if (item.Value > item.Minimum) // Render progress marker
            {
                float percent = Math.Min(1, (item.Value / (float)(item.Maximum - item.Minimum)));
                if (percent > 0)
                {
                    clip = item.Bounds;
                    clip.Width = (int)(clip.Width * percent);
                }
                if (!clip.IsEmpty)
                {
                    Region oldClip = g.Clip;
                    g.SetClip(clip, CombineMode.Intersect);
                    using (Brush brush = DisplayHelp.CreateBrush(path.GetBounds(),
                        (item.ProgressColors != null && item.ProgressColors.Length > 0) ? item.ProgressColors : ctProgress.BackColors, ctProgress.BackColorsGradientAngle, ctProgress.BackColorsPositions))
                    {
                        g.FillPath(brush, path);
                    }
                    g.Clip = oldClip;
                    oldClip.Dispose();
                }
            }

            if (ct.BorderColors.Length > 0)
            {
                for (int i = ct.BorderColors.Length - 1; i > 0; i--)
                {
                    Rectangle rb = item.Bounds;
                    rb.Inflate(-i, -i);
                    using (GraphicsPath borderPath = GetPath(item, pointerSize, rb))
                    {
                        using (Pen pen = new Pen(ct.BorderColors[i]))
                            g.DrawPath(pen, borderPath);
                    }
                }
                using (Pen pen = new Pen(ct.BorderColors[0]))
                    g.DrawPath(pen, path);
            }

            // Render content
            r.X += item.Padding.Left;
            r.Y += item.Padding.Top;
            r.Width -= item.Padding.Horizontal;
            r.Height -= item.Padding.Vertical;

            if (!item.IsFirst)
            {
                r.X += pointerSize;
                r.Width -= pointerSize;
            }

            Color textColor = ct.TextColor;
            if (!string.IsNullOrEmpty(item.SymbolRealized))
            {
                Color symbolColor = item.SymbolColor;
                if (symbolColor.IsEmpty) symbolColor = textColor;
                TextDrawing.DrawStringLegacy(g, item.SymbolRealized, Symbols.GetFont(item.SymbolSize, item.SymbolSet), symbolColor, new Rectangle(r.X, r.Y + r.Height / 2, 0, 0), eTextFormat.Default | eTextFormat.VerticalCenter);
                int imageSize = item.ActualSymbolSize.Width + item.ImageTextSpacing;
                r.Width -= imageSize;
                r.X += imageSize;
            }
            else if (item.Image != null)
            {
                g.DrawImage(item.Image, new Rectangle(r.X, r.Y + (r.Height - item.Image.Height) / 2, item.Image.Width, item.Image.Height));
                int imageSize = item.Image.Width + item.ImageTextSpacing;
                r.Width -= imageSize;
                r.X += imageSize;
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                if (!item.TextColor.IsEmpty) textColor = item.TextColor;
                Font font = e.ItemPaintArgs.Font;
                if (item.TextMarkupBody == null)
                {
                    eTextFormat textFormat = eTextFormat.Default | eTextFormat.VerticalCenter;
                    if (item.TextAlignment == eButtonTextAlignment.Center)
                    {
                        textFormat |= eTextFormat.HorizontalCenter;
                        if (!item.IsLast)
                            r.Width -= pointerSize;
                    }
                    else if (item.TextAlignment == eButtonTextAlignment.Right)
                    {
                        textFormat |= eTextFormat.Right;
                        if (!item.IsLast)
                            r.Width -= pointerSize;
                    }
                    TextDrawing.DrawString(g, item.Text, font, textColor, r, textFormat);
                }
                else
                {
                    TextMarkup.MarkupDrawContext d = new TextMarkup.MarkupDrawContext(g, font, textColor, e.ItemPaintArgs.RightToLeft);
                    d.HotKeyPrefixVisible = false;
                    d.ContextObject = item;
                    Rectangle mr = new Rectangle(r.X, r.Y + (r.Height - item.TextMarkupBody.Bounds.Height) / 2, item.TextMarkupBody.Bounds.Width, item.TextMarkupBody.Bounds.Height);
                    item.TextMarkupBody.Bounds = mr;
                    item.TextMarkupBody.Render(d);
                }

            }
        }

        private GraphicsPath GetPath(StepItem item, int arrowSize)
        {
            Rectangle r = item.Bounds;
            return GetPath(item, arrowSize, r);
        }

        private GraphicsPath GetPath(StepItem item, int arrowSize, Rectangle r)
        {
            r.Width--;
            r.Height--;
            if (item.IsFirst)
                return GetFirstItemPath(r, arrowSize);
            else if (item.IsLast)
                return GetLastItemPath(r, arrowSize);
            else
                return GetItemPath(r, arrowSize);
        }
        private GraphicsPath GetItemPath(Rectangle r, int arrowSize)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(r.X, r.Y, r.X + arrowSize, r.Y + r.Height / 2);
            path.AddLine(r.X + arrowSize, r.Y + r.Height / 2, r.X, r.Bottom);
            path.AddLine(r.X, r.Bottom, r.Right - arrowSize, r.Bottom);
            path.AddLine(r.Right - arrowSize, r.Bottom, r.Right, r.Y + r.Height / 2);
            path.AddLine(r.Right, r.Y + r.Height / 2, r.Right - arrowSize, r.Y);
            path.CloseAllFigures();

            return path;
        }
        private GraphicsPath GetLastItemPath(Rectangle r, int arrowSize)
        {
            GraphicsPath path = new GraphicsPath();

            ArcData ad = ElementStyleDisplay.GetCornerArc(r, 2, eCornerArc.TopRight);
            path.AddArc(ad.X, ad.Y, ad.Width, ad.Height, ad.StartAngle, ad.SweepAngle);
            ad = ElementStyleDisplay.GetCornerArc(r, 2, eCornerArc.BottomRight);
            path.AddArc(ad.X, ad.Y, ad.Width, ad.Height, ad.StartAngle, ad.SweepAngle);

            path.AddLine(r.X, r.Bottom, r.X + arrowSize, r.Y + r.Height / 2);
            path.AddLine(r.X + arrowSize, r.Y + r.Height / 2, r.X, r.Y);

            path.CloseAllFigures();

            return path;
        }
        private GraphicsPath GetFirstItemPath(Rectangle r, int arrowSize)
        {
            GraphicsPath path = new GraphicsPath();
            ArcData ad = ElementStyleDisplay.GetCornerArc(r, 2, eCornerArc.BottomLeft);
            path.AddArc(ad.X, ad.Y, ad.Width, ad.Height, ad.StartAngle, ad.SweepAngle);
            ad = ElementStyleDisplay.GetCornerArc(r, 2, eCornerArc.TopLeft);
            path.AddArc(ad.X, ad.Y, ad.Width, ad.Height, ad.StartAngle, ad.SweepAngle);
            path.AddLine(r.Right - arrowSize, r.Y, r.Right, r.Y + r.Height / 2);
            path.AddLine(r.Right, r.Y + r.Height / 2, r.Right - arrowSize, r.Bottom);
            path.CloseAllFigures();
            return path;

        }
        #endregion
    }
}
