using MihaZupan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Proxychecker
{
    class Scraper
    {
        public static List<ProxyInfo> ScrapePublicLists()
        {
            string list = scrapePublicLists();
            string[] proxies = list.Split(new[] { Environment.NewLine },StringSplitOptions.None);

            List<ProxyInfo> ProxyList = new List<ProxyInfo>();
            foreach(string proxy in proxies)
            {
                if (String.IsNullOrWhiteSpace(proxy))
                    continue;
                string[] ipPort = proxy.Split(':');
                HttpToSocks5Proxy socks5 = new HttpToSocks5Proxy(ipPort[0], Convert.ToInt32(ipPort[1]));
                ProxyInfo proxyCheck = new ProxyInfo(socks5, ipPort);
                ProxyList.Add(proxyCheck);
            }
            return ProxyList;
        }
        private static string scrapePublicLists()
        {
            FirefoxOptions options = new FirefoxOptions();
            options.AddArguments("--headless");
            string URL1 = "https://www.proxy-list.download/SOCKS4";
            string URL2 = "https://www.proxy-list.download/SOCKS5";

            using (var driver = new FirefoxDriver(options))
            {
                driver.Navigate().GoToUrl(URL1);
                Task.Delay(1000).Wait();

                string js = "var a = \"\";arr.forEach(function(t) {a += t.IP + \":\" + t.PORT + \"\\r\\n\"});return a;";
                IJavaScriptExecutor jsexec = (IJavaScriptExecutor)driver;
                string proxies = (string)jsexec.ExecuteScript(js);

                driver.Navigate().GoToUrl(URL2);
                Task.Delay(1000).Wait();

                jsexec = (IJavaScriptExecutor)driver;
                proxies += (string)jsexec.ExecuteScript(js);
                driver.Quit();
                return proxies;
            }
        }
    }
}
