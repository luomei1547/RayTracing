using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// 材质
    /// </summary>
    public  class Material
    {
        private Texture texture;
        private bool istexture = false;
        public Texture Texture { get => texture; set => texture = value; }
        public bool Istexture { get => istexture; set => istexture = value; }

        public virtual bool Scatter(Ray rIn, ShadeRec rec, ref Vector3D attenuation, ref Ray scattered)
        {
            return false;
        }
        /// <summary>
        /// 添加发光属性，默认不发光全黑
        /// </summary>
        public virtual SColor Emitted(ShadeRec rec)
        {
            return new SColor(0, 0, 0);
        }
    }
    /// <summary>
    /// 漫反射材质
    /// </summary>
    public class Lambertian : Material
    {
        private Vector3D albedo;
        public Lambertian(Vector3D albedo)
        {
            this.Albedo = albedo;
        }
        public Lambertian(Vector3D albedo, Texture texture)
        {
            this.Albedo = albedo;
            this.Texture = texture;
            this.Istexture = true;
        }
        internal Vector3D Albedo { get => albedo; set => albedo = value; }


        public override bool Scatter(Ray rIn, ShadeRec rec, ref Vector3D attenuation, ref Ray scattered)
        {
            Vector3D target = rec.P + rec.Normal + RTUtils.RandomInUnitSphere();
            scattered = new Ray(new Point3D(rec.P.X, rec.P.Y, rec.P.Z), target - rec.P);
            if (rec.Material.Istexture)
            {
                SColor color = rec.Material.Texture.GetColor(rec);
                attenuation = new Vector3D(color.R, color.G, color.B);
            }
            else
            {
                attenuation = albedo;
            }
            return true;
        }
    }

    /// <summary>
    /// 体积雾材质
    /// </summary>
    class Isotropic : Material
    {
        public Isotropic(Texture texture)
        {
            this.Texture = texture;
        }

        public override bool Scatter(Ray rIn, ShadeRec rec, ref Vector3D attenuation, ref Ray scattered)
        {
            scattered = new Ray(new Point3D(rec.P), RTUtils.RandomInUnitSphere());
            SColor color = rec.Material.Texture.GetColor(rec);
            attenuation = new Vector3D(color.R, color.G, color.B);
            return true;
        }
    }

    /// <summary>
    /// 金属材质
    /// </summary>
    public class Metal : Material
    {
        private Vector3D albedo;
        private double fuzz;

        public Metal(Vector3D albedo, double fuzz)
        {
            this.Albedo = albedo;
            this.fuzz = fuzz;
        }

        public Metal(Vector3D albedo, double fuzz, Texture texture)
        {
            this.Albedo = albedo;
            this.fuzz = fuzz;
            this.Texture = texture;
            this.Istexture = true;
        }
        public double Fuzz { get => fuzz; set => fuzz = value; }
        internal Vector3D Albedo { get => albedo; set => albedo = value; }


        public override bool Scatter(Ray rIn, ShadeRec rec, ref Vector3D attenuation, ref Ray scattered)
        {
            Vector3D reflected = RTUtils.Reflect(Vector3D.UnitVector(rIn.Direction), rec.Normal);
            scattered = new Ray(new Point3D(rec.P.X, rec.P.Y, rec.P.Z), reflected + fuzz * RTUtils.RandomInUnitSphere());
            if (rec.Material.Istexture)
            {
                SColor color = rec.Material.Texture.GetColor(rec);
                attenuation = new Vector3D(color.R, color.G, color.B);
            }
            else
            {
                attenuation = albedo;
            }
            return (Vector3D.DotProduct(scattered.Direction, rec.Normal) > 0);
        }
    }

    /// <summary>
    /// 玻璃材质
    /// </summary>
    public class Dielectrics : Material
    {
        private double refIdx;  //相对折射率
        private Vector3D attenuation;
        public Dielectrics(double refIdx,Vector3D attenuation)
        {
            this.RefIdx = refIdx;
            Attenuation = attenuation;
        }
        public Dielectrics(double refIdx, Vector3D attenuation, Texture texture)
        {
            this.RefIdx = refIdx;
            Attenuation = attenuation;
            this.Texture = texture;
            this.Istexture = true;
        }
        public double RefIdx { get => refIdx; set => refIdx = value; }
        public Vector3D Attenuation { get => attenuation; set => attenuation = value; }

        public override bool Scatter(Ray rIn, ShadeRec rec, ref Vector3D attenuation, ref Ray scattered)
        {
            Vector3D outwardNormal;
            Vector3D reflected = RTUtils.Reflect(rIn.Direction, rec.Normal);
            double niOverNt;
            if (Istexture)
            {
                attenuation = new Vector3D(Texture.GetColor(rec));
            }
            else
            {
                attenuation = Attenuation;//衰减率  
            }
            Vector3D refracted = new Vector3D();//折射向量
            double reflect_prob; //反射概率
            double cosine;
            //判断入射光线是与法线是否同侧，以保证Refract函数中dt<0,这样- n * dt = + n * Math.Abs(dt)
            if (Vector3D.DotProduct(rIn.Direction, rec.Normal) > 0)
            {
                //入射光线与法线不同侧，法线做反向，变成同侧
                outwardNormal = rec.Normal * -1;
                niOverNt = refIdx;
                cosine = refIdx * Vector3D.DotProduct(rIn.Direction, rec.Normal) / rIn.Direction.Length();
            }
            else
            {
                //入射光线与法线同侧
                outwardNormal = rec.Normal;
                niOverNt = 1.0 / refIdx;
                cosine = -Vector3D.DotProduct(rIn.Direction, rec.Normal) / rIn.Direction.Length();
            }
            if (RTUtils.Refract(rIn.Direction, outwardNormal, niOverNt, ref refracted))
            {
                reflect_prob = RTUtils.Schlick(cosine, RefIdx);
            }
            else
            {
                reflect_prob = 1.0;//无折射，全反射
            }
            if (RTUtils.rd.NextDouble() < reflect_prob)
            {
                scattered = new Ray(new Point3D(rec.P.X, rec.P.Y, rec.P.Z), reflected);
            }
            else
            {
                scattered = new Ray(new Point3D(rec.P.X, rec.P.Y, rec.P.Z), refracted);
            }
            return true;
        }
    }
    /// <summary>
    /// 发光材质
    /// </summary>
    public class AreaLight : Material
    {
        public AreaLight()
        {

        }
        public AreaLight(Texture texture)
        {
            this.Texture = texture;
            this.Istexture = true;
        }
        public override bool Scatter(Ray rIn, ShadeRec rec, ref Vector3D attenuation, ref Ray scattered)
        {
            return false;
        }
        public override SColor Emitted(ShadeRec rec)
        {
            return this.Texture.GetColor(rec);
        }
    }
}
