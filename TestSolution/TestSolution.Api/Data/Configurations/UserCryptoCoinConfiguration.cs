using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestSolution.Api.Data.Entities;

namespace TestSolution.Api.Data.Configurations
{
    public class UserCryptoCoinConfiguration : IEntityTypeConfiguration<UserCryptoCoin>
    {
        public void Configure(EntityTypeBuilder<UserCryptoCoin> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasOne(x => x.UserObject)
                .WithMany(x => x.UserCryptoCoinCollection)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.CryptoCoinObject)
                .WithMany(x => x.UserCryptoCoinCollection)
                .HasForeignKey(x => x.CryptoCoinId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.ToTable("UserCryptoCoin");
        }
    }
}
