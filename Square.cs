using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 扫雷1._0
{
    internal class Square
    {
        // 初始化方格
        public Square(int x, int y)
        {
            IsOpen = false;
            InsideThing = Material.None;
            OutsideThing = Material.NotOpen;
            Location = new Point(x, y);
            Squares[x, y] = this;
        }

        // 存储全部位置数据
        public static Square[,] Squares = new Square[30, 20];

        // 记录自己内部内容
        public Material InsideThing { get; set; }

        // 记录自己外部内容
        public Material OutsideThing { get; private set; }

        // 记录自己是否被打开
        public bool IsOpen { get; private set; }

        // 记录自身所在位置
        public Point Location;

        /// <summary>
        /// 翻开格子
        /// </summary>
        /// <param name="P">要翻开的格子的坐标</param>
        public static int Open(Point P)
        {
            int num = 0;
            // 若不在范围内则退出函数
            if(P.X < 0 || P.Y < 0 || P.X > 29 || P.Y > 19) return 0;


            // 若格子未翻开并且处于空状态则翻开该格子，然后置空
            if (Squares[P.X, P.Y].IsOpen == false && Squares[P.X, P.Y].OutsideThing == Material.NotOpen)
            {
                Squares[P.X, P.Y].IsOpen = true;
                num++;
            }
            else return 0;
        
            // 若格子为空则同时翻开周围的格子
            if (Squares[P.X, P.Y].InsideThing == Material.None)
            {
                int[] offsets = { -1, 0, 1 };
                foreach(int offsetX in offsets)
                {
                    foreach(int offsetY in offsets)
                    {      
                        // 某格子符合条件时翻开格子
                        num += Open(new Point(P.X + offsetX, P.Y + offsetY));
                        
                    }
                }
            }
            return num;
        }

        /// <summary>
        /// 修改外部内容
        /// 每次修改在 未打开 旗帜 问号 三个状态中循环
        /// </summary>
        public int AlterOtsideThing()
        {
            int flagnum = 0;
            switch (OutsideThing)
            {
                case Material.NotOpen:
                    OutsideThing = Material.Flag;
                    flagnum++;
                    break;
                case Material.Flag:
                    OutsideThing = Material.NotOpenQuestionMark;
                    flagnum--;
                    break;
                case Material.NotOpenQuestionMark:
                    OutsideThing = Material.NotOpen;
                    break;
            }
            return flagnum;
        }


        /// <summary>
        /// 获取（x, y）周围旗帜数量
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetFlagNum(int x , int y)
        {
            int num = 0;
            // 判断周围旗帜数量
            int[] offsets = { -1, 0, 1 };
            foreach (int offsetX in offsets)
            {
                foreach (int offsetY in offsets)
                {
                    if ((x + offsetX < 0) || (x + offsetX > 29) ||
                        (y + offsetY < 0) || (y + offsetY > 19) || 
                        (offsetX == 0 && offsetY == 0) || (Squares[x + offsetX, y + offsetY].IsOpen)
                       )
                    {
                        continue;
                    }
                    if (Squares[x + offsetX, y + offsetY].OutsideThing == Material.Flag)
                    {
                        num++;
                    }
                }
            }

            return num;
        }
    }
}
