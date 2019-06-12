using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BlaziatR.Server
{
    internal class RequestHandlerWrapper<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IMediator _mediator;

        public RequestHandlerWrapper(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            return _mediator.Send(request, cancellationToken);
        }
    }
}