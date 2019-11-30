using System;
using System.Collections.Generic;
using System.Web;
//using System.Web;
//using System.Web.Security;
//using System.Security.Principal;
//using System.Runtime.InteropServices;

/// <summary>
///AdLogin 的摘要说明
/// </summary>
public class AdLogin
{


         /// <summary>
    /// 域用户校验
    /// </summary>
    /// <param name="a_strUserCode"></param>
    /// <param name="a_strPwd"></param>
    /// <param name="a_strDomain"></param>
    /// <returns></returns>
    /// <remarks></remarks>

    public static bool UserLoginForDomain(string a_strUserCode, string a_strPwd)
    {
        return UserLoginForDomain(a_strUserCode, a_strPwd, "mysoft.com.cn");
    }

    //INSTANT C# NOTE: C# does not support optional parameters. Overloaded method(s) are created above.
    //ORIGINAL LINE: Public Shared Function UserLoginForDomain(ByVal a_strUserCode As String, ByVal a_strPwd As String, Optional ByVal a_strDomain As String = "mysoft.com.cn") As Boolean
    public static bool UserLoginForDomain(string a_strUserCode, string a_strPwd, string a_strDomain)
    {
        IntPtr phToken = IntPtr.Zero;
        if (LogonUser(a_strUserCode, a_strDomain, a_strPwd, LogonType.LOGON32_LOGON_NETWORK, LogonProvider.LOGON32_PROVIDER_DEFAULT, ref phToken) != 0)
        {
            if (!(phToken != IntPtr.Zero))
            {
                CloseHandle((long)phToken);
            }
            return true;
        }
        else
        {
            return false;
        }
    }


    [System.Runtime.InteropServices.DllImport("advapi32.dll", EntryPoint = "LogonUser", ExactSpelling = false, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
    public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, LogonType dwLogonType, LogonProvider dwLogonProvider, ref IntPtr phToken);

    [System.Runtime.InteropServices.DllImport("advapi32.dll", EntryPoint = "DuplicateToken", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
    public static extern bool DuplicateToken(IntPtr ExistingTokenHandle, Int16 SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

    [System.Runtime.InteropServices.DllImport("kernel32", EntryPoint = "CloseHandle", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
    public static extern long CloseHandle(long hObject);


    public enum LogonType : int
    {
        //This logon type is intended for users who will be interactively using the computer, such as a user being logged on 
        //by a terminal server, remote shell, or similar process.
        //This logon type has the additional expense of caching logon information for disconnected operations; 
        //therefore, it is inappropriate for some client/server applications,
        //such as a mail server.
        LOGON32_LOGON_INTERACTIVE = 2,

        //This logon type is intended for high performance servers to authenticate plaintext passwords.
        //The LogonUser function does not cache credentials for this logon type.
        LOGON32_LOGON_NETWORK = 3,

        //This logon type is intended for batch servers, where processes may be executing on behalf of a user without 
        //their direct intervention. This type is also for higher performance servers that process many plaintext
        //authentication attempts at a time, such as mail or Web servers. 
        //The LogonUser function does not cache credentials for this logon type.
        LOGON32_LOGON_BATCH = 4,

        //Indicates a service-type logon. The account provided must have the service privilege enabled. 
        LOGON32_LOGON_SERVICE = 5,

        //This logon type is for GINA DLLs that log on users who will be interactively using the computer. 
        //This logon type can generate a unique audit record that shows when the workstation was unlocked. 
        LOGON32_LOGON_UNLOCK = 7,

        //This logon type preserves the name and password in the authentication package, which allows the server to make 
        //connections to other network servers while impersonating the client. A server can accept plaintext credentials 
        //from a client, call LogonUser, verify that the user can access the system across the network, and still 
        //communicate with other servers.
        //NOTE: Windows NT:  This value is not supported. 
        LOGON32_LOGON_NETWORK_CLEARTEXT = 8,

        //This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
        //The new logon session has the same local identifier but uses different credentials for other network connections. 
        //NOTE: This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
        //NOTE: Windows NT:  This value is not supported. 
        LOGON32_LOGON_NEW_CREDENTIALS = 9
    }

    public enum LogonProvider : int
    {
        //Use the standard logon provider for the system. 
        //The default security provider is negotiate, unless you pass NULL for the domain name and the user name 
        //is not in UPN format. In this case, the default provider is NTLM. 
        //NOTE: Windows 2000/NT:   The default security provider is NTLM.
        LOGON32_PROVIDER_DEFAULT = 0
    }

    //public const int LOGON32_LOGON_INTERACTIVE = 2;
    //public const int LOGON32_PROVIDER_DEFAULT = 0;
    //static WindowsImpersonationContext impersonationContext;

    //[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    //public static extern int LogonUser(String lpszUserName, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

    //[DllImport("advapi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
    //public extern static int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);


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
  	
}
