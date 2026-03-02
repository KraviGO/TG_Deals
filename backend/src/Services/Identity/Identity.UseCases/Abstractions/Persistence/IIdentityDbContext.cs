using Identity.Entities.Users;

namespace Identity.UseCases.Abstractions.Persistence;

public interface IIdentityDbContext
{
    IQueryable<User> Users { get; }

    Task AddUserAsync(User user, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
