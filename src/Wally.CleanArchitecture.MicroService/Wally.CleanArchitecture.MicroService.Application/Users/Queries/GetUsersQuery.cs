using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.OData.Query;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Requests;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Responses;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Queries;

[ExcludeFromCodeCoverage]
public sealed class GetUsersQuery : PagedQuery<GetUsersRequest, GetUsersResponse>
{
	public GetUsersQuery(ODataQueryOptions<GetUsersRequest> queryOptions)
		: base(queryOptions)
	{
	}
}
