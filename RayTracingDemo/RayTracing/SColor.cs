using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// 颜色
    /// </summary>
   public  class SColor
    {
        private double r;  //未放宽的r值(0~1)
        private double g;  //未放宽的g值(0~1)
        private double b;  //未放宽的z值(0~1)

        public SColor() { }

        public SColor(double r, double g, double b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
        public SColor(Color color)
        {
            this.R = color.R / 255.0;
            this.G = color.G / 255.0;
            this.B = color.B / 255.0;
        }
        public SColor(Vector3D color)
        {
            this.R = color.X;
            this.G = color.Y;
            this.B = color.Z;
        }
        public double R
        {
            get { return r; }
            set
            {
                r = value;
            }
        }
        public double G
        {
            get { return g; }
            set
            {
                g = value;
            }
        }
        public double B
        {
            get { return b; }
            set
            {
                b = value;
            }
        }

        public static SColor operator +(SColor a, SColor b)
        {
            return new SColor(a.R + b.R, a.G + b.G, a.B + b.B);
        }
        public static SColor operator *(SColor a, SColor b)
        {
            return new SColor(a.R * b.R, a.G * b.G, a.B * b.B);
        }
        public static SColor operator *(double a, SColor b)
        {
            return new SColor(a * b.R, a * b.G, a * b.B);
        }
        public static SColor operator *(SColor a, double b)
        {
            return new SColor(a.R * b, a.G * b, a.B * b);
        }
        public static SColor operator /(SColor a, double b)
        {
            return new SColor(a.R / b, a.G / b, a.B / b);
        }

        public Color GetColor255()
        {
            if (R > 1)
            {
                R = 1;
            }
            if (G > 1)
            {
                G = 1;
            }
            if (B > 1)
            {
                B = 1;
            }
            if (R < 0)
            {
                R = 0;
            }
            if (G < 0)
            {
                G = 0;
            }
            if (B < 0)
            {
                B = 0;
            }
            return  Color.FromArgb((int)(R * 255.0), (int)(G * 255.0), (int)(B * 255.0));
        }
        public override string ToString()
        {
            return "{" + R.ToString() + "/" + G.ToString() + "/" + B.ToString() + "}";
        }
    }
}
