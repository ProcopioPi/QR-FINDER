using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vision.SNIPPETS
{
    public struct myPixel
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;

        // Constructor:
        public myPixel(byte red, byte green, byte blue, byte alpha)
        {
            this.R = red;
            this.G = green;
            this.B = blue;
            this.A = alpha;
        }

        // Override the ToString method:
        public override string ToString()
        {
            return (String.Format("({0},{1},{2},{3})", R, G, B, A));
        }
    }

    public class Pixel
    {
        public static int ALPHA = 3;
        public static int RED = 2;
        public static int GREEN = 1;
        public static int BLUE = 0;

        unsafe public static int Pixel2Int(myPixel* pixel)
        {
            return *(int*)&pixel;
        }
        unsafe public static myPixel Int2Pixel(int pixel)
        {
            return *(myPixel*)&pixel;
        }
    }
}
