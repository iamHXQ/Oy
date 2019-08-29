﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Oy.CAD2006.CommandMethod;
using AutoCADCommands;
using System.Linq;
using System.Collections.Generic;

[assembly: CommandClass(typeof(CommandMethod))]
namespace Oy.CAD2006.CommandMethod
{
    public class CommandMethod
    {
        //GUI.MainForm mainForm = new GUI.MainForm();
        //mainForm.ShowDialog(Application.MainWindow);
        //mainForm.Dispose();
        [CommandMethod("TT")]
        public void OpenMainForm() => Application.ShowModalDialog( Application.MainWindow, new GUI.MainForm());


        [CommandMethod("Greating")]
        public void Greating() => lib.AutoCAD.Greating();

        [CommandMethod("ExportDocument")]
        public void ExportDocument() => lib.Document.ExportDocument();

        /// <summary>
        /// 写入测试数据
        /// </summary>
        [CommandMethod("WNOD")]
        public void WNOD() => Utils.NamedObjectDictionary.WriteToNOD("asd", "asd");

        /// <summary>
        /// 读取测试数据
        /// </summary>
        [CommandMethod("RNOD")]
        public void RNOD() => Utils.NamedObjectDictionary.ReadFromNOD("asd");

        [CommandMethod("test")]
        public void Test()
        {
            ObjectId[] objectId =Interaction.GetSelection("\n选择多段线", "LWPOLYLINE");
            var ents = objectId.QSelect(x => x).ToList();
            List<TableData> tableDataList = new List<TableData>();

            int StartBoundaryPointID=1;

            for (int i = 0; i < ents.Count; i++)
            {
                Polyline polyline = ents[i] as Polyline;
                var point3Ds = polyline.GetPolyPoints().ToArray();
                //保留面积两位小数
                double Area = System.Math.Round(polyline.Area, 2);
                TableData tableData = new TableData(point3Ds, Area,i+1, 1, StartBoundaryPointID, (i + 1).ToString());
                StartBoundaryPointID += point3Ds.Length;
                tableDataList.Add(tableData);
            }

            string saveFilePath = new Utils.Interaction().GetFilePath();
            if (saveFilePath != null)
            {
                Points2Excel excel2 = new Points2Excel(saveFilePath, tableDataList.ToArray(),
                    "潘桥街道横塘村城中村改造工程二期(低效用地)", "NZ-2019-123");
                excel2.Save();
            }
        }




        
        [CommandMethod("test2")]
        public void Test2()
        {
            ObjectId[] ids = Interaction.GetSelection("\n选择多段线", "LWPOLYLINE");
            double epsilon = Interaction.GetValue("\n最小距离",0.05);//过滤距离,默认大小需要设置文件
            ids.QForEach<Polyline>(poly =>
            {
                int count = Algorithms.PolyClean_ReducePoints(poly, epsilon);
            });
        }
    }
}