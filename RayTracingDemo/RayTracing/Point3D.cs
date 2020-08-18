using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// 三维空间点
    /// </summary>
   public  class Point3D
    {
        private double x;  //对应点x的值
        private double y;  //对应点y的值
        private double z;  //对应点z的值

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Z { get => z; set => z = value; }

        public Point3D() { }

        public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double Get(int i)
        {
            if (i == 0)
            {
                return X;
            }
            else if (i == 1)
            {
                return Y;
            }
            else if (i == 2)
            {
                return Z;
            }
            return 0;
        }
        public Point3D(Vector3D vector3)
        {
            this.x = vector3.X;
            this.y = vector3.Y;
            this.z = vector3.Z;
        }
        public Point3D(Point3D vector3)
        {
            this.x = vector3.X;
            this.y = vector3.Y;
            this.z = vector3.Z;
        }
        /// <summary>
        /// 两个点相减得到这两个点构成的向量
        /// </summary>
        /// <param name="a">末尾的点a</param>
        /// <param name="b">开始的点b</param>
        /// <returns>两个点组成的向量</returns>
        public static Vector3D operator -(Point3D a, Point3D b)
        {
            return new Vector3D(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        /// <summary>
        /// 两个点相加得到向量
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3D operator +(Point3D a, Point3D b)
        {
            return new Vector3D(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Point3D operator *(Point3D a, double b)
        {
            return new Point3D(a.x *b, a.y *b, a.z* b);
        }
        public static Point3D operator *( double b, Point3D a)
        {
            return new Point3D(a.x * b, a.y * b, a.z * b);
        }
        public override string ToString()
        {
            return "{" + X.ToString() + "/" + Y.ToString() + "/" + Z.ToString() + "}";
        }
    }
}
