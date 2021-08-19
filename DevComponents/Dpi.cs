using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Provides High DPI support for DotNetBar controls.
    /// </summary>
    public static class Dpi
    {
        private static SizeF _Factor = new SizeF(1f, 1f);
        private static bool _UseFactor = false;
        private static bool _NormalizeScaleFactor = false;

        static Dpi()
        {
            //Debug.WriteLine("Static constructor");
        }

        private static bool _RecordScalePerControl = false;
        /// <summary>
        /// Indicates whether static scale factor is set when child controls get ScaleControl call. When set to false 
        /// this is done only from parent OfficeForm or RibbonForm.
        /// </summary>
        public static bool RecordScalePerControl
        {
            get { return _RecordScalePerControl; }
            set { _RecordScalePerControl = value; }
        }

        /// <summary>
        /// Gets or sets whether scale factor when set is normalized so both Width and Height values are the same. Default value is false.
        /// If using ScaleMode=font the scale factor might not be same for Width and Height so this allows opportunity to keep existing size ratios on the DotNetBar sized controls.
        /// When set to true the scale factor Height will always be set to scale factor Width.
        /// </summary>
        public static bool NormalizeScaleFactor
        {
            get { return _NormalizeScaleFactor; }
            set { _NormalizeScaleFactor = value; }
        }

        public static SizeF Factor
        {
            get { return _Factor; }
        }

        public static bool UseFactor
        {
            get { return (_UseFactor); }
        }

        public static Size Size(Size size)
        {
            if (!_UseFactor) return size;
            size.Width = (int)(size.Width * _Factor.Width);
            size.Height = (int)(size.Height * _Factor.Height);
            return size;
        }

        public static Size ImageSize(Size size)
        {
            if (!_UseFactor || !_AutoScaleImages) return size;
            size.Width = (int)(size.Width * _Factor.Width);
            size.Height = (int)(size.Height * _Factor.Height);
            return size;
        }

        public static Size Size(int width, int height)
        {
            if (!_UseFactor)
                return (new Size(width, height));

            Size size = new Size(
                (int)(width * _Factor.Width),
                (int)(height * _Factor.Height));

            return (size);
        }

        public static Rectangle Size(Rectangle r)
        {
            if (!_UseFactor) return r;
            r.Width = (int)(r.Width * _Factor.Width);
            r.Height = (int)(r.Height * _Factor.Height);
            return r;
        }

        public static Point Size(Point p)
        {
            if (!_UseFactor) return p;
            p.X = (int)(p.X * _Factor.Width);
            p.Y = (int)(p.Y * _Factor.Height);
            return p;
        }

        public static DevComponents.DotNetBar.Padding Size(DevComponents.DotNetBar.Padding padding)
        {
            if (!_UseFactor) return padding;
            return new Padding((int)(padding.Left * _Factor.Width), (int)(padding.Right * _Factor.Width), (int)(padding.Top * _Factor.Height), (int)(padding.Bottom * _Factor.Height));
        }

        public static int DescaleWidth(int width)
        {
            if (!_UseFactor) return width;
            return (int)(width / _Factor.Width);
        }
        public static int DescaleHeight(int height)
        {
            if (!_UseFactor) return height;
            return (int)(height / _Factor.Height);
        }

        internal static System.Drawing.Size Descale(System.Drawing.Size size)
        {
            if (!_UseFactor) return size;
            size.Width = (int)(size.Width / _Factor.Width);
            size.Height = (int)(size.Height / _Factor.Height);
            return size;
        }

        public static int Width(int width)
        {
            if (!_UseFactor) return width;
            return (int)(width * _Factor.Width);
        }
        public static int ImageWidth(int width)
        {
            if (!_UseFactor || !_AutoScaleImages) return width;
            return (int)(width * _Factor.Width);
        }

        public static int Height(int height)
        {
            if (!_UseFactor) return height;
            return (int)(height * _Factor.Height);
        }
        public static int ImageHeight(int height)
        {
            if (!_UseFactor || !_AutoScaleImages) return height;
            return (int)(height * _Factor.Height);
        }

        public static float Height(float height)
        {
            if (!_UseFactor) return height;
            return (height * _Factor.Height);
        }

        private static int _CaptionVerticalPadding = 10;
        public static int CaptionVerticalPadding
        {
            get
            {
                if (!_UseFactor) return 0;
                return _CaptionVerticalPadding;
            }
        }

        public static void SetScaling(SizeF factor)
        {
            if (factor.Width < 1f) factor.Width = 1f;
            if (factor.Height < 1f) factor.Height = 1f;
            
            if (factor == _Factor) return;

            if (_NormalizeScaleFactor)
                factor.Height = factor.Width;

            if (factor.Width == 1f && factor.Height == 1f)
            {
                _UseFactor = false;
                _Factor = factor;
            }
            else
            {
                _UseFactor = true;
                _Factor = factor;
            }

            UpdateStaticSizes();
        }

        private static bool _AutoScaleImages=true;
        /// <summary>
        /// Indicates whether controls will automatically scale current images based on the current DPI. Depending on scaling this may result in pixalted images.
        /// Best policy is to provide separate images for each DPI level Windows runs on and if you do that you need to set this property to false to disable
        /// automatic size scaling for the images. Default value is true which causes the images to be upscaled.
        /// </summary>
        public static bool AutoScaleImages
        {
            get { return _AutoScaleImages; }
            set { _AutoScaleImages = value; }
        }

        private static void UpdateStaticSizes()
        {
            Width1 = Dpi.Width(1);
            Width2 = Dpi.Width(2);
            Width3 = Dpi.Width(3);
            Width4 = Dpi.Width(4);
            Width5 = Dpi.Width(5);
            Width6 = Dpi.Width(6);
            Width7 = Dpi.Width(7);
            Width8 = Dpi.Width(8);
            Width9 = Dpi.Width(9);
            Width10 = Dpi.Width(10);
            Width11 = Dpi.Width(11);
            Width12 = Dpi.Width(12);
            Width13 = Dpi.Width(13);
            Width14 = Dpi.Width(14);
            Width15 = Dpi.Width(15);
            Width16 = Dpi.Width(16);
            Width17 = Dpi.Width(17);
            Width18 = Dpi.Width(18);
            Width20 = Dpi.Width(20);
            Width22 = Dpi.Width(22);
            Width24 = Dpi.Width(24);
            Width25 = Dpi.Width(25);
            Width26 = Dpi.Width(26);
            Width28 = Dpi.Width(28);
            Width32 = Dpi.Width(32);
            Width34 = Dpi.Width(34);
            Width80 = Dpi.Width(80);
            Width100 = Dpi.Width(100);

            Height1 = Dpi.Height(1);
            Height2 = Dpi.Height(2);
            Height3 = Dpi.Height(3);
            Height4 = Dpi.Height(4);
            Height5 = Dpi.Height(5);
            Height6 = Dpi.Height(6);
            Height7 = Dpi.Height(7);
            Height8 = Dpi.Height(8);
            Height9 = Dpi.Height(9);
            Height10 = Dpi.Height(10);
            Height11 = Dpi.Height(11);
            Height12 = Dpi.Height(12);
            Height13 = Dpi.Height(13);
            Height14 = Dpi.Height(14);
            Height15 = Dpi.Height(15);
            Height16 = Dpi.Height(16);
            Height17 = Dpi.Height(17);
            Height18 = Dpi.Height(18);
            Height20 = Dpi.Height(20);
            Height22 = Dpi.Height(22);
            Height23 = Dpi.Height(23);
            Height24 = Dpi.Height(24);
            Height25 = Dpi.Height(25);
            Height28 = Dpi.Height(28);
            Height32 = Dpi.Height(32);
            Height50 = Dpi.Height(50);
        }

        #region Static Sizes

        public static int Width1 = 1;
        public static int Width2 = 2;
        public static int Width3 = 3;
        public static int Width5 = 5;
        public static int Width4 = 4;
        public static int Width6 = 6;
        public static int Width7 = 7;
        public static int Width8 = 8;
        public static int Width9 = 9;
        public static int Width10 = 10;
        public static int Width11 = 11;
        public static int Width12 = 12;
        public static int Width13 = 13;
        public static int Width14 = 14;
        public static int Width15 = 15;
        public static int Width16 = 16;
        public static int Width17 = 17;
        public static int Width18 = 18;
        public static int Width20 = 20;
        public static int Width22 = 22;
        public static int Width24 = 24;
        public static int Width25 = 25;
        public static int Width26 = 26;
        public static int Width28 = 28;
        public static int Width32 = 32;
        public static int Width34 = 34;
        public static int Width80 = 80;
        public static int Width100 = 100;
        
        public static int Height1 = 1;
        public static int Height2 = 2;
        public static int Height3 = 3;
        public static int Height4 = 4;
        public static int Height5 = 5;
        public static int Height6 = 6;
        public static int Height7 = 7;
        public static int Height8 = 8;
        public static int Height9 = 9;
        public static int Height10 = 10;
        public static int Height11 = 11;
        public static int Height12 = 12;
        public static int Height13 = 13;
        public static int Height14 = 14;
        public static int Height15 = 15;
        public static int Height16 = 16;
        public static int Height17 = 17;
        public static int Height18 = 18;
        public static int Height20 = 20;
        public static int Height22 = 22;
        public static int Height23 = 23;
        public static int Height24 = 24;
        public static int Height25 = 25;
        public static int Height28 = 28;
        public static int Height32 = 32;
        public static int Height50 = 50;

        #endregion
    }
}
