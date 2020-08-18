using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    public class World 
    {
        private List<GeometryObject> _geometryObjects = new List<GeometryObject>();//存放物体的list
        internal List<GeometryObject> GeometryObjects { get => _geometryObjects; set => _geometryObjects = value; }
        public World() { }
        /// <summary>
        /// 返回是否存在最近的击中点
        /// </summary>
        /// <param name="r"></param>
        /// <param name="tMin"></param>
        /// <param name="tMax"></param>
        /// <param name="rec"></param>
        /// <returns></returns>
        public ShadeRec Hit(Ray r,double t_min)
        {
            ShadeRec newrest = new ShadeRec();
            newrest.T = Double.MaxValue;
            for (int i = 0; i < GeometryObjects.Count; i++)
            {
                ShadeRec tempRec = new ShadeRec();
                GeometryObjects[i].Hit(r, t_min, ref tempRec);
                if (tempRec.IsHit&&tempRec.T<newrest.T)
                {
                        newrest = tempRec;
                }
            }
            return newrest;
        }
        /// <summary>
        /// 世界的所有物体的最大包围盒的计算
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool Bounding_box(double t0, double t1, ref AABB box)
        {
            if (GeometryObjects.Count < 1) return false;
            AABB temp_box = new AABB(); ;
            bool first_true = GeometryObjects[0].Bounding_box(t0, t1, ref temp_box);
            if (!first_true)
                return false;
            else
                box = temp_box;
            for (int i = 0; i < GeometryObjects.Count; i++)
            {
                if (GeometryObjects[i].Bounding_box(t0, t1, ref temp_box))
                {
                    box = Surrounding_box(box, temp_box);
                }
                else
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 计算两个盒子的包围盒
        /// </summary>
        /// <param name="box1"></param>
        /// <param name="box2"></param>
        /// <returns></returns>
        public AABB Surrounding_box(AABB box1, AABB box2)
        {
            Vector3D small = new Vector3D(Math.Min(box1.Min.X, box2.Min.X), Math.Min(box1.Min.Y, box2.Min.Y), Math.Min(box1.Min.Z, box2.Min.Z));
            Vector3D big = new Vector3D(Math.Max(box1.Max.X, box2.Max.X), Math.Max(box1.Max.Y, box2.Max.Y), Math.Max(box1.Max.Z, box2.Max.Z));
            return new AABB(small, big);
        }
        /// <summary>
        /// 添加物体
        /// </summary>
        /// <param name="geometry">物体</param>
        public void AddObj(GeometryObject geometry)
        {
            GeometryObjects.Add(geometry);
        }
        /// <summary>
        /// 随机添加物体2
        /// </summary>
        /// <returns></returns>
        public static World RandomScene2()
        {
            int nb = 20;
            List<GeometryObject> list = new List<GeometryObject>();
            List<GeometryObject> boxlist = new List<GeometryObject>();

            Material white = new Lambertian(new Vector3D(1,1,1),new Constant_texture(new SColor(0.73, 0.73, 0.73)));
            Material ground = new Lambertian(new Vector3D(1,1,1),new Constant_texture(new SColor(0.48, 0.83, 0.53)));

            for (int i = 0; i < nb; ++i) {
                for (int j = 0; j < nb; ++j)
                {
                    double w = 100;
                    double x0 = -1000 + i * w;
                    double z0 = -1000 + j * w;
                    double y0 = 0;
                    double x1 = x0 + w;
                    double y1 = 100 * (RTUtils.rd.NextDouble() + 0.01);
                    double z1 = z0 + w;
                    list.Add(new Box(new Vector3D(x0, y0, z0), new Vector3D(x1, y1, z1), ground));
                }
            }
            Material light = new AreaLight(new Constant_texture(new SColor(10, 10, 10)));
            list.Add(new XZ_rect_Plane(123, 423, 147, 412, 550, light));
            Vector3D center=new Vector3D(400, 400, 200);

            list.Add( new Sphere(new Point3D(center),50, new Lambertian(new Vector3D(1,1,1), new Constant_texture(new SColor(0.7, 0.3, 0.1))),new Point3D(center),new Point3D(center + new Vector3D(30, 0, 0)), 0, 1));
            list.Add( new Sphere(new Point3D(260, 150, 45), 50, new Dielectrics(1.5,new Vector3D(1,1,1))));
            list.Add(new Sphere(new Point3D(0, 150, 145), 50, new Metal(new Vector3D(1,1,1),10.0, new Constant_texture(new SColor(0.8, 0.8, 0.9)))));

            Sphere boundary = new Sphere(new Point3D(360, 150, 145), 70, new Dielectrics(1.5, new Vector3D(1, 1, 1)));
            list.Add(boundary);
            list.Add(new Constant_medium(boundary, 0.2, new Isotropic(new Constant_texture(new SColor(0.2, 0.4, 0.9)))));
            boundary = new Sphere(new Point3D(0,0,0), 5000, new Dielectrics(1.5,new Vector3D(1,1,1)));
            list.Add(new Constant_medium(boundary, 0.0011, new Isotropic(new Constant_texture(new SColor(1.0 ,1.0, 1.0)))));

            BMP_texture texture1 = new BMP_texture();
            texture1.BMP = (Bitmap)Bitmap.FromFile("test1.jpg", false);
            Lambertian lambertian1 = new Lambertian(new Vector3D(1, 0, 0), texture1);
            Sphere sphere1 = new Sphere(new Point3D(400, 200, 400), 100, lambertian1);
            list.Add(sphere1);
            Noise_texture noise_Texture = new Noise_texture(0.1);
            Sphere sphere2 = new Sphere(new Point3D(220, 280,300), 80, new Lambertian(new Vector3D(1, 0, 0), noise_Texture));
            list.Add(sphere2);

            int ns = 1000;
            for (int j = 0; j < ns; ++j)
            {
                Sphere temp = new Sphere(new Point3D(165 * RTUtils.rd.NextDouble(), 165 * RTUtils.rd.NextDouble(), RTUtils.rd.NextDouble()), 10, white);
                list.Add(temp);
                boxlist.Add(temp);
            }

            for (int j = 0; j < boxlist.Count; j++)
            {
                Translate temp = new Translate(new Rotate_Y(boxlist[j],15),new Vector3D(-100, 270, 395));
                list.Add(temp);
            }

            World world = new World();
            world.GeometryObjects = list;
            return world;
        }
            /// <summary>
            /// 随机添加球体1
            /// </summary>
            /// <returns></returns>
            public static World RandomScene1()
        {
            World world = new World();
            world.GeometryObjects.Add(new Sphere(new Point3D(0, -1000, 0), 1000, new Lambertian(new Vector3D(0.5, 0.5, 0.5))));
            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    double chooseMat = RTUtils.rd.NextDouble();
                    Point3D center = new Point3D(a + 0.9 * RTUtils.rd.NextDouble(), 0.2, b + 0.9 * RTUtils.rd.NextDouble());
                    if ((center - new Vector3D(4, 0.2, 0)).Length() > 0.9)
                    {
                        if (chooseMat < 0.8)
                        {
                            world.GeometryObjects.Add(new Sphere(center, 0.2, new Lambertian(new Vector3D(RTUtils.rd.NextDouble() * RTUtils.rd.NextDouble(), 
                                RTUtils.rd.NextDouble() * RTUtils.rd.NextDouble(), RTUtils.rd.NextDouble() * RTUtils.rd.NextDouble()))));
                        }
                        else if (chooseMat < 0.95)
                        {
                            world.GeometryObjects.Add(new Sphere(center, 0.2, new Metal(new Vector3D(0.5 * (1 + RTUtils.rd.NextDouble()), 0.5 * (1 + RTUtils.rd.NextDouble()), 
                                0.5 * (1 + RTUtils.rd.NextDouble())), 0.5 * RTUtils.rd.NextDouble())));
                        }
                        else
                        {
                            world.GeometryObjects.Add(new Sphere(center, 0.2, new Dielectrics(1.5, new Vector3D(1, 1, 1))));
                        }
                    }
                }
            }
            world.GeometryObjects.Add(new Sphere(new Point3D(0, 1, 0), 1.0, new Dielectrics(1.5, new Vector3D(1, 1, 1))));
            world.GeometryObjects.Add(new Sphere(new Point3D(-4, 1, 0), 1.0, new Lambertian(new Vector3D(0.4, 0.2, 0.1))));
            world.GeometryObjects.Add(new Sphere(new Point3D(4, 1, 0), 1.0, new Metal(new Vector3D(0.7, 0.6, 0.5), 0)));
          
            return world;
        }
        public void Bulid()
        {
            //    //纹理测试组合
            //    //纯色纹理
            //    Constant_texture constant_Texture = new Constant_texture(new SColor(0.5, 0.5, 0.5));
            //    //棋盘纹理
            //    Checker_texture checker_Texture = new Checker_texture(new Constant_texture(new SColor(0.5,0.5,0.5)),new Constant_texture(new SColor(0,1,0)));
            //    //图片纹理
            //    BMP_texture bmp_Texture = new BMP_texture();
            //    bmp_Texture.BMP = (Bitmap)Bitmap.FromFile("test1.jpg", false);
            //    //柏林噪声纹理
            //    Noise_texture noise_Texture = new Noise_texture(40);
            //    Sphere sphere1 = new Sphere(new Point3D(-0.6, 0, -0.5), 0.2, new Lambertian(new Vector3D(1, 0, 0), constant_Texture));
            //    Sphere sphere2 = new Sphere(new Point3D(0, -100.5, -0.5), 100, new Lambertian(new Vector3D(0.8, 0.8, 0.0),checker_Texture));
            //    Sphere sphere3 =new Sphere(new Point3D(0.6, 0, -0.5), 0.2, new Metal(new Vector3D(0.8, 0.6, 0.2), 0.0,bmp_Texture));
            //    Sphere sphere4 = new Sphere(new Point3D(-0.2, 0, -0.5), 0.2, new Dielectrics(1.5,new Vector3D(1,1,1)));
            //    Sphere sphere5 = new Sphere(new Point3D(0.2, 0, -0.5), 0.2, new Lambertian(new Vector3D(0.8, 0.6, 0.2), noise_Texture));
            //    AddObj(sphere1);
            //    AddObj(sphere2);
            //    AddObj(sphere3);
            //    AddObj(sphere4);
            //    AddObj(sphere5);


            //    //材质测试图部分
            //    //漫反射镜面反射（哑光）玻璃球材质图
            //    Sphere sphere1 = new Sphere(new Point3D(0, 0, -1), 0.5, new Lambertian(new Vector3D(0.8, 0.3, 0.3)));
            //    Sphere sphere2 = new Sphere(new Point3D(0, -100.5, -1), 100, new Lambertian(new Vector3D(0.8, 0.8, 0.0)));
            //    Sphere sphere3 = new Sphere(new Point3D(1, 0, -1), 0.5, new Metal(new Vector3D(0.8, 0.6, 0.2), 0.3));
            //    Sphere sphere4 = new Sphere(new Point3D(-1, 0, -1), 0.5, new Dielectrics(1.5, new Vector3D(1, 1, 1)));
            //    Sphere sphere5 = new Sphere(new Point3D(-1, 0, -1), -0.45, new Dielectrics(1.5, new Vector3D(1, 1, 1)));
            //    AddObj(sphere1);
            //    AddObj(sphere2);
            //    AddObj(sphere3);
            //    AddObj(sphere4);
            //    AddObj(sphere5);

                  //测试obj解析(相机参数要重新设置)
                  BMP_texture apple_texture = new BMP_texture((Bitmap)Bitmap.FromFile(@".\apple.png", false));
                  Lambertian apple_material = new Lambertian(new Vector3D(1, 0, 0), apple_texture);
                  Obj apple = new Obj(@".\apple2.obj", apple_material);
                  BMP_texture plane_texture = new BMP_texture((Bitmap)Bitmap.FromFile(@".\plane.png", false));
                  Lambertian plane_material = new Lambertian(new Vector3D(1, 0, 0), plane_texture);
                  Obj plane = new Obj(@".\plane.obj", plane_material);
                  AddObj(apple);
                  AddObj(plane);
        }
    }
}
