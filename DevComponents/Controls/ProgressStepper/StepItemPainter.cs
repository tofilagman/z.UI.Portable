using System;
using System.Collections.Generic;
using System.Text;

namespace DevComponents.DotNetBar.Rendering
{
    internal class StepItemPainter: IOffice2007Painter
    {
        /// <summary>
        /// Paints StepItem.
        /// </summary>
        /// <param name="e">Provides arguments for the operation.</param>
        public virtual void Paint(StepItemRendererEventArgs e) { }

        #region IOffice2007Painter Members
        private Office2007ColorTable _ColorTable = null; //new Office2007ColorTable();
        public Office2007ColorTable ColorTable
        {
            get
            {
                return _ColorTable;
            }
            set
            {
                _ColorTable = value;
            }
        }


        #endregion
    }
}
