using System;
using System.Runtime.Serialization;

using AutoMapper;

using Wally.CleanArchitecture.MicroService.Contracts.Requests.Users;
using Wally.CleanArchitecture.MicroService.Contracts.Responses.Users;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.MapperProfiles;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.UnitTests;

public class MappingTests
{
	private readonly IConfigurationProvider _configuration;
	private readonly IMapper _mapper;

	public MappingTests()
	{
		_configuration = new MapperConfiguration(config => config.AddProfile<UserProfile>());

		_mapper = _configuration.CreateMapper();
	}

	[Fact]
	public void ShouldHaveValidConfiguration()
	{
		_configuration.AssertConfigurationIsValid();
	}

	[Theory]
	[InlineData(typeof(User), typeof(GetUsersRequest))]
	[InlineData(typeof(User), typeof(GetUsersResponse))]
	[InlineData(typeof(User), typeof(GetUserResponse))]
	[InlineData(typeof(GetUsersRequest), typeof(GetUsersResponse))]
	public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
	{
		var instance = GetInstanceOf(source);

		_mapper.Map(instance, source, destination);
	}

	private object GetInstanceOf(Type type)
	{
		if (type.GetConstructor(Type.EmptyTypes) != null)
		{
			return Activator.CreateInstance(type)!;
		}

		// Type without parameterless constructor
		return FormatterServices.GetUninitializedObject(type);
	}
}
