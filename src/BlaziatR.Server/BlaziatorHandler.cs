using System;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BlaziatR.Server
{
    public static class BlaziatorHandler
    {
        public static async Task Handle(HttpContext context)
        {
            var mediator = context.RequestServices.GetRequiredService<IMediator>();

            var requestType = Type.GetType(context.Request.Headers["__BlaziatrRequestType"]);

            if (requestType == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var implements = requestType.GetInterfaces().FirstOrDefault(f => f.GetGenericTypeDefinition() == typeof(IRequest<>));

            if (implements == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var responseType = implements.GetGenericArguments()[0];

            var wrapperType = typeof(RequestHandlerWrapper<,>).MakeGenericType(requestType, responseType);

            // If we got this long.. yay!

            var request = await JsonSerializer.ReadAsync(context.Request.Body, requestType);

            var wrapper = Activator.CreateInstance(wrapperType, mediator);

            var handleMethod = wrapperType.GetMethod("Handle", new[] { requestType, typeof(CancellationToken) });

            if (handleMethod == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var task = (Task)handleMethod.Invoke(wrapper, new[] { request, context.RequestAborted });

            await task;

            var response = ((dynamic)task).Result;

            var bytes = (byte[])JsonSerializer.ToUtf8Bytes(response, responseType);

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length, context.RequestAborted);
        }
    }
}
