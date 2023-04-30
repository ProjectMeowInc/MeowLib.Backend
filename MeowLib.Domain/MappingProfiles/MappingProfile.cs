using AutoMapper;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;

namespace MeowLib.Domain.MappingProfiles;

/// <summary>
/// Класс профилей для маппинга.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public MappingProfile()
    {
        CreateMap<CreateUserEntityModel, UserEntityModel>();
        CreateMap<UserEntityModel, UserDto>();
    }
}