using System;
using System.Collections.Generic;
using System.Text;
//该源码下载自www.51aspx.com(５１ａｓｐx．ｃｏｍ)

namespace SSOLab.SSOServer.Components
{
    public class User
    {
        private string _id;
        private string _username;
        private string _password;

        public string ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        public string Username
        {
            get
            {
                return this._username;
            }
            set
            {
                this._username = value;
            }
        }

        public string Password
        {
            get
            {
                return this._password;
            }
            set
            {
                this._password = value;
            }
        }

        public User()
        {

        }
    }
}
