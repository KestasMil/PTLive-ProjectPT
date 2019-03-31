using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLive.Fetcher
{
    public class Depth
    {
        [JsonProperty(PropertyName = "default")]
        public int dflt;
        public int solo;
    }
}
