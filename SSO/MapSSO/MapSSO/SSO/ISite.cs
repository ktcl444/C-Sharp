using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSO
{
    /// <summary>
    /// 站点接口
    /// </summary>
    public interface ISite
    {

        /// <summary>
        /// 站点ID
        /// </summary>
        string SiteID { get; }

        /// <summary>
        /// 主页
        /// </summary>
        string HomePage { get; }

        /// <summary>
        /// 私钥
        /// </summary>
        string PrivateUrl { get; }

        /// <summary>
        /// 公钥
        /// </summary>
        string PublicKey { get; }

        /// <summary>
        /// 新增站点
        /// </summary>
        void Add();

        /// <summary>
        /// 检测站点
        /// </summary>
        /// <returns></returns>
        bool CheckExist();
    }
}
