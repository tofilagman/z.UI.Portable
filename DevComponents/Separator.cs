using System;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using DevComponents.DotNetBar.Controls;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Represents the visual separator line that is displayed between items.
    /// </summary>
    [ToolboxItem(false)]
    public class Separator : BaseItem
    {
        #region Private Variables

        #endregion

        #region Constructors
        /// <summary>
        /// Creates new instance of Separator.
		/// </summary>
		public Separator():this("") {}
		/// <summary>
        /// Creates new instance of Separator and assigns the name to it.
		/// </summary>
		/// <param name="sItemName">Item name.</param>
		public Separator(string sItemName):base(sItemName) 
        {
        }
        #endregion

        #region Internal Implementation
        public override void Paint(ItemPaintArgs p)
        {
            Rectangle bounds = this.Bounds;
            if (bounds.Width < 1 || bounds.Height < 1) return;

            Color colorLine = p.Colors.ItemSeparator;
            if (p.ContainerControl is SideNavStrip)
                colorLine = p.Colors.BarDockedBorder;
            if (!_SeparatorColor.IsEmpty)
                colorLine = _SeparatorColor;
            Color colorShade = p.Colors.ItemSeparatorShade;
            if (!_ShadeColor.IsEmpty)
                colorShade = _ShadeColor;
            Graphics g=p.Graphics;
            Rectangle r;

            Padding padding = Dpi.Size(_Padding);
            if (this.Orientation == eOrientation.Vertical && _SeparatorOrientation == eDesignMarkerOrientation.NotSet || _SeparatorOrientation== eDesignMarkerOrientation.Vertical)
            {
                r = new Rectangle(bounds.X + padding.Left, bounds.Y + padding.Top + (bounds.Height - padding.Vertical) / 2, bounds.Width - padding.Horizontal, 1);
                if (!colorLine.IsEmpty)
                    DisplayHelp.DrawLine(g, r.X, r.Y, r.Right -1, r.Y, colorLine, 1);
            }
            else
            {
                r = new Rectangle(bounds.X + padding.Left + (bounds.Width - padding.Horizontal) / 2, 
                    bounds.Y + padding.Top, 1, bounds.Height - padding.Vertical);
                if (!colorLine.IsEmpty)
                    DisplayHelp.DrawLine(g, r.X, r.Y, r.X, r.Bottom - 1, colorLine, 1);
            }

            if (!colorShade.IsEmpty && (_FixedSize.Height>1 && 
                (_SeparatorOrientation == eDesignMarkerOrientation.Vertical || this.Orientation == eOrientation.Vertical && _SeparatorOrientation == eDesignMarkerOrientation.NotSet) ||
                _FixedSize.Width>1 && (_SeparatorOrientation == eDesignMarkerOrientation.Horizontal || this.Orientation == eOrientation.Horizontal && _SeparatorOrientation == eDesignMarkerOrientation.NotSet)))
            {
                r.Inflate(1, 1);
                DisplayHelp.DrawRectangle(g, colorShade, r);
            }

            if (this.DesignMode && this.Focused)
            {
                r = this.Bounds;
                r.Inflate(-1, -1);
                DesignTime.DrawDesignTimeSelection(p.Graphics, r, p.Colors.ItemDesignTimeBorder);
            }

            this.DrawInsertMarker(p.Graphics);
        }

        private Padding _Padding = new Padding(2, 2, 2, 2);
        /// <summary>
        /// Gets or sets separator padding.
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("Gets or sets separator padding."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Padding Padding
        {
            get { return _Padding; }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializePadding()
        {
            return _Padding.Bottom != 2 || _Padding.Top != 2 || _Padding.Left != 2 || _Padding.Right != 2;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        private void ResetPadding()
        {
            _Padding = new Padding(2, 2, 2, 2);
        }

        private Size _FixedSize = new Size(3, 16);
        /// <summary>
        /// Gets or sets the size of separator. Size specified is for separator in Vertical orientation. If orientation changes then the size will be internally switched to respect proper orientation.
        /// </summary>
        public Size FixedSize
        {
            get { return _FixedSize; }
            set
            {
                _FixedSize = value;
                NeedRecalcSize = true;
                OnAppearanceChanged();
            }
        }

        public override void RecalcSize()
        {
            if (this.Orientation == eOrientation.Horizontal)
                m_Rect.Size = new Size(Dpi.Width(_FixedSize.Width + _Padding.Horizontal), Dpi.Height(_FixedSize.Height + _Padding.Vertical));
            else
                m_Rect.Size = new Size(Dpi.Width(_FixedSize.Height + Padding.Horizontal), Dpi.Height(_FixedSize.Width + _Padding.Vertical));
            base.RecalcSize();
        }

        /// <summary>
		/// Returns copy of the item.
		/// </summary>
		public override BaseItem Copy()
		{
            Separator objCopy = new Separator(m_Name);
			this.CopyToItem(objCopy);
			return objCopy;
		}
		/// <summary>
		/// Copies the ButtonItem specific properties to new instance of the item.
		/// </summary>
		/// <param name="copy">New ButtonItem instance.</param>
        protected override void CopyToItem(BaseItem copy)
        {
            Separator objCopy = copy as Separator;
            base.CopyToItem(objCopy);
            objCopy.FixedSize = _FixedSize;
            objCopy.Padding.Left = _Padding.Left;
            objCopy.Padding.Right = _Padding.Right;
            objCopy.Padding.Top = _Padding.Top;
            objCopy.Padding.Bottom = _Padding.Bottom;
        }

        private eDesignMarkerOrientation _SeparatorOrientation = eDesignMarkerOrientation.NotSet;
        /// <summary>
        /// Indicates splitter orientation.
        /// </summary>
        [DefaultValue(eDesignMarkerOrientation.NotSet), Category("Appearance"), Description("Indicates splitter orientation.")]
        public eDesignMarkerOrientation SeparatorOrientation
        {
            get { return _SeparatorOrientation; }
            set
            {
                if (_SeparatorOrientation != value)
                {
                    eDesignMarkerOrientation oldValue = _SeparatorOrientation;
                    _SeparatorOrientation = value;
                    OnSeparatorOrientationChanged(value, oldValue);
                }
            }
        }

        protected virtual void OnSeparatorOrientationChanged(eDesignMarkerOrientation newValue, eDesignMarkerOrientation oldValue)
        {
            this.OnAppearanceChanged();
        }

        private Color _SeparatorColor = Color.Empty;

        /// <summary>
        /// Gets or sets the separator color.
        /// </summary>
        [Category("Appearance"), Description("Indicates separator color.")]
        public Color SeparatorColor
        {
            get { return _SeparatorColor; }
            set
            {
                _SeparatorColor = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets whether property should be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeSeparatorColor()
        {
            return !_SeparatorColor.IsEmpty;
        }

        /// <summary>
        /// Resets property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetSeparatorColor()
        {
            this.SeparatorColor = Color.Empty;
        }

        private Color _ShadeColor = Color.Empty;

        /// <summary>
        /// Gets or sets the separator shade color.
        /// </summary>
        [Category("Appearance"), Description("Indicates separator shade color.")]
        public Color ShadeColor
        {
            get { return _ShadeColor; }
            set
            {
                _ShadeColor = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets whether property should be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeShadeColor()
        {
            return !_ShadeColor.IsEmpty;
        }

        /// <summary>
        /// Resets property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetShadeColor()
        {
            this.ShadeColor = Color.Empty;
        }
        #endregion


    }
}
