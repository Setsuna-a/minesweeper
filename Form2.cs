using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 扫雷1._0
{
    public partial class 游戏结束 : Form
    {
        public 游戏结束(bool isfail)
        {
            InitializeComponent();
            if (isfail)
            {
                label1.Text = "你失败了";
            }
            else
            {
                label1.Text = "你成功了";
            }
        }


        /// <summary>
        /// 重新开始游戏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //Process currentProcess = Process.GetCurrentProcess();
            //currentProcess.Kill();

            //// 启动新的进程
            //Process.Start(currentProcess.ProcessName);
            Application.Exit();
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
