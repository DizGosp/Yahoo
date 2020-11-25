using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Yahoo.EF;
using Yahoo.Models;
using Yahoo.ViewModel;
using YahooFinanceApi;
using static Yahoo.Helper.getCompany;
using static Yahoo.Helper.getInfo;

namespace Yahoo.Controllers
{
    public class Yahoo : Controller
    {

        private MyContext _db;
        [ActivatorUtilitiesConstructor]
        public Yahoo(MyContext context)
        {
            _db = context;
        }

        public async Task<IActionResult> IndexAsync(DateTime datum)
        {
            if (!_db.StorageDB.Any())
            {
                await geListTickersAsync();
            }

            List<Storage> podaci = new List<Storage>();
            if (!_db.FinanceStorageDB.Any(x => x.Datum.Year == datum.Year && x.Datum.Month == datum.Month && x.Datum.Day == datum.Day))
            {
                foreach (var item in _db.StorageDB)
                {
                    try
                    {
                        //Uzimanje samo onih kompanija, koje imaju "historical finanace" na odabrani datum
                        var h = await YahooFinanceApi.Yahoo.GetHistoricalAsync(item.symbol, new DateTime(datum.Year, datum.Month, datum.Day), new DateTime(datum.Year, datum.Month, datum.Day).AddDays(1), Period.Daily);
                        foreach (var candle in h)
                        {
                            podaci.Add(new Storage 
                            {
                                OpenPrice=candle.Open,
                                ClosePrice=candle.Close,
                                Datum=candle.DateTime,
                                ID=item.ID
                            });
                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                for (int i = 0; i < podaci.Count; i++)
                {
                    FinanceStorageDB f = new FinanceStorageDB
                    {
                        OpenPrice = podaci[i].OpenPrice,
                        ClosePrice = podaci[i].ClosePrice,
                        Datum = podaci[i].Datum,
                        StorageDBId = podaci[i].ID
                    };
                    _db.FinanceStorageDB.Add(f);
                    _db.SaveChanges();
                }

            }

            //Dohvatanje podataka iz baze podataka
            StorageVM model = new StorageVM();

            model.podaci = _db.FinanceStorageDB
           .Include(i => i.StorageDB)
           .Where(i => i.Datum.Year == datum.Year && i.Datum.Month == datum.Month && i.Datum.Day == datum.Day)
          .Select(n => new StorageVM.Rows
          {
              symbol = n.StorageDB.symbol,
              CompanyName = n.StorageDB.CompanyName,
              numberOfEmployees = n.StorageDB.numberOfEmployees,
              City = n.StorageDB.City,
              State = n.StorageDB.State,
              MarketCap=n.StorageDB.MarketCap,
              ClosePrice=n.ClosePrice,
              OpenPrice=n.OpenPrice,
              Datum=n.Datum
          })
          .ToList();

            return View(model);
        }

        private async Task geListTickersAsync()
        {
            //Dohvatanje liste symbola i kompanija u EU regiji
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/get-trending-tickers?region=US"),
                Headers =
                    {
                        { "x-rapidapi-key", "59cd2485cemshf72578d1ef3ac2ap106076jsncc6280cf1332" },
                        { "x-rapidapi-host", "apidojo-yahoo-finance-v1.p.rapidapi.com" },
                    },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Root financije = JSONSerializerWrapper.Deserialize<Root>(body); //Convertovanje JSona u C#
                foreach (var item in financije.finance.result)
                {
                    foreach (var x in item.quotes)
                    {
                        //Dohvatanje detaljnih podataka za iste
                        var client2 = new HttpClient();
                        var request2 = new HttpRequestMessage
                        {
                            Method = HttpMethod.Get,
                            RequestUri = new Uri(@"https://apidojo-yahoo-finance-v1.p.rapidapi.com/stock/v2/get-summary?symbol=" + x.symbol + "&region=US"),
                            Headers =
    {
        { "x-rapidapi-key", "59cd2485cemshf72578d1ef3ac2ap106076jsncc6280cf1332" },
        { "x-rapidapi-host", "apidojo-yahoo-finance-v1.p.rapidapi.com" },
    },
                        };
                        System.Threading.Thread.Sleep(1000);
                        using (var response2 = await client2.SendAsync(request2))
                        {
                            response2.EnsureSuccessStatusCode();
                            var details = await response2.Content.ReadAsStringAsync();
                            Root2 financije2 = JSONSerializerWrapper.Deserialize<Root2>(details);
                            //Pohrana u bazu podataka
                            if (financije2.summaryProfile != null && financije2.price != null)
                            {
                                StorageDB s = new StorageDB()
                                {
                                    symbol = x.symbol,
                                    CompanyName = x.shortName,
                                    numberOfEmployees = financije2.summaryProfile.fullTimeEmployees,
                                    City = financije2.summaryProfile.city,
                                    State = financije2.summaryProfile.country,
                                    MarketCap = financije2.price.marketCap.longFmt
                                };
                                _db.StorageDB.Add(s);
                                _db.SaveChanges();
                            }
                        }
                    }
                }
               
          
                throw new NotImplementedException();
            }
        }
    }
}
