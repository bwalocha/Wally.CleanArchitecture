using System;
using Microsoft.AspNetCore.Http;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft;

public class HttpRequestContext : IRequestContext
{
	public HttpRequestContext(IHttpContextAccessor httpContextAccessor)
	{
		CorrelationId = new CorrelationId(Guid.NewGuid());

		// TODO:
		/*httpContextAccessor.HttpContext.User.Identity.*/
		UserId = new UserId(Guid.Parse("FFFFFFFF-0000-0000-0000-ADD702D3016B"));
	}

	public CorrelationId CorrelationId { get; }
	public UserId UserId { get; }
}
