using System;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.Drawing;
using DevComponents.DotNetBar.Rendering;
using System.Threading;
using System.Collections.Generic;

namespace DevComponents.DotNetBar
{
    [ToolboxBitmap(typeof(StyleManager), "StyleManager.ico"), ToolboxItem(true)]
    public class StyleManager : Component
    {
        #region Constructor
        static StyleManager()
        {
            StyleManager._ReadWriteClientsListLock = new ReaderWriterLock();
            StyleManager._ReadWriteAmbientListLock = new ReaderWriterLock();
        }
        /// <summary>
        /// Initializes a new instance of the StyleManager class.
        /// </summary>
        public StyleManager()
        {

        }

        /// <summary>
        /// Initializes a new instance of the StyleManager class with the specified container.
        /// </summary>
        /// <param name="container">An IContainer that represents the container for the command.</param>
        public StyleManager(IContainer container)
            : this()
        {
            container.Add(this);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates Ambient colors for control and its child controls.
        /// </summary>
        /// <param name="control"></param>
        internal static void UpdateMetroAmbientColors(Control control)
        {
            DevComponents.DotNetBar.Metro.ColorTables.MetroColorTable colorTable = DevComponents.DotNetBar.Metro.Rendering.MetroRender.GetColorTable();
            UpdateAmbientProperties(control, colorTable);
        }
        private static void UpdateAmbientProperties(Control item, DevComponents.DotNetBar.Metro.ColorTables.MetroColorTable colorTable)
        {
            Color backColor = Color.Empty;
            Color foreColor = Color.Empty;
            eAmbientSettings ambientSettings = GetAmbientSettings(item);

            if (ambientSettings != eAmbientSettings.None)
            {
                if (item is DevComponents.DotNetBar.Controls.PageSliderPage)
                {
                    if ((ambientSettings & eAmbientSettings.BackColor) == eAmbientSettings.BackColor)
                        item.BackColor = colorTable.CanvasColor;
                    if ((ambientSettings & eAmbientSettings.ForeColor) == eAmbientSettings.ForeColor)
                        item.ForeColor = colorTable.ForeColor;
                }
                else if (item is Panel)
                {
                    if (ShouldUpdateAmbientPanel((Panel)item))
                    {
                        if (item.BackColor != Color.Transparent && (ambientSettings & eAmbientSettings.BackColor) == eAmbientSettings.BackColor)
                            item.BackColor = colorTable.CanvasColorShadeLight;
                        if ((ambientSettings & eAmbientSettings.ForeColor) == eAmbientSettings.ForeColor)
                            item.ForeColor = colorTable.ForeColor;

                        if (item is DevComponents.DotNetBar.Controls.GroupPanel)
                        {
                            if ((ambientSettings & eAmbientSettings.BackColor) == eAmbientSettings.BackColor)
                            {
                                ((DevComponents.DotNetBar.Controls.GroupPanel)item).CanvasColor = colorTable.CanvasColor;
                                ((DevComponents.DotNetBar.Controls.GroupPanel)item).BackColor = colorTable.CanvasColor;
                            }
                        }
                    }
                }
                else if (item is AdvTree.AdvTree || item is DevComponents.DotNetBar.Controls.ComboTree)
                {
                    AdvTree.AdvTree tree = null;
                    if (item is AdvTree.AdvTree)
                        tree = (AdvTree.AdvTree)item;
                    else
                    {
                        if (item.BackColor != Color.Transparent && (ambientSettings & eAmbientSettings.BackColor) == eAmbientSettings.BackColor)
                            item.BackColor = colorTable.EditControlBackColor;
                        if ((ambientSettings & eAmbientSettings.ForeColor) == eAmbientSettings.ForeColor)
                            item.ForeColor = colorTable.ForeColor;
                        tree = ((DevComponents.DotNetBar.Controls.ComboTree)item).AdvTree;
                    }
                    if (tree.BackColor != Color.Transparent && (ambientSettings & eAmbientSettings.BackColor) == eAmbientSettings.BackColor)
                        tree.BackColor = colorTable.EditControlBackColor;
                    if ((ambientSettings & eAmbientSettings.ForeColor) == eAmbientSettings.ForeColor)
                    {
                        tree.ForeColor = colorTable.ForeColor;
                        if (tree.NodeStyle != null && tree.NodeStyle.BackColor.IsEmpty && !tree.NodeStyle.TextColor.IsEmpty)
                            tree.NodeStyle.TextColor = colorTable.ForeColor;
                        if (tree.CellStyleDefault != null && tree.CellStyleDefault.BackColor.IsEmpty && !tree.CellStyleDefault.TextColor.IsEmpty)
                            tree.CellStyleDefault.TextColor = colorTable.ForeColor;
                        if (tree.NodesConnector != null)
                            tree.NodesConnector.LineColor = colorTable.MetroPartColors.TextInactiveColor;
                    }
                }
                else if (item is DevComponents.DotNetBar.Controls.TextBoxX)
                {
                    if (item.BackColor != Color.Transparent && (ambientSettings & eAmbientSettings.BackColor) == eAmbientSettings.BackColor)
                    {
                        item.BackColor = colorTable.EditControlBackColor;
                        if(((DevComponents.DotNetBar.Controls.TextBoxX)item).DisabledBackColor.IsEmpty)
                            ((DevComponents.DotNetBar.Controls.TextBoxX)item).DisabledBackColor = colorTable.EditControlBackColor;
                    }
                    if ((ambientSettings & eAmbientSettings.ForeColor) == eAmbientSettings.ForeColor)
                        item.ForeColor = colorTable.ForeColor;
                }
                else if (item is DevComponents.DotNetBar.Controls.MaskedTextBoxAdv)
                {
                    if (item.BackColor != Color.Transparent && (ambientSettings & eAmbientSettings.BackColor) == eAmbientSettings.BackColor)
                    {
                        item.BackColor = colorTable.EditControlBackColor;
                        if (((DevComponents.DotNetBar.Controls.MaskedTextBoxAdv)item).DisabledBackColor.IsEmpty)
                            ((DevComponents.DotNetBar.Controls.MaskedTextBoxAdv)item).DisabledBackColor = colorTable.EditControlBackColor;
                    }
                    if ((ambientSettings & eAmbientSettings.ForeColor) == eAmbientSettings.ForeColor)
                        item.ForeColor = colorTable.ForeColor;
                }
                else if (IsEditControl(item))
                {
                    if (item.BackColor != Color.Transparent && (ambientSettings & eAmbientSettings.BackColor) == eAmbientSettings.BackColor)
                        item.BackColor = colorTable.EditControlBackColor;
                    if ((ambientSettings & eAmbientSettings.ForeColor) == eAmbientSettings.ForeColor)
                        item.ForeColor = colorTable.ForeColor;
                }
                else if (!(item is Bar || item is DevComponents.DotNetBar.Controls.SideNav))
                {
                    if (item.BackColor != Color.Transparent && (ambientSettings & eAmbientSettings.BackColor) == eAmbientSettings.BackColor)
                    {
                        if (item is RibbonForm)
                        {
                            RibbonForm form = (RibbonForm)item;
                            if (!form.BackColorSet)
                            {
                                form.BackColor = colorTable.CanvasColor;
                                form.BackColorSet = false;
                            }
                        }
                        else item.BackColor = colorTable.CanvasColor;
                        
                    }
                    if ((ambientSettings & eAmbientSettings.ForeColor) == eAmbientSettings.ForeColor)
                        item.ForeColor = colorTable.ForeColor;
                }
                backColor = item.BackColor;
                foreColor = item.ForeColor;
            }
            if ((ambientSettings & eAmbientSettings.ChildControls) == eAmbientSettings.ChildControls)
            {
                foreach (Control child in item.Controls)
                {
                    UpdateAmbientProperties(child, colorTable);
                }
            }
        }
        private static bool IsEditControl(Control item)
        {
            return item is TextBox || item is TreeView || item is ListView || item is DevComponents.Editors.VisualControlBase ||
                item is DevComponents.DotNetBar.Controls.ComboBoxEx || item is MaskedTextBox || item is DevComponents.DotNetBar.Controls.MaskedTextBoxAdv ||
                item is RichTextBox || item is DevComponents.DotNetBar.Controls.RichTextBoxEx || item is DevComponents.DotNetBar.Controls.TextBoxDropDown;
                
        }
        private static bool ShouldUpdateAmbientPanel(Panel item)
        {
            return !(item is DevComponents.DotNetBar.Metro.MetroTabPanel || item is DevComponents.DotNetBar.Controls.SideNavPanel);
        }
        private static bool ShouldUpdateAmbientProperties(Control item)
        {
            return !(item is ButtonX);
        }
        private static eAmbientSettings GetAmbientSettings(Control control)
        {
            if (control is ButtonX || control is ScrollBarAdv) return eAmbientSettings.None;

            foreach (WeakReference item in _RegisteredAmbients)
            {
                if (item.IsAlive)
                {
                    StyleManagerAmbient ambientProvider = (StyleManagerAmbient)item.Target;
                    if (ambientProvider.Contains(control))
                        return ambientProvider.GetEnableAmbientSettings(control);
                }
            }

            return eAmbientSettings.All;
        }

        /// <summary>
        /// Updates ambient colors for the control and its children.
        /// </summary>
        /// <param name="c"></param>
        public static void UpdateAmbientColors(Control c)
        {
            if (StyleManager.IsMetro(StyleManager.Style))
            {
                UpdateMetroAmbientColors(c);
            }
            else if (StyleManager.IsMetro(StyleManager.PreviousStyle))
            {
                eAmbientSettings ambientSettings = GetAmbientSettings(c);
                if ((ambientSettings & eAmbientSettings.BackColor) == eAmbientSettings.BackColor)
                    c.ResetBackColor();// = SystemColors.Control;
                if ((ambientSettings & eAmbientSettings.ForeColor) == eAmbientSettings.ForeColor)
                    c.ResetForeColor(); // = SystemColors.ControlText;
            }
        }
        private static DevComponents.DotNetBar.Metro.ColorTables.MetroColorTable GetMetroColorTable()
        {
            return DevComponents.DotNetBar.Metro.Rendering.MetroRender.GetColorTable();
        }

        /// <summary>
        /// Gets or sets the global style for the controls that have Style=ManagerControlled.
        /// </summary>
        [DefaultValue(eDotNetBarStyle.Office2007), Category("Appearance"), Description("Indicates global style for the controls that have Style=ManagerControlled.")]
        public eStyle ManagerStyle
        {
            get { return StyleManager.Style; }
            set
            {
                StyleManager.Style = value;
            }
        }
        /// <summary>
        /// Gets or sets the color current style is tinted with.
        /// </summary>
        [Category("Appearance"), Description("Indicates color current style is tinted with.")]
        public Color ManagerColorTint
        {
            get { return StyleManager.ColorTint; }
            set { StyleManager.ColorTint = value; }
        }
        /// <summary>
        /// Gets whether property should be serialized by WinForms designer.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeManagerColorTint()
        {
            return !ManagerColorTint.IsEmpty;
        }
        /// <summary>
        /// Resets property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetManagerColorTint()
        {
            ManagerColorTint = Color.Empty;
        }

        private static eStyle _PreviousStyle = eStyle.Office2007Blue;
        /// <summary>
        /// Gets previous effective style.
        /// </summary>
        public static eStyle PreviousStyle
        {
            get
            {
                return _PreviousStyle;
            }
        }

        //private Form _StyleContextForm = null;
        ///// <summary>
        ///// Gets or sets the context form for the style that is set on the style manager. Setting the context form will cause all controls on the form and form itself
        ///// to use style and color scheme that is set on this instance of StyleManager instead of global style and color scheme.
        ///// </summary>
        //[DefaultValue(null), Category("Behavior"), Description("Indicates context form for the style that is set on the style manager. Setting the context form will cause all controls on the form and form itself to use style and color scheme that is set on this instance of StyleManager instead of global style and color scheme.")]
        //public Form StyleContextForm
        //{
        //    get { return _StyleContextForm; }
        //    set 
        //    {
        //        if (_StyleContextForm != null)
        //        {
        //            UnregisterContextForm(_StyleContextForm);
        //            _StyleContextForm = value;
        //            if (_StyleContextForm != null)
        //                RegisterContextForm(_StyleContextForm);
        //        }
        //    }
        //}

        //private DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters _MetroColorParameters = DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters.Default;
        /// <summary>
        /// Gets or sets color generation parameters for Metro color generator. 
        /// </summary>
        [Editor("DevComponents.DotNetBar.Design.Metro.MetroColorThemeEditor, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf", typeof(System.Drawing.Design.UITypeEditor))]
        public DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters MetroColorParameters
        {
            get { return MetroColorGeneratorParameters; }
            set
            {
                MetroColorGeneratorParameters = value;
            }
        }

        private static DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters _MetroColorGeneratorParameters = DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters.Default;
        /// <summary>
        /// Gets or sets color generation parameters for Metro color generator. 
        /// </summary>
        public static DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters MetroColorGeneratorParameters
        {
            get { return _MetroColorGeneratorParameters; }
            set
            {
                _MetroColorGeneratorParameters = value;
                if (_Style == eStyle.Metro || _Style  == eStyle.OfficeMobile2014 || _Style == eStyle.Office2016 || _Style == eStyle.VisualStudio2012Dark || _Style == eStyle.VisualStudio2012Light)
                {
                    OnStyleChanged(eStyle.Metro, eStyle.Metro);
                }
            }
        }

        private static eStyle _Style = eStyle.Office2007Blue;
        /// <summary>
        /// Gets or sets the current visual style.
        /// </summary>
        public static eStyle Style
        {
            get
            {
                return _Style;
            }
            set
            {
                eStyle oldValue = _Style;
                _Style = value;
                _EffectiveStyle = DotNetBarStyleFromStyle(value);
                if (_Style == eStyle.VisualStudio2012Light)
                    _MetroColorGeneratorParameters = DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters.VisualStudio2012Light;
                else if(_Style == eStyle.VisualStudio2012Dark)
                    _MetroColorGeneratorParameters = DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters.VisualStudio2012Dark;
                else if (_Style == eStyle.OfficeMobile2014)
                    _MetroColorGeneratorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(Color.White, ColorScheme.GetColor(0xB7472A));
                else if (_Style == eStyle.Office2016)
                    _MetroColorGeneratorParameters = DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters.Office2016Blue;
                if (oldValue != _Style)
                    _PreviousStyle = oldValue;

                OnStyleChanged(oldValue, value);
            }
        }

        private static eDotNetBarStyle DotNetBarStyleFromStyle(eStyle value)
        {
            if (value == eStyle.Windows7Blue)
                return eDotNetBarStyle.Windows7;
            else if (value == eStyle.Office2010Silver || value == eStyle.Office2010Blue || value == eStyle.Office2010Black || value == eStyle.VisualStudio2010Blue)
                return eDotNetBarStyle.Office2010;
            else if (value == eStyle.Metro || StyleManager.IsVisualStudio2012(value) || value == eStyle.OfficeMobile2014 || value == eStyle.Office2016)
                return eDotNetBarStyle.Metro;

            return eDotNetBarStyle.Office2007;
        }
        private static eDotNetBarStyle _EffectiveStyle = eDotNetBarStyle.Office2007;
        public static eDotNetBarStyle GetEffectiveStyle()
        {
            return _EffectiveStyle;
        }

        private static Color _ColorTint = Color.Empty;
        /// <summary>
        /// Gets or sets the color tint that is applied to current Office 2007, Office 2010 or Windows 7 color table.
        /// Default value is Color.Empty which indicates that no color blending is performed.
        /// </summary>
        public static Color ColorTint
        {
            get
            {
                return _ColorTint;
            }
            set
            {
                Color oldValue = _ColorTint;
                _ColorTint = value;
                OnColorTintChanged(oldValue, value);
            }
        }

        /// <summary>
        /// Changes the StyleManager style and color tint in one step. Use this method if you need to change style and color tint simultaneously in single step for better performance.
        /// </summary>
        /// <param name="newStyle">New style.</param>
        /// <param name="colorTint">Color tint for the style.</param>
        public static void ChangeStyle(eStyle newStyle, Color colorTint)
        {
            _ColorTint = colorTint;
            Style = newStyle;
        }

        private static void OnColorTintChanged(Color oldValue, Color newValue)
        {
            ChangeColorTable(_Style, true);
            NotifyOnStyleChange();
#if FRAMEWORK20
            foreach (Form form in Application.OpenForms)
            {
                if (form.InvokeRequired)
                {
                    if (form is OfficeForm)
                        form.Invoke(new
                                MethodInvoker(delegate() { ((OfficeForm)form).InvalidateNonClient(true); }));

                    form.Invoke(new
                                MethodInvoker(delegate() { form.Invalidate(true); }));
                }
                else
                {
                    if (form is OfficeForm)
                        ((OfficeForm)form).InvalidateNonClient(true);
                    form.Invalidate(true);
                }
            }
#else
            WeakReference[] references = new WeakReference[_RegisteredControls.Count];
            _RegisteredControls.CopyTo(references);
            foreach (WeakReference reference in references)
            {
                Control target = reference.Target as Control;
                if (reference.IsAlive)
                {
                    if (target != null)
                    {
                        target.Invalidate(true);
                    }
                }
            }
#endif
        }

        private static void ChangeColorTable(eStyle style)
        {
            ChangeColorTable(style, false);
        }

        private static void ChangeColorTable(eStyle style, bool colorBlendChanged)
        {
            if (style == eStyle.Office2010Silver)
            {
                // Ensure that proper color table is selected.
                if (GlobalManager.Renderer is Office2007Renderer)
                {
                    Office2007Renderer renderer = (Office2007Renderer)GlobalManager.Renderer;
                    if (colorBlendChanged || !(renderer.ColorTable is Office2010ColorTable) ||
                        renderer.ColorTable is Office2010ColorTable && ((Office2010ColorTable)renderer.ColorTable).ColorScheme != eOffice2010ColorScheme.Silver)
                    {
                        ((Office2007Renderer)GlobalManager.Renderer).ColorTable = new Office2010ColorTable(eOffice2010ColorScheme.Silver, _ColorTint);
                    }
                }
            }
            else if (style == eStyle.Office2010Blue)
            {
                // Ensure that proper color table is selected.
                if (GlobalManager.Renderer is Office2007Renderer)
                {
                    Office2007Renderer renderer = (Office2007Renderer)GlobalManager.Renderer;
                    if (colorBlendChanged || !(renderer.ColorTable is Office2010ColorTable) ||
                        renderer.ColorTable is Office2010ColorTable && ((Office2010ColorTable)renderer.ColorTable).ColorScheme != eOffice2010ColorScheme.Blue)
                    {
                        ((Office2007Renderer)GlobalManager.Renderer).ColorTable = new Office2010ColorTable(eOffice2010ColorScheme.Blue, _ColorTint);
                    }
                }
            }
            else if (style == eStyle.Office2010Black)
            {
                // Ensure that proper color table is selected.
                if (GlobalManager.Renderer is Office2007Renderer)
                {
                    Office2007Renderer renderer = (Office2007Renderer)GlobalManager.Renderer;
                    if (colorBlendChanged || !(renderer.ColorTable is Office2010ColorTable) ||
                        renderer.ColorTable is Office2010ColorTable && ((Office2010ColorTable)renderer.ColorTable).ColorScheme != eOffice2010ColorScheme.Black)
                    {
                        ((Office2007Renderer)GlobalManager.Renderer).ColorTable = new Office2010ColorTable(eOffice2010ColorScheme.Black, _ColorTint);
                    }
                }
            }
            else if (style == eStyle.VisualStudio2010Blue)
            {
                // Ensure that proper color table is selected.
                if (GlobalManager.Renderer is Office2007Renderer)
                {
                    Office2007Renderer renderer = (Office2007Renderer)GlobalManager.Renderer;
                    if (colorBlendChanged || !(renderer.ColorTable is Office2010ColorTable) ||
                        renderer.ColorTable is Office2010ColorTable && ((Office2010ColorTable)renderer.ColorTable).ColorScheme != eOffice2010ColorScheme.VS2010)
                    {
                        ((Office2007Renderer)GlobalManager.Renderer).ColorTable = new Office2010ColorTable(eOffice2010ColorScheme.VS2010, _ColorTint);
                    }
                }
            }
            else if (style == eStyle.Windows7Blue)
            {
                // Ensure that proper color table is selected.
                if (GlobalManager.Renderer is Office2007Renderer)
                {
                    if (colorBlendChanged || !(((Office2007Renderer)GlobalManager.Renderer).ColorTable is Windows7ColorTable))
                    {
                        ((Office2007Renderer)GlobalManager.Renderer).ColorTable = new Windows7ColorTable(eWindows7ColorScheme.Blue, _ColorTint);
                    }

                }
            }
            else if (style == eStyle.Office2007Blue || style == eStyle.Office2007Silver || style == eStyle.Office2007Black || style == eStyle.Office2007VistaGlass)
            {
                // Ensure that proper color table is selected.
                if (GlobalManager.Renderer is Office2007Renderer)
                {
                    if (colorBlendChanged || (((Office2007Renderer)GlobalManager.Renderer).ColorTable is Office2010ColorTable) ||
                        (((Office2007Renderer)GlobalManager.Renderer).ColorTable is Windows7ColorTable) ||
                        (((Office2007Renderer)GlobalManager.Renderer).ColorTable.InitialColorScheme != ColorSchemeFromStyle(style)) || _PreviousStyle != style)
                    {
                        ((Office2007Renderer)GlobalManager.Renderer).ColorTable = new Office2007ColorTable(ColorSchemeFromStyle(style), _ColorTint);
                    }

                }
            }
            else if (style == eStyle.Metro || style == eStyle.OfficeMobile2014 || style == eStyle.Office2016)
            {
                DevComponents.DotNetBar.Metro.Rendering.MetroRender.UpdateColorTable(_MetroColorGeneratorParameters);
            }
            else if (style == eStyle.VisualStudio2012Light)
            {
                DevComponents.DotNetBar.Metro.Rendering.MetroRender.UpdateColorTable(DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters.VisualStudio2012Light);
            }
            else if (style == eStyle.VisualStudio2012Dark)
            {
                DevComponents.DotNetBar.Metro.Rendering.MetroRender.UpdateColorTable(DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters.VisualStudio2012Dark);
            }
        }

        private static eOffice2007ColorScheme ColorSchemeFromStyle(eStyle style)
        {
            eOffice2007ColorScheme colorScheme = eOffice2007ColorScheme.Blue;
            if (style == eStyle.Office2007Silver)
                colorScheme = eOffice2007ColorScheme.Silver;
            else if (style == eStyle.Office2007Black)
                colorScheme = eOffice2007ColorScheme.Black;
            else if (style == eStyle.Office2007VistaGlass)
                colorScheme = eOffice2007ColorScheme.VistaGlass;
            return colorScheme;
        }

        private static void NotifyOnStyleChange()
        {
            WeakReference[] references;
            StyleManager._ReadWriteClientsListLock.AcquireReaderLock(-1);
            try
            {
                references = new WeakReference[_RegisteredControls.Count];
                _RegisteredControls.CopyTo(references);
            }
            finally
            {
                StyleManager._ReadWriteClientsListLock.ReleaseReaderLock();
            }

            foreach (WeakReference reference in references)
            {
                object target = reference.Target;
                if (reference.IsAlive)
                {
                    if (target != null)
                    {
                        Control control = target as Control;
#if FRAMEWORK20
                        if (control != null && control.InvokeRequired)
                        {
                            control.Invoke(new
                                MethodInvoker(delegate() { InvokeStyleManagerStyleChanged(target, _EffectiveStyle); }));
                            if (BarFunctions.IsHandleValid(control))
                                control.Invoke(new
                                    MethodInvoker(delegate() { control.Invalidate(true); }));
                        }
                        else
#endif
                        {
                            InvokeStyleManagerStyleChanged(target, _EffectiveStyle);
                            if (control != null && BarFunctions.IsHandleValid(control))
                                control.Invalidate(true);
                        }
                    }
                }
            }
        }
        private static void OnStyleChanged(eStyle oldValue, eStyle newValue)
        {
            ChangeColorTable(newValue, !_ColorTint.IsEmpty);
            NotifyOnStyleChange();
        }

        private static void InvokeStyleManagerStyleChanged(object target, eDotNetBarStyle newStyle)
        {
            if (target == null) return;
            try
            {
                MethodInfo mi = target.GetType().GetMethod("StyleManagerStyleChanged");
                if (mi != null)
                {
                    mi.Invoke(target, new object[] { newStyle });
                }
            }
            catch { } // target is part of generics with undeclared type and other possibilities...  we don't care if it fails on borderline cases
        }

        private static ReaderWriterLock _ReadWriteClientsListLock;
        private static ArrayList _RegisteredControls = new ArrayList();
        /// <summary>
        /// Registers control with the StyleManager so control can be notified of global style changes.
        /// </summary>
        /// <param name="control">Control to register with the StyleManager.</param>
        public static void Register(Control control)
        {
            if (GetControlReference(control) == null)
            {
                WeakReference reference = new WeakReference(control);
                reference.Target = control;

                LockCookie cookie1 = new LockCookie();
                bool readerLockHeld = StyleManager._ReadWriteClientsListLock.IsReaderLockHeld;
                if (readerLockHeld)
                {
                    cookie1 = StyleManager._ReadWriteClientsListLock.UpgradeToWriterLock(-1);
                }
                else
                {
                    StyleManager._ReadWriteClientsListLock.AcquireWriterLock(-1);
                }
                try
                {
                    _RegisteredControls.Add(reference);
                }
                finally
                {
                    if (readerLockHeld)
                    {
                        StyleManager._ReadWriteClientsListLock.DowngradeFromWriterLock(ref cookie1);
                    }
                    else
                    {
                        StyleManager._ReadWriteClientsListLock.ReleaseWriterLock();
                    }
                }

                if (StyleManager.IsMetro(_Style) && ShouldUpdateAmbientProperties(control))
                    UpdateAmbientColors(control);

                control = null;
            }
        }

        private static WeakReference GetControlReference(Control control)
        {
            ArrayList registeredControls = _RegisteredControls;
            if (registeredControls == null) return null;

            StyleManager._ReadWriteClientsListLock.AcquireReaderLock(-1);
            try
            {
                foreach (WeakReference item in registeredControls)
                {
                    object target = item.Target;
                    if (target != null && target.Equals(control))
                        return item;
                }
            }
            finally
            {
                StyleManager._ReadWriteClientsListLock.ReleaseReaderLock();
            }
            return null;
        }
        /// <summary>
        /// Unregister the control from StyleManager notifications.
        /// </summary>
        /// <param name="control">Control that was registered through Register method.</param>
        public static void Unregister(Control control)
        {
            LockCookie cookie1 = new LockCookie();
            bool readerLockHeld = StyleManager._ReadWriteClientsListLock.IsReaderLockHeld;
            if (readerLockHeld)
            {
                cookie1 = StyleManager._ReadWriteClientsListLock.UpgradeToWriterLock(-1);
            }
            else
            {
                StyleManager._ReadWriteClientsListLock.AcquireWriterLock(-1);
            }
            try
            {
                ArrayList removeList = new ArrayList();
                foreach (WeakReference item in _RegisteredControls)
                {
                    object target = item.Target;

                    if (target != null && target.Equals(control))
                    {
                        removeList.Add(item);
                        //_RegisteredControls.Remove(item);
                        //break;
                    }
                    else if (target == null)
                        removeList.Add(item);
                }
                foreach (WeakReference item in removeList)
                {
                    _RegisteredControls.Remove(item);
                }
            }
            finally
            {
                if (readerLockHeld)
                {
                    StyleManager._ReadWriteClientsListLock.DowngradeFromWriterLock(ref cookie1);
                }
                else
                {
                    StyleManager._ReadWriteClientsListLock.ReleaseWriterLock();
                }
            }
        }
        private static ArrayList _RegisteredAmbients = new ArrayList();
        private static ReaderWriterLock _ReadWriteAmbientListLock;
        internal static void RegisterAmbientManager(StyleManagerAmbient ambientProvider)
        {
            if (GetAmbientReference(ambientProvider) == null)
            {
                WeakReference reference = new WeakReference(ambientProvider);

                LockCookie cookie1 = new LockCookie();
                bool readerLockHeld = StyleManager._ReadWriteAmbientListLock.IsReaderLockHeld;
                if (readerLockHeld)
                {
                    cookie1 = StyleManager._ReadWriteAmbientListLock.UpgradeToWriterLock(-1);
                }
                else
                {
                    StyleManager._ReadWriteAmbientListLock.AcquireWriterLock(-1);
                }
                try
                {
                    _RegisteredAmbients.Add(reference);
                }
                finally
                {
                    if (readerLockHeld)
                    {
                        StyleManager._ReadWriteAmbientListLock.DowngradeFromWriterLock(ref cookie1);
                    }
                    else
                    {
                        StyleManager._ReadWriteAmbientListLock.ReleaseWriterLock();
                    }
                }
                ambientProvider = null;
            }
        }
        internal static void UnregisterAmbientManager(StyleManagerAmbient ambientProvider)
        {
            LockCookie cookie1 = new LockCookie();
            bool readerLockHeld = StyleManager._ReadWriteAmbientListLock.IsReaderLockHeld;
            if (readerLockHeld)
            {
                cookie1 = StyleManager._ReadWriteAmbientListLock.UpgradeToWriterLock(-1);
            }
            else
            {
                StyleManager._ReadWriteAmbientListLock.AcquireWriterLock(-1);
            }
            try
            {

                foreach (WeakReference item in _RegisteredAmbients)
                {
                    object target = item.Target;
                    if (target != null && target.Equals(ambientProvider))
                    {
                        _RegisteredControls.Remove(item);
                        break;
                    }
                }
            }
            finally
            {
                if (readerLockHeld)
                {
                    StyleManager._ReadWriteAmbientListLock.DowngradeFromWriterLock(ref cookie1);
                }
                else
                {
                    StyleManager._ReadWriteAmbientListLock.ReleaseWriterLock();
                }
            }
        }
        private static WeakReference GetAmbientReference(StyleManagerAmbient ambientProvider)
        {
            ArrayList registeredAmbients = _RegisteredAmbients;
            if (registeredAmbients == null) return null;

            StyleManager._ReadWriteAmbientListLock.AcquireReaderLock(-1);
            try
            {
                foreach (WeakReference item in registeredAmbients)
                {
                    object target = item.Target;
                    if (target != null && target.Equals(ambientProvider))
                        return item;
                }
            }
            finally
            {
                StyleManager._ReadWriteAmbientListLock.ReleaseReaderLock();
            }
            return null;
        }

        /// <summary>
        /// Returns whether style is a Metro type style.
        /// </summary>
        /// <param name="style">style to test.</param>
        /// <returns>true if Metro type style otherwise false.</returns>
        public static bool IsMetro(eDotNetBarStyle style)
        {
            if (style == eDotNetBarStyle.StyleManagerControlled)
                return IsMetro(StyleManager.Style);
            return style == eDotNetBarStyle.Metro || style == eDotNetBarStyle.OfficeMobile2014;
        }

        /// <summary>
        /// Returns whether style is a Metro type style.
        /// </summary>
        /// <param name="style">style to test.</param>
        /// <returns>true if Metro type style otherwise false.</returns>
        public static bool IsMetro(eStyle style)
        {
            return style == eStyle.Metro || StyleManager.IsVisualStudio2012(style) || style == eStyle.OfficeMobile2014 || style == eStyle.Office2016;
        }

        /// <summary>
        /// Returns whether style is a Metro type style.
        /// </summary>
        /// <param name="style">style to test.</param>
        /// <returns>true if Metro type style otherwise false.</returns>
        internal static bool IsVisualStudio2012(eStyle style)
        {
            return style == eStyle.VisualStudio2012Light || style == eStyle.VisualStudio2012Dark;
        }
        #endregion
    }
    /// <summary>
    /// Defines the StyleManager styles.
    /// </summary>
    public enum eStyle
    {
        Office2007Blue,
        Office2007Silver,
        Office2007Black,
        Office2007VistaGlass,
        Office2010Silver,
        Office2010Blue,
        Office2010Black,
        Windows7Blue,
        VisualStudio2010Blue,
        Metro,
        Office2013 = Metro,
        VisualStudio2012Light,
        VisualStudio2012Dark,
        OfficeMobile2014,
        Office2016
    }


}
