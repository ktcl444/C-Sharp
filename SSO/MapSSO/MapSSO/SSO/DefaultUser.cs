using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSO
{
    class DefaultUser : IUser
    {
        #region IUser 成员

        public string UserID
        {
            get { throw new NotImplementedException(); }
        }

        public string Password
        {
            get { throw new NotImplementedException(); }
        }

        public void Register()
        {
            throw new NotImplementedException();
        }

        public bool CheckExist()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
