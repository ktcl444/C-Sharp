using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSO
{
    /// <summary>
    /// 服务器接口
    /// </summary>
    interface IServer
    {
        /// <summary>
        /// 站点
        /// </summary>
        ISite Site { get; }

        /// <summary>
        /// 用户
        /// </summary>
        IUser User { get; }

        /// <summary>
        /// 检测用户
        /// </summary>
        /// <returns></returns>
        bool CheckUser();

        /// <summary>
        /// 保存服务票据
        /// </summary>
        void SaveServiceTicket();

        /// <summary>
        /// 校验服务票据
        /// </summary>
        /// <returns>用户名</returns>
        string ValidateServiceTicket();

        /// <summary>
        /// 保存票据授权Cookie
        /// </summary>
        void SaveTicketGrantingCookie();

        /// <summary>
        /// 检测票据授权Cookie
        /// </summary>
        /// <returns></returns>
        bool CheckTicketGrantingCookie();

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        bool Login();

        /// <summary>
        /// 登出
        /// </summary>
        void Logout();
    }
}
