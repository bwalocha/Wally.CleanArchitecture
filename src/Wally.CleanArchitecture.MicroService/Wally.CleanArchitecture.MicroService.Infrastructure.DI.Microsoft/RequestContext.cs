using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft;

public class RequestContext : IRequestContext
{
	private const string CorrelationIdKey = "x-correlation-id";
	
	public RequestContext(IHttpContextAccessor httpContextAccessor)
	{
		if (httpContextAccessor.HttpContext == null)
		{
			return;
		}

		CorrelationId = httpContextAccessor.HttpContext.Request.Headers.TryGetValue(
				CorrelationIdKey,
				out var correlationIdHeader)
		&& Guid.TryParse(correlationIdHeader.SingleOrDefault(), out var correlationId)
				? new CorrelationId(correlationId)
				: new CorrelationId(Guid.NewGuid());
		
		// TODO:
		/*httpContextAccessor.HttpContext.User.Identity.*/
		UserId = new UserId(Guid.Parse("FFFFFFFF-0000-0000-0000-ADD702D3016B"));
	}

	public CorrelationId CorrelationId { get; private set; }
	public UserId UserId { get; private set; }
	
	public IRequestContext SetCorrelationId(CorrelationId correlationId)
	{
		CorrelationId = correlationId;

		return this;
	}
	
	public IRequestContext SetUserId(UserId userId)
	{
		UserId = userId;

		return this;
	}
}
