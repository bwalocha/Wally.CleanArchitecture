namespace Wally.CleanArchitecture.WebApi.Models
{
	public class AppSettings
	{
		public Authentication Authentication { get; } = new();

		public Authentication SwaggerAuthentication { get; } = new();

		public Cors Cors { get; } = new();

		public bool IsMigrationEnabled { get; set; } = true;
	}
}
