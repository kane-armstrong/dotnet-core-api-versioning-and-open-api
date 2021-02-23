using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WeatherForecastApi.Controllers.V2
{
    [ApiController]
    [ApiVersion("2")]
    [Route("v2/weatherforecast")]
    public class WeatherForecastV2Controller : ControllerBase
    {
        private static readonly string[] Summaries = {
            "freezing", "bracing", "chilly", "cool", "mild", "warm", "balmy", "hot", "sweltering", "scorching"
        };

        private const string CacheKey = "WeatherForecastV2Controller_db";

        private readonly IMemoryCache _memoryCache;
        private static readonly Random Rng = new Random();

        public WeatherForecastV2Controller(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        ///     Lists weather forecasts.
        /// </summary>
        /// <response code="200">An array consisting of zero or more weather forecasts.</response>
        /// <response code="401">If the request is not authorized.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<WeatherForecast>>> List()
        {
            return await LoadDatabase();
        }

        /// <summary>
        ///     Finds a weather forecast by its ID.
        /// </summary>
        /// <response code="200">The requested weather forecast, if it was found.</response>
        /// <response code="401">If the request is not authorized.</response>
        /// <response code="404">If the weather forecast was not found.</response>
        [HttpGet("{id}", Name = nameof(GetById))]
        [ProducesResponseType(typeof(WeatherForecast), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WeatherForecast>> GetById([FromRoute]Guid id)
        {
            if (id == default) return BadRequest();
            var db = await LoadDatabase();
            var item = db.FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody]CreateWeatherForecast weatherForecast)
        {
            var db = await LoadDatabase();
            var newWeatherForecast = new WeatherForecast
            {
                Id = Guid.NewGuid(),
                Summary = weatherForecast.Summary,
                Date = weatherForecast.Date,
                TemperatureC = weatherForecast.TemperatureC
            };
            db.Add(newWeatherForecast);
            SaveDatabase(db);
            const string route = nameof(GetById);
            return CreatedAtRoute(route, new { id = newWeatherForecast.Id, version = "2" }, null);
        }

        [HttpPut("{id:Guid}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit([FromRoute]Guid id, [FromBody]EditWeatherForecast weatherForecast)
        {
            if (id != weatherForecast.Id) return BadRequest();
            var db = await LoadDatabase();
            var item = db.FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            db.Remove(item);
            item.Date = weatherForecast.Date;
            item.Summary = weatherForecast.Summary;
            item.TemperatureC = weatherForecast.TemperatureC;
            db.Add(item);
            SaveDatabase(db);
            return NoContent();
        }

        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var db = await LoadDatabase();
            var item = db.FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            db.Remove(item);
            SaveDatabase(db);
            return NoContent();
        }

        private Task<List<WeatherForecast>> LoadDatabase()
        {
            return _memoryCache.GetOrCreateAsync(CacheKey, entry =>
            {
                var set = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Rng.Next(-20, 55),
                    Summary = Summaries[Rng.Next(Summaries.Length)]
                }).ToList();
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                return Task.FromResult(set);
            });
        }

        private void SaveDatabase(List<WeatherForecast> db)
        {
            _memoryCache.Set(CacheKey, db, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });
        }
    }
}