using MihaZupan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxychecker
{
    class ProxyManager
    {
        List<ProxyInfo> Proxies;
        private const string FILEPATH = @".\ActiveProxies.txt";
        public ProxyManager()
        {
            Proxies = importProxies();
            removeBadProxies();
        }

        public int ActiveProxies
        {
            get
            {
                return Proxies.Count;
            }
        }

        private void removeBadProxies()
        {
            if (Proxies.Count <= 0)
                return;

            List<ProxyInfo> newList;

            Console.WriteLine("Checking bad proxies in db...");
            Console.WriteLine("Checking for duplicates...");
            newList = removeDuplicateProxies(Proxies);
            int difference = Proxies.Count - newList.Count;
            Console.WriteLine(difference + " duplicate entries removed from db.");

            int threads = 25;
            while (threads > newList.Count)
                threads -= 1;

            List<ProxyInfo> activeProxies = ProxyCheck.checkProxiesAlive(newList, threads);
            difference = newList.Count - activeProxies.Count;
            Console.WriteLine(difference + " dead proxies removed from db.");
            Proxies = activeProxies;
            saveToDisk(true);
        }
        private List<ProxyInfo> removeDuplicateProxies(List<ProxyInfo> proxies)
        {
            return proxies.GroupBy(x => x.ipPort[0]).Select(x => x.First()).Distinct().ToList();
        }
        public void AppendWorkingProxies(List<ProxyInfo> activeProxies)
        {
            Proxies.AddRange(activeProxies);
            Proxies = removeDuplicateProxies(Proxies);

            saveToDisk(true);
            Console.WriteLine("Saved " + activeProxies.Count + " additional proxies.");
        }

        private void saveToDisk(bool overwrite)
        {
            string buffer = "";
            foreach (var proxy in Proxies)
                buffer += proxy.ipPort[0] + ":" + proxy.ipPort[1] + "\n";

            if(overwrite)
            {
                File.WriteAllText(FILEPATH, buffer);
            }
            else
            {
                File.AppendAllText(FILEPATH, buffer);
            }
        }

        private List<ProxyInfo> importProxies()
        {
            List<ProxyInfo> ProxyList = new List<ProxyInfo>();
            if (!File.Exists(FILEPATH))
                return ProxyList;

            string[] lines = File.ReadAllLines(FILEPATH);
            foreach(string line in lines)
            {
                string[] ipPort = line.Split(':');
                HttpToSocks5Proxy socks5 = new HttpToSocks5Proxy(ipPort[0], Convert.ToInt32(ipPort[1]));
                ProxyInfo proxyCheck = new ProxyInfo(socks5, ipPort);
                ProxyList.Add(proxyCheck);
            }
            Console.WriteLine(ProxyList.Count + " proxies have been imported.");
            return ProxyList;
        }
    }
}
