﻿namespace MeowLib.Domain.Chapter.Dto;

public class ChapterDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required DateTime ReleaseDate { get; init; }
    public required uint Volume { get; init; }
    public required uint Position { get; init; }
}