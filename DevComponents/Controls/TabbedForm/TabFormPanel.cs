using System;
using System.Collections.Generic;
using System.Text;

namespace DevComponents.DotNetBar.Controls
{
    public class TabFormPanel : PanelControl
    {
        #region Internal Implementation

        private TabFormItem _TabFormItem;
        /// <summary>
        /// Gets the TabFormItem this panel is associated with
        /// </summary>
        public TabFormItem TabFormItem
        {
            get { return _TabFormItem; }
            internal set { _TabFormItem = value; }
        }
        #endregion
    }
}
