using System;
using System.Data;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
using System.Web.Services.Protocols;
using System.Xml;
using WebApplication1.Entity;

namespace WebApplication1
{
    public class AppService
    {
        public AppResponse Execute(AppRequest request)
        {
            return new AppResponse { HasReturnValue = true, Result = GetCustomDataTable() };
        }


        public DataSet GetCustomDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("Test", typeof(string)));

            var dr = dt.NewRow();
            dr["Test"] = "Test";
            dt.Rows.Add(dr);
            var ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        private void ThrowSoapException()
        {
            var myException = new MyException("This is a MyException");
            XmlDocument doc = new XmlDocument();
            XmlNode detail = doc.CreateNode(XmlNodeType.Element,
                                            SoapException.DetailElementName.Name,
                                            SoapException.DetailElementName.Namespace);

            var errorCode = doc.CreateElement("ErrorCode");
            errorCode.InnerText = myException.ErrorCode.ToString(CultureInfo.InvariantCulture);
            var message = doc.CreateElement("Message");
            message.InnerText = myException.Message;
            var stackTrace = doc.CreateElement("StackTrace");
            stackTrace.InnerText = myException.StackTrace;
            detail.AppendChild(errorCode);
            detail.AppendChild(message);
            detail.AppendChild(stackTrace);

            // This is the detail part of the exception
            //detail.InnerText = "User not authorized to perform requested operation";
            throw new SoapException("Message string from your Web service",
                                    SoapException.ServerFaultCode,
                                    "Mysoft", detail, null);
        }

        [Serializable]
        public class MyException : Exception
        {
            private int _errorCode;

            public int ErrorCode
            {
                get { return _errorCode; }
                set { _errorCode = value; }
            }

            public MyException(string message): base(message)
            {
                _errorCode = 1;
            }
        }
    }
}