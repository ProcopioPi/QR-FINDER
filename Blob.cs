using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Vision.SNIPPETS
{
    public class Blob
    {
        private List<Point> blob;
        private Color tag;
        private Point start;
        private Point end;
        private Point centroid;
        private Point b;
        private Point c;
        private Size size;
        private float ratio;
        private bool undefined;
        private double density;
        private double distance;
        private double centroid2Origin;
        private double start2Origin;
        private double diagonal;
        private double avgSize;

        /// <summary>
        /// The blob object with the parameters used as extra information
        /// </summary>
        /// <param name="blob">the list of points which completes the blob</param>
        /// <param name="color">a color tag assigned to the blob</param>
        /// <param name="start">the left-upper corner of the blob</param>
        /// <param name="end">the right-bottom corner of the blob</param>
        public Blob(List<Point> blob,Color color,Point start,Point end)
        {
            this.blob = blob;
            this.tag = color;
            this.start = start;
            this.end = end;

            this.centroid.X = (start.X + end.X) / 2;
            this.centroid.Y = (start.Y + end.Y) / 2;

            this.size.Width = Math.Abs(start.X - end.X);
            this.size.Height = Math.Abs(start.Y - end.Y);
            
            this.centroid2Origin = DistanceToTheOrigin();
            this.start2Origin = StartToTheOrigin();
            
            if (this.size.Width < this.size.Height)
                this.ratio = (float)Math.Round(((double)this.size.Width / (double)this.size.Height), 5);
            else
                this.ratio = (float)Math.Round(((double)this.size.Height / (double)this.size.Width), 5);

            this.density = Math.Round((double)Points.Count / (this.size.Width * this.size.Height), 5);
            
            undefined = double.IsInfinity(this.density);
            if (!undefined)
                undefined = float.IsNaN(this.ratio);

            b = new Point((start.X + (int)size.Width), start.Y);
            c = new Point((start.X), start.Y + (int)size.Height);

            diagonal = Distance(start, end);
            avgSize = (Width + Height) / 2;
        }

        /// <summary>
        /// Computes the distance from the centroid of this blob 
        /// to the centroid of a given blob
        /// </summary>
        /// <param name="aBlob">the blob to which we compute the distance to its centroid</param>
        /// <returns></returns>
        public double Distance(Blob aBlob)
        {
            distance = Math.Sqrt(Math.Pow(this.centroid.X - aBlob.centroid.X, 2) + Math.Pow(this.centroid.Y - aBlob.centroid.Y, 2));
            return distance;
        }

        public double Distance(Point aPoint)
        {
            return Math.Sqrt(Math.Pow(this.centroid.X - aPoint.X, 2) + Math.Pow(this.centroid.Y - aPoint.Y, 2));
        }

        public double Distance(Point a,Point b)
        {
            return Math.Sqrt(Math.Pow(this.b.X - a.X, 2) + Math.Pow(this.b.Y - a.Y, 2));
        }

        private double DistanceToTheOrigin()
        {
            return Math.Sqrt(Math.Pow(this.centroid.X , 2) + Math.Pow(this.centroid.Y , 2));
        }

        private double StartToTheOrigin()
        {
            return Math.Sqrt(Math.Pow(this.start.X, 2) + Math.Pow(this.start.Y, 2));
        }

        public List<Point> Points
        {
            get { return blob; }
        }

        public override string ToString()
        {
            return name+" W: "+Width+" H: "+Height+ " avgS: "+avgSize+" D: "+density+" R: "+ratio;
        }

        public double AvgSize
        {
            get { return avgSize; }
            set { avgSize = value; }
        }

        public double Diagonal
        {
            get { return diagonal; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Point A
        {
            get { return start; }
        }
        public Point B
        {
            get { return b; }
        }
        public Point C
        {
            get { return c; }
        }
        public Point D
        {
            get { return end; }
        }

        public double Width
        {
            get { return size.Width; }
        }
        public double Height
        {
            get { return size.Height; }
        }

        public double Start2Origin
        {
            get { return start2Origin; }
        }

        public bool Undefined
        {
            get { return undefined; }
        }

        public float Ratio
        {
            get { return ratio; }
        }

        public double Centroid2Origin
        {
            get { return centroid2Origin; }
        }

        public double DistanceB
        {
            get { return distance; }
        }

        public double Density
        {
            get { return density; }
        }

        public Size Size
        {
            get { return size; }
        }

        public Point Centroid
        {
            get { return centroid; }
        }

        public Point End
        {
            get { return end; }
        }

        public Point Start
        {
            get { return start; }
        }

        public Color Tag
        {
            get { return tag; }
        }
    }
}
