using System;
using System.Collections.Generic;
using System.Text;
using DevComponents.DotNetBar.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DevComponents.DotNetBar.Rendering
{
    internal class OfficeTabFormStripPainter : TabFormStripPainter, IOffice2007Painter
    {
        #region IOffice2007Painter
        private Office2007ColorTable _ColorTable = null;

        /// <summary>
        /// Gets or sets color table used by renderer.
        /// </summary>
        public Office2007ColorTable ColorTable
        {
            get { return _ColorTable; }
            set { _ColorTable = value; }
        }
        #endregion

        public override void Paint(TabFormStripPainterArgs renderingInfo)
        {
            TabFormStripControl strip = renderingInfo.TabFormStrip;
            Graphics g = renderingInfo.Graphics;
            Rectangle bounds = strip.ClientRectangle;
            bool isFormActive = true;
            Form form = strip.FindForm();
            if (form != null && (form != System.Windows.Forms.Form.ActiveForm && form.MdiParent == null ||
                    form.MdiParent != null && form.MdiParent.ActiveMdiChild != form))
                isFormActive = false;
            
            //if (ct.BackgroundStyle != null)
            //{
            //    ElementStyleDisplayInfo di = new ElementStyleDisplayInfo(ct.BackgroundStyle, g, bounds);
            //    ElementStyleDisplay.PaintBackground(di);
            //}
            TabFormControl tabControl = strip.Parent as TabFormControl;
            TabFormColorTable formColorTable = ColorTable.TabForm;
            if (tabControl != null && tabControl.ColorTable != null)
                formColorTable = tabControl.ColorTable;

            TabFormStateColorTable stateColorTable = isFormActive ? formColorTable.Active : formColorTable.Inactive;
            if (stateColorTable.CaptionBackColors.Length > 0)
            {
                Rectangle cb = strip.ClientRectangle;
                cb.Inflate(1, 1);
                using (
                    Brush brush = DisplayHelp.CreateBrush(cb, stateColorTable.CaptionBackColors,
                        stateColorTable.CaptionBackColorsGradientAngle, stateColorTable.CaptionBackColorsPositions))
                    g.FillRectangle(brush, cb);
            }

            if (strip.CaptionVisible)
            {
                if (strip.CaptionBounds.IsEmpty || strip.SystemCaptionItemBounds.IsEmpty)
                    SetCaptionItemBounds(strip, renderingInfo);
                Color captionTextColor = stateColorTable.CaptionText;
                eTextFormat textFormat = formColorTable.CaptionTextFormat;

                Font font = SystemFonts.DefaultFont;
                bool disposeFont = true;
                if (strip.CaptionFont != null)
                {
                    font.Dispose();
                    font = strip.CaptionFont;
                    disposeFont = false;
                }
                string text = strip.TitleText;
                if (string.IsNullOrEmpty(text) && form != null) text = form.Text;
                bool isTitleTextMarkup = strip.TitleTextMarkupBody != null;
                Rectangle captionRect = strip.CaptionBounds;
                const int CAPTION_TEXT_PADDING = 12;
                captionRect.X += CAPTION_TEXT_PADDING;
                captionRect.Width -= CAPTION_TEXT_PADDING;

                if (StyleManager.Style == eStyle.OfficeMobile2014)
                {
                    captionRect.Y -= 2;
                    captionRect.Height -= 2;
                    // Center text in center of window instead of center of available space
                    if (!strip.SystemCaptionItemBounds.IsEmpty && captionRect.Width > strip.SystemCaptionItemBounds.Width)
                    {
                        captionRect.X += strip.SystemCaptionItemBounds.Width / 2 + 18;
                        captionRect.Width -= strip.SystemCaptionItemBounds.Width / 2 + 18;
                    }
                }

                if (!isTitleTextMarkup)
                    TextDrawing.DrawString(g, text, font, captionTextColor, captionRect, textFormat);
                else
                {
                    TextMarkup.MarkupDrawContext d = new TextMarkup.MarkupDrawContext(g, font, captionTextColor, strip.RightToLeft == System.Windows.Forms.RightToLeft.Yes, captionRect, false);
                    d.AllowMultiLine = false;
                    d.IgnoreFormattingColors = !isFormActive;
                    TextMarkup.BodyElement body = strip.TitleTextMarkupBody;
                    if (strip.TitleTextMarkupLastArrangeBounds != captionRect)
                    {
                        body.Measure(captionRect.Size, d);
                        body.Arrange(captionRect, d);
                        strip.TitleTextMarkupLastArrangeBounds = captionRect;
                        Rectangle mr = body.Bounds;
                        if (mr.Width < captionRect.Width)
                            mr.Offset((captionRect.Width - mr.Width) / 2, 0);
                        if (mr.Height < captionRect.Height)
                            mr.Offset(0, (captionRect.Height - mr.Height) / 2);
                        body.Bounds = mr;
                    }
                    Region oldClip = g.Clip;
                    g.SetClip(captionRect, CombineMode.Intersect);
                    body.Render(d);
                    g.Clip = oldClip;
                    if (oldClip != null) oldClip.Dispose();
                }

                if (disposeFont) font.Dispose();
            }

        }

        private void SetCaptionItemBounds(TabFormStripControl strip, TabFormStripPainterArgs renderingInfo)
        {
            if (!strip.CaptionVisible)
                return;
            bool rightToLeft = (strip.RightToLeft == System.Windows.Forms.RightToLeft.Yes);

            System.Windows.Forms.Form form = strip.FindForm();

            // Get right most X position of the Quick Access Toolbar
            int right = 0, sysLeft = 0;
            for (int i = strip.CaptionContainerItem.SubItems.Count - 1; i >= 0; i--)
            {
                BaseItem item = strip.CaptionContainerItem.SubItems[i];
                if (!item.Visible || !item.Displayed)
                    continue;
                if (item.ItemAlignment == eItemAlignment.Near && item.Visible && i > 0)
                {
                    if (rightToLeft)
                        right = item.DisplayRectangle.X;
                    else
                        right = item.DisplayRectangle.Right;
                    break;
                }
                else if (item.ItemAlignment == eItemAlignment.Far && item.Visible)
                {
                    if (rightToLeft)
                        sysLeft = item.DisplayRectangle.Right;
                    else
                        sysLeft = item.DisplayRectangle.X;
                }
            }

            if (strip.CaptionContainerItem is CaptionItemContainer && ((CaptionItemContainer)strip.CaptionContainerItem).MoreItems != null)
            {
                if (rightToLeft)
                    right = ((CaptionItemContainer)strip.CaptionContainerItem).MoreItems.DisplayRectangle.X;
                else
                    right = ((CaptionItemContainer)strip.CaptionContainerItem).MoreItems.DisplayRectangle.Right;
            }

            Rectangle r = new Rectangle(right, 2, strip.CaptionContainerItem.WidthInternal - right - (strip.CaptionContainerItem.WidthInternal - sysLeft), strip.GetTotalCaptionHeight());
            strip.CaptionBounds = r;

            if (sysLeft > 0)
            {
                if (rightToLeft)
                    strip.SystemCaptionItemBounds = new Rectangle(r.X, r.Y, sysLeft, r.Height);
                else
                    strip.SystemCaptionItemBounds = new Rectangle(sysLeft, r.Y, strip.CaptionContainerItem.WidthInternal - sysLeft, r.Height);
            }

            //if (right == 0 || r.Height <= 0 || r.Width <= 0)
            //    return;

            //BaseItem startButton = strip.GetApplicationButton();
            //if (startButton != null)
            //{
            //    int startIndex = strip.QuickToolbarItems.IndexOf(startButton);
            //    if (strip.QuickToolbarItems.Count - 1 > startIndex)
            //    {
            //        BaseItem firstItem = strip.QuickToolbarItems[startIndex + 1];
            //        if (rightToLeft)
            //        {
            //            r.Width -= r.Right - firstItem.DisplayRectangle.Right;
            //        }
            //        else
            //        {
            //            r.Width -= firstItem.DisplayRectangle.X - r.X;
            //            r.X = firstItem.DisplayRectangle.X;
            //        }
            //    }
            //}

            //r.Height = ((CaptionItemContainer)strip.CaptionContainerItem).MaxItemHeight + 6;
            //r.X = 0;
            //r.Width = right;
            //strip.QuickToolbarBounds = r;
        }
    }
}
