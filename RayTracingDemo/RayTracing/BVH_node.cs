using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RayTracing
{
    /// <summary>
    /// bvh树结点
    /// </summary>
    public class BVH_node:GeometryObject
    {
        GeometryObject left;//左节点
        GeometryObject right;//右节点
        AABB box;//当前节点盒

        internal GeometryObject Left { get => left; set => left = value; }
        internal GeometryObject Right { get => right; set => right = value; }
        public AABB Box { get => box; set => box = value; }

        public override bool Bounding_box(double t0, double t1, ref AABB b)
        {
            b = Box;
            return true;
        }

        public override bool Hit(Ray r,double t_min, ref ShadeRec rec)
        {
            if (Box.Hit(r,t_min))
            {
                ShadeRec left_rec = new ShadeRec();
                ShadeRec right_rec = new ShadeRec();
                bool hit_left = Left.Hit(r, t_min, ref left_rec);
                bool hit_right = Right.Hit(r, t_min, ref right_rec);
                if (hit_left && hit_right)
                {
                    //击中重叠部分
                    if (left_rec.T < right_rec.T)
                    {
                        //击中左子树
                        rec = left_rec;
                    }
                    else
                    {
                        //击中右子树
                        rec = right_rec;
                    }
                    return true;
                }
                else if (hit_left)
                {
                    rec = left_rec;
                    return true;
                }
                else if (hit_right)
                {
                    rec = right_rec;
                    return true;
                }
                else
                    return false;
            }
            else
                return false; //未击中任何物体
        }

        public BVH_node()
        {

        }
        public BVH_node(List<GeometryObject> list,int n,double time0,double time1)
        {
            //从geometry中构建BVH树
            int axis = (int)(3 * RTUtils.rd.NextDouble());//随机一个数，然后把整个数组按照某个方向进行排列
            //按照x/y/z的方向进行快速排序，从单一方向划分盒子。
            if (axis == 0)
            {
               list.Sort(new Box_x_compare());
            }
            else if (axis == 1)
            {
                list.Sort(new Box_y_compare());
            }
            else
            {
                list.Sort(new Box_z_compare());
            }
            if (n == 1)
            {
                Left = Right = list[0];
            }
            else if (n == 2)
            {
                Left = list[0];
                Right = list[1];
            }
            else
            {
                //把list分成两半,构建左右子树的信息
                List<GeometryObject> left = list.GetRange(0, n/ 2);
                List<GeometryObject> right = list.GetRange(n/2, n-n/2);
                Left = new BVH_node(left, left.Count, time0, time1);
                Right = new BVH_node(right, right.Count, time0, time1);
            }
            //完善本节点的信息
            AABB box_left=new AABB();
            AABB box_right=new AABB();
            if (!Left.Bounding_box(time0, time1, ref box_left) || !Right.Bounding_box(time0, time1, ref box_right))
            {
                MessageBox.Show("no bounding box in bvh_node constructor");
            }
            //节点的box就是左右子节点的合集
            box = Surrounding_box(box_left, box_right);
        }
        public AABB Surrounding_box(AABB box1, AABB box2)
        {
            Vector3D small = new Vector3D(Math.Min(box1.Min.X, box2.Min.X), Math.Min(box1.Min.Y, box2.Min.Y), Math.Min(box1.Min.Z, box2.Min.Z));
            Vector3D big = new Vector3D(Math.Max(box1.Max.X, box2.Max.X), Math.Max(box1.Max.Y, box2.Max.Y), Math.Max(box1.Max.Z, box2.Max.Z));
            return new AABB(small, big);
        }
        class Box_x_compare : IComparer<GeometryObject>
        {
            public int Compare(GeometryObject x, GeometryObject y)
            {
                AABB box_left = new AABB();
                AABB box_right = new AABB();
                if (!x.Bounding_box(0, 0, ref box_left) || !y.Bounding_box(0, 0, ref box_right))
                {
                    MessageBox.Show("no bounding box in bvh_node constructor");
                }
                return box_left.Min.X.CompareTo(box_right.Min.X);
            }
        }
        class Box_y_compare : IComparer<GeometryObject>
        {
            public int Compare(GeometryObject x, GeometryObject y)
            {
                AABB box_left = new AABB();
                AABB box_right = new AABB();
                if (!x.Bounding_box(0, 0, ref box_left) || !y.Bounding_box(0, 0, ref box_right))
                {
                    MessageBox.Show("no bounding box in bvh_node constructor");
                }
                return box_left.Min.Y.CompareTo(box_right.Min.Y);
            }
        }
        class Box_z_compare : IComparer<GeometryObject>
        {
            public int Compare(GeometryObject x, GeometryObject y)
            {
                AABB box_left = new AABB();
                AABB box_right = new AABB();
                if (!x.Bounding_box(0, 0, ref box_left) || !y.Bounding_box(0, 0, ref box_right))
                {
                    MessageBox.Show("no bounding box in bvh_node constructor");
                }
                return box_left.Min.Z.CompareTo(box_right.Min.Z);
            }
        }
    }
}
