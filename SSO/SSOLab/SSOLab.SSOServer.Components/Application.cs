using System;
using System.Collections.Generic;
using System.Text;

namespace SSOLab.SSOServer.Components
{
    public class Application
    {
        private string _id;
        private string _name;
        private string _ssoKey;

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

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public string SSOKey
        {
            get
            {
                return this._ssoKey;
            }
            set
            {
                this._ssoKey = value;
            }
        }

        public Application()
        {

        }
    }
}
