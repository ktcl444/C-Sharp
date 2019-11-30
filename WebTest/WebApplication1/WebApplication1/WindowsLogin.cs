using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;

namespace WebApplication1
{

        public enum LogonType : int
        {
            LOGON32_LOGON_INTERACTIVE = 2,
            LOGON32_LOGON_NETWORK = 3,
            LOGON32_LOGON_BATCH = 4,
            LOGON32_LOGON_SERVICE = 5,
            LOGON32_LOGON_UNLOCK = 7,
            LOGON32_LOGON_NETWORK_CLEARTEXT = 8,    // Only for Win2K or higher
            LOGON32_LOGON_NEW_CREDENTIALS = 9        // Only for Win2K or higher
        };

        public enum LogonProvider : int
        {
            LOGON32_PROVIDER_DEFAULT = 0,
            LOGON32_PROVIDER_WINNT35 = 1,
            LOGON32_PROVIDER_WINNT40 = 2,
            LOGON32_PROVIDER_WINNT50 = 3
        };

        class SecuUtil32
        {
            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
                int dwLogonType, int dwLogonProvider, ref IntPtr TokenHandle);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public extern static bool CloseHandle(IntPtr handle);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public extern static bool DuplicateToken(IntPtr ExistingTokenHandle,
                int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);
        }

        /// <summary>
        /// Summary description for NetworkSecurity.
        /// </summary>
        public class NetworkSecurity
        {
            public NetworkSecurity()
            {
                //
                // TODO: Add constructor logic here
                //
            }

            public static Hashtable LogonTypeEntry;
            public static Hashtable LogonProviderEntry;

            static NetworkSecurity()
            {
                LogonTypeEntry = new Hashtable();
                LogonTypeEntry.Add("LOGON32_LOGON_INTERACTIVE", LogonType.LOGON32_LOGON_INTERACTIVE);
                LogonTypeEntry.Add("LOGON32_LOGON_NETWORK", LogonType.LOGON32_LOGON_NETWORK);
                LogonTypeEntry.Add("LOGON32_LOGON_BATCH", LogonType.LOGON32_LOGON_BATCH);
                LogonTypeEntry.Add("LOGON32_LOGON_SERVICE", LogonType.LOGON32_LOGON_SERVICE);
                LogonTypeEntry.Add("LOGON32_LOGON_UNLOCK", LogonType.LOGON32_LOGON_UNLOCK);
                LogonTypeEntry.Add("LOGON32_LOGON_NETWORK_CLEARTEXT", LogonType.LOGON32_LOGON_NETWORK_CLEARTEXT);
                LogonTypeEntry.Add("LOGON32_LOGON_NEW_CREDENTIALS", LogonType.LOGON32_LOGON_NEW_CREDENTIALS);

                LogonProviderEntry = new Hashtable();
                LogonProviderEntry.Add("LOGON32_PROVIDER_DEFAULT", LogonProvider.LOGON32_PROVIDER_DEFAULT);
                LogonProviderEntry.Add("LOGON32_PROVIDER_WINNT35", LogonProvider.LOGON32_PROVIDER_WINNT35);
                LogonProviderEntry.Add("LOGON32_PROVIDER_WINNT40", LogonProvider.LOGON32_PROVIDER_WINNT40);
                LogonProviderEntry.Add("LOGON32_PROVIDER_WINNT50", LogonProvider.LOGON32_PROVIDER_WINNT50);
            }

            public const int SecurityImpersonation = 2;

            public static WindowsImpersonationContext ImpersonateUser(string strDomain,
                string strLogin,
                string strPwd,
                LogonType logonType,
                LogonProvider logonProvider)
            {
                IntPtr tokenHandle = new IntPtr(0);
                IntPtr dupeTokenHandle = new IntPtr(0);
                try
                {
                    tokenHandle = IntPtr.Zero;
                    dupeTokenHandle = IntPtr.Zero;

                    // Call LogonUser to obtain a handle to an access token.
                    bool returnValue = SecuUtil32.LogonUser(
                        strLogin,
                        strDomain,
                        strPwd,
                        (int)logonType,
                        (int)logonProvider,
                        ref tokenHandle);
                    if (false == returnValue)
                    {
                        int ret = Marshal.GetLastWin32Error();
                        string strErr = String.Format("LogonUser failed with error code : {0}", ret);
                        throw new ApplicationException(strErr, null);
                    }

                    bool retVal = SecuUtil32.DuplicateToken(tokenHandle, SecurityImpersonation, ref dupeTokenHandle);
                    if (false == retVal)
                    {
                        SecuUtil32.CloseHandle(tokenHandle);
                        throw new ApplicationException("Failed to duplicate token", null);
                    }

                    // The token that is passed to the following constructor must 
                    // be a primary token in order to use it for impersonation.
                    WindowsIdentity newId = new WindowsIdentity(dupeTokenHandle);
                    WindowsImpersonationContext impersonatedUser = newId.Impersonate();

                    return impersonatedUser;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }

                //return null;
            }

            public static WindowsImpersonationContext ImpersonateUser(string strDomain,
                string strLogin,
                string strPwd,
                string logonType,
                string logonProvider)
            {
                if (!LogonTypeEntry.ContainsKey(logonType) || !LogonProviderEntry.ContainsKey(logonProvider))
                {
                    return null;
                }
                return ImpersonateUser(strDomain, strLogin, strPwd, (LogonType)LogonTypeEntry[logonType], (LogonProvider)LogonProviderEntry[logonProvider]);
            }

            //登录接口
            public static bool LogonAndGetToken(string strDomain,
                string strLogin,
                string strPwd,
                LogonType logonType,
                LogonProvider logonProvider, ref IntPtr accessToken)
            {
                try
                {

                    IntPtr orgToken = IntPtr.Zero;
                    accessToken = IntPtr.Zero;

                    // Call LogonUser to obtain a handle to an access token.
                    bool returnValue = SecuUtil32.LogonUser(
                        strLogin,
                        strDomain,
                        strPwd,
                        (int)logonType,
                        (int)logonProvider,
                        ref orgToken);
                    if (false == returnValue)
                    {
                        int ret = Marshal.GetLastWin32Error();
                        string strErr = String.Format("LogonUser failed with error code : {0}", ret);
                        throw new ApplicationException(strErr, null);
                    }

                    bool retVal = SecuUtil32.DuplicateToken(orgToken, SecurityImpersonation, ref accessToken);
                    if (false == retVal)
                    {
                        SecuUtil32.CloseHandle(orgToken);
                        throw new ApplicationException("Failed to duplicate token", null);
                    }

                    /* TO TEST */
                    //SecuUtil32.CloseHandle(orgToken);
                    return true;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }

            public static bool LogonAndGetToken(string strDomain,
                string strLogin,
                string strPwd,
                string logonType,
                string logonProvider, ref IntPtr accessToken)
            {
                if (!LogonTypeEntry.ContainsKey(logonType) || !LogonProviderEntry.ContainsKey(logonProvider))
                {
                    return false;
                }

                return LogonAndGetToken(strDomain, strLogin, strPwd, (LogonType)LogonTypeEntry[logonType], (LogonProvider)LogonProviderEntry[logonProvider], ref accessToken);
            }

            public static WindowsImpersonationContext ImpersonateUser(IntPtr access_token)
            {
                try
                {
                    WindowsIdentity newId = new WindowsIdentity(access_token);
                    WindowsImpersonationContext impersonatedUser = newId.Impersonate();

                    return impersonatedUser;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }

            public static bool CloseToken(IntPtr token)
            {
                return SecuUtil32.CloseHandle(token);
            }
        }

    
}
