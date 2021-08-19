using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar.Controls
{
    internal struct IconInfo
    {
        public bool fIcon;
        public int xHotspot;
        public int yHotspot;
        public IntPtr hbmMask;
        public IntPtr hbmColor;
    }
    internal class CursorHelper
    {
        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr handle);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);
        public static Cursor CreateCursor(Bitmap bm, int xHotspot, int yHotspot, bool resize = true)
        {
            IntPtr ptr = (resize) ? ((Bitmap)ResizeBitmap(bm, 32, 32)).GetHicon() : bm.GetHicon();
            IconInfo inf = new IconInfo();
            GetIconInfo(ptr, ref inf);
            inf.xHotspot = xHotspot;
            inf.yHotspot = yHotspot;
            inf.fIcon = false;
            IntPtr cursorPtr = CreateIconIndirect(ref inf);
            if (inf.hbmColor != IntPtr.Zero) { DeleteObject(inf.hbmColor); }
            if (inf.hbmMask != IntPtr.Zero) { DeleteObject(inf.hbmMask); }
            if (ptr != IntPtr.Zero) { DestroyIcon(ptr); }
            Cursor c = new Cursor(cursorPtr);
            c.Tag = (resize) ? new Size(32, 32) : bm.Size;
            return c;
        }
        public static Bitmap ResizeBitmap(Image image, int maxWidth, int maxHeight)
        {
            double ratio = System.Math.Min((double)maxHeight / image.Height, (double)maxWidth / image.Width);
            var propWidth = (int)(image.Width * ratio);
            var propHeight = (int)(image.Height * ratio);
            var newImage = new Bitmap(propWidth, propHeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, propWidth, propHeight);
            }
            return newImage;
        }
        public static Bitmap GetControlBitmap(Control c, Color transparent)
        {
            var bm = new Bitmap(c.Width, c.Height);
            c.DrawToBitmap(bm, new Rectangle(0, 0, c.Width, c.Height));
            if (!transparent.IsEmpty)
            {
                bm.MakeTransparent(transparent);
            }
            return bm;
        }
        public static Bitmap OverlayBitmap(Bitmap baseBitmap, Bitmap overlay, Point atPosition)
        {
            using (var g = Graphics.FromImage(baseBitmap))
            {
                g.DrawImage(overlay, new Rectangle(atPosition, overlay.Size));
            }
            return baseBitmap;
        }
    }
}
