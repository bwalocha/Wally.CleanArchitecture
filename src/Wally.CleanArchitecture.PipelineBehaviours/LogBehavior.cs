using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wally.Lib.DDD.Abstractions.Requests;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.PipelineBehaviours
{
	public class LogBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest where TResponse : IResponse
	{
		private readonly ILogger<LogBehavior<TRequest, TResponse>> _logger;

		public LogBehavior(ILogger<LogBehavior<TRequest, TResponse>> logger)
		{
			_logger = logger;
		}

		public async Task<TResponse> Handle(
			TRequest request,
			CancellationToken cancellationToken,
			MediatR.RequestHandlerDelegate<TResponse> next)
		{
			var correlationId = Guid.NewGuid();
			_logger.LogInformation(
				$"[{correlationId}] Executing request handler for request type: '{typeof(TRequest).Name}' and response type: '{typeof(TResponse).Name}' '{JsonConvert.SerializeObject(request, Formatting.Indented)}'.");
			var stopWatch = Stopwatch.StartNew();
			try
			{
				return await next();
			}
			finally
			{
				stopWatch.Stop();
				_logger.LogInformation($"[{correlationId}] Executed in {stopWatch.ElapsedMilliseconds} ms.");
			}
		}
	}
}
