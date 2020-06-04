using Newtonsoft.Json;
using System;

namespace TVShowCast.Core.Domain
{
    public class Person
    {
        public Person(int id, string name, DateTime? birthday)
        {
            Id = id;
            Name = name;
            Birthday = birthday;
        }

        [JsonProperty("id")]
        public int Id { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("birthday")]
        public DateTime? Birthday { get; }
    }
}
