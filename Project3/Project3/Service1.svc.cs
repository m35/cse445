using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Project3
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public decimal SolarIntensity(decimal latitude, decimal longitude)
        {
            const string URL_FORMAT = "https://developer.nrel.gov/api/solar/solar_resource/v1.json?api_key={0}&lat={1}&lon={2}";
            const string API_KEY = "OUbP3a7RftShy3Xrk0PBXgtKW3RH8OlUkxA4Ydvg";

            string latString = latitude.ToString("0.000");
            string lonString = longitude.ToString("0.000");

            try
            {
                Task<SolarResponse> x = JsonUtil.JasonAsync<SolarResponse>(URL_FORMAT, API_KEY, latString, lonString);
                return x.Result.outputs.avg_dni.annual;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public string WordFilter(string text)
        {
            bool isML = text.Contains("<html") || text.Contains("<?xml");
            string r5;
            if (isML)
            {
                string r1 = Regex.Replace(text, "<script.+?</script>", " ", RegexOptions.Singleline);
                string r2 = Regex.Replace(r1, "<style.+?</style>", " ", RegexOptions.Singleline);
                string r3 = Regex.Replace(r2, "<!--.*?-->", " ", RegexOptions.Singleline);
                string r4 = Regex.Replace(r3, "<[^>]+>", " ", RegexOptions.Singleline);
                r5 = HttpUtility.HtmlDecode(r4);
            }
            else
            {
                r5 = text;
            }
            string r6 = Regex.Replace(r5, @"\b(a|an|be|are|was|is|are|am|it|its|it's|do|did|has|had|or|and|but|as|like|at|to|in|out|on|off|this|that|of|for|the)\b", "", RegexOptions.IgnoreCase);
            string r7 = Regex.Replace(r6, @"\s\s+", " ");
            return r7.Trim();
        }

    }


    public class SolarResponse
    {
        public string version { get; set; }
        public string[] errors { get; set; }
        public string[] warnings { get; set; }
        public Dictionary<string, object> metadata { get; set; }
        public Dictionary<string, object> inputs { get; set; }
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
        public Dictionary<string, object> monthly { get; set; }
    }


    public class JsonUtil
    {

        public static async Task<T> JasonAsync<T>(string urlFormat, params string[] urlParams)
        {
            string[] escapedUrlParams = new string[urlParams.Length];
            for (int i = 0; i < urlParams.Length; i++)
            {
                escapedUrlParams[i] = Uri.EscapeDataString(urlParams[i]);
            }

            string url = String.Format(urlFormat, escapedUrlParams);

            using (HttpClient http = new HttpClient())
            {
                // Must set UserAgent or we get an error on GitHub
                // https://gist.github.com/BellaCode/c0ba0a842bbe22c9215e
                http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
                using (HttpResponseMessage client = await http.GetAsync(new Uri(url)))
                {
                    string resultJson = await client.Content.ReadAsStringAsync();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    T response = serializer.Deserialize<T>(resultJson);

                    return response;
                }
            }
        }
    }

}
