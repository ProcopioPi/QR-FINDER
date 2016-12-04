using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Vision.SNIPPETS.CONV
{
    public unsafe sealed class BlobFinder
    {       
        private static double mu;
        private static byte* pBase;
        private static Random random;
        private static myPixel* pixel;
        private static List<Blob> blobs;
        private static BitmapData bmData;
        private static KnownColor[] names;
        private static List<Point> pointList;
        private static int minX, minY, maxX, maxY, i, j, edgeX, edgeY;
        
        /// <summary>
        /// Recieves a black and white image to start
        /// extracting the blobs from it
        /// </summary>
        /// <param name="bitmap">Black and white image</param>
        /// <returns>Image with colored blobs</returns>
        public static Bitmap Execute(Bitmap bitmap)
        {
            int x, y, blobIndex;
            Blob tmpBlob;
            Color blobColor;
            MyRender.SetImage(bitmap);                      // Instatiates image pointers
            Point pixelPt;

            random = new Random();            
            blobs = new List<Blob>();
            names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            
            bmData = MyRender.Image.LockBits(new Rectangle(0, 0, MyRender.Width, MyRender.Height), // Locks image memory 
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            
            pBase = (byte*)bmData.Scan0.ToPointer();        // Pointer of image in memory
            pixel = PixelAt(0, 0);                          // Retrieves pointer pixel X,Y (0,0)
            
            mu = 0;
            pixelPt = new Point();
            for (y = 1; y < bmData.Height - 1; y++)
            {
                pixel = PixelAt(0, y);
                for (x = 1 ; x < bmData.Width - 1; x++)
                {
                    if (pixel->R == 0 && pixel->G == 0 && pixel->B == 0)    // If pixel is black, starts a blob
                    {
                        pointList = new List<Point>();
                        blobColor = RandomColor();

                        minX = int.MaxValue;
                        minY = int.MaxValue;

                        maxX = int.MinValue;
                        maxY = int.MinValue;
                       
                        //Grabs blob
                        pixelPt.X = x;
                        pixelPt.Y = y;

                        pointList.Add(pixelPt);

                        blobIndex = 0;
                        while (blobIndex < pointList.Count)
                        {
                            TrackBlob(pointList[blobIndex], blobColor);
                            blobIndex++;
                        }
                                                
                        if ( pointList.Count > 3)
                        {
                            tmpBlob = new Blob(pointList, blobColor, new Point(minX, minY), new Point(maxX, maxY));
                            if (!tmpBlob.Undefined)
                            {
                                blobs.Add(tmpBlob);
                                mu += tmpBlob.Points.Count;
                            }
                        }
                    }
                    pixel++;
                }
            }
            mu = mu / blobs.Count;

            MyRender.Image.UnlockBits(bmData);
            
            return MyRender.Image;
        }
       
        /// <summary>
        /// It tracks the 8 neighbours of the pixel to see if any of them are black pixels
        /// as part of the blob
        /// </summary>
        /// <param name="pCenter">the center pixel that is going to be analized</param>
        /// <param name="blobColor">the color assigned to that specific blob</param>
        private static void TrackBlob(Point pCenter , Color blobColor)
        {
            if (pCenter.X > 0 && pCenter.Y > 0 && 
                pCenter.X + 2 < bmData.Width && pCenter.Y + 2 < bmData.Height)    // safe image edge
            {
                Point tmpPoint;
                
                edgeX = pCenter.X - 1;          // X start left corner
                edgeY = pCenter.Y - 1;          // Y start left corner

                for (j = 0; j < 3; j++)
                {
                    pixel = PixelAt(edgeX, j + edgeY);            // gets pixel at X,Y
                    for (i = 0; i < 3; i++)
                    {
                        if (pixel->R == 0 && pixel->G==0 && pixel->B==0 )   // only if pixel is black avoiding repetition
                        {
                            tmpPoint = new Point(i + edgeX, j + edgeY);

                            // GETS THE EDGES OF THE RECTANGLE CONTAINIG THE BLOB
                            if (tmpPoint.X < minX)
                                minX = tmpPoint.X;
                            if (tmpPoint.X > maxX)
                                maxX = tmpPoint.X;
                            if (tmpPoint.Y < minY)
                                minY = tmpPoint.Y;
                            if (tmpPoint.Y > maxY)
                                maxY = tmpPoint.Y;

                            pointList.Add(tmpPoint);
                            
                            // Paint the blob in a color to avoid repetition or loops
                            pixel->R = blobColor.R;
                            pixel->G = blobColor.G;
                            pixel->B = blobColor.B;

                            // Paint blob avoiding black color
                            if (blobColor.R == 0)
                                pixel->R = 200;                                                      
                        }
                        pixel++;
                    }
                }
            }
        }

        public static void Clear()
        {
            blobs.Clear();
        }

        private static myPixel* PixelAt(int x, int y)
        {
            return (myPixel*)(((byte*)pBase + y * bmData.Width * sizeof(myPixel)) + x * sizeof(myPixel));
        }

        private static void UpdateMu()
        {
            for (int i = 0; i < blobs.Count; i++)
            {
                mu += blobs[i].Points.Count;
            }
            mu = mu / blobs.Count;
        }
                
        private static Color RandomColor()
        {
            return Color.FromKnownColor(names[random.Next(names.Length)]);
        }

        public static List<Blob> Blobs
        {
            get { return BlobFinder.blobs; }
            set
            {
                BlobFinder.blobs = value;
                UpdateMu();
            }
        }

        public static void Remove(Blob aBlob)
        {
            BlobFinder.blobs.Remove( aBlob);
        }

        public static int Count
        {
            get { return BlobFinder.blobs.Count; }
        }

        public Blob this[int index]
        {
            get { return BlobFinder.blobs[index]; }
        }

        public static double Mu
        {
            get { return BlobFinder.mu; }
            set { BlobFinder.mu = value; }
        }

    }
}
