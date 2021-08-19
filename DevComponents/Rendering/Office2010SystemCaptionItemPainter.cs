using System;
using System.Text;
using DevComponents.DotNetBar.Rendering;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar.Rendering
{
    internal class Office2010SystemCaptionItemPainter : Office2007SystemCaptionItemPainter
    {
        #region Internal Implementation
        protected override void PaintMinimize(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
        {
            //SmoothingMode sm = g.SmoothingMode;
            //g.SmoothingMode = SmoothingMode.AntiAlias;

            Size s = new Size(Dpi.Width11, Dpi.Height5);
            Rectangle rm = GetSignRect(r, s);

            DisplayHelp.DrawRoundedRectangle(g, ct.DarkShade, (foregroundOverride ?? ct.Foreground), rm, 1);

            //g.SmoothingMode = sm;
        }

        protected override void PaintRestore(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
        {
            //SmoothingMode sm = g.SmoothingMode;
            //g.SmoothingMode = SmoothingMode.Default;

            Size s = new Size(Dpi.Width12, Dpi.Height11);
            Rectangle rm = GetSignRect(r, s);
            Region oldClip = g.Clip;

            LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
            using (Brush fill = DisplayHelp.CreateBrush(rm, foreground))
            {
                using (Pen pen = new Pen(ct.DarkShade))
                {
                    using (GraphicsPath path = DisplayHelp.GetRoundedRectanglePath(new Rectangle(rm.X + Dpi.Width5, rm.Y, Dpi.Width8, Dpi.Height8), 1))
                    {
                        Rectangle inner = new Rectangle(rm.X + Dpi.Width7, rm.Y + Dpi.Height4, Dpi.Width4, Dpi.Width2);
                        g.SetClip(inner, CombineMode.Exclude);
                        g.SetClip(new Rectangle(rm.X, rm.Y + Dpi.Height3, Dpi.Width8, Dpi.Height8), CombineMode.Exclude);
                        g.FillPath(fill, path);
                        g.DrawPath(pen, path);
                        g.ResetClip();
                        g.DrawRectangle(pen, inner);
                    }
                    using (GraphicsPath path = DisplayHelp.GetRoundedRectanglePath(new Rectangle(rm.X, rm.Y + Dpi.Height3, Dpi.Width8, Dpi.Height8), 1))
                    {
                        Rectangle inner = new Rectangle(rm.X + Dpi.Width2, rm.Y + Dpi.Height7, Dpi.Width4, Dpi.Width2);
                        g.SetClip(inner, CombineMode.Exclude);
                        g.FillPath(fill, path);
                        g.DrawPath(pen, path);
                        g.ResetClip();
                        g.DrawRectangle(pen, inner);
                    }
                }
            }
            if (oldClip != null)
            {
                g.Clip = oldClip;
                oldClip.Dispose();
            }
            //g.SmoothingMode = sm;
        }

        protected override void PaintMaximize(Graphics g, Rectangle r, Office2007SystemButtonStateColorTable ct, bool isEnabled, LinearGradientColorTable foregroundOverride)
        {
            Size s = new Size(Dpi.Width11, Dpi.Height9);
            Rectangle rm = GetSignRect(r, s);
            Region oldClip = g.Clip;

            LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
            using (Brush fill = DisplayHelp.CreateBrush(rm, foreground))
            {
                using (Pen pen = new Pen(ct.DarkShade))
                {
                    Rectangle inner = new Rectangle(rm.X + Dpi.Width3, rm.Y + Dpi.Height3, Dpi.Width(ct.DarkShade.IsEmpty ? 5 : 4), Dpi.Height(ct.DarkShade.IsEmpty ? 3 : 2));
                    g.SetClip(inner, CombineMode.Exclude);
                    DisplayHelp.DrawRoundedRectangle(g, pen, fill, rm.X, rm.Y, rm.Width, rm.Height, 1);
                    g.ResetClip();
                    g.DrawRectangle(pen, inner);
                }
            }

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

            Size s = new Size(Dpi.Width11, Dpi.Height9);
            Rectangle rm = GetSignRect(r, s);

            Rectangle r1 = rm;
            r1.Inflate(-1, 0);
            r1.Height--;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddLine(r1.X + Dpi.Width1, r1.Y, r1.X + Dpi.Width3, r1.Y);
                path.AddLine(r1.X + Dpi.Width3, r1.Y, r1.X + Dpi.Width5, r1.Y + Dpi.Width2);
                path.AddLine(r1.X + Dpi.Width5, r1.Y + Dpi.Width2, r1.X + Dpi.Width7, r1.Y);
                path.AddLine(r1.X + Dpi.Width7, r1.Y, r1.X + Dpi.Width9, r1.Y);
                path.AddLine(r1.X + Dpi.Width10, r1.Y + Dpi.Height1, r1.X + Dpi.Width7, r1.Y + Dpi.Height4);
                path.AddLine(r1.X + Dpi.Width7, r1.Y + Dpi.Height4, r1.X + Dpi.Width10, r1.Y + Dpi.Height7);
                path.AddLine(r1.X + Dpi.Width10, r1.Y + Dpi.Height7, r1.X + Dpi.Width9, r1.Y + Dpi.Height8);
                path.AddLine(r1.X + Dpi.Width9, r1.Y + Dpi.Height8, r1.X + Dpi.Width7, r1.Y + Dpi.Height8);
                path.AddLine(r1.X + Dpi.Width7, r1.Y + Dpi.Height8, r1.X + Dpi.Width5, r1.Y + Dpi.Height6);
                path.AddLine(r1.X + Dpi.Width5, r1.Y + Dpi.Height6, r1.X + Dpi.Width3, r1.Y + Dpi.Height8);
                path.AddLine(r1.X + Dpi.Width3, r1.Y + Dpi.Height8, r1.X + Dpi.Width1, r1.Y + Dpi.Height8);
                path.AddLine(r1.X, r1.Y + Dpi.Height7, r1.X + Dpi.Width3, r1.Y + Dpi.Height4);
                path.AddLine(r1.X + Dpi.Width3, r1.Y + Dpi.Height4, r1.X, r1.Y + Dpi.Height1);

                LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
                if (isEnabled)
                {
                    DisplayHelp.FillPath(g, path, foreground);
                    if (!ct.DarkShade.IsEmpty)
                    {
                        using (Pen pen = new Pen(ct.DarkShade))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                    else
                    {
                        using (Pen pen = new Pen(foreground.Start))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
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
                s.Width += 4;
                s.Height -= 2;
                Rectangle rm = GetSignRect(r, s);

                rm.Offset(1, 1);
                using (SolidBrush brush = new SolidBrush(ct.DarkShade))
                    g.DrawString("?", font, brush, rm);
                rm.Offset(-1, -1);
                LinearGradientColorTable foreground = foregroundOverride ?? ct.Foreground;
                using (SolidBrush brush = new SolidBrush(foreground.Start))
                    g.DrawString("?", font, brush, rm);

            }
            g.SmoothingMode = sm;
            g.TextRenderingHint = th;
        }
        
        #endregion
    }
}
