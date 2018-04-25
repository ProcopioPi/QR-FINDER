using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

namespace Vision.SNIPPETS
{
    public unsafe class Filter
    {
        protected static int Xi, Yi, length;
        protected static byte data, dataR, dataG, dataB;
        protected static BitmapData bmData, bmTemp;
        protected static Bitmap bmp;

        protected static byte* pBase, pTemp;
        protected static myPixel* pixel, pixT;

        protected static void Init(Bitmap img)
        {
            bmData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            pBase = (byte*)bmData.Scan0.ToPointer();
            pixel = PixelAt(0, 0, pBase, bmData.Width);
            length = (bmData.Height * bmData.Width);
        }

        protected static void InitTemp()
        {
            bmp = new Bitmap(bmData.Width, bmData.Height);
            bmTemp = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            pTemp = (byte*)bmTemp.Scan0.ToPointer();
            pixT = PixelAt(0, 0, pTemp, bmTemp.Width);
        }
        protected static void InitTemp(Bitmap second)
        {
            bmp = new Bitmap(second);
            bmTemp = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            pTemp = (byte*)bmTemp.Scan0.ToPointer();
            pixT = PixelAt(0, 0, pTemp, bmTemp.Width);
        }

        protected static myPixel* PixelAt(int x, int y, byte* pBase, int width)
        {
            return (myPixel*)(((byte*)pBase + y * width * sizeof(myPixel)) + x * sizeof(myPixel));
        }

        protected static myPixel* PixelAt(int x, int y, int width)
        {
            return (myPixel*)(((byte*)pBase + y * width * sizeof(myPixel)) + x * sizeof(myPixel));
        }

        public static byte ClampByte(float value)
        {
            return (byte)((value < byte.MinValue) ? byte.MinValue : (value > byte.MaxValue) ? byte.MaxValue : value);
        }

    }
}
