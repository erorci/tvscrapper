using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TVShowCast.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly ShowsService _showsService;
        private readonly ILogger<ShowsController> _logger;

        public ShowsController(ShowsService showsService, ILogger<ShowsController> logger)
        {
            _showsService = showsService;
            _logger = logger;
        }

        // GET api/shows
        [HttpGet]
        public async Task<IActionResult> GetShowsWithCast([FromQuery] ShowParameters showParameters)
        {
            var shows = await _showsService.GetShowsWithCastByBirthdayAsync(showParameters);

            var metadata = new
            {
                shows.TotalCount,
                shows.PageSize,
                shows.CurrentPage,
                shows.TotalPages,
                shows.HasNext,
                shows.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            _logger.LogInformation($"Returned {shows.TotalCount} shows from database.");

            return Ok(shows);
        }
    }
}
