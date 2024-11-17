using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Mappings;

internal class UserMapping : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasIndex(a => a.Name)
			.IsUnique()
			.HasFilter($"{nameof(User.IsDeleted)} != 1");

		builder.Property(a => a.Name)
			.HasMaxLength(User.MaxNameLength);

		// TODO: add example of ValueObject using Complex
	}
}
