using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Wrappers;
using Microsoft.Extensions.DependencyInjection;

namespace Silk.Utilities;

public class ScopingMediator : IMediator
{
    private readonly        ServiceFactory                                 _serviceFactory;
    private static readonly ConcurrentDictionary<Type, RequestHandlerBase> _requestHandlers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceFactory">The single instance factory.</param>
    public ScopingMediator(ServiceFactory serviceFactory)
        => _serviceFactory = serviceFactory;

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();

        var handler = (RequestHandlerWrapper<TResponse>)_requestHandlers.GetOrAdd
            (
             requestType,
             static t => (RequestHandlerBase)(Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(t, typeof(TResponse))) ?? throw new InvalidOperationException($"Could not create wrapper type for {t}"))
            );

        await using var scope = _serviceFactory.GetInstance<IServiceProvider>().CreateAsyncScope();
        
        return await handler.Handle(request, cancellationToken, scope.ServiceProvider.GetService!);
    }
    
    
    public async Task<object?>               Send(object                                       request, CancellationToken cancellationToken = new CancellationToken()) => null;
    public       IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken()) => null;
    public       IAsyncEnumerable<object?>   CreateStream(object                               request, CancellationToken cancellationToken = new CancellationToken()) => null;
    public async Task Publish(object                       notification, CancellationToken cancellationToken = new CancellationToken())                                     { }
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = new CancellationToken()) where TNotification : INotification { }
}