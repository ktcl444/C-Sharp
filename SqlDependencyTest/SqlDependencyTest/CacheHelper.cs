using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Web.Caching ;

namespace SqlDependencyTest
{
    public class CacheHelper
    {
        public static void Clear()
        {
            IDictionaryEnumerator CacheEnum = GetCache().GetEnumerator();
            while (CacheEnum.MoveNext())//找出所有的Cache
            {
                GetCache().Remove(CacheEnum.Key.ToString());
            } 
        }

        public static Cache GetCache()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Cache;
            }
            else
            {
                return HttpRuntime.Cache;
            }
        }
         
    }
}
