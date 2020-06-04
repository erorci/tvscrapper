using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TVShowCast.Core.Domain;
using TVShowCast.Infrastructure.Exceptions;
using TVShowCast.Infrastructure.Repository;

namespace TVShowCast.Presentation
{
    public class TVMazeScapper
    {
        private const string BASE_URL = "http://api.tvmaze.com";

        private readonly ILogger<TVMazeScapper> _logger;

        private readonly HttpClient _httpClient;

        private readonly TVMazeRepository _repository;

        public TVMazeScapper(ILogger<TVMazeScapper> logger, HttpClient httpClient, TVMazeRepository repository)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BASE_URL);
            _repository = repository;
        }

        public async void ExecuteAsync()
        {
            //TODO: Needs to implement a page import control, for now just clean everything before import
            await _repository.DeleteAllShows();

            const int PAGE_LIMIT = 99999;
            for (int page = 0; page < PAGE_LIMIT; page++)
            {
                IngestionStatus ingestStatus = await Ingest(page);
                switch (ingestStatus)
                {
                    case IngestionStatus.Success:
                        return;
                    case IngestionStatus.NoNewDataAvailable:
                        _logger.LogInformation("Finished processing all Shows");
                        return;
                    case IngestionStatus.Error:
                        break;
                }
            }
            _logger.LogError("Stopped ingestion as error count exceeded the limit of pages");
        }

        private async Task<IngestionStatus> Ingest(int page)
        {
            try
            {
                List<Show> shows = await GetJsonAsync<List<Show>>($"/shows?page={page}");
                List<ShowInformation> showsInformation = new List<ShowInformation>();
                await _repository.CreateShowsAsync(shows);           
                Parallel.ForEach(
                    shows,
                    new ParallelOptions { MaxDegreeOfParallelism = 5 },
                    async show =>
                    {
                        try
                        {
                            List<CastEntry> casts = await GetJsonAsync<List<CastEntry>>($"/shows/{show.Id}/cast");
                            showsInformation.Add(new ShowInformation(show, GetPersonsFromCasts(casts)));
                            await _repository.CreatePersonsAsync(GetPersonsFromCasts(casts));                            
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(exception, "Error when retrieving cast for show {0}", show.Id);
                        }                        
                        await Task.Delay(10000);
                });
                await _repository.CreateShowMetadataAsync(showsInformation);
                return IngestionStatus.Success;
            }
            catch (NotFoundException)
            {
                _logger.LogInformation("No Shows available for page {0}", page);
                return IngestionStatus.NoNewDataAvailable;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error when getting page {0} from the API", page);
                return IngestionStatus.Error;
            }
        }

        private async Task<T> GetJsonAsync<T>(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new NotFoundException();
                    default:
                        throw new ApiErrorException(response.StatusCode);
                }
            }

            string content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }

        private List<Person> GetPersonsFromCasts(List<CastEntry> casts)
        {
            return casts.Select(cast => cast.Person).ToList();
        }
    }
}
