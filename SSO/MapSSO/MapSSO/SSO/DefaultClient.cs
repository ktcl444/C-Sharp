using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSO
{
    class SSOClient : IClient
    {
        #region IClient 成员

        public string SiteID
        {
            get { throw new NotImplementedException(); }
        }

        public string PrivateKey
        {
            get { throw new NotImplementedException(); }
        }

        public string LoginUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string LogoutUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string UserID
        {
            get { throw new NotImplementedException(); }
        }

        public int TimeOut
        {
            get { throw new NotImplementedException(); }
        }

        public void Login()
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public string PublicKey
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
