using AutoMapper;
using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Dto.Chapter;
using MeowLib.Domain.Dto.Tag;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Requests.Author;
using MeowLib.Domain.Requests.Book;
using MeowLib.Domain.Requests.Tag;
using MeowLib.Domain.Requests.User;
using MeowLib.Domain.Responses.Book;
using MeowLib.Domain.Responses.Chapter;

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
        // Users mapping
        CreateMap<CreateUserEntityModel, UserEntityModel>();
        CreateMap<UserEntityModel, UserDto>();
        CreateMap<UpdateUserRequest, UpdateUserEntityModel>()
            .ReverseMap();
        
        // Authors mapping
        CreateMap<CreateAuthorEntityModel, AuthorEntityModel>();
        CreateMap<AuthorEntityModel, AuthorDto>();
        CreateMap<UpdateAuthorEntityModel, UpdateAuthorRequest>()
            .ReverseMap();
        
        // Tag mapping
        CreateMap<TagEntityModel, TagDto>();
        CreateMap<CreateTagRequest, CreateTagEntityModel>()
            .ReverseMap();
        CreateMap<UpdateTagRequest, UpdateTagEntityModel>()
            .ReverseMap();
        
        // Book mapping
        CreateMap<CreateBookRequest, CreateBookEntityModel>()
            .ReverseMap();
        CreateMap<BookEntityModel, GetBookResponse>();
        
        // Chapter mapping
        CreateMap<ChapterEntityModel, ChapterDto>();
        CreateMap<ChapterEntityModel, GetBookChapterResponse>();
    }
}