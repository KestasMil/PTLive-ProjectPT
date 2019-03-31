namespace PTLive.StatsGenerator
{
    public class AccountDeathsInfo
    {
        public string accName;
        public int deaths;
        public AccountDeathsInfo(string accName, int deaths)
        {
            this.accName = accName;
            this.deaths = deaths;
        }
    }
}