using System.Collections.Generic;
using TVShowCast.Core.Domain;

namespace TVShowCast.Presentation.Mapping
{
    public class ShowDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Person> Cast { get; set; } = new List<Person>();
    }
}
