using talker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace talker.Infrastructure.Persistence.Configurations
{
    public class UserDictionaryConfiguration : IEntityTypeConfiguration<UserDictionary>
    {
        public void Configure(EntityTypeBuilder<UserDictionary> builder)
        {
        }
    }
}