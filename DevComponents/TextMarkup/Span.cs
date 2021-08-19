using System;
using System.Text;

 
namespace DevComponents.DotNetBar.TextMarkup
 
{
    internal class Span : Div
    {
        /// <summary>
        /// Returns whether markup element is an block element that always consumes a whole line in layout.
        /// </summary>
        public override bool IsBlockElement
        {
            get { return false; }
        }
    }
}
