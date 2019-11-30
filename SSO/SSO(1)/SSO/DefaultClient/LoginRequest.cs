namespace SSO
{
    using System;

    public class LoginRequest : ILoginRequest
    {
        private string _identity = Guid.NewGuid().ToString();
        private DateTime _timeStamp = DateTime.Now;

        public string Identity
        {
            get
            {
                return this._identity;
            }         
        }

        public DateTime TimeStamp
        {
            get
            {
                return this._timeStamp;
            }         
        }
    }
}

