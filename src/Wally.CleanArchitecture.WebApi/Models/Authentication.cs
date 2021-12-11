namespace Wally.CleanArchitecture.WebApi.Models
{
	public class Authentication
	{
		public string Authority { get; set; } = string.Empty;

		public string ClientId { get; set; } = string.Empty;

		public string ClientSecret { get; set; } = string.Empty;
	}
}
