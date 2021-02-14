using System.Linq;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Job> Jobs { get; set; } = null!;
        public DbSet<JobItem> JobItems { get; set; } = null!;
        public DbSet<PerformedJob> PerformedJobs { get; set; } = null!;
        
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //removing the cascade delete, can add back later if needed
            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => !e.IsOwned())
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
        
        // dotnet aspnet-codegenerator razorpage -m Item -dc AppDbContext -udl -outDir Pages/Items --referenceScriptLibraries
        // dotnet aspnet-codegenerator razorpage -m Job -dc AppDbContext -udl -outDir Pages/Jobs --referenceScriptLibraries
        // dotnet aspnet-codegenerator razorpage -m JobItem -dc AppDbContext -udl -outDir Pages/JobItems --referenceScriptLibraries
        // dotnet aspnet-codegenerator razorpage -m PerformedJob -dc AppDbContext -udl -outDir Pages/PerformedJobs --referenceScriptLibraries

    }
}