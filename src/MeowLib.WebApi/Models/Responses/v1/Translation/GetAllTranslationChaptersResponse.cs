﻿using MeowLib.Domain.Dto.Chapter;

namespace MeowLib.WebApi.Models.Responses.v1.Translation;

public class GetAllTranslationChaptersResponse
{
    public required IEnumerable<ChapterDto> Items { get; init; }
    public required int Count { get; init; }
}