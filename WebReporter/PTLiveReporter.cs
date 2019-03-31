using PTLive.StatsGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using PTLive.Data;
using Newtonsoft.Json;

namespace PTLive.WebReporter
{
    public static class PTLiveReporter
    {
        private static int pin = 7878123;
        //public static readonly string url = "http://localhost/projectpt.live/scripts/";
        public static readonly string url = "http://projectpt.live/scripts/";

        public static string ReportMultipleAliveChars(List<string> accNames)
        {
            string strToSend = string.Join(",", accNames);

            WebClient wc = new WebClient();
            wc.QueryString.Add("pin", pin.ToString());
            wc.QueryString.Add("accounts", strToSend);

            var data = wc.UploadValues(url + "multiCharDetected.php", "POST", wc.QueryString);

            // data here is optional, in case we recieve any string data back from the POST request.
            var responseString = UnicodeEncoding.UTF8.GetString(data);

            return responseString;
        }

        public static void UpdateAscendancyClasses(List<ClassInfo> CI)
        {
            //ascendancy_by_popularity
            string strToSend = JsonConvert.SerializeObject(CI);

            WebClient wc = new WebClient();
            wc.QueryString.Add("pin", pin.ToString());
            wc.QueryString.Add("StatName", "ascendancy_by_popularity");
            wc.QueryString.Add("jsonData", strToSend);

            var data = wc.UploadValues(url + "statUpdate.php", "POST", wc.QueryString);
        }

        public static void UpdateLeagueStats(LeagueStats LS)
        {
            string strToSend = JsonConvert.SerializeObject(LS);

            WebClient wc = new WebClient();
            wc.QueryString.Add("pin", pin.ToString());
            wc.QueryString.Add("StatName", "league_stats");
            wc.QueryString.Add("jsonData", strToSend);

            var data = wc.UploadValues(url + "statUpdate.php", "POST", wc.QueryString);
        }

        public static void UpdateRipsCounter(int deaths)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    string htmlCode = client.DownloadString("http://51.141.96.202/PTStats/update.php?count=" + deaths);
                }
                catch (Exception) { }
            }
        }
    }
}
