using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/* DbContext for LogiTrack application */
public class LogiTrackContext :  IdentityDbContext<ApplicationUser>
{
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=logitrack.db");

    // Configure primary keys using Fluent API (safe: picks first matching CLR property)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        void TrySetKey(Type entityType, string[] candidateNames)
        {
            foreach (var name in candidateNames)
            {
                if (entityType.GetProperty(name) != null)
                {
                    modelBuilder.Entity(entityType).HasKey(name);
                    break;
                }
            }
        }

        TrySetKey(typeof(InventoryItem), new[] { "Id", "InventoryItemId", "ItemId" });
    }
}
