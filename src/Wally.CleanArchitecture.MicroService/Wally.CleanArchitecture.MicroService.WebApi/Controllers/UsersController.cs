﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Requests.Users;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Responses.Users;
using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.CleanArchitecture.MicroService.Application.Users.Queries;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.MicroService.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(int), 200, "application/json")]
public class UsersController : ControllerBase
{
	private readonly ISender _mediator;

	public UsersController(ISender mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	///     Gets Users.
	/// </summary>
	/// <param name="queryOptions">OData query.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Users.</returns>
	/// <remarks>
	///     Sample request:
	///     GET /Users
	/// </remarks>
	[HttpGet]
	public async Task<ActionResult<PagedResponse<GetUsersResponse>>> GetAsync(
		ODataQueryOptions<GetUsersRequest> queryOptions, CancellationToken cancellationToken)
	{
		var query = new GetUsersQuery(queryOptions);
		var response = await _mediator.Send(query, cancellationToken);

		return Ok(response);
	}

	/// <summary>
	///     Gets User by Id.
	/// </summary>
	/// <param name="id">Unique identifier of User.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>User details.</returns>
	/// <remarks>
	///     Sample request:
	///     GET /Users/6ff34249-ef96-432a-9822-d3aca639a986
	/// </remarks>
	[HttpGet("{id:guid}")]
	public async Task<ActionResult<GetUserResponse>> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		var query = new GetUserQuery(new UserId(id));
		var result = await _mediator.Send(query, cancellationToken);

		return Ok(result);
	}

	/// <summary>
	///     Creates User.
	/// </summary>
	/// <param name="request">The Request.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>.</returns>
	/// <remarks>
	///     Sample request:
	///     POST /Users
	///     {
	///     "name": "sampleName"
	///     }
	/// </remarks>
	[HttpPost]
	public async Task<ActionResult<object>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
	{
		var command = new CreateUserCommand(request.Name);
		await _mediator.Send(command, cancellationToken);

		return Ok();
	}

	/// <summary>
	///     Updates User by Id.
	/// </summary>
	/// <param name="id">Unique identifier of User.</param>
	/// <param name="request">The Request.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>.</returns>
	/// <remarks>
	///     Sample request:
	///     PUT /Users/6ff34249-ef96-432a-9822-d3aca639a986
	///     {
	///     "name": "sampleName"
	///     }
	/// </remarks>
	[HttpPut("{id:guid}")]
	public async Task<ActionResult<object>> UpdateAsync(Guid id, UpdateUserRequest request,
		CancellationToken cancellationToken)
	{
		var command = new UpdateUserCommand(new UserId(id), request.Name);
		await _mediator.Send(command, cancellationToken);

		return Ok();
	}

	/// <summary>
	///     Deletes User by Id.
	/// </summary>
	/// <param name="id">Unique identifier of User.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>.</returns>
	/// <remarks>
	///     Sample request:
	///     DELETE /Users/6ff34249-ef96-432a-9822-d3aca639a986
	/// </remarks>
	[HttpDelete("{id:guid}")]
	public async Task<ActionResult<object>> DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		var command = new DeleteUserCommand(new UserId(id));
		await _mediator.Send(command, cancellationToken);

		return Accepted();
	}
}
