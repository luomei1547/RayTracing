using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RayTracing
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public partial class Form1 : Form
    {
        List<Bitmap> avi_bmps = new List<Bitmap>();//存放生成avi的每一帧bmp位图
        /// <summary>
        /// 窗体初始化
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 渲染
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_render_Click(object sender, EventArgs e)
        {
            //开始时间
            DateTime startTime = DateTime.Now;

            //构建世界
            World world = new World();
            world.Bulid();//根据bulid构建
            //world = World.RandomScene1();//随机构建函数1（封面图1）
            //world = World.RandomScene2();//随即构建函数2（封面图2）

            //根据world的物体构造层次包围盒树
            BVH_node bVH = new BVH_node(world.GeometryObjects, world.GeometryObjects.Count, 0, 0);

            //画布参数
            int nx = 200;//宽
            int ny = 100;//高
            Bitmap bitmap = new Bitmap(nx, ny);//画布
            int ns = 10;//画布上一个像素点的实际随机采样数

            //相机参数
            Point3D lookfrom = new Point3D(1, 0.5f, 1.5f);
            Vector3D lookat = new Vector3D(0, 0, 0f);
            Camera cam = new Camera(lookfrom, lookat, new Vector3D(0, 1, 0), 60, (double)(nx) / (double)(ny), 0, (lookfrom - lookat).Length(), 0.0, 1.0);
            //Camera cam = new Camera();//默认相机参数,使用默认相机，则相机获取射线的函数一样要替换成默认

            //遍历画布上的每一点，计算出该点的颜色并画到画布上
            for (int i = 0; i < nx; i++)
            {
                for (int j = 0; j < ny; j++)
                {
                    SColor col = new SColor();//像素点颜色
                    for (int s = 0; s < ns; s++)//随机采样，消除锯齿
                    {
                        //像素点的每一次采样具体计算
                        double u = (double)(i + RTUtils.rd.NextDouble()) / (double)nx;//该点在水平方向上的比例
                        double v = (double)(j + RTUtils.rd.NextDouble()) / (double)ny;//该点在垂直方向上的比例
                        Ray r = cam.GetRayWith_Time(u, v);//获取从相机到该点的射线（运动+景深）
                        //Ray r = cam.GetRay(u, v);//获取从相机到该点的射线(默认)
                        //Ray r = cam.GetRayWith_Time(u, v);//获取从相机到该点的射线（景深）
                        SColor colTemp = RTUtils.Color(r, world, 0);//采样点颜色值
                        col = col + colTemp;//采样颜色总和
                    }
                    //ns次采样的平均颜色
                    col = col / ns;
                    col = new SColor(Math.Sqrt(col.R), Math.Sqrt(col.G), Math.Sqrt(col.B));//对颜色进行伽马矫正
                    bitmap.SetPixel(i, ny - j - 1, col.GetColor255());//画上该点颜色
                    //设置进度
                    int pro = (int)((((i + 1) * ny + j) * 100.0) / (nx * ny) + 1);
                    SetPos(pro);
                }
            }
            string fileneame = "result" + DateTime.Now.Millisecond.ToString() + ".jpeg";
            bitmap.Save(fileneame);
            //avi_bmps.Add(bitmap);
            pictureBox1.Image = bitmap;
            textBox1.Text = "渲染时间：" + (DateTime.Now - startTime).ToString();
        }
        /// <summary>
        /// 设置进度条的进度
        /// </summary>
        /// <param name="value">进度值</param>
        private void SetPos(int value)//设置进度条当前进度
        {
            if (value <= progressBar1.Maximum)//如果值有效
            {
                progressBar1.Value = value;//设置进度值
            }
            else
            {
                progressBar1.Value = progressBar1.Maximum;//设置进度值
            }
            Application.DoEvents();//防止父子窗体假死
        }
        /// <summary>
        /// 根据avi_bmps内的bmp生成avi
        /// </summary>
        /// <param name="avi_bmps">存放生成avi的每一帧bmp位图的list</param>
        public void CreateAVI(List<Bitmap> avi_bmps)
        {
            AVIWriter writer = new AVIWriter();
            //参数分别为：(avi名称，帧率(24常用)，帧宽，帧高)所以存放的bitmap宽高不能低于该值。
            Bitmap avi_frame = writer.Create("test.avi", 24, 200, 100);
            for (int i = 0; i < avi_bmps.Count; i++)
            {
                avi_bmps[i].RotateFlip(RotateFlipType.Rotate180FlipX);
                //加载一张图作为帧
                writer.LoadFrame(avi_bmps[i]);
                //将帧添加到AVI文件里
                writer.AddFrame();
            }
            //释放资源
            writer.Close();
            avi_frame.Dispose();
            MessageBox.Show("AVI生成完成");
        }
    }
}
