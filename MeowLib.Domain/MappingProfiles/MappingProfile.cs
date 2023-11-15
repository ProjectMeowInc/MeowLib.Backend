using AutoMapper;
using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.DbModels.BookCommentEntity;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.BookmarkEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Dto.BookComment;
using MeowLib.Domain.Dto.Bookmark;
using MeowLib.Domain.Dto.Chapter;
using MeowLib.Domain.Dto.Tag;
using MeowLib.Domain.Dto.Translation;
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
        CreateMap<BookEntityModel, GetBookResponse>()
            .ForMember(
                b => b.Translations,
                b 
                    => b.MapFrom(b => b.Translations.Select(tr => new TranslationDto
                    {
                        Id = tr.Id,
                        Name = tr.Team.Name
                    }))
            );
        CreateMap<UpdateBookInfoRequest, UpdateBookEntityModel>();

        // Chapter mapping
        CreateMap<ChapterEntityModel, ChapterDto>();

        // Bookmark mapping
        CreateMap<BookmarkEntityModel, BookmarkDto>()
            .ForMember(b => b.ChapterId,
                opt
                    => opt.MapFrom(b => b.Chapter.Id));

        // BookComments mapping
        CreateMap<BookCommentEntityModel, BookCommentDto>();
    }
}