using System;
using System.Collections.Generic;
using System.Text;

namespace DevComponents.DotNetBar
{
    public class ListBoxAdvItemCheckEventArgs : EventArgs
    {
        /// <summary>
        /// Indicates that CheckState change of the item should be canceled.
        /// </summary>
        public bool Cancel = false;
        /// <summary>
        /// Specifies the ListBoxItem that was changing.
        /// </summary>
        public readonly ListBoxItem Item;
        /// <summary>
        /// When data-bound provides the object which was used to generate an ListBoxItem.
        /// </summary>
        public readonly object Value;

        /// <summary>
        /// Initializes a new instance of the ListBoxAdvItemCheckEventArgs class.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="value"></param>
        public ListBoxAdvItemCheckEventArgs(ListBoxItem item, object value)
        {
            Item = item;
            Value = value;
        }
    }
}
