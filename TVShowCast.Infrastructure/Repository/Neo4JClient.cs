using Neo4j.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVShowCast.Core.Domain;
using TVShowCast.Infrastructure.Exceptions;

namespace TVShowCast.Infrastructure.Repository
{
    public class Neo4JClient : IDisposable
    {
        private readonly IDriver driver;

        public Neo4JClient(IConnectionSettings settings)
        {
            this.driver = GraphDatabase.Driver(settings.Uri, settings.AuthToken);
        }

        public async Task CreatePersons(IList<Person> persons)
        {
            string cypher = new StringBuilder()
                .AppendLine("UNWIND $persons AS person")
                .AppendLine("MERGE (p:Person {name: person.name})")
                .AppendLine("SET p = person")
                .ToString();

            var session = driver.AsyncSession();
            try
            {
                await session.RunAsync(cypher, new Dictionary<string, object>() { { "persons", ParameterSerializer.ToDictionary(persons) } });
            }
            catch(InvalidOperationException)
            {
                throw new RepositoryException();
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task CreateShows(IList<Show> shows)
        {
            string cypher = new StringBuilder()
                .AppendLine("UNWIND $shows AS show")
                .AppendLine("MERGE (s:Show {id: show.id})")
                .AppendLine("SET s = show")
                .ToString();

            var session = driver.AsyncSession();
            try
            {
                await session.RunAsync(cypher, new Dictionary<string, object>() { { "shows", ParameterSerializer.ToDictionary(shows) } });

            }
            catch(InvalidOperationException)
            {
                throw new RepositoryException();
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task CreateRelationships(IList<ShowInformation> metadatas)
        {
            string cypher = new StringBuilder()
                .AppendLine("UNWIND $metadatas AS metadata")
                 // Find the Movie:
                 .AppendLine("MATCH (m:Show { name: metadata.show.name })")
                 // Create Cast Relationships:
                 .AppendLine("UNWIND metadata.cast AS actor")
                 .AppendLine("MATCH (a:Person { name: actor.name })")
                 .AppendLine("MERGE (a)-[r:ACTED_IN]->(m)")
                .ToString();

            var session = driver.AsyncSession();
            try
            {
                await session.RunAsync(cypher, new Dictionary<string, object>() { { "metadatas", ParameterSerializer.ToDictionary(metadatas) } });

            }
            catch (InvalidOperationException)
            {
                throw new RepositoryException();
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<IEnumerable<JObject>> GetShowsWithCast()
        {
            string cypher = new StringBuilder()
                .AppendLine("MATCH (show:Show)<-[:ACTED_IN]-(person:Person)")
                .AppendLine("WITH show, person")
                .AppendLine("ORDER BY show.id, person.birthday")
                .AppendLine("WITH show, collect(person) AS cast")
                .AppendLine("RETURN show, cast")
                .ToString();

            var session = driver.AsyncSession();
            try
            {
                return await session.ReadTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(cypher);
                    var res = await result.ToListAsync();
                    string values = JsonConvert.SerializeObject(res.Select(x => x.Values), Formatting.Indented);
                    List<JObject> nodes = JsonConvert.DeserializeObject<List<JObject>>(values);
                    return nodes;
                });
            }
            catch (InvalidOperationException)
             {
                 throw new RepositoryException();
             }
             finally
             {
                 await session.CloseAsync();
             }
        }

        public async Task DeleteAllShows()
        {
            string cypher = new StringBuilder()
                .AppendLine("MATCH (all) DETACH DELETE all")
                .ToString();

            var session = driver.AsyncSession();
            try
            {
                await session.RunAsync(cypher);
                await Task.Delay(5000);
            }
            catch (InvalidOperationException)
            {
                throw new RepositoryException();
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public void Dispose()
        {
            driver?.Dispose();
        }
    }
}
