using Kur.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;
using System.Linq;

namespace Kur.Controllers
{
    public class HomeController : Controller
    {
        private readonly KurDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(KurDbContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return Ok("Verileri TCMB'den alıp, kaydetme işlemi başlamıştır.");
        }

        public async Task FetchAndSaveTcmbKurlar()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync("https://www.tcmb.gov.tr/kurlar/today.xml");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                
                XDocument doc = XDocument.Parse(content);
                var usdNode = doc.Descendants("Currency").FirstOrDefault(x => x.Element("CurrencyName")?.Value == "US DOLLAR");
                var eurNode = doc.Descendants("Currency").FirstOrDefault(x => x.Element("CurrencyName")?.Value == "EURO");

                if (usdNode != null && eurNode != null)
                {
                    double usdValue = double.Parse(usdNode.Element("ForexBuying")?.Value ?? "0");
                    double eurValue = double.Parse(eurNode.Element("ForexBuying")?.Value ?? "0");

                    var tcmbKurUsd = new TcmbKur
                    {
                        Kur = "USD",
                        Deger = usdValue,
                        Tarih = DateTime.Now
                    };

                    var tcmbKurEur = new TcmbKur
                    {
                        Kur = "EUR",
                        Deger = eurValue,
                        Tarih = DateTime.Now
                    };

                    _dbContext.TcmbKurlar.Add(tcmbKurUsd);
                    _dbContext.TcmbKurlar.Add(tcmbKurEur);

                    await _dbContext.SaveChangesAsync();
                }
            }
            else
            {
                Console.WriteLine("HTTP Request failed with status code: " + response.StatusCode);
            }
        }
    }
}
