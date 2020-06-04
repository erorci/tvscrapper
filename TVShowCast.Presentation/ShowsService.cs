using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TVShowCast.Core.Domain;
using TVShowCast.Infrastructure.Repository;
using TVShowCast.Presentation.Controllers;
using TVShowCast.Presentation.Mapping;

namespace TVShowCast.Presentation
{
    public class ShowsService
    {
        private readonly ILogger<ShowsService> _logger;
        private readonly TVMazeRepository _repository;

        public ShowsService(ILogger<ShowsService> logger, TVMazeRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<PagedList<ShowDTO>> GetShowsWithCastByBirthdayAsync(ShowParameters showParameters)
        {
            _logger.LogInformation("Querying shows with cast order by Birthday");

            IEnumerable<JObject> showsRawObject = await _repository.GetShowsWithCastAsync();
            List<ShowDTO> showsWithCast = new List<ShowDTO>();
            foreach(JObject obj in showsRawObject)
            {
                Show show = (Show) obj.SelectToken("show.Properties").ToObject(typeof(Show));
                List<JToken> castObject = obj.SelectToken("cast").ToList();
                List<Person> cast = new List<Person>();
                
                castObject.ForEach(item =>
                {
                    Person p = (Person) item.SelectToken("Properties").ToObject(typeof(Person));
                    cast.Add(p);
                });

                showsWithCast.Add(
                    new ShowDTO { Id = show.Id, 
                        Name = show.Name, 
                        Cast = new List<Person>(cast) });
            }
            
            return PagedList<ShowDTO>.ToPagedList(showsWithCast.AsQueryable(), 
                showParameters.PageNumber,
                showParameters.PageSize);
        }
    }
}
