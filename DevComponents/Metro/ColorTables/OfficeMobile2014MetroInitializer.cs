using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using DevComponents.DotNetBar.Rendering;
using DevComponents.DotNetBar.Metro.Helpers;

namespace DevComponents.DotNetBar.Metro.ColorTables
{
    /// <summary>
    /// Initializes Office Mobile 2014 color scheme Metro color table based on colors specified.
    /// </summary>
    internal class OfficeMobile2014MetroInitializer
    {
        public static MetroColorTable CreateColorTable(MetroColorGeneratorParameters colorParams)
        {
            return CreateColorTable(colorParams.CanvasColor, colorParams.BaseColor);
        }
        public static MetroColorTable CreateColorTable(Color canvasColor, Color baseColor)
        {
            MetroColorTable metroTable = new MetroColorTable();
            Office2007ColorTable officeMetroColorTable = new Office2007ColorTable();
            CreateColors(metroTable, officeMetroColorTable, canvasColor, baseColor);
            if (StyleManager.IsMetro(StyleManager.Style))
                ((Office2007Renderer)GlobalManager.Renderer).ColorTable = officeMetroColorTable;
            return metroTable;
        }

        private const double DEGREE = 1d / 360;
        public static MetroPartColors CreateMetroPartColors(Color canvasColor, Color baseColor)
        {
            ColorFunctions.HLSColor canvasHsl = ColorFunctions.RGBToHSL(canvasColor);
            ColorFunctions.HLSColor baseHsl = ColorFunctions.RGBToHSL(baseColor);
            HSVColor baseHsv = ColorHelpers.ColorToHSV(baseColor);
            HSVColor canvasHsv = ColorHelpers.ColorToHSV(canvasColor);

            // Create metro colors
            MetroPartColors partColors = new MetroPartColors();
            partColors.CanvasColor = canvasColor;
            partColors.BaseColor = baseColor;
            partColors.TextColor = (canvasHsl.Lightness < .4) ? Color.White : Color.Black;
            //HSVColor textHsv = ColorHelpers.ColorToHSV(partColors.TextColor);
            partColors.TextInactiveColor = ColorHelpers.HSVToColor(canvasHsv.Hue, canvasHsv.Saturation, (partColors.TextColor == Color.White ? canvasHsv.Value + .53 : canvasHsv.Value - .47));
            partColors.TextDisabledColor = ColorHelpers.HSVToColor(canvasHsv.Hue, canvasHsv.Saturation, (partColors.TextColor == Color.Black ? canvasHsv.Value - .33 : canvasHsv.Value + .67));
            partColors.TextLightColor = ColorHelpers.HSVToColor(canvasHsv.Hue, canvasHsv.Saturation, (partColors.TextColor == Color.Black ? canvasHsv.Value - .6 : canvasHsv.Value + .4));
            partColors.CanvasColorDarkShade = partColors.TextColor == Color.Black ? ColorScheme.GetColor(0xC8C8C8) : ColorHelpers.HSVToColor(canvasHsv.Hue, canvasHsv.Saturation, (partColors.TextColor == Color.Black ? canvasHsv.Value - .33 : canvasHsv.Value + .33));
            partColors.CanvasColorDarkerShade = ColorScheme.GetColor(0xB4B4B4);
            partColors.CanvasColorLightShade = ColorScheme.GetColor(0xCCCCCC);
            partColors.CanvasColorLighterShade = ColorHelpers.HSVToColor(canvasHsv.Hue, canvasHsv.Saturation, (partColors.TextColor == Color.White ? canvasHsv.Value + .05 : canvasHsv.Value - .05));
            partColors.CanvasColorLight = (partColors.TextColor == Color.Black) ? ColorScheme.GetColor(0xFAFAFA) : ColorHelpers.HSVToColor(canvasHsv.Hue, canvasHsv.Saturation, canvasHsv.Value + .06);
            partColors.BaseTextColor = GetTextColor(baseColor);

            partColors.BaseColorLight = ColorHelpers.HSVToColor(baseHsv.Hue, GetColorMin(0.08, baseHsv.Saturation - .41), baseHsv.Value + .3);
            partColors.BaseColorLight1 = ColorHelpers.HSVToColor(baseHsv.Hue, GetColorMin(0.05, baseHsv.Saturation - .12), baseHsv.Value + .12);
            partColors.BaseColorLightText = GetTextColor(partColors.BaseColorLight);

            partColors.BaseColorLightest = ColorHelpers.HSVToColor(baseHsv.Hue, GetColorMin(0.05, baseHsv.Saturation - .6), baseHsv.Value + .5);
            partColors.BaseColorLighter = ColorHelpers.HSVToColor(baseHsv.Hue, GetColorMin(0.08, baseHsv.Saturation - .46), baseHsv.Value + .39);
            partColors.BaseColorDark = ColorHelpers.HSVToColor(baseHsv.Hue, baseHsv.Saturation + .1, baseHsv.Value - .06);
            partColors.BaseColorDarker = ColorFunctions.HLSToRGB(baseHsl.Hue, baseHsl.Lightness - .2, baseHsl.Saturation);

            partColors.ComplementColor = ColorHelpers.HSVToColor(GetComplementHue(baseHsv.Hue), baseHsv.Saturation, baseHsv.Value + (baseHsv.Value > .5d ? -.35 : 0d));
            ColorFunctions.HLSColor compHsl = ColorFunctions.RGBToHSL(partColors.ComplementColor);
            HSVColor compHsv = ColorHelpers.ColorToHSV(partColors.ComplementColor);
            partColors.ComplementColorLight = ColorHelpers.HSVToColor(compHsv.Hue, compHsv.Saturation, compHsv.Value + .2d);
            partColors.ComplementColorDark = ColorFunctions.HLSToRGB(compHsl.Hue, compHsl.Lightness - .10, compHsl.Saturation);
            partColors.ComplementColorDarker = ColorFunctions.HLSToRGB(compHsl.Hue, compHsl.Lightness - .2, compHsl.Saturation);
            partColors.ComplementColorText = GetTextColor(partColors.ComplementColor);
            partColors.ComplementColorLightText = GetTextColor(partColors.ComplementColorLight);

            partColors.BaseButtonGradientStart = ColorHelpers.HSVToColor(canvasHsv.Hue, canvasHsv.Saturation, (canvasHsv.Value > 0 ? canvasHsv.Value - .01 : canvasHsv.Value + .08));
            partColors.BaseButtonGradientEnd = ColorHelpers.HSVToColor(canvasHsv.Hue, canvasHsv.Saturation, (canvasHsv.Value > 0 ? canvasHsv.Value - .08 : canvasHsv.Value + .20));

            if (partColors.TextColor == Color.Black)
                partColors.EditControlBackColor = Color.White;
            else
                partColors.EditControlBackColor = Color.Black;

            return partColors;
        }
        public static void CreateColors(MetroColorTable metroTable, Office2007ColorTable officeMetroColorTable, Color canvasColor, Color baseColor)
        {
            MetroPartColors partColors = CreateMetroPartColors(canvasColor, baseColor);

            metroTable.CanvasColor = partColors.CanvasColor;
            metroTable.CanvasColorShadeLight = partColors.CanvasColorLightShade;
            metroTable.CanvasColorShadeLighter = partColors.CanvasColorLighterShade;
            metroTable.ForeColor = partColors.TextColor;
            metroTable.BaseColor = baseColor;
            metroTable.EditControlBackColor = partColors.EditControlBackColor;
            metroTable.MetroPartColors = partColors;

            metroTable.MetroAppForm.BorderThickness = new Thickness(1, 1, 1, 1);
            metroTable.MetroAppForm.BorderColors = new BorderColors(partColors.BaseColor);
            metroTable.MetroAppForm.BorderColorsInactive = new BorderColors(partColors.BaseColorLight);

            metroTable.MetroForm.BorderThickness = new Thickness(1, 1, 1, 1);
            metroTable.MetroForm.BorderColors = new BorderColors[1] { new BorderColors(partColors.BaseColor) };
            metroTable.MetroForm.BorderColorsInactive = new BorderColors[4] { new BorderColors(partColors.BaseColorLight), new BorderColors(partColors.BaseColorLight), new BorderColors(partColors.BaseColorLight), new BorderColors(partColors.BaseColorLight) };

            metroTable.MetroTab.ActiveCaptionText = partColors.BaseTextColor;
            metroTable.MetroTab.InactiveCaptionText = partColors.BaseTextColor;

            metroTable.MetroTab.MetroTabItem.Default = GetMetroTabItemStateTable(partColors.BaseTextColor, Color.Empty);
            metroTable.MetroTab.MetroTabItem.Selected = GetMetroTabItemStateTable(partColors.BaseColor, partColors.CanvasColorLight);
            metroTable.MetroTab.MetroTabItem.Disabled = GetMetroTabItemStateTable(partColors.TextDisabledColor, Color.Empty);
            metroTable.MetroTab.MetroTabItem.MouseOver = null;
            metroTable.MetroTab.MetroTabItem.Pressed = null;
            metroTable.MetroTab.TabStrip.BackgroundStyle = new ElementStyle(partColors.BaseTextColor, partColors.BaseColor);
            metroTable.MetroTab.CaptionTextFormat = eTextFormat.VerticalCenter | eTextFormat.HorizontalCenter | eTextFormat.EndEllipsis | eTextFormat.NoPrefix;

            metroTable.MetroTab.TabPanelBackgroundStyle = new ElementStyle(partColors.TextColor, partColors.CanvasColorLight);

            // Toolbar
            metroTable.MetroToolbar.BackgroundStyle = new ElementStyle(partColors.TextColor, partColors.CanvasColorLight);

            // Status Bar
            metroTable.MetroStatusBar.BackgroundStyle = new ElementStyle(partColors.BaseTextColor, partColors.BaseColor);
            metroTable.MetroStatusBar.TopBorders = new Color[0];
            metroTable.MetroStatusBar.BottomBorders = new Color[0];
            metroTable.MetroStatusBar.ResizeMarkerLightColor = Color.FromArgb(196, Color.White);
            metroTable.MetroStatusBar.ResizeMarkerColor = partColors.BaseColorDarker;

            OfficeMobile2014ColorTableInitializer.InitializeColorTable(officeMetroColorTable, ColorFactory.Empty, partColors);
        }
        private static double GetColorMin(double minValue, double value)
        {
            if (value < 0)
                return minValue;
            return value;
        }

        private static Color GetTextColor(Color color)
        {
            ColorFunctions.HLSColor hslColor = ColorFunctions.RGBToHSL(color);
            return hslColor.Lightness < .65 ? Color.White : Color.Black;
            //HSVColor hsvColor = ColorHelpers.ColorToHSV(backColor);
            //return hsvColor.Value < .65 ? Color.White : Color.Black;
        }

        private static double GetComplementHue(double hue)
        {
            if (hue <= .37777d)
            {
                return Math.Min(1, hue + (137 * DEGREE + (86 * DEGREE * (hue / .37777d))));
            }
            else
            {
                return Math.Max(0, hue - (137 * DEGREE + (86 * DEGREE * (hue / .6222222d))));//     ((137 + 86 * DEGREE * (hue / .6222222) * DEGREE)));
            }
        }

        private static MetroTabItemStateColorTable GetMetroTabItemStateTable(Color textColor, Color backColor)
        {
            ElementStyle style = new ElementStyle(textColor);
            style.TextAlignment = eStyleTextAlignment.Center;
            style.TextLineAlignment = eStyleTextAlignment.Center;
            style.HideMnemonic = true;
            if (!backColor.IsEmpty)
            {
                style.CornerTypeTopRight = eCornerType.Rounded;
                style.CornerTypeTopLeft = eCornerType.Rounded;
                style.CornerDiameter = 2;
                style.BorderLeft = eStyleBorderType.Solid;
                style.BorderTop = eStyleBorderType.Solid;
                style.BorderRight = eStyleBorderType.Solid;
                style.BorderLeftWidth = 1;
                style.BorderTopWidth = 1;
                style.BorderRightWidth = 1;
                style.BorderColor = backColor;
                style.BackColor = backColor;
            }
            style.TextColor = textColor;
            return new MetroTabItemStateColorTable(style);
        }
    }
}
