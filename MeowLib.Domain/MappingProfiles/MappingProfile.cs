using AutoMapper;
using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Dto.User;

namespace MeowLib.Domain.MappingProfiles;

/// <summary>
/// Класс профилей для маппинга.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Конструктор. sueta
    /// </summary>
    public MappingProfile()
    {
        // Users mapping
        CreateMap<CreateUserEntityModel, UserEntityModel>();
        CreateMap<UserEntityModel, UserDto>();
        
        // Authors mapping
        CreateMap<CreateAuthorEntityModel, AuthorEntityModel>();
        CreateMap<AuthorEntityModel, AuthorDto>();
    }
}