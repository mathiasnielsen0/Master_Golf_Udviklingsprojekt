using Microsoft.EntityFrameworkCore;

namespace TSDB2.Data;

public interface ITimeScaleDbContext : IDisposable
{
    public DbSet<holdings_in_accounts> holdings_in_accounts { get; set; } // holdings_in_accounts

    
    int SaveChanges();
    int SaveChanges(bool acceptAllChangesOnSuccess);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    string ToString();

    Task AddRangeAsync(params object[] entities);
    Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default);
    void AddRange(IEnumerable<object> entities);
    void AddRange(params object[] entities);

    void AttachRange(IEnumerable<object> entities);
    void AttachRange(params object[] entities);

    TEntity Find<TEntity>(params object[] keyValues) where TEntity : class;
    ValueTask<TEntity> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken) where TEntity : class;
    ValueTask<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;
    ValueTask<object> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken);
    ValueTask<object> FindAsync(Type entityType, params object[] keyValues);
    object Find(Type entityType, params object[] keyValues);

    void RemoveRange(IEnumerable<object> entities);
    void RemoveRange(params object[] entities);

    void UpdateRange(IEnumerable<object> entities);
    void UpdateRange(params object[] entities);
}


public class TimeScaleDbContext : DbContext, ITimeScaleDbContext
{
    public TimeScaleDbContext()
    {
    }

    public TimeScaleDbContext(DbContextOptions<TimeScaleDbContext> options)
        : base(options)
    {
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Server=localhost;User Id=postgres;Password=1234;Database=example;");
        }
    }
    
    public DbSet<holdings_in_accounts> holdings_in_accounts { get; set; } // Holding
}