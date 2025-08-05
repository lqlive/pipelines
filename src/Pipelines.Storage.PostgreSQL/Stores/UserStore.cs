using Microsoft.EntityFrameworkCore;
using Pipelines.Core.Entities;
using Pipelines.Core.Entities.Users;
using Pipelines.Core.Stores;

namespace Pipelines.Storage.PostgreSQL.Stores;
public class UserStore(IContext context) : IUserStore
{
    public async Task<bool> CreateAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (DbUpdateException ex)
            when (context.IsUniqueConstraintViolationException(ex))
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await context.Users.Include(x => x.Setting).SingleOrDefaultAsync(x => x.Id == id);
        if (user is null)
        {
            return false;
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Email == email);
        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync(id);
        return user;
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}