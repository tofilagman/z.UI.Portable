using System.Drawing;

namespace DevComponents.DotNetBar.Rendering
{
    internal static class SuperTabStyleColorFactory
    {
        #region GetColorTable

        public static SuperTabColorTable GetColorTable(eSuperTabStyle style, ColorFactory factory)
        {
            switch (style)
            {
                case eSuperTabStyle.Office2007:
                    return (GetOffice2007ColorTable());

                case eSuperTabStyle.Office2010BackstageBlue:
                    return (GetOffice2010BackstageColorTable());

                case eSuperTabStyle.OneNote2007:
                    return (GetOneNote2007ColorTable(factory));

                case eSuperTabStyle.VisualStudio2008Dock:
                    return (GetVisualStudio2008DockColorTable(factory));

                case eSuperTabStyle.VisualStudio2008Document:
                    return (GetVisualStudio2008DocumentColorTable(factory));

                case eSuperTabStyle.WinMediaPlayer12:
                    return (GetWinMediaPlayer12ColorTable(factory));

                case eSuperTabStyle.OfficeMobile2014:
                    return (GetOfficeMobile2014ColorTable(factory));
            }

            return (GetOffice2007ColorTable());
        }

        #endregion

        #region GetItemColorTable

        public static SuperTabItemColorTable GetItemColorTable(eSuperTabStyle style, ColorFactory factory)
        {
            switch (style)
            {
                case eSuperTabStyle.Office2007:
                    return (GetOffice2007ItemColorTable());

                case eSuperTabStyle.Office2010BackstageBlue:
                    return (GetOffice2010BackstageItemColorTable());

                case eSuperTabStyle.OneNote2007:
                    return (GetOneNote2007ItemColorTable(factory));

                case eSuperTabStyle.VisualStudio2008Dock:
                    return (GetVisualStudio2008DockItemColorTable(factory));

                case eSuperTabStyle.VisualStudio2008Document:
                    return (GetVisualStudio2008DocumentItemColorTable(factory));

                case eSuperTabStyle.WinMediaPlayer12:
                    return (GetWinMediaPlayer12ItemColorTable(factory));

                case eSuperTabStyle.OfficeMobile2014:
                    return (GetOfficeMobile2014ItemColorTable(factory));
            }

            return (GetOffice2007ItemColorTable());
        }

        #endregion

        #region GetPanelColorTable

        public static SuperTabPanelColorTable GetPanelColorTable(eSuperTabStyle style, ColorFactory factory)
        {
            switch (style)
            {
                case eSuperTabStyle.Office2007:
                    return (GetOffice2007PanelColorTable());

                case eSuperTabStyle.Office2010BackstageBlue:
                    return (GetOffice2010BackstagePanelColorTable());

                case eSuperTabStyle.OneNote2007:
                    return (GetOneNote2007PanelColorTable(factory));

                case eSuperTabStyle.VisualStudio2008Dock:
                    return (GetVisualStudio2008DockPanelColorTable(factory));

                case eSuperTabStyle.VisualStudio2008Document:
                    return (GetVisualStudio2008DocumentPanelColorTable(factory));

                case eSuperTabStyle.WinMediaPlayer12:
                    return (GetWinMediaPlayer12PanelColorTable(factory));

                case eSuperTabStyle.OfficeMobile2014:
                    return (GetOfficeMobile2014PanelColorTable(factory));
           }

            return (GetOffice2007PanelColorTable());
        }

        #endregion

        #region Office2007

        #region GetOffice2007ColorTable

        private static SuperTabColorTable GetOffice2007ColorTable()
        {
            Office2007Renderer r = GlobalManager.Renderer as Office2007Renderer;

            return (r != null ?
                r.ColorTable.SuperTab : new SuperTabColorTable());
        }

        #endregion

        #region GetOffice2007ItemColorTable

        private static SuperTabItemColorTable GetOffice2007ItemColorTable()
        {
            Office2007Renderer r = GlobalManager.Renderer as Office2007Renderer;

            return (r != null
                        ? r.ColorTable.SuperTabItem
                        : new SuperTabItemColorTable());
        }

        #endregion

        #region GetOffice2007PanelColorTable

        private static SuperTabPanelColorTable GetOffice2007PanelColorTable()
        {
            Office2007Renderer r = GlobalManager.Renderer as Office2007Renderer;

            return (r != null
                        ? r.ColorTable.SuperTabPanel
                        : new SuperTabPanelColorTable());
        }

        #endregion

        #endregion

        #region Office2010Backstage

        #region Office2010BackstageColorTable

        #region GetOffice2010BackstageColorTable

        /// <summary>
        /// GetOffice2010BackstageColorTable
        /// </summary>
        /// <returns>Office2010BackstageColorTable</returns>
        private static SuperTabColorTable GetOffice2010BackstageColorTable()
        {
            Office2007Renderer r = GlobalManager.Renderer as Office2007Renderer;

            return (r != null ?
                r.ColorTable.Backstage : new SuperTabColorTable());
        }

        #endregion

        #region GetOffice2010BackstageBlueColorTable

        /// <summary>
        /// GetOffice2010BackstageBlueColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageBlueColorTable</returns>
        internal static SuperTabColorTable GetOffice2010BackstageBlueColorTable(
            SuperTabColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabColorTable();

            ct.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0xE4EEF8), factory.GetColor(0xBBD0EA));

            ct.InnerBorder = factory.GetColor(0xABBCCD);
            ct.OuterBorder = factory.GetColor(0xCAD5E4);

            ct.ControlBoxDefault.Image = factory.GetColor(0x000000);

            ct.ControlBoxMouseOver.Background = factory.GetColor(0xCEEDFA);
            ct.ControlBoxMouseOver.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxMouseOver.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxPressed.Background = factory.GetColor(0xB7CAE0);
            ct.ControlBoxPressed.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxPressed.Image = factory.GetColor(0xFF, 0x000000);

            ct.InsertMarker = factory.GetColor(0xFF, 0x000000);
            ct.SelectionMarker = factory.GetColor(0xFF, 0x000000);

            return (ct);
        }

        #endregion

        #region GetOffice2010BackstageBlueColorTable

        /// <summary>
        /// GetVS2010BackstageBlueColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageBlueColorTable</returns>
        internal static SuperTabColorTable GetVS2010BackstageBlueColorTable(
            SuperTabColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabColorTable();

            ct.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0xCAD3E2), factory.GetColor(0xAEB9CD));

            ct.InnerBorder = factory.GetColor(0xABBCCD);
            ct.OuterBorder = factory.GetColor(0xCAD5E4);

            ct.ControlBoxDefault.Image = factory.GetColor(0x000000);

            ct.ControlBoxMouseOver.Background = factory.GetColor(0xCEEDFA);
            ct.ControlBoxMouseOver.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxMouseOver.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxPressed.Background = factory.GetColor(0xB7CAE0);
            ct.ControlBoxPressed.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxPressed.Image = factory.GetColor(0xFF, 0x000000);

            ct.InsertMarker = factory.GetColor(0xFF, 0x000000);
            ct.SelectionMarker = factory.GetColor(0xFF, 0x000000);

            return (ct);
        }

        #endregion

        #region GetOffice2010BackstageBlackColorTable

        /// <summary>
        /// GetOffice2010BackstageBlackColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageBlackColorTable</returns>
        internal static SuperTabColorTable GetOffice2010BackstageBlackColorTable(
            SuperTabColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabColorTable();

            ct.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0x717171), factory.GetColor(0x3A3A3A));

            ct.InnerBorder = factory.GetColor(0x2C2C2C);
            ct.OuterBorder = factory.GetColor(0x444444);

            ct.ControlBoxDefault.Image = factory.GetColor(0xFFFFFF);

            ct.ControlBoxMouseOver.Background = factory.GetColor(0xCEEDFA);
            ct.ControlBoxMouseOver.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxMouseOver.Image = factory.GetColor(0x000000);

            ct.ControlBoxPressed.Background = factory.GetColor(0xB7CAE0);
            ct.ControlBoxPressed.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxPressed.Image = factory.GetColor(0x000000);

            ct.InsertMarker = factory.GetColor(0xFFFFFF);
            ct.SelectionMarker = factory.GetColor(0xFFFFFF);

            return (ct);
        }

        #endregion

        #region GetOffice2010BackstageSilverColorTable

        /// <summary>
        /// GetOffice2010BackstageSilverColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageSilverColorTable</returns>
        internal static SuperTabColorTable GetOffice2010BackstageSilverColorTable(
            SuperTabColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabColorTable();

            ct.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0xFBFCFD), factory.GetColor(0xE0E3E7));

            ct.InnerBorder = factory.GetColor(0xCCCDCE);
            ct.OuterBorder = factory.GetColor(0xE4E6E8);

            ct.ControlBoxDefault.Image = factory.GetColor(0x000000);

            ct.ControlBoxMouseOver.Background = factory.GetColor(0xCEEDFA);
            ct.ControlBoxMouseOver.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxMouseOver.Image = factory.GetColor(0x000000);

            ct.ControlBoxPressed.Background = factory.GetColor(0xB7CAE0);
            ct.ControlBoxPressed.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxPressed.Image = factory.GetColor(0x000000);

            ct.InsertMarker = factory.GetColor(0x000000);
            ct.SelectionMarker = factory.GetColor(0x000000);

            return (ct);
        }

        #endregion

        #region GetOffice2007BackstageVistaGlassColorTable

        /// <summary>
        /// GetOffice2007BackstageVistaGlassColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageVistaGlassColorTable</returns>
        internal static SuperTabColorTable GetOffice2007BackstageVistaGlassColorTable(
            SuperTabColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabColorTable();

            ct.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0x2F3A4F), factory.GetColor(0x3F4F6E));

            ct.InnerBorder = Color.Empty;
            ct.OuterBorder = factory.GetColor(0x3B4A67);

            ct.ControlBoxDefault.Image = factory.GetColor(0xFFFFFF);

            ct.ControlBoxMouseOver.Background = factory.GetColor(0xCEEDFA);
            ct.ControlBoxMouseOver.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxMouseOver.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxPressed.Background = factory.GetColor(0xB7CAE0);
            ct.ControlBoxPressed.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxPressed.Image = factory.GetColor(0xFF, 0x000000);

            ct.InsertMarker = factory.GetColor(0xFF, 0xFFFFFF);
            ct.SelectionMarker = factory.GetColor(0xFF, 0xFFFFFF);

            return (ct);
        }

        #endregion

        #region GetMetroBackstageColorTable

        /// <summary>
        /// GetOffice2010BackstageSilverColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageSilverColorTable</returns>
        internal static SuperTabColorTable GetMetroBackstageColorTable(
            SuperTabColorTable ct, ColorFactory factory, DevComponents.DotNetBar.Metro.ColorTables.MetroPartColors metroColors)
        {
            if (ct == null)
                ct = new SuperTabColorTable();

            ct.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(metroColors.BaseColor));

            ct.InnerBorder = Color.Empty;// factory.GetColor(metroColors.BaseColor);
            ct.OuterBorder = Color.Empty; // factory.GetColor(metroColors.BaseColor);

            ct.ControlBoxDefault.Image = factory.GetColor(metroColors.BaseTextColor);

            ct.ControlBoxMouseOver.Background = factory.GetColor(metroColors.BaseColorDark);
            ct.ControlBoxMouseOver.Border = factory.GetColor(metroColors.BaseColorDark);
            ct.ControlBoxMouseOver.Image = factory.GetColor(metroColors.BaseTextColor);

            ct.ControlBoxPressed.Background = factory.GetColor(metroColors.BaseColorDark);
            ct.ControlBoxPressed.Border = factory.GetColor(metroColors.BaseColorDark);
            ct.ControlBoxPressed.Image = factory.GetColor(metroColors.BaseTextColor);

            ct.InsertMarker = factory.GetColor(metroColors.BaseTextColor);
            ct.SelectionMarker = Color.Empty; // factory.GetColor(metroColors.BaseTextColor);

            return (ct);
        }

        #endregion

        #endregion

        #region GetOffice2010BackstageItemColorTable

        #region GetOffice2010BackstageItemColorTable

        /// <summary>
        /// GetOffice2010BackstageItemColorTable
        /// </summary>
        /// <returns>Office2010BackstageItemColorTable</returns>
        private static SuperTabItemColorTable GetOffice2010BackstageItemColorTable()
        {
            Office2007Renderer r = GlobalManager.Renderer as Office2007Renderer;

            return (r != null
                        ? r.ColorTable.BackstageItem
                        : new SuperTabItemColorTable());
        }

        #endregion

        #region GetOffice2010BackstageBlueItemColorTable

        /// <summary>
        /// GetOffice2010BackstageBlueItemColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageBlueItemColorTable</returns>
        internal static SuperTabItemColorTable GetOffice2010BackstageBlueItemColorTable(
            SuperTabItemColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabItemColorTable();

            // Top Default

            ct.Default.Normal.Text = factory.GetColor(0x000000);
            ct.Default.Normal.Background.AdaptiveGradient = false;
            ct.Default.Normal.CloseMarker = factory.GetColor(0x000000);

            // Top Selected

            ct.Default.Selected.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0x094DAD));

            ct.Default.Selected.Background.AdaptiveGradient = false;
            ct.Default.Selected.InnerBorder = Color.Empty;
            ct.Default.Selected.OuterBorder = factory.GetColor(0x073F97);
            ct.Default.Selected.Text = factory.GetColor(0xFFFFFF);
            ct.Default.Selected.CloseMarker = factory.GetColor(0xFFFFFF);
            ct.Default.Selected.SelectionMarker = factory.GetColor(0xFFFFFF);

            // Top SelectedMouseOver

            ct.Default.SelectedMouseOver = ct.Default.Selected;

            // Top MouseOver

            ct.Default.MouseOver.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xDDEBFA));
            ct.Default.MouseOver.Background.AdaptiveGradient = false;
            ct.Default.MouseOver.InnerBorder = factory.GetColor(0xF0F5FC);
            ct.Default.MouseOver.OuterBorder = factory.GetColor(0x527DE0);
            ct.Default.MouseOver.Text = factory.GetColor(0x000000);
            ct.Default.MouseOver.CloseMarker = factory.GetColor(0x000000);

            // Top Disabled

            ct.Default.Disabled.Text = factory.GetColor(0xE0E0E0);
            ct.Default.Disabled.Background.AdaptiveGradient = false;
            ct.Default.Disabled.CloseMarker = factory.GetColor(0xE0E0E0);

            // Left, Bottom, Right

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Right;

            return (ct);
        }

        #endregion

        #region GetOffice2010BackstageBlackItemColorTable

        /// <summary>
        /// GetOffice2010BackstageBlackItemColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageBlackItemColorTable</returns>
        internal static SuperTabItemColorTable GetOffice2010BackstageBlackItemColorTable(
            SuperTabItemColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabItemColorTable();

            // Top Default

            ct.Default.Normal.Text = factory.GetColor(0xFFFFFF);
            ct.Default.Normal.Background.AdaptiveGradient = false;
            ct.Default.Normal.CloseMarker = factory.GetColor(0xFFFFFF);

            // Top Selected

            ct.Default.Selected.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0x094DAD));

            ct.Default.Selected.Background.AdaptiveGradient = false;
            ct.Default.Selected.InnerBorder = Color.Empty;
            ct.Default.Selected.OuterBorder = factory.GetColor(0x073F97);
            ct.Default.Selected.Text = factory.GetColor(0xFFFFFF);
            ct.Default.Selected.CloseMarker = factory.GetColor(0xFFFFFF);
            ct.Default.Selected.SelectionMarker = factory.GetColor(0xFFFFFF);

            // Top SelectedMouseOver

            ct.Default.SelectedMouseOver = ct.Default.Selected;

            // Top MouseOver

            ct.Default.MouseOver.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0x54657D));
            ct.Default.MouseOver.Background.AdaptiveGradient = false;
            ct.Default.MouseOver.InnerBorder = factory.GetColor(0x55667E);
            ct.Default.MouseOver.OuterBorder = factory.GetColor(0x4991F5);
            ct.Default.MouseOver.Text = factory.GetColor(0xFFFFFF);
            ct.Default.MouseOver.CloseMarker = factory.GetColor(0xFFFFFF);

            // Left, Bottom, Right

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Right;

            return (ct);
        }

        #endregion

        #region GetOffice2010BackstageSilverItemColorTable

        /// <summary>
        /// GetOffice2010BackstageSilverItemColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageSilverItemColorTable</returns>
        internal static SuperTabItemColorTable GetOffice2010BackstageSilverItemColorTable(
            SuperTabItemColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabItemColorTable();

            // Top Default

            ct.Default.Normal.Text = factory.GetColor(0x000000);
            ct.Default.Normal.Background.AdaptiveGradient = false;
            ct.Default.Normal.CloseMarker = factory.GetColor(0x000000);

            // Top Selected

            ct.Default.Selected.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0x094DAD));

            ct.Default.Selected.Background.AdaptiveGradient = false;
            ct.Default.Selected.InnerBorder = Color.Empty;
            ct.Default.Selected.OuterBorder = factory.GetColor(0x073F97);
            ct.Default.Selected.Text = factory.GetColor(0xFFFFFF);
            ct.Default.Selected.CloseMarker = factory.GetColor(0xFFFFFF);
            ct.Default.Selected.SelectionMarker = factory.GetColor(0xFFFFFF);

            // Top SelectedMouseOver

            ct.Default.SelectedMouseOver = ct.Default.Selected;

            // Top MouseOver

            ct.Default.MouseOver.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xDDEBFA));
            ct.Default.MouseOver.Background.AdaptiveGradient = false;
            ct.Default.MouseOver.InnerBorder = factory.GetColor(0xF0F5FC);
            ct.Default.MouseOver.OuterBorder = factory.GetColor(0x527DE0);
            ct.Default.MouseOver.Text = factory.GetColor(0x000000);
            ct.Default.MouseOver.CloseMarker = factory.GetColor(0x000000);

            // Left, Bottom, Right

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Right;

            return (ct);
        }

        #endregion

        #region GetOffice2007BackstageVistaGlassItemColorTable

        /// <summary>
        /// GetOffice2007BackstageVistaGlassItemColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageVistaGlassItemColorTable</returns>
        internal static SuperTabItemColorTable GetOffice2007BackstageVistaGlassItemColorTable(
            SuperTabItemColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabItemColorTable();

            // Top Default

            ct.Default.Normal.Text = factory.GetColor(0xFFFFFF);
            ct.Default.Normal.Background.AdaptiveGradient = false;
            ct.Default.Normal.CloseMarker = factory.GetColor(0xFFFFFF);

            // Top Selected

            ct.Default.Selected.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0x597FDF), factory.GetColor(0x2E57AF));

            ct.Default.Selected.Background.AdaptiveGradient = false;
            ct.Default.Selected.InnerBorder = factory.GetColor(0x6889EC);
            ct.Default.Selected.OuterBorder = factory.GetColor(0x242D3D);
            ct.Default.Selected.Text = factory.GetColor(0xFFFFFF);
            ct.Default.Selected.CloseMarker = factory.GetColor(0xFFFFFF);
            ct.Default.Selected.SelectionMarker = factory.GetColor(0xFFFFFF);

            // Top SelectedMouseOver

            ct.Default.SelectedMouseOver.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0x597FDF), factory.GetColor(0x2E57AF));

            ct.Default.SelectedMouseOver.Background.AdaptiveGradient = false;
            ct.Default.SelectedMouseOver.InnerBorder = factory.GetColor(0x6889EC);
            ct.Default.SelectedMouseOver.OuterBorder = factory.GetColor(0x242D3D);
            ct.Default.SelectedMouseOver.Text = factory.GetColor(0xFFFFFF);
            ct.Default.SelectedMouseOver.CloseMarker = factory.GetColor(0xFFFFFF);
            ct.Default.SelectedMouseOver.SelectionMarker = factory.GetColor(0xFFFFFF);

            // Top MouseOver

            ct.Default.MouseOver.Background = new SuperTabLinearGradientColorTable(
                new Color[] { factory.GetColor(0x3B4F7C), factory.GetColor(0x364872), factory.GetColor(0x364872), factory.GetColor(0x293857) },
                new float[] { 0, 095f, .95f, 1 });

            ct.Default.MouseOver.Background.AdaptiveGradient = false;
            ct.Default.MouseOver.InnerBorder = factory.GetColor(0x739AFF);
            ct.Default.MouseOver.OuterBorder = factory.GetColor(0x0B1E54);
            ct.Default.MouseOver.Text = factory.GetColor(0xFFFFFF);
            ct.Default.MouseOver.CloseMarker = factory.GetColor(0xFFFFFF);

            // Left, Bottom, Right

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Right;

            return (ct);
        }

        #endregion

        #region GetMetroBackstageItemColorTable

        /// <summary>
        /// GetOffice2010BackstageSilverItemColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <param name="metroColors"></param>
        /// <returns>Office2010BackstageSilverItemColorTable</returns>
        internal static SuperTabItemColorTable GetMetroBackstageItemColorTable(
            SuperTabItemColorTable ct, ColorFactory factory, Metro.ColorTables.MetroPartColors metroColors)
        {
            if (ct == null)
                ct = new SuperTabItemColorTable();

            // Top Default

            ct.Default.Normal.Text = factory.GetColor(metroColors.BaseTextColor);
            ct.Default.Normal.Background.AdaptiveGradient = false;
            ct.Default.Normal.CloseMarker = factory.GetColor(metroColors.BaseTextColor);

            // Top Selected

            ct.Default.Selected.Background = new SuperTabLinearGradientColorTable(factory.GetColor(metroColors.BaseColorLight1));
            ct.Default.Selected.Background.AdaptiveGradient = false;
            ct.Default.Selected.InnerBorder = Color.Empty;
            ct.Default.Selected.OuterBorder = Color.Empty;
            ct.Default.Selected.Text = factory.GetColor(metroColors.BaseTextColor);
            ct.Default.Selected.CloseMarker = factory.GetColor(metroColors.BaseTextColor);
            ct.Default.Selected.SelectionMarker = Color.Empty;

            // Top SelectedMouseOver
            ct.Default.SelectedMouseOver = new SuperTabItemStateColorTable();
            ct.Default.SelectedMouseOver.Background = new SuperTabLinearGradientColorTable(factory.GetColor(metroColors.BaseColorDark));
            ct.Default.SelectedMouseOver.Background.AdaptiveGradient = false;
            ct.Default.SelectedMouseOver.InnerBorder = Color.Empty;
            ct.Default.SelectedMouseOver.OuterBorder = Color.Empty;
            ct.Default.SelectedMouseOver.Text = factory.GetColor(metroColors.BaseTextColor);
            ct.Default.SelectedMouseOver.CloseMarker = factory.GetColor(metroColors.BaseTextColor);
            ct.Default.SelectedMouseOver.SelectionMarker = Color.Empty;
            
            // Top MouseOver

            ct.Default.MouseOver.Background = new SuperTabLinearGradientColorTable(factory.GetColor(metroColors.BaseColorDark));
            ct.Default.MouseOver.Background.AdaptiveGradient = false;
            ct.Default.MouseOver.InnerBorder = Color.Empty;
            ct.Default.MouseOver.OuterBorder = Color.Empty;
            ct.Default.MouseOver.Text = factory.GetColor(metroColors.BaseTextColor);
            ct.Default.MouseOver.CloseMarker = factory.GetColor(metroColors.BaseTextColor);

            // Top Disabled

            ct.Default.Disabled.Text = factory.GetColor(metroColors.TextDisabledColor);
            ct.Default.Disabled.Background.AdaptiveGradient = false;
            ct.Default.Disabled.CloseMarker = factory.GetColor(metroColors.TextDisabledColor);

            // Left, Bottom, Right

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Right;

            return (ct);
        }

        #endregion

        #endregion

        #region GetOffice2010BackstagePanelColorTable

        #region GetOffice2010BackstagePanelColorTable

        /// <summary>
        /// GetOffice2010BackstagePanelColorTable
        /// </summary>
        /// <returns>Office2010BackstagePanelColorTable</returns>
        private static SuperTabPanelColorTable GetOffice2010BackstagePanelColorTable()
        {
            Office2007Renderer r = GlobalManager.Renderer as Office2007Renderer;

            return (r != null
                        ? r.ColorTable.BackstagePanel
                        : new SuperTabPanelColorTable());
        }

        #endregion

        #region GetOffice2010BackstageBluePanelColorTable

        /// <summary>
        /// GetOffice2010BackstageBluePanelColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageBluePanelColorTable</returns>
        internal static SuperTabPanelColorTable GetOffice2010BackstageBluePanelColorTable(
            SuperTabPanelColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabPanelColorTable();

            ct.Default.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0xFFFFFF), factory.GetColor(0xFFFFFF));

            ct.Default.InnerBorder = Color.Empty;
            ct.Default.OuterBorder = factory.GetColor(0xABBCCD);
            ct.Default.Background.AdaptiveGradient = false;

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Default;

            return (ct);
        }

        #endregion

        #region GetOffice2010BackstageBlackPanelColorTable

        /// <summary>
        /// GetOffice2010BackstageBlackPanelColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageBlackPanelColorTable</returns>
        internal static SuperTabPanelColorTable GetOffice2010BackstageBlackPanelColorTable(
            SuperTabPanelColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabPanelColorTable();

            ct.Default.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0xFFFFFF), factory.GetColor(0xFFFFFF));

            ct.Default.InnerBorder = Color.Empty;
            ct.Default.OuterBorder = factory.GetColor(0x2C2C2C);
            ct.Default.Background.AdaptiveGradient = false;

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Default;

            return (ct);
        }

        #endregion

        #region GetOffice2010BackstageSilverPanelColorTable

        /// <summary>
        /// GetOffice2010BackstageSilverPanelColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageSilverPanelColorTable</returns>
        internal static SuperTabPanelColorTable GetOffice2010BackstageSilverPanelColorTable(
            SuperTabPanelColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabPanelColorTable();

            ct.Default.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0xFFFFFF), factory.GetColor(0xFFFFFF));

            ct.Default.InnerBorder = Color.Empty;
            ct.Default.OuterBorder = factory.GetColor(0xCCCDCE);
            ct.Default.Background.AdaptiveGradient = false;

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Default;

            return (ct);
        }

        #endregion

        #region GetOffice2007BackstageVistaGlassPanelColorTable

        /// <summary>
        /// GetOffice2007BackstageVistaGlassPanelColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageVistaGlassPanelColorTable</returns>
        internal static SuperTabPanelColorTable GetOffice2007BackstageVistaGlassPanelColorTable(
            SuperTabPanelColorTable ct, ColorFactory factory)
        {
            if (ct == null)
                ct = new SuperTabPanelColorTable();

            ct.Default.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0xFFFFFF), factory.GetColor(0xFFFFFF));

            ct.Default.OuterBorder = factory.GetColor(0x3F4F6E);
            ct.Default.InnerBorder = factory.GetColor(0x3F4F6E);
            ct.Default.Background.AdaptiveGradient = false;

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Default;

            return (ct);
        }

        #endregion

        #region GetMetroBackstagePanelColorTable

        /// <summary>
        /// GetOffice2010BackstageSilverPanelColorTable
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="factory"></param>
        /// <returns>Office2010BackstageSilverPanelColorTable</returns>
        internal static SuperTabPanelColorTable GetMetroBackstagePanelColorTable(
            SuperTabPanelColorTable ct, ColorFactory factory, DevComponents.DotNetBar.Metro.ColorTables.MetroPartColors metroColors)
        {
            if (ct == null)
                ct = new SuperTabPanelColorTable();

            ct.Default.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(metroColors.CanvasColor), factory.GetColor(metroColors.CanvasColor));

            ct.Default.InnerBorder = Color.Empty;
            ct.Default.OuterBorder = factory.GetColor(metroColors.CanvasColor);
            ct.Default.Background.AdaptiveGradient = false;

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Default;

            return (ct);
        }

        #endregion

        #endregion

        #endregion

        #region OneNote2007

        #region GetOneNote2007ColorTable

        private static SuperTabColorTable GetOneNote2007ColorTable(ColorFactory factory)
        {
            SuperTabColorTable ct = new SuperTabColorTable();

            ct.Background = new SuperTabLinearGradientColorTable(Color.White);
            ct.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.OuterBorder = factory.GetColor(0x7C7C94);

            ct.ControlBoxDefault.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxMouseOver.Background = factory.GetColor(0xFFE7A2);
            ct.ControlBoxMouseOver.Border = factory.GetColor(0xFFBD69);
            ct.ControlBoxMouseOver.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxPressed.Background = factory.GetColor(0xFB8C3C);
            ct.ControlBoxPressed.Border = factory.GetColor(0xFFBD69);
            ct.ControlBoxPressed.Image = factory.GetColor(0xFF, 0x000000);

            ct.InsertMarker = factory.GetColor(0xFF, 0x000080);

            return (ct);
        }

        #endregion

        #region GetOneNote2007ItemColorTable

        private static SuperTabItemColorTable GetOneNote2007ItemColorTable(ColorFactory factory)
        {
            SuperTabItemColorTable ct = new SuperTabItemColorTable();

            // Top
            // Default

            ct.Default.Normal = new SuperTabItemStateColorTable();

            ct.Default.Normal.Background = new SuperTabLinearGradientColorTable(
                new Color[] { factory.GetColor(0xE1E9F9), factory.GetColor(0xC8D6F3), factory.GetColor(0x96B1E7), factory.GetColor(0x96B1E7) },
                new float[] { 0, .5f, .5f, 1 });

            ct.Default.Normal.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.Normal.OuterBorder = factory.GetColor(0x767492);
            ct.Default.Normal.Text = factory.GetColor(0xFF, 0x000000);
            ct.Default.Normal.CloseMarker = factory.GetColor(0x406F9F);

            // Selected

            ct.Default.Selected = new SuperTabItemStateColorTable();

            ct.Default.Selected.Background = new SuperTabLinearGradientColorTable(
                new Color[] { factory.GetColor(0xF0F4FB), factory.GetColor(0xDBE4F7), factory.GetColor(0xC5D4F2), factory.GetColor(0xC5D4F2) },
                new float[] { 0, .5f, .5f, 1 });

            ct.Default.Selected.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.Selected.OuterBorder = factory.GetColor(0x7C7C94);
            ct.Default.Selected.Text = factory.GetColor(0xFF, 0x000000);
            ct.Default.Selected.CloseMarker = factory.GetColor(0x406F9F);

            // Selected MouseOver

            ct.Default.SelectedMouseOver = new SuperTabItemStateColorTable();

            ct.Default.SelectedMouseOver.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0xFFFBF0), factory.GetColor(0xFFF0C8));

            ct.Default.SelectedMouseOver.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.SelectedMouseOver.OuterBorder = factory.GetColor(0x4B4B6F);
            ct.Default.SelectedMouseOver.Text = factory.GetColor(0xFF, 0x000000);
            ct.Default.SelectedMouseOver.CloseMarker = factory.GetColor(0x406F9F);

            // MouseOver

            ct.Default.MouseOver = new SuperTabItemStateColorTable();

            ct.Default.MouseOver.Background = new SuperTabLinearGradientColorTable(
                factory.GetColor(0xFFFBF0), factory.GetColor(0xFFF0C8));

            ct.Default.MouseOver.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.MouseOver.OuterBorder = factory.GetColor(0x4B4B6F);
            ct.Default.MouseOver.Text = factory.GetColor(0xFF, 0x000000);
            ct.Default.MouseOver.CloseMarker = factory.GetColor(0x406F9F);

            // Left
            // Default

            ct.Left.Normal = new SuperTabItemStateColorTable();

            ct.Left.Normal.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xE1E9F9), factory.GetColor(0x96B1E7));
            ct.Left.Normal.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Left.Normal.OuterBorder = factory.GetColor(0x767492);
            ct.Left.Normal.Text = factory.GetColor(0xFF, 0x000000);
            ct.Left.Normal.CloseMarker = factory.GetColor(0x406F9F);

            // Selected

            ct.Left.Selected = new SuperTabItemStateColorTable();

            ct.Left.Selected.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xF0F4FB), factory.GetColor(0xC5D4F2));
            ct.Left.Selected.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Left.Selected.OuterBorder = factory.GetColor(0x7C7C94);
            ct.Left.Selected.Text = factory.GetColor(0xFF, 0x000000);
            ct.Left.Selected.CloseMarker = factory.GetColor(0x406F9F);

            // SelectedMouseOver

            ct.Left.SelectedMouseOver = ct.Default.SelectedMouseOver;

            // MouseOver

            ct.Left.MouseOver = ct.Default.MouseOver;

            // Bottom, Right

            ct.Bottom = ct.Default;
            ct.Right = ct.Left;

            return (ct);
        }

        #endregion

        #region GetOneNote2007PanelColorTable

        private static SuperTabPanelColorTable GetOneNote2007PanelColorTable(ColorFactory factory)
        {
            SuperTabPanelColorTable ct = new SuperTabPanelColorTable();

            ct.Default.Background = new SuperTabLinearGradientColorTable(
                new Color[] { factory.GetColor(0xC4D3F1), factory.GetColor(0x8AA8E4), factory.GetColor(0x8AA8E4), factory.GetColor(0xC4D3F1) },
                new float[] { 0f, .5f, .5f, 1 });

            ct.Default.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.OuterBorder = factory.GetColor(0x7C7C94);

            ct.Left.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xC4D3F1), factory.GetColor(0xC4D3F1));
            ct.Left.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Left.OuterBorder = factory.GetColor(0x7C7C94);

            ct.Bottom = ct.Default;
            ct.Right = ct.Left;

            return (ct);
        }

        #endregion

        #endregion

        #region OfficeMobile2014

        #region GetOfficeMobile2014ColorTable

        private static SuperTabColorTable GetOfficeMobile2014ColorTable(ColorFactory factory)
        {
            SuperTabColorTable ct = new SuperTabColorTable();

            ct.InnerBorder = Color.Empty;
            ct.OuterBorder = Color.Empty;
            ct.Background = new SuperTabLinearGradientColorTable(Color.White);

            ct.ControlBoxDefault.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxMouseOver.Background = factory.GetColor(0xFFE7A2);
            ct.ControlBoxMouseOver.Border = factory.GetColor(0xFFBD69);
            ct.ControlBoxMouseOver.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxPressed.Background = factory.GetColor(0xFB8C3C);
            ct.ControlBoxPressed.Border = factory.GetColor(0xFFBD69);
            ct.ControlBoxPressed.Image = factory.GetColor(0xFF, 0x000000);

            ct.InsertMarker = factory.GetColor(0xFF, 0x000080);

            return (ct);
        }

        #endregion

        #region GetOfficeMobile2014ItemColorTable

        private static SuperTabItemColorTable GetOfficeMobile2014ItemColorTable(ColorFactory factory)
        {
            return (GetPredefinedTabColors(eTabItemColor.OfficeMobile2014Teal, factory));
        }

        #endregion

        #region GetOfficeMobile2014PanelColorTable

        private static SuperTabPanelColorTable GetOfficeMobile2014PanelColorTable(ColorFactory factory)
        {
            SuperTabPanelColorTable ct = new SuperTabPanelColorTable();

            SuperTabLinearGradientColorTable lgt =
                GetPredefinedSelectedBackground(eTabItemColor.OfficeMobile2014Teal, factory);

            Color color = (lgt.Colors != null && lgt.Colors.Length > 0)
                ? lgt.Colors[lgt.Colors.Length - 1] : Color.DimGray;

            ct.Default.InnerBorder = color;
            ct.Default.OuterBorder = color;
            ct.Default.Background = new SuperTabLinearGradientColorTable(Color.White);

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Left;

            return (ct);
        }

        #endregion

        #endregion

        #region VisualStudio2008Dock

        #region GetVisualStudio2008DockColorTable

        private static SuperTabColorTable GetVisualStudio2008DockColorTable(ColorFactory factory)
        {
            SuperTabColorTable ct = new SuperTabColorTable();

            ct.Background = new SuperTabLinearGradientColorTable(Color.White);
            ct.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.OuterBorder = factory.GetColor(0x7C7C94);

            ct.ControlBoxDefault.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxMouseOver.Background = factory.GetColor(0xCEEDFA);
            ct.ControlBoxMouseOver.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxMouseOver.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxPressed.Background = factory.GetColor(0xB7CAE0);
            ct.ControlBoxPressed.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxPressed.Image = factory.GetColor(0xFF, 0x000000);

            ct.InsertMarker = factory.GetColor(0xFF, 0x000080);

            return (ct);
        }

        #endregion

        #region GetVisualStudio2008DockItemColorTable

        private static SuperTabItemColorTable GetVisualStudio2008DockItemColorTable(ColorFactory factory)
        {
            SuperTabItemColorTable ct = new SuperTabItemColorTable();

            // Top
            // Default

            ct.Default.Normal = new SuperTabItemStateColorTable();

            ct.Default.Normal.Background = new SuperTabLinearGradientColorTable(
                new Color[] { factory.GetColor(0xF2F2F2), factory.GetColor(0xEBEBEB), factory.GetColor(0xDDDDDD), factory.GetColor(0xCFCFCF) },
                new float[] { 0, .5f, .5f, 1 });

            ct.Default.Normal.InnerBorder = factory.GetColor(0xFCFCFC);
            ct.Default.Normal.OuterBorder = factory.GetColor(0x898C95);
            ct.Default.Normal.Text = factory.GetColor(0x555564);
            ct.Default.Normal.CloseMarker = factory.GetColor(0x1A4875);

            // Selected

            ct.Default.Selected = new SuperTabItemStateColorTable();

            ct.Default.Selected.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xFFFFFF));
            ct.Default.Selected.InnerBorder = Color.Empty;
            ct.Default.Selected.OuterBorder = factory.GetColor(0x898C95);
            ct.Default.Selected.Text = factory.GetColor(0xFF, 0x000000);
            ct.Default.Selected.CloseMarker = factory.GetColor(0x1A4875);

            // SelectedMouseOver

            ct.Default.SelectedMouseOver = new SuperTabItemStateColorTable();

            ct.Default.SelectedMouseOver.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xFFFFFF));
            ct.Default.SelectedMouseOver.InnerBorder = Color.Empty;
            ct.Default.SelectedMouseOver.OuterBorder = factory.GetColor(0x898C95);
            ct.Default.SelectedMouseOver.Text = factory.GetColor(0xFF, 0x000000);
            ct.Default.SelectedMouseOver.CloseMarker = factory.GetColor(0x1A4875);

            // MouseOver

            ct.Default.MouseOver = new SuperTabItemStateColorTable();

            ct.Default.MouseOver.Background = new SuperTabLinearGradientColorTable(
                new Color[] { factory.GetColor(0xEAF6FD), factory.GetColor(0xD9F0FC), factory.GetColor(0xBEE6FD), factory.GetColor(0xA7D9F5) },
                new float[] { 0, .5f, .5f, 1 });

            ct.Default.MouseOver.InnerBorder = factory.GetColor(0xFAFDFE);
            ct.Default.MouseOver.OuterBorder = factory.GetColor(0x3C7FB1);
            ct.Default.MouseOver.Text = factory.GetColor(0x555564);
            ct.Default.MouseOver.CloseMarker = factory.GetColor(0x1A4875);

            // Bottom

            ct.Bottom = ct.Default;

            // Left, Right 

            ct.Left = (SuperTabColorStates) ct.Default.Clone();

            ct.Left.Normal.Background.AdaptiveGradient = false;
            ct.Left.MouseOver.Background.AdaptiveGradient = false;
            ct.Left.Selected.Background.AdaptiveGradient = false;
            ct.Left.SelectedMouseOver.Background.AdaptiveGradient = false;

            ct.Right = ct.Left;

            return (ct);
        }

        #endregion

        #region GetVisualStudio2008DockPanelColorTable

        private static SuperTabPanelColorTable GetVisualStudio2008DockPanelColorTable(ColorFactory factory)
        {
            SuperTabPanelColorTable ct = new SuperTabPanelColorTable();

            ct.Default.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xFFFFFF));
            ct.Default.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.OuterBorder = factory.GetColor(0x898C95);

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Default;

            return (ct);
        }

        #endregion

        #endregion

        #region VisualStudio2008Document

        #region GetVisualStudio2008DocumentColorTable

        private static SuperTabColorTable GetVisualStudio2008DocumentColorTable(ColorFactory factory)
        {
            SuperTabColorTable ct = new SuperTabColorTable();

            ct.Background = new SuperTabLinearGradientColorTable(Color.White);
            ct.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.OuterBorder = factory.GetColor(0x69A1BF);

            ct.ControlBoxDefault.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxMouseOver.Background = factory.GetColor(0xCEEDFA);
            ct.ControlBoxMouseOver.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxMouseOver.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxPressed.Background = factory.GetColor(0xB7CAE0);
            ct.ControlBoxPressed.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxPressed.Image = factory.GetColor(0xFF, 0x000000);

            ct.InsertMarker = factory.GetColor(0xFF, 0x000080);

            return (ct);
        }

        #endregion

        #region GetVisualStudio2008DocumentItemColorTable

        private static SuperTabItemColorTable GetVisualStudio2008DocumentItemColorTable(ColorFactory factory)
        {
            SuperTabItemColorTable ct = new SuperTabItemColorTable();

            // Top
            // Default

            ct.Default.Normal = new SuperTabItemStateColorTable();

            ct.Default.Normal.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xE6F1F9), factory.GetColor(0x98B4D2));
            ct.Default.Normal.InnerBorder = factory.GetColor(0xF2FAFF);
            ct.Default.Normal.OuterBorder = factory.GetColor(0x9196A2);
            ct.Default.Normal.Text = factory.GetColor(0xFF, 0x000000);
            ct.Default.Normal.CloseMarker = factory.GetColor(0x1A4875);

            // Selected

            ct.Default.Selected = new SuperTabItemStateColorTable();

            ct.Default.Selected.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xFCFDFE), factory.GetColor(0xD2E6FA));
            ct.Default.Selected.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.Selected.OuterBorder = factory.GetColor(0x69A1BF);
            ct.Default.Selected.Text = factory.GetColor(0xFF, 0x000000);
            ct.Default.Selected.CloseMarker = factory.GetColor(0x1A4875);

            // SelectedMouseOver

            ct.Default.SelectedMouseOver = new SuperTabItemStateColorTable();

            ct.Default.SelectedMouseOver.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xFCFDFE), factory.GetColor(0xD2E6FA));
            ct.Default.SelectedMouseOver.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.SelectedMouseOver.OuterBorder = factory.GetColor(0x69A1BF);
            ct.Default.SelectedMouseOver.Text = factory.GetColor(0xFF, 0x000000);
            ct.Default.SelectedMouseOver.CloseMarker = factory.GetColor(0x1A4875);

            // MouseOver

            ct.Default.MouseOver = new SuperTabItemStateColorTable();

            ct.Default.MouseOver.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xF7FCFE), factory.GetColor(0x81D0F1));
            ct.Default.MouseOver.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.MouseOver.OuterBorder = factory.GetColor(0x69A1BF);
            ct.Default.MouseOver.Text = factory.GetColor(0xFF, 0x000000);
            ct.Default.MouseOver.CloseMarker = factory.GetColor(0x1A4875);

            // Left, Bottom, Right

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Default;

            return (ct);
        }

        #endregion

        #region GetVisualStudio2008DocumentPanelColorTable

        private static SuperTabPanelColorTable GetVisualStudio2008DocumentPanelColorTable(ColorFactory factory)
        {
            SuperTabPanelColorTable ct = new SuperTabPanelColorTable();

            ct.Default.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xD2E6FA));
            ct.Default.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.OuterBorder = factory.GetColor(0x69A1BF);

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Default;

            return (ct);
        }

        #endregion

        #endregion

        #region WinMediaPlayer12

        #region GetWinMediaPlayer12ColorTable

        private static SuperTabColorTable
            GetWinMediaPlayer12ColorTable(ColorFactory factory)
        {
            SuperTabColorTable ct = new SuperTabColorTable();

            ct.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xE1EBF7));
            ct.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.OuterBorder = factory.GetColor(0xBCCBDD);

            ct.ControlBoxDefault.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxMouseOver.Background = factory.GetColor(0xCEEDFA);
            ct.ControlBoxMouseOver.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxMouseOver.Image = factory.GetColor(0xFF, 0x000000);

            ct.ControlBoxPressed.Background = factory.GetColor(0xB7CAE0);
            ct.ControlBoxPressed.Border = factory.GetColor(0x3399FF);
            ct.ControlBoxPressed.Image = factory.GetColor(0xFF, 0x000000);

            ct.InsertMarker = factory.GetColor(0xFF, 0x000080);

            return (ct);
        }

        #endregion

        #region GetWinMediaPlayer12ItemColorTable

        private static SuperTabItemColorTable
            GetWinMediaPlayer12ItemColorTable(ColorFactory factory)
        {
            SuperTabItemColorTable ct = new SuperTabItemColorTable();

            // Top
            // Default

            ct.Default.Normal = new SuperTabItemStateColorTable();

            ct.Default.Normal.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xD9E0E8), factory.GetColor(0xEAF2FA));
            ct.Default.Normal.Background.AdaptiveGradient = false;
            ct.Default.Normal.InnerBorder = factory.GetColor(0xEEEEEF);
            ct.Default.Normal.OuterBorder = factory.GetColor(0xA6BAD0);
            ct.Default.Normal.Text = factory.GetColor(0xFF, 0x000000);
            ct.Default.Normal.CloseMarker = factory.GetColor(0x1A4875);

            // Selected

            ct.Default.Selected = new SuperTabItemStateColorTable();

            ct.Default.Selected.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xF4F9FF), factory.GetColor(0xEFF6FD));
            ct.Default.Selected.Background.AdaptiveGradient = false;
            ct.Default.Selected.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.Selected.OuterBorder = factory.GetColor(0xBCCBDD);
            ct.Default.Selected.Text = factory.GetColor(0x002963);
            ct.Default.Selected.CloseMarker = factory.GetColor(0x1A4875);

            // Selected MouseOver

            ct.Default.SelectedMouseOver = new SuperTabItemStateColorTable();

            ct.Default.SelectedMouseOver.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xFFFFFF), factory.GetColor(0xF2F8FD));
            ct.Default.SelectedMouseOver.Background.AdaptiveGradient = false;
            ct.Default.SelectedMouseOver.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.SelectedMouseOver.OuterBorder = factory.GetColor(0xBCCBDD);
            ct.Default.SelectedMouseOver.Text = factory.GetColor(0x002963);
            ct.Default.SelectedMouseOver.CloseMarker = factory.GetColor(0x1A4875);

            // MouseOver

            ct.Default.MouseOver = new SuperTabItemStateColorTable();

            ct.Default.MouseOver.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xFFFFFF), factory.GetColor(0xF2F8FD));
            ct.Default.MouseOver.Background.AdaptiveGradient = false;
            ct.Default.MouseOver.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.MouseOver.OuterBorder = factory.GetColor(0xBCCBDD);
            ct.Default.MouseOver.Text = factory.GetColor(0x002963);
            ct.Default.MouseOver.CloseMarker = factory.GetColor(0x1A4875);

            // Left, Right, Bottom

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Default;

            return (ct);
        }

        #endregion

        #region GetWinMediaPlayer12PanelColorTable

        private static SuperTabPanelColorTable GetWinMediaPlayer12PanelColorTable(ColorFactory factory)
        {
            SuperTabPanelColorTable ct = new SuperTabPanelColorTable();

            ct.Default.Background = new SuperTabLinearGradientColorTable(factory.GetColor(0xEFF6FD));
            ct.Default.Background.AdaptiveGradient = false;
            ct.Default.InnerBorder = factory.GetColor(0xFFFFFF);
            ct.Default.OuterBorder = factory.GetColor(0xBCCBDD);

            ct.Left = ct.Default;
            ct.Bottom = ct.Default;
            ct.Right = ct.Default;

            return (ct);
        }

        #endregion

        #endregion

        #region GetPredefinedTabColors

        public static SuperTabItemColorTable GetPredefinedTabColors(eTabItemColor c, ColorFactory factory)
        {
            SuperTabItemColorTable sct = new SuperTabItemColorTable();

            sct.Default = new SuperTabColorStates();

            sct.Default.Normal = GetPredefinedDefaultColors(c, factory);
            sct.Default.Selected = GetPredefinedSelectedColors(c, factory);
            sct.Default.MouseOver = GetPredefinedMouseOverColors(c, factory);
            sct.Default.SelectedMouseOver = GetPredefinedSelectedMouseOverColors(c, factory);

            sct.Left = sct.Default;
            sct.Right = sct.Default;
            sct.Bottom = sct.Default;

            return (sct);
        }

        #region GetPredefinedDefaultColors

        internal static SuperTabItemStateColorTable GetPredefinedDefaultColors(
            eTabItemColor c, ColorFactory factory)
        {
            SuperTabItemStateColorTable ct = new SuperTabItemStateColorTable();

            ct.InnerBorder = Color.Empty;
            ct.OuterBorder = Color.Empty;
            ct.CloseMarker = factory.GetColor(0x406F9F);

            ct.Background = GetPredefinedDefaultBackground(c, factory);

            switch (c)
            {
                case eTabItemColor.OfficeMobile2014Teal:
                    ct.Text = factory.GetColor(0x00516D);
                    break;

                case eTabItemColor.OfficeMobile2014Blue:
                    ct.Text = factory.GetColor(0x074680);
                    break;

                case eTabItemColor.OfficeMobile2014Gold:
                    ct.Text = factory.GetColor(0x9D5A03);
                    break;

                case eTabItemColor.OfficeMobile2014Green:
                    ct.Text = factory.GetColor(0x44894F);
                    break;

                case eTabItemColor.OfficeMobile2014Coral:
                    ct.Text = factory.GetColor(0xBB4E43);
                    break;

                case eTabItemColor.OfficeMobile2014Lilac:
                    ct.Text = factory.GetColor(0x884D85);
                    break;

                case eTabItemColor.OfficeMobile2014Orange:
                    ct.Text = factory.GetColor(0x09D3900);
                    break;

                case eTabItemColor.OfficeMobile2014Pink:
                    ct.Text = factory.GetColor(0x89205A);
                    break;

                default:
                    ct.Text = Color.Black;
                    ct.InnerBorder = Color.White;
                    ct.OuterBorder = Color.DimGray;
                    break;
            }

            return (ct);
        }

        #endregion

        #region GetPredefinedSelectedColors

        internal static SuperTabItemStateColorTable GetPredefinedSelectedColors(
            eTabItemColor c, ColorFactory factory)
        {
            SuperTabItemStateColorTable ct = new SuperTabItemStateColorTable();

            ct.CloseMarker = factory.GetColor(0x406F9F);
            ct.SelectionMarker = factory.GetColor(0xFF, 0xFFFFFF);

            ct.Background = GetPredefinedSelectedBackground(c, factory);
            
            switch (c)
            {
                case eTabItemColor.OfficeMobile2014Teal:
                case eTabItemColor.OfficeMobile2014Blue:
                case eTabItemColor.OfficeMobile2014Gold:
                case eTabItemColor.OfficeMobile2014Green:
                case eTabItemColor.OfficeMobile2014Coral:
                case eTabItemColor.OfficeMobile2014Lilac:
                case eTabItemColor.OfficeMobile2014Orange:
                case eTabItemColor.OfficeMobile2014Pink:
                    ct.Text = factory.GetColor(Color.White);
                    ct.InnerBorder = Color.Empty;
                    ct.OuterBorder = Color.Empty;
                    break;

                default:
                    ct.Text = Color.Black;
                    ct.InnerBorder = Color.White;
                    ct.OuterBorder = Color.DimGray;
                    break;
            }

            return (ct);
        }

        #endregion

        #region GetPredefinedMouseOverColors

        internal static SuperTabItemStateColorTable GetPredefinedMouseOverColors(
            eTabItemColor c, ColorFactory factory)
        {
            switch (c)
            {
                case eTabItemColor.OfficeMobile2014Teal:
                case eTabItemColor.OfficeMobile2014Blue:
                case eTabItemColor.OfficeMobile2014Gold:
                case eTabItemColor.OfficeMobile2014Green:
                case eTabItemColor.OfficeMobile2014Coral:
                case eTabItemColor.OfficeMobile2014Lilac:
                case eTabItemColor.OfficeMobile2014Orange:
                case eTabItemColor.OfficeMobile2014Pink:
                    return (GetPredefinedDefaultColors(c, factory));

                default:
                    SuperTabItemStateColorTable ct = new SuperTabItemStateColorTable();

                    ct.Text = Color.Black;
                    ct.InnerBorder = Color.White;
                    ct.OuterBorder = Color.DimGray;
                    ct.CloseMarker = factory.GetColor(0x406F9F);
                    ct.SelectionMarker = factory.GetColor(0xFF, 0xFFFFFF);

                    ct.Background = new 
                        SuperTabLinearGradientColorTable(factory.GetColor(0xFFFBF0), factory.GetColor(0xFFF0C8));

                    return (ct);
            }
        }

        #endregion

        #region GetPredefinedSelectedMouseOverColors

        internal static SuperTabItemStateColorTable GetPredefinedSelectedMouseOverColors(
            eTabItemColor c, ColorFactory factory)
        {
            switch (c)
            {
                case eTabItemColor.OfficeMobile2014Teal:
                case eTabItemColor.OfficeMobile2014Blue:
                case eTabItemColor.OfficeMobile2014Gold:
                case eTabItemColor.OfficeMobile2014Green:
                case eTabItemColor.OfficeMobile2014Coral:
                case eTabItemColor.OfficeMobile2014Lilac:
                case eTabItemColor.OfficeMobile2014Orange:
                case eTabItemColor.OfficeMobile2014Pink:
                    return (GetPredefinedSelectedColors(c, factory));

                default:
                    SuperTabItemStateColorTable ct = new SuperTabItemStateColorTable();

                    ct.Text = Color.Black;
                    ct.InnerBorder = Color.White;
                    ct.OuterBorder = Color.DimGray;
                    ct.CloseMarker = factory.GetColor(0x406F9F);
                    ct.SelectionMarker = factory.GetColor(0xFF, 0xFFFFFF);

                    ct.Background = new
                        SuperTabLinearGradientColorTable(factory.GetColor(0xFFFBF0), factory.GetColor(0xFFF0C8));

                    return (ct);
            }
        }

        #endregion

        #region GetPredefinedPanelColors

        public static SuperTabPanelItemColorTable GetPredefinedPanelColors(eTabItemColor c, ColorFactory factory)
        {
            SuperTabPanelItemColorTable pct = new SuperTabPanelItemColorTable();

            SuperTabItemColorTable sct = GetPredefinedTabColors(c, factory);
            SuperTabLinearGradientColorTable lgt = sct.Default.Selected.Background;

            Color color = (lgt.Colors != null && lgt.Colors.Length > 0)
                ? lgt.Colors[lgt.Colors.Length - 1] : Color.DimGray;

            switch (c)
            {
                case eTabItemColor.OfficeMobile2014Teal:
                case eTabItemColor.OfficeMobile2014Blue:
                case eTabItemColor.OfficeMobile2014Gold:
                case eTabItemColor.OfficeMobile2014Green:
                case eTabItemColor.OfficeMobile2014Coral:
                case eTabItemColor.OfficeMobile2014Lilac:
                case eTabItemColor.OfficeMobile2014Orange:
                case eTabItemColor.OfficeMobile2014Pink:
                    pct.InnerBorder = color;
                    pct.OuterBorder = color;

                    pct.Background = new SuperTabLinearGradientColorTable(Color.White);
                    break;

                default:
                    pct.InnerBorder = sct.Default.Selected.InnerBorder;
                    pct.OuterBorder = sct.Default.Selected.OuterBorder;

                    pct.Background = new SuperTabLinearGradientColorTable(color);
                    break;
            }

            return (pct);
        }

        #endregion

        #region GetPredefinedDefaultBackground

        internal static SuperTabLinearGradientColorTable
            GetPredefinedDefaultBackground(eTabItemColor c, ColorFactory factory)
        {
            switch (c)
            {
                case eTabItemColor.Apple:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xDAF4CD), factory.GetColor(0xB1E898)));

                case eTabItemColor.Blue:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xDDE6F7), factory.GetColor(0x8AA8E4)));

                case eTabItemColor.BlueMist:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xD2E0EB), factory.GetColor(0xA1BFD4)));

                case eTabItemColor.Cyan:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xCDDFDB), factory.GetColor(0x97BEB2)));

                case eTabItemColor.Green:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xDEE6CF), factory.GetColor(0xBBCC9D)));

                case eTabItemColor.Lemon:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xFBFDBD), factory.GetColor(0xF5F977)));

                case eTabItemColor.Magenta:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xECD6E0), factory.GetColor(0xD7A9BF)));

                case eTabItemColor.Orange:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xFBDBC2), factory.GetColor(0xF6B47F)));

                case eTabItemColor.Purple:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xDDD3F1), factory.GetColor(0xB8A3E0)));

                case eTabItemColor.PurpleMist:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xD9D2DE), factory.GetColor(0xAFA1BB)));

                case eTabItemColor.Red:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xF7CFCF), factory.GetColor(0xEF9B9D)));

                case eTabItemColor.Silver:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xCFCFD9), factory.GetColor(0x9B9BAE)));

                case eTabItemColor.Tan:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xF5EBD0), factory.GetColor(0xE9D49F)));

                case eTabItemColor.Teal:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xCDECF0), factory.GetColor(0x4EBCCA)));

                case eTabItemColor.OfficeMobile2014Teal:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0x9CCDDA)));

                case eTabItemColor.OfficeMobile2014Blue:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xA9C7E3)));

                case eTabItemColor.OfficeMobile2014Gold:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xF0D08A)));

                case eTabItemColor.OfficeMobile2014Green:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xA6D0AD)));

                case eTabItemColor.OfficeMobile2014Coral:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xF0AEA8)));

                case eTabItemColor.OfficeMobile2014Lilac:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xD9B9D8)));

                case eTabItemColor.OfficeMobile2014Orange:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xF1C196)));

                case eTabItemColor.OfficeMobile2014Pink:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xE8B5D1)));

                default:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xFFF5D9), factory.GetColor(0xFFDC78)));
            }
        }

        #endregion

        #region GetPredefinedSelectedBackground

        internal static SuperTabLinearGradientColorTable
            GetPredefinedSelectedBackground(eTabItemColor c, ColorFactory factory)
        {
            switch (c)
            {
                case eTabItemColor.Apple:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xEDFAE7), factory.GetColor(0xD6F3C8)));

                case eTabItemColor.Blue:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xDDE6F7), factory.GetColor(0x8AA8E4)));

                case eTabItemColor.BlueMist:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xE9F0F5), factory.GetColor(0xCDDDE8)));

                case eTabItemColor.Cyan:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xE7F0EE), factory.GetColor(0xC8DCD6)));

                case eTabItemColor.Green:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xF0F3E8), factory.GetColor(0xDBE4CB)));

                case eTabItemColor.Lemon:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xFDFEDF), factory.GetColor(0xFAFCB7)));

                case eTabItemColor.Magenta:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xF6EBF0), factory.GetColor(0xEAD1DD)));

                case eTabItemColor.Orange:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xFDEEE1), factory.GetColor(0xFAD7BB)));

                case eTabItemColor.Purple:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xEFEAF7), factory.GetColor(0xD9CEEE)));

                case eTabItemColor.PurpleMist:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xECE9F0), factory.GetColor(0xD5CDDB)));

                case eTabItemColor.Red:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xFCE8E8), factory.GetColor(0xF6CACB)));

                case eTabItemColor.Silver:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xE8E8EC), factory.GetColor(0xCACAD4)));

                case eTabItemColor.Tan:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xFAF5E9), factory.GetColor(0xF3E8CC)));

                case eTabItemColor.Teal:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xCDECF0), factory.GetColor(0x4EBCCA)));

                case eTabItemColor.OfficeMobile2014Teal:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0x3196B2)));

                case eTabItemColor.OfficeMobile2014Blue:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0x4C8BC5)));

                case eTabItemColor.OfficeMobile2014Gold:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xE09D0C)));

                case eTabItemColor.OfficeMobile2014Green:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0x469C54)));

                case eTabItemColor.OfficeMobile2014Coral:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xDF5749)));

                case eTabItemColor.OfficeMobile2014Lilac:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xB16DAD)));

                case eTabItemColor.OfficeMobile2014Orange:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xE27E24)));

                case eTabItemColor.OfficeMobile2014Pink:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xCE659F)));

                default:
                    return (new SuperTabLinearGradientColorTable(factory.GetColor(0xFFFAEC), factory.GetColor(0xFFEAB3)));
            }
        }

        #endregion

        #endregion

        #region IsOfficeMobile2014Color

        public static bool IsOfficeMobile2014Color(eTabItemColor ec)
        {
            switch (ec)
            {
                case eTabItemColor.OfficeMobile2014Teal:
                case eTabItemColor.OfficeMobile2014Blue:
                case eTabItemColor.OfficeMobile2014Gold:
                case eTabItemColor.OfficeMobile2014Green:
                case eTabItemColor.OfficeMobile2014Coral:
                case eTabItemColor.OfficeMobile2014Lilac:
                case eTabItemColor.OfficeMobile2014Orange:
                case eTabItemColor.OfficeMobile2014Pink:
                    return (true);

                default:
                    return (false);
            }
        }

        #endregion
    }
}
