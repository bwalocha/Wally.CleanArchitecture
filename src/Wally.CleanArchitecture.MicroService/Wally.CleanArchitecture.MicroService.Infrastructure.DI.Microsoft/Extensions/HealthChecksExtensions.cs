﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using HealthChecks.Kafka;
using HealthChecks.MySql;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class HealthChecksExtensions
{
	// Adds Readiness
	public static IServiceCollection AddHealthChecks(this IServiceCollection services, AppSettings settings)
	{
		services.AddHealthChecks()
			.WithVersion()
			.WithDatabase(settings)
			.WithMessageBroker(settings);

		return services;
	}

	public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
	{
		app.UseHealthChecks(
			"/healthChecks",
			new HealthCheckOptions
			{
				Predicate = _ => true,
				ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
			});

		app.UseEndpoints(
			endpoints =>
			{
				// Adds Liveness
				endpoints.MapGet(
					"/",
					async context =>
					{
						await context.Response.WriteAsync(
							$"v{typeof(HealthChecksExtensions).Assembly.GetName().Version}",
							context.RequestAborted);
					});
			});

		return app;
	}

	private static IHealthChecksBuilder WithVersion(this IHealthChecksBuilder healthChecksBuilder)
	{
		return healthChecksBuilder.AddVersionHealthCheck();
	}

	private static IHealthChecksBuilder WithDatabase(this IHealthChecksBuilder healthChecksBuilder,
		AppSettings settings)
	{
		switch (settings.Database.ProviderType)
		{
			case DatabaseProviderType.None:
				break;
			case DatabaseProviderType.InMemory:
				break;
			case DatabaseProviderType.MySql:
				healthChecksBuilder.AddMySql(
					new MySqlHealthCheckOptions(settings.ConnectionStrings.Database),
					"DB",
					HealthStatus.Degraded,
					[
						"DB", "Database", nameof(DatabaseProviderType.MySql),
					]);
				break;
			case DatabaseProviderType.PostgreSQL:
				healthChecksBuilder.AddNpgSql(
					settings.ConnectionStrings.Database,
					name: "DB",
					failureStatus: HealthStatus.Degraded,
					tags:
					[
						"DB", "Database", nameof(DatabaseProviderType.PostgreSQL),
					]);
				break;
			case DatabaseProviderType.SQLite:
				healthChecksBuilder.AddSqlite(
					settings.ConnectionStrings.Database,
					name: "DB",
					failureStatus: HealthStatus.Degraded,
					tags:
					[
						"DB", "Database", nameof(DatabaseProviderType.SQLite),
					]);
				break;
			case DatabaseProviderType.SqlServer:
				healthChecksBuilder.AddSqlServer(
					settings.ConnectionStrings.Database,
					name: "DB",
					failureStatus: HealthStatus.Degraded,
					tags:
					[
						"DB", "Database", nameof(DatabaseProviderType.SqlServer),
					]);
				break;
			default:
				throw new NotSupportedException(
					$"Not supported Database Provider type: '{settings.Database.ProviderType}'");
		}

		return healthChecksBuilder;
	}

	private static IHealthChecksBuilder WithMessageBroker(this IHealthChecksBuilder healthChecksBuilder,
		AppSettings settings)
	{
		switch (settings.MessageBroker)
		{
			case MessageBrokerType.None:
				break;
			case MessageBrokerType.AzureServiceBus:
				healthChecksBuilder.WithAzureServiceBus();
				break;
			case MessageBrokerType.Kafka:
				healthChecksBuilder.WithKafka(settings);
				break;
			case MessageBrokerType.RabbitMQ:
				healthChecksBuilder.WithRabbitMq(settings);
				break;
			default:
				throw new NotSupportedException($"Not supported Message Broker type: '{settings.MessageBroker}'");
		}

		return healthChecksBuilder;
	}

	private static IHealthChecksBuilder WithAzureServiceBus(this IHealthChecksBuilder healthChecksBuilder)
	{
		// TODO: Add AzureServiceBus HealthCheck
		// healthChecksBuilder.AddAzureServiceBusQueue()
		// ...

		return healthChecksBuilder;
	}

	private static IHealthChecksBuilder WithRabbitMq(this IHealthChecksBuilder healthChecksBuilder,
		AppSettings settings)
	{
		healthChecksBuilder.AddRabbitMQ(
			_ => new ConnectionFactory
			{
				Uri	= new Uri(settings.ConnectionStrings.ServiceBus),
			}.CreateConnectionAsync(),
			name: "MQ",
			failureStatus: HealthStatus.Degraded,
			tags:
			[
				"MQ", "Messaging", nameof(MessageBrokerType.RabbitMQ),
			],
			TimeSpan.FromSeconds(600));

		return healthChecksBuilder;
	}

	private static IHealthChecksBuilder WithKafka(this IHealthChecksBuilder healthChecksBuilder, AppSettings settings)
	{
		healthChecksBuilder.AddKafka(
			new KafkaHealthCheckOptions
			{
				Configuration = new ProducerConfig(
					new ClientConfig
					{
						BootstrapServers = settings.ConnectionStrings.ServiceBus,
					}),
			},
			"MQ",
			HealthStatus.Degraded,
			[
				"MQ", "Messaging", nameof(MessageBrokerType.Kafka),
			]);

		return healthChecksBuilder;
	}

	private static IHealthChecksBuilder AddVersionHealthCheck(this IHealthChecksBuilder builder)
	{
		builder.AddCheck<VersionHealthCheck>("VER", tags: ["VER", "Version",]);

		return builder;
	}

	private sealed class VersionHealthCheck : IHealthCheck
	{
		private readonly string? _version = typeof(VersionHealthCheck).Assembly.GetName()
			.Version?.ToString();

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
			CancellationToken cancellationToken = default)
		{
			return Task.FromResult(HealthCheckResult.Healthy(_version));
		}
	}
}
