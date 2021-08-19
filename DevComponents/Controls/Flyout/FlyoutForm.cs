using DevComponents.DotNetBar.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar.Controls
{
    public partial class FlyoutForm : Form
    {
        public FlyoutForm()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.BackColor = Color.White;

            //_PointerSide = ePointerSide.Top;
            _PointerOffset = this.Width-50;
        }

        private int _PointerOffset;
        private const int CS_DROPSHADOW = 0x00020000;
        protected override CreateParams CreateParams
        {
            get
            {
                // add the drop shadow flag for automatically drawing
                // a drop shadow around the form
                CreateParams cp = base.CreateParams;
                if(_DropShadow)
                    cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
        protected override bool ShowWithoutActivation
        {
            get
            {
                return !_ActivateOnShow;
            }
        }

        private bool _DropShadow = true;
        /// <summary>
        /// Indicates whether flyout displays a drop shadow.
        /// </summary>
        [DefaultValue(true)]
        public bool DropShadow
        {
            get { return _DropShadow; }
            set
            {
                _DropShadow = value;
            }
        }
        

        private bool _ActivateOnShow = false;
        /// <summary>
        /// Gets or sets whether form is made active/focused when shown.
        /// </summary>
        [DefaultValue(false)]
        public bool ActivateOnShow
        {
            get { return _ActivateOnShow; }
            set
            {
                _ActivateOnShow = value;
            }
        }
        
        private GraphicsPath GetFormPath(Rectangle r)
        {
            return GetFormPath(r, false);
        }

        private static readonly Size _PointerSize = new Size(24, 12);
        internal static Size PointerSize
        {
            get
            {
                return _PointerSize;
            }
        }
        private GraphicsPath GetFormPath(Rectangle r, bool isBorder)
        {
            Size calloutSize = _PointerSize; // new Size(24, 12);
            GraphicsPath path = new GraphicsPath();

            if (_PointerSide == ePointerSide.Top)
            {
                int cX = Math.Min(Math.Max(3, _PointerOffset), r.Width - calloutSize.Width + 3); //r.Right - (calloutSize.Width + 10);
                path.AddLine(r.X, r.Y + calloutSize.Height, cX + (isBorder ? 1 : 0), r.Y + calloutSize.Height);
                path.AddLine(cX, r.Y + calloutSize.Height, cX + calloutSize.Width / 2, r.Y);
                path.AddLine(cX + calloutSize.Width / 2, r.Y, cX + calloutSize.Width, r.Y + calloutSize.Height);
                path.AddLine(cX + calloutSize.Width, r.Y + calloutSize.Height, r.Right, r.Y + calloutSize.Height);
                path.AddLine(r.Right, r.Y + calloutSize.Height, r.Right, r.Bottom);
                path.AddLine(r.Right, r.Bottom, r.X, r.Bottom);
                path.AddLine(r.X, r.Bottom, r.X, r.Y + calloutSize.Height);
                path.CloseAllFigures();
            }
            else if (_PointerSide == ePointerSide.Bottom)
            {
                int cX = Math.Min(Math.Max(3, _PointerOffset), r.Width - calloutSize.Width + 3); //r.Right - (calloutSize.Width + 10);
                path.AddLine(r.X, r.Bottom - calloutSize.Height, cX + (isBorder ? 1 : 0), r.Bottom - calloutSize.Height);
                path.AddLine(cX, r.Bottom - calloutSize.Height, cX + calloutSize.Width / 2, r.Bottom);
                path.AddLine(cX + calloutSize.Width / 2, r.Bottom, cX + calloutSize.Width, r.Bottom - calloutSize.Height);
                path.AddLine(cX + calloutSize.Width, r.Bottom - calloutSize.Height, r.Right, r.Bottom - calloutSize.Height);
                path.AddLine(r.Right, r.Bottom - calloutSize.Height, r.Right, r.Y);
                path.AddLine(r.Right, r.Y, r.X, r.Y);
                path.AddLine(r.X, r.Y, r.X, r.Bottom - calloutSize.Height);
                path.CloseAllFigures();
            }
            else if (_PointerSide == ePointerSide.Left)
            {
                int cY = Math.Min(Math.Max(3, _PointerOffset), r.Height - calloutSize.Width + 3); //r.Bottom - (calloutSize.Width + 10);
                path.AddLine(r.X + calloutSize.Height, r.Bottom, r.X + calloutSize.Height, cY + (isBorder ? 1 : 0));
                path.AddLine(r.X + calloutSize.Height, cY + (isBorder ? 1 : 0), r.X, cY + calloutSize.Width / 2);
                path.AddLine(r.X, cY + calloutSize.Width / 2, r.X + calloutSize.Height, cY + calloutSize.Width);
                path.AddLine(r.X + calloutSize.Height, cY + calloutSize.Width, r.X + calloutSize.Height, r.Y);
                path.AddLine(r.X + calloutSize.Height, r.Y, r.Right, r.Y);
                path.AddLine(r.Right, r.Y, r.Right, r.Bottom);
                path.AddLine(r.Right, r.Bottom, r.X + calloutSize.Height, r.Bottom);
                path.CloseAllFigures();
            }
            else if (_PointerSide == ePointerSide.Right)
            {
                int cY = Math.Min(Math.Max(3, _PointerOffset), r.Width - calloutSize.Height + 3); //r.Bottom - (calloutSize.Width + 10);
                path.AddLine(r.Right - calloutSize.Height, r.Y, r.Right - calloutSize.Height, cY + (isBorder ? 1 : 0));
                path.AddLine(r.Right - calloutSize.Height, cY + (isBorder ? 1 : 0), r.Right - (isBorder ? 1 : 0), cY + calloutSize.Width / 2);
                path.AddLine(r.Right - (isBorder ? 1 : 0), cY + calloutSize.Width / 2, r.Right - calloutSize.Height, cY + calloutSize.Width);
                path.AddLine(r.Right - calloutSize.Height, cY + calloutSize.Width, r.Right - calloutSize.Height, r.Bottom);
                path.AddLine(r.Right - calloutSize.Height, r.Bottom, r.X, r.Bottom);
                path.AddLine(r.X, r.Bottom, r.X, r.Y);
                path.AddLine(r.X, r.Y, r.Right - calloutSize.Height, r.Y);
                path.CloseAllFigures();
            }

            return path;
        }
        private Region GetFormRegion()
        {
            return new Region(GetFormPath(this.ClientRectangle));
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle r = this.ClientRectangle;
            r.Inflate(-1, -1);

            FlyoutColorTable table = GetColorTable();

            Color backColor = this.BackColor;
            if (backColor.IsEmpty)
                backColor = table.BackColor;
            Color borderColor = _BorderColor;
            if(borderColor == Color.Empty)
            {
                borderColor = table.BorderColor;
            }

            using (GraphicsPath path = GetFormPath(r, true))
            {
                using(SolidBrush borderBrush=new SolidBrush(borderColor))
                   e.Graphics.FillRectangle(borderBrush, this.ClientRectangle);
                e.Graphics.SetClip(path);
                using (SolidBrush brush = new SolidBrush(backColor))
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                //e.Graphics.DrawPath(Pens.Red, path);
            }
            //base.OnPaint(e);
        }
        private FlyoutColorTable GetColorTable()
        {
            return ((Office2007Renderer)GlobalManager.Renderer).ColorTable.Flyout;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //UpdateFormRegion();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            UpdateFormRegion();
            base.OnHandleCreated(e);
        }

        protected override void OnResize(EventArgs e)
        {
            UpdateFormRegion();
            base.OnResize(e);
        }

        private void UpdateFormRegion()
        {
            this.Region = GetFormRegion();
        }

        /// <summary>
        /// Gets or sets the pointer offset from the top-left corner
        /// </summary>
        public int PointerOffset
        {
            get { return _PointerOffset; }
            set
            {
                if (value !=_PointerOffset)
                {
                    int oldValue = _PointerOffset;
                    _PointerOffset = value;
                    OnPointerOffsetChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when PointerOffset property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnPointerOffsetChanged(int oldValue, int newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("PointerOffset"));
            if (this.IsHandleCreated)
                UpdateFormRegion();
        }

        private ePointerSide _PointerSide = ePointerSide.Bottom;
        /// <summary>
        /// Gets or sets the side pointer triangle is displayed on.
        /// </summary>
        public ePointerSide PointerSide
        {
            get { return _PointerSide; }
            set
            {
                if (value != _PointerSide)
                {
                    ePointerSide oldValue = _PointerSide;
                    _PointerSide = value;
                    OnPointerSideChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when PointerSide property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnPointerSideChanged(ePointerSide oldValue, ePointerSide newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("PointerSide"));
            if (this.IsHandleCreated)
                UpdateFormRegion();
        }

        private Color _BorderColor = Color.Empty;
        /// <summary>
        /// Gets or sets the flyout border color. Default value of Color.Empty indicates that color scheme will be used.
        /// </summary>
        [Category("Columns"), Description("Indicates flyout border color. Default value of Color.Empty indicates that color scheme will be used.")]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set { _BorderColor = value; }
        }
        /// <summary>
        /// Gets whether property should be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeBorderColor()
        {
            return !_BorderColor.IsEmpty;
        }
        /// <summary>
        /// Resets property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetBorderColor()
        {
            this.BorderColor = Color.Empty;
        }

        private bool _IsActive = false;
        protected override void OnActivated(EventArgs e)
        {
            _IsActive = true;
            base.OnActivated(e);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            _IsActive = false;
            base.OnDeactivate(e);
        }
        [Browsable(false)]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
        }
    }
    /// <summary>
    /// Defines the side of triangle pointer displayed on flyout popup.
    /// </summary>
    public enum ePointerSide
    {
        Top,
        Bottom,
        Left,
        Right
    }
}
