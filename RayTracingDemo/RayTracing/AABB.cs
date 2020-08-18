using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// AABB包围盒
    /// </summary>
    public class AABB
    {
        private Vector3D _min=new Vector3D();//左下角盒顶点
        private Vector3D _max=new Vector3D();//右上角盒顶点
        public AABB()
        {

        }
        public AABB(Vector3D a,Vector3D b)
        {
            Min = a;
            Max = b;
        }

        internal Vector3D Min { get => _min; set => _min = value; }
        internal Vector3D Max { get => _max; set => _max = value; }
        public bool Hit(Ray r, double tMin)
        {
            for (int a = 0; a < 3; a++)
            {
                double invD = 1.0 / r.Direction.Get(a);
                double t0 = (Min.Get(a) - r.Origin.Get(a)) * invD;
                double t1 = (Max.Get(a) - r.Origin.Get(a)) * invD;
                if (invD < 0)
                    Swap<double>(ref t0, ref t1);
                tMin = t0 > tMin ? t0 : tMin;
                tMin = t1 < Double.MaxValue ? t1 : Double.MaxValue;
                if (Double.MaxValue <= tMin)
                    return false;
            }
            return true;
        }
        void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }
    }
}
