using MihaZupan;

namespace Proxychecker
{
    public class ProxyInfo
    {
        public HttpToSocks5Proxy Proxy;
        public string[] ipPort;

        public ProxyInfo(HttpToSocks5Proxy _proxy, string[] _ipPort)
        {
            Proxy = _proxy;
            ipPort = _ipPort;
        }
    }
}
