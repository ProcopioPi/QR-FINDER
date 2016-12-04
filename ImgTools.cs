using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Vision.SNIPPETS
{
    public class ImgTools
    {
        public static Bitmap Scale(Image image, int maxWidth, int maxHeight)
        {
            double scale;
            Bitmap red;
            int nW, nH;

            scale = 1;
            if (image.Width != maxWidth || image.Height != maxHeight)
            {
                double scaleW, scaleH;

                scaleW = maxWidth / (double)image.Width;
                scaleH = maxHeight / (double)image.Height;

                scale = scaleW < scaleH ? scaleW : scaleH;
            }
            nW = (int)(image.Width * scale);
            nH = (int)(image.Height * scale);
            red = new Bitmap(nW, nH);

            Graphics.FromImage(red).DrawImage(image, 0, 0, (int)(image.Width * scale), (int)(image.Height * scale));

            return red;
        }
        public double FindAngle(Point A, Point B, Point C)
        {
            double angleRad, d12, d13, d23;
            angleRad = 0;

            d12 = Distance(A, B);
            d13 = Distance(A, C);
            d23 = Distance(B, C);

            angleRad = Math.Acos(((d12 * d12) + (d13 * d13) - (d23 * d23)) / (2 * d12 * d13));
            return angleRad * 180 / Math.PI;
        }

        public double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public  double Slope(Point A, Point B)
        {
            double slope;
            slope = 0;

            slope = (A.Y - B.Y) / (A.X - B.X);
            return slope;
        }
        public double FindAngles(Point v1, Point v2)
        {
            double angleRad = Math.Acos((v1.X * v2.X + v1.Y * v2.Y) / (Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y) * Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y)));
            return angleRad * 180 / Math.PI;//DEG
        }     

        public double DiamondAngle(int Px, int Py)
        {
            double x, y;
            y = (double)Py;
            x = (double)Px;

            if (y >= 0)
                return (x >= 0 ? y / (x + y) : 1 - x / (-x + y));
            else
                return (x < 0 ? 2 - y / (-x - y) : 3 + x / (x - y));
        }
    }
}
