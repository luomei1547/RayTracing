using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// 相机类
    /// </summary>
   public  class Camera
    {
        private Point3D lowerLeftCorner = new Point3D(-2, -1, -1);
        private Vector3D horizontal = new Vector3D(4, 0, 0);
        private Vector3D vertical = new Vector3D(0, 2, 0);
        private Point3D origin = new Point3D(0, 0, 0);
        private double lens_radius = 0;
        Vector3D u, v, w;

        //运动模糊
        //增加开始时间和结束时间
        double time0, time1;

        public Camera()
        {
            LowerLeftCorner = new Point3D(-2, -1, -1);
            Horizontal = new Vector3D(4, 0, 0);
            Vertical = new Vector3D(0, 2, 0);
            Origin = new Point3D(0, 0, 0);
        }

        //lookfrom为相机位置，lookat为观察位置，vup传(0,1,0)，vfov为视野角度，aspect为屏幕宽高比
        public  Camera(Point3D lookfrom, Vector3D lookat, Vector3D vup, double vfov, double aspect)
        {
            double theta = vfov * Math.PI / 180;
            double half_height = Math.Tan(theta / 2);//假设相机离屏幕距离为1时的屏幕半高计算
            double half_width = aspect * half_height;//宽高比*半高=半宽
            origin = lookfrom;//相机视点改变
            w = Vector3D.UnitVector(lookfrom - lookat);
            u = Vector3D.UnitVector(Vector3D.CrossProduct(vup, w));
            v = Vector3D.CrossProduct(w, u);
            LowerLeftCorner = new Point3D(origin - half_width * u -half_height * v - w);
            horizontal = 2 * half_width * u;
            vertical = 2 * half_height * v;
        }
        //lookfrom为相机位置，lookat为观察位置，vup传(0,1,0)，vfov为视野角度，aspect为屏幕宽高比
        //aperture为光圈大小，focus_dist为相机到观察点的距离
        public Camera(Point3D lookfrom, Vector3D lookat, Vector3D vup, double vfov, double aspect, double aperture, double focus_dist)
        {
           
            Lens_radius = aperture / 2;
            double theta = vfov * Math.PI / 180;
            double half_height = Math.Tan(theta / 2);//假设相机离屏幕距离为1时的屏幕半高计算
            double half_width = aspect * half_height;//宽高比*半高=半宽
            origin = lookfrom;//相机视点改变
            w = Vector3D.UnitVector(lookfrom - lookat);
            u = Vector3D.UnitVector(Vector3D.CrossProduct(vup, w));
            v = Vector3D.CrossProduct(w, u);
            LowerLeftCorner = new Point3D(origin - half_width * focus_dist * u - half_height * focus_dist * v - focus_dist * w);
            horizontal = 2 * half_width * focus_dist * u;
            vertical = 2 * half_height * focus_dist * v;
        }
        //lookfrom为相机位置，lookat为观察位置，vup传(0,1,0)，vfov为视野角度，aspect为屏幕宽高比
        //aperture为光圈大小，focus_dist为相机到观察点的距离
        //to为运动相机起始时间，t1为运动相机结束时间
        public Camera(Point3D lookfrom, Vector3D lookat, Vector3D vup, double vfov, double aspect, double aperture, double focus_dist,double t0,double t1)
        {
            Time0 = t0;
            Time1 = t1;
            Lens_radius = aperture / 2;
            double theta = vfov * Math.PI / 180;
            double half_height = Math.Tan(theta / 2);//假设相机离屏幕距离为1时的屏幕半高计算
            double half_width = aspect * half_height;//宽高比*半高=半宽
            origin = lookfrom;//相机视点改变
            w = Vector3D.UnitVector(lookfrom - lookat);
            u = Vector3D.UnitVector(Vector3D.CrossProduct(vup, w));
            v = Vector3D.CrossProduct(w, u);
            LowerLeftCorner = new Point3D(origin - half_width * focus_dist * u - half_height * focus_dist * v - focus_dist * w);
            horizontal = 2 * half_width * focus_dist * u;
            vertical = 2 * half_height * focus_dist * v;
        }
        public double Lens_radius { get => lens_radius; set => lens_radius = value; }
        public double Time0 { get => time0; set => time0 = value; }
        public double Time1 { get => time1; set => time1 = value; }
        internal Point3D LowerLeftCorner { get => lowerLeftCorner; set => lowerLeftCorner = value; }
        internal Vector3D Horizontal { get => horizontal; set => horizontal = value; }
        internal Vector3D Vertical { get => vertical; set => vertical = value; }
        internal Point3D Origin { get => origin; set => origin = value; }

        public Ray GetRay(double u, double v)
        {
            return new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
        }
        /// <summary>
        /// 景深效果射线获取
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Ray GetRayWith_Lens_radius(double s, double t)
        {
            Vector3D rd = lens_radius * RTUtils.RandomInUnitDisk();
            Vector3D offset = u * rd.X + v * rd.Y;
            return new Ray(new Point3D(origin + offset), LowerLeftCorner + s * horizontal + t * vertical - (origin + offset));
        }
        /// <summary>
        /// 运动模糊效果射线获取(含景深效果)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Ray GetRayWith_Time(double s, double t)
        {
            Vector3D rd = lens_radius * RTUtils.RandomInUnitDisk();
            Vector3D offset = u * rd.X + v * rd.Y;
            //随机时间戳的光线持续时长
            double time = time0 +RTUtils.rd.NextDouble() * (time1 - time0);
            return new Ray(new Point3D(origin + offset), LowerLeftCorner + s * horizontal + t * vertical - (origin + offset),time);
        }

    }
}
