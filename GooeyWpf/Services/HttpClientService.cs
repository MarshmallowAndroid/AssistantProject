using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GooeyWpf.Services
{
    internal class HttpClientService : Singleton<HttpClientService>
    {
        public HttpClientService()
        {
            Client = new HttpClient();
        }

        public HttpClient Client { get; }
    }
}
