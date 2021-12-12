using AutoMapper.Internal;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Types;
using MediatR;
using Wally.CleanArchitecture.Application.Abstractions;
using Wally.CleanArchitecture.ConventionTests.Helpers;
using Xunit;

namespace Wally.CleanArchitecture.ConventionTests
{
	public class AsyncTests
	{
		[Fact]
		public void AsyncMethods_ShouldHaveCancellationTokenAsLastParam()
		{
			using (new AssertionScope(new AssertionStrategy()))
			{
				foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
				{
					// TODO: Consider to exclude anonymous types in different way
					var types = AllTypes.From(assembly)
						.Where(a => !a.Name.Contains("<>"));

					foreach (var type in types)
					{
						foreach (var method in type.Methods()
							.Where(
								x => x.ReturnType == typeof(Task) ||
									x.ReturnType.ImplementsGenericInterface(typeof(Task<>)) ||
									x.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null))
						{
							if (type.ImplementsGenericInterface(typeof(IPipelineBehavior<,>)) && method.Name == nameof(IPipelineBehavior<object, object>.Handle))
							{
								continue;
							}
							
							var parameters = method.GetParameters();

							parameters.Last()
								.ParameterType.Should()
								.Be<CancellationToken>(
									"Method '{0}' should contain cancellation token as the last param in type '{1}'",
									method.Name,
									type);
						}
					}
				}
			}
		}

		[Fact]
		public void AsyncMethods_ShouldHaveAsyncSuffix()
		{
			using (new AssertionScope(new AssertionStrategy()))
			{
				foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
				{
					// TODO: Consider to exclude anonymous types in different way
					var types = AllTypes.From(assembly)
						.Where(a => !a.Name.Contains("<>"));

					foreach (var type in types)
					{
						foreach (var method in type.Methods()
							.Where(
								x => x.ReturnType == typeof(Task) ||
									x.ReturnType.ImplementsGenericInterface(typeof(Task<>)) ||
									x.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null))
						{
							if (type == typeof(CommandHandler<>))
							{
								continue;
							}
							
							if (type == typeof(QueryHandler<,>))
							{
								continue;
							}

							if (type.ImplementsGenericInterface(typeof(IPipelineBehavior<,>)) && method.Name == nameof(IPipelineBehavior<object, object>.Handle))
							{
								continue;
							}
							
							method.Name.Should()
								.EndWith(
									"Async",
									"Method '{0}' in type '{1}' should contain Async suffix ",
									method,
									type);
						}
					}
				}
			}
		}
	}
}
