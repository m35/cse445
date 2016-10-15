using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Project3
{
    public class SolarResponse
    {
        public string version { get; set; }
        public string[] errors { get; set; }
        public string[] warnings { get; set; }
        public Dictionary<string, string> metadata { get; set; }
        public SolarOutput outputs { get; set; }
    }

    public class SolarOutput
    {
        public SolarOutputFields avg_dni { get; set; }
        public SolarOutputFields avg_ghi { get; set; }
        public SolarOutputFields avg_lat_tilt { get; set; }
    }

    public class SolarOutputFields
    {
        public decimal annual { get; set; }
        public SolarMonthly monthly { get; set; }
    }

    public class SolarMonthly
    {
        public decimal jan { get; set; }
        public decimal feb { get; set; }
        public decimal mar { get; set; }
        public decimal apr { get; set; }
        public decimal may { get; set; }
        public decimal jun { get; set; }
        public decimal jul { get; set; }
        public decimal aug { get; set; }
        public decimal sep { get; set; }
        public decimal oct { get; set; }
        public decimal nov { get; set; }
        public decimal dec { get; set; }
    }

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public decimal AnnualAverageSunshineIndex(decimal lat, decimal lon)
        {

            Task<decimal> x = AnnualAverageSunshineIndexAsync(lat, lon);
            return x.Result;
        }

        private async Task<decimal> AnnualAverageSunshineIndexAsync(decimal lat, decimal lon)
        {
            const string URL_FORMAT = "https://developer.nrel.gov/api/solar/solar_resource/v1.json?api_key={0}&lat={1}&lon={2}";
            const string API_KEY = "";

            string latString = lat.ToString("0.000");
            string lonString = lon.ToString("0.000");
            string url = String.Format(URL_FORMAT, Uri.EscapeDataString(API_KEY), Uri.EscapeDataString(latString), Uri.EscapeDataString(lonString));

            using (HttpClient http = new HttpClient())
            {
                using (HttpResponseMessage client = await http.GetAsync(new Uri(url)))
                {
                    string resultJson = await client.Content.ReadAsStringAsync();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    SolarResponse response = serializer.Deserialize<SolarResponse>(resultJson);
                    return response.outputs.avg_dni.annual;
                }
            }

        }


        public string WordFilter(string text)
        {
            return "";
        }

        public string[] EcoFriendlySoftware(int count)
        {
            return new string[]{""};
        }

        public string[] EcoFriendlyProducts(int count)
        {
            return new string[] { "" };
        }

    }
}
