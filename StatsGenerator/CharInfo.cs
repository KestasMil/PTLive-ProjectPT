using PTLive.Fetcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLive.StatsGenerator
{
    public class CharInfo : Character
    {
        public bool alive;
        public CharInfo(Character c, bool alive)
        {
            this.charClass = c.charClass;
            this.depth = c.depth;
            this.experience = c.experience;
            this.name = c.name;
            this.level = c.level;
            this.alive = alive;
        }
    }
}
