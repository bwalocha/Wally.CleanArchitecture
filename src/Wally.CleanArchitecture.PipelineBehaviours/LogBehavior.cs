using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
// using Newtonsoft.Json;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.PipelineBehaviours
{
	public class LogBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse> 
		where TRequest : notnull
		where TResponse : IResponse
	{
		// TODO: move static field to base abstract class
		/*private static readonly JsonSerializerSettings _jsonSettings = new()
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			Error = (se, ev) => { ev.ErrorContext.Handled = true; },
			MaxDepth = 10,
		};*/

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
			// _logger.LogInformation($"[{correlationId}] Executing request handler for request type: '{typeof(TRequest).Name}' and response type: '{typeof(TResponse).Name}' '{JsonConvert.SerializeObject(request, Formatting.Indented, _jsonSettings)}'."); // TODO: add JsonIgnore attribute for ODataOptions
			// https://stackoverflow.com/questions/56600156/simple-serialize-odataqueryoptions
			_logger.LogInformation($"[{correlationId}] Executing request handler for request type: '{typeof(TRequest).Name}' and response type: '{typeof(TResponse).Name}'.");
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
