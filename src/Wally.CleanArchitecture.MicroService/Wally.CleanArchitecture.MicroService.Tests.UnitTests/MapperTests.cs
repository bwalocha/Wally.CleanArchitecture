using System;
using AutoMapper;
using FluentAssertions;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Requests;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Responses;
using Wally.CleanArchitecture.MicroService.Application.MapperProfiles;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.UnitTests;

public class MapperTests
{
	private readonly IConfigurationProvider _configuration;
	private readonly IMapper _sut;

	public MapperTests()
	{
		_configuration = new MapperConfiguration(
			config => config.AddMaps(typeof(IApplicationMapperProfilesAssemblyMarker).Assembly));

		_sut = _configuration.CreateMapper();
	}

	[Fact]
	public void ShouldHaveValidConfiguration()
	{
		// Arrange
		
		// Act
		var act = () => _configuration.AssertConfigurationIsValid();

		// Assert
		act.Should()
			.NotThrow();
	}

	[Theory]
	[InlineData(typeof(User), typeof(GetUsersRequest))]
	[InlineData(typeof(User), typeof(GetUsersResponse))]
	[InlineData(typeof(User), typeof(GetUserResponse))]
	public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
	{
		// Arrange
		var instance = GetInstanceOf(source);
		var idProperty = source.GetProperty(nameof(User.Id)) !;
		idProperty.DeclaringType!.GetProperty(nameof(User.Id)) !.SetValue(instance, new UserId());

		// Act
		var act = () => _sut.Map(instance, source, destination);

		// Assert
		act.Should()
			.NotThrow();
	}

	[Theory]
	[InlineData(typeof(GetUsersRequest), typeof(GetUsersResponse))]
	[InlineData(typeof(GetUsersResponse), typeof(GetUsersRequest))]
	public void ShouldNotSupportMappingFromSourceToDestination(Type source, Type destination)
	{
		// Arrange
		var instance = GetInstanceOf(source);

		// Act
		var act = () => _sut.Map(instance, source, destination);

		// Assert
		act.Should()
			.ThrowExactly<AutoMapperMappingException>();
	}

	private static object GetInstanceOf(Type type)
	{
		return Activator.CreateInstance(type, true) !;
	}
}
