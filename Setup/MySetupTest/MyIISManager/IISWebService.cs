using System;
using Microsoft.Win32;
using System.Diagnostics;
using System.DirectoryServices;

namespace MyIISManager
{

	/// <summary>
	/// IISWebService的控制类。
	/// WWW服务就是一个IISWebService，它下面包含多个WebServer（网站）。
	/// </summary>
	public class IISWebService
	{

		#region "静态的方法"

		/// <summary>
		/// 获取服务器IIS版本
		/// </summary>
		/// <param name="machine"></param>
		/// <returns></returns>
		public static IISVersionEnum GetIISVersion(string machine)
		{
			string path = "IIS://" + machine.ToUpper() + "/W3SVC/INFO";
			DirectoryEntry entry = null;
			try
			{
				entry = new DirectoryEntry(path);
			}
			catch
			{
				return IISVersionEnum.Unknown;
			}
			int num = 5;
			try
			{
				num = (int)entry.Properties["MajorIISVersionNumber"].Value;
			}
			catch
			{
                num = GetIISVersion();
                //return IISVersionEnum.IIS5;
			}
			switch (num)
			{
				case 6:
					return IISVersionEnum.IIS6;

				case 7:
					return IISVersionEnum.IIS7;
			}
			return IISVersionEnum.IIS6;
		}

        private static int GetIISVersion()
        {
            RegistryKey pregkey;

            pregkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\InetStp", true);

            return Convert.ToInt16(pregkey.GetValue("MajorVersion"));
        }

		/// <summary>
		/// 判断一个指定路径是否存在
		/// （由于DirectoryEntry.Exists(_adsPath)如果不存在返回异常，而不是false。
		/// 所以只能自己写个函数来判断。）
		/// </summary>
		/// <param name="adsPath"></param>
		/// <returns></returns>
		public static bool IsPathExists(string adsPath)
		{
			DirectoryEntry de = new DirectoryEntry(adsPath);
			try
			{
				string name = de.Name; // if this succeeds return true
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary> 
		/// 获取网站的标识符 
		/// </summary> 
		/// <param name="portNumber">端口号</param> 
		/// <returns></returns> 
		public static string GetIdentifier(string machine,string portNumber) 
		{ 
			DirectoryEntry iisservice = new DirectoryEntry(string.Format("IIS://{0}/W3SVC",machine)); 
			foreach(DirectoryEntry e in iisservice.Children) 
			{ 
				if(e.SchemaClassName == "IIsWebServer") 
				{ 
					foreach(object property in e.Properties["ServerBindings"]) 
					{ 
						if(property.Equals(":" + portNumber + ":")) 
						{ 
							return e.Name; 
						} 
					} 
				} 
			} 
	
			return ""; 
		}

        //注册ASP.NET版本
        public static void RegisterAspNet(string vDirPath, string aspNetVersion)
        {
            if (!string.IsNullOrEmpty(aspNetVersion))
            {
                string fileName = Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework\" + aspNetVersion + @"\aspnet_regiis.exe";
                ProcessStartInfo startInfo = new ProcessStartInfo(fileName);
                //处理目录路径
                string path = vDirPath.ToUpper();
                int index = path.IndexOf("W3SVC");
                path = path.Remove(0, index);
                //启动aspnet_iis.exe程序,刷新教本映射
                startInfo.Arguments = "-s " + path;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                string errors = process.StandardError.ReadToEnd();
                if (errors != string.Empty)
                {
                    throw new Exception(errors);
                }
            }
        }

		#endregion

		#region "私有的实例字段"
		private string _machine,_adsPath;
		private DirectoryEntry _svcEntry;
		#endregion

		#region "实例构造函数"

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="machine">IISWebService所在服务器的计算机名或者IP。本机可以使用"localhost"</param>
		/// <returns></returns>
		public IISWebService(string machine)
		{
			if(machine.ToString()=="")
			{
				_machine = "localhost";
			}
			else
			{
				_machine = machine;
			}

			Init();
		}
		#endregion

		#region "公共的实例属性"

		/// <summary>
		/// 计算机名或者IP。如果本机可以使用localhost。
		/// </summary>
		public string Machine
		{
			get {return _machine;}
		}

		/// <summary>
		/// 当前IISWebService的DirectoryEntry对象。
		/// </summary>
		public DirectoryEntry DirectoryEntry
		{
			get {return _svcEntry;}
		}

		/// <summary>
		/// 子集IISWebServer集合
		/// </summary>
		public IISWebServerCollection WebServers
		{
			get {return GetWebServers();}
		}

		#endregion

		#region "私有的方法"

		/// <summary>
		/// 初始化
		/// </summary>
		private void Init()
		{
			_adsPath=string.Format("IIS://{0}/W3SVC",_machine);
			_svcEntry=new DirectoryEntry(_adsPath);
		}


		/// <summary>
		/// 获取子集IISWebServer集合
		/// </summary>
		/// <returns></returns>
		private IISWebServerCollection GetWebServers()
		{
			IISWebServerCollection webServers = new IISWebServerCollection();
			IISWebServer webServer;

			foreach (DirectoryEntry de in _svcEntry.Children)
			{
				if (de.SchemaClassName=="IIsWebServer")
				{
					webServer=new IISWebServer(_machine,de.Properties["ServerBindings"][0].ToString().Replace(":",""));
					webServers.Add(webServer);
				}
			}

			return webServers;
		}

		#endregion

	}

}
