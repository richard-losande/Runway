using Hackathon.ApiService.Features.FinancialRunway;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.ApiService.Databases.DbContexts
{
    public class MainDbContext : DbContext, IMaindbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

        public DbSet<FinancialAnalysis> FinancialAnalyses => Set<FinancialAnalysis>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FinancialAnalysis>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ResponseJson).HasColumnType("jsonb");
            });
        }
    }
}
