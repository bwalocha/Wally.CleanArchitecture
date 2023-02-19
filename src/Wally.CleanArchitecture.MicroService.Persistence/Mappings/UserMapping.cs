using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Persistence.Mappings;

internal class UserMapping : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasIndex(a => a.Name)
			.IsUnique();

		builder.Property(a => a.Name)
			.IsRequired()
			.HasMaxLength(250);
	}
}
