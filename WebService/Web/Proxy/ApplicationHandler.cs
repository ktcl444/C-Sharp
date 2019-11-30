using System.Collections.Generic;
using System.Collections.Specialized;
using Web.Entity;

namespace Web.Proxy
{
    public class ApplicationHandler
    {
        [Mysoft.MAP2.Ajax.AjaxMethod(EnableSessionState = true, AllowHandleError = false)]
        public ApplicationResponse ProcessRequestService(string serviceName, string methodName, List<string> customParams, NameValueCollection queryString)
        {
            var response = new ApplicationResponse {HasReturnValue = true, Result = "Success"};
            return response;
        }
    }
}