using PTLive.Fetcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLive.StatsGenerator
{
    public class AccountInfo
    {
        public string accName;
        public List<CharInfo> characters = new List<CharInfo>();

        public int AliveCharacters
        {
            get
            {
                int count = 0;
                foreach (CharInfo chr in characters)
                {
                    if (chr.alive) count++;
                }
                return count;
            }
        }

        public int DeadCharacters
        {
            get
            {
                int count = 0;
                foreach (CharInfo chr in characters)
                {
                    if (!chr.alive) count++;
                }
                return count;
            }
        }

        public AccountInfo(string accName)
        {
            this.accName = accName;
        }
    }
}
