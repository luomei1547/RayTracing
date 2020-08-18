using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// 绕物体自身中心X轴转动
    /// </summary>
    class Rotate_self_X : GeometryObject
    {
        GeometryObject geometry = new GeometryObject();
        double sin_theta = 0;
        double cos_theta = 1;

        public GeometryObject Geometry { get => geometry; set => geometry = value; }
        public double Sin_theta { get => sin_theta; set => sin_theta = value; }
        public double Cos_theta { get => cos_theta; set => cos_theta = value; }

        public Rotate_self_X()
        {

        }
        public Rotate_self_X(GeometryObject geometry, double angle)
        {
            Geometry = geometry;
            if (angle != 0)
            {
                double radians = (Math.PI * angle) / 180.0;
                Sin_theta = Math.Sin(radians);
                Cos_theta = Math.Cos(radians);
            }
        }

        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            ShadeRec SHA = new ShadeRec();
            if (Geometry.Hit(r, t_min, ref SHA))
            {
                //求得交点后再往正的方向移

                Vector3D tempN = new Vector3D();

                Vector3D normal = new Vector3D(SHA.Normal);

                tempN.Y = Cos_theta * normal.Y + Sin_theta * normal.Z;
                tempN.Z = -Sin_theta * normal.Y + Cos_theta * normal.Z;
                tempN.X = normal.X;

                double theta = Math.Acos(tempN.Y);
                double phi = Math.Atan2(tempN.X, tempN.Z);
                if (phi < 0)
                {
                    phi += 2.0 * Math.PI;
                }
                double u = phi / (2.0 * Math.PI);
                double v = theta / Math.PI;
                SHA.U = u;
                SHA.V = v;
                SHA.IsHit = true;
            }
            else
            {
                SHA.IsHit = false;
            }
            if (SHA.IsHit)
            {
                rec = SHA;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            return geometry.Bounding_box(t0, t1, ref box);
        }
    }

    /// <summary>
    /// 绕物体自身中心Y轴转动
    /// </summary>
    class Rotate_self_Y : GeometryObject
    {
        GeometryObject geometry = new GeometryObject();
        double sin_theta = 0;
        double cos_theta = 1;

        public GeometryObject Geometry { get => geometry; set => geometry = value; }
        public double Sin_theta { get => sin_theta; set => sin_theta = value; }
        public double Cos_theta { get => cos_theta; set => cos_theta = value; }

        public Rotate_self_Y()
        {

        }
        public Rotate_self_Y(GeometryObject geometry, double angle)
        {
            Geometry = geometry;
            if (angle != 0)
            {
                double radians = (Math.PI * angle) / 180.0;
                Sin_theta = Math.Sin(radians);
                Cos_theta = Math.Cos(radians);
            }
        }

        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            ShadeRec SHA = new ShadeRec();
            if (Geometry.Hit(r, t_min, ref SHA))
            {
                //求得交点后再往正的方向移

                Vector3D tempN = new Vector3D();

                Vector3D normal = new Vector3D(SHA.Normal);

                tempN.X = Cos_theta * normal.X + Sin_theta * normal.Z;
                tempN.Z = -Sin_theta * normal.X + Cos_theta * normal.Z;
                tempN.Y = normal.Y;

                double theta = Math.Acos(tempN.Y);
                double phi = Math.Atan2(tempN.X, tempN.Z);
                if (phi < 0)
                {
                    phi += 2.0 * Math.PI;
                }
                double u = phi / (2.0 * Math.PI);
                double v = theta / Math.PI;
                SHA.U = u;
                SHA.V = v;
                SHA.IsHit = true;
            }
            else
            {
                SHA.IsHit = false;
            }
            if (SHA.IsHit)
            {
                rec = SHA;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            return geometry.Bounding_box(t0, t1, ref box);
        }
    }

    /// <summary>
    // /绕物体自身中心Z轴转动
    /// </summary>
    class Rotate_self_Z : GeometryObject
    {
        GeometryObject geometry = new GeometryObject();
        double sin_theta = 0;
        double cos_theta = 1;

        public GeometryObject Geometry { get => geometry; set => geometry = value; }
        public double Sin_theta { get => sin_theta; set => sin_theta = value; }
        public double Cos_theta { get => cos_theta; set => cos_theta = value; }

        public Rotate_self_Z()
        {

        }
        public Rotate_self_Z(GeometryObject geometry, double angle)
        {
            Geometry = geometry;
            if (angle != 0)
            {
                double radians = (Math.PI * angle) / 180.0;
                Sin_theta = Math.Sin(radians);
                Cos_theta = Math.Cos(radians);
            }
        }

        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            ShadeRec SHA = new ShadeRec();
            if (Geometry.Hit(r, t_min, ref SHA))
            {
                //求得交点后再往正的方向移

                Vector3D tempN = new Vector3D();

                Vector3D normal = new Vector3D(SHA.Normal);

                tempN.X = Cos_theta * normal.X + Sin_theta * normal.Y;
                tempN.Y = -Sin_theta * normal.X + Cos_theta * normal.Y;
                tempN.Z = normal.Z;

                double theta = Math.Acos(tempN.Y);
                double phi = Math.Atan2(tempN.X, tempN.Z);
                if (phi < 0)
                {
                    phi += 2.0 * Math.PI;
                }
                double u = phi / (2.0 * Math.PI);
                double v = theta / Math.PI;
                SHA.U = u;
                SHA.V = v;
                SHA.IsHit = true;
            }
            else
            {
                SHA.IsHit = false;
            }
            if (SHA.IsHit)
            {
                rec = SHA;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            return geometry.Bounding_box(t0, t1, ref box);
        }
    }

    /// <summary>
    /// 绕X轴旋转
    /// </summary>
    class Rotate_X : GeometryObject
    {
        GeometryObject geometry = new GeometryObject();
        double sin_theta = 0;
        double cos_theta = 1;

        public GeometryObject Geometry { get => geometry; set => geometry = value; }
        public double Sin_theta { get => sin_theta; set => sin_theta = value; }
        public double Cos_theta { get => cos_theta; set => cos_theta = value; }

        public Rotate_X()
        {

        }
        public Rotate_X(GeometryObject geometry, double angle)
        {
            Geometry = geometry;
            if (angle != 0)
            {
                double radians = (Math.PI * angle) / 180.0;
                Sin_theta = Math.Sin(radians);
                Cos_theta = Math.Cos(radians);
            }
        }
        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            Point3D origin = new Point3D();

            Vector3D direction = new Vector3D();
            Point3D temp = new Point3D(r.Origin);
            Vector3D tempD = new Vector3D(r.Direction);

            double Sin_theta1 = -Sin_theta;
            double Cos_theta1 = Cos_theta;

            //把光线往相反的方向移动，求交
            origin.Y = Cos_theta1 * temp.Y + Sin_theta1 * temp.Z;
            origin.Z = -Sin_theta1 * temp.Y + Cos_theta1 * temp.Z;
            origin.X = temp.X;
            direction.Y = Cos_theta1 * tempD.Y + Sin_theta1 * tempD.Z;
            direction.Z = -Sin_theta1 * tempD.Y + Cos_theta1 * tempD.Z;
            direction.X = tempD.X;

            Ray rotate_y = new Ray(origin, direction);
            ShadeRec SHA = new ShadeRec();
            if (Geometry.Hit(rotate_y, t_min, ref SHA))
            {
                //求得交点后再往正的方向移
                Vector3D tempP = new Vector3D();
                Vector3D tempN = new Vector3D();
                Vector3D p = new Vector3D(SHA.P);
                Vector3D normal = new Vector3D(SHA.Normal);
                tempP.Y = Cos_theta * p.Y + Sin_theta * p.Z;
                tempP.Z = -Sin_theta * p.Y + Cos_theta * p.Z;
                tempP.X = p.X;
                tempN.Y = Cos_theta * normal.Y + Sin_theta * normal.Z;
                tempN.Z = -Sin_theta * normal.Y + Cos_theta * normal.Z;
                tempN.X = normal.X;
                SHA.P = tempP;
                SHA.Normal = tempN;
                SHA.IsHit = true;
            }
            else
            {
                SHA.IsHit = false;
            }
            if (SHA.IsHit)
            {
                rec = SHA;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            return geometry.Bounding_box(t0, t1, ref box);
        }
    }

    /// <summary>
    /// 绕Y轴旋转
    /// </summary>
    class Rotate_Y : GeometryObject
    {
        GeometryObject geometry = new GeometryObject();
        double sin_theta = 0;
        double cos_theta = 1;

        public GeometryObject Geometry { get => geometry; set => geometry = value; }
        public double Sin_theta { get => sin_theta; set => sin_theta = value; }
        public double Cos_theta { get => cos_theta; set => cos_theta = value; }

        public Rotate_Y()
        {

        }
        public Rotate_Y(GeometryObject geometry, double angle)
        {
            Geometry = geometry;
            if (angle != 0)
            {
                double radians = (Math.PI * angle) / 180.0;
                Sin_theta = Math.Sin(radians);
                Cos_theta = Math.Cos(radians);
            }
        }
        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            Point3D origin = new Point3D();

            Vector3D direction = new Vector3D();
            Point3D temp = new Point3D(r.Origin);
            Vector3D tempD = new Vector3D(r.Direction);

            double Sin_theta1 = -Sin_theta;
            double Cos_theta1 = Cos_theta;

            //把光线往相反的方向移动，求交
            origin.X = Cos_theta1 * temp.X + Sin_theta1 * temp.Z;
            origin.Z = -Sin_theta1 * temp.X + Cos_theta1 * temp.Z;
            origin.Y = temp.Y;
            direction.X = Cos_theta1 * tempD.X + Sin_theta1 * tempD.Z;
            direction.Z = -Sin_theta1 * tempD.X + Cos_theta1 * tempD.Z;
            direction.Y = tempD.Y;

            Ray rotate_y = new Ray(origin, direction);
            ShadeRec SHA = new ShadeRec();
            if (Geometry.Hit(rotate_y, t_min, ref SHA))
            {
                //求得交点后再往正的方向移
                Vector3D tempP = new Vector3D();
                Vector3D tempN = new Vector3D();
                Vector3D p = new Vector3D(SHA.P);
                Vector3D normal = new Vector3D(SHA.Normal);
                tempP.X = Cos_theta * p.X + Sin_theta * p.Z;
                tempP.Z = -Sin_theta * p.X + Cos_theta * p.Z;
                tempP.Y = p.Y;
                tempN.X = Cos_theta * normal.X + Sin_theta * normal.Z;
                tempN.Z = -Sin_theta * normal.X + Cos_theta * normal.Z;
                tempN.Y = normal.Y;
                SHA.P = tempP;
                SHA.Normal = tempN;
                SHA.IsHit = true;
            }
            else
            {
                SHA.IsHit = false;
            }
            if (SHA.IsHit)
            {
                rec = SHA;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            return geometry.Bounding_box(t0, t1, ref box);
        }
    }

    /// <summary>
    /// 绕Z轴旋转
    /// </summary>
    class Rotate_Z : GeometryObject
    {
        GeometryObject geometry = new GeometryObject();
        double sin_theta = 0;
        double cos_theta = 1;

        public GeometryObject Geometry { get => geometry; set => geometry = value; }
        public double Sin_theta { get => sin_theta; set => sin_theta = value; }
        public double Cos_theta { get => cos_theta; set => cos_theta = value; }

        public Rotate_Z()
        {

        }
        public Rotate_Z(GeometryObject geometry, double angle)
        {
            Geometry = geometry;
            if (angle != 0)
            {
                double radians = (Math.PI * angle) / 180.0;
                Sin_theta = Math.Sin(radians);
                Cos_theta = Math.Cos(radians);
            }
        }
        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            Point3D origin = new Point3D();

            Vector3D direction = new Vector3D();
            Point3D temp = new Point3D(r.Origin);
            Vector3D tempD = new Vector3D(r.Direction);

            double Sin_theta1 = -Sin_theta;
            double Cos_theta1 = Cos_theta;

            //把光线往相反的方向移动，求交
            origin.X = Cos_theta1 * temp.X + Sin_theta1 * temp.Y;
            origin.Y = -Sin_theta1 * temp.X + Cos_theta1 * temp.Y;
            origin.Z = temp.Z;
            direction.X = Cos_theta1 * tempD.X + Sin_theta1 * tempD.Y;
            direction.Y = -Sin_theta1 * tempD.X + Cos_theta1 * tempD.Y;
            direction.Z = tempD.Z;

            Ray rotate_y = new Ray(origin, direction);
            ShadeRec SHA = new ShadeRec();
            if (Geometry.Hit(rotate_y, t_min, ref SHA))
            {
                //求得交点后再往正的方向移
                Vector3D tempP = new Vector3D();
                Vector3D tempN = new Vector3D();
                Vector3D p = new Vector3D(SHA.P);
                Vector3D normal = new Vector3D(SHA.Normal);
                tempP.X = Cos_theta * p.X + Sin_theta * p.Y;
                tempP.Y = -Sin_theta * p.X + Cos_theta * p.Y;
                tempP.Z = p.Z;
                tempN.X = Cos_theta * normal.X + Sin_theta * normal.Y;
                tempN.Y = -Sin_theta * normal.X + Cos_theta * normal.Y;
                tempN.Z = normal.Z;
                SHA.P = tempP;
                SHA.Normal = tempN;
                SHA.IsHit = true;
            }
            else
            {
                SHA.IsHit = false;
            }
            if (SHA.IsHit)
            {
                rec = SHA;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            return geometry.Bounding_box(t0, t1, ref box);
        }
    }

    /// <summary>
    /// 绕轴公转+自转
    /// </summary>
    class Rotate_self_and_other : GeometryObject
    {
        GeometryObject geometry = new GeometryObject();
        Vector3D self_normal_axis = new Vector3D();
        Vector3D other_normal_axis = new Vector3D();
        double self_angle=0;
        double self_sin_theta = 0;
        double self_cos_theta = 1;
        double other_angle = 0;
        double other_sin_theta = 0;
        double other_cos_theta = 1;

        public GeometryObject Geometry { get => geometry; set => geometry = value; }
        public double Self_sin_theta { get => self_sin_theta; set => self_sin_theta = value; }
        public double Self_cos_theta { get => self_cos_theta; set => self_cos_theta = value; }
        public double Other_sin_theta { get => other_sin_theta; set => other_sin_theta = value; }
        public double Other_cos_theta { get => other_cos_theta; set => other_cos_theta = value; }
        public double Self_angle { get => self_angle; set => self_angle = value; }
        public double Other_angle { get => other_angle; set => other_angle = value; }
        public Vector3D Other_normal_axis { get => other_normal_axis; set => other_normal_axis = value; }
        public Vector3D Self_normal_axis { get => self_normal_axis; set => self_normal_axis = value; }

        public Rotate_self_and_other()
        {

        }
        /// <summary>
        /// 自转+公转
        /// </summary>
        /// <param name="geometry">物体</param>
        /// <param name="other_normal_axis">公转轴</param>
        /// <param name="other_angle">公转角度</param>
        /// <param name="self_normal_axis">自转单位向量</param>
        /// <param name="self_angle">自转角度</param>
        public Rotate_self_and_other(GeometryObject geometry,Vector3D other_normal_axis, double other_angle,Vector3D self_normal_axis, double self_angle)
        {
            Geometry = geometry;
                Self_angle = self_angle;
                Self_normal_axis = self_normal_axis;
                double radians = (Math.PI * self_angle) / 180.0;
                Self_sin_theta = Math.Sin(radians);
                Self_cos_theta = Math.Cos(radians);
                Other_angle = other_angle;
                Other_normal_axis = other_normal_axis;
                double radians1 = (Math.PI * self_angle) / 180.0;
                Other_sin_theta = Math.Sin(radians1);
                Other_cos_theta = Math.Cos(radians1);
        }

        public override bool Hit(Ray r,double t_min, ref ShadeRec rec)
        {
            if (other_angle != 0)
            {
                //绕公转轴公转
                Point3D origin;
                Vector3D direction;

                Point3D temp = new Point3D(r.Origin);
                Vector3D tempD = new Vector3D(r.Direction);

                double Sin_theta_other = -Other_sin_theta;
                double Cos_theta_other = Other_cos_theta;

                //把光线往相反的方向移动，求交
                origin = RotateByVector(temp,Other_normal_axis,Cos_theta_other,Sin_theta_other);
                direction =new Vector3D( RotateByVector(new Point3D(tempD), Other_normal_axis, Cos_theta_other, Sin_theta_other));
                //新射线
                Ray rotate_y = new Ray(origin, direction);
                ShadeRec SHA = new ShadeRec();
                if (Geometry.Hit(rotate_y,t_min, ref SHA))
                {
                    //求得交点后再往正的方向移
                    Point3D tempP;
                    Vector3D tempN;
                    Point3D p = new Point3D(SHA.P);
                    Vector3D normal = new Vector3D(SHA.Normal);
                    tempP = RotateByVector(p, Other_normal_axis, Other_cos_theta, Other_sin_theta);
                    tempN = new Vector3D(RotateByVector(new Point3D(normal), Other_normal_axis, Other_cos_theta, Other_sin_theta));
                    SHA.P = new Vector3D(tempP);
                    SHA.Normal = tempN;
                    SHA.IsHit = true;
                    //处理自转，更新UV即可
                    if (self_angle != 0)
                    {
                        Vector3D tempN_self;
                        Vector3D normal_self = new Vector3D(SHA.Normal);

                        tempN_self = new Vector3D(RotateByVector(new Point3D(normal_self), Self_normal_axis, Self_cos_theta, Self_sin_theta));
                        double theta = Math.Acos(tempN_self.Y);
                        double phi = Math.Atan2(tempN_self.X, tempN_self.Z);
                        if (phi < 0)
                        {
                            phi += 2.0 * Math.PI;
                        }
                        double u = phi / (2.0 * Math.PI);
                        double v = theta / Math.PI;
                        SHA.U = u;
                        SHA.V = v;
                    }
                }
                else
                {
                    SHA.IsHit = false;
                }
                if (SHA.IsHit)
                {
                    rec = SHA;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (other_angle == 0)
            {
                //只处理自转（默认绕平行Y轴自转）
                ShadeRec SHA = new ShadeRec();
                if (Geometry.Hit(r,t_min, ref SHA))
                {
                    Vector3D tempN_self;
                    Vector3D normal_self = new Vector3D(SHA.Normal);

                    tempN_self = new Vector3D(RotateByVector(new Point3D(normal_self), Self_normal_axis, Self_cos_theta, Self_sin_theta));

                    double theta = Math.Acos(tempN_self.Y);
                    double phi = Math.Atan2(tempN_self.X, tempN_self.Z);
                    if (phi < 0)
                    {
                        phi += 2.0 * Math.PI;
                    }
                    double u = phi / (2.0 * Math.PI);
                    double v = theta / Math.PI;
                    SHA.U = u;
                    SHA.V = v;
                    SHA.IsHit = true;
                }
                else
                {
                    SHA.IsHit = false;
                }
                if (SHA.IsHit)
                {
                    rec = SHA;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            return geometry.Bounding_box(t0, t1, ref box);
        }

        /// <summary>
        /// 点绕单位轴旋转
        /// </summary>
        /// <param name="old_x">点原X</param>
        /// <param name="old_y">点原Y</param>
        /// <param name="old_z">点原Z</param>
        /// <param name="vx">单位轴X</param>
        /// <param name="vy">单位轴Y</param>
        /// <param name="vz">单位轴Z</param>
        /// <param name="c">cos(angle)</param>
        /// <param name="s">sin(angle)</param>
        /// <returns></returns>
        public static Point3D RotateByVector(Point3D old, Vector3D axis, double c, double s)
        {
            double old_x = old.X;
            double old_y = old.Y;
            double old_z = old.Z;
            double vx = axis.X;
            double vy = axis.Y;
            double vz = axis.Z;
            double new_x = (vx * vx * (1 - c) + c) * old_x + (vx * vy * (1 - c) - vz * s) * old_y + (vx * vz * (1 - c) + vy * s) * old_z;
            double new_y = (vy * vx * (1 - c) + vz * s) * old_x + (vy * vy * (1 - c) + c) * old_y + (vy * vz * (1 - c) - vx * s) * old_z;
            double new_z = (vx * vz * (1 - c) - vy * s) * old_x + (vy * vz * (1 - c) + vx * s) * old_y + (vz * vz * (1 - c) + c) * old_z;
            return new Point3D(new_x, new_y, new_z);
        }
    }
}