using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using System.Web;

namespace SSO
{
    public class SSOServer : IServer
    {
        private const string defaultUrl = "Default.aspx";

        private DataSet sitesData;
        public DataSet SitesData
        {
            get 
            {
                if (sitesData == null)
                {
                    sitesData = GetSitesData();
                }
                return sitesData;
            }
        }

        private ISite site;

        public SSOServer()
        {
            string siteID = HttpContext.Current.Request.QueryString["SiteID"];
            if (!string.IsNullOrEmpty(siteID))
            {
                siteID = RSAHelper.Decrypt(siteID, string.Empty);
                //site = new DefaultSite(siteID);
            }

        }

        private DataSet GetSitesData()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            DataColumn dc = new DataColumn("SiteID", typeof(string));
            dt.Columns.Add(dc);

            dc = new DataColumn("SiteName", typeof(string));
            dt.Columns.Add(dc);

            DataRow dr = dt.NewRow();
            dr["SiteID"] = "1";
            dr["SiteName"] = "Client1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["SiteID"] = "2";
            dr["SiteName"] = "Client2";
            dt.Rows.Add(dr);

            ds.Tables.Add(dt);

            return ds;
        }

        #region IServer 成员

        public ISite Site
        {
            get { throw new NotImplementedException(); }
        }

        public IUser User
        {
            get { throw new NotImplementedException(); }
        }

        public bool CheckUser()
        {
            throw new NotImplementedException();
        }

        public void SaveServiceTicket()
        {
            throw new NotImplementedException();
        }

        public string ValidateServiceTicket()
        {
            throw new NotImplementedException();
        }

        public void SaveTicketGrantingCookie()
        {
            throw new NotImplementedException();
        }

        public bool CheckTicketGrantingCookie()
        {
            throw new NotImplementedException();
        }

        public bool Login()
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
