using System.Threading;
using System.Threading.Tasks;

namespace SharpDj.Domain.Repository
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken);
    }
}