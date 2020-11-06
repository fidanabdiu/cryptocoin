using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using TestSolution.Api.Data.Entities;
using TestSolution.Api.Helpers;

namespace TestSolution.Api.Data
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/working-with-sql?view=aspnetcore-3.1&tabs=visual-studio
    /// </summary>
    public static class Initializer
    {
        /// <summary>
        /// https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/working-with-sql?view=aspnetcore-3.1&tabs=visual-studio
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
            using (var applicationDbContext = new ApplicationDbContext(options))
            {
                applicationDbContext.Database.EnsureCreated();
                if (applicationDbContext.User.Count() == 0)
                {
                    applicationDbContext.User.Add(new User()
                    {
                        Id = 0,
                        Username = "administrator",
                        Password = Utilities.GetSHA256("password"),
                        Name = "System",
                        Surname = "Administrator",
                        Email = "info@company.com"
                    });
                    applicationDbContext.SaveChanges();
                }
                if (applicationDbContext.CryptoCoin.Count() == 0)
                {
                    using (var httpClient = new HttpClient())
                    {
                        var response = httpClient.GetAsync("https://api.coincap.io/v2/assets").Result;
                        var json = response.Content.ReadAsStringAsync().Result;
                        var model = JsonConvert.DeserializeObject<Root>(json);
                        foreach (var item in model.data)
                        {
                            applicationDbContext.CryptoCoin.Add(new CryptoCoin()
                            {
                                Id = 0,
                                Code = item.id,
                                Rank = item.rank,
                                Symbol = item.symbol,
                                Name = item.name,
                                Supply = item.supply,
                                MaxSupply = item.maxSupply
                            });
                        }
                        applicationDbContext.SaveChanges();
                    }
                }
            }
        }
    }

    /// <summary>
    /// https://json2csharp.com/
    /// </summary>
    public class Datum
    {
        public string id { get; set; }
        public string rank { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string supply { get; set; }
        public string maxSupply { get; set; }
        public string marketCapUsd { get; set; }
        public string volumeUsd24Hr { get; set; }
        public string priceUsd { get; set; }
        public string changePercent24Hr { get; set; }
        public string vwap24Hr { get; set; }
    }

    /// <summary>
    /// https://json2csharp.com/
    /// </summary>
    public class Root
    {
        public List<Datum> data { get; set; }
        public long timestamp { get; set; }
    }
}
