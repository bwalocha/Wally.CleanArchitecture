﻿using System;
using System.Runtime.Serialization;

using AutoMapper;

using Wally.CleanArchitecture.MicroService.Application.Contracts.Requests.Users;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Responses.Users;
using Wally.CleanArchitecture.MicroService.Application.MapperProfiles;
using Wally.CleanArchitecture.MicroService.Domain.Users;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.UnitTests;

public class MappingTests
{
	private readonly IConfigurationProvider _configuration;
	private readonly IMapper _mapper;

	public MappingTests()
	{
		_configuration = new MapperConfiguration(config => config
			.AddMaps(typeof(IApplicationMapperProfilesAssemblyMarker).Assembly));

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
