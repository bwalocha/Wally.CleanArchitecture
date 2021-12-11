using System.Collections.Generic;

namespace Wally.CleanArchitecture.WebApi.Models
{
	public class Cors
	{
		public List<string> Origins { get; } = new();
	}
}
