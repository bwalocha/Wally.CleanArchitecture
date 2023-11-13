using System;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public class GuidId<TId> : StronglyTypedId<TId, Guid> where TId : StronglyTypedId<TId, Guid>
{
	protected GuidId()
		: this(Generate())
	{
	}

	protected GuidId(Guid value)
		: base(value)
	{
	}

	/// <summary>
	///     https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/Id/GuidCombGenerator.cs
	///     https://www.codeproject.com/Articles/388157/GUIDs-as-fast-primary-keys-under-multiple-database
	/// </summary>
	/// <returns>Sequential Id</returns>
	private static Guid Generate()
	{
		var guidArray = Guid.NewGuid()
			.ToByteArray();

		var now = DateTime.UtcNow;

		// Get the days and milliseconds which will be used to build the byte string
		var days = new TimeSpan(now.Ticks - DateTime.UnixEpoch.Ticks);
		var msecs = now.TimeOfDay;

		// Convert to a byte array
		// Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333
		var daysArray = BitConverter.GetBytes(days.Days);
		var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

		// Reverse the bytes to match SQL Servers ordering
		Array.Reverse(daysArray);
		Array.Reverse(msecsArray);

		// Copy the bytes into the guid
		Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
		Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

		return new Guid(guidArray);
	}
}
