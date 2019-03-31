using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLive.Fetcher
{
    public class Character
    {
        public string name;
        public int level;
        [JsonProperty(PropertyName = "class")]
        public string charClass;
        public uint experience;
        public Depth depth;
    }
}
