using System;
using System.Net;

namespace Rocket.Core.Utils
{
    public class RocketWebClient : WebClient
    {
        public int Timeout { get; set; }

        public RocketWebClient()
            : this(30000)
        {
        }

        public RocketWebClient(int timeout) => Timeout = timeout;

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request != null)
                request.Timeout = Timeout;
            return request;
        }
    }
}