using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BlaziatR.Client
{
    public interface IBlaziator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, string address, CancellationToken cancellationToken = new CancellationToken());
    }
}