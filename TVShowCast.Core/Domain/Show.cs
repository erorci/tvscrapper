using Newtonsoft.Json;

namespace TVShowCast.Core.Domain
{
    public class Show
    {
        public Show(int id, string name)
        {
            Id = id;
            Name = name;
        }
        
        [JsonProperty("id")]
        public int Id { get; }

        [JsonProperty("name")]
        public string Name { get; }
    }
}
