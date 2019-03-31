using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLive.StatsGenerator
{
    public class ClassInfo
    {
        public bool ascendedClass;
        public string charClass;
        public int totalChars;
        public int aliveChars;
        public uint highestExperienceReached;
        public int highestLevelCurrentlyAlive;
        public string highestAliveAccountName;
        public int highestLevelReached;

        public ClassInfo(string charClass, bool ascended)
        {
            this.charClass = charClass;
            this.ascendedClass = ascended;
        }
    }
}
