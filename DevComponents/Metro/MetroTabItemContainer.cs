using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DevComponents.DotNetBar.Metro
{
    internal class MetroTabItemContainer : SimpleItemContainer
    {
        //protected override bool OnBeforeLayout()
        //{
        //    if (this.Orientation != eOrientation.Horizontal)
        //        return true;

        //    ArrayList ribbonTabItems = new ArrayList();
        //    int totalWidth = 0;
        //    int totalRibbonTabItemsWidth = 0;
        //    int minimumSize = 24;
        //    int availableWidth = this.WidthInternal - (this.PaddingLeft + this.PaddingRight);

        //    foreach (BaseItem item in this.SubItems)
        //    {
        //        if (!item.Visible)
        //            continue;
        //        item.RecalcSize();
        //        totalWidth += (item.WidthInternal + this.ItemSpacing);
        //        if (item is RibbonTabItem)
        //        {
        //            // Reset reduced size flag
        //            ((RibbonTabItem)item).ReducedSize = false;
        //            ribbonTabItems.Add(item);
        //            totalRibbonTabItemsWidth += (item.WidthInternal + this.ItemSpacing);
        //        }
        //    }

        //    int totalReduction = totalWidth - availableWidth;

        //    if (totalWidth > availableWidth && totalRibbonTabItemsWidth > 0)
        //    {
        //        if (totalReduction >= totalRibbonTabItemsWidth - (minimumSize * ribbonTabItems.Count + ribbonTabItems.Count - 1))
        //        {
        //            foreach (RibbonTabItem item in ribbonTabItems)
        //            {
        //                item.WidthInternal = minimumSize;
        //                item.ReducedSize = true;
        //            }
        //        }
        //        else
        //        {
        //            float reduction = 1 - (float)totalReduction / (float)totalRibbonTabItemsWidth;
        //            bool reducedSize = false;
        //            if (reduction <= .75)
        //                reducedSize = true;
        //            for (int i = 0; i < ribbonTabItems.Count; i++)
        //            {
        //                RibbonTabItem item = ribbonTabItems[i] as RibbonTabItem;
        //                item.ReducedSize = reducedSize;
        //                if (i == ribbonTabItems.Count - 1)
        //                {
        //                    item.WidthInternal -= totalReduction;
        //                }
        //                else
        //                {
        //                    int width = (int)(item.WidthInternal * reduction);
        //                    if (width < minimumSize)
        //                        width = minimumSize;
        //                    totalReduction -= (item.WidthInternal - width);
        //                    item.WidthInternal = width;
        //                }
        //            }
        //        }
        //    }

        //    return false;
        //}
    }
}
