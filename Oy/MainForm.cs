﻿using System;
using System.Windows.Forms;

namespace Oy.CAD2006
{
    public partial class MainForm : Form
    {
        private readonly lib.Utils utils = new lib.Utils();
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
            string filePath = utils.GetFilePath();
            this.textBox1.Text = filePath;
            if (filePath != null)
            {
                lib.Excel excel= new lib.Excel();
                excel.SaveExcel(filePath);
            }

        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Esc键退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EscClose_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar==(char)Keys.Escape)
            {
                this.Close();
            }
        }

        private void PingButton_Click(object sender, EventArgs e)
        {
            string pingAddress = this.textBox1.Text;
            if (pingAddress.Length>0)
            {
                bool pingResult = utils.Ping(pingAddress);
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