using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Web.Caching;

using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;

namespace CacheMemoryTest
{
    public class CacheHelper
    {
        private readonly static Cache _cache;
        public static void Clear()
        {
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            while (CacheEnum.MoveNext())//找出所有的Cache
            {
                _cache.Remove(CacheEnum.Key.ToString());
            }
        }

        public static int GetNumber()
        {
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator(); 
            int i = 0;
            while (CacheEnum.MoveNext())//找出所有的Cache
            {
                i++;
            }
            return i;
        } 

        static CacheHelper()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                _cache = context.Cache;
            }
            else
            {
                _cache = HttpRuntime.Cache;
            }
        }

        public static Cache GetCache()
        {
            return _cache;
        }

        public struct MEMORYSTATUS1 //这个结构用于获得系统信息
        {
            internal uint dwLength;
            internal uint dwMemoryLoad;
            internal uint dwTotalPhys;
            internal uint dwAvailPhys;
            internal uint dwTotalPageFile;
            internal uint dwAvailPageFile;
            internal uint dwTotalVirtual;
            internal uint dwAvailVirtual;
        }

        [DllImport("kernel32.dll ")]//调用系统DLL
        public static extern void GlobalMemoryStatus(ref   MEMORYSTATUS1 lpBuffer); //获得系统DLL里的函数
        /// <summary>
        /// 获得系统内存使用情况
        /// </summary>
        /// <param name="useInfo">已用内存</param>
        /// <param name="allInfo">内存总量</param>
        public static void GetMemory(out string useInfo, out string allInfo)
        {
            MEMORYSTATUS1 vBuffer = new MEMORYSTATUS1();//实例化结构
            GlobalMemoryStatus(ref   vBuffer);//给此结构赋值
            useInfo = Convert.ToString(vBuffer.dwAvailPhys / 1024 / 1024 + "MB");//获得已用内存量
            allInfo = Convert.ToString(vBuffer.dwTotalPhys / 1024 / 1024 + "MB");//获得内存总量
        }

    }
}
