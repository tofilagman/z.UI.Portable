using System;
using System.Text;
using DevComponents.DotNetBar.Rendering;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Represents the painter for the Office 2007 SystemCaptionItem
    /// </summary>
    internal class Office2007SystemCaptionItemPainter : SystemCaptionItemPainter, IOffice2007Painter
    {
        #region IOffice2007Painter
        private Office2007ColorTable m_ColorTable = null; //new Office2007ColorTable();

        /// <summary>
        /// Gets or sets color table used by renderer.
        /// </summary>
        public Office2007ColorTable ColorTable
        {
            get { return m_ColorTable; }
            set { m_ColorTable = value; }
        }
        #endregion

        #region Internal Implementation
        /// <summary>
        /// Paints the SystemCaptionItem as icon in left hand corner.
        /// </summary>
        /// <param name="e"></param>
        public override void PaintSystemIcon(SystemCaptionItemRendererEventArgs e, bool isEnabled)
        {
            System.Drawing.Graphics g = e.Graphics;
            SystemCaptionItem item = e.SystemCaptionItem;

            Rectangle r = item.DisplayRectangle;
            r.Offset((r.Width - Dpi.Width16) / 2, (r.Height - Dpi.Height16) / 2);
            if (item.Icon != null)
            {
                if (System.Environment.Version.Build <= 3705 && System.Environment.Version.Revision == 288 && System.Environment.Version.Major == 1 && System.Environment.Version.Minor == 0)
                {
                    IntPtr hdc = g.GetHdc();
                    try
                    {
                        NativeFunctions.DrawIconEx(hdc, r.X, r.Y, item.Icon.Handle, r.Width, r.Height, 0, IntPtr.Zero, 3);
                    }
                    finally
                    {
                        g.ReleaseHdc(hdc);
                    }
                }
                else
                    g.DrawIcon(item.Icon, r);
            }
        }

        /// <summary>
        /// Paints the SystemCaptionItem as set of buttons minimize, restore/maximize and close.
        /// </summary>
        /// <param name="e"></param>
        public override void PaintFormButtons(SystemCaptionItemRendererEventArgs e)
        {
            if (e.GlassEnabled) // When Windows Vista Glass is enabled we let system caption button paint themselves
                return;

            System.Drawing.Graphics g = e.Graphics;
            SystemCaptionItem item = e.SystemCaptionItem;
            Office2007SystemButtonColorTable colorTable = m_ColorTable.SystemButton;
            Rectangle r = item.DisplayRectangle;
            Region oldClip = g.Clip;
            Rectangle rclip = r;
            rclip.Height++;
            g.SetClip(rclip);

            Size buttonSize = item.GetButtonSize();
            if (buttonSize.Height > r.Height)
                buttonSize = new Size(r.Height, r.Height);
            bool itemEnabled = item.GetEnabled();
            // Minimize button
            Rectangle rb = new Rectangle(r.X, r.Y + (r.Height - buttonSize.Height) / 2, buttonSize.Width, buttonSize.Height);
            eDotNetBarStyle effectiveStyle = item.EffectiveStyle;
            if (StyleManager.IsMetro(effectiveStyle) && item.ContainerControl is DevComponents.DotNetBar.Metro.MetroForm)
            {
                Form form = (Form)item.ContainerControl;
                if (form.FormBorderStyle == FormBorderStyle.FixedDialog)
                    rb.Inflate(0, -2);
            }
            Office2007SystemButtonStateColorTable ct = colorTable.Default;
            LinearGradientColorTable foregroundOverride = null;
            bool useForegroundOverride = item.ContainerControl is DevComponents.DotNetBar.Metro.MetroTabStrip && StyleManager.Style != eStyle.OfficeMobile2014;
            if (useForegroundOverride)
                foregroundOverride = new LinearGradientColorTable(m_ColorTable.Form.TextColor);
            else
                foregroundOverride = null;

            if (item.HelpVisible && (!item.IsRightToLeft || item.CloseVisible && item.IsRightToLeft))
            {
                Office2007SystemButtonColorTable originalColorTable = colorTable;
                if (item.CloseEnabled && itemEnabled && item.IsRightToLeft && m_ColorTable.SystemButtonClose != null)
                    colorTable = m_ColorTable.SystemButtonClose;

                if (item.CloseEnabled && itemEnabled && item.IsRightToLeft || !item.IsRightToLeft)
                {
                    if (item.MouseDownButton == SystemButton.Help && !item.IsRightToLeft ||
                        item.MouseDownButton == SystemButton.Close && item.IsRightToLeft)
                    {
                        ct = colorTable.Pressed;
                        foregroundOverride = null;
                    }
                    else if (item.MouseOverButton == SystemButton.Help && !item.IsRightToLeft ||
                        item.MouseOverButton == SystemButton.Close && item.IsRightToLeft)
                    {
                        ct = colorTable.MouseOver;
                        foregroundOverride = null;
                    }
                }
                PaintBackground(g, rb, ct);
                if (item.IsRightToLeft)
                    PaintClose(g, rb, ct, item.CloseEnabled && itemEnabled, foregroundOverride);
                else
                    PaintHelp(g, rb, ct, itemEnabled, foregroundOverride);

                rb.Offset(rb.Width + 1, 0);
                colorTable = originalColorTable;
            }

            if (item.MinimizeVisible && item.HelpVisible || item.MinimizeVisible && !item.HelpVisible && (!item.IsRightToLeft || item.CloseVisible && item.IsRightToLeft))
            {
                ct = colorTable.Default;
                if (useForegroundOverride)
                    foregroundOverride = new LinearGradientColorTable(m_ColorTable.Form.TextColor);
                else
                    foregroundOverride = null;

                if (item.CloseEnabled && itemEnabled && item.IsRightToLeft || !item.IsRightToLeft)
                {
                    if (item.MouseDownButton == SystemButton.Minimize && !item.IsRightToLeft ||
                        item.MouseDownButton == SystemButton.Close && item.IsRightToLeft)
                    {
                        ct = colorTable.Pressed;
                        foregroundOverride = null;
                    }
                    else if (item.MouseOverButton == SystemButton.Minimize && !item.IsRightToLeft ||
                        item.MouseOverButton == SystemButton.Close && item.IsRightToLeft)
                    {
                        ct = colorTable.MouseOver;
                        foregroundOverride = null;
                    }
                }
                PaintBackground(g, rb, ct);
                if (item.IsRightToLeft)
                    PaintClose(g, rb, ct, item.CloseEnabled && itemEnabled, foregroundOverride);
                else
                    PaintMinimize(g, rb, ct, itemEnabled, foregroundOverride);

                rb.Offset(rb.Width + 1, 0);
            }

            if (item.RestoreMaximizeVisible)
            {
                if (item.RestoreEnabled && itemEnabled)
                {
                    ct = colorTable.Default;
                    if (useForegroundOverride)
                        foregroundOverride = new LinearGradientColorTable(m_ColorTable.Form.TextColor);
                    else
                        foregroundOverride = null;

                    if (item.MouseDownButton == SystemButton.Restore)
                    {
                        ct = colorTable.Pressed;
                        foregroundOverride = null;
                    }
                    else if (item.MouseOverButton == SystemButton.Restore)
                    {
                        ct = colorTable.MouseOver;
                        foregroundOverride = null;
                    }
                    PaintBackground(g, rb, ct);
                    PaintRestore(g, rb, ct, itemEnabled, foregroundOverride);
                }
                else
                {
                    ct = colorTable.Default;
                    if (useForegroundOverride)
                        foregroundOverride = new LinearGradientColorTable(m_ColorTable.Form.TextColor);
                    else
                        foregroundOverride = null;

                    if (item.MouseDownButton == SystemButton.Maximize)
                    {
                        ct = colorTable.Pressed;
                        foregroundOverride = null;
                    }
                    else if (item.MouseOverButton == SystemButton.Maximize)
                    {
                        ct = colorTable.MouseOver;
                        foregroundOverride = null;
                    }
                    PaintBackground(g, rb, ct);
                    PaintMaximize(g, rb, ct, itemEnabled, foregroundOverride);
                }

                rb.Offset(rb.Width + 1, 0);
            }

            if (item.CloseVisible && !item.IsRightToLeft || !item.HelpVisible && item.MinimizeVisible && item.IsRightToLeft || item.HelpVisible && item.IsRightToLeft)
            {
                Office2007SystemButtonColorTable originalColorTable = colorTable;
                if (item.CloseEnabled && itemEnabled && !item.IsRightToLeft && m_ColorTable.SystemButtonClose != null)
                    colorTable = m_ColorTable.SystemButtonClose;

                ct = colorTable.Default;
                if (useForegroundOverride)
                    foregroundOverride = new LinearGradientColorTable(m_ColorTable.Form.TextColor);
                else
                    foregroundOverride = null;

                if (item.CloseEnabled && itemEnabled && !item.IsRightToLeft ||
                    item.IsRightToLeft)
                {
                    if (item.MouseDownButton == SystemButton.Close && !item.IsRightToLeft ||
                        item.MouseDownButton == SystemButton.Minimize && item.IsRightToLeft)
                    {
                        ct = colorTable.Pressed;
                        foregroundOverride = null;
                    }
                    else if (item.MouseOverButton == SystemButton.Close && !item.IsRightToLeft ||
                        item.MouseOverButton == SystemButton.Minimize && item.IsRightToLeft)
                    {
                        ct = colorTable.MouseOver;
                        foregroundOverride = null;
                    }
                }

                PaintBackground(g, rb, ct);
                if (item.IsRightToLeft)
                {
                    if (item.HelpVisible)
                        PaintHelp(g, rb, ct, itemEnabled, foregroundOverride);
                    else
                        PaintMinimize(g, rb, ct, itemEnabled, foregroundOverride);
                }
                else
                    PaintClose(g, rb, ct, item.CloseEnabled && itemEnabled, foregroundOverride);

                colorTable = originalColorTable;
            }

            g.Clip = oldClip;
            if (oldClip != null) oldClip.Dispose();
        }

        /// <summary>
        /// Paints the background of the button using specified color table colors.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="r">Background bounds</param>
        /// <param name="ct">Color Table</param>
        protected virtual void PaintBackground(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct)
        {
            int cornerSize = 2;
            Rectangle border = r;
            if (ct.OuterBorder != null && !ct.OuterBorder.IsEmpty)
                r.Inflate(-1, -1);

            Rectangle rt = new Rectangle(r.X, r.Y, r.Width, r.Height / 2);
            if (ct.TopBackground != null && !ct.TopBackground.IsEmpty)
            {
                rt.Height++;
                if (ct.BottomBackground == null) rt = r;
                DisplayHelp.FillRectangle(g, rt, ct.TopBackground);
                rt.Height--;
            }

            //Region oldClip = g.Clip;

            if (ct.BottomBackground != null && !ct.BottomBackground.IsEmpty)
            {
                rt.Y += rt.Height;
                rt.Height = (r.Height - rt.Height);
                DisplayHelp.FillRectangle(g, rt, ct.BottomBackground);
            }

            // Highlight
            if (ct.TopHighlight != null && !ct.TopHighlight.IsEmpty)
            {
                Rectangle fill = r;
                fill.Height = fill.Height / 2;
                DrawHighlight(g, ct.TopHighlight, fill, new PointF(fill.X + fill.Width / 2, fill.Bottom));
            }

            // Highlight
            if (ct.BottomHighlight != null && !ct.BottomHighlight.IsEmpty)
            {
                Rectangle fill = r;
                fill.Height = fill.Height / 2;
                fill.Y += (r.Height - fill.Height);
                DrawHighlight(g, ct.BottomHighlight, fill, new PointF(fill.X + fill.Width / 2, fill.Bottom));
            }

            if (ct.OuterBorder != null && !ct.OuterBorder.IsEmpty)
            {
                DisplayHelp.DrawRoundGradientRectangle(g, border, ct.OuterBorder, 1, cornerSize);
                border.Inflate(-1, -1);
            }

            if (ct.InnerBorder != null && !ct.InnerBorder.IsEmpty)
            {
                DisplayHelp.DrawRoundGradientRectangle(g, border, ct.InnerBorder, 1, cornerSize);
            }
        }

        private void DrawHighlight(Graphics g, LinearGradientColorTable c, Rectangle r, PointF centerPoint)
        {
            Rectangle ellipse = new Rectangle(r.X, r.Y, r.Width, r.Height * 2);
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(ellipse);
            PathGradientBrush brush = new PathGradientBrush(path);
            brush.CenterColor = c.Start;
            brush.SurroundColors = new Color[] { c.End };
            brush.CenterPoint = centerPoint;
            Blend blend = new Blend();
            blend.Factors = new float[] { 0f, .5f, 1f };
            blend.Positions = new float[] { .0f, .4f, 1f };
            brush.Blend = blend;

            g.FillRectangle(brush, r);
            brush.Dispose();
            path.Dispose();
        }

        protected virtual void PaintMinimize(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
        {
            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.Default;

            Size s = new Size(Dpi.Width7, Dpi.Height3);
            Rectangle rm = GetSignRect(r, s);

            DisplayHelp.DrawLine(g, rm.X, rm.Y, rm.Right, rm.Y, ct.DarkShade, 1);
            rm.Offset(0, 1);
            LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
            DisplayHelp.DrawLine(g, rm.X, rm.Y, rm.Right, rm.Y, foreground.Start, 1);
            rm.Offset(0, 1);
            DisplayHelp.DrawLine(g, rm.X, rm.Y, rm.Right, rm.Y, ct.LightShade, 1);

            g.SmoothingMode = sm;
        }

        protected virtual Rectangle GetSignRect(Rectangle r, Size s)
        {
            if (r.Height < 10)
                return Rectangle.Empty;

            return new Rectangle(r.X + (r.Width - s.Width) / 2, r.Bottom - r.Height / 4 - s.Height, s.Width, s.Height);
        }

        protected virtual void PaintRestore(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
        {
            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.Default;

            Size s = new Size(Dpi.Width9, Dpi.Height8);
            Rectangle rm = GetSignRect(r, s);

            Rectangle r1 = new Rectangle(rm.X, rm.Y + Dpi.Height1, Dpi.Width7, Dpi.Height7);
            Region oldClip = g.Clip;
            for (int i = 0; i < 2; i++)
            {
                LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
                DisplayHelp.DrawGradientRectangle(g, new Rectangle(r1.X, r1.Y + 1, r1.Width, r1.Height - 1), foreground, 1);
                DisplayHelp.DrawLine(g, r1.X, r1.Y, r1.Right - 1, r1.Y, ct.DarkShade, 1);
                DisplayHelp.DrawLine(g, r1.X + 1, r1.Y + 2, r1.Right - 2, r1.Y + 2, ct.LightShade, 1);
                DisplayHelp.DrawLine(g, r1.X + 1, r1.Y + 2, r1.Right - 2, r1.Y + 2, ct.LightShade, 1);
                g.SetClip(r1, CombineMode.Exclude);
                r1.Offset(Dpi.Width2, -Dpi.Height1);
            }

            if (oldClip != null)
                g.Clip = oldClip;
            else
                g.ResetClip();

            g.SmoothingMode = sm;
        }

        protected virtual void PaintMaximize(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
        {
            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.Default;

            Size s = new Size(Dpi.Width9, Dpi.Height8);
            Rectangle rm = GetSignRect(r, s);

            Color color = isEnabled ? ct.DarkShade : Color.FromArgb(128, ct.DarkShade);
            DisplayHelp.DrawLine(g, rm.X, rm.Y, rm.Right - 1, rm.Y, color, 1);
            rm.Y++;
            rm.Height--;
            LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
            color = isEnabled ? foreground.Start : Color.FromArgb(128, foreground.Start);
            DisplayHelp.DrawLine(g, rm.X, rm.Y, rm.Right - 1, rm.Y, color, 1);
            rm.Y++;
            rm.Height--;
            Rectangle r1 = rm;
            r1.Height--;
            if (isEnabled)
                DisplayHelp.DrawGradientRectangle(g, r1, foreground, 1);
            else
                DisplayHelp.DrawGradientRectangle(g, r1, Color.FromArgb(128, foreground.Start), Color.FromArgb(128, foreground.End), foreground.GradientAngle, 1);

            color = isEnabled ? ct.LightShade : Color.FromArgb(128, ct.LightShade);
            DisplayHelp.DrawLine(g, rm.X, rm.Bottom - 1, rm.Right - 1, rm.Bottom - 1, color, 1);

            g.SmoothingMode = sm;
        }

        protected virtual void PaintClose(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
        {
            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.Default;
            Size s = new Size(Dpi.Width11, Dpi.Height9);
            Rectangle rm = GetSignRect(r, s);

            Rectangle r1 = rm;
            r1.Inflate(-Dpi.Width1, 0);
            r1.Height--;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddLine(r1.X, r1.Y, r1.X + Dpi.Width2, r1.Y);
                path.AddLine(r1.X + Dpi.Width2, r1.Y, r1.X + Dpi.Width4, r1.Y + Dpi.Width2);
                path.AddLine(r1.X + Dpi.Width4, r1.Y + Dpi.Width2, r1.X + Dpi.Width6, r1.Y + 0);
                path.AddLine(r1.X + Dpi.Width6, r1.Y + 0, r1.X + Dpi.Width8, r1.Y + 0);
                path.AddLine(r1.X + Dpi.Width8, r1.Y + 0, r1.X + Dpi.Width5, r1.Y + Dpi.Height3);
                path.AddLine(r1.X + Dpi.Width5, r1.Y + Dpi.Height4, r1.X + Dpi.Width8, r1.Y + Dpi.Height7);
                path.AddLine(r1.X + Dpi.Width8, r1.Y + Dpi.Height7, r1.X + Dpi.Width6, r1.Y + Dpi.Height7);
                path.AddLine(r1.X + Dpi.Width6, r1.Y + Dpi.Height7, r1.X + Dpi.Width4, r1.Y + Dpi.Height5);
                path.AddLine(r1.X + Dpi.Width4, r1.Y + Dpi.Height5, r1.X + Dpi.Width2, r1.Y + Dpi.Height7);
                path.AddLine(r1.X + Dpi.Width2, r1.Y + Dpi.Height7, r1.X + 0, r1.Y + Dpi.Height7);
                path.AddLine(r1.X + 0, r1.Y + Dpi.Height7, r1.X + Dpi.Width3, r1.Y + Dpi.Height4);
                path.AddLine(r1.X + Dpi.Width3, r1.Y + Dpi.Height3, r1.X, r1.Y);
                LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
                if (isEnabled)
                {
                    DisplayHelp.FillPath(g, path, foreground);
                    DisplayHelp.DrawGradientPathBorder(g, path, foreground, 1);
                }
                else
                {
                    LinearGradientColorTable lg = new LinearGradientColorTable(foreground.Start.IsEmpty ? foreground.Start : Color.FromArgb(128, foreground.Start),
                        foreground.End.IsEmpty ? foreground.End : Color.FromArgb(128, foreground.End),
                        foreground.GradientAngle);
                    DisplayHelp.FillPath(g, path, lg);
                    DisplayHelp.DrawGradientPathBorder(g, path, lg, 1);
                }
            }

            if (!ct.DarkShade.IsEmpty)
            {
                using (Pen pen = new Pen(isEnabled ? ct.DarkShade : Color.FromArgb(128, ct.DarkShade), 1))
                {
                    g.DrawLine(pen, r1.X, r1.Y, r1.X + Dpi.Width2, r1.Y);
                    g.DrawLine(pen, r1.X + Dpi.Width2, r1.Y, r1.X + Dpi.Width4, r1.Y + Dpi.Width2);
                    g.DrawLine(pen, r1.X + Dpi.Width4, r1.Y + Dpi.Width2, r1.X + Dpi.Width6, r1.Y + 0);
                    g.DrawLine(pen, r1.X + Dpi.Width6, r1.Y + 0, r1.X + Dpi.Width8, r1.Y + 0);
                }
            }

            if (!ct.LightShade.IsEmpty)
            {
                using (Pen pen = new Pen(isEnabled ? ct.LightShade : Color.FromArgb(128, ct.LightShade), 1))
                {
                    g.DrawLine(pen, rm.X + 0, rm.Y + Dpi.Height8, rm.X + Dpi.Width3, rm.Y + Dpi.Height8);
                    g.DrawLine(pen, rm.X + Dpi.Width3, rm.Y + Dpi.Height8, rm.X + Dpi.Width5, rm.Y + Dpi.Height6);
                    g.DrawLine(pen, rm.X + Dpi.Width5, rm.Y + Dpi.Height6, rm.X + Dpi.Width7, rm.Y + Dpi.Height8);
                    g.DrawLine(pen, rm.X + Dpi.Width7, rm.Y + Dpi.Height8, rm.X + Dpi.Width10, rm.Y + Dpi.Height8);
                }
            }

            g.SmoothingMode = sm;

        }

        protected virtual void PaintHelp(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
        {
            SmoothingMode sm = g.SmoothingMode;
            TextRenderingHint th = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.Default;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
#if FRAMEWORK20
            using (Font font = new Font(SystemFonts.DefaultFont, FontStyle.Bold))
#else
			using(Font font = new Font("Arial", 10, FontStyle.Bold))
#endif
            {
                Size s = TextDrawing.MeasureString(g, "?", font);
                s.Width += 4;
                s.Height -= 2;
                Rectangle rm = GetSignRect(r, s);

                rm.Offset(1, 1);
                Color color = isEnabled ? ct.LightShade : Color.FromArgb(128, ct.LightShade);
                using (SolidBrush brush = new SolidBrush(color))
                    g.DrawString("?", font, brush, rm);
                rm.Offset(-1, -1);
                LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
                color = isEnabled ? foreground.Start : Color.FromArgb(128, foreground.Start);
                using (SolidBrush brush = new SolidBrush(color))
                    g.DrawString("?", font, brush, rm);

            }
            g.SmoothingMode = sm;
            g.TextRenderingHint = th;
        }
        #endregion
    }
}
