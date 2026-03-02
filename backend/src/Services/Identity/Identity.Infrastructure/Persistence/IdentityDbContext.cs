using Identity.Entities.Users;
using Identity.UseCases.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext : DbContext, IIdentityDbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    public DbSet<User> UsersSet => Set<User>();
    public IQueryable<User> Users => UsersSet.AsNoTracking();

    public Task AddUserAsync(User user, CancellationToken ct) => UsersSet.AddAsync(user, ct).AsTask();
    Task<int> IIdentityDbContext.SaveChangesAsync(CancellationToken ct) => base.SaveChangesAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(x => x.Id);

            b.Property(x => x.Email).IsRequired().HasMaxLength(320);
            b.HasIndex(x => x.Email).IsUnique();

            b.Property(x => x.PasswordHash).IsRequired();

            b.Property(x => x.Role).IsRequired();
            b.Property(x => x.Status).IsRequired();

            b.Property(x => x.CreatedAt).IsRequired();
        });
    }
}
