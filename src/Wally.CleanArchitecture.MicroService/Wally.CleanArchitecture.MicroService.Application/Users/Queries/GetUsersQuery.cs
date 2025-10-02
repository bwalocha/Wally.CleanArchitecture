using Microsoft.AspNetCore.OData.Query;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Application.Users.Requests;
using Wally.CleanArchitecture.MicroService.Application.Users.Results;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Queries;

[ExcludeFromCodeCoverage]
public sealed class GetUsersQuery : PagedQuery<GetUsersRequest, GetUsersResult>
{
	public GetUsersQuery(ODataQueryOptions<GetUsersRequest> queryOptions)
		: base(queryOptions)
	{
	}
}
