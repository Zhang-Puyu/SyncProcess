﻿using System;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;


namespace Processor.Methods
{
    public class ProSlk : AbstractProcessor
    {
        #region 单例模式
        private static ProSlk instance = null;
        private static readonly object padlock = new object();
        public static ProSlk Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ProSlk();
                    return instance;
                }
            }
        }

        private ProSlk() { }
        #endregion

        public override void SingleToSingle(string orifile, string csvfile)
        {
            // 创建Excel应用程序对象
            InfoMsgEvent?.Invoke("Init Excel");
            Excel.Application excelApp = new Excel.Application
            {
                Visible = false // 可以设置为true以显示Excel应用程序界面，方便调试
            };

            try
            {
                // 打开SLK文件
                InfoMsgEvent?.Invoke("Open .slk file");
                Excel.Workbook workbook = excelApp.Workbooks.Open(orifile);

                // 将SLK文件另存为CSV格式
                workbook.SaveAs(csvfile, Excel.XlFileFormat.xlCSV,
                    Type.Missing, Type.Missing,
                    false, false,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    Excel.XlSaveConflictResolution.xlLocalSessionChanges,
                    Type.Missing, Type.Missing);

                // 关闭工作簿和Excel应用程序
                workbook.Close(false, Type.Missing, Type.Missing);
                excelApp.Quit();
            }
            catch (Exception ex)
            {
                ErrorMsgEvent?.Invoke($"Error in process {Path.GetFileName(orifile)}: \n" + ex.Message);
            }
            finally
            {
                // 释放Excel对象
                if (excelApp != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            }
        }
    }
}
