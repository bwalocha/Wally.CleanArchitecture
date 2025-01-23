/*
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using FluentAssertions.Common;
using FluentAssertions.Execution;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;

public class AssertionStrategy : IAssertionStrategy
{
	private readonly int _allowedExceptionNumber;
	private readonly List<string> _failureMessages = new();

	[SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed")]
	[Obsolete("Fix test issues and use parameterless constructor.")]
	public AssertionStrategy(int allowedExceptionNumber)
	{
		_allowedExceptionNumber = allowedExceptionNumber;
	}

#pragma warning disable 618
	public AssertionStrategy()
		: this(0)
#pragma warning restore 618
	{
	}

	public IEnumerable<string> FailureMessages => _failureMessages.AsReadOnly();

	public void HandleFailure(string message)
	{
		_failureMessages.Add(message);
	}

	public IEnumerable<string> DiscardFailures()
	{
		var discardedFailures = _failureMessages.ToArray();
		_failureMessages.Clear();
		return discardedFailures;
	}

	public void ThrowIfAny(IDictionary<string, object> context)
	{
		if (_failureMessages.Count == _allowedExceptionNumber)
		{
			return;
		}

		if (_failureMessages.Count < _allowedExceptionNumber)
		{
			Services.ThrowException(
				$"The '{_allowedExceptionNumber}' threshold can be decreased to '{_failureMessages.Count}'.");
			return;
		}

		var builder = new StringBuilder();
		foreach (var pair in _failureMessages.Select((item, index) => new { item, index, }))
		{
			builder.AppendLine($"=== {_failureMessages.Count - pair.index} ===");
			builder.AppendLine(pair.item);
		}

		if (context.Any())
		{
			foreach (var pair in context)
			{
				builder.AppendFormat("\nWith {0}:\n{1}", pair.Key, pair.Value);
			}
		}

		Services.ThrowException(builder.ToString());
	}
}
*/
