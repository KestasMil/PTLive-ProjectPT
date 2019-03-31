using Newtonsoft.Json;
using PTLive.Fetcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PTLive
{
    public class PoeFetcher
    {
        //--------------
        //Events
        //--------------
        public event Action CompletedSuccessfully;
        public event Action CompletionFailed;
        public event Action UpdateCancelled;
        public event Action<int> UpdateProgressReport;


        //--------------
        //Private variable
        //--------------
        private CancellationTokenSource calcelationTokenSource;
        HttpClient client;
        private string leagueName;
        private string unformattedURL = "http://api.pathofexile.com/leagues/[league]/?ladder=1&ladderOffset={0}&ladderLimit={1}";
        //--------------
        //Properties
        //--------------
        public int PagingDelay { get; set; }
        public LeagueInfo Lf { get; private set; } = new LeagueInfo();
        public bool IsCurrentlyUpdating { get; private set; }
        public bool IsCurrentlyCanceling { get; private set; }

        //---------------
        //Public Methods
        //---------------
        public void CancelUpdate()
        {
            if (!this.IsCurrentlyCanceling && this.IsCurrentlyUpdating)
            {
                this.IsCurrentlyCanceling = true;
                this.calcelationTokenSource.CancelAfter(5000);
            }
        }

        public PoeFetcher(string leagueName, int pagingDelay)
        {
            // Format and set league name
            this.leagueName = leagueName;
            // Set league name in URL
            unformattedURL = unformattedURL.Replace("[league]", this.leagueName.Replace(' ', '+'));
            // Set delay between requesting different pages.
            this.PagingDelay = pagingDelay;
            // Create instance of http client
            this.client = new HttpClient();
            // Add an Accept header for JSON format.
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void InitUpdateData()
        {
            this.calcelationTokenSource = new CancellationTokenSource();
            // Do nothing if updating currently
            if (IsCurrentlyUpdating) return;

            // Set state
            IsCurrentlyUpdating = true;
            IsCurrentlyCanceling = false;

            //Create background task
            Task<LeagueInfo> t = new Task<LeagueInfo>((stateObj) =>
            {
                var castedToken = (CancellationToken)stateObj;
                const int maxPerPage = 200;
                LeagueInfo lf = new LeagueInfo();
                //Get header data
                HttpResponseMessage response = client.GetAsync(getFormatterUrl(0, maxPerPage)).Result;
                if (response.IsSuccessStatusCode)
                {
                    string jsonRawData = response.Content.ReadAsStringAsync().Result;
                    lf = JsonConvert.DeserializeObject<LeagueInfo>(jsonRawData);
                }
                else
                {
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    throw new Exception();
                }
                // Get remaining data
                for (int i = 200; i < lf.ladder.total; i += 200)
                {
                    if (castedToken.IsCancellationRequested)
                    {
                        response.Dispose();
                        castedToken.ThrowIfCancellationRequested();
                    }
                    //Sleep if explicitly requested.
                    Thread.Sleep(PagingDelay);
                    /*//Add implicit delay every 5 pages to prevent some errors that sometimes occure due to too many requests.
                    if (i > 0 && (Math.IEEERemainder(i/200, 5) == 0))
                    {
                        Console.WriteLine("Implicit Delay.");
                        Thread.Sleep(3000);
                    }*/

                    //Report progress trough event
                    UpdateProgressReport?.Invoke(i * 100 / lf.ladder.total);

                    // Get page data
                    response = client.GetAsync(getFormatterUrl(i, maxPerPage)).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string respon = response.Content.ReadAsStringAsync().Result;
                        LeagueInfo lfTemp = JsonConvert.DeserializeObject<LeagueInfo>(respon);
                        //append Entires (characters) to lf variable which holds all data.
                        lf.ladder.entries.AddRange(lfTemp.ladder.entries);
                    }
                    else
                    {
                        //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                        throw new Exception();
                    }
                }
                return lf;
            }, calcelationTokenSource.Token, calcelationTokenSource.Token);

            //Start task in the background
            t.Start();

            // Execute after completion of the task
            t.ContinueWith((task) => {
                IsCurrentlyUpdating = false;
                IsCurrentlyCanceling = false;
                if (task.IsCanceled)
                {
                    UpdateCancelled?.Invoke();
                }
                else if (!task.IsFaulted)
                {
                    Lf = task.Result;
                    CompletedSuccessfully?.Invoke();
                }
                else
                {
                    CompletionFailed?.Invoke();
                }
            });
        }

        //----------------
        //Private Methods
        //----------------
        private string getFormatterUrl(int ladderOffset, int ladderLimit)
        {
            return string.Format(this.unformattedURL, ladderOffset, ladderLimit);
        }
    }
}
