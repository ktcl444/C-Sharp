using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
//using System.Web;
//using System.Web.Security;
//using System.Security.Principal;
//using System.Runtime.InteropServices;

public class ExcelPermission
{

    //public const int LOGON32_LOGON_INTERACTIVE = 2;
    //public const int LOGON32_PROVIDER_DEFAULT = 0;
    //static WindowsImpersonationContext impersonationContext;

    //[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    //public static extern int LogonUser(String lpszUserName, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

    //[DllImport("advapi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
    //public extern static int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

    //public static void SetExcelPermission2(string excelPath, string excelNewPath, string userName)
    //{
    //    try
    //    {
    //        if (impersonateValidUser("kongy", "mysoft.com.cn", "ky19830816"))
    //        {
    //            SetExcelPermission(excelPath, excelNewPath, userName);
    //            undoImpersonation();
    //        }
    //        else
    //        {      //Your impersonation failed. Therefore, include a fail-safe mechanism here.   
    //        }
    //    }
    //    catch (Exception)
    //    {

    //        throw;
    //    }
    //}

    //private static bool impersonateValidUser(String userName, String domain, String password)
    //{
    //    WindowsIdentity tempWindowsIdentity;
    //    IntPtr token = IntPtr.Zero;
    //    IntPtr tokenDuplicate = IntPtr.Zero;
    //    if (LogonUser(userName, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token) != 0)
    //    {
    //        if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
    //        {
    //            tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
    //            impersonationContext = tempWindowsIdentity.Impersonate();
    //            if (impersonationContext != null)
    //                return true;
    //            else return false;
    //        }
    //        else return false;
    //    }
    //    else
    //        return false;
    //}
    //private static void undoImpersonation() { impersonationContext.Undo(); }



    public static void SetExcelPermission(string excelPath, string excelNewPath, string userName)
    { 
        if(AdLogin.UserLoginForDomain("kongy","ky19830816") )
        {
            SetExcelPermissionMain(excelPath, excelNewPath, userName);
        }
    }

    public static void SetExcelPermissionMain(string excelPath, string excelNewPath, string userName)
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

        //Microsoft.Office.Core.Permission IrmPermission;
        try
        {

            //if (excel.MailSession == null || excel.MailSession == System.DBNull.Value )
            //    excel.MailLogon("kongy@mysoft.com.cn", "ky19830816", Type.Missing);

            //IrmPermission = workbook.Permission;

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
            //  excel.MailLogon(@"kongy@mysoft.com.cn", "ky19830816", Type.Missing);
            MsoPermission m = (MsoPermission)Enum.Parse(typeof(MsoPermission), iPermissions.ToString());

            //IrmPermission.RemoveAll();
            //IrmPermission.Add(@userName, MsoPermission.msoPermissionRead, null);


            //if (impersonateValidUser("kongy", "mysoft.com.cn", "ky19830816"))
            //{

                // 添加权限
                workbook.Permission.Add(@userName, m, null);
            //    undoImpersonation();
            //}

            //// 保存文件
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
