using Microsoft.EntityFrameworkCore;
using SearchEngineTools.Models;

namespace SearchEngineTools.Data
{
    public class SearchEngineToolsDbContext(DbContextOptions<SearchEngineToolsDbContext> options) : DbContext(options)
    {

        public DbSet<IndexNowKey> IndexNowKeys => Set<IndexNowKey>();

        public DbSet<SearchEngineSubmissionItem> SearchEngineSubmissionQueue => Set<SearchEngineSubmissionItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IndexNowKey>(entity =>
            {
                entity.ToTable("IndexNowKeys");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Domain)
                    .HasMaxLength(IndexNowKey.MaxDomainLength)
                    .IsRequired();

                entity.HasIndex(x => x.Domain)
                    .IsUnique();

                entity.Property(x => x.Key)
                    .HasMaxLength(IndexNowKey.MaxKeyLength)
                    .IsRequired();
            });

            modelBuilder.Entity<SearchEngineSubmissionItem>(entity =>
            {
                entity.ToTable("SearchEngineSubmissionQueue");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Url)
                    .HasMaxLength(SearchEngineSubmissionItem.MaxUrlLength)
                    .IsRequired();

                entity.HasIndex(x => x.Url)
                    .IsUnique();

                entity.Property(x => x.Status)
                    .HasConversion<int>();

                entity.Property(x => x.LastError)
                    .HasMaxLength(SearchEngineSubmissionItem.MaxErrorLength);
            });
        }
    }
}
