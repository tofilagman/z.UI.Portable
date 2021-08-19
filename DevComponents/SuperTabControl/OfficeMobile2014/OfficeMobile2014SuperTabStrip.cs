 
using System.Drawing;
using System.Drawing.Drawing2D;
using DevComponents.DotNetBar.Rendering;
using DevComponents.UI.ContentManager;

namespace DevComponents.DotNetBar
{
    internal class OfficeMobile2014SuperTabStrip : SuperTabStripBaseDisplay
    {
        #region Constants

        private int HTabSpacing
        {
            get { return Dpi.Width4; }
        }

        private int HTabOverLap
        {
            get { return Dpi.Width25; }
        }

        private int VTabSpacing {
            get { return Dpi.Width4; }
        }

        private int VTabOverLap {
            get { return Dpi.Width14; }
        }

        #endregion

        /// <summary>
        /// OfficeMobile2014 SuperTabStripBaseDisplay
        /// </summary>
        /// <param name="tabStripItem">Associated TabStripItem</param>
        public OfficeMobile2014SuperTabStrip(SuperTabStripItem tabStripItem)
            : base(tabStripItem)
        {
        }

        #region Internal properties

        #region MinTabSize

        /// <summary>
        /// Returns the Minimum tab size for this style
        /// </summary>
        internal override Size MinTabSize
        {
            get
            {
                if (TabStripItem.IsVertical == false || TabStripItem.HorizontalText == true)
                    return Dpi.Size(new Size(52, 20));
                    
                return Dpi.Size(new Size(36, 20));
            }
        }

        #endregion

        #region TabLayoutOffset

        /// <summary>
        /// Tab layout offsets
        /// </summary>
        internal override Size TabLayoutOffset
        {
            get { return Dpi.Size(new Size(5, 3)); }
        }

        #endregion

        #region TabOverlap

        /// <summary>
        /// Tab Overlap
        /// </summary>
        internal override int TabOverlap
        {
            get { return (TabStripItem.IsVertical ? VTabOverLap : HTabOverLap); }
        }

        #endregion

        #region TabSpacing

        /// <summary>
        /// Tab Spacing
        /// </summary>
        internal override int TabSpacing
        {
            get
            {
                switch (TabStripItem.TabAlignment)
                {
                    case eTabStripAlignment.Top:
                    case eTabStripAlignment.Bottom:
                        return HTabSpacing;

                    case eTabStripAlignment.Right:
                        return TabStripItem.HorizontalText == false ? HTabSpacing : VTabSpacing;

                    default:
                        return VTabSpacing;
                }
            }
        }

        #endregion

        #endregion

        #region NextBlockPosition

        /// <summary>
        /// Gets the next layout block position
        /// </summary>
        /// <param name="e">LayoutManagerPositionEventArgs</param>
        protected override void NextBlockPosition(LayoutManagerPositionEventArgs e)
        {
            int n = Tabs.IndexOf((BaseItem)e.Block);

            if (n >= 0 && n + 1 < Tabs.Count)
            {
                SuperTabItem tab1 = Tabs[n] as SuperTabItem;

                if (tab1 != null)
                {
                    SuperTabItem tab2 = Tabs[n + 1] as SuperTabItem;

                    if (tab2 != null)
                    {
                        e.NextPosition = e.CurrentPosition;

                        if (TabStripItem.IsVertical == true)
                            e.NextPosition.Y += (e.Block.Bounds.Height - VTabOverLap);
                        else
                            e.NextPosition.X += (e.Block.Bounds.Width - HTabOverLap);

                        e.Cancel = true;
                    }
                }
            }
        }

        #endregion

        #region NextBlockPosition

        /// <summary>
        /// Gets the next block position when attempting
        /// to make a specific tab visible
        /// </summary>
        /// <param name="item">Potential item to replace</param>
        /// <param name="vItem">View item being placed</param>
        /// <returns>Block Rectangle</returns>
        internal override Rectangle NextBlockPosition(BaseItem item, BaseItem vItem)
        {
            Rectangle r = base.NextBlockPosition(item, vItem);

            if (item is SuperTabItem && vItem is SuperTabItem)
            {
                if (TabStripItem.IsVertical == true)
                    r.Y -= VTabOverLap;
                else
                    r.X -= HTabOverLap;
            }

            return (r);
        }

        #endregion

        #region AddDefaultPadding

        internal override void AddDefaultPadding(ref Size size)
        {
            base.AddDefaultPadding(ref size);

            if (TabStripItem.IsVertical == false || TabStripItem.HorizontalText == false)
                size.Height += Dpi.Height7;
        }

        #endregion

        #region DrawStripBorder

        protected override void DrawStripBorder(ItemPaintArgs p, Rendering.SuperTabColorTable ct)
        {
            Graphics g = p.Graphics;

            Rectangle t = TabStripItem.SelectedTab != null ?
                TabStripItem.SelectedTab.DisplayRectangle : Rectangle.Empty;

            Rectangle r = GetStripeRect();

            eTabState ts = GetTabState();
            SuperTabItemStateColorTable tct = TabStripItem.SelectedTab.GetTabColorTable(ts);

            if (tct.Background.Colors != null)
            {
                using (Brush br = new SolidBrush(tct.Background.Colors[tct.Background.Colors.Length - 1]))
                {
                    SmoothingMode mode = g.SmoothingMode;
                    g.SmoothingMode = SmoothingMode.None;

                    g.FillRectangle(br, r);

                    g.SmoothingMode = mode;
                }
            }

            if (ct.InnerBorder.IsEmpty == false)
                RenderInnerBorder(g, ct, r, t);

            if (ct.OuterBorder.IsEmpty == false)
                RenderOuterBorder(g, ct, r, t);
        }

        #region RenderInnerBorder

        private void RenderInnerBorder(Graphics g, 
            Rendering.SuperTabColorTable ct, Rectangle r, Rectangle t)
        {
            using (Pen pen = new Pen(ct.InnerBorder))
            {
                switch (TabStripItem.TabAlignment)
                {
                    case eTabStripAlignment.Top:
                        if (t.X > r.X)
                            g.DrawLine(pen, r.X, r.Y + Dpi.Height1, t.X + Dpi.Width4, r.Y + Dpi.Height1);

                        if (t.Right < r.Right)
                            g.DrawLine(pen, t.Right - Dpi.Width4, r.Y + Dpi.Height1, r.Right - Dpi.Width1, r.Y + Dpi.Height1);

                        g.DrawLine(pen, r.X + Dpi.Width1, r.Y + Dpi.Height1, r.X + Dpi.Width1, r.Y + Dpi.Height7);
                        g.DrawLine(pen, r.Right - Dpi.Width2, r.Y + Dpi.Height1, r.Right - Dpi.Width2, r.Y + Dpi.Height7);
                        break;

                    case eTabStripAlignment.Bottom:
                        if (t.X > r.X)
                            g.DrawLine(pen, r.X, r.Bottom - 1, t.X + 3, r.Bottom - 1);

                        if (t.Right < r.Right)
                            g.DrawLine(pen, t.Right - 3, r.Bottom - 1, r.Right - 1, r.Bottom - 1);

                        g.DrawLine(pen, r.X + 1, r.Bottom - 2, r.X + 1, r.Bottom - 7);
                        g.DrawLine(pen, r.Right - 2, r.Bottom - 2, r.Right - 2, r.Bottom - 7);
                        break;

                    case eTabStripAlignment.Left:
                        if (t.Y > 0)
                            g.DrawLine(pen, r.X + 1, r.Y, r.X + 1, t.Y + 3);

                        if (t.Bottom < r.Bottom)
                            g.DrawLine(pen, r.X + 1, t.Bottom + 3, r.X + 1, r.Bottom - 3);

                        g.DrawLine(pen, r.X, r.Y + 1, r.X + 7, r.Y + 1);
                        g.DrawLine(pen, r.X, r.Bottom - 2, r.X + 7, r.Bottom - 2);
                        break;

                    case eTabStripAlignment.Right:
                        if (t.Y > 0)
                            g.DrawLine(pen, r.Right - 1, r.Y, r.Right - 1, t.Y + 3);

                        if (t.Bottom < r.Bottom)
                            g.DrawLine(pen, r.Right - 1, t.Bottom + 2, r.Right - 1, r.Bottom - 1);

                        g.DrawLine(pen, r.X, r.Y + 1, r.Right, r.Y + 1);
                        g.DrawLine(pen, r.X, r.Bottom - 2, r.Right, r.Bottom - 2);
                        break;
                }
            }
        }

        #endregion

        #region RenderOuterBorder

        private void RenderOuterBorder(Graphics g, SuperTabColorTable ct, Rectangle r, Rectangle t)
        {
            using (Pen pen = new Pen(ct.OuterBorder))
            {
                switch (TabStripItem.TabAlignment)
                {
                    case eTabStripAlignment.Top:
                        if (t.X > r.X)
                            g.DrawLine(pen, r.X, r.Y, t.X + 4, r.Y);

                        if (t.Right < r.Right)
                            g.DrawLine(pen, t.Right - 4, r.Y, r.Right - 2, r.Y);

                        g.DrawLine(pen, r.X, r.Y, r.X, r.Y + 7);
                        g.DrawLine(pen, r.Right - 1, r.Y, r.Right - 1, r.Y + 7);
                        break;

                    case eTabStripAlignment.Bottom:
                        if (t.X > r.X)
                            g.DrawLine(pen, r.X, r.Bottom, t.X, r.Bottom);

                        if (t.Right < r.Right)
                            g.DrawLine(pen, t.Right, r.Bottom, r.Right - 2, r.Bottom);

                        g.DrawLine(pen, r.X, r.Bottom, r.X, r.Bottom - 7);
                        g.DrawLine(pen, r.Right - 1, r.Bottom, r.Right - 1, r.Bottom - 7);
                        break;

                    case eTabStripAlignment.Left:
                        if (t.Y > 0)
                            g.DrawLine(pen, r.X, r.Y, r.X, t.Y + 3);

                        if (t.Bottom < r.Bottom)
                            g.DrawLine(pen, r.X, t.Bottom + 2, r.X, r.Bottom - 3);

                        g.DrawLine(pen, r.X, r.Y, r.X + 7, r.Y);
                        g.DrawLine(pen, r.X, r.Bottom - 1, r.X + 7, r.Bottom - 1);
                        break;

                    case eTabStripAlignment.Right:
                        if (t.Y > 0)
                            g.DrawLine(pen, r.Right, r.Y, r.Right, t.Y);

                        if (t.Bottom < r.Bottom)
                            g.DrawLine(pen, r.Right, t.Bottom + 3, r.Right, r.Bottom - 1);

                        g.DrawLine(pen, r.X, r.Y, r.Right, r.Y);
                        g.DrawLine(pen, r.X, r.Bottom - 1, r.Right, r.Bottom - 1);
                        break;
                }
            }
        }

        #endregion

        #region GetStripeRect

        private Rectangle GetStripeRect()
        {
            Rectangle r = TabStripItem.Bounds;

            switch (TabStripItem.TabAlignment)
            {
                case eTabStripAlignment.Top:
                    r.Y = r.Bottom - Dpi.Height7 - Dpi.Height1;
                    break;

                case eTabStripAlignment.Bottom:
                    r.Y -= Dpi.Height1;
                    r.Height = Dpi.Height7 + Dpi.Height1;
                    break;

                case eTabStripAlignment.Left:
                    r.X = r.Right - Dpi.Width7 - Dpi.Width1;
                    break;

                default:
                    r.X -= Dpi.Width1;
                    r.Width = Dpi.Width7 + Dpi.Width1;
                    break;
            }

            return (r);
        }

        #endregion

        #region GetTabState

        private eTabState GetTabState()
        {
            SuperTabItem tab = TabStripItem.SelectedTab;

            if (tab.Enabled == false)
                return (eTabState.Disabled);

            return (tab.IsSelected ? eTabState.Selected : eTabState.Default);
        }

        #endregion

        #endregion

    }
} 