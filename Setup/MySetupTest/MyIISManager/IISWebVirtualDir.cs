using System;
using System.Text;
using System.DirectoryServices;
using System.Diagnostics;

namespace MyIISManager
{

	/// <summary>
	/// IISWebVirtualDir虚拟目录类。
	/// 必须要注意：ADsPath 是区分大小写的，因为它是从MetaBase.xml中解析。
	/// </summary>
	public class IISWebVirtualDir
	{
		#region "私有的实例字段"
		private string _machine,_identifier,_adsPath,_port,_appPoolId,_defaultDoc,
			_path,_name,_dir,_appFriendlyName,_aspNetVersion;
		private int _appIsolated;
		private bool _enableDefaultDoc,_enableDirBrowsing,_accessRead,_accessScript,_isApplication;
		private IISWebVirtualDir _parent;
		private DirectoryEntry _dirEntry;
		#endregion

		#region "实例构造函数"
        public IISWebVirtualDir(string machine, string port, string dir) : this(machine, port, dir, string.Empty)
        {
        }
        public IISWebVirtualDir(string machine, string port, string dir, string identifier)
		{
			_machine = machine;
			_port = port;
			_dir = dir;
            _identifier = identifier;
			Init();
		}
		#endregion

		#region "公共的实例属性"

		/// <summary>
		//父级虚拟目录对象
		/// </summary>
		public IISWebVirtualDir Parent
		{
			get{ return _parent;}
		}

//		/// <summary>
//		//MachineName属性定义访问机器的名字，可以是IP或计算名
//		/// </summary>
//		public string MachineName
//		{
//			get{ return _machine;}
//			set{ _machine = value;}
//		}

		/// <summary>
		// 虚拟目录名称
		/// </summary>
		public string Name
		{
			get{ return _name; }
			set{ _name = value; }
		}

		/// <summary>
		// 物理路径
		/// </summary>
		public string Path
		{
			get{ return _path; }
			set{ _path = value; }
		}


//		/// <summary>
//		/// 网站的端口号
//		/// </summary>
//		public string Port
//		{
//			get{ return _port; }
//			set{ _port = value; }
//		}

//		/// <summary>
//		/// 网站的标识符
//		/// </summary>
//		public string Identifier
//		{
//			get{ return _identifier; }
//			set{ _identifier = value; }
//		}


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
		/// 是否应用程序
		/// </summary>
		public bool IsApplication
		{
			get{ return _isApplication; }
			set{ _isApplication = value; }
		}

		/// <summary>
		/// 虚拟目录的DirectoryEntry对象(Root)
		/// </summary>
		public DirectoryEntry DirectoryEntry
		{
			get {return _dirEntry;}
		}

		//虚拟目录子集
		public IISWebVirtualDirCollection WebVirtualDirs
		{
			get{ return GetVirDirs(); }
		}

        //ASP.NET版本
        public string AspNetVersion
        {
            get
            {
                return _aspNetVersion;
             }
            set
            {
                _aspNetVersion = value;
            }
        }
		#endregion

		#region "公共的实例方法"

		//创建一个虚拟目录
		public bool Create()
		{
			//父级节点是否存在
			string parenADsPath = _adsPath.Substring(0,_adsPath.LastIndexOf("/"));
			if (!IISWebService.IsPathExists(parenADsPath))
			{
				throw(new Exception(string.Format("父级目录“{0}”不存在，创建失败！",parenADsPath)));
			}

			//要创建的目录是否已经存在
			DirectoryEntry parentEntry =new DirectoryEntry(parenADsPath);
            bool blnExistEntry = false;
			foreach (DirectoryEntry de in parentEntry.Children)
			{
				if (de.SchemaClassName=="IIsWebVirtualDir" && de.Name==_name)
				{
                    _dirEntry = de;
                    blnExistEntry = true;
                    break;
                    //throw(new Exception(string.Format("名称为“{0}”的虚拟目录已经存在，创建失败！",_name)));
				}
			}
            if (!blnExistEntry)
            {
                _dirEntry = parentEntry.Children.Add(_name, "IIsWebVirtualDir");
            }

			//是否定义为应用程序
			if(this._isApplication)
			{
				//为虚拟目录创建应用程序定义(不同IIS版本参数不同)
				IISVersionEnum iisVersion = IISWebService.GetIISVersion(_machine);

				if(iisVersion==IISVersionEnum.IIS5 || iisVersion==IISVersionEnum.IIS6 || iisVersion==IISVersionEnum.IIS7)
				{	
					//进程内运行(0)、进程外运行(1)、进程外缓冲池中运行(2)
					_dirEntry.Invoke("AppCreate2",Convert.ToInt32(this._appIsolated));
				}
				else
				{
					//进程内运行(TRUE) 、进程外运行(FALSE) 
					_dirEntry.Invoke("AppCreate",Convert.ToInt32(this._appIsolated));
				}
			}

            Update();
            if (!blnExistEntry)
            {
                parentEntry.CommitChanges();
            }

			//然后更新数据
            //Update();
            IISWebService.RegisterAspNet(_dirEntry.Path, _aspNetVersion);
			return true;
		}


		//删除一个虚拟目录
		public void Delete()
		{
			DirectoryEntry parentEntry = this._parent.DirectoryEntry;


			if(IISWebService.IsPathExists(this._adsPath))
			{
				DirectoryEntry dirEntry = new DirectoryEntry(this._adsPath);

				//方法一：使用根节点的删除方法
				object[] paras = new object[2];
				paras[0]	= "IIsWebVirtualDir";		//表示操作的是虚拟目录
				paras[1]	= _dir;
				parentEntry.Invoke("Delete",paras);
				parentEntry.CommitChanges();

				/*
				//方法二：也可以使用节点的自身删除方法（这种方法有问题，需要研究）
				DirectoryEntry virDir=_rootDirEntry.Children.Find(dir,"IIsWebVirtualDir");
				virDir.Invoke("AppDelete",true); 
				_rootDirEntry.CommitChanges(); 
				*/
			}
			else
			{
				throw new Exception(string.Format("目录 http://{0}:{1}/{2} 不存在，删除失败！",this._machine,this._port,this._dir));
			}
		}


        public bool SetMime(string extension, string mimeType)
        {
        //IIS://LocalHost/MimeMap
            //DirectoryEntry _dirEntry = GetIIsWebVirtualDir(virtualName);
            if (_dirEntry == null)
            {
                return false;
            }
            PropertyValueCollection mime = _dirEntry.Properties["MimeMap"];

            foreach (object value in mime)
            {
                IISOle.IISMimeType mimeTypeObj = (IISOle.IISMimeType)value;
                if (extension == mimeTypeObj.Extension)
                {
                    mime.Remove(value);
                    _dirEntry.CommitChanges();
                    break;
                }
            }
            IISOle.MimeMapClass newMime = new IISOle.MimeMapClass();
            newMime.Extension = extension;
            newMime.MimeType = mimeType;
            try
            {
                mime.Add(newMime);
                _dirEntry.CommitChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

		//更新虚拟目录配置
		public void Update()
		{
			_dirEntry.Properties["AppPoolId"][0]=_appPoolId;
			_dirEntry.Properties["AccessScript"][0]=_accessScript;
			_dirEntry.Properties["AccessRead"][0]=_accessRead;
			_dirEntry.Properties["DefaultDoc"][0]=_defaultDoc;
			_dirEntry.Properties["EnableDefaultDoc"][0]=_enableDefaultDoc;
			_dirEntry.Properties["EnableDirBrowsing"][0]=_enableDirBrowsing;
			_dirEntry.Properties["AppFriendlyName"][0]=_appFriendlyName; 
			_dirEntry.Properties["Path"][0]=_path;
			_dirEntry.CommitChanges();
		}

		#endregion

		#region "私有方法"
		//初始化
		private void Init()
		{
			try
			{
                if (string.IsNullOrEmpty(_identifier))
                {
                    this._identifier = IISWebService.GetIdentifier(this._machine, this._port);
                }

				if (_dir=="Root")
				{
					//如果是根节点Root，需要特殊处理。但如果虚拟目录为名就为Root呢？
					this._adsPath=string.Format("IIS://{0}/W3SVC/{1}/Root",_machine,_identifier);
				}
				else
				{
					this._adsPath=string.Format("IIS://{0}/W3SVC/{1}/Root/{2}",_machine,_identifier,_dir);
				}

				//获取父级节点
				if(IISWebService.IsPathExists(_adsPath.Substring(0,_adsPath.LastIndexOf("/"))))
				{
					if (_dir.IndexOf("/")>0)
					{
						this._parent=new IISWebVirtualDir(_machine,_port,_dir.Substring(0,_dir.LastIndexOf("/")));
					}
					else if(_dir=="Root")
					{
						this._parent=null;
					}
					else
					{
						this._parent=new IISWebVirtualDir(_machine,_port,"Root");
					}
				}
				else
				{
					throw new Exception(string.Format("http://{0}:{1}/{2}父级路径不存在！",this._machine,this._port,_dir));
				}

				//获取配置信息
				string[] arrDir = _dir.Split(new char[]{'/'});
				this._name = arrDir[arrDir.Length-1];

				if (IISWebService.IsPathExists(_adsPath))
				{
					_dirEntry=new DirectoryEntry(_adsPath);

					this._appPoolId	= (string)_dirEntry.Properties["AppPoolId"][0];
					this._accessScript=(bool)_dirEntry.Properties["AccessScript"][0];
					this._accessRead=(bool)_dirEntry.Properties["AccessRead"][0];
					this._defaultDoc	= (string)_dirEntry.Properties["DefaultDoc"][0];
					this._enableDefaultDoc	= (bool)_dirEntry.Properties["EnableDefaultDoc"][0];
					this._enableDirBrowsing	= (bool)_dirEntry.Properties["EnableDirBrowsing"][0];
					this._appFriendlyName	= (string)_dirEntry.Properties["AppFriendlyName"][0];
					this._appIsolated	= (int)_dirEntry.Properties["AppIsolated"][0];
					this._isApplication=(_appFriendlyName=="")?false:true;

					this._path=(string)_dirEntry.Properties["Path"][0];
				}
				else
				{
					this._appPoolId	= "DefaultAppPool";
					this._accessScript=true;
					this._accessRead=true;
					this._defaultDoc	= "Default.htm,Default.asp,index.htm,iisstart.htm,Default.aspx,index.aspx";
					this._enableDefaultDoc	= true;
					this._enableDirBrowsing	= false;
					this._appFriendlyName	= "默认应用程序";
					this._appIsolated	= 2;

					this._path = "";
				}

			} 
			catch(Exception e)
			{
				throw new Exception("无法连接到 http://"+ _machine +":"+_port+"/"+_dir+" ！",e);
			}
		}


		//获取当前目录下所有的直接下级虚拟目录
		private IISWebVirtualDirCollection GetVirDirs()
		{
			IISWebVirtualDirCollection webVirDirs = new IISWebVirtualDirCollection();
			IISWebVirtualDir webVirDir;

			foreach (DirectoryEntry de in _dirEntry.Children)
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


