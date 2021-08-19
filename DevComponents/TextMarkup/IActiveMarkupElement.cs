using System;
using System.Text;
using System.Windows.Forms;

 
namespace DevComponents.DotNetBar.TextMarkup
 
{
    internal interface IActiveMarkupElement
    {
        bool HitTest(int x, int y);
        void MouseEnter(Control parent);
        void MouseLeave(Control parent);
        void MouseDown(Control parent, MouseEventArgs e);
        void MouseUp(Control parent, MouseEventArgs e);
        void Click(Control parent);
    }
}
