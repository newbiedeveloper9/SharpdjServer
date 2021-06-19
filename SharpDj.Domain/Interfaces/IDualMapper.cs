namespace SharpDj.Domain.Interfaces
{
    public interface IDualMapper<TEntity, TDto>
    {
        TDto MapToDto(TEntity entity);
        TEntity MapToEntity(TDto dto);
    }

    public interface IDualMapper<TEntity, TDto, TBag>
    {
        TDto MapToDto(TEntity entity, TBag mapperBag);
        TEntity MapToEntity(TDto dto, TBag mapperBag);
    }
}