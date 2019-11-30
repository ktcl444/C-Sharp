using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;
using MyIISManager;
using System.DirectoryServices;

namespace IIS7Manager
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestIIS7();
            try
            {
                TestIIS73();
                //TestProperties();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }

        private static void TestProperties()
        {
            string iden = IISWebService.GetIdentifier("localhost", "8051");
            string strPath = string.Format("IIS://localhost/W3SVC/{0}/ROOT", iden);
            DirectoryEntry root = new DirectoryEntry(strPath);
            string configuration = @"*,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,4,全部";
            // 英文版请将全部改为All
            root.Properties["ScriptMaps"].Add(configuration);
            root.CommitChanges();
        }

        private static void TestIIS7()
        {
            ServerManager _iisManager = new ServerManager();
            Configuration _configuration = _iisManager.GetApplicationHostConfiguration();
            //foreach( string s in _configuration.GetLocationPaths() )
            //{
            //    Console.WriteLine(s);
            //}
            try
            {
                //foreach (SectionDefinition s in _configuration.RootSectionGroup.Sections)
                //{
                //    Console.WriteLine(s.Name +" "+s.ToString()+" "+s.Type.ToString ());
                //}
                _configuration.SetMetadata("", "");
                ConfigurationSection _myConfiguration = _configuration.GetSection("location", "Test/222/Zlxt");
                foreach (ConfigurationAttribute e in _myConfiguration.Attributes)
                {
                    if (e != null)
                    {
                        Console.WriteLine(e.Name + " : " + e.Value);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }

        private static void TestIIS72()
        {
            using (ServerManager m = new ServerManager())
            {
                Configuration config = m.GetApplicationHostConfiguration();

                SectionDefinition definition =
                    RegisterSectionDefinition(config, "location");

                m.CommitChanges();
            }
        }

        private static SectionDefinition RegisterSectionDefinition(Configuration config, string sectionPath)
        {
            string[] paths = sectionPath.Split('/');

            SectionGroup group = config.RootSectionGroup;
            for (int i = 0; i < paths.Length - 1; i++)
            {
                SectionGroup newGroup = group.SectionGroups[paths[i]];
                if (newGroup == null)
                {
                    newGroup = group.SectionGroups.Add(paths[i]);
                }
                group = newGroup;
            }

            SectionDefinition section = group.Sections[paths[paths.Length - 1]];
            if (section == null)
            {
                section = group.Sections.Add(paths[paths.Length - 1]);
            }

            return section;
        }

        private static void TestIIS73()
        {
            string iden = IISWebService.GetIdentifier("localhost", "8001");
            string strPath = string.Format("IIS://localhost/W3SVC/{0}/ROOT", iden);

            //设置普通目录的属性：
            //对于web站点中的普通目录，如果没有通过IIS管理器修改其属性，
            //则该目录不会在MetaBase.xml中创建IIsWebDirectory节点。
            //此时该目录不是IIS Admin Object，所以你是无法通过new	DirectoryEntry("IIS://localhost/W3SVC/1/Root/目录")方法获取。
            //但一旦你修改了普通目录的属性，IIS管理器就会在MetaBase.xml中创建IIsWebDirectory节点，
            //该目录也就变成IIsWebDirectory对象，此时你就可以使用DirectoryEntry获取并设置其属性了。
            DirectoryEntry root = new DirectoryEntry(strPath);
            DirectoryEntry serverEntry;
            try
            {
                serverEntry = root.Children.Find("ClassLibrary1", "IIsWebDirectory"); //如果UpFiles节点不存在，则会报DirectoryNotFoundException异常		
            }
            catch (System.IO.DirectoryNotFoundException ee)
            {
                serverEntry = null;
            }
            if (serverEntry == null)
            {
                serverEntry = root.Children.Add("ClassLibrary1", "IIsWebDirectory");
            }
            serverEntry.Properties["AccessRead"][0] = false;
            serverEntry.CommitChanges();
        }

        private static void SetMapping()
        {
            IISWebService parent = new IISWebService("localhost");
            string iden = IISWebService.GetIdentifier("localhost", "201");
            string Path = String.Format("IIS://localhost/w3svc/{0}/root", iden);
            DirectoryEntry root = new DirectoryEntry(Path);

            string configuration = @"Dowload/*.*,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG";
            // 英文版请将全部改为All
            root.Properties["ScriptMaps"].Add(configuration);
            root.CommitChanges();

            ArrayList arl;
            bool isFindMap = false;
            object[] maps = { };
            if (root.Properties["ScriptMaps"].Value != null)
            {
                maps = (object[])root.Properties["ScriptMaps"].Value;
                //判断是否已经存在映射信息
                for (int i = 0; i < maps.Length; i++)
                {
                    if (maps[i].ToString().IndexOf(@"Dowload/*.*") >= 0)
                    {
                        //如果存在,修改该映射信息
                        maps[i] = @"Dowload/*.*,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG";
                        isFindMap = true;
                    }
                }
                if (!isFindMap)
                {
                    //如果不存在,在数组最后添加该映射信息
                    object[] newMaps = new object[maps.Length];
                    Array.Copy(maps, newMaps, maps.Length);

                    maps = newMaps;
                    maps[maps.Length - 1] = @"Dowload/*.*,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG";
                }
            }
            else
            {
                //如果网站没有任何映射信息,设置第一条映射信息
                maps[0] = @"Dowload/*.*,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG";
            }
            root.Properties["ScriptMaps"].Value = maps; //注意一定要以数组保存
            root.CommitChanges();
        }

        private static void SetMapping2()
        {
            IISWebService parent = new IISWebService("localhost");
            string iden = IISWebService.GetIdentifier("localhost", "201");
            string Path = String.Format("IIS://localhost/w3svc/{0}/root", iden);
            DirectoryEntry root = new DirectoryEntry(Path);
            ArrayList arl;
            bool isFindMap = false;
            object[] maps = { };
            if (root.Properties["ScriptMaps"].Value != null)
            {
                maps = (object[])root.Properties["ScriptMaps"].Value;

                object[] newMaps = new object[maps.Length];
                Array.Copy(maps, newMaps, maps.Length);

                maps = newMaps;
                maps[maps.Length - 1] = @"Dowload/*.*,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG";
            }
            else
            {
                //如果网站没有任何映射信息,设置第一条映射信息
                maps[0] = @"Dowload/*.*,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG";
            }
            root.Properties["ScriptMaps"].Value = maps; //注意一定要以数组保存
            root.CommitChanges();
        }
    }
}
