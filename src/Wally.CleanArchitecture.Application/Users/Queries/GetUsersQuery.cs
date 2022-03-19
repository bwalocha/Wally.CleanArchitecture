using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.OData.Query;

using Wally.CleanArchitecture.Contracts.Requests.User;
using Wally.CleanArchitecture.Contracts.Responses.Users;
using Wally.Lib.DDD.Abstractions.Queries;

namespace Wally.CleanArchitecture.Application.Users.Queries;

[ExcludeFromCodeCoverage]
public class GetUsersQuery : PagedQuery<GetUsersRequest, GetUsersResponse>
{
	public GetUsersQuery(ODataQueryOptions<GetUsersRequest> queryOptions)
		: base(queryOptions)
	{
	}
}
