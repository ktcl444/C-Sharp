using System;
using System.Text;
using System.DirectoryServices;
using System.Diagnostics;

namespace MyIISManager
{

	/// <summary>
	/// IISWebVirtualDir����Ŀ¼�ࡣ
	/// ����Ҫע�⣺ADsPath �����ִ�Сд�ģ���Ϊ���Ǵ�MetaBase.xml�н�����
	/// </summary>
	public class IISWebVirtualDir
	{
		#region "˽�е�ʵ���ֶ�"
		private string _machine,_identifier,_adsPath,_port,_appPoolId,_defaultDoc,
			_path,_name,_dir,_appFriendlyName,_aspNetVersion;
		private int _appIsolated;
		private bool _enableDefaultDoc,_enableDirBrowsing,_accessRead,_accessScript,_isApplication;
		private IISWebVirtualDir _parent;
		private DirectoryEntry _dirEntry;
		#endregion

		#region "ʵ�����캯��"
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

		#region "������ʵ������"

		/// <summary>
		//��������Ŀ¼����
		/// </summary>
		public IISWebVirtualDir Parent
		{
			get{ return _parent;}
		}

//		/// <summary>
//		//MachineName���Զ�����ʻ��������֣�������IP�������
//		/// </summary>
//		public string MachineName
//		{
//			get{ return _machine;}
//			set{ _machine = value;}
//		}

		/// <summary>
		// ����Ŀ¼����
		/// </summary>
		public string Name
		{
			get{ return _name; }
			set{ _name = value; }
		}

		/// <summary>
		// ����·��
		/// </summary>
		public string Path
		{
			get{ return _path; }
			set{ _path = value; }
		}


//		/// <summary>
//		/// ��վ�Ķ˿ں�
//		/// </summary>
//		public string Port
//		{
//			get{ return _port; }
//			set{ _port = value; }
//		}

//		/// <summary>
//		/// ��վ�ı�ʶ��
//		/// </summary>
//		public string Identifier
//		{
//			get{ return _identifier; }
//			set{ _identifier = value; }
//		}


//		/// <summary>
//		/// ADsPath·��
//		/// </summary>
//		public string ADsPath
//		{
//			get{ return _adsPath; }
//		}

		/// <summary>
		/// Ӧ�ó����Id
		/// </summary>
		public string AppPoolId
		{
			get{ return _appPoolId; }
			set{ _appPoolId = value; }
		}

		/// <summary>
		/// ��ȡȨ��
		/// </summary>
		public bool AccessRead
		{
			get{ return _accessRead; }
			set{ _accessRead = value; }
		}


		/// <summary>
		/// �ű�֧��
		/// </summary>
		public bool AccessScript
		{
			get{ return _accessScript; }
			set{ _accessScript = value; }
		}

		/// <summary>
		/// Ĭ���ĵ�
		/// ��ʽΪ��"Default.htm,Default.asp,index.htm,Default.aspx"
		/// </summary>
		public string DefaultDoc
		{
			get{ return _defaultDoc; }
			set{ _defaultDoc = value; }
		}

		/// <summary>
		/// ʹ��Ĭ���ĵ�
		/// </summary>
		public bool EnableDefaultDoc
		{
			get{ return _enableDefaultDoc; }
			set{ _enableDefaultDoc = value; }
		}

		/// <summary>
		/// Ŀ¼���
		/// </summary>
		public bool EnableDirBrowsing
		{
			get{ return _enableDirBrowsing; }
			set{ _enableDirBrowsing = value; }
		}


		/// <summary>
		/// Web Ӧ�ó�������
		/// </summary>
		public string AppFriendlyName
		{
			get{ return _appFriendlyName; }
			set{ _appFriendlyName = value; }
		}

		/// <summary>
		/// Web Ӧ�ó���Ľ�������
		/// IIS5�����ϰ汾��ʹ��AppCreate2()���� ���������� (0)������������ (1)�����̳���(2)	
		/// IIS5���°汾��ʹ��AppCreate()���������� (1) �������� (0)	
		/// </summary>
		public int AppIsolated
		{
			get{ return _appIsolated; }
			set{ _appIsolated = value; }
		}

		/// <summary>
		/// �Ƿ�Ӧ�ó���
		/// </summary>
		public bool IsApplication
		{
			get{ return _isApplication; }
			set{ _isApplication = value; }
		}

		/// <summary>
		/// ����Ŀ¼��DirectoryEntry����(Root)
		/// </summary>
		public DirectoryEntry DirectoryEntry
		{
			get {return _dirEntry;}
		}

		//����Ŀ¼�Ӽ�
		public IISWebVirtualDirCollection WebVirtualDirs
		{
			get{ return GetVirDirs(); }
		}

        //ASP.NET�汾
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

		#region "������ʵ������"

		//����һ������Ŀ¼
		public bool Create()
		{
			//�����ڵ��Ƿ����
			string parenADsPath = _adsPath.Substring(0,_adsPath.LastIndexOf("/"));
			if (!IISWebService.IsPathExists(parenADsPath))
			{
				throw(new Exception(string.Format("����Ŀ¼��{0}�������ڣ�����ʧ�ܣ�",parenADsPath)));
			}

			//Ҫ������Ŀ¼�Ƿ��Ѿ�����
			DirectoryEntry parentEntry =new DirectoryEntry(parenADsPath);
            bool blnExistEntry = false;
			foreach (DirectoryEntry de in parentEntry.Children)
			{
				if (de.SchemaClassName=="IIsWebVirtualDir" && de.Name==_name)
				{
                    _dirEntry = de;
                    blnExistEntry = true;
                    break;
                    //throw(new Exception(string.Format("����Ϊ��{0}��������Ŀ¼�Ѿ����ڣ�����ʧ�ܣ�",_name)));
				}
			}
            if (!blnExistEntry)
            {
                _dirEntry = parentEntry.Children.Add(_name, "IIsWebVirtualDir");
            }

			//�Ƿ���ΪӦ�ó���
			if(this._isApplication)
			{
				//Ϊ����Ŀ¼����Ӧ�ó�����(��ͬIIS�汾������ͬ)
				IISVersionEnum iisVersion = IISWebService.GetIISVersion(_machine);

				if(iisVersion==IISVersionEnum.IIS5 || iisVersion==IISVersionEnum.IIS6 || iisVersion==IISVersionEnum.IIS7)
				{	
					//����������(0)������������(1)�������⻺���������(2)
					_dirEntry.Invoke("AppCreate2",Convert.ToInt32(this._appIsolated));
				}
				else
				{
					//����������(TRUE) ������������(FALSE) 
					_dirEntry.Invoke("AppCreate",Convert.ToInt32(this._appIsolated));
				}
			}

            Update();
            if (!blnExistEntry)
            {
                parentEntry.CommitChanges();
            }

			//Ȼ���������
            //Update();
            IISWebService.RegisterAspNet(_dirEntry.Path, _aspNetVersion);
			return true;
		}


		//ɾ��һ������Ŀ¼
		public void Delete()
		{
			DirectoryEntry parentEntry = this._parent.DirectoryEntry;


			if(IISWebService.IsPathExists(this._adsPath))
			{
				DirectoryEntry dirEntry = new DirectoryEntry(this._adsPath);

				//����һ��ʹ�ø��ڵ��ɾ������
				object[] paras = new object[2];
				paras[0]	= "IIsWebVirtualDir";		//��ʾ������������Ŀ¼
				paras[1]	= _dir;
				parentEntry.Invoke("Delete",paras);
				parentEntry.CommitChanges();

				/*
				//��������Ҳ����ʹ�ýڵ������ɾ�����������ַ��������⣬��Ҫ�о���
				DirectoryEntry virDir=_rootDirEntry.Children.Find(dir,"IIsWebVirtualDir");
				virDir.Invoke("AppDelete",true); 
				_rootDirEntry.CommitChanges(); 
				*/
			}
			else
			{
				throw new Exception(string.Format("Ŀ¼ http://{0}:{1}/{2} �����ڣ�ɾ��ʧ�ܣ�",this._machine,this._port,this._dir));
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

		//��������Ŀ¼����
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

		#region "˽�з���"
		//��ʼ��
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
					//����Ǹ��ڵ�Root����Ҫ���⴦�����������Ŀ¼Ϊ����ΪRoot�أ�
					this._adsPath=string.Format("IIS://{0}/W3SVC/{1}/Root",_machine,_identifier);
				}
				else
				{
					this._adsPath=string.Format("IIS://{0}/W3SVC/{1}/Root/{2}",_machine,_identifier,_dir);
				}

				//��ȡ�����ڵ�
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
					throw new Exception(string.Format("http://{0}:{1}/{2}����·�������ڣ�",this._machine,this._port,_dir));
				}

				//��ȡ������Ϣ
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
					this._appFriendlyName	= "Ĭ��Ӧ�ó���";
					this._appIsolated	= 2;

					this._path = "";
				}

			} 
			catch(Exception e)
			{
				throw new Exception("�޷����ӵ� http://"+ _machine +":"+_port+"/"+_dir+" ��",e);
			}
		}


		//��ȡ��ǰĿ¼�����е�ֱ���¼�����Ŀ¼
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


