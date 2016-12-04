using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using Vision.SNIPPETS.MONADIC;

namespace Vision.SNIPPETS.THR
{
    public unsafe sealed class IAdaptative : Filter
    {
        private const float VAL_R = 0.299f;
        private const float VAL_G = 0.587f;
        private const float VAL_B = 0.114f;

        private static float[,] iimg;

        /// <summary>
        /// Adaptative threshold with Integral Image
        /// </summary>
        /// <param name="bitmap">the bitmap to binarize</param>
        /// <param name="maskSize">the size of the mask to use to binarize the pixels</param>
        /// <returns></returns>
        public static Bitmap Execute(Bitmap bitmap, int maskSize)
        {
            if (bitmap == null)
                return bitmap;
                        
            Init( IImage.Execute(bitmap) );

            float A, B, C, D,value,total;

            iimg = new float[bitmap.Width, bitmap.Height];
            
            for (Yi = (maskSize / 2) + 1; Yi < (bitmap.Height - maskSize/2); Yi++)
            {
                for (Xi = (maskSize / 2) + 1; Xi < (bitmap.Width - maskSize/2); Xi++)
                {
                    pixel = PixelAt(Xi, Yi, bitmap.Width);

                    A = IImage.Iimg[Xi - ((maskSize / 2) + 1),   Yi - (maskSize / 2 + 1)];
                    B = IImage.Iimg[Xi + (maskSize / 2),         Yi - (maskSize / 2 + 1)];
                    C = IImage.Iimg[Xi - ((maskSize / 2) + 1),   Yi + (maskSize / 2)];
                    D = IImage.Iimg[Xi + (maskSize / 2),         Yi + (maskSize / 2)];

                    value = (A + D) - (C + B);
                    value /= (maskSize * maskSize);
                    
                    total = (byte)(((pixel->R * IImage.VAL_R) + (pixel->G * IImage.VAL_G) + (pixel->B * IImage.VAL_B)));

                    pixel->R = (total > ((value * byte.MaxValue) * .98)) ? byte.MaxValue : byte.MinValue;
                    pixel->G = pixel->R;
                    pixel->B = pixel->R;

                    //IAdaptative.iimg[Xi, Yi] = (pixel->R < 128) ? byte.MaxValue : byte.MinValue;
                    
                    pixel++;               
                }
            }

            bitmap.UnlockBits(bmData);


            return bitmap;
        }

        public static float[,] Iimg
        {
            get { return IAdaptative.iimg; }
        }
    }
}
