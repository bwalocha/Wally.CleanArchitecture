using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wally.CleanArchitecture.Application.Users.Commands;
using Wally.CleanArchitecture.Application.Users.Queries;
using Wally.CleanArchitecture.Contracts.Requests.User;
using Wally.CleanArchitecture.Contracts.Responses.Users;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.WebApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class Users : ControllerBase
	{
		private readonly IMediator _mediator;

		public Users(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Gets Users.
		/// </summary>
		/// <param name="queryOptions">OData query.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Users.</returns>
		[HttpGet]
		public async Task<ActionResult<PagedResponse<GetUsersResponse>>> GetAsync(ODataQueryOptions<GetUsersRequest> queryOptions, CancellationToken cancellationToken)
		{
			var query = new GetUsersQuery(queryOptions);
			var response = await _mediator.Send(query, cancellationToken);
			return Ok(response);
		}
		
		/// <summary>
		/// Gets User by Id.
		/// </summary>
		/// <param name="id">Unique identifier of User.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>User details.</returns>
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<GetUserResponse>> GetAsync(Guid id, CancellationToken cancellationToken)
		{
			var query = new GetUserQuery(id);
			var result = await _mediator.Send(query, cancellationToken);
			return Ok(result);
		}

		/// <summary>
		/// Updates User by Id.
		/// </summary>
		/// <param name="id">Unique identifier of User.</param>
		/// <param name="request">The Request.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>User details.</returns>
		[HttpPut("{id:guid}")]
		public async Task<ActionResult<object>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
		{
			var command = new UpdateUserCommand(id, request.Name);
			return Ok(await _mediator.Send(command, cancellationToken));
		}
	}
}
