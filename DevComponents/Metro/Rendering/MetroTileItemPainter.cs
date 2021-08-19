using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using DevComponents.DotNetBar.Metro.ColorTables;

namespace DevComponents.DotNetBar.Metro.Rendering
{
    internal class MetroTileItemPainter : MetroRenderer
    {
        private static readonly int InflatePixels = 1;
        private static readonly int InflatePixelsMouseDown = 2;
        private static readonly int DragEffectInflatePixels = -4;
        private static readonly int DragInsertOffsetPixels = 10;

        //private static readonly Point ImageIndent = new Point(2, 2);
        public override void Render(MetroRendererInfo renderingInfo)
        {
            MetroTileItem item = (MetroTileItem)renderingInfo.Control;
            if (item.Frames.Count == 0)
            {
                using (HatchBrush brush = new HatchBrush(HatchStyle.ForwardDiagonal, Color.Red))
                    renderingInfo.PaintEventArgs.Graphics.FillRectangle(brush, item.Bounds);
                return;
            }
            
            Matrix currentTransform = null;
            float zoom = 0.95f;
            System.Drawing.Drawing2D.Matrix mx = null;

            if (renderingInfo.ItemPaintArgs.DragInProgress)
            {
                currentTransform = renderingInfo.PaintEventArgs.Graphics.Transform;
                
                mx = new System.Drawing.Drawing2D.Matrix(zoom, 0, 0, zoom, 0, 0);
                float offsetX = (item.WidthInternal * (1.0f / zoom) - item.WidthInternal) / 2;
                float offsetY = (item.HeightInternal * (1.0f / zoom) - item.HeightInternal) / 2;
                mx.Translate(offsetX, offsetY);
                renderingInfo.PaintEventArgs.Graphics.Transform = mx;
            }

            if (item.CurrentFrameOffset != 0 && item.LastFrame != item.CurrentFrame)
            {
                Graphics g = renderingInfo.PaintEventArgs.Graphics;
                if (currentTransform == null)
                    currentTransform = g.Transform;
                g.TranslateTransform(0, -(item.HeightInternal - item.CurrentFrameOffset - InflatePixels * 2), MatrixOrder.Append);
                // Draw last frame first then offset the current frame
                RenderFrame(renderingInfo, item.LastFrame);
                if (mx != null)
                    g.Transform = mx;
                else
                    g.Transform = currentTransform;
                g.TranslateTransform(0, item.CurrentFrameOffset, MatrixOrder.Append);

                RenderFrame(renderingInfo, item.CurrentFrame);
            }
            else
                RenderFrame(renderingInfo, item.CurrentFrame);

            if (currentTransform != null)
            {
                renderingInfo.PaintEventArgs.Graphics.Transform = currentTransform;
                currentTransform.Dispose();
            }
        }
        private void RenderFrame(MetroRendererInfo renderingInfo, int frameIndex)
        {
            MetroTileItem item = (MetroTileItem)renderingInfo.Control;
            MetroTileFrame frame = item.Frames[frameIndex];
            MetroTileColorTable colorTable = renderingInfo.ColorTable.MetroTile;
            Graphics g = renderingInfo.PaintEventArgs.Graphics;
            Rectangle bounds = item.Bounds;
            Region clip = null;
            if (!item.DragStartPoint.IsEmpty) // When dragging tile moves with the mouse
            {
                bounds.Location = renderingInfo.ItemPaintArgs.ContainerControl.PointToClient(Control.MousePosition);
                bounds.Location.Offset(-item.DragStartPoint.X, -item.DragStartPoint.Y);
                clip = g.Clip;
                g.SetClip(bounds, CombineMode.Replace);
            }
            Control control = item.ContainerControl as Control;
            bounds.Inflate(-InflatePixels, -InflatePixels);
            if (item.IsLeftMouseButtonDown)
                bounds.Inflate(-InflatePixelsMouseDown, -InflatePixelsMouseDown);
            else if (item.IsMouseOver)
                bounds.Inflate(InflatePixels, InflatePixels);

            //if (renderingInfo.ItemPaintArgs.DragInProgress)
            //    bounds.Inflate(DragEffectInflatePixels, DragEffectInflatePixels);

            eDesignInsertPosition insertMarker = item.DesignInsertMarker;
            if (insertMarker == eDesignInsertPosition.After)
            {
                if (item.IsDesignMarkHorizontal)
                    bounds.Offset(-DragInsertOffsetPixels, 0);
                else
                    bounds.Offset(0, -DragInsertOffsetPixels);
                g.ResetClip();
            }
            else if (insertMarker == eDesignInsertPosition.Before)
            {
                if (item.IsDesignMarkHorizontal)
                    bounds.Offset(DragInsertOffsetPixels, 0);
                else
                    bounds.Offset(0, DragInsertOffsetPixels);
                g.ResetClip();
            }

            bool dispose = false;
            bool enabled = item.GetEnabled(renderingInfo.ItemPaintArgs.ContainerControl);
            ElementStyle style = ElementStyleDisplay.GetElementStyle(frame.EffectiveStyle, out dispose);

            if (bounds.Width > 2048) bounds.Width = 2048;
            if (bounds.Height > 1600) bounds.Height = 1600;

            if (enabled)
            {
                ElementStyleDisplayInfo di = new ElementStyleDisplayInfo(style, g,  bounds);
                ElementStyleDisplay.Paint(di);
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(item.DisabledBackColor.IsEmpty ? renderingInfo.ColorTable.MetroPartColors.CanvasColorLighterShade : item.DisabledBackColor))
                    g.FillRectangle(brush, bounds);
            }

            Rectangle textRect = bounds;
            textRect.X += style.PaddingLeft;
            textRect.Y += style.PaddingTop;
            textRect.Width -= style.PaddingHorizontal;
            textRect.Height -= style.PaddingVertical;
            if (item.IsLeftMouseButtonDown)
            {
                textRect.Width += InflatePixelsMouseDown*2;
                textRect.Height += InflatePixelsMouseDown*2;
            }
            else if(item.IsMouseOver)
                textRect.Inflate(-InflatePixels, -InflatePixels);
            Size tileSize = Dpi.Size(item.TileSize);
            if (item.SubItems.Count > 0 && frameIndex < item.SubItems.Count)
            {
                BaseItem child = item.SubItems[frameIndex];
                if (child.Displayed)
                {
                    child.TopInternal = bounds.Y + style.PaddingTop + ((bounds.Height - style.PaddingTop - frame.TitleTextBounds.Height) - child.HeightInternal) / 2;
                    child.LeftInternal = bounds.X + style.PaddingLeft;
                    child.WidthInternal = tileSize.Width - style.PaddingHorizontal;
                    child.Paint(renderingInfo.ItemPaintArgs);
                }
            }

            Image image = frame.Image;
            ContentAlignment imageTextAlign = frame.ImageTextAlignment;
            Color textColor = enabled ? style.TextColor : renderingInfo.ColorTable.MetroPartColors.CanvasColorLightShade;
            Color symbolColor = textColor;
            if (!frame.SymbolColor.IsEmpty) symbolColor = frame.SymbolColor;
            if (image != null || !string.IsNullOrEmpty(frame.SymbolRealized))
            {
                Font symFont = null;
                Rectangle imageRect = Rectangle.Empty;
                if (string.IsNullOrEmpty(frame.SymbolRealized))
                    imageRect = new Rectangle(0, 0, Dpi.ImageWidth(image.Width), Dpi.ImageHeight(image.Height));
                else
                {
                    symFont = Symbols.GetFont(frame.SymbolSize, frame.SymbolSet);
                    Size imageSize = TextDrawing.MeasureString(g, frame.SymbolRealized, symFont);
                    int descent = (int)Math.Ceiling((symFont.FontFamily.GetCellDescent(symFont.Style) *
                        symFont.Size / symFont.FontFamily.GetEmHeight(symFont.Style)));
                    imageSize.Height -= descent;
                    imageRect = new Rectangle(0, 0, imageSize.Width, imageSize.Height);
                }

                imageRect.Offset(bounds.Location);

                if (imageTextAlign == ContentAlignment.TopLeft)
                {
                    textRect.X += (imageRect.Width + frame.ImageIndent.X);
                    textRect.Width -= (imageRect.Width + frame.ImageIndent.X);
                    imageRect.Offset(frame.ImageIndent.X, frame.ImageIndent.Y);
                }
                else if (imageTextAlign == ContentAlignment.TopCenter)
                {
                    imageRect.X += (tileSize.Width - imageRect.Width) / 2;
                    imageRect.Offset(frame.ImageIndent.X, frame.ImageIndent.Y);
                    textRect.Y += (imageRect.Height + frame.ImageIndent.Y);
                    textRect.Height -= (imageRect.Height);
                }
                else if (imageTextAlign == ContentAlignment.TopRight)
                {
                    imageRect.X += (tileSize.Width - imageRect.Width - frame.ImageIndent.X);
                    imageRect.Offset(0, frame.ImageIndent.Y);
                    textRect.Width -= (imageRect.Width + frame.ImageIndent.X);
                }
                else if (imageTextAlign == ContentAlignment.BottomCenter)
                {
                    imageRect.Offset((tileSize.Width - imageRect.Width) / 2, tileSize.Height - imageRect.Height);
                    imageRect.Offset(frame.ImageIndent.X, frame.ImageIndent.Y);
                    textRect.Height -= (imageRect.Height);
                }
                else if (imageTextAlign == ContentAlignment.BottomLeft)
                {
                    imageRect.Offset(0, tileSize.Height - imageRect.Height);
                    imageRect.Offset(frame.ImageIndent.X, frame.ImageIndent.Y);
                    textRect.X += (imageRect.Width + frame.ImageIndent.X);
                    textRect.Width -= (imageRect.Width + frame.ImageIndent.X);
                }
                else if (imageTextAlign == ContentAlignment.BottomRight)
                {
                    imageRect.Offset((tileSize.Width - imageRect.Width - frame.ImageIndent.X), tileSize.Height - imageRect.Height);
                    imageRect.Offset(0, frame.ImageIndent.Y);
                    textRect.Width -= (imageRect.Width + frame.ImageIndent.X);
                }
                else if (imageTextAlign == ContentAlignment.MiddleCenter)
                {
                    imageRect.Offset((tileSize.Width - imageRect.Width) / 2, (tileSize.Height - imageRect.Height) / 2);
                    imageRect.Offset(frame.ImageIndent.X, frame.ImageIndent.Y);
                    textRect.Height = Math.Max(0, textRect.Bottom - imageRect.Bottom);
                    textRect.Y = imageRect.Bottom + 1;
                }
                else if (imageTextAlign == ContentAlignment.MiddleLeft)
                {
                    imageRect.Offset(0, (tileSize.Height - imageRect.Height) / 2);
                    imageRect.Offset(frame.ImageIndent.X, frame.ImageIndent.Y);
                    textRect.X += (imageRect.Width + frame.ImageIndent.X);
                    textRect.Width -= (imageRect.Width + frame.ImageIndent.X);
                }
                else if (imageTextAlign == ContentAlignment.MiddleRight)
                {
                    imageRect.Offset((tileSize.Width - imageRect.Width - frame.ImageIndent.X), (tileSize.Height - imageRect.Height) / 2);
                    imageRect.Offset(0, frame.ImageIndent.Y);
                    textRect.Width -= (imageRect.Width + frame.ImageIndent.X);
                }
                else
                    imageRect.Offset(frame.ImageIndent.X, frame.ImageIndent.Y);

                if (string.IsNullOrEmpty(frame.SymbolRealized))
                    g.DrawImage(image, imageRect);
                else
                    TextDrawing.DrawStringLegacy(g, frame.SymbolRealized, symFont, symbolColor, new Rectangle(imageRect.X, imageRect.Y, 0, 0), eTextFormat.Default);
            }

            if (textRect.Width > 0 && textRect.Height > 0 && frame.Text != null)
            {
                Font font = renderingInfo.DefaultFont;
                if (style.Font != null)
                    font = style.Font;
                bool rightToLeft = renderingInfo.RightToLeft;
                if (frame.TextMarkupBody == null)
                {
                    eTextFormat textFormat = eTextFormat.Default | eTextFormat.WordBreak | eTextFormat.NoClipping;
                    if (style.TextLineAlignment == eStyleTextAlignment.Center)
                        textFormat |= eTextFormat.VerticalCenter;
                    else if (style.TextLineAlignment == eStyleTextAlignment.Far)
                        textFormat |= eTextFormat.Bottom;
                    if (style.TextAlignment == eStyleTextAlignment.Center)
                        textFormat |= eTextFormat.HorizontalCenter;
                    else if (style.TextAlignment == eStyleTextAlignment.Far)
                        textFormat |= eTextFormat.Right;
                    //if (frame.Text.Contains("Explorer")) Console.WriteLine("{0}    {1}", textRect, DateTime.Now);
                    TextDrawing.DrawString(g, frame.Text, font, textColor, textRect, textFormat);
                }
                else
                {
                    TextMarkup.MarkupDrawContext d = new TextMarkup.MarkupDrawContext(g, font, textColor, rightToLeft);
                    d.HotKeyPrefixVisible = false;
                    d.ContextObject = item;
                    Rectangle markupBounds = textRect;
                    // Can't do this because it will break all apps default for TextLineAlignment is eStyleTextAlignment.Center
                    //if (style.TextLineAlignment == eStyleTextAlignment.Center)
                    //    markupBounds = new Rectangle(new Point(textRect.X, textRect.Y + (textRect.Height - frame.TextMarkupBody.Bounds.Height) / 2), frame.TextMarkupBody.Bounds.Size);
                    //else if (style.TextLineAlignment == eStyleTextAlignment.Far)
                    //    markupBounds = new Rectangle(new Point(textRect.X, textRect.Bottom - frame.TextMarkupBody.Bounds.Height), frame.TextMarkupBody.Bounds.Size);

                    frame.TextMarkupBody.Bounds = markupBounds;
                    frame.TextMarkupBody.Render(d);
                }
            }

            if (frame.TitleText != null)
            {
                Color titleTextColor = enabled ? frame.TitleTextColor : renderingInfo.ColorTable.MetroPartColors.CanvasColorLightShade;
                if (titleTextColor.IsEmpty)
                    titleTextColor = style.TextColor;
                Font font = item.GetTitleTextFont(frame, style, control);
                Rectangle titleTextRect = frame.TitleTextBounds;
                titleTextRect.Offset(bounds.Location);
                if (item.IsLeftMouseButtonDown)
                    titleTextRect.Offset(1, 1);
                else if (item.IsMouseOver)
                    titleTextRect.Offset(InflatePixels, InflatePixels);
                TextDrawing.DrawString(g, frame.TitleText, font, titleTextColor, titleTextRect, eTextFormat.Default | eTextFormat.SingleLine);
            }

            if (item.Checked)
            {
                Size checkMarkSize = Dpi.Size(CheckMarkSize);
                Rectangle markBounds = new Rectangle(bounds.Right - checkMarkSize.Width, bounds.Y, checkMarkSize.Width, checkMarkSize.Height);
                using (GraphicsPath markPath = new GraphicsPath())
                {
                    markPath.AddLine(markBounds.X, markBounds.Y, markBounds.Right - 1, markBounds.Y);
                    markPath.AddLine(markBounds.Right - 1, markBounds.Y, markBounds.Right - 1, markBounds.Bottom - 1);
                    markPath.CloseFigure();
                    using (SolidBrush brush = new SolidBrush(colorTable.CheckBackground))
                        g.FillPath(brush, markPath);
                }
                using (SolidBrush brush = new SolidBrush(colorTable.CheckForeground))
                {
                    Rectangle checkCircleBounds = new Rectangle();
                    checkCircleBounds.Size = new Size(Dpi.Width11, Dpi.Height11);
                    checkCircleBounds.X = markBounds.Right - checkCircleBounds.Size.Width - Dpi.Width3;
                    checkCircleBounds.Y = markBounds.Y + Dpi.Height2;
                    using (Pen pen = new Pen(colorTable.CheckForeground, Dpi.Width2))
                    {
                        g.DrawEllipse(pen, checkCircleBounds);
                        g.DrawLine(pen, checkCircleBounds.X + Dpi.Width3, checkCircleBounds.Y + Dpi.Height5, checkCircleBounds.X + Dpi.Width6, checkCircleBounds.Y + Dpi.Height8);
                        g.DrawLine(pen, checkCircleBounds.X + Dpi.Width6, checkCircleBounds.Y + Dpi.Height8, checkCircleBounds.X + Dpi.Width9, checkCircleBounds.Y + Dpi.Height3);
                    }
                }
            }

            if (dispose) style.Dispose();

            if (clip != null) 
            {
                g.Clip = clip;
                clip.Dispose();
            }
        }

        private static readonly Size CheckMarkSize = new Size(27, 27);
    }
}
