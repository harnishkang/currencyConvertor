using currencyConvertor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace currencyConvertor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            
            var model = new currencyList()
            {
                ListofCurrency = GetAvailableCurrency(),
            };
            model.conversionDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");

            return View(model);
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult check(string fromCurrency, string toCurrency, double amount,string convDate)
        {

            string showVal = "Converted From : "+fromCurrency+"\nConverted To : "+toCurrency+"\nDated : "+convDate+"\nConverted Amount : {0,14:N4}";
                string dateval = convDate;
                decimal convertedVal = 0;
                // If currency's are empty abort
                if (fromCurrency == null || toCurrency == null)
                    TempData["convertedValue"] = amount;
                // Convert CAD to CAD
                else if (fromCurrency.ToLower() == "cad" && toCurrency.ToLower() == "cad")
                    TempData["convertedValue"] = string.Format(showVal, amount);
                else
                {
                    try
                    {
                        // First Get the exchange rate of both currencies in CAD
                        float toRate = GetCurrencyRateInCAD(toCurrency, dateval);
                        float fromRate = GetCurrencyRateInCAD(fromCurrency, dateval);

                        // Convert Between CAD to Other Currency
                        if (fromCurrency.ToLower() == "cad")
                        {
                            
                            convertedVal = Convert.ToDecimal((amount / toRate));
                        }
                        else if (toCurrency.ToLower() == "cad")
                        {
                            
                            convertedVal = Convert.ToDecimal((amount * fromRate));
                        }
                        else
                        {
                            
                            convertedVal = Convert.ToDecimal((amount * toRate) / fromRate);
                        }
                    }
                    catch { TempData["convertedValue"] = string.Format(showVal, convertedVal); }
                }

                TempData["convertedValue"] = string.Format(showVal, convertedVal);
           
            return RedirectToAction("Index");
        }

        public static float GetCurrencyRateInCAD(string currency, string dateval)
        {
            if (currency.ToLower() == "")
                throw new ArgumentException("Invalid Argument! currency parameter cannot be empty!");
            //if (currency.ToLower() == "cad")
            //    throw new ArgumentException("Invalid Argument! Cannot get exchange rate from CAD to CAD");

            try
            {
           
            
                string rssUrl = string.Concat("https://www.bankofcanada.ca/valet/observations/FX"+currency.ToUpper()+"CAD/json?start_date="+ dateval + "&end_date="+ dateval);

                
                using (WebClient wc = new WebClient())
                {
                    var json = wc.DownloadString(rssUrl);
                    dynamic data = JObject.Parse(json);
                    string json2 = data["observations"][0]["FX" + currency.ToUpper() + "CAD"]["v"];
                    CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    ci.NumberFormat.CurrencyDecimalSeparator = ".";
                    float exchangeRate = float.Parse(
                           json2,
                            NumberStyles.Any,
                            ci);

                    return exchangeRate;
                }


                // currency not parsed!! 
                // return default value
                return 0;
            }
            catch
            {
                // currency not parsed!! 
                // return default value
                return 0;
            }
        }


        public List<SelectListItem> GetAvailableCurrency()
        {
            List<SelectListItem> availableCurrency = new List<SelectListItem>();

            string[] availCurr = new string[] { "CAD","USD", "EUR", "JPY", "GBP", "AUD", "CHF", "CNY", "HKD", "MXN", "INR" };
            int i = 0;
            while(i < availCurr.Length)
            {
                availableCurrency.Add(new SelectListItem
                { Text = availCurr[i], Value = availCurr[i] });

                
                i++;
            }
            
            return availableCurrency;
        }
    }
}