using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.OData.ModelBuilder;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application;

public class ODataQueryOptionsBuilder<TRequest>
	where TRequest : class, IRequest
{
	private readonly List<string> _orderBy = new();
	private int? _skip;
	private int? _top;
	private string _filter = null!;
	private string _search = null!;

	public ODataQueryOptionsBuilder<TRequest> WithSkip(int? value)
	{
		_skip = value;

		return this;
	}

	public ODataQueryOptionsBuilder<TRequest> WithTop(int? value)
	{
		_top = value;

		return this;
	}

	public ODataQueryOptionsBuilder<TRequest> WithSearch(string term)
	{
		_search = term;

		return this;
	}

	public ODataQueryOptionsBuilder<TRequest> WithFilter(string filter)
	{
		_filter = filter;

		return this;
	}

	public ODataQueryOptionsBuilder<TRequest> WithOrderBy(string orderBy)
	{
		if (string.IsNullOrEmpty(orderBy))
		{
			return this;
		}

		foreach (var order in orderBy.Split(','))
		{
			_orderBy.Add(order);
		}

		return this;
	}

	public ODataQueryOptionsBuilder<TRequest> WithOrderBy<TProperty>(
		Expression<Func<TRequest, TProperty>> propertyExpression)
	{
		if (propertyExpression.Body is MemberExpression memberExpression)
		{
			return WithOrderBy(memberExpression.Member.Name);
		}

		throw new ArgumentException(nameof(propertyExpression));
	}

	public ODataQueryOptionsBuilder<TRequest> WithOrderByDesc<TProperty>(
		Expression<Func<TRequest, TProperty>> propertyExpression)
	{
		if (propertyExpression.Body is MemberExpression memberExpression)
		{
			return WithOrderBy($"{memberExpression.Member.Name} desc");
		}

		throw new ArgumentException(nameof(propertyExpression));
	}

	public ODataQueryOptions<TRequest> Build()
	{
		var httpContext = new DefaultHttpContext
		{
			Request =
			{
				Method = "GET",
				Scheme = "http",
				Host = new HostString("localhost"),
				ContentType = "application/json",
				Query = new QueryCollection(QueryHelpers.ParseQuery(BuildQuery())),
			},
		};
		var stream = new MemoryStream();
		var writer = new StreamWriter(stream);
		writer.Write("{}");
		writer.Flush();
		stream.Position = 0;
		httpContext.Request.Body = stream;
		var request = httpContext.Request;
		var type = typeof(TRequest);
		var odataPath = request.ODataFeature()
			.Path;
		var builder = new ODataConventionModelBuilder();
		var entityType = builder.AddEntityType(type);
		foreach (var property in type.GetProperties())
		{
			entityType.AddProperty(property);
		}

		var model = builder.GetEdmModel();
		var oDataQueryContext = new ODataQueryContext(model, type, odataPath);

		return new ODataQueryOptions<TRequest>(oDataQueryContext, request);
	}

	private string BuildQuery()
	{
		var query = new StringBuilder();

		if (_skip.HasValue)
		{
			query.Append($"&$skip={_skip}");
		}

		if (_top.HasValue)
		{
			query.Append($"&$top={_top}");
		}

		if (_orderBy.Count > 0)
		{
			query.Append($"&$orderby={string.Join(",", _orderBy)}");
		}

		if (!string.IsNullOrEmpty(_search))
		{
			query.Append($"&$search={Encode(_search)}");
		}

		if (!string.IsNullOrEmpty(_filter))
		{
			query.Append($"&$filter={Encode(_filter)}");
		}

		return query.ToString()
			.TrimStart('&');
	}

	private static string Encode(string data)
	{
		return data
			.Replace("&", "%26")
			.Replace("$", "%24")
			.Replace("#", "%23")
			.Replace("/", "%2F")
			.Replace(":", "%3A")
			.Replace(";", "%3B")
			.Replace("=", "%3D")
			.Replace("?", "%3F")
			.Replace("@", "%40")
			.Replace("|", "%7C");
	}
}
