using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ExcelToSql
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            AntdUI.Config.TextRenderingHighQuality = true;
            AntdUI.Config.Font = new Font("Microsoft YaHei UI", 10);
            AntdUI.Config.SetCorrectionTextRendering("Microsoft YaHei UI", "宋体");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
