using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Threading;

namespace DevComponents.DotNetBar.Rendering
{
    internal class Office2010RangeSliderPainter : RangeSliderPainter, IOffice2007Painter
    {
        #region IOffice2007Painter
        private Office2007ColorTable _ColorTable = null; //new Office2007ColorTable();

        /// <summary>
        /// Gets or sets color table used by renderer.
        /// </summary>
        public Office2007ColorTable ColorTable
        {
            get { return _ColorTable; }
            set { _ColorTable = value; }
        }
        #endregion

        #region Implementation
        public override void Paint(RangeSliderItemRendererEventArgs e)
        {
            RangeSliderItem item = e.SliderItem;
            ItemPaintArgs p = e.ItemPaintArgs;
            Graphics g = e.Graphics;
            Office2010RangeSliderColorTable table = GetColorTable(item, p.ContainerControl);
            eOrientation sliderOrientation = item.SliderOrientation;
            RangeValue minmaxValue = item.GetNormalizedMinMaxValues();
            int min = minmaxValue.Min;
            int max = minmaxValue.Max;
            RangeValue value = item.GetNormalizedRangeValue();

            if (table == null)
            {
                g.FillRectangle(Brushes.Gray, item.Bounds);
                return;
            }

            if (item.RangeLineHeight > 0)
            {
                Rectangle rangeBounds;
                if (item.SliderOrientation == eOrientation.Horizontal)
                {
                    rangeBounds = new Rectangle(item.TicksBounds.X,
                        item.RangeButtonMinBounds.Y + (item.RangeButtonMinBounds.Height - item.RangeLineHeight) / 2 + 1,
                        item.TicksBounds.Width, item.RangeLineHeight);
                }
                else
                {
                    rangeBounds = new Rectangle(
                        item.RangeButtonMinBounds.X + (item.RangeButtonMinBounds.Width - item.RangeLineHeight) / 2,
                        item.TicksBounds.Y,
                        item.RangeLineHeight, item.TicksBounds.Height);
                }
                if (table.RangeBackground != null)
                {
                    table.RangeBackground.LinearGradientAngle = (item.SliderOrientation == eOrientation.Horizontal) ? 90 : 180;
                    Rectangle rb = rangeBounds;
                    rb.Offset(item.Bounds.Location);
                    rb.Height--;
                    using (Brush brush = DisplayHelp.CreateBrush(rb, table.RangeBackground))
                        DisplayHelp.FillRoundedRectangle(g, brush, rb, table.RangeCornerRadius);
                    rb.Height++;
                    if (!table.RangeLineColor.IsEmpty)
                        DisplayHelp.DrawRoundedRectangle(g, table.RangeLineColor, rb, table.RangeCornerRadius);
                }
                if (table.RangeValueBackground != null)
                {
                    if (item.SliderOrientation == eOrientation.Horizontal)
                    {
                        rangeBounds.X = item.RangeButtonMinBounds.Right;
                        rangeBounds.Width = item.RangeButtonMaxBounds.X - item.RangeButtonMinBounds.Right;
                    }
                    else
                    {
                        rangeBounds.Y = item.RangeButtonMinBounds.Bottom;
                        rangeBounds.Height = item.RangeButtonMaxBounds.Y - item.RangeButtonMinBounds.Bottom;
                    }
                    Rectangle rb = rangeBounds;
                    rb.Offset(item.Bounds.Location);
                    table.RangeValueBackground.LinearGradientAngle = (item.SliderOrientation == eOrientation.Horizontal) ? 90 : 180;
                    rb.Height--;
                    if (item.RangeValueColor.IsEmpty)
                    {
                        using (Brush brush = DisplayHelp.CreateBrush(rb, table.RangeValueBackground))
                            g.FillRectangle(brush, rb);
                    }
                    else
                    {
                        using(SolidBrush brush=new SolidBrush(item.RangeValueColor))
                            g.FillRectangle(brush, rb);
                    }
                    rb.Height++;
                    if (!table.RangeValueLineColor.IsEmpty)
                    {
                        using (Pen pen = new Pen(table.RangeValueLineColor))
                        {
                            if (item.SliderOrientation == eOrientation.Horizontal)
                            {
                                g.DrawLine(pen, rb.X, rb.Y, rb.Right, rb.Y);
                                g.DrawLine(pen, rb.X, rb.Bottom - 1, rb.Right, rb.Bottom - 1);
                            }
                            else
                            {
                                g.DrawLine(pen, rb.X, rb.Y, rb.X, rb.Bottom);
                                g.DrawLine(pen, rb.Right - 1, rb.Y, rb.Right - 1, rb.Bottom);
                            }
                        }
                    }
                }
            }

            if (item.TicksVisible && !table.TickLineColor.IsEmpty)
            {
                Rectangle ticksBounds = item.TicksBounds;
                ticksBounds.Offset(item.Bounds.Location);
                int steps = item.TicksStep;
                //g.FillRectangle(Brushes.Yellow, ticksBounds);
                if (sliderOrientation == eOrientation.Horizontal)
                {
                    DrawHTicks(g, table, min, max, ticksBounds, steps);
                    if (!item.TicksBounds2.IsEmpty)
                    {
                        ticksBounds = item.TicksBounds2;
                        ticksBounds.Offset(item.Bounds.Location);
                        DrawHTicks(g, table, min, max, ticksBounds, steps);
                    }
                }
                else
                {
                    DrawVTicks(g, table, min, max, ticksBounds, steps);
                    if (!item.TicksBounds2.IsEmpty)
                    {
                        ticksBounds = item.TicksBounds2;
                        ticksBounds.Offset(item.Bounds.Location);
                        DrawVTicks(g, table, min, max, ticksBounds, steps);
                    }
                }
            }
            g.ResetClip();
            if (item.MinRangeSliderImage != null)
            {
                Rectangle br = item.RangeButtonMinBounds;
                br.Offset(item.Bounds.Location);
                g.DrawImage(item.MinRangeSliderImage, br);
            }
            else
            {
                Office2010RangeChangePartColorTable ct = table.MinRangeSlider.Default;
                if (!item.GetEnabled() && table.MinRangeSlider.Disabled != null)
                    ct = table.MinRangeSlider.Disabled;
                else if (item.MouseDownPart == eRangeSliderPart.MinRangeSlider && table.MinRangeSlider.Pressed != null)
                    ct = table.MinRangeSlider.Pressed;
                else if (item.MouseOverPart == eRangeSliderPart.MinRangeSlider && table.MinRangeSlider.MouseOver != null)
                    ct = table.MinRangeSlider.MouseOver;
                if (ct.Background != null)
                    ct.Background.LinearGradientAngle = (item.SliderOrientation == eOrientation.Horizontal) ? 90 : 0;
                Rectangle br = item.RangeButtonMinBounds;
                br.Offset(item.Bounds.Location);
                using (GraphicsPath sliderButtonPath = CreateMinSlider(br, item.TicksPosition, item.SliderOrientation))
                {
                    using (Brush brush = DisplayHelp.CreateBrush(br, ct.Background))
                        g.FillPath(brush, sliderButtonPath);
                    if (!ct.BorderLightColor.IsEmpty)
                    {
                        br.Inflate(-1, -1);
                        using (GraphicsPath borderPath = CreateMinSlider(br, item.TicksPosition, item.SliderOrientation))
                        {
                            using (Pen pen = new Pen(ct.BorderLightColor, 1))
                                g.DrawPath(pen, sliderButtonPath);
                        }
                    }
                    if (!ct.BorderColor.IsEmpty)
                    {
                        using (Pen pen = new Pen(ct.BorderColor, 1))
                            g.DrawPath(pen, sliderButtonPath);
                    }
                }
            }

            if (item.MaxRangeSliderImage != null)
            {
                Rectangle br = item.RangeButtonMaxBounds;
                br.Offset(item.Bounds.Location);
                g.DrawImage(item.MaxRangeSliderImage, br);
            }
            else
            {
                Office2010RangeChangePartColorTable ct = table.MaxRangeSlider.Default;
                if (!item.GetEnabled() && table.MaxRangeSlider.Disabled != null)
                    ct = table.MaxRangeSlider.Disabled;
                else if (item.MouseDownPart == eRangeSliderPart.MaxRangeSlider && table.MaxRangeSlider.Pressed != null)
                    ct = table.MaxRangeSlider.Pressed;
                else if (item.MouseOverPart == eRangeSliderPart.MaxRangeSlider && table.MaxRangeSlider.MouseOver != null)
                    ct = table.MaxRangeSlider.MouseOver;
                if (ct.Background != null)
                    ct.Background.LinearGradientAngle = (item.SliderOrientation == eOrientation.Horizontal) ? 90 : 0;
                Rectangle br = item.RangeButtonMaxBounds;
                br.Offset(item.Bounds.Location);
                using (GraphicsPath sliderButtonPath = CreateMaxSlider(br, item.TicksPosition, item.SliderOrientation))
                {
                    using (Brush brush = DisplayHelp.CreateBrush(br, ct.Background))
                        g.FillPath(brush, sliderButtonPath);
                    if (!ct.BorderLightColor.IsEmpty)
                    {
                        br.Inflate(-1, -1);
                        using (GraphicsPath borderPath = CreateMaxSlider(br, item.TicksPosition, item.SliderOrientation))
                        {
                            using (Pen pen = new Pen(ct.BorderLightColor, 1))
                                g.DrawPath(pen, sliderButtonPath);
                        }
                    }
                    if (!ct.BorderColor.IsEmpty)
                    {
                        using (Pen pen = new Pen(ct.BorderColor, 1))
                            g.DrawPath(pen, sliderButtonPath);
                    }
                }

                //using (GraphicsPath sliderButtonPath = CreateMaxSlider(item.RangeButtonMaxBounds, item.TicksPosition, item.SliderOrientation))
                //{
                //    g.FillPath(Brushes.Red, sliderButtonPath);
                //    g.DrawPath(Pens.BlanchedAlmond, sliderButtonPath);
                //}
            }
        }

        private static void DrawHTicks(Graphics g, Office2010RangeSliderColorTable table, int min, int max, Rectangle ticksBounds, int steps)
        {
            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            if (ticksBounds.Width / 2 < Math.Abs(max - min) / steps)
                steps = (int)Math.Max(2, Math.Ceiling(Math.Abs(max - min) / ((float)ticksBounds.Width / 2)));
            int xstep = (int)RangeSliderItem.GetXStep(ticksBounds.Width, min, max, steps);
            int x = ticksBounds.X;
            using (Pen linePen = new Pen(table.TickLineColor))
            {
                for (int i = min; i <= max; i += steps)
                {
                    g.DrawLine(linePen, x, ticksBounds.Y, x, ticksBounds.Bottom - 1);
                    x += xstep;
                }
            }
            g.SmoothingMode = sm;
        }
        private static void DrawVTicks(Graphics g, Office2010RangeSliderColorTable table, int min, int max, Rectangle ticksBounds, int steps)
        {
            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;
            if (ticksBounds.Height / 2 < Math.Abs(max - min) / steps)
                steps = Math.Max(2, Math.Abs(max - min) / (ticksBounds.Height / 2));
            int y = ticksBounds.Y;
            int  ystep = (int)RangeSliderItem.GetXStep(ticksBounds.Height, min, max, steps);
            using (Pen linePen = new Pen(table.TickLineColor))
            {
                for (int i = min; i <= max; i += steps)
                {
                    g.DrawLine(linePen, ticksBounds.X, y, ticksBounds.Right - 1, y);
                    y += ystep;
                }
            }
            g.SmoothingMode = sm;
        }

        //private void PaintTrackPart(Office2010RangeChangePartColorTable ct, Rectangle r, Graphics g, eOrientation orientation, eDotNetBarStyle effectiveStyle)
        //{
        //    if (r.Width <= 0 || r.Height <= 0) return;

        //    if (orientation == eOrientation.Vertical)
        //    {
        //        // Left
        //        Matrix m = new Matrix();
        //        m.RotateAt(-90, new PointF(r.X, r.Bottom));
        //        m.Translate(r.Height, r.Width - r.Height, MatrixOrder.Append);
        //        g.Transform = m;
        //    }
        //    if (StyleManager.IsMetro(effectiveStyle))
        //    {
        //        if (!ct.PartBackground.IsEmpty)
        //        {
        //            SmoothingMode sm = g.SmoothingMode;
        //            g.SmoothingMode = SmoothingMode.Default;
        //            Rectangle slideBounds = new Rectangle(r.X + (r.Width - 3) / 2, r.Y + 2, 3, r.Height - 4);
        //            using (Brush brush = DisplayHelp.CreateBrush(slideBounds, ct.Background))
        //            {
        //                g.FillRectangle(brush, slideBounds);
        //            }
        //            g.SmoothingMode = sm;
        //        }
        //    }
        //    else
        //    {
        //        using (GraphicsPath path = new GraphicsPath())
        //        {
        //            path.AddLine(r.X, r.Y, r.X + 11, r.Y);
        //            path.AddLine(r.X + 11, r.Y, r.X + 11, r.Y + 9);
        //            path.AddLine(r.X + 11, r.Y + 9, r.X + 6, r.Y + 15);
        //            path.AddLine(r.X + 5, r.Y + 15, r.X, r.Y + 10);
        //            path.CloseAllFigures();
        //            using (SolidBrush brush = new SolidBrush(ct.BorderLightColor))
        //                g.FillPath(brush, path);
        //        }

        //        SmoothingMode sm = g.SmoothingMode;
        //        g.SmoothingMode = SmoothingMode.AntiAlias;
        //        r.Offset(1, 1);
        //        using (GraphicsPath path = new GraphicsPath())
        //        {
        //            path.AddLine(r.X, r.Y, r.X + 8, r.Y);
        //            path.AddLine(r.X + 8, r.Y + 8, r.X + 4, r.Y + 12);
        //            path.AddLine(r.X, r.Y + 8, r.X, r.Y);
        //            path.CloseAllFigures();

        //            if (ct.PartBackground.Colors.Count > 0)
        //            {
        //                using (Brush brush = DisplayHelp.CreateBrush(Rectangle.Ceiling(path.GetBounds()), ct.Background))
        //                {
        //                    g.FillPath(brush, path);
        //                }
        //            }

        //            using (Pen pen = new Pen(ct.PartBorderColor, 1))
        //                g.DrawPath(pen, path);
        //        }

        //        using (Pen pen = new Pen(Color.FromArgb(200, ct.PartForeColor), 1))
        //            g.DrawLine(pen, r.X + 4, r.Y + 3, r.X + 4, r.Y + 8);

        //        using (Pen pen = new Pen(ct.PartForeLightColor, 1))
        //            g.DrawLine(pen, r.X + 5, r.Y + 4, r.X + 5, r.Y + 7);

        //        g.SmoothingMode = sm;
        //    }

        //    if (orientation == eOrientation.Vertical)
        //        g.ResetTransform();
        //}

        private GraphicsPath CreateMinSlider(Rectangle r, eTicksPosition position, eOrientation sliderOrientation)
        {
            GraphicsPath path = new GraphicsPath();
            if (sliderOrientation == eOrientation.Horizontal)
            {
                if (position == eTicksPosition.Top)
                {
                    path.AddLine(r.Right, r.Y, r.Right, r.Bottom);
                    path.AddLine(r.Right, r.Bottom, r.X, r.Bottom);
                    path.AddLine(r.X, r.Bottom, r.X, r.Y + r.Width);
                    path.CloseAllFigures();
                }
                else if (position == eTicksPosition.Bottom)
                {
                    path.AddLine(r.Right, r.Bottom, r.Right, r.Y);
                    path.AddLine(r.Right, r.Y, r.X, r.Y);
                    path.AddLine(r.X, r.Y, r.X, r.Bottom - r.Width);
                    path.CloseAllFigures();
                }
                else
                {
                    path.AddRectangle(r);
                }
            }
            else
            {
                if (position == eTicksPosition.Top)
                {
                    path.AddLine(r.X, r.Bottom, r.Right, r.Bottom);
                    path.AddLine(r.Right, r.Bottom, r.Right, r.Y);
                    path.AddLine(r.Right, r.Y, r.X + r.Height, r.Y);
                    path.CloseAllFigures();
                }
                else if (position == eTicksPosition.Bottom)
                {
                    path.AddLine(r.Right, r.Bottom, r.X, r.Bottom);
                    path.AddLine(r.X, r.Bottom, r.X, r.Y);
                    path.AddLine(r.X, r.Y, r.Right - r.Height, r.Y);
                    path.CloseAllFigures();
                }
                else
                {
                    path.AddRectangle(r);
                }
            }
            return path;
        }
        private GraphicsPath CreateMaxSlider(Rectangle r, eTicksPosition position, eOrientation sliderOrientation)
        {
            GraphicsPath path = new GraphicsPath();

            if (sliderOrientation == eOrientation.Horizontal)
            {
                if (position == eTicksPosition.Top)
                {
                    path.AddLine(r.X, r.Y, r.X, r.Bottom);
                    path.AddLine(r.X, r.Bottom, r.Right, r.Bottom);
                    path.AddLine(r.Right, r.Bottom, r.Right, r.Y + r.Width);
                    path.CloseAllFigures();
                }
                else if (position == eTicksPosition.Bottom)
                {
                    path.AddLine(r.X, r.Bottom, r.X, r.Y);
                    path.AddLine(r.X, r.Y, r.Right, r.Y);
                    path.AddLine(r.Right, r.Y, r.Right, r.Bottom - r.Width);
                    path.CloseAllFigures();
                }
                else
                {
                    path.AddRectangle(r);
                }
            }
            else
            {
                if (position == eTicksPosition.Top)
                {
                    path.AddLine(r.X, r.Y, r.Right, r.Y);
                    path.AddLine(r.Right, r.Y, r.Right, r.Bottom);
                    path.AddLine(r.Right, r.Bottom, r.X + r.Height, r.Bottom);
                    path.CloseAllFigures();
                }
                else if (position == eTicksPosition.Bottom)
                {
                    path.AddLine(r.Right, r.Y, r.X, r.Y);
                    path.AddLine(r.X, r.Y, r.X, r.Bottom);
                    path.AddLine(r.X, r.Bottom, r.Right - r.Height, r.Bottom);
                    path.CloseAllFigures();
                }
                else
                {
                    path.AddRectangle(r);
                }
            }
            return path;
        }
        protected virtual Office2010RangeSliderColorTable GetColorTable(RangeSliderItem item, System.Windows.Forms.Control container)
        {
            if (container == null)
                return _ColorTable.RangeSlider;

            Office2007ColorTable table = _ColorTable;

            string key = Office2007ColorTable.GetContextualKey(typeof(Office2010RangeSliderColorTable), container.GetType());
            object st = null;

            if (container is Bar)
            {
                if (table.ContextualTables.TryGetValue(key + "+" + ((Bar)container).BarType.ToString(), out st))
                    return (Office2010RangeSliderColorTable)st;
            }

            if (table.ContextualTables.TryGetValue(key, out st))
                return (Office2010RangeSliderColorTable)st;

            return _ColorTable.RangeSlider;
        }
        #endregion
    }
}
