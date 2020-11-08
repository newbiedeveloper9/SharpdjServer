using System.Threading;
using System.Threading.Tasks;

namespace SharpDj.Domain.SeedWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess = true,
            CancellationToken cancellationToken = default);
    }
}