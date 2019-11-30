using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using MyIISManager;

using System.DirectoryServices;
using System.Reflection;
using System.Management;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace MySetupAction
{
    [RunInstaller(true)]
    public partial class MySetupAction : Installer
    {
        
        public MySetupAction()
        {
            //System.Diagnostics.Debugger.Launch();
            InitializeComponent();
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
        }

        protected override void OnAfterRollback(IDictionary savedState)
        {
            base.OnAfterRollback(savedState);
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            base.OnAfterUninstall(savedState);
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
        }

        protected override void OnBeforeRollback(IDictionary savedState)
        {
            base.OnBeforeRollback(savedState);
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);
        }

        protected override void OnCommitted(IDictionary savedState)
        {
            base.OnCommitted(savedState);
        }

        protected override void OnCommitting(IDictionary savedState)
        {
            base.OnCommitting(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        /// <summary>
        /// 安装
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            
                this.InitParameters();
                //this.CopyFile();
                //this.RegOeprate();
                this.CreateWeb();
        }

        private void RegOeprate()
        {
            //System.Diagnostics.Debugger.Launch();
            //Registry Reg;
            RegistryKey RegKey;
            string strRegName = "ERP20";
            RegKey = Registry.LocalMachine.OpenSubKey(@"Software\mysoft\" + strRegName, true);
            if(RegKey != null)
            {
                for(int i = 0;i < RegKey.GetSubKeyNames().Length - 1;i ++)
                {
                    RegKey.SetValue(RegKey.GetSubKeyNames()[i], "0");
                }
            }
        }

        private void CopyFile()
        {
            //System.Diagnostics.Debugger.Launch();

            string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string sourceDir = strSourceDir.Substring(0, strSourceDir.LastIndexOf(@"\")) + @"\SUPPORT\system32\";
            string targetDir = systemFolder + @"\";

            string fileName = "ChilkatDotNet2.dll";
            CopyFile(fileName,sourceDir,targetDir);

            fileName = "hasp_net_windows.dll";
            CopyFile(fileName, sourceDir, targetDir);

            fileName = "hasp_net_windows_x64.dll";
            CopyFile(fileName, sourceDir, targetDir);
        }

        private void CopyFile(string fileName, string sourceDir, string targetDir)
        {
            if (File.Exists(sourceDir + fileName) && Directory.Exists(targetDir))
            {
                //File.Copy(sourceDir + fileName, targetDir + fileName, true);
            }
        }

        /// <summary>
        /// 新建站点
        /// </summary>
        private void CreateWeb()
        {
            //System.Diagnostics.Debugger.Launch();
            string createInfo = string.Empty;
            if (!Directory.Exists(strWebDir))
            {
                Directory.CreateDirectory(strWebDir);
            }

            IIISWebServer isrv = IISWebServerFactory.GetIISWebServer(strMachineName);
            isrv.Init(strMachineName,strWebName, strWebPort, strWebDir, strAppFriendlyName, appIsolated, strAppPool20);
            if (isrv.CheckPortRepeated())
            {
                if (MessageBox.Show("端口重复,你确定继续安装吗?\n确定继续\n取消停止", "安装提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    throw (new Exception(string.Format("端口“{0}”上已经存在网站，创建失败！", strWebPort)));
                }
            }
            
                if (isrv.Create())
                {
                    isrv.Start();
                    createInfo = "创建成功！";
                }
                else
                {
                    createInfo = "创建失败！";
                }
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameters()
        {
            strMachineName = this.Context.Parameters["strMachineName"];
            strWebName = this.Context.Parameters["strWebName"];
            strWebPort = this.Context.Parameters["strWebPort"];
            strWebDir = this.Context.Parameters["strWebDir"];
            strSourceDir = this.Context.Parameters["strSourceDir"];
            if (string.IsNullOrEmpty(strAppFriendlyName))
            {
                strAppFriendlyName = "默认应用程序";
            }
        }
    }
}
