using System;
using System.Collections.Generic;
using System.Text;

namespace SSOLab.SSOServer.Components
{
    public class UserService
    {
        public bool AuthenticationUser(string username, string password, out string id)
        {
            User user = GetUserByName(username);

            if (user != null && user.Password == password)
            {
                id = user.ID;

                return true;
            }
            else
            {
                id = String.Empty;

                return false;
            }
        }

        public User GetUserByID(string userID)
        {
            if (userID == "464FA65A-0DFF-46a9-AC0B-3EF1E4CDFF94")
            {
                User user = new User();
                user.ID = "464FA65A-0DFF-46a9-AC0B-3EF1E4CDFF94";
                user.Username = "51aspx";
                user.Password = "51aspx";

                return user;
            }
            else
            {
                return null;
            }
        }

        public User GetUserByName(string username)
        {
            if (username == "51aspx")
            {
                User user = new User();
                user.ID = "464FA65A-0DFF-46a9-AC0B-3EF1E4CDFF94";
                user.Username = "51aspx";
                user.Password = "51aspx";

                return user;
            }
            else
            {
                return null;
            }
        }

    }
}
