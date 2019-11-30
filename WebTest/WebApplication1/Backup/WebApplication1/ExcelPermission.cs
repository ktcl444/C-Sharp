using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using System.Security.Principal;
using System.Runtime.InteropServices;

/// <summary>
///ExcelPermission 的摘要说明
/// </summary>
public class ExcelPermission
{
    /// <summary>
    /// 模拟windows身份登录
    /// </summary>
    /// <param name="excelPath"></param>
    /// <param name="excelNewPath"></param>
    /// <param name="userName"></param>
    /// <param name="SimulationUserName"></param>
    /// <param name="SimulationUserPwd"></param>
    /// <param name="SimulationDomain"></param>
    public static void SetExcelPermission(string excelPath, string excelNewPath, string userName, string SimulationUserName, string SimulationUserPwd, string SimulationDomain)
    {
        IntPtr admin_token = default(IntPtr);
        WindowsIdentity wid_admin = null;
        WindowsImpersonationContext wic = null;

        //在程序中模拟域帐户登录
        if (WinLogonHelper.LogonUser(SimulationUserName, SimulationDomain, SimulationUserPwd, 9, 0, ref admin_token) != 0)
        {
            using (wid_admin = new WindowsIdentity(admin_token))
            {
                using (wic = wid_admin.Impersonate())
                {
                    SetExcelPermission(excelPath, excelNewPath, userName);
                }
            }
        }
    }

    /// <summary>
    /// 创建虚拟桌面
    /// </summary>
    /// <param name="excelPath"></param>
    /// <param name="excelNewPath"></param>
    /// <param name="userName"></param>
    /// <param name="SimulationUserName"></param>
    /// <param name="SimulationUserPwd"></param>
    /// <param name="SimulationDomain"></param>
    public static void SetExcelPermissionByGetDesktop(string excelPath, string excelNewPath, string userName, string SimulationUserName, string SimulationUserPwd, string SimulationDomain)
    {
        GetDesktopWindow();
        IntPtr hwinstaSave = GetProcessWindowStation();
        IntPtr dwThreadId = GetCurrentThreadId();
        IntPtr hdeskSave = GetThreadDesktop(dwThreadId);
        IntPtr hwinstaUser = OpenWindowStation("WinSta0", false, 33554432);
        if (hwinstaUser == IntPtr.Zero)
        {
            RpcRevertToSelf();

            throw new Exception("GetProcessWindowStation fauled :" + hwinstaUser.ToString());
            return;
        }
        SetProcessWindowStation(hwinstaUser);
        IntPtr hdeskUser = OpenDesktop("Default", 0, false, 33554432);
        RpcRevertToSelf();
        if (hdeskUser == IntPtr.Zero)
        {
            SetProcessWindowStation(hwinstaSave);
            CloseWindowStation(hwinstaUser);
            throw new Exception("OpenDesktop fauled :" + hdeskUser.ToString());
            return;
        }
        SetThreadDesktop(hdeskUser);

        IntPtr dwGuiThreadId = dwThreadId;

        if (string.IsNullOrEmpty(SimulationUserName))
        {
            SetExcelPermission(excelPath, excelNewPath, userName);
        }
        else
        {
            SetExcelPermission(excelPath, excelNewPath, userName, SimulationUserName, SimulationUserPwd, SimulationDomain);
        }

        dwGuiThreadId = IntPtr.Zero;
        SetThreadDesktop(hdeskSave);
        SetProcessWindowStation(hwinstaSave);
        CloseDesktop(hdeskUser);
        CloseWindowStation(hwinstaUser);
    }

    /// <summary>
    /// 创建虚拟桌面
    /// </summary>
    /// <param name="excelPath"></param>
    /// <param name="excelNewPath"></param>
    /// <param name="userName"></param>
    public static void SetExcelPermissionByGetDesktop(string excelPath, string excelNewPath, string userName)
    {
        SetExcelPermissionByGetDesktop(excelPath, excelNewPath, userName,string.Empty, string.Empty ,string.Empty );
    }

    public static void SetExcelPermission(string excelPath, string excelNewPath, string userName)
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

            // 添加权限
           workbook.Permission.Add(userName, iPermissions, null);

            // 保存文件
            workbook.SaveAs(newPath, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }
        catch (Exception ex )
        {

            throw ex;
        }
        finally
        {
            // 清空工作薄
            workbook = null;
            // 退出Excel
            excel.Quit();
            Kill(excel);
            // 回收
            System.GC.Collect();
        }
    }

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public extern static int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
    public static void Kill(Microsoft.Office.Interop.Excel.Application excel)
    {

        if (excel == null)
        {
            return;
        }
        try
        {
            IntPtr t = new IntPtr(excel.Hwnd); //得到这个句柄，具体作用是得到这块内存入口

            int k = 0;
            GetWindowThreadProcessId(t, out k); //得到本进程唯一标志k
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k); //得到对进程k的引用

            p.Kill(); //关闭进程k
        }
        catch (Exception ex)
        {

        }

    }

    internal static class WinLogonHelper
    {
        /// <summary>
        /// 模拟windows登录域
        /// </summary>
        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
    }

            [DllImport("user32.dll")]
        static extern int GetDesktopWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetProcessWindowStation();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThreadId();

        [DllImport("user32.dll")]
        static extern IntPtr GetThreadDesktop(IntPtr dwThread);

        [DllImport("user32.dll")]
        static extern IntPtr OpenWindowStation(string a, bool b, int c);

        [DllImport("user32.dll")]
        static extern IntPtr OpenDesktop(string lpszDesktop, uint dwFlags,
        bool fInherit, uint dwDesiredAccess);

        [DllImport("user32.dll")]
        static extern IntPtr CloseDesktop(IntPtr p);

        [DllImport("rpcrt4.dll", SetLastError = true)]
        static extern IntPtr RpcImpersonateClient(int i);


        [DllImport("rpcrt4.dll", SetLastError = true)]
        static extern IntPtr RpcRevertToSelf();

        [DllImport("user32.dll")]
        static extern IntPtr SetThreadDesktop(IntPtr a);

        [DllImport("user32.dll")]
        static extern IntPtr SetProcessWindowStation(IntPtr a);
        [DllImport("user32.dll")]
        static extern IntPtr CloseWindowStation(IntPtr a);

}
