using MediatR;

namespace BlaziatR.Samples.App.Shared
{
    public class GetWeatherForecast : IRequest<WeatherForecast[]> { }
}