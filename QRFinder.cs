using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vision.SNIPPETS;
using Vision.SNIPPETS.CONV;
using Vision.SNIPPETS.MONADIC;
using Vision.SNIPPETS.THR;

namespace Vision.PRJ
{
    public enum SCALARS
    {
        NONE,
        SCALE
    }

    public struct Line
    {
        public Blob a, b;
        public double lenght;

        public Line(Blob a, Blob b)
        {
            this.a = a;
            this.b = b;

            lenght = a.Distance(b);
        }
    }

    public struct Triangle
    {
        public List<Blob> points;

        public List<Line> lines;        
        public Pen pen;
        public double diff;
        public double area;
        
        public Triangle(Blob one,Color color)
        {
            points = new List<Blob>();
            lines = new List<Line>();
            
            points.Add(one);
            diff = 0;
            area = 0;
            pen = new Pen(color,.4f);
        }        
    }

    public unsafe sealed class QRFinder
    {
        private static Bitmap bmp;
        private Pen pRed = new Pen(Color.Red, 2);
        private Pen pBlue = new Pen(Color.Blue, 2);
        private Pen pWhite = new Pen(Color.White, 2);
        private Pen pGreen = new Pen(Color.Green, 7);
        private static KnownColor[] names;
        private static Random random;
       
        /// <summary>
        /// The constructor of the QR finde, which starts by
        /// scaling the input image and binarizes the image using
        /// the integral image in adaptative threshold
        /// </summary>
        /// <param name="image">input image to analize</param>
        /// <param name="scale">an option to tell the software if we want to scale the input</param>
        /// <param name="maskSize">size of the adaptative threshold window</param>
        public QRFinder(Bitmap image,SCALARS scale,int maskSize)
        {
            names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            random = new Random();
            switch (scale)
            {
                case SCALARS.NONE:
                    bmp = image;
                    break;

                case SCALARS.SCALE:
                    bmp = ImgTools.Scale(image, 640, 480);
                    break;
            }

            bmp = IAdaptative.Execute(bmp, maskSize);
            BlobFinder.Execute(bmp);

        }

        /// <summary>
        /// Method that implements the sequential filters to obtain
        /// the QR marks pairs.
        /// </summary>
        /// <returns>Bitmap with the marked triangles</returns>
        public Bitmap AnalizeQRBlobs()
        {   
            List<Triangle> triangles;

            FilterMinimumDensityandRatio();             // FIRST FILTER

            FindBlobPairs();                            // SECOND FILTER: find pairs

            FilterAbnormalBlobs();                      // THIRD BLOB FILTER

            triangles = FindTriangles();                // FOURTH FILTER
                        
            bmp = new Bitmap(CanvasIMG.Image);
            bmp = GrayScale_Lum.Execute(bmp);

            for (int t = 0; t < triangles.Count; t++)
            {
                DrawTriangle(triangles[t], bmp);
            }

            GC.Collect();

            return bmp;
        }

        private void DrawFromScratch()
        {
            Point pointA, pointB, pointC, pointD, ptTmp;
            Point midPointA, midPointB, midPointC;
            int bs = 0;
            int distanceA, distanceB, distanceC;

            while (BlobFinder.Count > (bs + 2))
            {
                pointA = BlobFinder.Blobs[bs].Centroid;
                pointB = BlobFinder.Blobs[bs + 1].Centroid;
                pointC = BlobFinder.Blobs[bs + 2].Centroid;

                midPointC = MidPoint(pointA, pointB);
                midPointB = MidPoint(pointA, pointC);
                midPointA = MidPoint(pointB, pointC);

                distanceA = (int)Distance(pointA, midPointA);
                distanceB = (int)Distance(pointB, midPointB);
                distanceC = (int)Distance(pointC, midPointC);

                ptTmp = midPointA;

                if (distanceC < distanceB)
                {
                    if (distanceC < distanceA)
                    {
                        ptTmp = pointA;
                        pointA = pointC;
                        pointC = ptTmp;
                        ptTmp = midPointC;
                    }
                }
                else
                {
                    if (distanceB < distanceA)
                    {
                        ptTmp = pointA;
                        pointA = pointB;
                        pointB = ptTmp;
                        ptTmp = midPointB;
                    }
                }

                Graphics.FromImage(bmp).DrawLine(pRed, pointA, pointB);
                Graphics.FromImage(bmp).DrawLine(pRed, pointA, pointC);
                Graphics.FromImage(bmp).DrawLine(pRed, pointB, pointC);

                Graphics.FromImage(bmp).DrawEllipse(pWhite, pointA.X - 2, pointA.Y - 2, 5, 5);
                DrawPoint(bmp, pBlue, ptTmp);
                Graphics.FromImage(bmp).DrawLine(pBlue, pointA, ptTmp);
                Graphics.FromImage(bmp).DrawEllipse(pGreen, ptTmp.X - 2, ptTmp.Y - 2, 5, 5);

                pointD = new Point(pointB.X + (pointC.X - pointA.X), pointB.Y + (pointC.Y - pointA.Y));

                Graphics.FromImage(bmp).DrawEllipse(pGreen, pointD.X - 3, pointD.Y - 3, 7, 7);
                Graphics.FromImage(bmp).DrawEllipse(pRed, pointD.X - 2, pointD.Y - 2, 5, 5);

                bs += 3;
            }//*/
        }

        private void DrawTriangle(Triangle aTriangle,Bitmap bmp)
        {
            Point pointA, pointB, pointC, pointD, ptTmp;
            Point midPointA, midPointB, midPointC;
            int bs = 0;
            int distanceA, distanceB, distanceC;
            Pen p = aTriangle.pen;

            pointA = aTriangle.points[bs].Centroid;
            pointB = aTriangle.points[bs + 1].Centroid;
            pointC = aTriangle.points[bs + 2].Centroid;

            midPointC = MidPoint(pointA, pointB);
            midPointB = MidPoint(pointA, pointC);
            midPointA = MidPoint(pointB, pointC);

            distanceA = (int)Distance(pointA, midPointA);
            distanceB = (int)Distance(pointB, midPointB);
            distanceC = (int)Distance(pointC, midPointC);

            ptTmp = midPointA;

            if (distanceC < distanceB)
            {
                if (distanceC < distanceA)
                {
                    ptTmp = pointA;
                    pointA = pointC;
                    pointC = ptTmp;
                    ptTmp = midPointC;
                }
            }
            else
            {
                if (distanceB < distanceA)
                {
                    ptTmp = pointA;
                    pointA = pointB;
                    pointB = ptTmp;
                    ptTmp = midPointB;
                }
            }

            pointD = new Point(pointB.X + (pointC.X - pointA.X), pointB.Y + (pointC.Y - pointA.Y));

            Graphics.FromImage(bmp).DrawLine(pRed, pointA, pointB);
            Graphics.FromImage(bmp).DrawLine(pRed, pointA, pointC);
            Graphics.FromImage(bmp).DrawLine(pRed, pointB, pointC);

            Graphics.FromImage(bmp).DrawEllipse(pWhite, pointA.X - 2, pointA.Y - 2, 5, 5);
            DrawPoint(bmp, pBlue, ptTmp);
            Graphics.FromImage(bmp).DrawLine(pBlue, pointA, ptTmp);
            Graphics.FromImage(bmp).DrawEllipse(pGreen, ptTmp.X - 2, ptTmp.Y - 2, 5, 5);


            Graphics.FromImage(bmp).DrawEllipse(pGreen, pointD.X - 3, pointD.Y - 3, 7, 7);
            Graphics.FromImage(bmp).DrawEllipse(pRed, pointD.X - 2, pointD.Y - 2, 5, 5);

        }

        private void FilterMinimumDensityandRatio()
        {
            List<Blob> tmpBlobs;
            Blob tmpBlob;

            tmpBlobs = new List<Blob>();
            pBlue.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            //FIRST FILTERS OF PROPORTIONS FOR SQUARED BLOBS
            for (int i = 0; i < BlobFinder.Count; i++)
            {
                tmpBlob = BlobFinder.Blobs[i];

                if (tmpBlob.Density < 0.25)
                    continue;

                if (tmpBlob.Ratio < 0.15)
                    continue;

                tmpBlobs.Add(tmpBlob);
            }

            BlobFinder.Clear();
            BlobFinder.Blobs.AddRange(tmpBlobs.OrderBy(b => b.Centroid2Origin).ToList());
        }

        private void FindBlobPairs()
        {
            List<Blob> tmpBlobs;
            float maxR, minR;
            Blob one, two, tmp;
            Point p1, p2, p3, p4, midAB, midAC;

            tmpBlobs = new List<Blob>();
            maxR = 0;
            minR = 1000;

            //FINDS PAIRS OF BLOBS ONE INSIDE ANOTHER WITH CLOSE CENTROIDS
            for (int index = 0; index < BlobFinder.Count - 1; index++)
            {
                one = BlobFinder.Blobs[index];

                for (int t = index + 1; t < BlobFinder.Count; t++)
                {
                    two = BlobFinder.Blobs[t];

                    if (Math.Abs(one.Centroid2Origin - two.Centroid2Origin) > one.Diagonal / 3)
                    {
                        t = BlobFinder.Count;
                        BlobFinder.Blobs.Remove(one);
                        index--;
                        continue;
                    }//*/

                    if (one.Width < two.Width)
                    {
                        tmp = one;
                        one = two;
                        two = tmp;
                    }

                    midAB = MidPoint(one.A, one.B);
                    midAC = MidPoint(one.A, one.C);

                    p1 = MidPoint(one.A, midAB);
                    p2 = MidPoint(midAB, one.B);

                    p3 = MidPoint(one.A, midAC);
                    p4 = MidPoint(midAC, one.C);

                    if (
                            (two.Centroid.X > p1.X) && (two.Centroid.X < p2.X) &&
                            (two.Centroid.Y > p3.Y) && (two.Centroid.Y < p4.Y) &&
                            (one.Density / two.Density) < 1.3 && (two.Density / one.Density) > 1 &&
                            (one.Width / two.Width) > 1.5 && (one.Width / two.Width) < 3.5
                        )
                    {
                        tmpBlobs.Add(two);

                        if (two.Ratio > maxR)
                            maxR = two.Ratio;

                        if (two.Ratio < minR)
                            minR = two.Ratio;

                        BlobFinder.Blobs.Remove(one);
                        BlobFinder.Blobs.Remove(two);

                        index--;
                        t = BlobFinder.Count;
                    }
                    else
                    {
                        one = BlobFinder.Blobs[index];
                    }
                }
            }

            BlobFinder.Clear();
            BlobFinder.Blobs.AddRange(tmpBlobs.OrderBy(b => b.Centroid2Origin).ToList());
        }

        private static void FilterAbnormalBlobs()
        {
            int maxW, minW, avg;

            if (BlobFinder.Count > 0)
            {
                maxW = (int)BlobFinder.Blobs.Max(c => c.AvgSize);
                minW = (int)BlobFinder.Blobs.Min(c => c.AvgSize);
                avg = (int)BlobFinder.Blobs.Average(c => c.AvgSize);

                BlobFinder.Blobs.RemoveAll(b => b.AvgSize > (2 * (maxW + avg) / 3));
                maxW = (int)BlobFinder.Blobs.Max(c => c.AvgSize);
                BlobFinder.Blobs.RemoveAll(b => b.AvgSize < ((maxW - minW) / 2));

                BlobFinder.Blobs = BlobFinder.Blobs.OrderBy(b => b.Centroid2Origin).ThenBy(b => b.AvgSize).ToList();
            }
        }

        private List<Triangle> FindAllTriangles()
        {
            Blob one, two, three;
            List<Triangle> triangles;
            List<double> sizes;
            Triangle tmpTria;
            Point pointA, pointB, pointC, pointD, ptTmp;
            Point midPointA, midPointB, midPointC;  
            int distanceA, distanceB, distanceC, maxX,minX,maxY,minY,width,height;

            triangles = new List<Triangle>();

            IImage.Execute(bmp);

            for (int index = 0; index < BlobFinder.Count; index++)
            {
                one = BlobFinder.Blobs[index];

                for (int next = index + 1; next < BlobFinder.Count-1; next++)
                {
                    maxX = int.MinValue;
                    minX = int.MaxValue;
                    maxY = int.MinValue;
                    minY = int.MaxValue;

                    sizes = new List<double>();
                    two = BlobFinder.Blobs[next];
                    three = BlobFinder.Blobs[next+1];

                    pointA = one.Centroid;
                    pointB = two.Centroid;
                    pointC = three.Centroid;

                    midPointC = MidPoint(pointA, pointB);
                    midPointB = MidPoint(pointA, pointC);
                    midPointA = MidPoint(pointB, pointC);

                    distanceA = (int)Distance(pointA, midPointA);
                    distanceB = (int)Distance(pointB, midPointB);
                    distanceC = (int)Distance(pointC, midPointC);

                    ptTmp = midPointA;

                    if (distanceC < distanceB)
                    {
                        if (distanceC < distanceA)
                        {
                            ptTmp = pointA;
                            pointA = pointC;
                            pointC = ptTmp;
                            ptTmp = midPointC;
                        }
                    }
                    else
                    {
                        if (distanceB < distanceA)
                        {
                            ptTmp = pointA;
                            pointA = pointB;
                            pointB = ptTmp;
                            ptTmp = midPointB;
                        }
                    }
                    pointD = new Point(pointB.X + (pointC.X - pointA.X), pointB.Y + (pointC.Y - pointA.Y));   
                    float A, B, C, D,value;
                                        
                    maxX = (pointA.X > maxX) ? pointA.X : maxX;
                    maxX = (pointB.X > maxX) ? pointB.X : maxX;
                    maxX = (pointC.X > maxX) ? pointC.X : maxX;
                    maxX = (pointD.X > maxX) ? pointD.X : maxX;

                    minX = (pointA.X < minX) ? pointA.X : minX;
                    minX = (pointB.X < minX) ? pointB.X : minX;
                    minX = (pointC.X < minX) ? pointC.X : minX;
                    minX = (pointD.X < minX) ? pointD.X : minX;

                    maxY = (pointA.Y > maxY) ? pointA.Y : maxY;
                    maxY = (pointB.Y > maxY) ? pointB.Y : maxY;
                    maxY = (pointC.Y > maxY) ? pointC.Y : maxY;
                    maxY = (pointD.Y > maxY) ? pointD.Y : maxY;

                    minY = (pointA.Y < minY) ? pointA.Y : minY;
                    minY = (pointB.Y < minY) ? pointB.Y : minY;
                    minY = (pointC.Y < minY) ? pointC.Y : minY;
                    minY = (pointD.Y < minY) ? pointD.Y : minY;
                    
                    if (minX > 0 && minY > 0)
                    {
                        width = maxX - minX;
                        height = maxY - minY;

                        A = IImage.Iimg[minX - 1, minY - 1];
                        B = IImage.Iimg[maxX, minY - 1];
                        C = IImage.Iimg[minX - 1, maxY];
                        D = IImage.Iimg[maxX, maxY];

                        value = (A + D) - (C + B);
                        value /= (width * height);

                        if (value  < .6 && Math.Abs(width-height) < 90)
                        {
                            tmpTria = new Triangle(one, Color.Red);
                            tmpTria.points.Add(two);
                            tmpTria.points.Add(three);

                            tmpTria.lines.Add(new Line(one, two));
                            tmpTria.lines.Add(new Line(one, three));
                            tmpTria.lines.Add(new Line(three, two));

                            tmpTria.lines.OrderByDescending(l => l.lenght);

                            tmpTria.area = (tmpTria.lines[2].lenght * tmpTria.lines[0].lenght) / 2;
                            triangles.Add(tmpTria);
                        }
                    }

                }
            }

            return triangles;
        }

        private List<Line> FindLines()
        {
            Blob one, two;
            Line line;
            List<List<Line>> lines;
            List<Line> linesList;

            lines = new List<List<Line>>();
            for (int index = 0; index < BlobFinder.Count; index++)
            {
                one = BlobFinder.Blobs[index];
                linesList = new List<Line>();

                for (int next = index + 1; next < BlobFinder.Count; next++)
                {
                    two = BlobFinder.Blobs[next];
                    line = new Line(one, two);

                    linesList.Add(line);
                }
                if (linesList.Count > 0)
                    lines.Add(linesList);
            }

            return lines[0];
        }

        private List<Triangle> FindTriangles()
        {
            Blob one, two;
            double distance;
            List<Triangle> triangles;
            List<Blob> tmpBlobs;

            triangles = new List<Triangle>();
            tmpBlobs = new List<Blob>();
            Triangle triangle;
            for (int index = 0; index < BlobFinder.Count; index++)
            {
                one = BlobFinder.Blobs[index];
                triangle = new Triangle(one,Color.Red);
                tmpBlobs.Add(one);
                
                for (int next = index + 1; next < BlobFinder.Count; next++)
                {
                    two = BlobFinder.Blobs[next];

                    distance = Distance(one.Centroid, two.Centroid);

                    if ((distance / ((one.AvgSize+two.AvgSize)/2)) > 10)//works
                    {
                        next = BlobFinder.Count;
                        BlobFinder.Remove(one);
                        tmpBlobs.Remove(one);
                        index--;
                    }
                    else
                    {
                        tmpBlobs.Add(two);
                        triangle.points.Add(two);
                        BlobFinder.Remove(two);
                        next--;
                        if (triangle.points.Count > 2)
                        {
                            triangles.Add(triangle);
                            next = BlobFinder.Count;
                        }
                    }
                }
            }

            BlobFinder.Clear();
            BlobFinder.Blobs.AddRange(tmpBlobs.ToList());

            return triangles;
        }

        public double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public Point MidPoint(Point a, Point b)
        {
            return new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }

        public void DrawPoint(Bitmap bmp, Pen pen, Point p)
        {
            int size;

            size = 5;
            Graphics.FromImage(bmp).DrawLine(pen, p.X - size, p.Y - size, p.X + size, p.Y + size);
            Graphics.FromImage(bmp).DrawLine(pen, p.X + size, p.Y - size, p.X - size, p.Y + size);
        }

        private static Color RandomColor()
        {
            return Color.FromKnownColor(names[random.Next(names.Length)]);
        }
    }
}
