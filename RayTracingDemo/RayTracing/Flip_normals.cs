using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// 颠倒物体法线的向量
    /// </summary>
    class Flip_normals:GeometryObject
    {
        GeometryObject geometry=new GeometryObject();
        public Flip_normals()
        {

        }
        public Flip_normals(GeometryObject geometry)
        {
            Geometry = geometry;
        }
        public GeometryObject Geometry { get => geometry; set => geometry = value; }
        public override bool Hit(Ray r,double t_min, ref ShadeRec rec)
        {
            if (Geometry.Hit(r,t_min, ref rec))
            {
                rec.Normal = rec.Normal*(-1);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            return geometry.Bounding_box(t0,t1,ref box);
        }
    }
}
