using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TVShowCast.Core.Domain;

namespace TVShowCast.Infrastructure.Repository
{
    public class TVMazeRepository
    {
        private readonly ConnectionSettings settings;

        public TVMazeRepository()
        {
            settings = ConnectionSettings.CreateBasicAuth("bolt://localhost:7687/neo4j/shows", "neo4j", "rambo12345");
        }

        public async Task CreatePersonsAsync(IList<Person> persons)
        {
            using(var client = new Neo4JClient(settings))
            {
                await client.CreatePersons(persons);
            }
        }

        public async Task CreateShowsAsync(IList<Show> shows)
        {
            using (var client = new Neo4JClient(settings))
            {
                await client.CreateShows(shows);
            }
        }

        public async Task CreateShowMetadataAsync(IList<ShowInformation> metadatas)
        {
            using (var client = new Neo4JClient(settings))
            {
                await client.CreateRelationships(metadatas);
            }
        }

        public async Task<IEnumerable<JObject>> GetShowsWithCastAsync()
        {
            using (var client = new Neo4JClient(settings))
            {
                return await client.GetShowsWithCast();
            }
        }

        public async Task DeleteAllShows()
        {
            using (var client = new Neo4JClient(settings))
            {
                await client.DeleteAllShows();
            }
        }
    }
}
