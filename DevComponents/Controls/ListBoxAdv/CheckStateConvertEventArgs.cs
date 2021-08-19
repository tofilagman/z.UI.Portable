using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Provides data for CheckStateConvert event.
    /// </summary>
    public class CheckStateConvertEventArgs : EventArgs
    {
        public readonly object Value;
        public CheckState? CheckState = null;
        /// <summary>
        /// Initializes a new instance of the CheckStateConvertEventArgs class.
        /// </summary>
        /// <param name="value"></param>
        public CheckStateConvertEventArgs(object value)
        {
            Value = value;
        }
    }
}
