using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;


namespace WebApplication1
{
    class ExcelPermission2
    {
        public static void SetExcelPermission(string excelPath,string excelNewPath,string userName)
        {
            string path = excelPath;
            string newPath = excelNewPath;

            if (File.Exists(newPath))
                File.Delete(newPath);

            // 打开Excel程序
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            // 打开工作薄
            Workbook workbook = excel.Workbooks.Open(path, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            excel.Visible = false;
            excel.DisplayAlerts = false;
            try
            {

                // 设置工程薄权限
                // 获取只读权限
                int iPermissions = Convert.ToInt32(MsoPermission.msoPermissionRead);
                // 是否允许打印
                bool isAllowPrint = false;
                if (isAllowPrint)
                {
                    iPermissions += Convert.ToInt32(MsoPermission.msoPermissionPrint);
                }
                // 是否允许拷贝
                bool isAllowCopy = false;
                if (isAllowCopy)
                {
                    iPermissions += Convert.ToInt32(MsoPermission.msoPermissionExtract);
                }
               // if (excel.MailSession == null)
                  //  excel.MailLogon("kongy@mysoft.com.cn", "ky19830816", Type.Missing);

                // 添加权限
                workbook.Permission.Add(userName, iPermissions, null);

                // 保存文件
                workbook.SaveAs(newPath, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                // 清空工作薄
                workbook = null;
                // 退出Excel
                excel.Quit();

                // 回收
                System.GC.Collect();
            }
        }
    }
}
