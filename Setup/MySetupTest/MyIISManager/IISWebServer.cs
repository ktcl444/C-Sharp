using System;
using System.Collections;
using System.Text;
using System.DirectoryServices;
using System.Reflection;


namespace MyIISManager
{

	/// <summary>
	/// IISWebServer的控制类。IIS中每个网站都是一个WebServer。
	/// 必须要注意：ADsPath 是区分大小写的，因为它是从MetaBase.xml中解析。
	/// </summary>
	public class IISWebServer : IIISWebServer
	{

		#region "私有的实例字段"
		private string _machine,_identifier,_adsPath,_port,_appPoolId,_defaultDoc,_siteName,
			_path,_appFriendlyName;
		private int _appIsolated;
		private bool _enableDefaultDoc,_enableDirBrowsing,_autoStart,_accessRead,_accessScript;
		private IISWebService _parent;
		protected DirectoryEntry _serverEntry,_rootDirEntry;
		#endregion

		#region "实例构造函数"
        public IISWebServer()
        { 
        
        }

        public IISWebServer(string machine, string port)
        {
            _machine = machine;
            _port = port;
            Init(machine,"",port,"","",2,"");
        }
		#endregion

		#region "公共的实例属性"

		/// <summary>
		/// 父级webservice对象
		/// </summary>
		public IISWebService Parent
		{
			get{ return _parent;}
		}


		/// <summary>
		/// 计算机名或者IP。如果本机可以使用localhost。
		/// </summary>
		public string Machine
		{
			get{ return _machine;}
		}


		/// <summary>
		/// 网站名称
		/// </summary>
		public string SiteName
		{
			get{ return _siteName; }
			set{ _siteName = value; }
		}


		/// <summary>
		/// 端口号
		/// </summary>
		public string Port
		{
			get{ return _port; }
			set{ _port = value; }
		}


		/// <summary>
		/// 物理路径
		/// </summary>
		public string Path
		{
			get{ return _path; }
			set{ _path = value; }
		}


		/// <summary>
		/// 网站标识符
		/// </summary>
		public string Identifier
		{
			get{ return _identifier; }
			set{ _identifier = value; }
		}


//		/// <summary>
//		/// ADsPath路径
//		/// </summary>
//		public string ADsPath
//		{
//			get{ return _adsPath; }
//		}

		/// <summary>
		/// 应用程序池Id
		/// </summary>
		public string AppPoolId
		{
			get{ return _appPoolId; }
			set{ _appPoolId = value; }
		}

		/// <summary>
		/// 读取权限
		/// </summary>
		public bool AccessRead
		{
			get{ return _accessRead; }
			set{ _accessRead = value; }
		}

		/// <summary>
		/// 脚本支持
		/// </summary>
		public bool AccessScript
		{
			get{ return _accessScript; }
			set{ _accessScript = value; }
		}

		/// <summary>
		/// 默认文档
		/// 格式为："Default.htm,Default.asp,index.htm,Default.aspx"
		/// </summary>
		public string DefaultDoc
		{
			get{ return _defaultDoc; }
			set{ _defaultDoc = value; }
		}

		/// <summary>
		/// 使用默认文档
		/// </summary>
		public bool EnableDefaultDoc
		{
			get{ return _enableDefaultDoc; }
			set{ _enableDefaultDoc = value; }
		}

		/// <summary>
		/// 目录浏览
		/// </summary>
		public bool EnableDirBrowsing
		{
			get{ return _enableDirBrowsing; }
			set{ _enableDirBrowsing = value; }
		}


		/// <summary>
		/// Web 应用程序名称
		/// </summary>
		public string AppFriendlyName
		{
			get{ return _appFriendlyName; }
			set{ _appFriendlyName = value; }
		}


		/// <summary>
		/// Web 应用程序的进程类型
		/// IIS5及以上版本（使用AppCreate2()）： 进程内运行 (0)、进程外运行 (1)、进程池中(2)	
		/// IIS5以下版本（使用AppCreate()）：进程内 (1) 、进程外 (0)	
		/// </summary>
		public int AppIsolated
		{
			get{ return _appIsolated; }
			set{ _appIsolated = value; }
		}

		/// <summary>
		/// 自启动
		/// </summary>
		public bool AutoStart
		{
			get{ return _autoStart; }
			set{ _autoStart = value; }
		}


		/// <summary>
		/// 网站的DirectoryEntry对象(Root)
		/// </summary>
		public DirectoryEntry DirectoryEntry
		{
			get {return _rootDirEntry;}
		}

		/// <summary>
		/// 虚拟目录子集
		/// </summary>
		public IISWebVirtualDirCollection WebVirtualDirs
		{
			get{ return GetVirDirs(); }
		}

		#endregion

		#region "公共的实例方法"

		//创建网站
		public bool Create()
		{
			if (_port.ToString()=="" || _siteName.ToString()=="" || _path.ToString()=="")
			{
				throw(new Exception("网站名称、端口号、物理路径不允许为空！"));
			}
            CreateAppPool(AppPoolId);
			int identifier=1;

			//校验不允许同名或者同端口，同时获取最大的标识符号
			foreach (IISWebServer server in this._parent.WebServers)
			{
                if (server.SiteName == _siteName)
                {
                    throw (new Exception(string.Format("名称为“{0}”的网站已经存在，创建失败！", _siteName)));
                }
				if (server.Port==_port)
				{
                    //throw(new Exception(string.Format("端口“{0}”上已经存在网站，创建失败！",_port)));
                    this._parent.DirectoryEntry.Children.Find(server.Identifier, "IIsWebServer").Invoke("Stop", null);
				}
				if (Convert.ToInt32(server.Identifier) > identifier)
					identifier = Convert.ToInt32(server.Identifier);
			}
			_identifier=(identifier+1).ToString();
			_adsPath=string.Format("IIS://{0}/W3SVC/{1}",_machine,_identifier);

			//创建webserver
			DirectoryEntry svcEntry = this._parent.DirectoryEntry;
			_serverEntry = svcEntry.Children.Add(_identifier,"IIsWebServer");

			_serverEntry.Properties["ServerComment"][0]=_siteName;
			_serverEntry.Properties["ServerBindings"].Add(":" + _port + ":");
			_serverEntry.Properties["ServerAutoStart"][0]=_autoStart;

			_serverEntry.CommitChanges();
			svcEntry.CommitChanges();

			//创建Root目录
			_rootDirEntry=new DirectoryEntry(_adsPath+"/Root");
            IISWebVirtualDir virDir = new IISWebVirtualDir(_machine, _port, "Root", _identifier);
			virDir.Path = this._path;
            virDir.AppFriendlyName = this._appFriendlyName;
			virDir.AppIsolated=this._appIsolated;
            virDir.IsApplication = !string.IsNullOrEmpty(_appFriendlyName);
            virDir.AppPoolId = _appPoolId;
            virDir.AspNetVersion = "v2.0.50727";
			virDir.Create();
    
            SetMime(virDir.DirectoryEntry, "ini", "application/octet-stream");
			return true;
		}


        public void SetMime(DirectoryEntry entry, string extension, string mimeType)
        {
            if (entry == null)
            {
                return;
            }
            PropertyValueCollection mime = entry.Properties["MimeMap"];

            foreach (object value in mime)
            {
                IISOle.IISMimeType mimeTypeObj = (IISOle.IISMimeType)value;
                if (extension == mimeTypeObj.Extension)
                {
                    mime.Remove(value);
                    entry.CommitChanges();
                    break;
                }
            }
            IISOle.MimeMapClass newMime = new IISOle.MimeMapClass();
            newMime.Extension = extension;
            newMime.MimeType = mimeType;

            mime.Add(newMime);
            entry.CommitChanges();
        }

		//删除网站
		public void Delete()
		{
			DirectoryEntry svcEntry = this._parent.DirectoryEntry;

			if(IISWebService.IsPathExists(this._adsPath))
			{
				DirectoryEntry serverEntry = new DirectoryEntry(this._adsPath);

				svcEntry.Children.Remove(serverEntry);
				svcEntry.CommitChanges();
			}
			else
			{
				throw new Exception(string.Format("网站 http://{0}:{1} 不存在，删除失败！",this._machine,this._port));
			}
		}

		//更新当前网站配置
		public bool Update()
		{
			_serverEntry.Properties["ServerComment"][0]=_siteName;
			_serverEntry.Properties["ServerBindings"].Add(":" + _port + ":");
			_serverEntry.Properties["ServerAutoStart"][0]=_autoStart;

			_rootDirEntry.Properties["AppPoolId"][0]=_appPoolId;
			_rootDirEntry.Properties["AccessScript"][0]=_accessScript;
			_rootDirEntry.Properties["AccessRead"][0]=_accessRead;
			_rootDirEntry.Properties["DefaultDoc"][0]=_defaultDoc;
			_rootDirEntry.Properties["EnableDefaultDoc"][0]=_enableDefaultDoc;
			_rootDirEntry.Properties["EnableDirBrowsing"][0]=_enableDirBrowsing;
			_rootDirEntry.Properties["AppFriendlyName"][0]=_appFriendlyName; 

			_rootDirEntry.Properties["Path"][0]=_path;

			_rootDirEntry.CommitChanges();
			_serverEntry.CommitChanges();

			return true;
		}


		//停止当前网站
		public void Stop()
		{
			_serverEntry.Invoke("Stop",null);
		}


		//启动当前网站
		public void Start()
		{
			_serverEntry.Invoke("Start",null);
		}


		//暂停当前网站
		public void Pause()
		{
			_serverEntry.Invoke("Pause",null);
		}


		//取消暂停
		public void Continue()
		{
			_serverEntry.Invoke("Continue",null);
		}

		//销毁ADSI对象
		public void Dispose()
		{
			_rootDirEntry.Dispose();
			_serverEntry.Dispose();
		}

		//获取网站的当前状态
		//1 正在启动 
		//2 已启动 
		//3 正在停止 
		//4 已停止 
		//5 正在暂停 
		//6 已暂停 
		//7 正在继续 
		public IISServerState GetStatus()
		{
			//Status实际不是方法，而是属性，所以不能使用下行
			//return (int)_serverEntry.Invoke("Status",null);

			//获取ADSI对象的属性
			Object ads = _serverEntry.NativeObject; 
			Type type = ads.GetType();			//typeof(DirectoryEntry.NativeObject);
			int status  = (int)type.InvokeMember("Status", BindingFlags.GetProperty,null, ads, null);

			IISServerState e;
			switch (status)
			{
				case 1:
					e=IISServerState.Starting;
					break;
				case 2:
					e=IISServerState.Started;
					break;
				case 3:
					e=IISServerState.Stopping;
					break;
				case 4:
					e=IISServerState.Stopped;
					break;
				case 5:
					e=IISServerState.Pausing;
					break;
				case 6:
					e=IISServerState.Paused;
					break;
				case 7:
					e=IISServerState.Continuing;
					break;
				default:
					e=IISServerState.Unkonwn;
					break;
			}
			return e;
		}

        //初始化
        public void Init(string strMachine,string strWebName, string strWebPort, string strWebDir, string strAppFriendlyName, int appIsolated, string strAppPool)
        {
            try
            {
                this._port = (string.IsNullOrEmpty(strWebPort) ? "" : strWebPort);
                this._machine = (string.IsNullOrEmpty(strMachine) ? "" : strMachine);
                //获取父级节点
                this._parent = new IISWebService(this._machine);

                //获取配置信息
                this._identifier = IISWebService.GetIdentifier(_machine, _port);
                if (this._identifier != "" && string.IsNullOrEmpty(strWebName))
                {
                    this._adsPath = string.Format("IIS://{0}/W3SVC/{1}", _machine, _identifier);

                    //webserver配置信息
                    _serverEntry = new DirectoryEntry(_adsPath);
                    this._siteName = (string)_serverEntry.Properties["ServerComment"][0];
                    this._port = _serverEntry.Properties["ServerBindings"][0].ToString().Replace(":", "");
                    this._autoStart = (bool)_serverEntry.Properties["ServerAutoStart"][0];

                    //webserver root配置信息(属性返回object类型，必须进行显示转换。)
                    if (IISWebService.IsPathExists(_adsPath + "/Root"))
                    {
                        _rootDirEntry = new DirectoryEntry(_adsPath + "/Root");
                        this._appPoolId = (string)_rootDirEntry.Properties["AppPoolId"][0];
                        this._accessScript = (bool)_rootDirEntry.Properties["AccessScript"][0];
                        this._accessRead = (bool)_rootDirEntry.Properties["AccessRead"][0];
                        this._defaultDoc = (string)_rootDirEntry.Properties["DefaultDoc"][0];
                        this._enableDefaultDoc = (bool)_rootDirEntry.Properties["EnableDefaultDoc"][0];
                        this._enableDirBrowsing = (bool)_rootDirEntry.Properties["EnableDirBrowsing"][0];
                        this._appFriendlyName = (string)_rootDirEntry.Properties["AppFriendlyName"][0];
                        this._appIsolated = (int)_rootDirEntry.Properties["AppIsolated"][0];

                        this._path = (string)_rootDirEntry.Properties["Path"][0];
                    }
                }
                else
                {
                    this._appPoolId = (string.IsNullOrEmpty(strAppPool) ? "DefaultAppPool" : strAppPool);
                    this._accessScript = true;
                    this._accessRead = true;
                    this._defaultDoc = "Default.htm,Default.asp,index.htm,iisstart.htm,Default.aspx,index.aspx";
                    this._enableDefaultDoc = true;
                    this._enableDirBrowsing = false;
                    this._appFriendlyName = (string.IsNullOrEmpty(strAppFriendlyName) ? "默认应用程序" : strAppFriendlyName);
                    this._appIsolated = appIsolated;
                    this._path = (string.IsNullOrEmpty(strWebDir) ? "" : strWebDir);
                    this._siteName = (string.IsNullOrEmpty(strWebName) ? "" : strWebName);
                }
            }
            catch (Exception e)
            {
                throw new Exception("无法连接到网站 http://" + _machine + ":" + _port + " ！", e);
            }
        }


        public bool CheckPortRepeated()
        {
            foreach (IISWebServer server in this._parent.WebServers)
            {
                if (server.Port == _port)
                {
                    return true;
                }
            }
            return false;
        }
		#endregion

		#region "私有方法"

        /// <summary>
        /// 创建应用程序池
        /// </summary>
        /// <param name="strAppPoolName">应用程序池名称</param>
        private void CreateAppPool(string strAppPoolName)
        {
            try
            {
                bool blnExistAppPool = false;
                DirectoryEntry appPoolRoot = new System.DirectoryServices.DirectoryEntry(@"IIS://localhost/W3SVC/AppPools");
                foreach (DirectoryEntry appPool in appPoolRoot.Children)
                {
                    if (appPool.Name == strAppPoolName)
                    {
                        blnExistAppPool = true;
                        break;
                    }
                }
                if (!blnExistAppPool)
                {
                    DirectoryEntry newAppPool = appPoolRoot.Children.Add(strAppPoolName, "IIsApplicationPool");
                    // 0 = Local System（本地系统）
                    // 1 = Local Service（本地服务）
                    // 2 = Network Service（网络服务）
                    // 3 = Custom Identity（自定义配置）需要设置 WAMUserName 和 WAMUserPass 属性

                    newAppPool.Properties["AppPoolIdentityType"][0] = 2;
                    //newAppPool.Properties["WAMUserName"][0] = @"VISTA\1"; //domain\用户，注意：此用户必须在IIS_WPG组中
                    //newAppPool.Properties["WAMUserPass"][0] = "1";
                    //其它属性类似，如设置Web园数目：
                    //newAppPool.Properties["MaxProcesses"][0] = 5;
                    newAppPool.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		//获取根目录下所有的直接下级虚拟目录
		private IISWebVirtualDirCollection GetVirDirs()
		{
			IISWebVirtualDirCollection webVirDirs = new IISWebVirtualDirCollection();
			IISWebVirtualDir webVirDir;

			foreach (DirectoryEntry de in _rootDirEntry.Children)
			{
				if (de.SchemaClassName=="IIsWebVirtualDir")
				{
					webVirDir=new IISWebVirtualDir(_machine,_port,de.Name);
					webVirDirs.Add(webVirDir);
				}
			}

			return webVirDirs;
		}


		#endregion

    }

}
