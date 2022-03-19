﻿using EasyNetQ.AutoSubscribe;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.Messaging.Consumers;
using Wally.Lib.ServiceBus.DI.Microsoft;
using Wally.Lib.ServiceBus.RabbitMQ;

namespace Wally.CleanArchitecture.WebApi.Extensions;

public static class MessagingExtensions
{
	public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
	{
		services.Scan(
			a => a.FromAssemblyOf<UserCreatedConsumer>()
				.AddClasses(c => c.AssignableTo(typeof(IConsumeAsync<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

		services.AddSingleton(
			_ => Factory.Create(new Settings(configuration.GetConnectionString(Constants.ServiceBus))));
		services.AddServiceBus();

		return services;
	}
}
