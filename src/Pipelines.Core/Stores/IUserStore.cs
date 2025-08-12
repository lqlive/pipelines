using Pipelines.Core.Entities.Users;

namespace Pipelines.Core.Stores;
public interface IUserStore
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> CreateAsync(User user, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> CreateProviderAsync(User user, UserProvider provider, CancellationToken cancellationToken);
}