using System;
using Microsoft.Win32;
using System.Diagnostics;
using System.DirectoryServices;

namespace MyIISManager
{

	/// <summary>
	/// IISWebService�Ŀ����ࡣ
	/// WWW�������һ��IISWebService��������������WebServer����վ����
	/// </summary>
	public class IISWebService
	{

		#region "��̬�ķ���"

		/// <summary>
		/// ��ȡ������IIS�汾
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
		/// �ж�һ��ָ��·���Ƿ����
		/// ������DirectoryEntry.Exists(_adsPath)��������ڷ����쳣��������false��
		/// ����ֻ���Լ�д���������жϡ���
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
		/// ��ȡ��վ�ı�ʶ�� 
		/// </summary> 
		/// <param name="portNumber">�˿ں�</param> 
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

        //ע��ASP.NET�汾
        public static void RegisterAspNet(string vDirPath, string aspNetVersion)
        {
            if (!string.IsNullOrEmpty(aspNetVersion))
            {
                string fileName = Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework\" + aspNetVersion + @"\aspnet_regiis.exe";
                ProcessStartInfo startInfo = new ProcessStartInfo(fileName);
                //����Ŀ¼·��
                string path = vDirPath.ToUpper();
                int index = path.IndexOf("W3SVC");
                path = path.Remove(0, index);
                //����aspnet_iis.exe����,ˢ�½̱�ӳ��
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

		#region "˽�е�ʵ���ֶ�"
		private string _machine,_adsPath;
		private DirectoryEntry _svcEntry;
		#endregion

		#region "ʵ�����캯��"

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="machine">IISWebService���ڷ������ļ����������IP����������ʹ��"localhost"</param>
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

		#region "������ʵ������"

		/// <summary>
		/// �����������IP�������������ʹ��localhost��
		/// </summary>
		public string Machine
		{
			get {return _machine;}
		}

		/// <summary>
		/// ��ǰIISWebService��DirectoryEntry����
		/// </summary>
		public DirectoryEntry DirectoryEntry
		{
			get {return _svcEntry;}
		}

		/// <summary>
		/// �Ӽ�IISWebServer����
		/// </summary>
		public IISWebServerCollection WebServers
		{
			get {return GetWebServers();}
		}

		#endregion

		#region "˽�еķ���"

		/// <summary>
		/// ��ʼ��
		/// </summary>
		private void Init()
		{
			_adsPath=string.Format("IIS://{0}/W3SVC",_machine);
			_svcEntry=new DirectoryEntry(_adsPath);
		}


		/// <summary>
		/// ��ȡ�Ӽ�IISWebServer����
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
