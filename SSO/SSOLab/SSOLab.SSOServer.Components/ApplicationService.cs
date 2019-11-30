using System;
using System.Collections.Generic;
using System.Text;

namespace SSOLab.SSOServer.Components
{
    public class ApplicationService
    {
        public static readonly int SSO_KEY_LENGTH = 128;

        public Application GetApplicationByName(string name)
        {
            if (name == "portal")
            {
                Application application = new Application();
                application.ID = "C8288957-B6AA-4522-99FF-7D1E60509974";
                application.Name = "门户系统";
                application.SSOKey = "Xj1wD4DT7UicRVxOBJdjAg2AjErHkoEDlB9GqMJtYMwbfbnc9slagStcVt0Y3lY0XVKDnn6nO9cnCPDwM0tJU6iCBlWEoomDfjAjhobLurOxHR8ua8a25NGNQXQ1Q34X";

                return application;
            }
            else if (name == "app1")
            {
                Application application = new Application();
                application.ID = "1466B140-D840-430a-9598-619D6888E9DB";
                application.Name = "人力资源管理系统";
                application.SSOKey = "XD0cEmXD0IcmYD0gBmYE0OdnYE1jHnZE1USnZF3y3GYpm93Gjp2s2GSog32FfoDm2FZoaG2FZnmhpkVBlJXkWB1eGkWCqA2lWC7k2lXCw1dlXDMqolZr805InrQk4Ixq";

                return application;
            }
            else if (name == "app2")
            {
                Application application = new Application();
                application.ID = "2A5F9A65-7490-480e-836F-DF18608D617B";
                application.Name = "财务管理系统";
                application.SSOKey = "XBtyndN8yHpZCiM1eO9XtE1qii9Oey17CYosH8cM7nRnXBIBjdN811pZrtw1PfhcBDyq7S9OeHcGmWAR7ycM7aloXBCsXQhe10FgrBEwPfSndDZpGwxbL55ymWAmhycM";

                return application;
            }
            else if (name == "app3")
            {
                Application application = new Application();
                application.ID = "F788FED3-32EE-470a-8DDF-0E6AD8A2FCEC";
                application.Name = "网上办公系统";
                application.SSOKey = "XJbbaaAnnQC67829OLkEKwgwiZL30oegpTbptQG0SLQG97665k4O32bb5CQdnffggufXJmBW16nZesssc2AOJl6bO0wiZLiu7k7FTbq27d0CdUG9110ykINvggh5CRjn";

                return application;
            }
            else
            {
                return null;
            }
        }
    }
}
