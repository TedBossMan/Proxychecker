using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MihaZupan;

namespace Proxychecker
{
    class Program
    {
        static void Main(string[] args)
        {
            ProxyManager manager = new ProxyManager();

            var listOfProxies = Scraper.ScrapePublicLists();
            var aliveProxies = ProxyCheck.checkProxiesAlive(listOfProxies, 25);
            manager.AppendWorkingProxies(aliveProxies);

        }

    }
}
