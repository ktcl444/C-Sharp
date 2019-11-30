using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices;
using System.Management;
using System.Management.Instrumentation;
using System.IO;
using System.Timers;
using System.Runtime.InteropServices;
using System.Security.Principal;
namespace WebApplication1
{
    public partial class _Default : System.Web.UI.Page
    {
        public static System.Timers.Timer cleanTimer = null;


        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Mysoft.Map.Data.MyDB.GetDataItemInt("Select 1");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //int timerInterval = 1;
            //cleanTimer = new System.Timers.Timer(timerInterval * 60 * 1000);

            //cleanTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            //cleanTimer.Enabled = true;
            //cleanTimer.Start();

            //HttpContext.Current.Response.Clear();
            //HttpContext.Current.Response.ContentType = "multipart/x-mixed-replace;boundary=--TempString--";
            //HttpContext.Current.Response.StatusCode = 200;
            //HttpContext.Current.Response.Write("");
            //HttpContext.Current.Response.Write("--TempString--\r\n");
            //HttpContext.Current.Response.Flush();
            //while (HttpContext.Current.Response.IsClientConnected)
            //{
            //    HttpContext.Current.Response.Clear();
            //    HttpContext.Current.Response.Write("date : " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "\r\n");
            //    HttpContext.Current.Response.Write("--TempString--\r\n");
            //    HttpContext.Current.Response.Flush();
            //    Thread.Sleep(1000);
            //}
            //Literal1.Text = "<script> window.open('/Index.aspx', '_blank');</script>";
            //Response.Write("Test");
            //string test = string.Empty;
            //if (test == "")
            //    Response.Write("True");
            //else
            //    Response.Write("False");

            //HttpCookie cookie = new HttpCookie("Test");
            //cookie["Name"] = "1";
            //Response.Cookies.Add(cookie);

            //if (Request.Cookies["Test"] == null)
            //{
            //    Response.Write("Can't find cookie");
            //}
            //else
            //{
            //    Response.Write("Find cookie");
            //}
            //HttpCookie cookie2 = HttpContext.Current.Request.Cookies["Test"];
           
            //    cookie2.Expires = DateTime.Now.AddYears(-1);
            //    HttpContext.Current.Response.Cookies.Add(cookie2);
            //    Response.Write("Delete cookie");

            //    if (Request.Cookies["Test"] == null)
            //    {
            //        Response.Write("Can't find cookie");
            //    }
            //    else
            //    {
            //        Response.Write("Find cookie");
            //    }
            //test();
            //ShowSessionID();
        }


        private void ShowSessionID()
        {
            txtSessionID.Text = HttpContext.Current.Session.SessionID.ToString();
        }

        private void TestCreateDirectory()
        {
            String XmlTempPath = "/XmlTemp/AppGridEx/";
            string strFilePath = Page.Server.MapPath(XmlTempPath) + HttpContext.Current.Session.SessionID + "\\";
            string strFileName = Guid.NewGuid().ToString() + ".xml";
            string time = DateTime.Now.Ticks.ToString();

            if (Directory.Exists(strFilePath) == false)
            {
                Directory.CreateDirectory(strFilePath);
            }
        }

        private void test()
        {
            ManagementObjectCollection managementObjectCollection = CollectServerInfoByWMI("MicrosoftIISv2", "IIsWebServerSetting");
            foreach (ManagementObject managementObject in managementObjectCollection)
            {
        
            }
            string result = string.Empty;
            foreach (DirectoryEntry Site in new DirectoryEntry("IIS://localhost/w3svc").Children)
            {
                if (String.Compare(Site.SchemaClassName, "IIsWebServer", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    result += Site.Name + Site.Properties["LogFileDirectory"].ToString() + Environment.NewLine;
                }
            }
            Response.Write(result);
        }

        /// <summary>
        /// 通过WMI采集服务器信息
        /// </summary>
        /// <param name="spaceName">空间名</param>
        /// <param name="className">类名</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static ManagementObjectCollection CollectServerInfoByWMI(string spaceName, string className)
        {
            string queryString = "select * from " + className;
            SelectQuery selectQuery = new SelectQuery(queryString);
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(selectQuery);
            managementObjectSearcher.Scope = GetScope(spaceName);
            return managementObjectSearcher.Get();
        }


        /// <summary>
        /// 获得命名空间范围
        /// </summary>
        /// <param name="spaceName">命名空间名称</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static ManagementScope GetScope(string spaceName)
        {
            ConnectionOptions connectionOptions = new ConnectionOptions();
            connectionOptions.EnablePrivileges = true;
            ManagementScope scope = new ManagementScope("\\\\.\\root\\" + spaceName, connectionOptions);
            return scope;
        }

        protected void Button9_Click(object sender, EventArgs e)
        {
            string s = this.TextBox2.Text.Trim();
            this.TextBox3.Text = GetSplitString(s);
        }

        private string GetSplitString(string s)
        {
            return s.Substring(0, s.LastIndexOf(";"));
        }

        protected void btnCheckExp_Click(object sender, EventArgs e)
        {
            string docName = txtDocName.Text.Trim();
          

          
            string expName = System.IO.Path.GetExtension(docName);
            txtExpName.Text = expName;
            if (docName.LastIndexOf(expName) > -1)
            {
                txtNewDocName.Text = docName.Substring(0, docName.LastIndexOf(expName));

            }
            //string expName = txtExpName.Text.Trim();
            //txtNewDocName.Text = CheckExp(docName, expName);
        }

        private string CheckExp(string docName, string expName)
        {
            string returnValue = string.Empty;
            int expLength = expName.Length;
            int expIndex = docName.LastIndexOf(expName);
            if (expIndex > 0)
            {
                if ((expIndex + expLength) == docName.Length)
                {
                    return docName;
                }
            }
                return docName + expName;
        }

        protected void btnTestDeleteDirectory_Click(object sender, EventArgs e)
        {
            String XmlTempPath = "/XmlTemp/AppGridEx/";
            string strFilePath = Page.Server.MapPath(XmlTempPath);
            Directory.Delete(strFilePath,true);
            ShowSessionID();
        }

        protected void btnTestCreateDirectory_Click(object sender, EventArgs e)
        {

            TestCreateDirectory();
            ShowSessionID();
        }

        protected void btnExcelPermission_Click(object sender, EventArgs e)
        {         
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;

            //在程序中模拟域帐户登录
            if (WinLogonHelper.LogonUser(TextBox5.Text, TextBox4.Text, TextBox6.Text, 9, 0, ref admin_token) != 0)
            {
                using (wid_admin = new WindowsIdentity(admin_token))
                {
                    using (wic = wid_admin.Impersonate())
                    {
                        string filePath = this.txtFilePath.Text;
                        string newFilePath = this.txtNewFilePath.Text;
                        string userName = this.txtUserName.Text;
                        try
                        {
                            ExcelPermission2.SetExcelPermission(filePath, newFilePath, userName);
                        }
                        catch (Exception ex)
                        {
                            this.txtExcelPermissionResult.Text = ex.Message;
                        }
                    }
                }
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

        private void WindowsLogin(string domain,string user,string password,LogonType l,LogonProvider p)
        {          

            IntPtr accessToken = new IntPtr(0);
            if (NetworkSecurity.LogonAndGetToken(domain,
                user, password,
               l, p,
                ref accessToken) == false)
            {
                throw new Exception("Logon Failed");
            }
        }

        protected void btnExcelPermission0_Click(object sender, EventArgs e)
        {
            WindowsLogin(TextBox4.Text, TextBox5.Text, TextBox6.Text, (LogonType)Enum.Parse(typeof(LogonType), TextBox7.Text), (LogonProvider)Enum.Parse(typeof(LogonProvider), TextBox8.Text));
        }

        protected void btnCalBrokerage_Click(object sender, EventArgs e)
        {
            var amount = this.txtAmount.Text;
            var rate = this.txtRate.Text;
            var lowerLimit = this.txtLowerLimit.Text;
            var higherLimit = this.txtHigherLimit.Text;
            var rate1 = this.txtRate1.Text;
            var rate2 = this.txtRate2.Text;
            var rate3 = this.txtRate3.Text;

            var brokerage = Convert.ToDecimal(amount) * Convert.ToDecimal(rate) / 1000;
            var brokerage1 = brokerage * Convert.ToDecimal(rate1) / 100;
            var brokerage2= brokerage * Convert.ToDecimal(rate2) / 100;
            var brokerage3 = brokerage * Convert.ToDecimal(rate3) / 100;

            this.txtBrokerage.Text = brokerage.ToString();
            this.txtBrokerage1.Text = brokerage1.ToString();
            this.txtBrokerage2.Text = brokerage2.ToString();
            this.txtBrokerage3.Text = brokerage3.ToString();
        }

        protected void Button11_Click(object sender, EventArgs e)
        {
            var emptyDate = "0000-00-00 00:00:00";
            DateTime dt;
            if(DateTime.TryParse(emptyDate, out dt))
            {
                var s = dt.ToString();
            }
        }
    }
}
