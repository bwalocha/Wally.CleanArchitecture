﻿using System.IO;
using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;

namespace Wally.CleanArchitecture.ApiGateway.Tests.IntegrationTests.Helpers;

public static class ModuleInitializer
{
	[ModuleInitializer]
	public static void Initialize()
	{
		Verifier.DerivePathInfo(
			(sourceFile, projectDirectory, type, method) => new PathInfo(
				Path.Combine(projectDirectory, "Snapshots"),
				type.Name,
				method.Name));

		VerifierSettings.ScrubInlineGuids();
		VerifierSettings.IgnoreMembers("traceId");

		VerifyHttp.Initialize();
		Recording.Start();
	}
}
