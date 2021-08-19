using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace DevComponents.DotNetBar.Controls
{
    [ToolboxItem(false), DesignTimeVisible(false), DefaultEvent("Click"), Designer("DevComponents.DotNetBar.Design.ToolboxItemDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf")]
    public class ToolboxItem : ButtonItem
    {
        #region Constructor
        public ToolboxItem()
        {
            //this.GlobalItem = false;
        }
        public ToolboxItem(string text)
        {
            this.Text = text;
            this.ButtonStyle = eButtonStyle.ImageAndText;
        }
        public ToolboxItem(string text, Image image)
        {
            this.Text = text;
            this.Image = image;
            this.ButtonStyle = eButtonStyle.ImageAndText;
        }

        public ToolboxItem(string text, string symbol, eSymbolSet symbolSet)
        {
            this.Text = text;
            this.SymbolSet = symbolSet;
            this.Symbol = symbol;
            this.ButtonStyle = eButtonStyle.ImageAndText;
        }

        /// <summary>
        /// Returns copy of the item.
        /// </summary>
        public override BaseItem Copy()
        {
            ToolboxItem objCopy = new ToolboxItem(this.Name);
            this.CopyToItem(objCopy);
            return objCopy;
        }

        /// <summary>
        /// Copies the ButtonItem specific properties to new instance of the item.
        /// </summary>
        /// <param name="copy">New ButtonItem instance.</param>
        internal void InternalCopyToItem(ToolboxItem copy)
        {
            CopyToItem((BaseItem)copy);
        }

        protected override void OnCheckedChanged()
        {
            base.OnCheckedChanged();
            ToolboxControl tc = GetToolboxControl();
            if (tc != null)
                tc.ToolboxItemCheckedChanged(this);
        }

        /// <summary>
        /// Returns reference to parent ToolboxControl, if group is parented to it.
        /// </summary>
        /// <returns>reference to ToolboxControl or null</returns>
        public ToolboxControl GetToolboxControl()
        {
            ItemPanel c = this.ContainerControl as ItemPanel;
            if (c != null)
                return c.Parent as ToolboxControl;
            return null;
        }

        #endregion

        #region Implementation
        protected override bool ShouldDrawInsertMarker()
        {
            return DesignInsertMarker != eDesignInsertPosition.None && this.Visible && this.Displayed && !this.DesignMode;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

        #endregion

        }
}
