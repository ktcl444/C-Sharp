using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSO
{
    public class ServiceTicket
    {
        private string ticket;

        private string app;

        private string user;

        public ServiceTicket(string appName,string userName)
        {
            ticket = Guid.NewGuid().ToString();
            app = appName;
            user = userName;
        }

        public string App
        {
            get { return app; }
        }

        public string User
        {
            get { return user; }
        }

        public string Ticket
        {
            get { return ticket; }
        }
    }
}
