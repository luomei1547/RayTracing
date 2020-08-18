using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    /// <summary>
    /// 纹理
    /// </summary>
    public abstract class Texture
    {
        public abstract SColor GetColor(ShadeRec sr);
    }

    /// <summary>
    /// 纯色纹理
    /// </summary>
    class Constant_texture : Texture
    {
        SColor color = new SColor();
        public Constant_texture()
        {

        }
        public Constant_texture(SColor color)
        {
            Color = color;
        }

        public SColor Color { get => color; set => color = value; }

        public override SColor GetColor(ShadeRec sr)
        {
            return Color;
        }
    }

    /// <summary>
    /// 格子纹理
    /// </summary>
    class Checker_texture : Texture
    {
        Constant_texture one = new Constant_texture();
        Constant_texture two = new Constant_texture();
        public Checker_texture()
        {

        }
        public Checker_texture(Constant_texture _one, Constant_texture _two)
        {
            One = _one;
            Two = _two;
        }

        internal Constant_texture One { get => one; set => one = value; }
        internal Constant_texture Two { get => two; set => two = value; }

        public override SColor GetColor(ShadeRec sr)
        {
            double sines = Math.Sin(10 * sr.P.X) * Math.Sin(10 * sr.P.Y) * Math.Sin(10 * sr.P.Z);
            if (sines < 0)
            {
                return one.GetColor(sr);
            }
            else
            {
                return two.GetColor(sr);
            }
        }
    }

    /// <summary>
    /// 图片纹理
    /// </summary>
    class BMP_texture : Texture
    {
        //纹理图片
        Bitmap bmp = new Bitmap(100, 100);

        //分辨率
        int hres = 100;
        int vres = 100;
        public Bitmap BMP
        {
            get
            {
                return bmp;
            }
            set
            {
                bmp = value;
                hres = value.Width;
                vres = value.Height;
            }
        }
        public BMP_texture()
        {

        }
        public BMP_texture(Bitmap bitmap)
        {
            BMP = bitmap;
        }
        public override SColor GetColor(ShadeRec sr)
        {
            int row;
            int column;
            Vector3D uv = new Vector3D(sr.U, sr.V, 0);
            uv = FormatUV(uv);
            column = (int)((hres - 1) * uv.X);
            row = (int)((vres - 1) * uv.Y);
            SColor sColor = new SColor(bmp.GetPixel(column,  row));
            return sColor;
        }
        /// <summary>
        /// 规格化uv
        /// </summary>
        /// <param name="uv"></param>
        /// <returns></returns>
        Vector3D FormatUV(Vector3D uv)
        {
            while (uv.X > 1)
                uv.X -= 1;
            while (uv.X < 0)
                uv.X += 1;
            while (uv.Y > 1)
                uv.Y -= 1;
            while (uv.Y < 0)
                uv.Y += 1;
            return uv;
        }
    }

    /// <summary>
    /// 柏林噪声纹理
    /// </summary>
    class Noise_texture : Texture
    {
        double scale=0;

        public override SColor GetColor(ShadeRec sr)
        {
            SColor sColor;
            if (Scale != 0)
            {
                // 加缩放和扰动后
                sColor = new SColor(1, 1, 1) * 0.5 * (1 + Math.Sin(scale * sr.P.X + 5 * Turb(scale * sr.P)));
                //sColor= new SColor(1, 1, 1) * Noise(Scale * sr.P);
                return sColor;
            }
            else
            {
                sColor = new SColor(1, 1, 1) * Noise(sr.P);
            }

            return sColor;
        }


        static Vector3D[] randouble;
        static int[] perm_x;
        static int[] perm_y;
        static int[] perm_z;

        public Noise_texture(double scale)
        {
            Scale = scale;
            randouble = Perlin_generate();
            Perm_x = Perlin_generate_perm();
            Perm_y = Perlin_generate_perm();
            Perm_z = Perlin_generate_perm();
        }
        public Noise_texture()
        {
            randouble = Perlin_generate();
            Perm_x = Perlin_generate_perm();
            Perm_y = Perlin_generate_perm();
            Perm_z = Perlin_generate_perm();
        }
        public Vector3D[] Randouble { get => randouble; set => randouble = value; }
        public int[] Perm_x { get => perm_x; set => perm_x = value; }
        public int[] Perm_y { get => perm_y; set => perm_y = value; }
        public int[] Perm_z { get => perm_z; set => perm_z = value; }
        public double Scale { get => scale; set => scale = value; }

        public double Trilinear_interp(double[,,] c, double u, double v, double w)
        {
            double accum = 0;
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < 2; k++)
                        accum += (i * u + (1 - i) * (1 - u)) * (j * v + (1 - j) * (1 - v)) * (k * w + (1 - k) * (1 - w)) * c[i, j, k];
            return accum;
        }

        public double Perlin_interp(Vector3D[,,] c, double u, double v, double w)
        {
            double uu = u * u * (3 - 2 * u);
            double vv = v * v * (3 - 2 * v);
            double ww = w * w * (3 - 2 * w);
            double accum = 0;
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    for (int k = 0; k < 2; ++k)
                    {
                        Vector3D weight_v = new Vector3D(u - i, v - j, w - k);
                        accum += (i * uu + (1 - i) * (1 - uu)) *
                            (j * vv + (1 - j) * (1 - vv)) *
                            (k * ww + (1 - k) * (1 - ww)) * Vector3D.DotProduct(c[i, j, k], weight_v);
                    }
                }
            }
            return accum;
        }

        // 噪声扰动
        public double Turb(Vector3D p, int depth = 7)
        {
            double accum = 0;
            Vector3D temp_p = p;
            double weight = 1.0;
            for (int i = 0; i < depth; i++)
            {
                accum += weight * Noise(temp_p);
                weight *= 0.5;
                temp_p *= 2;
            }
            return Math.Abs(accum);
        }
        public double Noise(Vector3D p)
        {
            double u = p.X - Math.Floor(p.X);
            double v = p.Y - Math.Floor(p.Y);
            double w = p.Z - Math.Floor(p.Z);

             // hermite cubic 方法平滑
             u = u * u * (3 - 2 * u);
             v = v * v * (3 - 2 * v);
             w = w * w * (3 - 2 * w);

            int i = (int)(4 * p.X) & 255;
            int j = (int)(4 * p.Y) & 255;
            int k = (int)(4 * p.Z) & 255;

            Vector3D[,,] c = new Vector3D[2, 2, 2];
            for (int di = 0; di < 2; di++)
                for (int dj = 0; dj < 2; dj++)
                    for (int dk = 0; dk < 2; dk++)
                        c[di, dj, dk] = randouble[perm_x[(i + di) & 255] ^ perm_y[(j + dj) & 255] ^ perm_z[(k + dk) & 255]];

            //return trilinear_interp(c, u, v, w);
            return Perlin_interp(c, u, v, w);
        }

        /// <summary>
        /// 主序列，生成（0-1）的随机序列
        /// </summary>
        /// <returns></returns>
        public Vector3D[] Perlin_generate()
        {
            Vector3D[] p = new Vector3D[256];
            for (int i = 0; i < 256; ++i)
            {
                p[i] = new Vector3D(RTUtils.rd.NextDouble(), RTUtils.rd.NextDouble(), RTUtils.rd.NextDouble());
            }
            return p;
        }
        /// <summary>
        /// 分量的随机序列，初始序列为1-255，然后遍历，当前位置和一个随机生成的位置进行交换
        /// 达到序列随机化
        /// </summary>
        /// <param name="p"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public int[] Permute(int[] p, int n)
        {
            for (int i = n - 1; i > 0; --i)
            {
                int target = (int)(RTUtils.rd.NextDouble() * (i + 1));
                int temp = p[target];
                p[target] = p[i];
                p[i] = temp;
            }
            return p;
        }
        public int[] Perlin_generate_perm()
        {
            int[] p = new int[256];
            for (int i = 0; i < 256; ++i)
            {
                p[i] = i;
            }
            return Permute(p, 256);
        }

    }
}
