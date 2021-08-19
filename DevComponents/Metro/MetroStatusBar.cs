using System;
using System.Collections.Generic;
using System.Text;
using DevComponents.DotNetBar.Metro.Rendering;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DevComponents.DotNetBar.Metro
{
    /// <summary>
    /// Represents Metro-UI Status Bar control.
    /// </summary>
    [ToolboxBitmap(typeof(MetroShell), "MetroStatusBar.ico"), ToolboxItem(true), Designer("DevComponents.DotNetBar.Design.MetroStatusBarDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf"), System.Runtime.InteropServices.ComVisible(false)]
    public class MetroStatusBar : ItemControl
    {
        #region Constructor
        private GenericItemContainer _ItemContainer = null;
        /// <summary>
        /// Initializes a new instance of the MetroStatusBar class.
        /// </summary>
        public MetroStatusBar()
        {
            _ItemContainer = new GenericItemContainer();
            _ItemContainer.GlobalItem = false;
            _ItemContainer.ContainerControl = this;
            _ItemContainer.Stretch = true;
            _ItemContainer.Displayed = true;
            _ItemContainer.WrapItems = false;
            _ItemContainer.ItemSpacing = 2;
            _ItemContainer.PaddingTop = 1;
            _ItemContainer.PaddingBottom = 4;
            _ItemContainer.PaddingLeft = 4;
            _ItemContainer.PaddingRight = 1;
            _ItemContainer.ToolbarItemsAlign = eContainerVerticalAlignment.Middle;
            _ItemContainer.EventHeight = false;
            _ItemContainer.FillsContainerControl = true;
            _ItemContainer.Style = eDotNetBarStyle.StyleManagerControlled;
            this.ColorScheme.Style = eDotNetBarStyle.StyleManagerControlled;
            _ItemContainer.SetOwner(this);
            _ItemContainer.LayoutType = eLayoutType.Toolbar;

            this.SetBaseItemContainer(_ItemContainer);
            this.DragDropSupport = true;
            this.ItemAdded += new EventHandler(ChildItemAdded);
            this.ItemRemoved += new ItemRemovedEventHandler(ChildItemRemoved);
        }

        protected override void Dispose(bool disposing)
        {
            this.ItemAdded -= new EventHandler(ChildItemAdded);
            this.ItemRemoved -= new ItemRemovedEventHandler(ChildItemRemoved);
            base.Dispose(disposing);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Indicates whether items that cannot fit are displayed on popup.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether items that cannot fit are displayed on popup.")]
        public bool OverflowEnabled
        {
            get { return _ItemContainer.OverflowEnabled; }
            set
            {
                _ItemContainer.OverflowEnabled = value;
                if (this.IsHandleCreated)
                    RecalcLayout();
            }
        }
        

        private void ChildItemAdded(object sender, EventArgs e)
        {
            this.RecalcLayout();
        }
        private void ChildItemRemoved(object sender, ItemRemovedEventArgs e)
        {
            this.RecalcLayout();
        }

        protected override void PaintControlBackground(ItemPaintArgs pa)
        {
            MetroRender.Paint(this, pa);
            base.PaintControlBackground(pa);
        }

        /// <summary>
        /// Returns collection of items on a bar.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
        public SubItemsCollection Items
        {
            get
            {
                return _ItemContainer.SubItems;
            }
        }

        private readonly int ResizeHandleWidth = 10;
        protected override Rectangle GetItemContainerRectangle()
        {
            Rectangle ic = base.GetItemContainerRectangle();
            if (_ResizeHandleVisible)
            {
                ic.Width -= ResizeHandleWidth;
                if (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                    ic.X += ResizeHandleWidth;
            }
            return ic;
        }

        private bool _ResizeHandleVisible = true;
        /// <summary>
        /// Gets or sets whether resize handle used to resize the parent form is visible. 
        /// </summary>
        [DefaultValue(true), Category("Appearance"), Description("Indicates whether resize handle used to resize the parent form is visible.")]
        public bool ResizeHandleVisible
        {
            get { return _ResizeHandleVisible; }
            set
            {
                if (value != _ResizeHandleVisible)
                {
                    bool oldValue = _ResizeHandleVisible;
                    _ResizeHandleVisible = value;
                    OnResizeHandleVisibleChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when ResizeHandleVisible property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnResizeHandleVisibleChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("ResizeHandleVisible"));
            this.RecalcLayout();
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (_ResizeHandleVisible && m.Msg == (int)WinApi.WindowsMessages.WM_NCHITTEST)
            {
                // Get position being tested...
                int x = WinApi.LOWORD(m.LParam);
                int y = WinApi.HIWORD(m.LParam);
                Point p = PointToClient(new Point(x, y));
                int resizeHandleWidth = Dpi.Width(ResizeHandleWidth);
                Rectangle resizeBounds = (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes) ?
                    new Rectangle(0, this.Height - resizeHandleWidth, resizeHandleWidth, resizeHandleWidth) :
                    new Rectangle(this.Width - resizeHandleWidth, this.Height - resizeHandleWidth, resizeHandleWidth, resizeHandleWidth);
                if (resizeBounds.Contains(p))
                {
                    m.Result = new IntPtr((int)WinApi.WindowHitTestRegions.TransparentOrCovered);
                    return;
                }
            }
            base.WndProc(ref m);
        }


        /// <summary>
        /// Gets or sets spacing between items, default value is 2.
        /// </summary>
        [DefaultValue(2), Category("Appearance"), Description("Gets or sets spacing between items.")]
        public int ItemSpacing
        {
            get { return _ItemContainer.ItemSpacing; }
            set
            {
                _ItemContainer.ItemSpacing = value;
                this.RecalcLayout();
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, 22);
            }
        }
        #endregion

        #region Licensing
#if !TRIAL
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (NativeFunctions.keyValidated2 != 266)
                TextDrawing.DrawString(e.Graphics, "Invalid License", this.Font, Color.FromArgb(180, Color.Red), this.ClientRectangle, eTextFormat.Bottom | eTextFormat.HorizontalCenter);
        }

        private string _LicenseKey = "";
        [Browsable(false), DefaultValue("")]
        public string LicenseKey
        {
            get { return _LicenseKey; }
            set
            {
                if (NativeFunctions.ValidateLicenseKey(value))
                    return;
                _LicenseKey = (!NativeFunctions.CheckLicenseKey(value) ? "9dsjkhds7" : value);
            }
        }
#endif
        #endregion
    }
}
