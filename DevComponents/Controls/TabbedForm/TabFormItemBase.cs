using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text;

namespace DevComponents.DotNetBar.Controls
{
    [ToolboxItem(false), DesignTimeVisible(false), Designer("DevComponents.DotNetBar.Design.TabFormItemDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf")]
    public class TabFormItemBase : ButtonItem
    {
        protected override void Dispose(bool disposing)
        {
            if (_TabPath != null)
            {
                _TabPath.Dispose();
                _TabPath = null;
            }
            base.Dispose(disposing);
        }

        private bool _RenderTabState = true;
        /// <summary>
        /// Gets or sets whether tab renders its state. Used internally by DotNetBar. Do not set.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool RenderTabState
        {
            get { return _RenderTabState; }
            set
            {
                _RenderTabState = value;
                if (this.ContainerControl is System.Windows.Forms.Control)
                    ((System.Windows.Forms.Control)this.ContainerControl).Invalidate();
                else
                    this.Refresh();
            }
        }

        private GraphicsPath _TabPath = null;
        /// <summary>
        /// Gets the actual tab path.
        /// </summary>
        [Browsable(false)]
        public GraphicsPath TabPath
        {
            get { return _TabPath; }
            internal set
            {
                if(_TabPath!=null)
                    _TabPath.Dispose();
                _TabPath = value;
            }
        }
    }
}
