using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BlaziatR.Client
{
    public class Blaziator : IBlaziator
    {
        private readonly HttpClient _httpClient;

        public Blaziator(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, string address, CancellationToken cancellationToken = new CancellationToken())
        {
            var json = JsonSerializer.ToString(request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, address)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            httpRequest.Headers.Add("__BlaziatrRequestType", request.GetType().AssemblyQualifiedName);

            var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);

            httpResponse.EnsureSuccessStatusCode();

            var bytes = await httpResponse.Content.ReadAsByteArrayAsync();

            return JsonSerializer.Parse<TResponse>(bytes);
        }
    }
}
