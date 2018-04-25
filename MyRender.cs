using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using Vision.SNIPPETS.PIXELS;

namespace Vision.SNIPPETS
{
    public class MyRender
    {
        #region Properties
        private static short BYTES = 4;
        private static IntPtr ptr_Address;
        private static GCHandle handle;
        private static Graphics graphics;////************
     
        private static Bitmap image;////***** QUEDO !!!

        private static byte[] data;////************
        private static Histogram histogram;

        public static Histogram Histogram
        {
            get { return histogram; }
        }

        private static int widthVar, heightVar;

        public Size Size
        {
            get { return image.Size; }
        }

        public  int Lenght
        {
            get { return data.Length; }
        }

        public static int Width
        {
            get { return image.Width; }
        }

        public static int Height
        {
            get { return image.Height; }
        }

        public static Bitmap Image
        {
            get { return image; }
        }

        public static byte[] ImageData
        {
            get { return data; }
            set { data = value; }
        }

        public Graphics Graphics
        {
            get { return graphics; }
        }

        public static IntPtr Ptr_Address
        {
            get { return ptr_Address; }
        }
        #endregion

        public MyRender(Bitmap bmp)
        {
            widthVar = bmp.Width;
            heightVar = bmp.Height;
           
            Create(bmp);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        public static void SetImage(Bitmap bmp)
        {
            widthVar = bmp.Width;
            heightVar = bmp.Height;
            BYTES = 4;
             
            Create(bmp);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }
        
        private static void Create(Bitmap bmp)
        {
            CreateMe(bmp);
            //********************************************       
            LockBitmap.SetBitmap(bmp);
            LockBitmap.LockBits();
            Marshal.Copy(LockBitmap.Pixels, 0, ptr_Address, LockBitmap.Pixels.Length);
            LockBitmap.UnlockBits();
            LockBitmap.Pixels = new byte[0];
        }

        public void Dispose()
        {
            image.Dispose();
            histogram.Dispose();
            ptr_Address = IntPtr.Zero;
            data = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            graphics = null;
            histogram = null;
        }

        private static void CreateMe(Bitmap bmp)
        {
            try
            {
                if (data == null || data.Length != (widthVar * heightVar * BYTES))
                {
                    ptr_Address = IntPtr.Zero;
                    data = null;
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

                    data = new byte[widthVar * heightVar * BYTES];
                    handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                    ptr_Address = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
                }

                image = new Bitmap(widthVar, heightVar, BYTES * widthVar, PixelFormat.Format32bppPArgb, ptr_Address);
            }
            catch (Exception) { }
        }

        public static void GenerateHistogram()
        {
            histogram = new Histogram(data);
        }

        public static Color GetPixel(int x, int y)
        {
            byte R, G, B, A;
            int index = GetIndex(x, y);

            A = data[index + ARGB.A];
            R = data[index + ARGB.R];
            G = data[index + ARGB.G];
            B = data[index + ARGB.B];

            return Color.FromArgb(A, R, G, B);
        }

        public static void SetPixel(int x, int y, Color pixel)
        {
            int index = GetIndex(x, y);

            data[index + ARGB.A] = pixel.A;
            data[index + ARGB.R] = pixel.R;
            data[index + ARGB.G] = pixel.G;
            data[index + ARGB.B] = pixel.B;
        }

        public static void SetAlpha(int x, int y, byte alpha)
        {
            data[GetIndex(x, y) + ARGB.A] = alpha;
        }

        public static int GetIndex(int x, int y)
        {
            return (y * 4 * image.Width) + (x * 4);
        }
    }
}
