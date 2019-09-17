using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
            var json = JsonSerializer.Serialize(request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, address)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            httpRequest.Headers.Add("__BlaziatrRequestType", request.GetType().AssemblyQualifiedName);

            var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);

            httpResponse.EnsureSuccessStatusCode();

            var stream = await httpResponse.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<TResponse>(stream);
        }
    }
}
