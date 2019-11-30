using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSO
{
    public class DefaultSite : ISite
    {
        private string siteID;
        private string siteName;
        private string homePage;
        string privateKey;

        public DefaultSite(string siteID,string siteName,string homePage,string privateKey)
        {
            this.siteID = siteID;
            this.siteName = siteName;
            this.homePage = homePage;
            this.privateKey = privateKey;
        }

        #region ISite 成员

        public string SiteID
        {
            get { throw new NotImplementedException(); }
        }

        public string HomePage
        {
            get { throw new NotImplementedException(); }
        }

        public string PrivateUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string PublicKey
        {
            get { throw new NotImplementedException(); }
        }

        public void Add()
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
