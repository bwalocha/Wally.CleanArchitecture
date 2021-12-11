using MediatR;
using Microsoft.AspNetCore.OData.Query;
using Wally.CleanArchitecture.Contracts.Requests.User;
using Wally.CleanArchitecture.Contracts.Responses.Users;
using Wally.Lib.DDD.Abstractions.Queries;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.Application.Users.Queries
{
	public class GetUsersQuery : PagedQuery<GetUsersRequest, GetUsersResponse>, IRequest<PagedResponse<GetUsersResponse>>
	{
		public GetUsersQuery(ODataQueryOptions<GetUsersRequest> queryOptions) : base(queryOptions)
		{
		}
	}
}
