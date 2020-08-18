using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// 射线
    /// </summary>
    public class Ray
    {
        private Point3D origin;
        private Vector3D direction;
        double _lastTime = 0;//射线存在时间范围（即物体运动时间）

        public Ray() { }

        public Ray(Point3D origin, Vector3D direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
        public Ray(Point3D origin, Vector3D direction,double time)
        {
            this.origin = origin;
            this.direction = direction;
            LastTime = time;
        }

        public double LastTime { get => _lastTime; set => _lastTime = value; }
        internal Point3D Origin { get => origin; set => origin = value; }
        internal Vector3D Direction { get => direction; set => direction = value; }

        public Vector3D PointAtPara(double t)
        {
            return origin + new Point3D(t * direction.X, t * direction.Y, t * direction.Z);
        }
    }
}
