using SharpDj.Domain.SeedWork;

namespace SharpDj.Domain.Repository
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }
}