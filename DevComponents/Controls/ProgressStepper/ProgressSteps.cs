using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Represents the progress steps control.
    /// </summary>
    [ToolboxBitmap(typeof(ProgressSteps), "ProgressSteps.ico"), ToolboxItem(true), Designer("DevComponents.DotNetBar.Design.ProgressStepsDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf"), System.Runtime.InteropServices.ComVisible(false), DefaultEvent("ItemClick")]
    public class ProgressSteps : ItemControl
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ProgressSteps class.
        /// </summary>
        public ProgressSteps()
            : base()
        {
            _ViewContainer = new StepItemContainer();
            _ViewContainer.GlobalItem = false;
            _ViewContainer.ContainerControl = this;
            _ViewContainer.Displayed = true;
            _ViewContainer.Style = eDotNetBarStyle.StyleManagerControlled;
            _ViewContainer.SetOwner(this);
            this.SetBaseItemContainer(_ViewContainer);
        }
        private StepItemContainer _ViewContainer = null;
        #endregion

        #region Implementation
        /// <summary>
        /// Returns collection of items on a bar.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
        public SubItemsCollection Items
        {
            get
            {
                return _ViewContainer.SubItems;
            }
        }

        /// <summary>
        /// Gets/Sets the visual style for items and color scheme.
        /// </summary>
        [Browsable(true), DevCoBrowsable(true), Category("Appearance"), Description("Specifies the visual style of the control."), DefaultValue(eDotNetBarStyle.StyleManagerControlled)]
        public eDotNetBarStyle Style
        {
            get
            {
                return _ViewContainer.Style;
            }
            set
            {
                this.ColorScheme.SwitchStyle(value);
                _ViewContainer.Style = value;
                this.Invalidate();
                this.RecalcLayout();
            }
        }

        private Size _PreferredSize = Size.Empty;
        [Localizable(true), Browsable(false)]
        public new System.Windows.Forms.Padding Padding
        {
            get { return base.Padding; }
            set { base.Padding = value; }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            if (!_PreferredSize.IsEmpty) return _PreferredSize;

            if (!BarFunctions.IsHandleValid(this))
                return base.GetPreferredSize(proposedSize);

            if (this.Items.Count == 0 || !BarFunctions.IsHandleValid(this) || _ViewContainer.SubItems.Count == 0)
                return new Size(base.GetPreferredSize(proposedSize).Width, Dpi.Height22);

            int height = GetAutoSizePreferredHeight();

            _PreferredSize = new Size(proposedSize.Width, height);
            return _PreferredSize;
        }

        private int GetAutoSizePreferredHeight()
        {
            int height = ElementStyleLayout.VerticalStyleWhiteSpace(this.GetBackgroundStyle());
            height += _ViewContainer.CalculatedHeight > 0 ? _ViewContainer.CalculatedHeight : 20;
            return height;
        }
        protected override void RecalcSize()
        {
            base.RecalcSize();
            if (this.AutoSize && this.IsHandleCreated && GetAutoSizePreferredHeight() != this.Height)
            {
                InvalidateAutoSize();
                this.AdjustSize();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is automatically resized to display its entire contents. You can set MaximumSize.Width property to set the maximum width used by the control.
        /// </summary>
        [Browsable(true), DefaultValue(false), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                if (this.AutoSize != value)
                {
                    base.AutoSize = value;
                    InvalidateAutoSize();
                    AdjustSize();
                }
            }
        }

        private void InvalidateAutoSize()
        {
            _PreferredSize = Size.Empty;
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (this.AutoSize)
            {
                Size preferredSize = base.PreferredSize;
                if (preferredSize.Width > 0)
                    width = preferredSize.Width;
                if (preferredSize.Height > 0)
                    height = preferredSize.Height;
            }
            base.SetBoundsCore(x, y, width, height, specified);
        }

        private void AdjustSize()
        {
            if (this.AutoSize)
            {
                System.Drawing.Size prefSize = base.PreferredSize;
                if (prefSize.Width > 0 && prefSize.Height > 0)
                    this.Size = base.PreferredSize;
                else if (prefSize.Height > 0)
                    this.Size = new Size(this.Width, base.PreferredSize.Height);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (this.AutoSize)
                this.AdjustSize();
        }

        /// <summary>
        /// Gets or sets the arrow pointer width for the StepItem objects hosted within this container.
        /// </summary>
        [DefaultValue(10), Category("Appearance"), Description("Gets or sets the arrow pointer width for the StepItem objects hosted within this container.")]
        public int PointerSize
        {
            get { return _ViewContainer.PointerSize; }
            set
            {
                _ViewContainer.PointerSize = value;
                
            }
        }
        #endregion

        #region Property Hiding
        [Browsable(false)]
        public override eBarImageSize ImageSize
        {
            get
            {
                return base.ImageSize;
            }
            set
            {
                base.ImageSize = value;
            }
        }
        [Browsable(false)]
        public override System.Windows.Forms.ImageList ImagesLarge
        {
            get
            {
                return base.ImagesLarge;
            }
            set
            {
                base.ImagesLarge = value;
            }
        }
        [Browsable(false)]
        public override System.Windows.Forms.ImageList ImagesMedium
        {
            get
            {
                return base.ImagesMedium;
            }
            set
            {
                base.ImagesMedium = value;
            }
        }
        [Browsable(false)]
        public override Font KeyTipsFont
        {
            get
            {
                return base.KeyTipsFont;
            }
            set
            {
                base.KeyTipsFont = value;
            }
        }
        [Browsable(false)]
        public override bool ShowShortcutKeysInToolTips
        {
            get
            {
                return base.ShowShortcutKeysInToolTips;
            }
            set
            {
                base.ShowShortcutKeysInToolTips = value;
            }
        }
        [Browsable(false)]
        public override bool ThemeAware
        {
            get
            {
                return base.ThemeAware;
            }
            set
            {
                base.ThemeAware = value;
            }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
        #endregion

        #region Licensing
#if !TRIAL
        private string m_LicenseKey = "";
        [Browsable(false), DefaultValue("")]
        public string LicenseKey
        {
            get { return m_LicenseKey; }
            set
            {
                if (NativeFunctions.ValidateLicenseKey(value))
                    return;
                m_LicenseKey = (!NativeFunctions.CheckLicenseKey(value) ? "9dsjkhds7" : value);
            }
        }
#endif
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
#if !TRIAL
            if (NativeFunctions.keyValidated2 != 266)
                TextDrawing.DrawString(e.Graphics, "Invalid License", this.Font, Color.FromArgb(180, Color.Red), this.ClientRectangle, eTextFormat.Bottom | eTextFormat.HorizontalCenter);
#else
            if (NativeFunctions.ColorExpAlt() || !NativeFunctions.CheckedThrough)
		    {
			    e.Graphics.Clear(SystemColors.Control);
                return;
            }
#endif
        }
        #endregion
    }
}
