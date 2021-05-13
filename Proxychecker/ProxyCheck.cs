using MihaZupan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proxychecker
{
    class ProxyCheck
    {
        private static async Task<bool> isProxyAlive(HttpToSocks5Proxy proxy)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler { Proxy = proxy };
                using (HttpClient httpClient = new HttpClient(handler, true))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(15);
                    var result = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://www.google.com/"));
                    await result;
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        public static List<ProxyInfo> checkProxiesAlive(List<ProxyInfo> proxies, int threads)
        {
            List<ProxyInfo> aliveProxies = new List<ProxyInfo>();

            for (int i = 0; i < proxies.Count; i += threads)
            {
                //create a list with thread number of proxies
                //to be processed at a time
                var miniList = proxies.Skip(i * threads).Take(threads).ToArray();
                Task<bool>[] tasks = new Task<bool>[threads];
                if (miniList.Length <= 0)
                    continue;

                for (int j = 0; j < threads; j++)
                    tasks[j] = isProxyAlive(miniList[j].Proxy);

                Task.WaitAll(tasks);

                //once thread num proxies checked declare any alive ones and add to return var
                var proxiesAlive = Array.FindAll(tasks, t => t.Result);
                foreach (var proxy in proxiesAlive)
                {
                    int index = Array.FindIndex(tasks, t => t == proxy);

                    Console.WriteLine(miniList[index].ipPort[0] + ":" + miniList[index].ipPort[1]);
                    aliveProxies.Add(miniList[index]);
                }
            }
            return aliveProxies;
        }


    }
}
