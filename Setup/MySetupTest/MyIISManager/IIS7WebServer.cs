using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;
using System.DirectoryServices;
using System.Diagnostics;

namespace MyIISManager
{
    class IIS7WebServer : IIISWebServer
    {
        #region 私有属性
        private string _machine, _port, _appPoolId, _siteName,
    _path, _appFriendlyName;
        private int _appIsolated;
        private Site _site;
        private ServerManager _iisManager;
        private ApplicationPool _appPool;
        #endregion

        #region 公有属性
        /// <summary>
        /// 计算机名或者IP。如果本机可以使用localhost。
        /// </summary>
        public string Machine
        {
            get { return _machine; }
        }


        /// <summary>
        /// 网站名称
        /// </summary>
        public string SiteName
        {
            get { return _siteName; }
            set { _siteName = value; }
        }

        /// <summary>
        /// 端口号
        /// </summary>
        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// 物理路径
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        /// <summary>
        /// 应用程序池Id
        /// </summary>
        public string AppPoolId
        {
            get { return _appPoolId; }
            set { _appPoolId = value; }
        }

        /// <summary>
        /// Web 应用程序名称
        /// </summary>
        public string AppFriendlyName
        {
            get { return _appFriendlyName; }
            set { _appFriendlyName = value; }
        }

        /// <summary>
        /// Web 应用程序的进程类型
        /// IIS5及以上版本（使用AppCreate2()）： 进程内运行 (0)、进程外运行 (1)、进程池中(2)	
        /// IIS5以下版本（使用AppCreate()）：进程内 (1) 、进程外 (0)	
        /// </summary>
        public int AppIsolated
        {
            get { return _appIsolated; }
            set { _appIsolated = value; }
        }
        #endregion

        #region 公共方法

        #region IIISWebServer 成员
        /// <summary>
        /// 新建网站
        /// </summary>
        public bool Create()
        {
            if (_port.ToString() == "" || _siteName.ToString() == "" || _path.ToString() == "")
            {
                throw (new Exception("网站名称、端口号、物理路径不允许为空！"));
            }

            CreateAppPool(_appPoolId);
            CreateSite(_siteName, _path, Convert.ToInt32(_port));
            SetMime(_siteName);
            return true;
        }
       
        public void Delete()
        {
            if (_site != null && _iisManager.Sites.Contains(_site))
            {
                _iisManager.Sites.Remove(_site);
            }
        }

        public bool Update()
        {
            _iisManager.CommitChanges();
            return true;
        }

        public void Start()
        {
            if (_site != null && _site.State != ObjectState.Started)
            {
                _site.Start();
            }
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            if (_site != null && _site.State != ObjectState.Stopped)
            {
                _site.Stop();
            }
        }

        public void Continue()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _iisManager.Dispose();
        }

        public IISServerState GetStatus()
        {
            throw new NotImplementedException();
        }

        public void Init(string strMachine, string strWebName, string strWebPort, string strWebDir, string strAppFriendlyName, int appIsolated, string strAppPool)
        {
            this._port = (string.IsNullOrEmpty(strWebPort) ? "" : strWebPort);
            this._machine = (string.IsNullOrEmpty(strMachine) ? "" : strMachine);
            this._appPoolId = (string.IsNullOrEmpty(strAppPool) ? "DefaultAppPool" : strAppPool);
            this._appFriendlyName = (string.IsNullOrEmpty(strAppFriendlyName) ? "默认应用程序" : strAppFriendlyName);
            this._appIsolated = appIsolated;
            this._path = (string.IsNullOrEmpty(strWebDir) ? "" : strWebDir);
            this._siteName = (string.IsNullOrEmpty(strWebName) ? "" : strWebName);
            _iisManager = new ServerManager();
        }

        public bool CheckPortRepeated()
        {
            foreach (Site site in _iisManager.Sites)
            {
                foreach (Binding bd in site.Bindings)
                {
                    if (bd.EndPoint.Port == Convert.ToInt32(_port))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #endregion

        #region 私有方法
        /// <summary>
        /// 创建网站
        /// </summary>
        /// <param name="_siteName">名称</param>
        /// <param name="_path">物理路径</param>
        /// <param name="_port">端口号</param>
        /// <returns></returns>
        private void CreateSite(string _siteName, string _path, int _port)
        {
            bool blnExistSite = false;
            foreach (Site site in _iisManager.Sites)
            {
                if (site.Name == _siteName)
                {
                    throw (new Exception(string.Format("名称为“{0}”的网站已经存在，创建失败！", _siteName)));
                }
                foreach(Binding bd in site.Bindings)
                {
                    if (bd.EndPoint.Port == _port)
                    {
                        site.Stop();
                        break;
                    }
                }
            }
            if (!blnExistSite)
            {
                _site = _iisManager.Sites.Add(_siteName, _path, _port);
                _site.ApplicationDefaults.ApplicationPoolName = _appPoolId;
                _iisManager.CommitChanges();
            }
        }

        /// <summary>
        /// 创建应用程序池
        /// </summary>
        /// <param name="strAppPoolName">名称</param>
        /// <returns></returns>
        private void CreateAppPool(string strAppPoolName)
        {
            bool blnExistAppPool = false;
            foreach (ApplicationPool appPool in _iisManager.ApplicationPools)
            {
                if (appPool.Name == strAppPoolName)
                {
                    blnExistAppPool = true;
                    _appPool = appPool;
                    break;
                }
            }
            if (!blnExistAppPool)
            {
                _appPool = _iisManager.ApplicationPools.Add(strAppPoolName);
            }
            _appPool.ManagedRuntimeVersion = "v2.0";
            _appPool.ManagedPipelineMode = ManagedPipelineMode.Classic;
            _appPool.ProcessModel.IdentityType = ProcessModelIdentityType.NetworkService;
        }

        /// <summary>
        /// 设置Mime类型
        /// </summary>
        /// <param name="siteName">网站名称</param>
        private void SetMime(string siteName)
        {
            if (_iisManager != null)
            {
                Configuration config = _iisManager.GetWebConfiguration(siteName);
                ConfigurationSection staticContentSection = config.GetSection("system.webServer/staticContent");
                ConfigurationElementCollection staticContentCollection = staticContentSection.GetCollection();

                SetMime(staticContentCollection, ".ini", "application/octet-stream");
                SetMime(staticContentCollection, ".ifd", "application/ifd");
                SetMime(staticContentCollection, ".dbf", "application/dbf");

                _iisManager.CommitChanges();
            }
        }

        /// <summary>
        /// 设置Mime类型
        /// </summary>
        /// <param name="staticContentCollection">站点配置集合</param>
        /// <param name="extension">扩展名</param>
        /// <param name="mimeType">Mime类型</param>
        private void SetMime(ConfigurationElementCollection staticContentCollection, string extension, string mimeType)
        {
            ConfigurationElement mimeMapElement = staticContentCollection.CreateElement("mimeMap");
            mimeMapElement["fileExtension"] = @extension;
            mimeMapElement["mimeType"] = @mimeType;
            staticContentCollection.Add(mimeMapElement);
        }

        #endregion
    }
}
