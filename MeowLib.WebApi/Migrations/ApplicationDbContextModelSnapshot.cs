﻿// <auto-generated />
using System;
using MeowLib.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MeowLib.WebApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0-rc.2.23480.1");

            modelBuilder.Entity("BookEntityModelTagEntityModel", b =>
                {
                    b.Property<int>("BooksId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TagsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("BooksId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("BookEntityModelTagEntityModel");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.AuthorEntity.AuthorEntityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.BookCommentEntity.BookCommentEntityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuthorId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BookEntityModelId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("PostedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("BookEntityModelId");

                    b.HasIndex("BookId");

                    b.ToTable("BookComments");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.BookEntity.BookEntityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuthorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.BookmarkEntity.BookmarkEntityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BookEntityModelId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChapterId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BookEntityModelId");

                    b.HasIndex("ChapterId");

                    b.HasIndex("UserId");

                    b.ToTable("Bookmarks");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.ChapterEntity.ChapterEntityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Position")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TranslationId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TranslationId");

                    b.ToTable("Chapters");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.NotificationEntity.NotificationEntityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsWatched")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.TagEntity.TagEntityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.TeamEntity.TeamEntityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.TeamMemberEntity.TeamMemberEntityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.HasIndex("UserId");

                    b.ToTable("TeamMembers");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.TranslationEntity.TranslationEntityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("TeamId");

                    b.ToTable("Translations");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.UserEntity.UserEntityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("TEXT");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.UserFavoriteEntity.UserFavoriteEntityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersFavorite");
                });

            modelBuilder.Entity("BookEntityModelTagEntityModel", b =>
                {
                    b.HasOne("MeowLib.Domain.DbModels.BookEntity.BookEntityModel", null)
                        .WithMany()
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MeowLib.Domain.DbModels.TagEntity.TagEntityModel", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.BookCommentEntity.BookCommentEntityModel", b =>
                {
                    b.HasOne("MeowLib.Domain.DbModels.UserEntity.UserEntityModel", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MeowLib.Domain.DbModels.BookEntity.BookEntityModel", null)
                        .WithMany()
                        .HasForeignKey("BookEntityModelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MeowLib.Domain.DbModels.BookEntity.BookEntityModel", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Book");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.BookEntity.BookEntityModel", b =>
                {
                    b.HasOne("MeowLib.Domain.DbModels.AuthorEntity.AuthorEntityModel", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.BookmarkEntity.BookmarkEntityModel", b =>
                {
                    b.HasOne("MeowLib.Domain.DbModels.BookEntity.BookEntityModel", null)
                        .WithMany()
                        .HasForeignKey("BookEntityModelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MeowLib.Domain.DbModels.ChapterEntity.ChapterEntityModel", "Chapter")
                        .WithMany()
                        .HasForeignKey("ChapterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MeowLib.Domain.DbModels.UserEntity.UserEntityModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chapter");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.ChapterEntity.ChapterEntityModel", b =>
                {
                    b.HasOne("MeowLib.Domain.DbModels.TranslationEntity.TranslationEntityModel", "Translation")
                        .WithMany("Chapters")
                        .HasForeignKey("TranslationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Translation");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.NotificationEntity.NotificationEntityModel", b =>
                {
                    b.HasOne("MeowLib.Domain.DbModels.UserEntity.UserEntityModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.TeamEntity.TeamEntityModel", b =>
                {
                    b.HasOne("MeowLib.Domain.DbModels.UserEntity.UserEntityModel", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.TeamMemberEntity.TeamMemberEntityModel", b =>
                {
                    b.HasOne("MeowLib.Domain.DbModels.TeamEntity.TeamEntityModel", "Team")
                        .WithMany("Members")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MeowLib.Domain.DbModels.UserEntity.UserEntityModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.TranslationEntity.TranslationEntityModel", b =>
                {
                    b.HasOne("MeowLib.Domain.DbModels.BookEntity.BookEntityModel", "Book")
                        .WithMany("Translations")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MeowLib.Domain.DbModels.TeamEntity.TeamEntityModel", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.UserFavoriteEntity.UserFavoriteEntityModel", b =>
                {
                    b.HasOne("MeowLib.Domain.DbModels.BookEntity.BookEntityModel", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MeowLib.Domain.DbModels.UserEntity.UserEntityModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.BookEntity.BookEntityModel", b =>
                {
                    b.Navigation("Translations");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.TeamEntity.TeamEntityModel", b =>
                {
                    b.Navigation("Members");
                });

            modelBuilder.Entity("MeowLib.Domain.DbModels.TranslationEntity.TranslationEntityModel", b =>
                {
                    b.Navigation("Chapters");
                });
#pragma warning restore 612, 618
        }
    }
}
