using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// 移动
    /// </summary>
    class Translate:GeometryObject
    {
        GeometryObject geometry = new GeometryObject();
        Vector3D offset = new Vector3D();

        public GeometryObject Geometry { get => geometry; set => geometry = value; }
        public Vector3D Offset { get => offset; set => offset = value; }

        public Translate()
        {

        }
        public Translate(GeometryObject geometry,Vector3D displacement)
        {
            Geometry = geometry;
            offset = displacement;

        }
        public override bool Hit(Ray r,double t_min, ref ShadeRec rec)
        {
            Ray moved_r=new Ray(new Point3D(r.Origin-offset),r.Direction);
            if (Geometry.Hit(moved_r, t_min, ref rec))
            {
                rec.P += offset;
                return true;
            }
            else
                return false;
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            if (Geometry.Bounding_box(t0, t1, ref box))
            {
                box = new AABB(box.Min + offset, box.Max + offset);
                return true;
            }
            return false;
        }
    }
}
