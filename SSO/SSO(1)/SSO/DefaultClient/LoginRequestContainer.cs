namespace SSO
{
    using System;
    using System.Collections.Generic;

    public class LoginRequestContainer:ILoginRequestContainer
    {
        private static List<ILoginRequest> container = new List<ILoginRequest>();
        private static readonly LoginRequestContainer instance = new LoginRequestContainer();

        private LoginRequestContainer()
        {
        }

        public void Add(ILoginRequest r)
        {
            lock (container)
            {
                container.Add(r);
            }
        }

        public bool Check(string id)
        {
            lock (container)
            {
                foreach (LoginRequest request2 in container)
                {
                    if (request2.Identity == id)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void Remove(string id)
        {
            lock (container)
            {
                LoginRequest item = null;
                foreach (LoginRequest request2 in container)
                {
                    if (request2.Identity == id)
                    {
                        item = request2;
                        break;
                    }
                }
                if (item != null)
                {
                    container.Remove(item);
                }
            }
        }

        public static LoginRequestContainer Instance
        {
            get
            {
                return instance;
            }
        }
    }
}

