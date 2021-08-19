using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace DevComponents.DotNetBar.Controls
{
    public class NewTabFormItem : TabFormItemBase
    {
        #region Internal Implementation
        public override void Paint(ItemPaintArgs p)
        {
            Rendering.BaseRenderer renderer = p.Renderer;
            if (renderer != null)
            {
                p.ButtonItemRendererEventArgs.Graphics = p.Graphics;
                p.ButtonItemRendererEventArgs.ButtonItem = this;
                p.ButtonItemRendererEventArgs.ItemPaintArgs = p;
                renderer.DrawNewTabFormItem(p.ButtonItemRendererEventArgs);
            }

            if (!string.IsNullOrEmpty(NotificationMarkText))
                DevComponents.DotNetBar.Rendering.NotificationMarkPainter.Paint(p.Graphics, this.Bounds, NotificationMarkPosition,
                    NotificationMarkText, new Size(NotificationMarkSize, NotificationMarkSize), NotificationMarkOffset, NotificationMarkColor);
            this.DrawInsertMarker(p.Graphics);
        }

        public override void RecalcSize()
        {
            m_Rect.Width = Dpi.Width32 + TabFormItem.TabOverlap/2;
            m_Rect.Height = Dpi.Height16;
            m_NeedRecalcSize = false;

        }

        private string _CashedColorTableName = "Default";
        internal override string GetColorTableName()
        {
            return this.CustomColorName != "" ? this.CustomColorName : _CashedColorTableName;
        }

        private Color[] _BackColors = null;
        /// <summary>
        /// Indicates the array of colors that when set are used to draw the background of the item.
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates the array of colors that when set are used to draw the background of the item."), TypeConverter(typeof(ArrayConverter))]
        public Color[] BackColors
        {
            get
            {
                return _BackColors;
            }
            set
            {
                if (_BackColors != value)
                {
                    _BackColors = value;
                    //OnPropertyChanged(new PropertyChangedEventArgs("Colors"));
                    this.Refresh();
                }
            }
        }
#endregion
    }
}
