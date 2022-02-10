using System;
using AutoMapper.Extensions.ExpressionMapping;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wally.CleanArchitecture.Application.Users.Commands;
using Wally.CleanArchitecture.Application.Users.Queries;
using Wally.CleanArchitecture.Contracts.Requests.User;
using Wally.CleanArchitecture.Domain.Abstractions;
using Wally.CleanArchitecture.MapperProfiles;
using Wally.CleanArchitecture.Messaging.Consumers;
using Wally.CleanArchitecture.Persistence;
using Wally.CleanArchitecture.Persistence.SqlServer.Migrations;
using Wally.CleanArchitecture.PipelineBehaviours;
using Wally.CleanArchitecture.WebApi.Filters;
using Wally.CleanArchitecture.WebApi.Models;
using Wally.Lib.ServiceBus.Abstractions;
using Wally.Lib.ServiceBus.DI.Microsoft;
using Wally.Lib.ServiceBus.RabbitMQ;

namespace Wally.CleanArchitecture.WebApi;

public class Startup
{
	private const string CorsPolicy = nameof(CorsPolicy);

	public Startup(IConfiguration configuration, IWebHostEnvironment env)
	{
		Configuration = configuration;
		Environment = env;
	}

	public IConfiguration Configuration { get; }

	public IWebHostEnvironment Environment { get; }

	public AppSettings AppSettings { get; } = new();

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		Configuration.Bind(AppSettings);

		services.AddMediatR(typeof(GetUserQuery));

		services.AddControllers(
				settings =>
				{
					settings.Filters.Add(typeof(HttpGlobalExceptionFilter));

					// Workaround for Swagger
					/*foreach (var outputFormatter in settings.OutputFormatters
						.OfType<ODataOutputFormatter>()
						.Where(_ => _.SupportedMediaTypes.Count == 0))
					{
						outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(ODataMediaTypeHeader));
					}

					foreach (var inputFormatter in settings.InputFormatters
						.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
					{
						inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(ODataMediaTypeHeader));
					}*/
				})
			.AddFluentValidation(
				config =>
				{
					config.ImplicitlyValidateChildProperties = true;
					config.RegisterValidatorsFromAssemblyContaining<UpdateUserRequestValidator>();
					config.RegisterValidatorsFromAssemblyContaining<UpdateUserCommandValidator>();
				})
			.AddOData(
				options =>
				{
					options.Filter()
						.OrderBy()
						.Count()
						.SetMaxTop(1000);
				})
			.AddNewtonsoftJson(
				options => { options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; });

		services.AddCors(
			options => options.AddPolicy(
				CorsPolicy,
				builder => builder.WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
					.AllowAnyHeader()
					.AllowCredentials()
					.WithOrigins(AppSettings.Cors.Origins.ToArray())));

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();

		services.AddHealthChecks()
			.AddSqlServer(
				Configuration.GetConnectionString("Database"),
				name: "DB",
				failureStatus: HealthStatus.Degraded,
				tags: new[] { "DB", "Database", "MSSQL", })
			.AddRabbitMQ(
				new Uri(Configuration.GetConnectionString("ServiceBus")),
				name: "MQ",
				failureStatus: HealthStatus.Degraded,
				tags: new[] { "MQ", "Messaging", "ServiceBus", });
		services.AddHealthChecksUI()
			.AddInMemoryStorage();

		Action<DbContextOptionsBuilder> dbContextOptions;
		dbContextOptions = options =>
		{
			options.UseSqlServer(
				Configuration.GetConnectionString("Database"),
				builder =>
				{
					builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
					builder.MigrationsAssembly(
						typeof(Initial).Assembly.GetName()
							.Name);
				});

			options.ConfigureWarnings(
				builder =>
				{
					builder.Default(WarningBehavior.Throw);
					builder.Ignore(RelationalEventId.MultipleCollectionIncludeWarning);
					builder.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS);
				});
		};
		services.AddDbContext<ApplicationDbContext>(dbContextOptions);

		services.AddAutoMapper(cfg => { cfg.AddExpressionMapping(); }, typeof(UserProfile).Assembly);

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogBehavior<,>));

		// services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
		// services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainEventsDispatcherBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandHandlerValidatorBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryHandlerValidatorBehavior<,>));

		services.Scan(
			a => a.FromApplicationDependencies(b => b.FullName!.StartsWith("Wally.CleanArchitecture."))
				.AddClasses(c => c.AssignableTo(typeof(IRepository<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

		services.Scan(
			a => a.FromAssemblyOf<UserCreatedConsumer>()
				.AddClasses(c => c.AssignableTo(typeof(IConsumeAsync<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

		services.AddSingleton(_ => Factory.Create(new Settings(Configuration.GetConnectionString("ServiceBus"))));
		services.AddServiceBus();
	}

	public void Configure(
		IApplicationBuilder app,
		IWebHostEnvironment env,
		IHostApplicationLifetime appLifetime,
		ILogger<Startup> logger,
		ApplicationDbContext dbContext)
	{
		appLifetime.ApplicationStarted.Register(
			() => logger.LogInformation("The 'Wally.CleanArchitecture' is started"));
		appLifetime.ApplicationStopping.Register(
			() => logger.LogInformation("The 'Wally.CleanArchitecture' is stopping"));
		appLifetime.ApplicationStopped.Register(
			() => logger.LogInformation("The 'Wally.CleanArchitecture' is stopped"));

		// Configure the HTTP request pipeline.
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();

			app.UseSwagger();
			app.UseSwaggerUI(
				opt =>
				{
					// opt.SwaggerEndpoint("v1/swagger.json", "Wally.CleanArchitecture WebApi v1");
					opt.OAuthClientId(AppSettings.SwaggerAuthentication.ClientId);
					opt.OAuthClientSecret(AppSettings.SwaggerAuthentication.ClientSecret);
					opt.OAuthUsePkce();
				});
		}

		// App is hosted by Docker, HTTPS is not required inside container
		// app.UseHttpsRedirection();

		app.UseRouting();
		app.UseCors(CorsPolicy);

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseHealthChecks(
			"/healthChecks",
			new HealthCheckOptions
			{
				Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
			});
		app.UseHealthChecksUI();

		app.UseEndpoints(
			endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapGet(
					"/",
					async context =>
					{
						await context.Response.WriteAsync(
							$"v{GetType().Assembly.GetName().Version}",
							context.RequestAborted);
					});
			});

		if (AppSettings.IsMigrationEnabled)
		{
			dbContext.Database.Migrate();
		}

		app.UseServiceBus();
	}
}
