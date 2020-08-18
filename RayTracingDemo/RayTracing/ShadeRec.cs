using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// 击中点信息记录
    /// </summary>
    public class ShadeRec
    {
        private double t;
        private Vector3D p;
        private Vector3D normal;
        private Material material ;
        private bool isHit;
        private double u = 0;
        private double v = 0;

        public double T { get => t; set => t = value; }
        public bool IsHit { get => isHit; set => isHit = value; }
        public double U { get => u; set => u = value; }
        public double V { get => v; set => v = value; }
        internal Vector3D P { get => p; set => p = value; }
        internal Vector3D Normal { get => normal; set => normal = value; }
        internal Material Material { get => material; set => material = value; }
    }
}
