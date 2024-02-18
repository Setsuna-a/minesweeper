using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 扫雷1._0.Properties;

namespace 扫雷1._0
{
    // 设创建素材枚举类型
    enum Material
    {
        None, One, Two, Three, Four, Five, Six, Seven, Eight,
        QuestionMark,
        Landmine,
        XLandmine, RedLandmine,
        NotOpenQuestionMark, Flag, NotOpen
    }

    public partial class 扫雷 : Form
    {
        // 创建画布对象
        private static Graphics WindowG;

        // 存储图片素材
        private static readonly Bitmap[] GameMaterial = new Bitmap[16];

        // 记录游戏是否开始
        private static bool IsStart;

        // 记录游戏是否结束
        private static bool IsFail = false;

        // 定义地雷数量
        private const int LandmineNumber = 50;

        // 定义变量存储当前旗子数量
        private static int FlagNumber;

        // 记录当前已经打开的格子数量
        private static int OpenNumber;

        //// 创建鼠标对应事件的委托
        //private delegate void MouseDelegate(object sender, MouseEventArgs e);
        //static MouseDelegate mouse1 = new MouseDelegate(Mouse);

        public 扫雷()
        {
            // 初始化窗口
            InitializeComponent();

            // 为窗口创建画布对象
            WindowG = this.CreateGraphics();

            // 初始化游戏素材
            MaterialStart();

            // 初始化游戏
            GameStart();
            IsStart = false;
            FlagNumber = 0;
            OpenNumber = 0;
        }

        /// <summary>
        /// 初始化游戏
        /// </summary>
        private void GameStart()
        {
            // 将所有格子初始化为一个新的对象
            Square.Squares = new Square[30, 20];

            for (int x = 0; x < 30 ; x++)
            {
                for (int y = 0; y < 20 ; y++)
                {
                    Square.Squares[x, y] = new Square(x, y);
                }
            }

            // 初始化文本
            label1.Text = $"地雷数量：{LandmineNumber}";
            label2.Text = $"旗帜数量：{FlagNumber}";
        }


        /// <summary>
        /// 游戏素材初始化
        /// </summary>
        private void MaterialStart()
        {
            // 加载原始图片
            Bitmap tempBitmap = Resources.gameMaterial;

            // 切割图片
            int tileSize = 20; // 切割后每个小图片的宽度和高度
            int index = 15; // 切割后图片的索引

            for (int y = 0; y < tempBitmap.Height; y += tileSize)
            {
                // 创建一个新的Bitmap对象用于存储切割后的小图片
                Bitmap tile = new Bitmap(tileSize, tileSize);

                // 使用Graphics对象将原始图片的一部分绘制到新的Bitmap对象中
                using (Graphics g = Graphics.FromImage(tile))
                {
                    g.DrawImage(tempBitmap, new Rectangle(0, 0, tileSize, tileSize),
                        new Rectangle(0, y, tileSize, tileSize), GraphicsUnit.Pixel);
                }
                // 将切割后的小图片存储到数组中
                GameMaterial[index] = tile;
                index--;

            }

            // 释放原始图片资源
            tempBitmap.Dispose();
        }


        /// <summary>
        /// 绘制游戏界面
        /// </summary>
        private void Draw()
        {
            Graphics tempWindowG;
            // 临时的 Bitmap 对象，将画面绘制到上面再覆盖到画布上
            Bitmap tempBitmap;

            //在一个图片并在其上创建一个临时画布
            tempBitmap = new Bitmap(640, 460);
            tempWindowG = Graphics.FromImage(tempBitmap);


            //TODO:
            //绘制地图
            for (int x = 0; x < 30; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    if (Square.Squares[x, y].IsOpen == false)
                    {
                        tempWindowG.DrawImage(GameMaterial[(int)Square.Squares[x, y].OutsideThing],
                            20 + 20 * x, 40 + 20 + 20 * y);
                    }
                    else
                    {
                        // 若绘制地雷，则游戏结束
                        if (Square.Squares[x, y].InsideThing == Material.Landmine)
                        {
                            IsFail = true;
                            End(IsFail);
                            tempWindowG.DrawImage(GameMaterial[(int)Material.RedLandmine],
                                20 + 20 * x, 40 + 20 + 20 * y);
                        }
                        else
                            tempWindowG.DrawImage(GameMaterial[(int)Square.Squares[x, y].InsideThing],
                                20 + 20 * x, 40 + 20 + 20 * y);
                    }
                }
            }


            WindowG.DrawImage(tempBitmap, 0, 0);
        }


        /// <summary>
        /// 放置地雷（px, py）处及其周围不放置
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        private static void LayLandmine(int px, int py)
        {
            // 随机位置生成地雷
            Random random = new Random();
            int currentNumber = 0;

            while (currentNumber < LandmineNumber)
            {
                // 随机到一个位置
                int x = random.Next(1, 29);
                int y = random.Next(1, 19);

                // 若该位置位于( px, py )周围则不放置
                if (px >= x - 1 && px <= x + 1 && py >= y - 1 && py <= y + 1) continue;

                // 若位置无雷则放置地雷
                if (Square.Squares[x, y].InsideThing == Material.None)
                {
                    Square.Squares[x, y].InsideThing = Material.Landmine;
                    currentNumber++;
                }

            }
        }

        /// <summary>
        /// 放置地雷后放置数字
        /// </summary>
        private static void LayNumber()
        {
            // 地雷生成完毕之后，在空白区域加载数字
            for (int x = 0; x < 30; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    //若某个格子不为地雷，则访问其周围地雷数量
                    if (Square.Squares[x, y].InsideThing != Material.Landmine)
                    {
                        ushort temp = 0;
                        int[] offsets = { -1, 0, 1 };

                        foreach (int offsetX in offsets)
                        {
                            foreach (int offsetY in offsets)
                            {
                                if ((x == 0 && offsetX < 0) || (x == 29 && offsetX > 0) ||
                                    (y == 0 && offsetY < 0) || (y == 19 && offsetY > 0) || (offsetX == 0 && offsetY == 0))
                                    continue;

                                if (Square.Squares[x + offsetX, y + offsetY].InsideThing == Material.Landmine)
                                    temp++;
                            }
                        }
                        // 设置该位置数字
                        Square.Squares[x, y].InsideThing = (Material)temp;
                    }
                }

            }
        }


        private static void Mouse(object sender, MouseEventArgs e)
        {

            //确认鼠标按下的位置
            int x = (e.X) / 20 - 1;
            int y = (e.Y) / 20 - 1 - 2;

            // 若鼠标位置处于格子外则退出函数
            if (x < 0 || y < 0 || x > 29 || y > 19) { return; }


            switch (e.Button)
            {
                // 左键
                case MouseButtons.Left:
                    //若未开始，则生成地雷并根据地雷生成数字
                    if (!IsStart)
                    {
                        // 生成地雷和数字
                        LayLandmine(x, y);
                        LayNumber();

                        // 翻开选中格子
                        OpenNumber += Square.Open(new Point(x, y));

                        IsStart = true;
                    }
                    //若已经开始，则翻开对应位置
                    else
                    {
                        // 未翻开：翻开对应位置
                        if (Square.Squares[x, y].IsOpen == false)
                        {
                            OpenNumber += Square.Open(new Point(x, y));
                        }
                        // 已翻开：若非空 且 数字等于周围旗帜数量则翻开周围位置
                        else if (
                            Square.Squares[x, y].InsideThing != Material.None
                            && (int)Square.Squares[x, y].InsideThing == Square.Squares[x, y].GetFlagNum(x, y)
                            )
                        {
                            int[] offsets = { -1, 0, 1 };
                            foreach (int offsetX in offsets)
                            {
                                foreach (int offsetY in offsets)
                                {
                                    OpenNumber += Square.Open(new Point(x + offsetX, y + offsetY));
                                }
                            }
                        }
                    }
                    // 若打开的格子数量与地雷数量和为总格子数量 则游戏结束
                    if (OpenNumber + LandmineNumber == 20 * 30)
                    {
                        End(IsFail);
                    }

                    break;
                // 右键
                case MouseButtons.Right:
                    // 对未翻开格子进行切换
                    if (Square.Squares[x, y].IsOpen == false)
                    {
                        FlagNumber += Square.Squares[x, y].AlterOtsideThing();
                    }
                    break;
            }
        }



        /// <summary>
        /// 设置鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 扫雷_MouseDown(object sender, MouseEventArgs e)
        {
            //mouse1(sender, e);
            if (IsFail) return;
            Mouse(sender, e);
            label2.Text = $"旗帜数量：{FlagNumber}";
            Draw();
        }
        /// <summary>
        /// 游戏结束时调用的方法
        /// </summary>
        /// <param name="isFail"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void End(bool isFail)
        {
            游戏结束 form2 = new 游戏结束(isFail);
            //mouse1 = new MouseDelegate((object sender, MouseEventArgs e) => { return; });
            form2.Show();
        }




        /// <summary>
        /// 绘制事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 扫雷_Paint(object sender, PaintEventArgs e)
        {
            Draw();
        }
    }
}
