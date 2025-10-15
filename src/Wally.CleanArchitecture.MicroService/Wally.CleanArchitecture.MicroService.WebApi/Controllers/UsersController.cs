using System;
using AutoMapper;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.CleanArchitecture.MicroService.Application.Users.Queries;
using Wally.CleanArchitecture.MicroService.Application.Users.Results;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.WebApi.Abstractions;
using Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Requests;
using Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Responses;

namespace Wally.CleanArchitecture.MicroService.WebApi.Controllers;

/// <summary>
/// The Controller
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(int), 200, "application/json")]
public class UsersController : ControllerBase
{
	private readonly ISender _sender;
	private readonly IMapper _mapper;

	/// <summary>
	/// Initializes a new instance of the <see cref="UsersController"/> class.
	/// </summary>
	/// <param name="sender">The Sender.</param>
	/// <param name="mapper">The Mapper.</param>
	public UsersController(ISender sender, IMapper mapper)
	{
		_sender = sender;
		_mapper = mapper;
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
		var options = new QueryOption<GetUsersRequest, Wally.CleanArchitecture.MicroService.Application.Users.Requests.GetUsersRequest>(queryOptions, _mapper);
		var query = new GetUsersQuery(options);
		var response = await _sender.Send(query, cancellationToken);
		
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
	public async Task<ActionResult<GetUserResult>> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		var query = new GetUserQuery(new UserId(id));
		var result = await _sender.Send(query, cancellationToken);

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
	public async Task<ActionResult> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
	{
		var command = new CreateUserCommand(request.Name);
		await _sender.Send(command, cancellationToken);

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
	public async Task<ActionResult> UpdateAsync(Guid id, UpdateUserRequest request,
		CancellationToken cancellationToken)
	{
		var command = new UpdateUserCommand(new UserId(id), request.Name);
		await _sender.Send(command, cancellationToken);

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
	// [Authorize("Users.Write")]
	// [Authorize]
	public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		var command = new DeleteUserCommand(new UserId(id));
		await _sender.Send(command, cancellationToken);

		return Accepted();
	}
}
