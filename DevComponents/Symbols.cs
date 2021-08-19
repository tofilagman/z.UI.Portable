using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Reflection;
#if LAYOUT
namespace DevComponents.DotNetBar.Layout
#else
namespace DevComponents.DotNetBar
#endif
{
    // Uses Font Awesome - http://fortawesome.github.com/Font-Awesome
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Symbols
    {
        #region WinApi
        [DllImport("gdi32")]
        static extern IntPtr AddFontMemResourceEx(IntPtr pbFont,
                                                           uint cbFont,
                                                           IntPtr pdv,
                                                           [In] ref uint pcFonts);
        [DllImport("gdi32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RemoveFontMemResourceEx(IntPtr fh);
        #endregion

        #region Internal Implementation

        public static string GetSymbol(string symbol)
        {
            if (string.IsNullOrEmpty(symbol) == true)
                return (String.Empty);

            if (symbol.Length == 1)
                return symbol;

            int utf32 = 0;

            if (int.TryParse(symbol, out utf32))
                return char.ConvertFromUtf32(utf32);

            return symbol;
        }

        /// <summary>
        /// Returns specific font set at specified size/
        /// </summary>
        /// <param name="fontSize">Font size</param>
        /// <param name="symbolSet">Symbol set to return</param>
        /// <returns>Font</returns>
        public static Font GetFont(float fontSize, eSymbolSet symbolSet)
        {
            if (symbolSet == eSymbolSet.Awesome)
                return GetFontAwesome(fontSize);
            else
                return GetFontMaterial(fontSize);
        }

        private static Dictionary<float, Font> _FontAwesomeCache = new Dictionary<float, Font>(10);
        private static Dictionary<float, Font> _FontMaterialCache = new Dictionary<float, Font>(10);
        /// <summary>
        /// Returns FontAwesome at specific size.
        /// </summary>
        /// <param name="fontSize">Font size in points</param>
        /// <returns>Font in desired size.</returns>
        private static Font GetFontAwesome(float fontSize)
        {
            Font font = null;
            EnsureFontAwesomeLoaded();
            if (fontSize <= 0) return _FontAwesome;
            if(_FontAwesomeCache.TryGetValue(fontSize, out font))
                return font;
            font = new Font(_FontAwesome.FontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Point);
            _FontAwesomeCache.Add(fontSize, font);
            return font;
        }

        /// <summary>
        /// Returns FontAwesome at specific size.
        /// </summary>
        /// <param name="fontSize">Font size in points</param>
        /// <returns>Font in desired size.</returns>
        public static Font GetFontMaterial(float fontSize)
        {
            Font font = null;
            EnsureFontMaterialLoaded();
            if (fontSize <= 0) return _FontMaterial;
            if (_FontMaterialCache.TryGetValue(fontSize, out font))
                return font;
            font = new Font(_FontMaterial.FontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Point);
            _FontMaterialCache.Add(fontSize, font);
            return font;
        }

        private static Font _FontAwesome = null;
        /// <summary>
        /// Gets FontAwesome at default size.
        /// </summary>
        public static Font FontAwesome
        {
            get
            {
                EnsureFontAwesomeLoaded();
                return _FontAwesome;
            }
        }
        /// <summary>
        /// Returns FontAwesome Family.
        /// </summary>
        public static FontFamily FontAwesomeFamily
        {
            get
            {
                EnsureFontAwesomeLoaded();
                return _FontAwesome.FontFamily;
            }
        }

        private static Font _FontMaterial = null;
        /// <summary>
        /// Gets Material Font at default size.
        /// </summary>
        public static Font FontMaterial
        {
            get
            {
                EnsureFontMaterialLoaded();
                return _FontMaterial;
            }
        }
        /// <summary>
        /// Returns Material Font Family.
        /// </summary>
        public static FontFamily FontMaterialFamily
        {
            get
            {
                EnsureFontMaterialLoaded();
                return _FontMaterial.FontFamily;
            }
        }

        private static PrivateFontCollection _PrivateFontAwesomeCollection;
        private static PrivateFontCollection _PrivateFontMaterialCollection;
        private static GCHandle _FontAwesomeHandle;
        private static IntPtr _FontAwesomePointer;
        private static GCHandle _FontMaterialHandle;
        private static IntPtr _FontMaterialPointer;
        private static void EnsureFontAwesomeLoaded()
        {
            if (_FontAwesome == null)
            {
                _PrivateFontAwesomeCollection = new PrivateFontCollection();

                byte[] fontAwesomeBuffer = LoadFont("SystemImages.FontAwesome.ttf");
                _FontAwesomeHandle = GCHandle.Alloc(fontAwesomeBuffer, GCHandleType.Pinned);
                _PrivateFontAwesomeCollection.AddMemoryFont(_FontAwesomeHandle.AddrOfPinnedObject(), fontAwesomeBuffer.Length);
                uint rsxCnt = 1;
                _FontAwesomePointer = AddFontMemResourceEx(_FontAwesomeHandle.AddrOfPinnedObject(),
                                                     (uint)fontAwesomeBuffer.Length, IntPtr.Zero, ref rsxCnt);
                using (FontFamily ff = _PrivateFontAwesomeCollection.Families[0])
                {
                    if (ff.IsStyleAvailable(FontStyle.Regular))
                    {
                        _FontAwesome = new Font(ff, _FontAwesomeDefaultSize, FontStyle.Regular, GraphicsUnit.Point);
                        _FontAwesomeCache.Add(_FontAwesomeDefaultSize, _FontAwesome);
                    }
                    else
                    {
                        // Error use default font...
                        _FontAwesome = SystemInformation.MenuFont;
                    }
                }
            }
        }

        private static void EnsureFontMaterialLoaded()
        {
            if (_FontMaterial == null)
            {
                _PrivateFontMaterialCollection = new PrivateFontCollection();

                byte[] fontMaterialBuffer = LoadFont("SystemImages.MaterialIcons-Regular.ttf");
                _FontMaterialHandle = GCHandle.Alloc(fontMaterialBuffer, GCHandleType.Pinned);
                _PrivateFontMaterialCollection.AddMemoryFont(_FontMaterialHandle.AddrOfPinnedObject(), fontMaterialBuffer.Length);
                uint rsxCnt = 1;
                _FontMaterialPointer = AddFontMemResourceEx(_FontMaterialHandle.AddrOfPinnedObject(),
                                                     (uint)fontMaterialBuffer.Length, IntPtr.Zero, ref rsxCnt);
                using (FontFamily ff = _PrivateFontMaterialCollection.Families[0])
                {
                    if (ff.IsStyleAvailable(FontStyle.Regular))
                    {
                        _FontMaterial = new Font(ff, _FontMaterialDefaultSize, FontStyle.Regular, GraphicsUnit.Point);
                        _FontMaterialCache.Add(_FontMaterialDefaultSize, _FontMaterial);
                    }
                    else
                    {
                        // Error use default font...
                        _FontMaterial = SystemInformation.MenuFont;
                    }
                }
            }
        }

        private static float _FontMaterialDefaultSize = 18;
        /// <summary>
        /// Gets the default size for the Material font size in points.
        /// </summary>
        public static float FontMaterialDefaultSize
        {
            get { return _FontMaterialDefaultSize; }
        }

        private static float _FontAwesomeDefaultSize = 18;
        /// <summary>
        /// Gets the default size for the FontAwesome font size in points.
        /// </summary>
        public static float FontAwesomeDefaultSize
        {
            get { return _FontAwesomeDefaultSize; }
        }

        internal static byte[] LoadFont(string fontFileName)
        {
            DotNetBarResourcesAttribute att = Attribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(), typeof(DotNetBarResourcesAttribute)) as DotNetBarResourcesAttribute;
            if (att != null && att.NamespacePrefix != "")
            {
                using (Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(att.NamespacePrefix + "." + fontFileName))
                {
                    byte[] fontData = new byte[fontStream.Length];
                    fontStream.Read(fontData, 0, fontData.Length);
                    fontStream.Close();
                    return fontData;
                }
            }
            else
            {
                using (Stream fontStream = typeof(DotNetBarManager).Module.Assembly.GetManifestResourceStream(typeof(DotNetBarManager), fontFileName))
                {
                    byte[] fontData = new byte[fontStream.Length];
                    fontStream.Read(fontData, 0, fontData.Length);
                    fontStream.Close();
                    return fontData;
                }
            }
            return new byte[0];
        }
        #endregion
    }
}
