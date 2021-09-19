using Notino.Common.Service;
using System.Net;

namespace Notino.Service
{
    public class WebService : IWebService
    {
        private readonly WebClient _webClient;

        public WebService()
        {
            _webClient = new WebClient();
        }

        public byte[] DownloadFile(string url)
        {
            return _webClient.DownloadData(url);
        }
    }
}
