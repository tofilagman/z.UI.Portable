using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar.Metro;
using DevComponents.DotNetBar.Metro.Rendering;

namespace DevComponents.DotNetBar.Rendering
{
    internal abstract class TabFormPainter
    {
        public abstract void Paint(TabFormPainterArgs args);
    }

    internal class OfficeTabFormPainter : TabFormPainter, IOffice2007Painter
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

        public override void Paint(TabFormPainterArgs args)
        {
            TabParentForm form = args.TabParentForm;
            Graphics g = args.Graphics;
            TabFormColorTable colorTable = GetFormColorTable();
            if (form.FormTabsControl != null && form.FormTabsControl.ColorTable != null)
                colorTable = form.FormTabsControl.ColorTable;

            Thickness borderThickness = form.BorderThickness;
            BorderColors colors = form.BorderColor;
            if (borderThickness.IsZero && colors.IsEmpty)
            {
                // Get it from table
                borderThickness = colorTable.BorderThickness;
                colors = form.IsActive ? colorTable.Active.BorderColors : colorTable.Inactive.BorderColors;
            }

            using (SolidBrush brush = new SolidBrush(colorTable.BackColor.IsEmpty ? form.BackColor : colorTable.BackColor))
                g.FillRectangle(brush, new Rectangle(0, 0, form.Width, form.Height));

            if (!borderThickness.IsZero && !colors.IsEmpty)
            {
                RectangleF br = new RectangleF(0, 0, form.Width, form.Height);
                DrawingHelpers.DrawBorder(g, br, borderThickness, colors);
            }
        }

        private TabFormColorTable GetFormColorTable()
        {
            return _ColorTable.TabForm;
        }
    }

    public class TabFormPainterArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets Graphics object group is rendered on.
        /// </summary>
        public Graphics Graphics = null;

        /// <summary>
        /// Gets or sets the reference to TabParentForm being rendered.
        /// </summary>
        public TabParentForm TabParentForm = null;


        /// <summary>
        /// Indicates whether to cancel system rendering of the item.
        /// </summary>
        public bool Cancel = false;

        public TabFormPainterArgs(TabParentForm form, Graphics graphics)
        {
            Graphics = graphics;
            TabParentForm = form;
        }
    }
}
