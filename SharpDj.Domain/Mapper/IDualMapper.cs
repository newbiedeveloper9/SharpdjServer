using System;

namespace SharpDj.Domain.Mapper
{
    public interface IDualMapper<TEntity, TDto>
    {
        TDto MapToDTO(TEntity entity);
        TEntity MapToEntity(TDto dto);
    }

    public interface IDualMapper<TEntity, TDto, TBag>
    {
        TDto MapToDTO(TEntity entity, TBag mapperBag);
        TEntity MapToEntity(TDto dto, TBag mapperBag);
    }
}