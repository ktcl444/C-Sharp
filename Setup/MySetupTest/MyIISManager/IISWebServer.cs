using System;
using System.Collections;
using System.Text;
using System.DirectoryServices;
using System.Reflection;


namespace MyIISManager
{

	/// <summary>
	/// IISWebServer�Ŀ����ࡣIIS��ÿ����վ����һ��WebServer��
	/// ����Ҫע�⣺ADsPath �����ִ�Сд�ģ���Ϊ���Ǵ�MetaBase.xml�н�����
	/// </summary>
	public class IISWebServer : IIISWebServer
	{

		#region "˽�е�ʵ���ֶ�"
		private string _machine,_identifier,_adsPath,_port,_appPoolId,_defaultDoc,_siteName,
			_path,_appFriendlyName;
		private int _appIsolated;
		private bool _enableDefaultDoc,_enableDirBrowsing,_autoStart,_accessRead,_accessScript;
		private IISWebService _parent;
		protected DirectoryEntry _serverEntry,_rootDirEntry;
		#endregion

		#region "ʵ�����캯��"
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

		#region "������ʵ������"

		/// <summary>
		/// ����webservice����
		/// </summary>
		public IISWebService Parent
		{
			get{ return _parent;}
		}


		/// <summary>
		/// �����������IP�������������ʹ��localhost��
		/// </summary>
		public string Machine
		{
			get{ return _machine;}
		}


		/// <summary>
		/// ��վ����
		/// </summary>
		public string SiteName
		{
			get{ return _siteName; }
			set{ _siteName = value; }
		}


		/// <summary>
		/// �˿ں�
		/// </summary>
		public string Port
		{
			get{ return _port; }
			set{ _port = value; }
		}


		/// <summary>
		/// ����·��
		/// </summary>
		public string Path
		{
			get{ return _path; }
			set{ _path = value; }
		}


		/// <summary>
		/// ��վ��ʶ��
		/// </summary>
		public string Identifier
		{
			get{ return _identifier; }
			set{ _identifier = value; }
		}


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
		/// ������
		/// </summary>
		public bool AutoStart
		{
			get{ return _autoStart; }
			set{ _autoStart = value; }
		}


		/// <summary>
		/// ��վ��DirectoryEntry����(Root)
		/// </summary>
		public DirectoryEntry DirectoryEntry
		{
			get {return _rootDirEntry;}
		}

		/// <summary>
		/// ����Ŀ¼�Ӽ�
		/// </summary>
		public IISWebVirtualDirCollection WebVirtualDirs
		{
			get{ return GetVirDirs(); }
		}

		#endregion

		#region "������ʵ������"

		//������վ
		public bool Create()
		{
			if (_port.ToString()=="" || _siteName.ToString()=="" || _path.ToString()=="")
			{
				throw(new Exception("��վ���ơ��˿ںš�����·��������Ϊ�գ�"));
			}
            CreateAppPool(AppPoolId);
			int identifier=1;

			//У�鲻����ͬ������ͬ�˿ڣ�ͬʱ��ȡ���ı�ʶ����
			foreach (IISWebServer server in this._parent.WebServers)
			{
                if (server.SiteName == _siteName)
                {
                    throw (new Exception(string.Format("����Ϊ��{0}������վ�Ѿ����ڣ�����ʧ�ܣ�", _siteName)));
                }
				if (server.Port==_port)
				{
                    //throw(new Exception(string.Format("�˿ڡ�{0}�����Ѿ�������վ������ʧ�ܣ�",_port)));
                    this._parent.DirectoryEntry.Children.Find(server.Identifier, "IIsWebServer").Invoke("Stop", null);
				}
				if (Convert.ToInt32(server.Identifier) > identifier)
					identifier = Convert.ToInt32(server.Identifier);
			}
			_identifier=(identifier+1).ToString();
			_adsPath=string.Format("IIS://{0}/W3SVC/{1}",_machine,_identifier);

			//����webserver
			DirectoryEntry svcEntry = this._parent.DirectoryEntry;
			_serverEntry = svcEntry.Children.Add(_identifier,"IIsWebServer");

			_serverEntry.Properties["ServerComment"][0]=_siteName;
			_serverEntry.Properties["ServerBindings"].Add(":" + _port + ":");
			_serverEntry.Properties["ServerAutoStart"][0]=_autoStart;

			_serverEntry.CommitChanges();
			svcEntry.CommitChanges();

			//����RootĿ¼
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

		//ɾ����վ
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
				throw new Exception(string.Format("��վ http://{0}:{1} �����ڣ�ɾ��ʧ�ܣ�",this._machine,this._port));
			}
		}

		//���µ�ǰ��վ����
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


		//ֹͣ��ǰ��վ
		public void Stop()
		{
			_serverEntry.Invoke("Stop",null);
		}


		//������ǰ��վ
		public void Start()
		{
			_serverEntry.Invoke("Start",null);
		}


		//��ͣ��ǰ��վ
		public void Pause()
		{
			_serverEntry.Invoke("Pause",null);
		}


		//ȡ����ͣ
		public void Continue()
		{
			_serverEntry.Invoke("Continue",null);
		}

		//����ADSI����
		public void Dispose()
		{
			_rootDirEntry.Dispose();
			_serverEntry.Dispose();
		}

		//��ȡ��վ�ĵ�ǰ״̬
		//1 �������� 
		//2 ������ 
		//3 ����ֹͣ 
		//4 ��ֹͣ 
		//5 ������ͣ 
		//6 ����ͣ 
		//7 ���ڼ��� 
		public IISServerState GetStatus()
		{
			//Statusʵ�ʲ��Ƿ������������ԣ����Բ���ʹ������
			//return (int)_serverEntry.Invoke("Status",null);

			//��ȡADSI���������
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

        //��ʼ��
        public void Init(string strMachine,string strWebName, string strWebPort, string strWebDir, string strAppFriendlyName, int appIsolated, string strAppPool)
        {
            try
            {
                this._port = (string.IsNullOrEmpty(strWebPort) ? "" : strWebPort);
                this._machine = (string.IsNullOrEmpty(strMachine) ? "" : strMachine);
                //��ȡ�����ڵ�
                this._parent = new IISWebService(this._machine);

                //��ȡ������Ϣ
                this._identifier = IISWebService.GetIdentifier(_machine, _port);
                if (this._identifier != "" && string.IsNullOrEmpty(strWebName))
                {
                    this._adsPath = string.Format("IIS://{0}/W3SVC/{1}", _machine, _identifier);

                    //webserver������Ϣ
                    _serverEntry = new DirectoryEntry(_adsPath);
                    this._siteName = (string)_serverEntry.Properties["ServerComment"][0];
                    this._port = _serverEntry.Properties["ServerBindings"][0].ToString().Replace(":", "");
                    this._autoStart = (bool)_serverEntry.Properties["ServerAutoStart"][0];

                    //webserver root������Ϣ(���Է���object���ͣ����������ʾת����)
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
                    this._appFriendlyName = (string.IsNullOrEmpty(strAppFriendlyName) ? "Ĭ��Ӧ�ó���" : strAppFriendlyName);
                    this._appIsolated = appIsolated;
                    this._path = (string.IsNullOrEmpty(strWebDir) ? "" : strWebDir);
                    this._siteName = (string.IsNullOrEmpty(strWebName) ? "" : strWebName);
                }
            }
            catch (Exception e)
            {
                throw new Exception("�޷����ӵ���վ http://" + _machine + ":" + _port + " ��", e);
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

		#region "˽�з���"

        /// <summary>
        /// ����Ӧ�ó����
        /// </summary>
        /// <param name="strAppPoolName">Ӧ�ó��������</param>
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
                    // 0 = Local System������ϵͳ��
                    // 1 = Local Service�����ط���
                    // 2 = Network Service���������
                    // 3 = Custom Identity���Զ������ã���Ҫ���� WAMUserName �� WAMUserPass ����

                    newAppPool.Properties["AppPoolIdentityType"][0] = 2;
                    //newAppPool.Properties["WAMUserName"][0] = @"VISTA\1"; //domain\�û���ע�⣺���û�������IIS_WPG����
                    //newAppPool.Properties["WAMUserPass"][0] = "1";
                    //�����������ƣ�������Web԰��Ŀ��
                    //newAppPool.Properties["MaxProcesses"][0] = 5;
                    newAppPool.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		//��ȡ��Ŀ¼�����е�ֱ���¼�����Ŀ¼
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
