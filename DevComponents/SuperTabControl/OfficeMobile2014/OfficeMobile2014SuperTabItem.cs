 
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DevComponents.DotNetBar
{
    public class OfficeMobile2014SuperTabItem : SuperTabItemBaseDisplay
    {
        #region Constants

        private const int Radius = 8;
        private const int Dia = Radius * 2;
        private const int Offset = 2;
        private const int StripeTop = 7;
        private const int StripeBot = 5;
        private const int PadFill = 3;

        #endregion

        /// <summary>
        /// Constructor for OfficeMobile2014 style SuperTabItem base display
        /// </summary>
        /// <param name="tabItem">Associated SuperTabItem</param>
        public OfficeMobile2014SuperTabItem(SuperTabItem tabItem)
            : base(tabItem)
        {
        }

        #region ContentRectangle

        /// <summary>
        /// Calculates the Content Rectangle for the tab
        /// </summary>
        /// <returns>Content Rectangle</returns>
        internal override Rectangle ContentRectangle()
        {
            Rectangle r = TabItem.DisplayRectangle;

            int tabOverlap = (TabStripItem.TabDisplay.TabOverlap);
            int tabSpacing = (TabStripItem.TabDisplay.TabSpacing);
            int n = tabOverlap + tabSpacing;

            if (TabStripItem.IsVertical == true)
            {
                r.Y += (tabOverlap / 2 + tabSpacing);
                r.Height -= (tabOverlap + tabSpacing * 2);
            }
            else
            {
                r.X += (tabOverlap / 2 + tabSpacing);
                r.Width -= (tabOverlap + tabSpacing * 2);
            }

            int stripeTop = Dpi.Width(StripeTop);
            int padFill = Dpi.Width(PadFill);
            switch (TabItem.TabAlignment)
            {
                case eTabStripAlignment.Top:
                    r.Height -= stripeTop;
                    break;

                case eTabStripAlignment.Bottom:
                    r.Height -= StripeBot;
                    r.Y += StripeBot;
                    break;

                case eTabStripAlignment.Left:
                    if (TabStripItem.HorizontalText == false)
                    {
                        r.X -= padFill;
                        r.Y += padFill;
                    }

                    r.Width -= stripeTop;
                    break;

                default:
                    if (TabStripItem.HorizontalText == true)
                    {
                        r.X += padFill;
                        r.Width -= StripeBot;
                    }
                    else
                    {
                        r.X += stripeTop;
                        r.Width -= stripeTop;
                        r.Height += stripeTop;
                    }
                    break;
            }

            return (r);
        }

        #endregion

        #region TabItemPath

        /// <summary>
        /// Creates the tab item GraphicsPath
        /// </summary>
        /// <returns>Tab path</returns>
        internal override GraphicsPath TabItemPath()
        {
            GraphicsPath path = base.TabItemPath();

            if (path != null)
                return (path);

            switch (TabItem.TabAlignment)
            {
                case eTabStripAlignment.Top:
                    return (TopTabPath());

                case eTabStripAlignment.Bottom:
                    return (BottomTabPath());

                case eTabStripAlignment.Left:
                    return (LeftTabPath());

                default:
                    return (RightTabPath());
            }
        }

        #region TopTabPath

        /// <summary>
        /// Create the Top tab path
        /// </summary>
        /// <returns>GraphicsPath</returns>
        private GraphicsPath TopTabPath()
        {
            Rectangle r = TabItem.DisplayRectangle;
            r.Width -= Dpi.Width1;
            r.Height -= Dpi.Height(StripeBot);

            int k = Math.Min(20, r.Height);
            float scale = (float)k / 20;

            int delta = (int)(90 * scale) - 25;
            int n = (int)((TabStripItem.TabDisplay.TabOverlap / 2) * scale);

            // Create the path

            GraphicsPath path = new GraphicsPath();

            int dia = Dpi.Width(Dia);
            Rectangle ar = new Rectangle(r.X - Dpi.Width(Radius), r.Bottom - dia, dia, dia);
            path.AddArc(ar, 90, -delta);

            int offset = Dpi.Width(Offset);
            ar = new Rectangle(r.X + Dpi.Width(Radius) + Dpi.Width(offset), r.Y, dia, dia);
            path.AddArc(ar, 270 - delta, delta);

            ar = new Rectangle(r.Right - (dia + offset + n), r.Y, dia, dia);
            path.AddArc(ar, 270, delta);

            ar = new Rectangle(r.Right - (dia - offset * 2), r.Bottom - dia, dia, dia);
            path.AddArc(ar, 90 + delta, -delta);

            return (path);
        }

        #endregion

        #region BottomTabPath

        /// <summary>
        /// Creates the Bottom tab path
        /// </summary>
        /// <returns>GraphicsPath</returns>
        private GraphicsPath BottomTabPath()
        {
            Rectangle r = TabItem.DisplayRectangle;
            r.Width -= 1;
            r.Height -= (StripeBot + 1);
            r.Y += StripeBot;

            // Create the path

            GraphicsPath path = new GraphicsPath();

            int k = Math.Min(20, r.Height);
            float scale = (float)k / 20;

            int delta = (int)(90 * scale) - 25;
            int n = (int)((TabStripItem.TabDisplay.TabOverlap / 2) * scale);

            Rectangle ar = new Rectangle(r.X - Radius, r.Y, Dia, Dia);
            path.AddArc(ar, 270, delta);

            ar = new Rectangle(r.X + Radius + Offset, r.Bottom - Dia, Dia, Dia);
            path.AddArc(ar, 90 + delta, -delta);

            ar = new Rectangle(r.Right - (Dia + Offset + n), r.Bottom - Dia, Dia, Dia);
            path.AddArc(ar, 90, -delta);

            ar = new Rectangle(r.Right - (Dia - Offset * 2), r.Y, Dia, Dia);
            path.AddArc(ar, 270 - delta, delta);

            return (path);
        }

        #endregion

        #region LeftTabPath

        /// <summary>
        /// Creates the Left tab path
        /// </summary>
        /// <returns>GraphicsPath</returns>
        private GraphicsPath LeftTabPath()
        {
            Rectangle r = TabItem.DisplayRectangle;
            r.Width -= StripeBot;
            r.Height -= 1;

            // Create the path

            GraphicsPath path = new GraphicsPath();

            int k = Math.Min(20, r.Width);
            float scale = (float)k / 20;

            int delta = (int)(90 * scale) - 25;
            int n = (int)((TabStripItem.TabDisplay.TabOverlap / 2) * scale);

            Rectangle ar = new Rectangle(r.Right - Dia, r.Y - Radius, Dia, Dia);
            path.AddArc(ar, 0, delta);

            ar = new Rectangle(r.X, r.Y + Offset + n, Dia, Dia);
            path.AddArc(ar, -180 + delta, -delta);

            ar = new Rectangle(r.X, r.Bottom - Dia - Offset, Dia, Dia);
            path.AddArc(ar, 180, -delta);

            ar = new Rectangle(r.Right - Dia, r.Bottom - Radius + n, Dia, Dia);
            path.AddArc(ar, -delta, delta);

            return (path);
        }

        #endregion

        #region RightTabPath

        /// <summary>
        /// Create the Right tab path
        /// </summary>
        /// <returns>GraphicsPath</returns>
        private GraphicsPath RightTabPath()
        {
            Rectangle r = TabItem.DisplayRectangle;
            r.X += StripeBot;
            r.Width -= (StripeBot + 1);
            r.Height -= 1;

            // Create the path

            GraphicsPath path = new GraphicsPath();

            int k = Math.Min(20, r.Width);
            float scale = (float)k / 20;

            int delta = (int)(90 * scale) - 25;
            int n = (int)((TabStripItem.TabDisplay.TabOverlap / 2) * scale);

            Rectangle ar = new Rectangle(r.X, r.Y - Radius, Dia, Dia);
            path.AddArc(ar, 180, -delta);

            ar = new Rectangle(r.Right - Dia, r.Y + Offset + n, Dia, Dia);
            path.AddArc(ar, -delta, delta);

            ar = new Rectangle(r.Right - Dia, r.Bottom - Dia - Offset, Dia, Dia);
            path.AddArc(ar, 0, delta);

            ar = new Rectangle(r.X, r.Bottom - Radius + n, Dia, Dia);
            path.AddArc(ar, 180 + delta, -delta);

            return (path);
        }

        #endregion

        #endregion
    }
}