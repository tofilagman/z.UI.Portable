using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using DevComponents.DotNetBar.Rendering;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace DevComponents.DotNetBar.Metro.Rendering
{
    internal class MetroCaptionItemPainter : Office2007SystemCaptionItemPainter
    {
        #region Internal Implementation
        protected override Rectangle GetSignRect(Rectangle r, Size s)
        {
            if (r.Height < 10)
                return Rectangle.Empty;

            return new Rectangle(r.X + (r.Width - s.Width) / 2, r.Y + (int)Math.Ceiling((double)(r.Height - s.Height)/2), s.Width, s.Height);
            //return new Rectangle(r.X + (r.Width - s.Width) / 2, r.Bottom - r.Height / 4 - s.Height - 3, s.Width, s.Height);
        }

        protected override void PaintMinimize(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
        {
            Size s = new Size(Dpi.Width9, Dpi.Height2);
            Rectangle rm = GetSignRect(r, s);
            LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
            if (isEnabled)
                DisplayHelp.FillRectangle(g, rm, foreground);
            else
                DisplayHelp.FillRectangle(g, rm, GetDisabledColor(foreground.Start), GetDisabledColor(foreground.End), foreground.GradientAngle);
        }

        internal static Color GetDisabledColor(Color color)
        {
            if (color.IsEmpty) return color;
            return Color.FromArgb(128, color);
        }


        protected override void PaintRestore(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
        {
            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            Size s = new Size(Dpi.Width10, Dpi.Height10);
            Rectangle rm = GetSignRect(r, s);
            Region oldClip = g.Clip;

            LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
            LinearGradientColorTable buttonTable = isEnabled ? foreground : new LinearGradientColorTable(GetDisabledColor(foreground.Start), GetDisabledColor(foreground.End), foreground.GradientAngle);
            using (Brush fill = DisplayHelp.CreateBrush(rm, foreground))
            {
                Rectangle inner = new Rectangle(rm.X + Dpi.Width4, rm.Y + Dpi.Width2, Dpi.Width6, Dpi.Height4);
                g.SetClip(inner, CombineMode.Exclude);
                g.SetClip(new Rectangle(rm.X + Dpi.Width1, rm.Y + Dpi.Height5, Dpi.Width6, Dpi.Height4), CombineMode.Exclude);

                g.FillRectangle(fill, rm.X + Dpi.Width3, rm.Y, Dpi.Width8, Dpi.Height7);
                g.ResetClip();

                inner = new Rectangle(rm.X + Dpi.Width1, rm.Y + Dpi.Height5, Dpi.Width6, Dpi.Height4);
                g.SetClip(inner, CombineMode.Exclude);
                g.FillRectangle(fill, rm.X, rm.Y + Dpi.Height3, Dpi.Width8, Dpi.Height7);
                g.ResetClip();
            }
            if (oldClip != null)
            {
                g.Clip = oldClip;
                oldClip.Dispose();
            }
            g.SmoothingMode = sm;
        }

        protected override void PaintMaximize(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
        {
            Size s = new Size(Dpi.Width10, Dpi.Height10);
            Rectangle rm = GetSignRect(r, s);
            Region oldClip = g.Clip;

            Rectangle inner = new Rectangle(rm.X + Dpi.Width1, rm.Y + Dpi.Height3, (ct.DarkShade.IsEmpty ? Dpi.Width8 : Dpi.Width7), (ct.DarkShade.IsEmpty ? Dpi.Height6 : Dpi.Height5));
            g.SetClip(inner, CombineMode.Exclude);
            LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
            if (isEnabled)
                DisplayHelp.FillRectangle(g, rm, foreground);
            else
                DisplayHelp.FillRectangle(g, rm, GetDisabledColor(foreground.Start), GetDisabledColor(foreground.End), foreground.GradientAngle);

            if (oldClip != null)
            {
                g.Clip = oldClip;
                oldClip.Dispose();
            }
        }

        protected override void PaintClose(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
        {
            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.Default;

            Size s = new Size(Dpi.Width8, Dpi.Height8);
            Rectangle rm = GetSignRect(r, s);

            Rectangle r1 = rm;
            r1.Offset(0, -1);

            LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
            if (isEnabled)
            {
                using (Pen pen = new Pen(foreground.Start, Dpi.Width2))
                {
                    g.DrawLine(pen, r1.X, r1.Y, r1.Right, r1.Bottom);
                    g.DrawLine(pen, r1.Right, r1.Y, r1.X, r1.Bottom);
                }
            }
            else
            {
                using (Pen pen = new Pen(GetDisabledColor(foreground.Start), Dpi.Width2))
                {
                    g.DrawLine(pen, r1.X, r1.Y, r1.Right, r1.Bottom);
                    g.DrawLine(pen, r1.Right, r1.Y, r1.X, r1.Bottom);
                }
            }

            g.SmoothingMode = sm;
        }

        protected override void PaintHelp(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
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
                s.Width += Dpi.Width4;
                s.Height -= Dpi.Width2;
                Rectangle rm = GetSignRect(r, s);

                rm.Offset(1, 1);
                Color color = isEnabled ? ct.DarkShade : GetDisabledColor(ct.DarkShade);
                using (SolidBrush brush = new SolidBrush(color))
                    g.DrawString("?", font, brush, rm);
                rm.Offset(-1, -1);
                LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
                color = isEnabled ? foreground.Start : GetDisabledColor(foreground.Start);
                using (SolidBrush brush = new SolidBrush(color))
                    g.DrawString("?", font, brush, rm);

            }
            g.SmoothingMode = sm;
            g.TextRenderingHint = th;
        }

        #endregion
    }
}
