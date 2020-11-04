namespace SharpDj.Domain.Repository
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }
}