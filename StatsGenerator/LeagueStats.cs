using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLive.StatsGenerator
{
    public class LeagueStats
    {
        public int ActivePlayers;
        public int AccountsParticipating;
        public int[] DeadAtLevel = new int[101];
        public int TotalCharacters;
        public int DeadCharacters;
        public int AliveCharacters;
        public int CharsAliveAboveLvl10;
        public int CharsAliveAboveLvl50;
        public int DeadBeforeLvl10;
        public List<AccountDeathsInfo> accountableForMostDeaths = new List<AccountDeathsInfo>();
    }
}
