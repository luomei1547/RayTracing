using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// 三维向量实体类
    /// 其中包含了一些对向量的运算符重定义
    /// </summary>
   public class Vector3D
    {
        private double x;
        private double y;
        private double z;

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Z { get => z; set => z = value; }

        public Vector3D() { }

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
        public Vector3D(double x_, double y_, double z_)
        {
            this.x = x_;
            this.y = y_;
            this.z = z_;
        }

        public Vector3D(Vector3D v_)
        {
            this.x = v_.x;
            this.y = v_.y;
            this.z = v_.z;
        }

        public Vector3D(Point3D vector3)
        {
            this.x = vector3.X;
            this.y = vector3.Y;
            this.z = vector3.Z;
        }
        public Vector3D(SColor vector3)
        {
            this.x = vector3.R;
            this.y = vector3.G;
            this.z = vector3.B;
        }
        /// <summary>
        /// 得到该向量的模
        /// </summary>
        /// <returns>返回向量的模</returns>
        public double Length()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// 向量模的平方
        /// </summary>
        /// <returns></returns>
        public double SquaredLength()
        {
            return x * x + y * y + z * z;
        }
        /// <summary>
        /// 使该向量单位化
        /// </summary>
        public void MakeUnitVector()
        {
            double k = 1 / Length();
            x *= k;
            y *= k;
            z *= k;
        }

        //对向量的运算符重载
        #region

        /// <summary>
        /// 重载+运算符，向量的加法
        /// </summary>
        /// <param name="a">向量加数a</param>
        /// <param name="b">向量加数b</param>
        /// <returns>向量a和向量b相加的结果</returns>
        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        /// <summary>
        /// 点与向量相加左
        /// </summary>
        /// <param name="a">点a</param>
        /// <param name="b">向量b</param>
        /// <returns>结果是一个向量</returns>
        public static Vector3D operator +(Point3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// 点与向量相加右
        /// </summary>
        /// <param name="a">向量a</param>
        /// <param name="b">点b</param>
        /// <returns>结果是一个向量</returns>
        public static Vector3D operator +(Vector3D a, Point3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        /// <summary>
        /// 点与向量相减左
        /// </summary>
        /// <param name="a">点a</param>
        /// <param name="b">向量b</param>
        /// <returns>结果是一个向量</returns>
        public static Vector3D operator -(Point3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vector3D operator -( Vector3D b)
        {
            return new Vector3D(- b.X,- b.Y, - b.Z);
        }

        /// <summary>
        /// 点与向量相减右
        /// </summary>
        /// <param name="a">向量a</param>
        /// <param name="b">点b</param>
        /// <returns>结果是一个向量</returns>
        public static Vector3D operator -(Vector3D a, Point3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        /// <summary>
        /// 重载-运算符，向量的减法
        /// </summary>
        /// <param name="a">向量被减数a</param>
        /// <param name="b">向量减数b</param>
        /// <returns>向量a和向量b相减的结果</returns>
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// 重载*运算符，向量左乘以一个数
        /// </summary>
        /// <param name="a">向量a</param>
        /// <param name="b">数b</param>
        /// <returns>向量a和数b相乘的结果</returns>
        public static Vector3D operator *(Vector3D a, double b)
        {
            return new Vector3D(a.X * b, a.Y * b, a.Z * b);
        }

        /// <summary>
        /// 重载*运算符，向量右乘以一个数
        /// </summary>
        /// <param name="a">数a</param>
        /// <param name="b">向量b</param>
        /// <returns>数a和向量b相乘的结果</returns>
        public static Vector3D operator *(double a, Vector3D b)
        {
            return new Vector3D(a * b.X, a * b.Y, a * b.Z);
        }

        /// <summary>
        /// 重载*运算符,向量对应的坐标相乘
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3D operator *(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        /// <summary>
        /// 重载/运算符，向量除以一个数
        /// </summary>
        /// <param name="a">向量a</param>
        /// <param name="b">除数b</param>
        /// <returns>向量a和数b相除的结果</returns>
        public static Vector3D operator /(Vector3D a, double b)
        {
            return new Vector3D(a.X / b, a.Y / b, a.Z / b);
        }

        /// <summary>
        /// 重载/运算符,向量对应的坐标相除
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3D operator /(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        #endregion


        //向量的一些运算
        #region

        /// <summary>
        /// 向量的点积
        /// </summary>
        /// <param name="a">向量a</param>
        /// <param name="b">向量b</param>
        /// <returns>向量a和向量b的点积结果</returns>
        public static double DotProduct(Vector3D a, Vector3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// 向量的叉积
        /// </summary>
        /// <param name="a">向量a</param>
        /// <param name="b">向量b</param>
        /// <returns>向量a和向量b的差积结果</returns>
        public static Vector3D CrossProduct(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }

        /// <summary>
        /// 单位化向量
        /// </summary>
        /// <param name="v">需要进行单位化的向量v</param>
        /// <returns>单位化向量结果</returns>
        public static Vector3D UnitVector(Vector3D v)
        {
            return v / v.Length();
        }
        #endregion

        public override string ToString()
        {
            return "{" + X.ToString() + "/" + Y.ToString() + "/" + Z.ToString() + "}";
        }
    }
}
