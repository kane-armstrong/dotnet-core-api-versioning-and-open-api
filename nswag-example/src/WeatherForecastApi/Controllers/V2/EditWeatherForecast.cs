﻿using System;

namespace WeatherForecastApi.Controllers.V2
{
    public class EditWeatherForecast
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
    }
}
