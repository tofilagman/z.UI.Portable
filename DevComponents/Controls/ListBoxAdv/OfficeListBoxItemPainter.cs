using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using DevComponents.Editors.DateTimeAdv;

namespace DevComponents.DotNetBar.Rendering
{
    internal class OfficeListBoxItemPainter : ListBoxItemPainter
    {
        #region Implementation
        /// <summary>
        /// Paints StepItem.
        /// </summary>
        /// <param name="e">Provides arguments for the operation.</param>
        public override void Paint(ListBoxItemRendererEventArgs e)
        {
            Graphics g = e.ItemPaintArgs.Graphics;
            ListBoxItem item = e.Item;
            ListBoxAdv listBox = e.ItemPaintArgs.ContainerControl as ListBoxAdv;
            bool checkBoxes = false;
            bool useMnemonic = true;
            if (listBox != null)
            {
                checkBoxes = listBox.CheckBoxesVisible;
                useMnemonic = listBox.UseMnemonic;
            }
            OfficeListBoxItemColorTable table = ColorTable.ListBoxItem;
            OfficeListBoxItemStateColorTable ct = item.IsSelected ? table.Selected : table.Default;
            if (item.HotTracking && item.IsMouseOver)
                ct = table.MouseOver;

            Rectangle r = item.Bounds;

            if (checkBoxes)
            {
                Office2007CheckBoxStateColorTable cbt = GetCheckBoxStateColorTable(item.CheckBoxMouseState);
                Size checkBoxSize = Dpi.Size(item.CheckBoxSize);
                Rectangle cbr = new Rectangle(r.X + 2, r.Y + (r.Height - checkBoxSize.Height) / 2, checkBoxSize.Width, checkBoxSize.Height);
                _CheckBoxPainter.PaintCheckBox(g, cbr, cbt, item.CheckState);
                int checkBoxTextSpacing = Dpi.Width(ListBoxItem.CheckBoxTextSpacing);
                r.X += checkBoxSize.Width + checkBoxTextSpacing - 1;
                r.Width -= checkBoxSize.Width + checkBoxTextSpacing - 1;
                item.CheckBoxBounds = cbr;
            }

            if (ct.BackColors != null && ct.BackColors.Length > 0 || item.BackColors != null && item.BackColors.Length > 0)
            {
                using (Brush brush = DisplayHelp.CreateBrush(r, (item.BackColors != null && item.BackColors.Length > 0) ? item.BackColors : ct.BackColors, ct.BackColorsGradientAngle, ct.BackColorsPositions))
                {
                    g.FillRectangle(brush, r);
                }
            }

            Color textColor = ct.TextColor;

            if (!string.IsNullOrEmpty(item.SymbolRealized))
            {
                Color symbolColor = item.SymbolColor;
                if (symbolColor.IsEmpty) symbolColor = textColor;
                TextDrawing.DrawStringLegacy(g, item.SymbolRealized, Symbols.GetFont(item.SymbolSize, item.SymbolSet), 
                    symbolColor, new Rectangle(r.X, r.Y + r.Height / 2, 0, 0), eTextFormat.Default | eTextFormat.VerticalCenter);
                int imageSize = item.ActualSymbolSize.Width + Dpi.Width(ListBoxItem.ImageTextSpacing);
                r.Width -= imageSize;
                r.X += imageSize;
            }
            else if (item.Image != null)
            {
                Size imgSize = Dpi.ImageSize(item.Image.Size);
                g.DrawImage(item.Image, new Rectangle(r.X, r.Y + (r.Height - imgSize.Height) / 2, imgSize.Width, imgSize.Height));
                int imageSize = imgSize.Width + Dpi.Width(ListBoxItem.ImageTextSpacing);
                r.Width -= imageSize;
                r.X += imageSize;
            }
                        
            if (!string.IsNullOrEmpty(item.Text))
            {
                if (checkBoxes) { r.X += 1; r.Width -= 1; }
                if (!item.TextColor.IsEmpty) textColor = item.TextColor;
                Font font = e.ItemPaintArgs.Font;
                if (item.TextMarkupBody == null)
                {
                    eTextFormat textFormat = eTextFormat.Default | eTextFormat.VerticalCenter;
                    if (item.TextAlignment == eButtonTextAlignment.Center)
                    {
                        textFormat |= eTextFormat.HorizontalCenter;
                    }
                    else if (item.TextAlignment == eButtonTextAlignment.Right)
                    {
                        textFormat |= eTextFormat.Right;
                    }
                    if (!useMnemonic)
                        textFormat |= eTextFormat.NoPrefix;
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

        private Office2007CheckBoxStateColorTable GetCheckBoxStateColorTable(eMouseState state)
        {
            if (state == eMouseState.Down)
                return ColorTable.CheckBoxItem.Pressed;
            if (state == eMouseState.Hot)
                return ColorTable.CheckBoxItem.MouseOver;
            return ColorTable.CheckBoxItem.Default;
        }

        private Office2007CheckBoxItemPainter _CheckBoxPainter;
        public Office2007CheckBoxItemPainter CheckBoxPainter
        {
            get { return _CheckBoxPainter; }
            set { _CheckBoxPainter = value; }
        }
        #endregion
    }
}

