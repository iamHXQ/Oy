﻿using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Oy.CAD2006.lib
{
    //Excel类
    class Excel
    {

        //文件路径
        static readonly string filePath = @"c:\hxq.xlsx";

        //保持并打开文件
        protected internal static void SaveExcel()
        {
            ExcelPackage package = new ExcelPackage();

            ExcelWorksheet excelWorksheet = package.Workbook.Worksheets.Add("汇总表");
            excelWorksheet.Cells["A5"].LoadFromDataTable(GetDataTable(), false);
            try
            {
                package.SaveAs(new FileInfo(filePath));
                Utils.OpenFileWithConfirmDialog(filePath);
            }

            catch (Exception)
            {
                //重试操作
                if (Utils.RetryDialog().Equals(DialogResult.Retry))
                {
                    SaveExcel();
                }
            }


        }

        private static DataTable GetDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("序列号", Type.GetType("System.Int32"));
            table.Columns.Add("地块号", Type.GetType("System.Int32"));
            table.Columns.Add("圈号", Type.GetType("System.Int32"));
            table.Columns.Add("界址点号", Type.GetType("System.Int32"));
            table.Columns.Add("纵坐标(X)", Type.GetType("System.Double"));
            table.Columns.Add("横坐标(Y)", Type.GetType("System.Double"));
            table.Columns.Add("指向点号", Type.GetType("System.Int32"));
            table.Columns.Add("距离", Type.GetType("System.Double"));
            
            
            table.Rows.Add(new object[] { 1, 1, 1, 1, 111.111, 222.2222, 2, 10 });
            table.Rows.Add(new object[] { 2, 1, 1, 2, 333.300, 444.440, 3, 20 });
            table.Rows.Add(new object[] { 3, 1, 1, 3, 555.000, 666.666, 4, 30 });
            table.Rows.Add(new object[] { 4, 1, 1, 4, 777.7, 888.88, 5, 40 });
            

            return table;
        }
    }

}