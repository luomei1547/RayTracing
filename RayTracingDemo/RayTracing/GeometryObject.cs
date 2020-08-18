using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// 物体
    /// </summary>
    public  class GeometryObject
    {
        /// <summary>
        /// 传入射线是否击中物体
        /// </summary>
        /// <param name="r">射线</param>
        /// <param name="t_min">t可以取的最小值</param>
        /// <param name="rec">击中点信息</param>
        /// <returns>是否击中</returns>
        public virtual bool Hit(Ray r,double t_min, ref ShadeRec rec)
        {
            return false;
        }
        /// <summary>
        /// 物体的包围盒
        /// </summary>
        /// <param name="t0">物体运动开始时间</param>
        /// <param name="t1">物体运动结束时间</param>
        /// <param name="box">物体的包围盒</param>
        /// <returns></returns>
        public virtual bool Bounding_box(double t0, double t1, ref AABB box)
        {
            return false;
        }
    }

    /// <summary>
    /// 球体
    /// </summary>
    public class Sphere : GeometryObject
    {
        private double radius;//半径
        private Point3D center;//球心
        private Material material;//材质
        double time0 = 0, time1 = 0;//运动时间范围
        Point3D center0 = new Point3D();//运动开始球心
        Point3D center1 = new Point3D();//运动结束球心

        public double Radius { get => radius; set => radius = value; }
        internal Point3D Center { get => center; set => center = value; }
        internal Material Material { get => material; set => material = value; }
        public double Time0 { get => time0; set => time0 = value; }
        public double Time1 { get => time1; set => time1 = value; }
        internal Point3D Center0 { get => center0; set => center0 = value; }
        internal Point3D Center1 { get => center1; set => center1 = value; }

        public Sphere() { }

        public Sphere(Point3D center, double radius, Material material)
        {
            this.radius = radius;
            this.center = center;
            this.material = material;
        }

        public Sphere(Point3D center, double radius, Material material, Point3D center0, Point3D center1, double t1, double t2)
        {
            time0 = t1;
            Time1 = t2;
            Center0 = center0;
            Center1 = center1;
            this.radius = radius;
            this.center = center;
            this.material = material;
        }
        /// <summary>
        /// 获取当前时间球体运动过程的实际球心位置
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public Point3D Moving_sphere(double time)
        {
            if ((Time1 - Time0) != 0 && time != 0)
            {
                Point3D newcenter = new Point3D(center + ((time - Time0) / (Time1 - Time0)) * (Center1 - Center0));
                return newcenter;
            }
            else
            {
                return Center;
            }
        }
        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            Point3D tem2p = Moving_sphere(r.LastTime);
            Vector3D oc = r.Origin - tem2p;
            double a = Vector3D.DotProduct(r.Direction, r.Direction);
            double b = Vector3D.DotProduct(oc, r.Direction);
            double c = Vector3D.DotProduct(oc, oc) - radius * radius;
            double discriminant = b * b - a * c;
            if (discriminant > 0)
            {
                double temp = (-b - Math.Sqrt(b * b - a * c)) / a;
                if (temp < 0.00001)//去除double精度影响
                {
                    temp = (-b + Math.Sqrt(b * b - a * c)) / a;
                }
                if (temp > t_min)
                {
                    rec.T = temp;
                    rec.P = r.PointAtPara(rec.T);
                    rec.Normal = (rec.P - center) / radius;
                    rec.Material = material;
                    rec.IsHit = true;
                    double theta = Math.Acos(rec.Normal.Y);
                    double phi = Math.Atan2(rec.Normal.X, rec.Normal.Z);
                    if (phi < 0)
                    {
                        phi += 2.0 * Math.PI;
                    }
                    double u = phi / (2.0 * Math.PI);
                    double v = theta / Math.PI;
                    rec.U = u;
                    rec.V = v;
                    return true;
                }
                else
                {
                    rec.IsHit = false;
                }
            }
            return false;
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            if ((Time1 - Time0) != 0)
            {
                //运动球体的盒模型
                AABB box0 = new AABB(Center0 - new Vector3D(Radius, Radius, Radius), Center0 + new Vector3D(Radius, Radius, Radius));
                AABB box1 = new AABB(Center1 - new Vector3D(Radius, Radius, Radius), Center1 + new Vector3D(Radius, Radius, Radius));
                box = Surrounding_box(box0, box1);
            }
            else
            {
                //静止球体的盒模型
                box = new AABB(Center - new Vector3D(Radius, Radius, Radius), Center + new Vector3D(Radius, Radius, Radius));
            }
            return true;
        }
        /// <summary>
        /// 两个盒子的包围盒
        /// </summary>
        /// <param name="box1">盒子1</param>
        /// <param name="box2">盒子2</param>
        /// <returns>包围盒</returns>
        public AABB Surrounding_box(AABB box1, AABB box2)
        {
            Vector3D small = new Vector3D(Math.Min(box1.Min.X, box2.Min.X), Math.Min(box1.Min.Y, box2.Min.Y), Math.Min(box1.Min.Z, box2.Min.Z));
            Vector3D big = new Vector3D(Math.Max(box1.Max.X, box2.Max.X), Math.Max(box1.Max.Y, box2.Max.Y), Math.Max(box1.Max.Z, box2.Max.Z));
            return new AABB(small, big);
        }
    }

    /// <summary>
    /// XY矩形平面
    /// </summary>
    public class XY_rect_Plane : GeometryObject
    {
        double _x0 = 0;
        double _x1 = 0;
        double _y0 = 0;
        double _y1 = 0;
        double _k = 0;
        Material material;
        public XY_rect_Plane()
        {

        }
        public XY_rect_Plane(double _x0, double _x1, double _y0, double _y1, double k, Material material)
        {
            this.X0 = _x0;
            this.X1 = _x1;
            this.Y0 = _y0;
            this.Y1 = _y1;
            this.K = k;
            this.Material = material;
        }

        public double X0 { get => _x0; set => _x0 = value; }
        public double X1 { get => _x1; set => _x1 = value; }
        public double Y0 { get => _y0; set => _y0 = value; }
        public double Y1 { get => _y1; set => _y1 = value; }
        public Material Material { get => material; set => material = value; }
        public double K { get => _k; set => _k = value; }

        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            double t;
            if (r.Direction.Z == 0)
            {
                t = K;
            }
            else
            {
                t = (K - r.Origin.Z) / r.Direction.Z;
            }
            if (t < t_min)
            {
                return false;
            }
            double x = r.Origin.X + t * r.Direction.X;
            double y = r.Origin.Y + t * r.Direction.Y;
            if (x < X0 || x > X1 || y < Y0 || y > Y1)
            {
                return false;
            }
            if (((X1 - X0) != 0) && ((Y1 - Y0) != 0))
            {
                rec.U = (x - X0) / (X1 - X0);
                rec.V = (y - Y0) / (Y1 - Y0);
            }
            rec.T = t;
            rec.Material = material;
            rec.P = r.PointAtPara(t);
            rec.Normal = new Vector3D(0, 0, 1);
            rec.IsHit = true;
            return true;
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            //k+0.0001  k-0.0001 表示光源的厚度
            box = new AABB(new Vector3D(X0, Y0, K - 0.0001), new Vector3D(X1, Y1, K + 0.0001));
            return true;
        }
    }

    /// <summary>
    /// XZ矩形平面
    /// </summary>
    public class XZ_rect_Plane : GeometryObject
    {
        double _x0 = 0;
        double _x1 = 0;
        double _Z0 = 0;
        double _Z1 = 0;
        double _k = 0;
        Material material;
        public XZ_rect_Plane()
        {

        }
        public XZ_rect_Plane(double _x0, double _x1, double _Z0, double _Z1, double k, Material material)
        {
            this.X0 = _x0;
            this.X1 = _x1;
            this.Z0 = _Z0;
            this.Z1 = _Z1;
            this.K = k;
            this.Material = material;
        }

        public double X0 { get => _x0; set => _x0 = value; }
        public double X1 { get => _x1; set => _x1 = value; }
        public double Z0 { get => _Z0; set => _Z0 = value; }
        public double Z1 { get => _Z1; set => _Z1 = value; }
        public Material Material { get => material; set => material = value; }
        public double K { get => _k; set => _k = value; }

        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            double t;
            if (r.Direction.Y == 0)
            {
                t = K;
            }
            else
            {
                t = (K - r.Origin.Y) / r.Direction.Y;
            }
            if (t < t_min)
            {
                return false;
            }
            double x = r.Origin.X + t * r.Direction.X;
            double y = r.Origin.Z + t * r.Direction.Z;
            if (x < X0 || x > X1 || y < Z0 || y > Z1)
            {
                return false;
            }
            if (((X1 - X0) != 0) && ((Z1 - Z0) != 0))
            {
                rec.U = (x - X0) / (X1 - X0);
                rec.V = (y - Z0) / (Z1 - Z0);
            }
            rec.T = t;
            rec.Material = material;
            rec.P = r.PointAtPara(t);
            rec.Normal = new Vector3D(0, 1, 0);
            rec.IsHit = true;
            return true;
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            //k+0.0001  k-0.0001 表示光源的厚度
            box = new AABB(new Vector3D(X0, Z0, K - 0.0001), new Vector3D(X1, Z1, K + 0.0001));
            return true;
        }
    }

    /// <summary>
    /// YZ矩形平面
    /// </summary>
    public class YZ_rect_Plane : GeometryObject
    {
        double _z0 = 0;
        double _z1 = 0;
        double _y0 = 0;
        double _y1 = 0;
        double _k = 0;
        Material material;
        public YZ_rect_Plane()
        {

        }
        public YZ_rect_Plane(double _y0, double _y1, double _Z0, double _Z1, double k, Material material)
        {
            this.Z0 = _Z0;
            this.Z1 = _Z1;
            this.Y0 = _y0;
            this.Y1 = _y1;
            this.K = k;
            this.Material = material;
        }

        public double Z0 { get => _z0; set => _z0 = value; }
        public double Z1 { get => _z1; set => _z1 = value; }
        public double Y0 { get => _y0; set => _y0 = value; }
        public double Y1 { get => _y1; set => _y1 = value; }
        public Material Material { get => material; set => material = value; }
        public double K { get => _k; set => _k = value; }

        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {

            double t;
            if (r.Direction.X == 0)
            {
                t = K;
            }
            else
            {
                t = (K - r.Origin.X) / r.Direction.X;
            }
            if (t < t_min)
            {
                return false;
            }
            double x = r.Origin.Z + t * r.Direction.Z;
            double y = r.Origin.Y + t * r.Direction.Y;

            if (x < Z0 || x > Z1 || y < Y0 || y > Y1)
            {
                return false;
            }

            if (((Z1 - Z0) != 0) && ((Y1 - Y0) != 0))
            {
                rec.U = (x - Z0) / (Z1 - Z0);
                rec.V = (y - Y0) / (Y1 - Y0);
            }
            rec.T = t;
            rec.Material = material;
            rec.P = r.PointAtPara(t);
            rec.Normal = new Vector3D(1, 0, 0);
            rec.IsHit = true;
            return true;
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            //k+0.0001  k-0.0001 表示光源的厚度
            box = new AABB(new Vector3D(Z0, Y0, K - 0.0001), new Vector3D(Z1, Y1, K + 0.0001));
            return true;
        }
    }

    /// <summary>
    /// 盒子
    /// </summary>
    public class Box : GeometryObject
    {
        World world = new World();
        Vector3D pmin = new Vector3D();
        Vector3D pmax = new Vector3D();

        public Vector3D Pmin { get => pmin; set => pmin = value; }
        public Vector3D Pmax { get => pmax; set => pmax = value; }
        public World World { get => world; set => world = value; }

        public Box()
        {

        }
        public Box(Vector3D p0, Vector3D p1, Material material)
        {
            Pmin = p0;
            Pmax = p1;
            world.AddObj(new XY_rect_Plane(p0.X, p1.X, p0.Y, p1.Y, p1.Z, material));
            world.AddObj(new Flip_normals(new XY_rect_Plane(p0.X, p1.X, p0.Y, p1.Y, p0.Z, material)));
            world.AddObj(new XZ_rect_Plane(p0.X, p1.X, p0.Z, p1.Z, p1.Y, material));
            world.AddObj(new Flip_normals(new XZ_rect_Plane(p0.X, p1.X, p0.Z, p1.Z, p0.Y, material)));
            world.AddObj(new YZ_rect_Plane(p0.Y, p1.Y, p0.Z, p1.Z, p1.X, material));
            world.AddObj(new Flip_normals(new YZ_rect_Plane(p0.Y, p1.Y, p0.Z, p1.Z, p0.X, material)));
        }
        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            rec = world.Hit(r, t_min);
            return rec.IsHit;
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            box = new AABB(Pmin, Pmax);
            return true;
        }
    }

    /// <summary>
    /// 体积雾
    /// </summary>
    public class Constant_medium : GeometryObject
    {
        GeometryObject _geometryObject = new GeometryObject();
        double density = 0;
        Material _materialg = new Material();

        public GeometryObject GeometryObject { get => _geometryObject; set => _geometryObject = value; }
        public double Density { get => density; set => density = value; }
        public Material Materialg { get => _materialg; set => _materialg = value; }

        public Constant_medium()
        {

        }
        public Constant_medium(GeometryObject geometry, double density, Material material)
        {
            GeometryObject = geometry;
            Density = density;
            Materialg = material;
        }
        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            ShadeRec rec1 = new ShadeRec();
            ShadeRec rec2 = new ShadeRec();
            if (GeometryObject.Hit(r, t_min, ref rec1)) //直线与盒子的近交点        
            {
                if (GeometryObject.Hit(r, rec1.T + 0.0001, ref rec2)) //直线与盒子远点的交点            
                {
                    if (rec1.T < t_min)
                        rec1.T = t_min;
                    if (rec2.T > Double.MaxValue)
                        rec2.T = Double.MaxValue;
                    if (rec1.T >= rec2.T)
                        return false;
                    //以上三个if确保交于体且位于体内                
                    if (rec1.T < 0)
                        rec1.T = 0;
                    //光线与体的交点的起点与终点之间的长度      
                    r.Direction.MakeUnitVector();
                    double distance_inside_boundary = (rec2.T - rec1.T) * r.Direction.Length();
                    //hit_distance是光线在体中传输的距离，可以看到与密度成反比，密度越大，传输距离越短，使用一个随机数的log     
                    //表达随机的传输距离，但是整体与密度呈反比               
                    double hit_distance = -(Density) * Math.Log(RTUtils.rd.NextDouble());
                    if (hit_distance < distance_inside_boundary)
                    {
                        rec.T = rec1.T + hit_distance / r.Direction.Length();
                        rec.P = r.PointAtPara(rec.T);
                        rec.Normal = Vector3D.UnitVector(new Vector3D(RTUtils.rd.NextDouble(), RTUtils.rd.NextDouble(), RTUtils.rd.NextDouble())); //随机的法线方向 
                        rec.Material = Materialg;
                        rec.IsHit = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            return GeometryObject.Bounding_box(t0, t1, ref box);
        }

    }
    /// <summary>
    /// 任意多边形
    /// </summary>
    class MPplane : GeometryObject
    {
        List<OBJPoint> planePoints = new List<OBJPoint>();//点信息
        Material material = new Material();//材质
        List<Point3D> points = new List<Point3D>();//点的坐标
        List<Vector3D> normals = new List<Vector3D>();//点的向量
        List<Vector3D> uvs = new List<Vector3D>();//点的UV
        public List<OBJPoint> PlanePoints { get => planePoints; set => planePoints = value; }
        public Material Material { get => material; set => material = value; }
        public MPplane()
        {

        }
        public MPplane(List<OBJPoint> objpoints, Material material)
        {
            if (objpoints.Count < 3)
                throw new Exception("多边形面片至少需要3个点");
            PlanePoints = objpoints;
            Material = material;
            for (int i = 0; i < objpoints.Count; i++)
            {
                points.Add(objpoints[i].P);
                normals.Add(objpoints[i].N);
                uvs.Add(new Vector3D(objpoints[i].U, objpoints[i].V, 0));
            }
        }

      
        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            var normal = (Vector3D.CrossProduct((points[0] - points[1]), (points[1] - points[2])));
            normal.MakeUnitVector();
            r.Direction.MakeUnitVector();
            //面片背面剔除
            var cos = Vector3D.DotProduct(r.Direction, normal);
            if (cos == 0)
            {
                return false;
            }
            //计算t（距离）
            rec.T = Vector3D.DotProduct(points[0] - r.Origin, normal) / Vector3D.DotProduct(r.Direction, normal);
            
            if (Double.IsNaN(rec.T) || rec.T < t_min || rec.T > Double.MaxValue)
                return false;

            //计算p（射线与面交点坐标）
            var p = r.Origin + rec.T * r.Direction;

            //光栅剔除
            points.Add(points[0]);
            Vector3D firstTowards = new Vector3D(0, 0, 0);

            for (int i = 0; i < points.Count - 1; i++)
            {
                var oa = (points[i] - points[i + 1]);
                oa.MakeUnitVector();
                var op = (points[i] - p);
                op.MakeUnitVector();
                var towards = (Vector3D.CrossProduct(oa, op));
                towards.MakeUnitVector();
                if (firstTowards.X==0&&firstTowards.Y==0&&firstTowards.Z==0)
                    firstTowards = towards;
                else if (Vector3D.DotProduct(firstTowards, towards) < 0.99999)
                {
                    points.RemoveAt(points.Count - 1);
                    return false;
                }
            }
            points.RemoveAt(points.Count - 1);

            rec.P = p;

            //是否有normal记录
               if (normals.Count == points.Count)
               {
                   if (uvs.Count == 3)
                        rec.Normal = GetAvgNormalInTriangle(p);
                    else
                        rec.Normal = GetAvgNormal(p);
                }
                else
                  rec.Normal = normal;
             

            Vector3D UV = new Vector3D(0,0,0);
            //是否有uv记录
             if (uvs.Count == points.Count)
             {
                 if (uvs.Count == 3)
                     UV = GetAvgUVInTriangle(p);
                else
                    UV = GetAvgUV(p);
            }

            rec.U = UV.X;
            rec.V = UV.Y;
            rec.IsHit = true;
            rec.Material = material;

            return true;
        }

        Vector3D GetAvgNormal(Vector3D point)
        {
            List<double> distances = new List<double>();
            double allDis = 0;
            for (int i = 0; i < points.Count; i++)
            {
                var pt = points[i];//这个面得每一个点
                var dis = (point- pt).Length();//与击中点之间的距离
                allDis += dis;//距离和
                distances.Add(dis);
                if (dis == 0)//击中点就是顶点，直接返回顶点信息
                    return normals[i];
            }

            Vector3D n = new Vector3D(0, 0, 0);
            int index = 0;
            foreach (var item in normals)
            {
                n += item * (allDis - distances[index++]) / allDis / (points.Count - 1);//每个顶点法线的贡献
            }
            return n;

        }
        Vector3D GetAvgUV(Vector3D point)
        {
            List<double> distances = new List<double>();
            double allDis = 0;
            for (int i = 0; i < points.Count; i++)
            {
                var pt = points[i];
                var dis = (point-pt).Length();
                allDis += dis;
                distances.Add(dis);
                if (dis == 0)
                    return uvs[i];
            }

            Vector3D uv = new Vector3D(0,0,0);
            int index = 0;
            foreach (var item in uvs)
            {
                uv += item * (allDis - distances[index++]) / allDis / (points.Count - 1);
            }
            return uv;

        }

        Vector3D GetAvgNormalInTriangle(Vector3D point)
        {
            List<double> distances = new List<double>();
            double allDis = 0;

            points.Add(points[0]);
            points.Add(points[1]);
            points.Remove(points[0]);//1 2 0 1
            for (int i = 0; i < points.Count - 1; i++)
            {
                var dis = GetArea(
                     (points[i] - points[i + 1]).Length(),
                    (point - points[i + 1]).Length(),
                    (points[i] - point).Length()
                    );
                allDis += dis;
                distances.Add(dis);
            }
            points.Add(points[1]);
            points.Remove(points[0]);
            points.Remove(points[0]);

            Vector3D n = new Vector3D(0, 0, 0);
            int index = 0;
            foreach (var normal in normals)
            {
                n += normal * distances[index++] / allDis;
            }
            return n;

        }



        Vector3D GetAvgUVInTriangle(Vector3D point)
        {
            List<double> distances = new List<double>();
            double allDis = 0;

            points.Add(points[0]);
            points.Add(points[1]);
            points.Remove(points[0]);
            for (int i = 0; i < points.Count - 1; i++)
            {
                var dis = GetArea(
                    (points[i]-points[i + 1]).Length(),
                    (point-points[i + 1]).Length(),
                    (points[i]-point).Length()
                    );
                allDis += dis;
                distances.Add(dis);
            }
            points.Add(points[1]);
            points.Remove(points[0]);
            points.Remove(points[0]);

            Vector3D uv = new Vector3D(0,0,0);
            int index = 0;
            foreach (var item in uvs)
            {
                uv += item * distances[index++] / allDis;
            }
            return uv;

        }

         double GetArea(double a, double b, double c)
        {
            //一条直线
            if (a + b - c < 1e-5 || b + c - a < 1e-5 || c + a - b < 1e-5)
                return 0;
            var p = (a + b + c) / 2;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            box = null;
            return false;
        }
        public AABB Surrounding_box(AABB box1, AABB box2)
        {
            Vector3D small = new Vector3D(Math.Min(box1.Min.X, box2.Min.X), Math.Min(box1.Min.Y, box2.Min.Y), Math.Min(box1.Min.Z, box2.Min.Z));
            Vector3D big = new Vector3D(Math.Max(box1.Max.X, box2.Max.X), Math.Max(box1.Max.Y, box2.Max.Y), Math.Max(box1.Max.Z, box2.Max.Z));
            return new AABB(small, big);
        }
    }
    /// <summary>
    /// OBJ模型
    /// </summary>
    public class Obj:GeometryObject
    {
        World world = new World();
        public World World { get => world; set => world = value; }

        public Obj()
        {

        }
        public Obj(string objfilename,Material material)
        {
            Objanalize textReader = new Objanalize();
            List<GeometryObject> v = new List<GeometryObject>();
            textReader.Read(objfilename, material, ref v);//解析obj文件数据，并返回所有物体
            World.GeometryObjects = v;
        }
        public override bool Hit(Ray r, double t_min, ref ShadeRec rec)
        {
            rec = world.Hit(r, t_min);
            return rec.IsHit;
        }

        public override bool Bounding_box(double t0, double t1, ref AABB box)
        {
            world.Bounding_box(t0,t1,ref box);
            return true;
        }
    }
}
