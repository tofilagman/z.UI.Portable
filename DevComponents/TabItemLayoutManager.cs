using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using DevComponents.UI.ContentManager;

namespace DevComponents.DotNetBar
{
	/// <summary>
	/// Summary description for TabItemLayoutManager.
	/// </summary>
	public class TabItemLayoutManager:DevComponents.UI.ContentManager.BlockLayoutManager
	{
		private int m_TextPadding=4;
		private int m_ImagePadding=2;
		private int m_PaddingHeight=4;
		private int m_PaddingWidth=0;
		private bool m_HorizontalText=false;
		private int m_SelectedPaddingWidth=0;
        private Size m_FixedTabSize = Size.Empty;
        private bool m_CloseButtonOnTabs = false;
        private Size m_CloseButtonSize = new Size(11, 11);
        private int m_CloseButtonSpacing = 3;
        private TabStrip m_TabStrip = null;

		public TabItemLayoutManager()
		{
		}

        public TabItemLayoutManager(TabStrip tabStrip)
        {
            m_TabStrip = tabStrip;
        }

		/// <summary>
		/// Resizes the content block and sets it's Bounds property to reflect new size.
		/// </summary>
		/// <param name="block">Content block to resize.</param>
        public override void Layout(IBlock block, Size availableSize)
		{
			if(this.Graphics==null)
				throw(new InvalidOperationException("Graphics property must be set to valid instance of Graphics object."));

			TabItem tab=block as TabItem;
			if(!tab.Visible)
				return;

			int width=0;
			int height=0;
            bool isVertical = (tab.Parent != null && (tab.Parent.TabAlignment == eTabStripAlignment.Left || tab.Parent.TabAlignment == eTabStripAlignment.Right) && !m_HorizontalText);

			eTabStripStyle style=tab.Parent.Style;
			eTextFormat strFormat=eTextFormat.Default;

			Image tabImage=tab.GetImage();
			if(tab.Icon!=null)
			{
				width+=tab.IconSize.Width;
                if (style != eTabStripStyle.OneNote && style != eTabStripStyle.Office2007Document)
					width+=m_ImagePadding;
				height=tab.IconSize.Height+m_PaddingHeight;
			}
			else if(tabImage!=null)
			{
				width+=Dpi.Width(tabImage.Width);
                if (style != eTabStripStyle.OneNote && style != eTabStripStyle.Office2007Document)
					width+=Dpi.Width(m_ImagePadding);
				height=Dpi.Height(tabImage.Height)+Dpi.Height(m_PaddingHeight);
			}

			if((!tab.Parent.DisplaySelectedTextOnly || tab==tab.Parent.SelectedTab))
			{
                string text = tab.Text;
                if (text == "")
                    text = "M";
				Font font=tab.Parent.Font;
                if (tab.Parent.SelectedTabFont != null && (tab == tab.Parent.SelectedTab || 
                    tab.Parent.CloseButtonOnTabsAlwaysDisplayed && tab.Parent.CloseButtonPosition == eTabCloseButtonPosition.Right || 
                    ((tab.Parent.TabAlignment == eTabStripAlignment.Left || tab.Parent.TabAlignment == eTabStripAlignment.Right) && m_HorizontalText)))
					font=tab.Parent.SelectedTabFont;
                Size textSize = Size.Empty;
                if(isVertical)
                    textSize = TextDrawing.MeasureStringLegacy(this.Graphics, text, font, Size.Empty, strFormat);
                else
				    textSize=TextDrawing.MeasureString(this.Graphics,text,font,0,strFormat);
				width+=textSize.Width;
				if(style!=eTabStripStyle.OneNote)
					width+=Dpi.Width(m_TextPadding);
				if(textSize.Height>height)
					height=(int)(textSize.Height+Dpi.Height(m_PaddingHeight));
			}

            if (m_CloseButtonOnTabs && tab.CloseButtonVisible)
                width += m_CloseButtonSize.Width + m_CloseButtonSpacing * 2;

			width+=m_PaddingWidth;
			if(tab.IsSelected)
				width+=m_SelectedPaddingWidth;

            if (m_FixedTabSize.Width > 0)
                width = m_FixedTabSize.Width;
            if (m_FixedTabSize.Height > 0)
                height = m_FixedTabSize.Height;

            if (style == eTabStripStyle.OneNote || style == eTabStripStyle.Office2007Document)
                width += (int)(height * .5)-4;

            Rectangle bounds = new Rectangle(0, 0, width, height);

            if (isVertical)
                bounds = new Rectangle(0, 0, height, width);

            if (m_TabStrip != null && m_TabStrip.HasMeasureTabItem)
            {
                MeasureTabItemEventArgs mea = new MeasureTabItemEventArgs(tab, bounds.Size);
                m_TabStrip.InvokeMeasureTabItem(mea);
                bounds.Size = mea.Size;
            }

            block.Bounds = bounds;
		}

        public override Rectangle FinalizeLayout(Rectangle containerBounds, Rectangle blocksBounds, ArrayList lines)
        {
            if (lines.Count > 1 && m_TabStrip.MultiLinePanelAlignSelectedTab)
            {
                // Make sure that selected tab is always at the bottom i.e. next to the tab panel
                int selectedTabLine = GetSelectedTabLine(lines);
                if (selectedTabLine >= 0)
                {
                    if (m_TabStrip.TabAlignment == eTabStripAlignment.Bottom && selectedTabLine > 0)
                        SwitchTabLines(lines, selectedTabLine, 0);
                    else if(m_TabStrip.TabAlignment== eTabStripAlignment.Top && selectedTabLine<lines.Count-1)
                        SwitchTabLines(lines, selectedTabLine, lines.Count - 1);
                    else if (m_TabStrip.TabAlignment == eTabStripAlignment.Left && selectedTabLine < lines.Count - 1)
                        SwitchTabLines(lines, selectedTabLine, lines.Count - 1);
                    else if (m_TabStrip.TabAlignment == eTabStripAlignment.Right && selectedTabLine > 0)
                        SwitchTabLines(lines, selectedTabLine, 0);
                }
            }

            return (blocksBounds);
        }

        private void SwitchTabLines(ArrayList lines, int fromLine, int toLine)
        {
            SerialContentLayoutManager.BlockLineInfo lineFrom = (SerialContentLayoutManager.BlockLineInfo)lines[fromLine];
            SerialContentLayoutManager.BlockLineInfo lineTo = (SerialContentLayoutManager.BlockLineInfo)lines[toLine];
            bool horizontalTabs = (m_TabStrip.TabAlignment == eTabStripAlignment.Top ||
                                   m_TabStrip.TabAlignment == eTabStripAlignment.Bottom);
            int fromPos = horizontalTabs ? ((IBlock) lineFrom.Blocks[0]).Bounds.Y
                : ((IBlock) lineFrom.Blocks[0]).Bounds.X;
            int toPos = horizontalTabs ? ((IBlock)lineTo.Blocks[0]).Bounds.Y
                : ((IBlock)lineTo.Blocks[0]).Bounds.X;
            lines[fromLine] = lineTo;
            lines[toLine] = lineFrom;
            foreach (IBlock item in lineTo.Blocks)
            {
                if (horizontalTabs)
                    item.Bounds = new Rectangle(item.Bounds.X, fromPos, item.Bounds.Width, item.Bounds.Height);
                else
                    item.Bounds = new Rectangle(fromPos, item.Bounds.Y, item.Bounds.Width, item.Bounds.Height);
            }
            foreach (IBlock item in lineFrom.Blocks)
            {
                if (horizontalTabs)
                    item.Bounds = new Rectangle(item.Bounds.X, toPos, item.Bounds.Width, item.Bounds.Height);
                else
                    item.Bounds = new Rectangle(toPos, item.Bounds.Y, item.Bounds.Width, item.Bounds.Height);
            }

        }

        private int GetSelectedTabLine(ArrayList lines)
        {
            foreach (SerialContentLayoutManager.BlockLineInfo line in lines)
            {
                foreach (TabItem item in line.Blocks)
                {
                    if (item.IsSelected)
                        return line.Line;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets or sets the padding in pixels for the measured text. Default is 4.
        /// </summary>
        public int TextPadding
		{
			get {return m_TextPadding;}
			set {m_TextPadding=value;}
		}

		/// <summary>
		/// Gets or sets the padding in pixels for the measured image size. Default is 4.
		/// </summary>
		public int ImagePadding
		{
			get {return m_ImagePadding;}
			set {m_ImagePadding=value;}
		}

		/// <summary>
		/// Gets or sets the padding in pixels that is added to the measured height of the tab. Default is 4.
		/// </summary>
		public int PaddingHeight
		{
			get {return m_PaddingHeight;}
			set {m_PaddingHeight=value;}
		}

		/// <summary>
		/// Gets or sets the padding in pixels that is added to the measured width of the tab. Default is 0.
		/// </summary>
		public int PaddingWidth
		{
			get {return m_PaddingWidth;}
			set {m_PaddingWidth=value;}
		}

		/// <summary>
		/// Gets or sets whether text is always layed out horizontaly even if tabs are vertically aligned.
		/// </summary>
		public bool HorizontalText
		{
			get {return m_HorizontalText;}
			set {m_HorizontalText=value;}
		}

		/// <summary>
		/// Gets or sets the additional padding for the selected item.
		/// </summary>
		public int SelectedPaddingWidth
		{
			get {return m_SelectedPaddingWidth;}
			set {m_SelectedPaddingWidth=value;}
		}

        /// <summary>
        /// Gets or sets the fixed tab size in pixels. Either member can be set. Value of 0 indicates that size is automatically calculated which is
        /// default behavior.
        /// </summary>
        public Size FixedTabSize
        {
            get { return m_FixedTabSize; }
            set { m_FixedTabSize = value; }
        }

        public bool CloseButtonOnTabs
        {
            get { return m_CloseButtonOnTabs; }
            set { m_CloseButtonOnTabs = value; }
        }

        public Size CloseButtonSize
        {
            get { return m_CloseButtonSize; }
            set { m_CloseButtonSize = value; }
        }
	}
}
