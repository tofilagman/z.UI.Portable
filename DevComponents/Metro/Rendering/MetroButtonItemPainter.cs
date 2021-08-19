using DevComponents.DotNetBar.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevComponents.DotNetBar.Metro.Rendering
{
    internal class MetroButtonItemPainter : Office2007ButtonItemPainter
    {
        private static RoundRectangleShapeDescriptor _DefaultMetroShape = new RoundRectangleShapeDescriptor(0);
        private static RoundRectangleShapeDescriptor _DefaultMobileShape = new RoundRectangleShapeDescriptor(2);
        protected override IShapeDescriptor GetButtonShape(ButtonItem button, ItemPaintArgs pa)
        {
            IShapeDescriptor shape = MetroButtonItemPainter.GetButtonShape(button);
            
            if (pa.ContainerControl is ButtonX)
                shape = ((ButtonX)pa.ContainerControl).GetButtonShape();
            else if (pa.ContainerControl is NavigationBar)
                shape = ((NavigationBar)pa.ContainerControl).ButtonShape;
            return shape;
        }
        private static IShapeDescriptor GetButtonShape(ButtonItem button)
        {
            if (button.Shape != null)
                return button.Shape;
            else if(StyleManager.Style == eStyle.OfficeMobile2014)
                return _DefaultMobileShape;
            return _DefaultMetroShape;
        }

        public override DotNetBar.Rendering.Office2007ButtonItemColorTable GetColorTable(ButtonItem button, eButtonContainer buttonCont)
        {
            if (buttonCont == eButtonContainer.MetroTabStrip)
            {
                Office2007ColorTable colorTable = this.ColorTable;
                object st = null;
                if (colorTable.ContextualTables.TryGetValue(Office2007ColorTable.GetContextualKey(ButtonColorTableType, "MetroTabStrip"), out st))
                    return (Office2007ButtonItemColorTable)st;
            }
            else if (buttonCont == eButtonContainer.NavigationPane)
            {
                Office2007ColorTable colorTable = this.ColorTable;
                object st = null;
                if (colorTable.ContextualTables.TryGetValue(Office2007ColorTable.GetContextualKey(ButtonColorTableType, "NavigationBar"), out st))
                    return (Office2007ButtonItemColorTable)st;
            }

            return base.GetColorTable(button, buttonCont);
        }
    }
}
