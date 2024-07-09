using MediatR;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface IQuery<out TResult> : IRequest<TResult>
	where TResult : IResponse
{
}
