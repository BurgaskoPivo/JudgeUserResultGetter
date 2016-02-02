using System;
using System.Net;

namespace JugdeStudentInfo
{
    public class CookieAwareWebClient : WebClient
    {
        public CookieAwareWebClient()
        {
            this.CookieContainer = new CookieContainer();
        }

        public CookieContainer CookieContainer { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = this.CookieContainer;
            }
            return request;
        }
    }
}
