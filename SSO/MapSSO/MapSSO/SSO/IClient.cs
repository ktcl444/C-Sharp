using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSO
{
    /// <summary>
    /// 客户端接口
    /// </summary>
    interface IClient
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        string SiteID { get; }

        /// <summary>
        /// 私钥
        /// </summary>
        string PrivateKey { get; }

        /// <summary>
        /// 公钥
        /// </summary>
        string PublicKey { get; }

        /// <summary>
        /// 服务器登录链接
        /// </summary>
        string LoginUrl { get; }

        /// <summary>
        /// 服务器登出链接
        /// </summary>
        string LogoutUrl { get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        string UserID { get; }

        /// <summary>
        /// 过期时间
        /// </summary>
        int TimeOut { get; }

        /// <summary>
        /// 登录
        /// </summary>
        void Login();

        /// <summary>
        /// 登出
        /// </summary>
        void Logout();
    }
}
