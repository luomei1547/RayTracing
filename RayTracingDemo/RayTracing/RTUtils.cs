using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RayTracing
{
    /// <summary>
    /// 渲染工具类
    /// </summary>
    public class RTUtils
    {
        //随机数
        public static Random rd = new Random();
        /// <summary>
        /// 单位球内随机向量
        /// </summary>
        /// <returns></returns>
        public static Vector3D RandomInUnitSphere()
        {
            Vector3D p;
            do
            {
                p = 2.0 * new Vector3D(rd.NextDouble(), rd.NextDouble(), rd.NextDouble()) - new Vector3D(1, 1, 1);
            } while (p.SquaredLength() >= 1.0);
            return p;
        }
        /// <summary>
        /// 单位圆内向量
        /// </summary>
        /// <returns></returns>
        public static Vector3D RandomInUnitDisk()
        {
            Vector3D p;
            do
            {
                p = 2.0 * new Vector3D(rd.NextDouble(), rd.NextDouble(), 0) - new Point3D(1, 1, 0);
            } while (Vector3D.DotProduct(p, p) >= 1.0);
            return p;
        }
        /// <summary>
        /// 利用层次包围盒计算颜色
        /// </summary>
        /// <param name="r"></param>
        /// <param name="world"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static SColor Color(Ray r, BVH_node world, int depth)
        {
            AABB aBB = new AABB();
            world.Bounding_box(0, 0, ref aBB);
            if (aBB.Hit(r, 1e-5))
            {
                ShadeRec rec = new ShadeRec();
                world.Hit(r, 1e-5, ref rec);
                if (rec.IsHit)
                {
                    Ray scattered = new Ray();
                    Vector3D attenuation = new Vector3D();
                    SColor emitted = rec.Material.Emitted(rec);//光颜色
                    if (depth < 1 && rec.Material.Scatter(r, rec, ref attenuation, ref scattered))
                    {
                        SColor colorTemp = Color(scattered, world, depth + 1);
                        colorTemp = colorTemp * new SColor(attenuation);
                        //有反射则将当前发光体发出的光和散射的颜色预以叠加
                        return colorTemp + emitted;
                    }
                    else
                    {
                        //无反射则直接取发光射出的颜色
                        return emitted;
                    }
                }
                else
                {
                     Vector3D unitDirection = Vector3D.UnitVector(r.Direction);
                     double t = 0.5 * (unitDirection.Y + 1.0);
                     return new SColor((1.0 - t) + t * 0.5, (1.0 - t) + t * 0.7, (1.0 - t) + t * 1);
                   // return new SColor(1, 1, 1);//无背景色
                }
            }
            else
            {
                Vector3D unitDirection = Vector3D.UnitVector(r.Direction);
                double t = 0.5 * (unitDirection.Y + 1.0);
                return new SColor((1.0 - t) + t * 0.5, (1.0 - t) + t * 0.7, (1.0 - t) + t * 1);
               // return new SColor(1, 0, 0);//无背景色
            }
        }
        /// <summary>
        /// 直接计算颜色
        /// </summary>
        /// <param name="r">射线</param>
        /// <param name="world">世界</param>
        /// <param name="depth">迭代次数</param>
        /// <returns></returns>
        public static SColor Color(Ray r, World world, int depth)
        {
            ShadeRec rec = world.Hit(r, 1e-5);
            if (rec.IsHit)
            {
                Ray scattered = new Ray();
                Vector3D attenuation = new Vector3D();
                SColor emitted = rec.Material.Emitted(rec);//光颜色
                if (depth < 50 && rec.Material.Scatter(r, rec, ref attenuation, ref scattered))
                {
                    SColor colorTemp = Color(scattered, world, depth + 1);
                    colorTemp = colorTemp * new SColor(attenuation);
                    //有反射则将当前发光体发出的光和散射的颜色预以叠加
                    return colorTemp + emitted;
                }
                else
                {
                    //无反射则直接取发光射出的颜色
                    return emitted;
                }
            }
            else
            {
                 Vector3D unitDirection = Vector3D.UnitVector(r.Direction);
                 double t = 0.5 * (unitDirection.Y + 1.0);
                 return new SColor((1.0 - t) + t * 0.5, (1.0 - t) + t * 0.7, (1.0 - t) + t * 1);
                 //return new SColor(0, 0, 0);//无背景色
            }
        }

        /// <summary>
        /// 镜面反射向量计算
        /// </summary>
        /// <param name="v">入射向量</param>
        /// <param name="n">归一化法线向量</param>
        /// <returns></returns>
        public static Vector3D Reflect(Vector3D v, Vector3D n)
        {
            return v - 2 * Vector3D.DotProduct(v, n) * n;
        }
        /// <summary>
        /// 计算折射向量
        /// 为了保证有解需要discriminant大于0，而cosθ1可以表示为-n*v，即法向量和入射向量（均为单位向量）的点积取负，
        /// 这样一来就会得到refracted中的折射向量（注意也是单位向量）
        /// </summary>
        /// <param name="v">入射向量</param>
        /// <param name="n">归一化法线向量</param>
        /// <param name="niOverNt">折射率比</param>
        /// <param name="refracted">折射向量</param>
        /// <returns></returns>
        public static bool Refract(Vector3D v, Vector3D n, double niOverNt, ref Vector3D refracted)
        {
            Vector3D uv = Vector3D.UnitVector(v);
            double dt = Vector3D.DotProduct(uv, n);
            double discriminant = 1.0 - niOverNt * niOverNt * (1 - dt * dt);
            if (discriminant > 0)
            {
                refracted = niOverNt * (uv - n * dt) - n * Math.Sqrt(discriminant);
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 使用Fresnel-Schlick近似法求反射比的近似解
        /// </summary>
        /// <param name="cosine"></param>
        /// <param name="refIdx"></param>
        /// <returns></returns>
        public static double Schlick(double cosine, double refIdx)
        {
            double r0 = (1 - refIdx) / (1 + refIdx);
            r0 = r0 * r0;
            return r0 + (1 - r0) * Math.Pow((1 - cosine), 5);
        }
    }
    /// <summary>
    /// Obj解析
    /// </summary>
    public class Objanalize
    {
        /// <summary>
        /// 读取文本文件
        /// </summary>
        public void Read(string filepath, Material material,ref List<GeometryObject> geometryObjects)
        {
            string path = filepath;
            if (path.Equals(null) || path.Equals(""))
            {
                return;
            }
           try
           {
                StreamReader sr = new StreamReader(path, Encoding.Default);  //path为文件路径

                String line;
                List<Point3D> v = new List<Point3D>();
                List<Point3D> vt = new List<Point3D>();
                List<Vector3D> vn = new List<Vector3D>();
                List<List<List<int>>> findex = new List<List<List<int>>>();
                List<MPplane> f = new List<MPplane>();
                List<double> jiange = new List<double>();
                while ((line = sr.ReadLine()) != null)//按行读取 line为每行的数据
                {
                    if (line.Contains("v "))
                    {
                        string[] vtemp = line.Split(' ');
                        Point3D vtempP = new Point3D(Convert.ToDouble(vtemp[1]) , Convert.ToDouble(vtemp[2]), Convert.ToDouble(vtemp[3]) );
                        v.Add(vtempP);
                    }
                    if (line.Contains("vt "))
                    {
                        string[] vtemp = line.Split(' ');
                        Point3D vtempP = new Point3D(Convert.ToDouble(vtemp[1]), Convert.ToDouble(vtemp[2]), 0);
                        vt.Add(vtempP);
                    }
                    if (line.Contains("vn "))
                    {
                        string[] vtemp = line.Split(' ');
                        Vector3D vtempP = new Vector3D(Convert.ToDouble(vtemp[1]), Convert.ToDouble(vtemp[2]), Convert.ToDouble(vtemp[3]));
                        vn.Add(vtempP);
                    }
                    if (line.Contains("f "))
                    {
                        string[] vtemp = line.Split(' ');//值代表一个点
                        List<List<int>> listFD = new List<List<int>>();//把点以及点的三个信息分割开来
                        for (int i = 1; i < vtemp.Length; i++)
                        {
                            string[] temp = vtemp[i].Split('/');//分割某一个点的三个信息
                            List<int> templ = new List<int>();//记录一个点的三个信息
                            for (int k = 0; k < temp.Length; k++)
                            {
                                templ.Add(Convert.ToInt32(temp[0])-1);
                            }
                            listFD.Add(templ);//把每一个点信息放到这个面里面。
                        }
                        findex.Add(listFD);//把面用list存放起来
                    }
                }
                for (int i = 0; i < findex.Count;i++)//每一个面
                {
                    List<OBJPoint> planePoints = new List<OBJPoint>();
                    for (int j = 0; j < findex[i].Count; j++)//每一个面的每一个点
                    {
                        //每一个点的每一个信息（顶点三维坐标、顶点UV、顶点法线索引值）
                        Point3D p = new Point3D();
                        Point3D uv = new Point3D();
                        Vector3D normal = new Vector3D();
                        p = v[findex[i][j][0]];
                        uv = vt[findex[i][j][1]];
                        normal= vn[findex[i][j][2]];
                        planePoints.Add(new OBJPoint(p,uv.X,uv.Y,normal));
                    }
                    geometryObjects.Add(new MPplane(planePoints, material));
                }
                }
         catch (Exception ex)
         {
             MessageBox.Show(ex.Message);
         }
       }
    }
    /// <summary>
    /// obj点信息
    /// </summary>
    public class OBJPoint
    {
        private Point3D p = new Point3D();//三维坐标
        private double u = 0;
        private double v = 0;
        private Vector3D n = new Vector3D();//点上法线

        public Point3D P { get => p; set => p = value; }
        public double U { get => u; set => u = value; }
        public double V { get => v; set => v = value; }
        public Vector3D N { get => n; set => n = value; }

        public OBJPoint(Point3D p, double u, double t, Vector3D n)
        {
            P = p;
            U = u;
            V = t;
            N = n;
        }
    }
}
