using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSO
{
    /// <summary>
    /// 用户接口
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        string UserID { get; }

        /// <summary>
        /// 密码
        /// </summary>
        string Password { get; }

        /// <summary>
        ///注册
        /// </summary>
        void Register();

        /// <summary>
        /// 检测用户
        /// </summary>
        /// <returns></returns>
        bool CheckExist();
    }
}
