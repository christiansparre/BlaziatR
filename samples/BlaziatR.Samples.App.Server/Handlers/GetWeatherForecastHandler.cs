using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlaziatR.Samples.App.Shared;
using MediatR;

namespace BlaziatR.Samples.App.Server.Handlers
{
    public class GetWeatherForecastHandler : IRequestHandler<GetWeatherForecast, WeatherForecast[]>
    {
        public Task<WeatherForecast[]> Handle(GetWeatherForecast request, CancellationToken cancellationToken)
        {
            var rng = new Random();
            var weatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
            return Task.FromResult(weatherForecasts.ToArray());
        }

        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


    }
}