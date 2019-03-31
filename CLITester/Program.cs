using PTLive;
using PTLive.Fetcher;
using PTLive.PoeJSONFileManager;
using PTLive.StatsGenerator;
using PTLive.WebReporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CLITester
{
    class Program
    {
        //static PoeFetcher pf = new PoeFetcher("Get Fucked Again (PL3226)", 0);
        static PoeFetcher pf = new PoeFetcher("Actual Hardcore No Lube All Mods (PL3419)", 0);
        static Timer timer = new Timer();
        static int updateIntervalAfterCompletion = 60000*8;
        static int UpdatesCount = 0;

        static void Main(string[] args)
        {
            // Assigning event callbacks to PF object.
            pf.UpdateProgressReport += (rep) => { Console.WriteLine(rep + "%"); };
            pf.CompletedSuccessfully += Pf_CompletedSuccessfully;
            pf.CompletionFailed += Pf_CompletionFailed;

            // Start Updating.
            Console.WriteLine("Initiating update for the first time.");
            pf.InitUpdateData();

            // Assign method to be called by timer.
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;

            // For preventing the app from closing.
            Console.Read();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Initiating update.");
            pf.InitUpdateData();
        }

        private static void Pf_CompletionFailed()
        {
            pf.PagingDelay += 1000;
            timer.Interval = updateIntervalAfterCompletion/4;
            Console.WriteLine("Updating at " + DateTime.Now.AddMilliseconds(updateIntervalAfterCompletion/4) + ".");
            timer.Start();
            Console.WriteLine("Update Failed.");
        }

        private static void Pf_CompletedSuccessfully()
        {
            Console.WriteLine("Update completed Sucessfully.");

            // Set update delay and restart timer.
            Console.WriteLine("Updating at " + DateTime.Now.AddMilliseconds(updateIntervalAfterCompletion) + ".");
            pf.PagingDelay = 0;
            timer.Interval = updateIntervalAfterCompletion;
            timer.Start();

            //If ladder empty do nothing
            if (pf.Lf.ladder.entries.Count() == 0) return;

            // Save data to file.
            PoeFilesControl.SaveToFile(pf.Lf);

            // Get different data objects for statistics/reporting.
            LeagueInfo data = pf.Lf;
            List<AccountInfo> accInfo = StatsGenerator.GetAccountsInfo(data);
            List<string> namesList = StatsGenerator.ListOfAccMultipleCharsAlive(accInfo);

            // Report owners of multiple alive characters.
            PTLiveReporter.ReportMultipleAliveChars(namesList);
            Console.WriteLine("Accounts with multiple alive characters reported to web service.");

            // Get classes info.
            List<ClassInfo> x = StatsGenerator.GetClassesInfo(data);
            // Sort by popularity.
            List<ClassInfo> orderedByPopularity = x.OrderByDescending(d => d.totalChars).ToList<ClassInfo>();
            // Update ascendacy classes info
            PTLiveReporter.UpdateAscendancyClasses(orderedByPopularity);
            Console.WriteLine("Ascendancy classes info sent to server.");

            // Get league statistics.
            LeagueStats LS = StatsGenerator.GenerateLeagueStats(pf.Lf, (3600000 / updateIntervalAfterCompletion)+2);
            // Send league statistics
            PTLiveReporter.UpdateLeagueStats(LS);
            Console.WriteLine("General league stats sent to server.");

            // Update rips counter
            PTLiveReporter.UpdateRipsCounter(LS.DeadCharacters);
            Console.WriteLine("Rips Counter updated.");

            // Increase Updates Counter
            UpdatesCount++;
        }
    }
}
