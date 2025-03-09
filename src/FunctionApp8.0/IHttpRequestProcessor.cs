using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CleanArchitecture.Presentation.FunctionApp8
{
    public interface IHttpRequestProcessor
    {
        Task<HttpResponseData> ExecuteAsync<TRequest, TResponse>(FunctionContext executionContext, 
                                                                                HttpRequestData httpRequest,
                                                                                TRequest request, 
                                                                                Func<TResponse, Task<HttpResponseData>> resultMethod )
                                                                                where TRequest : IRequest<TResponse>;
    }
}
