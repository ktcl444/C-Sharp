using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Entity
{
    public class AppResponse
    {
        /// <summary>
        /// 正常执行时的返回信息
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// 是否包含返回值
        /// </summary>
        public bool HasReturnValue { get; set; }
    }
}