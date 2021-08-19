using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using DevComponents.DotNetBar.Controls;
using System.Drawing.Imaging;

namespace DevComponents.DotNetBar.Rendering
{
    internal class OfficeSideNavItemPainter : SideNavItemPainter
    {
        /// <summary>
        /// Paints ListBoxItem.
        /// </summary>
        /// <param name="e">Provides arguments for the operation.</param>
        public override void Paint(SideNavItemRendererEventArgs e)
        {
            Graphics g = e.ItemPaintArgs.Graphics;
            SideNavItem item = e.Item;
            SideNavStrip navStrip = e.ItemPaintArgs.ContainerControl as SideNavStrip;

            SideNavItemColorTable table = ColorTable.SideNav.SideNavItem;
            SideNavItemStateColorTable ct = item.IsMouseOver ? table.MouseOver : table.Default;
            if (item.Checked)
                ct = table.Selected;
            else if (item.IsMouseDown)
                ct = table.Pressed;

            Rectangle r = item.Bounds;
            Rectangle textBounds = item.TextRenderBounds;
            Rectangle imageBounds = item.ImageRenderBounds;
            CompositeImage image = item.GetImage();

            if (ct.BackColors != null && ct.BackColors.Length > 0 || item.BackColors != null && item.BackColors.Length > 0)
            {
                using (Brush brush = DisplayHelp.CreateBrush(r, (item.BackColors != null && item.BackColors.Length > 0) ? item.BackColors : ct.BackColors, ct.BackColorsGradientAngle, ct.BackColorsPositions))
                {
                    DisplayHelp.FillRoundedRectangle(g, brush, r, ct.CornerRadius);
                    //g.FillRectangle(brush, r);
                }
            }

            Region oldClip = null;
            // For top item do not draw the top border
            if (r.Y == 0 && item.Checked)
            {
                oldClip = g.Clip;
                g.SetClip(new Rectangle(r.X, r.Y, r.Width, 1), CombineMode.Exclude);
            }
            if (item.BorderColors != null && item.BorderColors.Length > 0)
                DisplayHelp.DrawRoundedRectangle(g, r, item.BorderColors, ct.CornerRadius);
            else if (ct.BorderColors != null && ct.BorderColors.Length > 0)
                DisplayHelp.DrawRoundedRectangle(g, r, ct.BorderColors, ct.CornerRadius);
            if (r.Y == 0 && item.Checked)
            {
                g.Clip = oldClip;
                oldClip.Dispose();
            }

            Color textColor = ct.TextColor;
            bool hasImage = false;
            if ((image != null || !string.IsNullOrEmpty(item.SymbolRealized)) && item.ButtonStyle != eButtonStyle.TextOnlyAlways)
            {
                if (imageBounds.IsEmpty)
                    imageBounds = GetImageRectangle(item, image);
                if (textBounds.IsEmpty)
                    textBounds = GetTextRectangle(item, image, imageBounds);
                hasImage = true;

            }
            else if (textBounds.IsEmpty)
            {
                textBounds = r;
                r.X += 2;
                r.Width -= 2;
            }

            if (!string.IsNullOrEmpty(item.SymbolRealized))
            {
                Color symbolColor = item.SymbolColor;
                if (symbolColor.IsEmpty) symbolColor = textColor;
                TextDrawing.DrawStringLegacy(g, item.SymbolRealized, Symbols.GetFont(item.SymbolSize, item.SymbolSet),
                    symbolColor, new Rectangle(imageBounds.X, imageBounds.Y + imageBounds.Height / 2, 0, 0), eTextFormat.Default | eTextFormat.VerticalCenter);
            }
            else if (image != null)
            {
                if (!item.IsMouseOver && item.HotTrackingStyle == eHotTrackingStyle.Color)
                {
                    // Draw gray-scale image for this hover style...
                    float[][] array = new float[5][];
                    array[0] = new float[5] { 0.2125f, 0.2125f, 0.2125f, 0, 0 };
                    array[1] = new float[5] { 0.5f, 0.5f, 0.5f, 0, 0 };
                    array[2] = new float[5] { 0.0361f, 0.0361f, 0.0361f, 0, 0 };
                    array[3] = new float[5] { 0, 0, 0, 1, 0 };
                    array[4] = new float[5] { 0.2f, 0.2f, 0.2f, 0, 1 };
                    ColorMatrix grayMatrix = new ColorMatrix(array);
                    ImageAttributes att = new ImageAttributes();
                    att.SetColorMatrix(grayMatrix);
                    image.DrawImage(g, imageBounds, 0, 0, image.ActualWidth, image.ActualHeight, GraphicsUnit.Pixel, att);
                }
                else
                {
                    image.DrawImage(g, imageBounds);
                }
            }
            item.ImageRenderBounds = imageBounds;

            if (!string.IsNullOrEmpty(item.Text) && (item.ButtonStyle!= eButtonStyle.Default || !hasImage))
            {
                if (!item.ForeColor.IsEmpty) textColor = item.ForeColor;
                Font font = e.ItemPaintArgs.Font;
                if (item.TextMarkupBody == null)
                {
                    eTextFormat textFormat = eTextFormat.Default | eTextFormat.VerticalCenter;
                    //if (item.TextAlignment == eButtonTextAlignment.Center)
                    //{
                    //    textFormat |= eTextFormat.HorizontalCenter;
                    //}
                    //else if (item.TextAlignment == eButtonTextAlignment.Right)
                    //{
                    //    textFormat |= eTextFormat.Right;
                    //}
                    TextDrawing.DrawString(g, item.Text, font, textColor, textBounds, textFormat);
                }
                else
                {
                    TextMarkup.MarkupDrawContext d = new TextMarkup.MarkupDrawContext(g, font, textColor, e.ItemPaintArgs.RightToLeft);
                    d.HotKeyPrefixVisible = false;
                    d.ContextObject = item;
                    Rectangle mr = new Rectangle(textBounds.X, textBounds.Y + (textBounds.Height - item.TextMarkupBody.Bounds.Height) / 2, item.TextMarkupBody.Bounds.Width, item.TextMarkupBody.Bounds.Height);
                    item.TextMarkupBody.Bounds = mr;
                    item.TextMarkupBody.Render(d);
                }

            }
            item.TextRenderBounds = textBounds;
        }

        private Rectangle GetImageRectangle(SideNavItem item, CompositeImage image)
        {
            Rectangle imageRect = Rectangle.Empty;
            // Calculate image position
            if (image != null || !string.IsNullOrEmpty(item.SymbolRealized))
            {
                Size imageSize = item.ImageSize;

                if (item.ImagePosition == eImagePosition.Top || item.ImagePosition == eImagePosition.Bottom)
                    imageRect = new Rectangle(item.ImageDrawRect.X, item.ImageDrawRect.Y, item.DisplayRectangle.Width, item.ImageDrawRect.Height);
                else if (item.ImagePosition == eImagePosition.Left)
                {
                    if (item.ButtonStyle == eButtonStyle.Default)
                        return new Rectangle(item.Bounds.X + (item.Bounds.Width - imageSize.Width)/2,
                            item.Bounds.Y + (item.Bounds.Height - imageSize.Height)/2, imageSize.Width, imageSize.Height);
                    else
                        imageRect = new Rectangle(item.ImageDrawRect.X + 4, item.ImageDrawRect.Y,
                            item.ImageDrawRect.Width,
                            item.ImageDrawRect.Height);
                }
                else if (item.ImagePosition == eImagePosition.Right)
                    imageRect = new Rectangle(item.ImageDrawRect.X + item.ImagePaddingHorizontal + 4,
                        item.ImageDrawRect.Y, item.ImageDrawRect.Width, item.ImageDrawRect.Height);
                imageRect.Offset(item.DisplayRectangle.Left, item.DisplayRectangle.Top);
                imageRect.Offset((imageRect.Width - imageSize.Width) / 2, (imageRect.Height - imageSize.Height) / 2);

                imageRect.Width = imageSize.Width;
                imageRect.Height = imageSize.Height;
            }

            return imageRect;
        }

        private Rectangle GetTextRectangle(SideNavItem item, CompositeImage image, Rectangle imageBounds)
        {
            Rectangle itemRect = item.DisplayRectangle;
            Rectangle textRect = item.TextDrawRect;

            if (item.ImagePosition == eImagePosition.Top || item.ImagePosition == eImagePosition.Bottom)
            {
                textRect = new Rectangle(1, textRect.Y, itemRect.Width - 2, textRect.Height);
            }
            textRect.Offset(itemRect.Left, itemRect.Top);

            if (item.ImagePosition == eImagePosition.Left)
                textRect.X = imageBounds.Right + item.ImagePaddingHorizontal;

            if (textRect.Right > itemRect.Right)
                textRect.Width = itemRect.Right - textRect.Left;

            return textRect;
        }
    }
}
