using System;

namespace WeatherForecastApi.NSwag.Controllers.V2
{
    public class CreateWeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
    }
}
