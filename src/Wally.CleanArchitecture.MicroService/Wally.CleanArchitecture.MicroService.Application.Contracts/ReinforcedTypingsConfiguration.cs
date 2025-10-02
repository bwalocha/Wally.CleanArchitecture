using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Contracts;

public static class ReinforcedTypingsConfiguration
{
	public static void Configure(ConfigurationBuilder builder)
	{
		var assemblies = new[]
		{
			Assembly.GetAssembly(typeof(IApplicationAssemblyMarker)), Assembly.GetAssembly(typeof(ReinforcedTypingsConfiguration)),
		};

		var types = assemblies.SelectMany(a => a!.GetTypes())
			.Where(x => x.IsPublic)
			.Where(x => x.IsClass)
			.Where(
				x => x.GetInterfaces()
					.Contains(typeof(IRequest)) || x.GetInterfaces()
					.Contains(typeof(IResult)))
			.ToArray();

		builder.Global(
			g =>
			{
				g.UseModules();
				g.CamelCaseForProperties();
				g.AutoOptionalProperties();
				g.ReorderMembers();
				g.RootNamespace("Wally.CleanArchitecture.MicroService");
			});

		builder.Substitute(typeof(Guid), new RtSimpleTypeName("string"));
		builder.Substitute(typeof(Uri), new RtSimpleTypeName("string"));
		builder.Substitute(typeof(Stream), new RtSimpleTypeName("any"));
		builder.Substitute(typeof(DateTime), new RtSimpleTypeName("Dayjs"))
			.AddImport("{ Dayjs }", "dayjs");

		builder.ExportAsInterfaces(
			types,
			exportBuilder =>
			{
				var name = exportBuilder.Type.Name.Replace("`1", string.Empty);
				exportBuilder.AutoI(false);
				exportBuilder.WithPublicProperties();
				exportBuilder.WithAllProperties();

				exportBuilder.OverrideName(name);
				if (exportBuilder.Type.Namespace == null)
				{
					return;
				}

				if (exportBuilder.Type.Namespace == typeof(ReinforcedTypingsConfiguration).Namespace)
				{
					exportBuilder.ExportTo($"{name}.ts");
				}
				else
				{
					var segments = exportBuilder.Type.Namespace.Split('.')
						.Reverse()
						.ToArray();
					exportBuilder.ExportTo($"{segments[1]}/{segments[0]}/{name}.ts");
				}
			});
	}
}
