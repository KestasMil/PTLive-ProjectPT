using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLive.Fetcher
{
    public class LeagueInfo
    {
        public string id;
        public string description;
        public string startAt;
        public string endAt;
        public Ladder ladder;
    }
}
