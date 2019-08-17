﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;

[assembly: CommandClass(typeof(Oy.CAD2006.AutoCADCommand))]
namespace Oy.CAD2006
{
    public class AutoCADCommand
    {
        /// <summary>
        /// 注册cad命令:Test
        /// </summary>
        [CommandMethod("TT")]
        public void OpenMainFormest()
        {
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog(Application.MainWindow);
            mainForm.Dispose();
        }

        /// <summary>
        /// 注册cad命令:Greating
        /// </summary>
        [CommandMethod("Greating")]
        public void Greating()
        {
            lib.AutoCAD.Greating();
        }

        /// <summary>
        /// 注册cad命令:ExportDocument
        /// </summary>
        [CommandMethod("ExportDocument")]
        public void ExportDocument()
        {
            lib.Document.ExportDocument();
        }
    }
}

