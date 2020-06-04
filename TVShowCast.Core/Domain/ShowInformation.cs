using Newtonsoft.Json;
using System.Collections.Generic;

namespace TVShowCast.Core.Domain
{
    public class ShowInformation
    {
        public ShowInformation(Show show, List<Person> persons)
        {
            Show = show;
            Cast = new List<Person>(persons);
        }

        [JsonProperty("show")]
        public Show Show { get; private set; }

        [JsonProperty("cast")]
        public List<Person> Cast { get; private set; }
    }
}
