using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CsQuery;
using HtmlAgilityPack;
using PharmStd.Data;

namespace PharmStd.Crawl
{
    class Program
    {
        public static WebClient Client = new WebClient();
        private static PostgresContext _ctx = new PostgresContext();

        public static async Task Main(string[] args)
        {
            var categoriesPage = await Client.DownloadStringTaskAsync("https://www.vidal.ru/drugs/clinic-groups");
            /*
            var catDoc = new HtmlDocument();
            catDoc.LoadHtml(categoriesPage);*/
            var dom = CQ.Create(categoriesPage);
            var links = dom["ul.ul-style a"];
            List<Drug> drugs = new List<Drug>();
            int count = 0, fails = 0;
            foreach (var a in links.Elements)
            {
                var href = a.Attributes["href"];
                Console.WriteLine("href: " + href);
                var categoryName = a.InnerText.Trim();
                var x = await GetDrugsInCategory(href, categoryName);
                drugs.AddRange(x);
                foreach (var drug in x)
                {
                    count++;
                    try
                    {
                        await _ctx.DrugsInsert(drug);
                    }
                    catch (Exception e)
                    {
                        fails++;
                    }
                }
            }
            Console.WriteLine($"count {count}, fails {fails}");
        }

        private static async Task<List<Drug>> GetDrugsInCategory(string href, string categoryName)
        {
            var drugsPage = await Client.DownloadStringTaskAsync("https://www.vidal.ru" + href);

            var dom = CQ.Create(drugsPage);
            var drugsInCat = new List<Drug>();
            foreach (var drugA in dom[".products-table-name a"].Elements)
            {
                var drugHref = "https://www.vidal.ru" + drugA.Attributes["href"];
                Console.WriteLine("drughref: " + drugHref);
                drugsInCat.Add(await GetDrugInfo(drugHref, drugA.InnerText.Trim(), categoryName));
            }

            return drugsInCat.Where(d => d != null).ToList();
        }

        private static async Task<Drug> GetDrugInfo(string href, string drugName, string categoryName)
        {
            var drugPage = await Client.DownloadStringTaskAsync(href);
            var dom = CQ.Create(drugPage);
            try
            {
                var name = HttpUtility.HtmlDecode(drugName);
                var activeComponent = HttpUtility.HtmlDecode(string.Join(", ",
                    dom[".block-content.composition tbody tr td:first-child"].Elements.Skip(1)
                        .Select(x => x.InnerText.Trim())));
                var atxCode = HttpUtility.HtmlDecode(dom["#atc_codes a"].Elements.First().InnerText.Trim());
                var pharmGroup = HttpUtility.HtmlDecode(dom["#phthgroup a"].Elements.First().InnerText.Trim())
                    .Split('.');
                var category = HttpUtility.HtmlDecode(categoryName);
                var rxOnly = dom["#pharm"].Elements.Any();
                return new Drug()
                {
                    Name = name,
                    ActiveComponent = activeComponent,
                    AtxCode = atxCode,
                    PharmGroup = pharmGroup,
                    Category = category,
                    RxOnly = rxOnly,
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}