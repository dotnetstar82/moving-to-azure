using Microsoft.EntityFrameworkCore;

namespace MovingToAzure.Data;

public interface ISqlDbContext
{
    DbSet<ProfileEntity> Profiles { get; }
    int SaveChanges();
}

public class SqlDbContext : DbContext, ISqlDbContext
{
    public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
    {
        this.ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<ProfileEntity> Profiles => Set<ProfileEntity>();

}
