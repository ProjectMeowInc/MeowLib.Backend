﻿using MeowLib.Domain.DbModels.BookEntity;

namespace MeowLib.Domain.DbModels.ChapterEntity;

/// <summary>
/// Класс главы, хранящийся в БД.
/// </summary>
public class ChapterEntityModel
{
    /// <summary>
    /// Id главы.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Имя главы.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Текст главы.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Время загрузки главы.
    /// </summary>
    public required DateTime ReleaseDate { get; set; }

    /// <summary>
    /// Книга, к которой принадлежит глава.
    /// </summary>
    public required BookEntityModel Book { get; set; }
}