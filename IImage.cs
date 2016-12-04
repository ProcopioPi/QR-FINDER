using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Vision.SNIPPETS.MONADIC
{
    public unsafe sealed class IImage : Filter
    {
        public const float VAL_R = 0.299f;
        public const float VAL_G = 0.587f;
        public const float VAL_B = 0.114f;
        public const float MY_BYTE = 255.0f;

        private static float[,] iimg;
        
        /// <summary>
        /// It computes the matrix with the values of 
        /// the integral image from a given bitmap
        /// </summary>
        /// <param name="bitmap">the bitmap wich it computes its integral values</param>
        /// <returns></returns>
        public static Bitmap Execute(Bitmap bitmap)
        {
            if (bitmap == null)
                return bitmap;

            float value;
            double total;

            MyRender.SetImage(bitmap);
            CanvasIMG.SetImage(bitmap);
            
            Init(bitmap);            

            iimg = new float[bitmap.Width, bitmap.Height];

            value = (((pixel->R * VAL_R) + (pixel->G * VAL_G) + (pixel->B * VAL_B)));
            total = value / MY_BYTE;

            iimg[0, 0] = (float)total;

            for (Xi = 1; Xi < bitmap.Width; Xi++)
            {
                value = (((pixel->R * VAL_R) + (pixel->G * VAL_G) + (pixel->B * VAL_B)));
                total = value / MY_BYTE;

                if (total > 0)
                {
                    iimg[Xi, 0] += (float)total + iimg[Xi - 1, 0];
                }
                pixel++;
            }

            pixel = PixelAt(0, 0, bitmap.Width);
            
            value = (((pixel->R * VAL_R) + (pixel->G * VAL_G) + (pixel->B * VAL_B)));
            total = value / MY_BYTE;
            
            for (Yi = 1; Yi < bitmap.Height; Yi++)
            {
                value = (((pixel->R * VAL_R) + (pixel->G * VAL_G) + (pixel->B * VAL_B)));
                total = value / MY_BYTE;

                if (total > 0)
                {
                    iimg[0, Yi] = (float)total + iimg[0, Yi - 1];
                }
                pixel += bitmap.Width;
            }
            
           
            for (Yi = 1; Yi < bitmap.Height; Yi++)
            {
                pixel = PixelAt(1, Yi, bitmap.Width);
                for (Xi = 1; Xi < bitmap.Width; Xi++)
                {
                    value = (((pixel->R * VAL_R) + (pixel->G * VAL_G) + (pixel->B * VAL_B)));
                    total = value / MY_BYTE;

                    iimg[Xi, Yi] += (float)total + (iimg[Xi - 1, Yi] - iimg[Xi - 1, Yi - 1]) + iimg[Xi, Yi - 1];

                    pixel++;
                }
            }
        
            bitmap.UnlockBits(bmData);

            return bitmap;
        }

        public static float[,] Iimg
        {
            get { return IImage.iimg; }
        }

    }
}
