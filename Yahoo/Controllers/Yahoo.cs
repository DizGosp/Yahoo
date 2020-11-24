using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

            //Dohvatanje liste symbola i kompanija u EU regiji
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/v2/get-summary"),
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
                List<Storage> podaci = new List<Storage>();
                foreach (var item in financije.marketSummaryAndSparkResponse.result)
                {
                    try
                    {
                        //Uzimanje samo onih kompanija, koje imaju "historical finanace" na odabrani datum
                        var h = await YahooFinanceApi.Yahoo.GetHistoricalAsync(item.symbol, new DateTime(datum.Year, datum.Month, datum.Day), new DateTime(datum.Year, datum.Month, datum.Day).AddDays(1), Period.Daily);
                        foreach (var candle in h)
                        {
                            podaci.Add(new Storage()
                            {
                                symbol = item.symbol,
                                CompanyName = item.shortName,
                                OpenPrice = candle.Open,
                                ClosePrice = candle.Close,
                                Datum = candle.DateTime
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                }

                //Dohvatanje detaljnih podataka za novonastalu listu
                if (podaci != null)
                {
                    for (int i = 0; i < podaci.Count; i++)
                    {
                        var client2 = new HttpClient();
                        var request2 = new HttpRequestMessage
                        {
                            Method = HttpMethod.Get,
                            RequestUri = new Uri("https://apidojo-yahoo-finance-v1.p.rapidapi.com/stock/v2/get-summary?symbol=AMRN&region=US"),
                            Headers =
            {
                { "x-rapidapi-key", "59cd2485cemshf72578d1ef3ac2ap106076jsncc6280cf1332" },
                { "x-rapidapi-host", "apidojo-yahoo-finance-v1.p.rapidapi.com" },
            },
                        };

                        System.Threading.Thread.Sleep(1000);   //Pauza za zahtjev od 1 sekunde

                        using (var response2 = await client.SendAsync(request2))
                        {
                            response2.EnsureSuccessStatusCode();
                            var details = await response2.Content.ReadAsStringAsync();
                            Root2 financije2 = JSONSerializerWrapper.Deserialize<Root2>(details);
                            podaci[i].City = financije2.summaryProfile.city;
                            podaci[i].State = financije2.summaryProfile.country;
                            podaci[i].numberOfEmployees = financije2.summaryProfile.fullTimeEmployees;
                            podaci[i].MarketCap = financije2.price.marketCap.longFmt;
                        }
                    }

                    //Pohrana u bazu podataka
                    for (int i = 0; i < podaci.Count; i++)
                    {
                        StorageDB s = new StorageDB
                        {
                            symbol = podaci[i].symbol,
                            CompanyName = podaci[i].CompanyName,
                            numberOfEmployees = podaci[i].numberOfEmployees,
                            City = podaci[i].City,
                            State = podaci[i].State,
                            OpenPrice = podaci[i].OpenPrice,
                            ClosePrice = podaci[i].ClosePrice,
                            MarketCap = podaci[i].MarketCap,
                            Datum = podaci[i].Datum
                        };

                        _db.StorageDB.Add(s);
                        _db.SaveChanges();
                    }

                    //Dohvatanje podataka iz baze podataka
                    StorageVM model = new StorageVM();

                    model.podaci = _db.StorageDB
                        .Where(i=>i.Datum.Year==datum.Year && i.Datum.Month==datum.Month && i.Datum.Day==datum.Day)
                  .Select(n => new StorageVM.Rows
                  {
                      symbol = n.symbol,
                      CompanyName = n.CompanyName,
                      numberOfEmployees = n.numberOfEmployees,
                      City = n.City,
                      State = n.State,
                      OpenPrice = n.OpenPrice,
                      ClosePrice = n.ClosePrice,
                      MarketCap = n.MarketCap,
                      Datum = n.Datum
                  })
                  .ToList();
                    return View(model);


                }

                return View();


            }

        }
    }
}
