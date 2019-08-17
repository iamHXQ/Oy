﻿using System;
using System.Windows.Forms;

namespace Oy.CAD2006.GUI
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 初始化form
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 保存文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFIleButton_Click(object sender, EventArgs e)
        {
            string filePath = new Utils.InterOperation().GetFilePath();
            this.textBox1.Text = filePath;
            if (filePath != null)
            {
                lib.Excel excel = new lib.Excel();
                excel.SaveExcel(filePath);
            }

        }

        /// <summary>
        /// Esc键退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EscClose_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }

        private void PingButton_Click(object sender, EventArgs e)
        {
            string pingAddress = this.textBox1.Text;
            if (pingAddress.Length > 0)
            {
                bool pingResult = new Utils.Server().Ping(pingAddress);
                if (pingResult == true && pingAddress.Length > 0)
                {
                    MessageBox.Show("成功");
                }
                else
                {
                    MessageBox.Show("失败");
                }
            }
            else
            {
                MessageBox.Show("未输入内容");
            }
        }
    }
}
