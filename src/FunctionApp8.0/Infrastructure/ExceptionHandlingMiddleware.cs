// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CleanArchitecture8.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Presentation.FunctionApp8
{
    /// <summary>
    /// This middleware catches any exceptions during function invocations and
    /// returns a response with 500 status code for http invocations.
    /// </summary>
    internal sealed class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing invocation");

                var httpReqData = await context.GetHttpRequestDataAsync();

                if (httpReqData != null)
                {
                    // Create an instance of HttpResponseData with 500 status code.
                    var newHttpResponse = httpReqData.CreateResponse(HttpStatusCode.InternalServerError);

                    var exception = (ValidationException)ex;

                    await newHttpResponse.WriteAsJsonAsync(new ValidationProblemDetails(exception.Errors)
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                    });

                    var invocationResult = context.GetInvocationResult();

                    var httpOutputBindingFromMultipleOutputBindings = GetHttpOutputBindingFromMultipleOutputBinding(context);
                    if (httpOutputBindingFromMultipleOutputBindings is not null)
                    {
                        httpOutputBindingFromMultipleOutputBindings.Value = newHttpResponse;
                    }
                    else
                    {
                        invocationResult.Value = newHttpResponse;
                    }
                }
            }
        }

        private static OutputBindingData<HttpResponseData>? GetHttpOutputBindingFromMultipleOutputBinding(FunctionContext context)
        {
            // The output binding entry name will be "$return" only when the function return type is HttpResponseData
            var httpOutputBinding = context.GetOutputBindings<HttpResponseData>()
                .FirstOrDefault(b => b.BindingType == "http" && b.Name != "$return");

            return httpOutputBinding;
        }
    }
}
